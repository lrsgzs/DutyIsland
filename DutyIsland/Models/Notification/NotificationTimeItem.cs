using CommunityToolkit.Mvvm.ComponentModel;

namespace DutyIsland.Models.Notification;

public partial class NotificationTimeItem : ObservableRecipient
{
    [ObservableProperty] private TimeSpan _notificationTime = TimeSpan.Zero;

    public override string ToString()
    {
        return NotificationTime.ToString();
    }
}