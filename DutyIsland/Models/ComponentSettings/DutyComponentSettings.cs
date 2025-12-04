using CommunityToolkit.Mvvm.ComponentModel;
using DutyIsland.Models.Worker;

namespace DutyIsland.Models.ComponentSettings;

public partial class DutyComponentSettings : ObservableRecipient
{
    [ObservableProperty] private Guid _jobGuid;
    [ObservableProperty] private string _connectorString = " ";
    [ObservableProperty] private FallbackSettings _fallbackSettings = new();
}