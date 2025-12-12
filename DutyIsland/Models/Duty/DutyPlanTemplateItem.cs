using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using DutyIsland.Models.Notification;

namespace DutyIsland.Models.Duty;

public partial class DutyPlanTemplateItem : ObservableObject
{
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private int _workerCount = 1;
    [ObservableProperty] private NotificationSettings _notificationSettings = new();
    [ObservableProperty] private NotificationTimes _notificationTimes = new();
    
    private bool _isActivated = false;

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