using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.ComponentModels;
using ClassIsland.Core.Enums;
using DutyIsland.Interface.Models.Duty;
using DutyIsland.Models;
using DutyIsland.Models.AttachedSettings;
using DutyIsland.Shared;

namespace DutyIsland.Controls.AttachedSettingsControls;

[AttachedSettingsUsage(AttachedSettingsTargets.ClassPlan)]
[AttachedSettingsControlInfo(GlobalConstants.Guids.DutyPlanAttachedSettings, "值日表设置", "\uE31E")]
public partial class DutyPlanAttachedSettingsControl : AttachedSettingsControlBase<DutyPlanAttachedSettings>
{
    private SyncDictionaryList<Guid, DutyPlan> DutyPlans { get; set; }
    private Settings DutyIslandSettings { get; } = GlobalConstants.Config!.Data;
    
    public DutyPlanAttachedSettingsControl()
    {
        DutyPlans = new SyncDictionaryList<Guid, DutyPlan>(
            DutyIslandSettings.Profile.DutyPlans
                .Select(item => KeyValuePair.Create(item.Key, item.Value))
                .ToDictionary(),
            Guid.NewGuid);
        
        InitializeComponent();
    }
}