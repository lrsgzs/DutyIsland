using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DutyIsland.Models.Duty;

public partial class DutyPlanItem : ObservableObject
{
    [ObservableProperty] private ObservableCollection<DutyWorkerItem> _workers = [];
}