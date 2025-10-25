using System.Collections.ObjectModel;
using System.ComponentModel;
using ClassIsland.Core.ComponentModels;
using ClassIsland.Shared.ComponentModels;
using CommunityToolkit.Mvvm.ComponentModel;
using DutyIsland.Models;
using DutyIsland.Models.Duty;
using DutyIsland.Shared;

namespace DutyIsland.ViewModels.SettingPages;

public partial class DutyViewModel : ObservableRecipient
{
    public Settings Settings { get; } = GlobalConstants.Config!.Data;
    
    public SyncDictionaryList<Guid, DutyPlan> DutyPlans { get; set; }
    public SyncDictionaryList<Guid, DutyPlanTemplate> DutyPlanTemplates { get; set; }

    [ObservableProperty] private DutyPlan? _selectedDutyPlan = null;

    public DutyViewModel()
    {
        DutyPlans = new SyncDictionaryList<Guid, DutyPlan>(Settings.Profile.DutyPlans, Guid.NewGuid);
        DutyPlanTemplates = new SyncDictionaryList<Guid, DutyPlanTemplate>(Settings.Profile.DutyPlanTemplates, Guid.NewGuid);
    }
}