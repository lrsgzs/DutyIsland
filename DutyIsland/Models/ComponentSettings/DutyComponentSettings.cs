using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DutyIsland.Models.Duty;

namespace DutyIsland.Models.ComponentSettings;

public partial class DutyComponentSettings : ObservableRecipient
{
    [ObservableProperty] private Guid _jobGuid;
    [ObservableProperty] private bool _fallbackEnabled = true;
    [ObservableProperty] private string _fallbackJobName = string.Empty;
    [ObservableProperty] private ObservableCollection<WorkerItem> _fallbackWorkers = [];
    [ObservableProperty] private string _connectorString = " ";
}