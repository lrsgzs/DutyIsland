using CommunityToolkit.Mvvm.ComponentModel;

namespace DutyIsland.Models;

public partial class NotificationData : ObservableObject
{
    [ObservableProperty] private bool _enableNotification = false;
    [ObservableProperty] private TimeSpan _notificationTime = TimeSpan.Zero;
    
    [ObservableProperty] private string _notificationTitle = "值日提醒";
    [ObservableProperty] private double _notificationTitleDuration = 3.0;
    
    [ObservableProperty] private string _notificationText = "该 %n 搞 %j 了";
    [ObservableProperty] private double _notificationTextDuration = 10.0;
}