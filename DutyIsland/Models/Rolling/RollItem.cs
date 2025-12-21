using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using DutyIsland.Models.Duty;
using DutyIsland.Shared;

namespace DutyIsland.Models.Rolling;

public partial class RollItem : ObservableRecipient
{
    [ObservableProperty] private bool _hasDutyPlan = true;
    private Guid _dutyPlanGuid = Guid.Empty;
    
    public Guid DutyPlanGuid
    {
        get => _dutyPlanGuid;
        set
        {
            _dutyPlanGuid = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(DutyPlan));
        }
    }
    
    [JsonIgnore]
    public DutyPlan? DutyPlan => GlobalConstants.Config!.Data.Profile.DutyPlans.GetValueOrDefault(DutyPlanGuid);

    public override string ToString()
    {
        return (HasDutyPlan ? DutyPlan?.Name : null) ?? "无值日表";
    }
}