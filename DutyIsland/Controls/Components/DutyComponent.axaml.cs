using Avalonia.Interactivity;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Shared;
using DutyIsland.Models.ComponentSettings;
using DutyIsland.Services;

namespace DutyIsland.Controls.Components;

[ComponentInfo("00318064-DACC-419F-8228-79F3413CAB54", "值日人员(第一代)", "\uE31E", "第一代值日人员显示组件（只显示执行者）。")]
public partial class DutyComponent : ComponentBase<DutyComponentSettings>
{
    private DutyPlanService DutyPlanService { get; } = IAppHost.GetService<DutyPlanService>();

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
        WorkersContentTextBlock.Text = DutyPlanService.GetWorkersContent(
            Settings.JobGuid, Settings.FallbackSettings, Settings.ConnectorString);
    }
}