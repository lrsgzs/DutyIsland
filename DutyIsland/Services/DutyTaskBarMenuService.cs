using Avalonia.Controls;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using ClassIsland.Shared.Models.Automation;
using DutyIsland.Extensions;
using DutyIsland.Models.Notification;
using DutyIsland.Services.NotificationProviders;
using DutyIsland.Shared;
using DutyIsland.Shared.Logger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DutyIsland.Services;

public class DutyTaskBarMenuService
{
    private ITaskBarIconService TaskBarIconService { get; }
    private DutyPlanService DutyPlanService { get; }
    private DutyNotificationProvider DutyNotificationProvider { get; }
    private Logger<DutyTaskBarMenuService> Logger { get; } = new();

    private NativeMenuItem MenuItem { get; } = new("提醒值日人员");
    private NativeMenuItem NotifyAllJobMenuItem { get; } = new("提醒所有值日人员");
    private NativeMenu SubMenu { get; } = [];

    private Dictionary<(Guid, string), int> _state = [];

    public DutyTaskBarMenuService(ITaskBarIconService taskBarIconService, DutyPlanService dutyPlanService)
    {
        TaskBarIconService = taskBarIconService;
        DutyPlanService = dutyPlanService;
        DutyNotificationProvider = IAppHost.Host!.Services.GetServices<IHostedService>().OfType<DutyNotificationProvider>().First();
        
        NotifyAllJobMenuItem.Click += NotifyAllJobMenuItem_OnClick;
        MenuItem.Menu = SubMenu;
        UpdateMenu();

        GlobalConstants.Config!.Data.PropertyChanged += (sender, args) =>
        {
            UpdateMenu();
        };
        
        DutyPlanService.WhenDutyPlanChanged += (sender, args) =>
        {
            UpdateMenu();
        };
        
        DutyPlanService.WhenRefreshDutyPlan += (sender, args) =>
        {
            UpdateMenuItem();
        };
    }

    private void UpdateMenu()
    {
        if (TaskBarIconService.MoreOptionsMenuItems.Contains(MenuItem))
        {
            if (!GlobalConstants.Config!.Data.EnableTaskBarNotificationMenu)
            {
                Logger.Info("移除托盘菜单");
                TaskBarIconService.MoreOptionsMenuItems.Remove(MenuItem);
            }
            
            return;
        }

        if (!GlobalConstants.Config!.Data.EnableTaskBarNotificationMenu)
        {
            return;
        }
        
        Logger.Info("添加托盘菜单");
        TaskBarIconService.MoreOptionsMenuItems.Add(MenuItem);
        UpdateMenuItem();
    }
    
    public void UpdateMenuItem()
    {
        foreach (var kvp in DutyPlanService.CurrentDutyPlan?.Template?.WorkerTemplateDictionary ?? [])
        {
            _state[(kvp.Key, kvp.Value.Name)] = _state.GetValueOrDefault((kvp.Key, kvp.Value.Name), 0) + 1;
        }

        if (_state.All(kvp => kvp.Value == 2))
        {
            goto final;
        }
        
        Logger.Info("刷新托盘菜单内容...");
        SubMenu.Items.Clear();

        if (GlobalConstants.Config!.Data.EnableExperimentFeature)
        {
            SubMenu.Items.Add(NotifyAllJobMenuItem);
            SubMenu.Items.Add(new NativeMenuItemSeparator());
        }
        
        foreach (var kvp in DutyPlanService.CurrentDutyPlan?.Template?.WorkerTemplateDictionary ?? [])
        {
            var menuItem = new NativeMenuItem(kvp.Value.Name);
            menuItem.Click += (sender, args) =>
            {
                Logger.Info($"触发菜单「{kvp.Value.Name}」点击");
                
                var item = DutyPlanService.CurrentDutyPlan?.ComplexItems?.List.First(item => item.Key == kvp.Key);
                if (item == null)
                {
                    return;
                }

                _ = DutyNotificationProvider.ShowTaskBarNotification(new AutoNotificationInfo
                {
                    Guid = item.Value.Key,
                    Item = item.Value.Value.First,
                    TemplateItem = item.Value.Value.Second,
                    NotificationSettings = item.Value.Value.Second.NotificationSettings
                }.GenerateNotificationRequest());
            };

            SubMenu.Items.Add(menuItem);
        }
        
        final:
        _state.Clear();
        foreach (var kvp in DutyPlanService.CurrentDutyPlan?.Template?.WorkerTemplateDictionary ?? [])
        {
            _state[(kvp.Key, kvp.Value.Name)] = 1;
        }
    }

    private async void NotifyAllJobMenuItem_OnClick(object? sender, EventArgs e)
    {
        Logger.Info("「提醒所有值日人员」被点击");
        
        List<AutoNotificationInfo> infos = [];
        infos.AddRange(
            (DutyPlanService.CurrentDutyPlan?.ComplexItems?.List ?? [])
            .Select(kvp =>
                new AutoNotificationInfo
                {
                    Guid = kvp.Key,
                    Item = kvp.Value.First,
                    TemplateItem = kvp.Value.Second,
                    NotificationSettings = kvp.Value.Second.NotificationSettings
                }));
        await DutyNotificationProvider.ShowTaskBarChainedNotification(infos.ToRequests().ToArray());
    }
}