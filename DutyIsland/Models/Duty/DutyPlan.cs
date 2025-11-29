using System.Text.Json.Serialization;
using ClassIsland.Core.ComponentModels;
using ClassIsland.Shared.ComponentModels;
using CommunityToolkit.Mvvm.ComponentModel;
using DutyIsland.ComponentModels;
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
            OnPropertyChanged(nameof(ComplexItems));
        }
    }

    [JsonIgnore]
    public DutyPlanTemplate? Template => GlobalConstants.Config!.Data.Profile.DutyPlanTemplates.GetValueOrDefault(TemplateGuid);

    [JsonIgnore]
    public SyncDictionaryList<Guid, ObservableValueTuple<DutyPlanItem, DutyPlanTemplateItem>>? ComplexItems
    {   
        get
        {
            if (GlobalConstants.Config!.Data.Profile.DutyPlanTemplates.TryGetValue(TemplateGuid, out var template))
            {
                return new SyncDictionaryList<Guid, ObservableValueTuple<DutyPlanItem, DutyPlanTemplateItem>>(
                    template.WorkerTemplateDictionary
                        .Select(e =>
                        {
                            var item = WorkerDictionary.GetValueOrDefault(e.Key, new DutyPlanItem());
                            return KeyValuePair.Create(e.Key, new ObservableValueTuple<DutyPlanItem, DutyPlanTemplateItem>(item, e.Value));
                        })
                        .ToDictionary(),
                    Guid.NewGuid);
            }

            return null;
        }
    }
}