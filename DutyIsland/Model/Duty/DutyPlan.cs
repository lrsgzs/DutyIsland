using System.Collections.ObjectModel;
using ClassIsland.Shared.ComponentModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DutyIsland.Model.Duty;

public partial class DutyPlan : ObservableObject
{
    [ObservableProperty] private Guid _guid;
    [ObservableProperty] private string _name = string.Empty;
    
    [ObservableProperty] private Guid _templateGuid;
    [ObservableProperty] private ObservableDictionary<Guid, ObservableCollection<string>> _workerDictionary = new();
}