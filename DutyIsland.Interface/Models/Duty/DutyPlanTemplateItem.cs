using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using DutyIsland.Interface.Models.Notification;

namespace DutyIsland.Interface.Models.Duty;

/// <summary>
/// 值日表模板项
/// </summary>
public partial class DutyPlanTemplateItem : ObservableObject
{
    /// <summary>
    /// 任务名称
    /// </summary>
    [ObservableProperty] private string _name = string.Empty;
    
    /// <summary>
    /// 标准人数
    /// </summary>
    [ObservableProperty] private int _workerCount = 1;
    
    /// <summary>
    /// 提醒设置
    /// </summary>
    [ObservableProperty] private NotificationSettings _notificationSettings = new();
    
    /// <summary>
    /// 自动提醒时间设
    /// </summary>
    [ObservableProperty] private NotificationTimes _notificationTimes = new();
    
    /// <summary>
    /// 是否激活（是否处于自动提醒状态）
    /// </summary>
    private bool _isActivated = false;

    /// <summary>
    /// 是否激活（是否处于自动提醒状态）
    /// </summary>
    [JsonIgnore]
    public bool IsActivated
    {
        get => _isActivated;
        set
        {
            OnPropertyChanging();
            _isActivated = value;
            OnPropertyChanged();
        }
    }

    public override string ToString()
    {
        return $"任务「{Name}」";
    }
}