using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using CommunityToolkit.Mvvm.ComponentModel;
using DutyIsland.Models.AttachedSettings;
using DutyIsland.Models.Duty;
using DutyIsland.Shared;

namespace DutyIsland.Services;

public partial class DutyPlanService : ObservableRecipient
{
    [ObservableProperty] private DutyPlan? _currentDutyPlan = null;
    public event EventHandler? WhenRefreshDutyPlan;
    
    private ILessonsService LessonsService { get; } = IAppHost.GetService<ILessonsService>();
    private byte _ticks = 0;
    
    public DutyPlanService()
    {
        LessonsService.PostMainTimerTicked += LessonsServiceOnPostMainTimerTicked;
    }

    private void LessonsServiceOnPostMainTimerTicked(object? sender, EventArgs e)
    {
        _ticks += 1;
        if (_ticks >= 10)
        {
            _ticks = 0;
            RefreshDutyPlan();
        }
    }

    public void RefreshDutyPlan()
    {
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
        
        WhenRefreshDutyPlan?.Invoke(this, EventArgs.Empty);
    }
}