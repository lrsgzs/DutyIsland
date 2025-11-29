using Avalonia.Interactivity;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Shared;
using DutyIsland.Models.ComponentSettings;
using DutyIsland.Models.Duty;
using DutyIsland.Services;
using DutyIsland.Shared;

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

    private string WorkersToString(IEnumerable<WorkerItem> workers)
    {
        var text = string.Empty;
        foreach (var workerItem in workers)
        {
            if (text == string.Empty)
            {
                text += workerItem.Name;
            }
            else
            {
                text += $"{Settings.ConnectorString}{workerItem.Name}";
            }
        }

        return text;
    }
    
    private string GetWorkersContent()
    {
        if (DutyPlanService.CurrentDutyPlan is null)
        {
            return "???";
        }
        
        if (DutyPlanService.CurrentDutyPlan.WorkerDictionary.TryGetValue(Settings.JobGuid, out var item))
        {
            return WorkersToString(item.Workers);
        }
        
        if (!Settings.FallbackEnabled)
        {
            return "???";
        }
        
        if (Settings.FallbackJobName != string.Empty)
        {
            var fallbackItems =
                DutyPlanService.CurrentDutyPlan.ComplexItems?.List
                .Where(pair => pair.Value.Second.Name == Settings.FallbackJobName)
                .ToList();
            
            if (fallbackItems != null && fallbackItems.Count != 0)
            {
                return WorkersToString(fallbackItems[0].Value.First.Workers);
            }
        }
        
        if (Settings.FallbackWorkers.Count != 0)
        {
            return WorkersToString(Settings.FallbackWorkers);
        }
        
        return "???";
    }
    
    private void RefreshContent(object? sender, EventArgs e)
    {
        WorkersContentTextBlock.Text = GetWorkersContent();
    }
}