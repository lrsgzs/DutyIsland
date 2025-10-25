using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DutyIsland.Model.Duty;

namespace DutyIsland.Model.Profile;

public partial class Profile : ObservableObject
{
    [ObservableProperty] private ObservableCollection<DutyPlan> _dutyPlans = [];
    [ObservableProperty] private ObservableCollection<DutyPlanTemplate> _dutyPlanTemplates = [];
}