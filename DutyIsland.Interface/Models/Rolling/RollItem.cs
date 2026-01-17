using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using DutyIsland.Interface.Models.Duty;
using DutyIsland.Interface.Services;

namespace DutyIsland.Interface.Models.Rolling;

/// <summary>
/// 轮换项
/// </summary>
public partial class RollItem : ObservableRecipient
{
    /// <summary>
    /// 是否存在值日表
    /// </summary>
    [ObservableProperty] private bool _hasDutyPlan = true;
    private Guid _dutyPlanGuid = Guid.Empty;
    
    /// <summary>
    /// 值日表 Guid
    /// </summary>
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
    
    /// <summary>
    /// 值日表
    /// </summary>
    [JsonIgnore]
    public DutyPlan? DutyPlan => IDutyPlanService.GetService().Profile.DutyPlans.GetValueOrDefault(DutyPlanGuid);

    public override string ToString()
    {
        return (HasDutyPlan ? DutyPlan?.Name : null) ?? "无值日表";
    }
}