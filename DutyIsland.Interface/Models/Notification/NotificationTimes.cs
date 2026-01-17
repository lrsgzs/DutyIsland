using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DutyIsland.Interface.Models.Notification;

/// <summary>
/// 自动提醒时间
/// </summary>
public partial class NotificationTimes : ObservableRecipient
{
    /// <summary>
    /// 是否启用自动提醒
    /// </summary>
    [ObservableProperty] private bool _enable = false;
    
    /// <summary>
    /// 时间列表
    /// </summary>
    [ObservableProperty] private ObservableCollection<NotificationTimeItem> _times = [];
}