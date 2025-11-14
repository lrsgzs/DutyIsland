using System.Collections.ObjectModel;
using System.ComponentModel;
using ClassIsland.Core.ComponentModels;
using ClassIsland.Core.Models.UI;
using ClassIsland.Shared.ComponentModels;
using CommunityToolkit.Mvvm.ComponentModel;
using DutyIsland.Models;
using DutyIsland.Models.Duty;
using DutyIsland.Services;
using DutyIsland.Shared;

namespace DutyIsland.ViewModels.SettingPages;

public partial class DutyViewModel : ObservableRecipient
{
    public ConfigHandler ConfigHandler { get; } = GlobalConstants.Config!;
    public Settings Settings { get; } = GlobalConstants.Config!.Data;
    public int AppIconClickCount { get; set; } = 0;
    
    public SyncDictionaryList<Guid, DutyPlan> DutyPlans { get; set; }
    public SyncDictionaryList<Guid, DutyPlanTemplate> DutyPlanTemplates { get; set; }

    [ObservableProperty] private DutyPlan? _selectedDutyPlan = null;
    [ObservableProperty] private string _importDutyPlanText = string.Empty;
    [ObservableProperty] private KeyValuePair<Guid, DutyPlanTemplate>? _dutyPlanSelectedDutyPlanTemplateKvp = null;
    [ObservableProperty] private Guid? _selectedDutyPlanItemGuid = null;
    [ObservableProperty] private DutyPlanItem? _selectedDutyPlanItem = null;
    private KeyValuePair<Guid, DutyPlanTemplateItem>? _selectedDutyPlanItemKvp = null;
    public KeyValuePair<Guid, DutyPlanTemplateItem>? SelectedDutyPlanItemKvp
    {
        get => _selectedDutyPlanItemKvp;
        set
        {
            OnPropertyChanging();

            _selectedDutyPlanItemKvp = value;
            if (value != null)
            {
                SelectedDutyPlanItemGuid = value.Value.Key;

                if (SelectedDutyPlan!.WorkerDictionary.TryGetValue(value.Value.Key, out var dutyPlanItem))
                {
                    SelectedDutyPlanItem = dutyPlanItem;
                }
                else
                {
                    var newDutyPlanItem = new DutyPlanItem();
                    SelectedDutyPlanItem = newDutyPlanItem;
                    SelectedDutyPlan.WorkerDictionary[value.Value.Key] = newDutyPlanItem;
                }
            }
            else
            {
                SelectedDutyPlanItemGuid = null;
                SelectedDutyPlanItem = null;
            }
            
            OnPropertyChanged();
        }
    }
    
    [ObservableProperty] private DutyPlanTemplate? _selectedDutyPlanTemplate = null;
    [ObservableProperty] private ToastMessage? _currentTemplateItemDeleteRevertToast;
    [ObservableProperty] private Guid? _selectedDutyPlanTemplateItem = null;
    private KeyValuePair<Guid, DutyPlanTemplateItem>? _selectedDutyPlanTemplateItemKvp = null;
    public KeyValuePair<Guid, DutyPlanTemplateItem>? SelectedDutyPlanTemplateItemKvp
    {
        get => _selectedDutyPlanTemplateItemKvp;
        set
        {
            OnPropertyChanging();

            _selectedDutyPlanTemplateItemKvp = value;
            if (value != null)
            {
                SelectedDutyPlanTemplateItem = value.Value.Key;
            }
            else
            {
                SelectedDutyPlanTemplateItem = null;
            }
            
            OnPropertyChanged();
        }
    }


    public DutyViewModel()
    {
        DutyPlans = new SyncDictionaryList<Guid, DutyPlan>(Settings.Profile.DutyPlans, Guid.NewGuid);
        DutyPlanTemplates = new SyncDictionaryList<Guid, DutyPlanTemplate>(Settings.Profile.DutyPlanTemplates, Guid.NewGuid);
    }
}