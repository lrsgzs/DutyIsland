using CommunityToolkit.Mvvm.ComponentModel;

namespace DutyIsland.Models.Duty;

public partial class DutyPlanTemplateItem : ObservableObject
{
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private int _workerCount = 1;
    
    [ObservableProperty] private bool _enableNotification = false;
    [ObservableProperty] private TimeSpan _notificationTime = TimeSpan.Zero;
    [ObservableProperty] private string _notificationTitle = "值日提醒";
    [ObservableProperty] private string _notificationText = "该 {names} 搞 {job} 了";
}