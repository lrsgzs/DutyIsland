using Avalonia.Interactivity;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Shared;
using DutyIsland.Interface.Services;
using DutyIsland.Models.ComponentSettings;

namespace DutyIsland.Controls.Components;

[ComponentInfo("00318064-DACC-419F-8228-79F3413CAB54", "值日人员", "\uE31E", "值日人员显示组件。")]
public partial class DutyComponent : ComponentBase<DutyComponentSettings>
{
    private IDutyPlanService DutyPlanService { get; } = IAppHost.GetService<IDutyPlanService>();

    public DutyComponent()
    {
        InitializeComponent();
    }

    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        DutyPlanService.WhenRefreshDutyPlan += RefreshContent;
        Settings.PropertyChanged += RefreshContent;
        RefreshContent(sender, e);
    }

    private void Control_OnUnloaded(object? sender, RoutedEventArgs e)
    {
        DutyPlanService.WhenRefreshDutyPlan -= RefreshContent;
        Settings.PropertyChanged -= RefreshContent;
    }
    
    private void RefreshContent(object? sender, EventArgs e)
    {
        var templateItem = DutyPlanService.GetTemplateItem(Settings.JobGuid, Settings.FallbackSettings);
        var workersText =
            DutyPlanService.GetWorkersContent(Settings.JobGuid, Settings.FallbackSettings, Settings.ConnectorString);
        WorkersContentTextBlock.Text = IDutyPlanService.FormatString(Settings.FormatString, workersText, templateItem);
    }
}