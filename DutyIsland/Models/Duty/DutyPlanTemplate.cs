using ClassIsland.Shared.ComponentModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DutyIsland.Model.Duty;

public partial class DutyPlanTemplate : ObservableObject
{
    [ObservableProperty] private Guid _guid;
    [ObservableProperty] private string _templateName = string.Empty;
    
    [ObservableProperty] private ObservableDictionary<Guid, DutyPlanTemplateItem> _workerTemplateDictionary = new();

    public DutyPlanTemplate()
    {
        Guid = Guid.NewGuid();
    }
}