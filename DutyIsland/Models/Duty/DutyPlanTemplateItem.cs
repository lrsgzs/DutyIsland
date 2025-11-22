using CommunityToolkit.Mvvm.ComponentModel;

namespace DutyIsland.Models.Duty;

public partial class DutyPlanTemplateItem : ObservableObject
{
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private int _workerCount = 1;
    [ObservableProperty] private Notification.NotificationData _notificationData = new();

    public override string ToString()
    {
        return $"任务「{Name}」";
    }
}