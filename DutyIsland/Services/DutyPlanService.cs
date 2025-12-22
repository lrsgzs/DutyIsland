using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using CommunityToolkit.Mvvm.ComponentModel;
using DutyIsland.Enums;
using DutyIsland.Models.AttachedSettings;
using DutyIsland.Models.Duty;
using DutyIsland.Models.Notification;
using DutyIsland.Models.Worker;
using DutyIsland.Shared;
using DutyIsland.Shared.Logger;

namespace DutyIsland.Services;

public partial class DutyPlanService : ObservableRecipient
{
    [ObservableProperty] private DutyPlan? _currentDutyPlan = null;
    
    public event EventHandler? WhenRefreshDutyPlan;
    public event EventHandler? WhenDutyPlanChanged;
    public event EventHandler<AutoNotificationInfo>? OnDutyJobAutoNotificationEvent;
    
    private ILessonsService LessonsService { get; }
    private IExactTimeService ExactTimeService { get; }
    private Logger<DutyPlanService> Logger { get;} = new();
    private byte _ticks = 0;
    
    public DutyPlanService(ILessonsService lessonService, IExactTimeService exactTimeService)
    {
        LessonsService = lessonService;
        ExactTimeService = exactTimeService;
        
        LessonsService.PostMainTimerTicked += LessonsServiceOnPostMainTimerTicked;

        WhenDutyPlanChanged += (sender, args) =>
        {
            Logger.Info($"值日表变化为「{CurrentDutyPlan?.Name}」");
        };

        OnDutyJobAutoNotificationEvent += (sender, info) =>
        {
            Logger.Info($"触发「{info.TemplateItem.Name}」自动提醒");
        };
        
        RefreshDutyPlan();

        if (GlobalConstants.Config!.Data.DutyPlanGetMode == DutyPlanGetMode.AutoRolling)
        {
            UpdateRollingIndex();
        }
        else
        {
            GlobalConstants.Config.Data.Profile.Rolling.LastChangedDate = DateOnly.FromDateTime(DateTime.Now);
        }
    }

    public void UpdateRollingIndex()
    {
        var settings = GlobalConstants.Config!.Data.Profile.Rolling;
        if (settings.RollItems.Count == 0)
        {
            return;
        }
        
        var dayDelta = DateOnly.FromDateTime(DateTime.Now).DayNumber - settings.LastChangedDate.DayNumber;
        
        if (settings.RollOnUnopenDay)
        {
            var changes = 0;
            var curGroup = 0;
            var curDay = settings.LastChangedDate.ToDateTime(new TimeOnly());

            for (var i = 0; i < dayDelta; i++)
            {
                curDay = curDay.AddDays(1);
                
                if (settings.SkipWeekend && curDay.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                {
                    continue;
                }
                curGroup++;
                
                if (curGroup >= settings.RollDays)
                {
                    changes++;
                    curGroup = 0;
                }
            }

            settings.RollIndex += changes;
        }
        else
        {
            if (!settings.SkipWeekend || DateTime.Now.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday))
            {
                settings.RollIndex++;
            }
        }
        
        if (settings.RollIndex >= settings.RollItems.Count)
        {
            settings.RollIndex %= settings.RollItems.Count;
        }
        settings.LastChangedDate = DateOnly.FromDateTime(DateTime.Now);
    }
    
    private void LessonsServiceOnPostMainTimerTicked(object? sender, EventArgs e)
    {
        _ticks += 1;
        if (_ticks >= 4)
        {
            _ticks = 0;
            RefreshDutyPlan();
            CheckDutyJobNotificationTime();
        }
    }

    public void RefreshDutyPlan()
    {
        var beforeDutyPlan = CurrentDutyPlan;
        Guid? afterDutyPlanGuid;

        switch (GlobalConstants.Config!.Data.DutyPlanGetMode)
        {
            case DutyPlanGetMode.AttachedSettings:
                afterDutyPlanGuid = GetDutyPlanGuidByAttachedSettings();
                break;
            case DutyPlanGetMode.AutoRolling:
                afterDutyPlanGuid = GetDutyPlanGuidByRollingSettings();
                break;
            default:
                return;
        }
        
        if (afterDutyPlanGuid != null &&
            GlobalConstants.Config.Data.Profile.DutyPlans.TryGetValue(afterDutyPlanGuid.Value, out var plan))
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

    public Guid? GetDutyPlanGuidByAttachedSettings()
    {
        return IAttachedSettingsHostService.GetAttachedSettingsByPriority
            <DutyPlanAttachedSettings>(
                Guid.Parse(GlobalConstants.DutyPlanAttachedSettingsGuid),
                classPlan: LessonsService.CurrentClassPlan)?
            .DutyPlanGuid;
    }

    public static Guid? GetDutyPlanGuidByRollingSettings()
    {
        var settings = GlobalConstants.Config!.Data.Profile.Rolling;
        if (settings.RollItems.Count == 0)
        {
            return null;
        }
        
        if (settings.RollIndex >= settings.RollItems.Count)
        {
            settings.RollIndex %= settings.RollItems.Count;
        }
        
        return settings.RollItems[settings.RollIndex].DutyPlanGuid;
    }
    
    public void CheckDutyJobNotificationTime()
    {
        if (!GlobalConstants.Config!.Data.GlobalEnableNotification)
        {
            return;
        }

        if (CurrentDutyPlan?.Template == null)
        {
            return;
        }

        var now = GlobalConstants.Config!.Data.TimeSource switch
        {
            TimeSource.ClassIsland => ExactTimeService.GetCurrentLocalDateTime().TimeOfDay,
            TimeSource.System => DateTime.Now.TimeOfDay,
            _ => TimeSpan.Zero
        };
        
        var notifications = CurrentDutyPlan.Template.WorkerTemplateDictionary
            .Where(kvp => kvp.Value.NotificationTimes.Enable
                          && !kvp.Value.IsActivated
                          && kvp.Value.NotificationTimes.Times.Any(item => TimeSpanHelper.IsTimeSpanEqual(item.Time, now))
                          && CurrentDutyPlan.WorkerDictionary.ContainsKey(kvp.Key))
            .Select(kvp =>
            {
                kvp.Value.IsActivated = true;
                
                return new AutoNotificationInfo
                {
                    Guid = kvp.Key,
                    Item = CurrentDutyPlan.WorkerDictionary[kvp.Key],
                    TemplateItem = kvp.Value,
                    NotificationSettings = kvp.Value.NotificationSettings
                };
            });

        foreach (var notification in notifications)
        {
            OnDutyJobAutoNotificationEvent?.Invoke(this, notification);
        }
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

    public static string FormatString(string text, string workersText, DutyPlanTemplateItem? dutyPlanTemplateItem)
    {
        return text
            .Replace("%j", dutyPlanTemplateItem?.Name ?? "???")
            .Replace("%n", workersText);
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