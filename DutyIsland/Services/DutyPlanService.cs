using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using CommunityToolkit.Mvvm.ComponentModel;
using DutyIsland.Models.AttachedSettings;
using DutyIsland.Models.Duty;
using DutyIsland.Models.Worker;
using DutyIsland.Shared;
using DutyIsland.Shared.Logger;

namespace DutyIsland.Services;

public partial class DutyPlanService : ObservableRecipient
{
    [ObservableProperty] private DutyPlan? _currentDutyPlan = null;
    public event EventHandler? WhenRefreshDutyPlan;
    public event EventHandler? WhenDutyPlanChanged;
    
    private ILessonsService LessonsService { get; } = IAppHost.GetService<ILessonsService>();
    private Logger<DutyPlanService> Logger { get;} = new();
    private byte _ticks = 0;
    
    public DutyPlanService()
    {
        LessonsService.PostMainTimerTicked += LessonsServiceOnPostMainTimerTicked;

        WhenDutyPlanChanged += (sender, args) =>
        {
            Logger.Info($"值日表变化为「{CurrentDutyPlan?.Name}」");
        };
        
        RefreshDutyPlan();
    }

    private void LessonsServiceOnPostMainTimerTicked(object? sender, EventArgs e)
    {
        _ticks += 1;
        if (_ticks >= 4)
        {
            _ticks = 0;
            RefreshDutyPlan();
        }
    }

    public void RefreshDutyPlan()
    {
        var beforeDutyPlan = CurrentDutyPlan;
        var attachedSettings = IAttachedSettingsHostService.GetAttachedSettingsByPriority
            <DutyPlanAttachedSettings>(
                Guid.Parse(GlobalConstants.DutyPlanAttachedSettingsGuid),
                classPlan: LessonsService.CurrentClassPlan);
        
        if (attachedSettings?.DutyPlanGuid != null &&
            GlobalConstants.Config!.Data.Profile.DutyPlans.TryGetValue(attachedSettings.DutyPlanGuid.Value, out var plan))
        {
            CurrentDutyPlan = plan;
        }
        else
        {
            CurrentDutyPlan = null;
        }
        
        if (CurrentDutyPlan != beforeDutyPlan)
        {
            WhenDutyPlanChanged?.Invoke(this, EventArgs.Empty);
        }
        
        WhenRefreshDutyPlan?.Invoke(this, EventArgs.Empty);
    }
    
    
    public static string WorkersToString(IEnumerable<DutyWorkerItem> workers, string connectorString)
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
                text += $"{connectorString}{workerItem.Name}";
            }
        }

        return text;
    }

    public DutyPlanTemplateItem? GetTemplateItem(Guid jobGuid, FallbackSettings fallbackSettings)
    {
        if (CurrentDutyPlan is null)
        {
            return null;
        }
        
        if (CurrentDutyPlan.Template!.WorkerTemplateDictionary.TryGetValue(jobGuid, out var item))
        {
            return item;
        }
        
        if (!fallbackSettings.Enabled)
        {
            return null;
        }
        
        if (fallbackSettings.JobName != string.Empty)
        {
            var fallbackItems =
                CurrentDutyPlan.ComplexItems?.List
                    .Where(pair => pair.Value.Second.Name == fallbackSettings.JobName)
                    .ToList();
            
            if (fallbackItems != null && fallbackItems.Count != 0)
            {
                return fallbackItems[0].Value.Second;
            }
        }
        
        if (fallbackSettings.Workers.Count != 0)
        {
            return new DutyPlanTemplateItem
            {
                Name = fallbackSettings.JobName,
                WorkerCount = fallbackSettings.Workers.Count
            };
        }

        return null;
    }
    
    public string GetWorkersContent(Guid jobGuid, FallbackSettings fallbackSettings, string connectorString = "、")
    {
        if (CurrentDutyPlan is null)
        {
            return "???";
        }
        
        if (CurrentDutyPlan.WorkerDictionary.TryGetValue(jobGuid, out var item))
        {
            return WorkersToString(item.Workers, connectorString);
        }
        
        if (!fallbackSettings.Enabled)
        {
            return "???";
        }
        
        if (fallbackSettings.JobName != string.Empty)
        {
            var fallbackItems =
                CurrentDutyPlan.ComplexItems?.List
                    .Where(pair => pair.Value.Second.Name == fallbackSettings.JobName)
                    .ToList();
            
            if (fallbackItems != null && fallbackItems.Count != 0)
            {
                return WorkersToString(fallbackItems[0].Value.First.Workers, connectorString);
            }
        }
        
        if (fallbackSettings.Workers.Count != 0)
        {
            return WorkersToString(fallbackSettings.Workers, connectorString);
        }
        
        return "???";
    }
}