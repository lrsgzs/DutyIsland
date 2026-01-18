using ClassIsland.Core.Abstractions.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using dotnetCampus.Ipc.CompilerServices.GeneratedProxies;
using DutyIsland.Enums;
using DutyIsland.Interface.Models.Duty;
using DutyIsland.Interface.Models.Notification;
using DutyIsland.Interface.Models.Profile;
using DutyIsland.Interface.Models.Worker;
using DutyIsland.Interface.Services;
using DutyIsland.Interface.Shared;
using DutyIsland.Models.AttachedSettings;
using DutyIsland.Shared;
using Microsoft.Extensions.Logging;

namespace DutyIsland.Services;

public class DutyPlanService : ObservableRecipient, IDutyPlanService
{
    public DutyPlan? CurrentDutyPlan
    {
        get => _currentDutyPlan;
        private set
        {
            _currentDutyPlan = value;
            OnPropertyChanged();
        }
    }
    
    public Profile Profile { get; } = GlobalConstants.Config!.Data.Profile;
    
    public event EventHandler? WhenRefreshDutyPlan;
    public event EventHandler? WhenDutyPlanChanged;
    public event EventHandler<AutoNotificationInfo>? OnDutyJobAutoNotificationEvent;
    
    private ILessonsService LessonsService { get; }
    private IExactTimeService ExactTimeService { get; }
    private IIpcService IpcService { get; }
    private ILogger<DutyPlanService> Logger { get; }
    private DutyPlan? _currentDutyPlan = null;
    private byte _ticks = 0;
    
    public DutyPlanService(ILogger<DutyPlanService> logger, ILessonsService lessonService, IExactTimeService exactTimeService, IIpcService ipcService)
    {
        Logger = logger;
        LessonsService = lessonService;
        ExactTimeService = exactTimeService;
        IpcService = ipcService;
        
        LessonsService.PostMainTimerTicked += LessonsServiceOnPostMainTimerTicked;
        IpcService.IpcProvider.CreateIpcJoint<IPublicDutyPlanService>(this);
        
        WhenDutyPlanChanged += async (sender, args) =>
        {
            Logger.LogInformation("值日表变化为「{Name}」", CurrentDutyPlan?.Name);
            await IpcService.BroadcastNotificationAsync(IpcRoutedNotifyIds.OnDutyPlanChanged);
        };

        OnDutyJobAutoNotificationEvent += async (sender, info) =>
        {
            Logger.LogInformation("触发「{Name}」自动提醒", info.TemplateItem.Name);
            await IpcService.BroadcastNotificationAsync(IpcRoutedNotifyIds.OnDutyJobAutoNotificationEvent, info);
        };
        
        RefreshDutyPlan();

        if (GlobalConstants.Config!.Data.DutyPlanGetMode == DutyPlanGetMode.AutoRolling)
        {
            UpdateRollingIndex(DateOnly.FromDateTime(DateTime.Now));
        }
        else
        {
            GlobalConstants.Config.Data.Profile.Rolling.LastChangedDate = DateOnly.FromDateTime(DateTime.Now);
        }
    }

    public void UpdateRollingIndex(DateOnly currentDate)
    {
        var settings = Profile.Rolling;
        if (settings.RollItems.Count == 0)
        {
            return;
        }
        
        var dayDelta = currentDate.DayNumber - settings.LastChangedDate.DayNumber;
        var changes = 0;

        if (dayDelta <= 0)
        {
            return;
        }
        
        if (settings.RollOnUnopenDay)
        {
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
        }
        else
        {
            if (dayDelta > settings.RollDays && (!settings.SkipWeekend || currentDate.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday)))
            {
                changes = 1;
            }
        }
        
        Logger.LogDebug("轮换 - {Before} ==> {After}", settings.LastChangedDate, currentDate);
        Logger.LogDebug("轮换 - 差异为 {DayDelta} 天，应偏移 {Changes} 次。", dayDelta, changes);

        if (changes == 0) return;
        
        settings.LastChangedDate = currentDate;
        settings.RollIndex += changes;
        if (settings.RollIndex >= settings.RollItems.Count)
        {
            settings.RollIndex %= settings.RollItems.Count;
        }
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
                Guid.Parse(GlobalConstants.Guids.DutyPlanAttachedSettings),
                classPlan: LessonsService.CurrentClassPlan)?
            .DutyPlanGuid;
    }

    public Guid? GetDutyPlanGuidByRollingSettings()
    {
        var settings = Profile.Rolling;
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

        var now = GlobalConstants.Config.Data.TimeSource switch
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
            return IDutyPlanService.WorkersToString(item.Workers, connectorString);
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
                return IDutyPlanService.WorkersToString(fallbackItems[0].Value.First.Workers, connectorString);
            }
        }
        
        if (fallbackSettings.Workers.Count != 0)
        {
            return IDutyPlanService.WorkersToString(fallbackSettings.Workers, connectorString);
        }
        
        return "???";
    }
}