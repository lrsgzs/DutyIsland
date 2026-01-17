using CommunityToolkit.Mvvm.ComponentModel;
using DutyIsland.Interface.Models.Notification;
using DutyIsland.Interface.Models.Worker;

namespace DutyIsland.Models.ActionSettings;

public partial class NotifyDutyActionSettings : ObservableRecipient
{
    [ObservableProperty] private Guid _jobGuid;
    [ObservableProperty] private FallbackSettings _fallbackSettings = new();

    [ObservableProperty] private bool _useCustomNotificationSettings = false;
    [ObservableProperty] private NotificationSettings _customNotificationSettings = new();
}