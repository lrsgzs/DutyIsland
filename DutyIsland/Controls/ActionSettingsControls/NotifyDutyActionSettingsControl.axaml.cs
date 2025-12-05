using Avalonia.Controls;
using Avalonia.Interactivity;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Helpers.UI;
using ClassIsland.Core.Models.UI;
using ClassIsland.Shared;
using CommunityToolkit.Mvvm.Input;
using DutyIsland.Models.ActionSettings;
using DutyIsland.Models.Duty;
using DutyIsland.Services;

namespace DutyIsland.Controls.ActionSettingsControls;

public partial class NotifyDutyActionSettingsControl : ActionSettingsControlBase<NotifyDutyActionSettings>
{
    private DutyPlanService DutyPlanService { get; } = IAppHost.GetService<DutyPlanService>();
    
    public NotifyDutyActionSettingsControl()
    {
        InitializeComponent();
    }

    private void ButtonShowSettings_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.FindResource("NotificationSettingsDrawer") is not ContentControl cc) return;
        cc.DataContext = this;
        _ = ShowDrawer(cc);
    }
    
    private void UseCurrentValueButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var templateItem = DutyPlanService.CurrentDutyPlan?.Template?.WorkerTemplateDictionary.GetValueOrDefault(Settings.JobGuid, new DutyPlanTemplateItem
        {
            Name = "???"
        });
        Settings.FallbackSettings.JobName = templateItem?.Name ?? "???";
    }
}