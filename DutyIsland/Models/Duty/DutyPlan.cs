using System.Collections.ObjectModel;
using ClassIsland.Shared.ComponentModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DutyIsland.Models.Duty;

public partial class DutyPlan : ObservableObject
{
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private Guid _templateGuid = Guid.Empty;
    [ObservableProperty] private ObservableDictionary<Guid, ObservableCollection<string>> _workerDictionary = new();
}