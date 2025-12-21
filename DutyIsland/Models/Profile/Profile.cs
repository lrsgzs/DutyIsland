using System.Collections.ObjectModel;
using ClassIsland.Shared.ComponentModels;
using CommunityToolkit.Mvvm.ComponentModel;
using DutyIsland.Models.Duty;
using DutyIsland.Models.Rolling;
using DutyIsland.Models.Worker;

namespace DutyIsland.Models.Profile;

public partial class Profile : ObservableObject
{
    [ObservableProperty] private ObservableDictionary<Guid, DutyPlan> _dutyPlans = [];
    [ObservableProperty] private ObservableDictionary<Guid, DutyPlanTemplate> _dutyPlanTemplates = [];
    [ObservableProperty] private ObservableCollection<WorkerItem> _workers = [];
    [ObservableProperty] private RollSettings _rolling = new();
}