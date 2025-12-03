using ClassIsland.Core.Abstractions.Automation;
using ClassIsland.Core.Attributes;
using DutyIsland.Models.ActionSettings;

namespace DutyIsland.Services.Automations.Actions;

[ActionInfo("duty.actions.notifyDuty", "提醒值日人员", "\uE314")]
public class NotifyDutyAction : ActionBase<NotifyDutyActionSettings>
{
    protected override async Task OnInvoke()
    {
        await base.OnInvoke();

    }
}