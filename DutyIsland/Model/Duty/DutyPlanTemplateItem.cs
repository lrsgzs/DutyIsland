using CommunityToolkit.Mvvm.ComponentModel;

namespace DutyIsland.Model.Duty;

public partial class DutyPlanTemplateItem : ObservableObject
{
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private int _workerCount;
}