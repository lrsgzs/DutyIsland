using CommunityToolkit.Mvvm.ComponentModel;

namespace DutyIsland.Models.Notification;

public partial class NotificationSettings : ObservableRecipient
{
    [ObservableProperty] private string _title = "值日提醒";
    [ObservableProperty] private double _titleDuration = 3.0;
    [ObservableProperty] private bool _titleEnableSpeech = true;
    
    [ObservableProperty] private string _text = "请 %n 进行 %j，谢谢配合。";
    [ObservableProperty] private double _textDuration = 10.0;
    [ObservableProperty] private bool _textEnableSpeech = true;
}