using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.ComponentModels;
using ClassIsland.Core.Enums;
using DutyIsland.Models;
using DutyIsland.Models.AttachedSettings;
using DutyIsland.Models.Duty;
using DutyIsland.Shared;

namespace DutyIsland.Controls.AttachedSettingsControls;

[AttachedSettingsUsage(AttachedSettingsTargets.ClassPlan)]
[AttachedSettingsControlInfo(GlobalConstants.DutyPlanAttachedSettingsGuid, "值日表设置", "\uE31E")]
public partial class DutyPlanAttachedSettingsControl : AttachedSettingsControlBase<DutyPlanAttachedSettings>
{
    private SyncDictionaryList<Guid, DutyPlan> DutyPlans { get; set; }
    private Settings DutyIslandSettings { get; } = GlobalConstants.Config!.Data;
    
    public DutyPlanAttachedSettingsControl()
    {
        DutyPlans = new SyncDictionaryList<Guid, DutyPlan>(DutyIslandSettings.Profile.DutyPlans, Guid.NewGuid);
        
        InitializeComponent();
    }
}