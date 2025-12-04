using CommunityToolkit.Mvvm.ComponentModel;

namespace DutyIsland.Models.Notification;

public partial class NotificationSettings : ObservableRecipient
{
    [ObservableProperty] private string _title = "值日提醒";
    [ObservableProperty] private double _titleDuration = 3.0;
    
    [ObservableProperty] private string _text = "该 %n 搞 %j 了";
    [ObservableProperty] private double _textDuration = 10.0;
}