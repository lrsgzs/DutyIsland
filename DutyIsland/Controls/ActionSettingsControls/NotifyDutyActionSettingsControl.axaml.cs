using Avalonia.Controls;
using Avalonia.Interactivity;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Shared;
using DutyIsland.Interface.Models.Duty;
using DutyIsland.Interface.Services;
using DutyIsland.Models.ActionSettings;

namespace DutyIsland.Controls.ActionSettingsControls;

public partial class NotifyDutyActionSettingsControl : ActionSettingsControlBase<NotifyDutyActionSettings>
{
    private IDutyPlanService DutyPlanService { get; } = IAppHost.GetService<IDutyPlanService>();
    
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
    
    protected override bool IsUndoDeleteRequested() => true;
}