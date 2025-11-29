using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using CommunityToolkit.Mvvm.ComponentModel;
using DutyIsland.Models.AttachedSettings;
using DutyIsland.Models.Duty;
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
}