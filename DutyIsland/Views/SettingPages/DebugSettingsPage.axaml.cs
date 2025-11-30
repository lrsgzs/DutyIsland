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
using DutyIsland.Models.AttachedSettings;
using DutyIsland.Shared;
using DutyIsland.ViewModels.SettingPages;
using FluentAvalonia.UI.Controls;

namespace DutyIsland.Views.SettingPages;

[SettingsPageInfo("duty.settings.debug","DutyIsland 调试","\uE2C2","\uE2C1", SettingsPageCategory.Debug)]
public partial class DebugSettingsPage : SettingsPageBase
{
    private ILessonsService LessonsService { get; } = IAppHost.GetService<ILessonsService>();
    
    public DebugSettingsPage()
    {
        InitializeComponent();
    }
    
    private void MenuItemGetAttachedSettings_OnClick(object? sender, RoutedEventArgs e)
    {
        var settings = IAttachedSettingsHostService.GetAttachedSettingsByPriority
            <DutyPlanAttachedSettings>(
                Guid.Parse(GlobalConstants.DutyPlanAttachedSettingsGuid),
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
        
        this.ShowToast(new ToastMessage($"附加设置中当前启用的课表为「{currentDutyPlan.Name}」。")
        {
            Duration = TimeSpan.FromSeconds(10),
            ActionContent = new TextBlock
            {
                Text = settings.DutyPlanGuid.ToString(),
                Foreground = Brushes.Gray
            }
        });
    }
}