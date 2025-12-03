using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DutyIsland.Models.Duty;

namespace DutyIsland.Models.ActionSettings;

public partial class NotifyDutyActionSettings : ObservableRecipient
{
    [ObservableProperty] private Guid _jobGuid;
    
    [ObservableProperty] private bool _fallbackEnabled = true;
    [ObservableProperty] private string _fallbackJobName = string.Empty;
    [ObservableProperty] private ObservableCollection<WorkerItem> _fallbackWorkers = [];

    [ObservableProperty] private bool _useCustomNotificationSettings = true;
    [ObservableProperty] private string _notificationTitle = "值日提醒";
    [ObservableProperty] private double _notificationTitleDuration = 3.0;
    [ObservableProperty] private string _notificationText = "该 %n 搞 %j 了";
    [ObservableProperty] private double _notificationTextDuration = 10.0;
}