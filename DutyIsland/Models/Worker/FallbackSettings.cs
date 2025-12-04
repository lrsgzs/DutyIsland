using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DutyIsland.Models.Duty;

namespace DutyIsland.Models.Worker;

public partial class FallbackSettings : ObservableRecipient
{
    [ObservableProperty] private bool _enabled = false;
    [ObservableProperty] private string _jobName = string.Empty;
    [ObservableProperty] private ObservableCollection<DutyWorkerItem> _workers = [];
}