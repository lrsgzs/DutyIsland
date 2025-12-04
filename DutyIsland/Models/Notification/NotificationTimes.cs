using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DutyIsland.Models.Notification;

public partial class NotificationTimes : ObservableRecipient
{
    [ObservableProperty] private bool _enable = false;
    [ObservableProperty] private ObservableCollection<NotificationTimeItem> _times = [];
}