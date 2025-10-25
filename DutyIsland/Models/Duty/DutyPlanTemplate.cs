using ClassIsland.Shared.ComponentModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DutyIsland.Models.Duty;

public partial class DutyPlanTemplate : ObservableObject
{
    [ObservableProperty] private string _templateName = string.Empty;
    [ObservableProperty] private ObservableDictionary<Guid, DutyPlanTemplateItem> _workerTemplateDictionary = new();
}