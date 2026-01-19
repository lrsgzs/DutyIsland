using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Enums.SettingsWindow;
using ClassIsland.Core.Helpers.UI;
using ClassIsland.Core.Models.UI;
using ClassIsland.Shared;
using DutyIsland.Interface.Services;
using DutyIsland.Models.AttachedSettings;
using DutyIsland.Services.NotificationProviders;
using DutyIsland.Shared;
using FluentAvalonia.UI.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DutyIsland.Views.SettingPages;

[SettingsPageInfo("duty.debug","DutyIsland 调试","\uE2C2","\uE2C1", SettingsPageCategory.Debug)]
public partial class DebugSettingsPage : SettingsPageBase
{
    private ILessonsService LessonsService { get; } = IAppHost.GetService<ILessonsService>();
    private IDutyPlanService DutyPlanService { get; } = IAppHost.GetService<IDutyPlanService>();
    private DutyNotificationProvider DutyNotificationProvider { get; } = IAppHost.Host!.Services
        .GetServices<IHostedService>().OfType<DutyNotificationProvider>().First();
    
    public DebugSettingsPage()
    {
        InitializeComponent();
    }
    
    private void ExpanderItemGetAttachedSettings_OnClick(object? sender, RoutedEventArgs e)
    {
        var settings = IAttachedSettingsHostService.GetAttachedSettingsByPriority
            <DutyPlanAttachedSettings>(
                Guid.Parse(GlobalConstants.Guids.DutyPlanAttachedSettings),
                classPlan: LessonsService.CurrentClassPlan);
        
        if (settings?.DutyPlanGuid == null)
        {
            this.ShowToast(new ToastMessage("未获取到附加设置。")
            {
                Severity = InfoBarSeverity.Warning,
                Duration = TimeSpan.FromSeconds(10)
            });
            
            return;
        }

        var currentDutyPlan = GlobalConstants.Config!.Data.Profile.DutyPlans[settings.DutyPlanGuid.Value];
        
        this.ShowToast(new ToastMessage($"课表附加设置中获取到的值日表为「{currentDutyPlan.Name}」。")
        {
            Duration = TimeSpan.FromSeconds(10),
            ActionContent = new TextBlock
            {
                Text = settings.DutyPlanGuid.ToString(),
                Foreground = Brushes.Gray
            }
        });
    }

    private void ExpanderItemGetActiveDutyPlan_OnClick(object? sender, RoutedEventArgs e)
    {
        var dutyPlan = DutyPlanService.CurrentDutyPlan;
        
        if (dutyPlan == null)
        {
            this.ShowToast(new ToastMessage("未获取到启用的值日表。")
            {
                Severity = InfoBarSeverity.Warning,
                Duration = TimeSpan.FromSeconds(10)
            });
            
            return;
        }
        
        this.ShowToast(new ToastMessage($"当前启用的值日表为「{dutyPlan.Name}」。")
        {
            Duration = TimeSpan.FromSeconds(10)
        });
    }

    private void ExpanderItemTestUnwelcomedChainedNotification_OnClick(object? sender, RoutedEventArgs e)
    {
        _ = DutyNotificationProvider.TestUnwelcomedChainedNotification();
    }
}