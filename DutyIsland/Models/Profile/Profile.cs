using ClassIsland.Shared.ComponentModels;
using CommunityToolkit.Mvvm.ComponentModel;
using DutyIsland.Models.Duty;

namespace DutyIsland.Models.Profile;

public partial class Profile : ObservableObject
{
    [ObservableProperty] private ObservableDictionary<Guid, DutyPlan> _dutyPlans = [];
    [ObservableProperty] private ObservableDictionary<Guid, DutyPlanTemplate> _dutyPlanTemplates = [];
}