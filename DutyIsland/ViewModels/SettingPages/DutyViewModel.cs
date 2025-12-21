using System.Collections.ObjectModel;
using ClassIsland.Core.ComponentModels;
using ClassIsland.Core.Models.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using DutyIsland.ComponentModels;
using DutyIsland.Models;
using DutyIsland.Models.Duty;
using DutyIsland.Models.Rolling;
using DutyIsland.Models.Worker;
using DutyIsland.Services;
using DutyIsland.Shared;
using DynamicData;
using DynamicData.Alias;
using DynamicData.Binding;

namespace DutyIsland.ViewModels.SettingPages;

public partial class DutyViewModel : ObservableRecipient
{
    public ConfigHandler ConfigHandler { get; } = GlobalConstants.Config!;
    public Settings Settings { get; } = GlobalConstants.Config!.Data;
    public int AppIconClickCount { get; set; } = 0;
    
    public RollSettings Rolling { get; }
    public SyncDictionaryList<Guid, DutyPlan> DutyPlans { get; }
    public SyncDictionaryList<Guid, DutyPlanTemplate> DutyPlanTemplates { get; }
    public ObservableCollection<WorkerItem> Workers { get; }

    // 轮换
    [ObservableProperty] private ObservableCollection<int> _rollIndexItems = [];
    [ObservableProperty] private RollItem? _selectedRollItem = null;
    
    // 值日表
    [ObservableProperty] private DutyPlan? _selectedDutyPlan = null;
    [ObservableProperty] private string _importDutyPlanText = string.Empty;
    [ObservableProperty] private KeyValuePair<Guid, DutyPlanTemplate>? _dutyPlanSelectedDutyPlanTemplateKvp = null;
    [ObservableProperty] private Guid? _selectedDutyPlanItemGuid = null;
    [ObservableProperty] private DutyPlanItem? _selectedDutyPlanItem = null;
    private KeyValuePair<Guid, ObservableValueTuple<DutyPlanItem, DutyPlanTemplateItem>>? _selectedDutyPlanItemKvp = null;
    public KeyValuePair<Guid, ObservableValueTuple<DutyPlanItem, DutyPlanTemplateItem>>? SelectedDutyPlanItemKvp
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
    
    // 值日表模板
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

    // 人员
    [ObservableProperty] private WorkerItem? _selectedWorkerItem = null;

    public DutyViewModel()
    {
        Rolling = Settings.Profile.Rolling;
        DutyPlans = new SyncDictionaryList<Guid, DutyPlan>(Settings.Profile.DutyPlans, Guid.NewGuid);
        DutyPlanTemplates = new SyncDictionaryList<Guid, DutyPlanTemplate>(Settings.Profile.DutyPlanTemplates, Guid.NewGuid);
        Workers = Settings.Profile.Workers;

        Rolling.RollItems.CollectionChanged += (sender, args) =>
        {
            UpdateRollIndexItems();
        };
        
        UpdateRollIndexItems();
    }

    private void UpdateRollIndexItems()
    {
        var indexCount = RollIndexItems.Count;
        var itemsCount = Rolling.RollItems.Count;
        
        if (indexCount == itemsCount)
        {
            return;
        }
        
        RollIndexItems.Clear();
        for (var i = 0; i < itemsCount; i++)
        {
            RollIndexItems.Add(i);
        }
    }
}