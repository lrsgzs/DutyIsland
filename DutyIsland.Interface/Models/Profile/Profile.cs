using System.Collections.ObjectModel;
using ClassIsland.Shared.ComponentModels;
using CommunityToolkit.Mvvm.ComponentModel;
using DutyIsland.Interface.Models.Duty;
using DutyIsland.Interface.Models.Rolling;
using DutyIsland.Interface.Models.Worker;

namespace DutyIsland.Interface.Models.Profile;

/// <summary>
/// 档案
/// </summary>
public partial class Profile : ObservableObject
{
    /// <summary>
    /// 值日表
    /// </summary>
    [ObservableProperty] private ObservableDictionary<Guid, DutyPlan> _dutyPlans = [];
    
    /// <summary>
    /// 值日表模板
    /// </summary>
    [ObservableProperty] private ObservableDictionary<Guid, DutyPlanTemplate> _dutyPlanTemplates = [];
    
    /// <summary>
    /// 执行者列表
    /// </summary>
    [ObservableProperty] private ObservableCollection<WorkerItem> _workers = [];
    
    /// <summary>
    /// 轮换设置
    /// </summary>
    [ObservableProperty] private RollSettings _rolling = new();
}