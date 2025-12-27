using Avalonia.Controls;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using DutyIsland.Models.Notification;
using DutyIsland.Services.NotificationProviders;
using DutyIsland.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DutyIsland.Services;

public class DutyTaskBarMenuService
{
    private ITaskBarIconService TaskBarIconService { get; }
    private DutyPlanService DutyPlanService { get; }
    private DutyNotificationProvider DutyNotificationProvider { get; }

    private NativeMenuItem MenuItem { get; } = new("提醒值日人员");
    private NativeMenu SubMenu { get; } = [];

    public DutyTaskBarMenuService(ITaskBarIconService taskBarIconService, DutyPlanService dutyPlanService)
    {
        TaskBarIconService = taskBarIconService;
        DutyPlanService = dutyPlanService;
        DutyNotificationProvider = IAppHost.Host!.Services.GetServices<IHostedService>().OfType<DutyNotificationProvider>().First();
        
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
                TaskBarIconService.MoreOptionsMenuItems.Remove(MenuItem);
            }
            
            return;
        }

        if (!GlobalConstants.Config!.Data.EnableTaskBarNotificationMenu)
        {
            return;
        }
        
        TaskBarIconService.MoreOptionsMenuItems.Add(MenuItem);
        UpdateMenuItem();
    }
    
    public void UpdateMenuItem()
    {
        SubMenu.Items.Clear();

        foreach (var kvp in DutyPlanService.CurrentDutyPlan?.Template?.WorkerTemplateDictionary ?? [])
        {
            var menuItem = new NativeMenuItem(kvp.Value.Name);
            menuItem.Click += (sender, args) =>
            {
                var item = DutyPlanService.CurrentDutyPlan?.ComplexItems?.List.First(item => item.Key == kvp.Key);
                if (item == null)
                {
                    return;
                }

                _ = DutyNotificationProvider.AutoNotification(new AutoNotificationInfo
                {
                    Guid = item.Value.Key,
                    Item = item.Value.Value.First,
                    TemplateItem = item.Value.Value.Second,
                    NotificationSettings = item.Value.Value.Second.NotificationSettings
                });
            };

            SubMenu.Items.Add(menuItem);
        }
    }
}