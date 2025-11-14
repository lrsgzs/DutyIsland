using System.Text.Json.Serialization;
using ClassIsland.Core.ComponentModels;
using ClassIsland.Shared.ComponentModels;
using CommunityToolkit.Mvvm.ComponentModel;
using DutyIsland.Shared;

namespace DutyIsland.Models.Duty;

public partial class DutyPlan : ObservableObject
{
    [ObservableProperty] private string _name = "新值日表";
    private Guid _templateGuid = Guid.Empty;
    [ObservableProperty] private ObservableDictionary<Guid, DutyPlanItem> _workerDictionary = [];

    public Guid TemplateGuid
    {
        get => _templateGuid;
        set
        {
            _templateGuid = value;
            
            OnPropertyChanged();
            OnPropertyChanged(nameof(TemplateItems));
        }
    }

    [JsonIgnore]
    public SyncDictionaryList<Guid, DutyPlanTemplateItem>? TemplateItems
    {   
        get
        {
            if (GlobalConstants.Config!.Data.Profile.DutyPlanTemplates.TryGetValue(TemplateGuid, out var template))
            {
                return new SyncDictionaryList<Guid, DutyPlanTemplateItem>(template.WorkerTemplateDictionary, Guid.NewGuid);
            }

            return null;
        }
    }
}