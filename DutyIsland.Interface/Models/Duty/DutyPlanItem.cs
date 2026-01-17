using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DutyIsland.Interface.Models.Duty;

/// <summary>
/// 值日表项
/// </summary>
public partial class DutyPlanItem : ObservableObject
{
    /// <summary>
    /// 执行者列表
    /// </summary>
    /// <seealso cref="DutyWorkerItem"/>
    [ObservableProperty] private ObservableCollection<DutyWorkerItem> _workers = [];
}