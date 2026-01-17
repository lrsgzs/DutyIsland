using CommunityToolkit.Mvvm.ComponentModel;

namespace DutyIsland.Interface.Models.Notification;

/// <summary>
/// 提醒时间项
/// </summary>
public partial class NotificationTimeItem : ObservableRecipient
{
    /// <summary>
    /// 提醒时间
    /// </summary>
    [ObservableProperty] private TimeSpan _time = TimeSpan.Zero;

    public override string ToString()
    {
        return Time.ToString();
    }
}