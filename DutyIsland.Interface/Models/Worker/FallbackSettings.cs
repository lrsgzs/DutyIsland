using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DutyIsland.Interface.Models.Duty;

namespace DutyIsland.Interface.Models.Worker;

/// <summary>
/// 回滚设置
/// </summary>
public partial class FallbackSettings : ObservableRecipient
{
    /// <summary>
    /// 是否启用回滚
    /// </summary>
    [ObservableProperty] private bool _enabled = false;
    
    /// <summary>
    /// 回滚任务名称
    /// </summary>
    [ObservableProperty] private string _jobName = string.Empty;
    
    /// <summary>
    /// 回滚执行者列表
    /// </summary>
    [ObservableProperty] private ObservableCollection<DutyWorkerItem> _workers = [];
}