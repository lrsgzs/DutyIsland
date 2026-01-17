using CommunityToolkit.Mvvm.ComponentModel;

namespace DutyIsland.Interface.Models.Notification;

/// <summary>
/// 提醒设置
/// </summary>
public partial class NotificationSettings : ObservableRecipient
{
    /// <summary>
    /// 遮罩文本
    /// </summary>
    [ObservableProperty] private string _title = "值日提醒";
    
    /// <summary>
    /// 遮罩时长
    /// </summary>
    [ObservableProperty] private double _titleDuration = 3.0;
    
    /// <summary>
    /// 是否启用遮罩语音
    /// </summary>
    [ObservableProperty] private bool _titleEnableSpeech = true;
    
    /// <summary>
    /// 内容文本
    /// </summary>
    [ObservableProperty] private string _text = "请 %n 进行 %j，谢谢配合。";
    
    /// <summary>
    /// 内容时长
    /// </summary>
    [ObservableProperty] private double _textDuration = 10.0;
    
    /// <summary>
    /// 是否启用内容语音
    /// </summary>
    [ObservableProperty] private bool _textEnableSpeech = true;
}