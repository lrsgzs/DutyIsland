using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DutyIsland.Models.Notification;

public partial class NotificationData : ObservableRecipient
{
    [ObservableProperty] private bool _enableNotification = false;
    [ObservableProperty] private ObservableCollection<NotificationTimeItem> _notificationTimes = [];
    
    [ObservableProperty] private string _notificationTitle = "值日提醒";
    [ObservableProperty] private double _notificationTitleDuration = 3.0;
    
    [ObservableProperty] private string _notificationText = "该 %n 搞 %j 了";
    [ObservableProperty] private double _notificationTextDuration = 10.0;
}