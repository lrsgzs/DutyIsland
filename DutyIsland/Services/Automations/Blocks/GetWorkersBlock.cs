using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using DutyIsland.Interface.Models.Worker;
using DutyIsland.Interface.Services;
using DutyIsland.Models.DataSettings;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;

namespace DutyIsland.Services.Automations.Blocks;

public class GetWorkersBlock : DataBlockBase
{
    public override string Id => "duty.data.getWorkers";
    public override string Name => "获取任务成员";
    public override (string, string) Icon => ("人们", FluentIcons.PeopleRegular);
    public override Type SettingsType => typeof(GetWorkersDataSettings);

    public override void GetFields(FieldsRegister it) => it
        .AddField("JobGuid", BasicFields.DynamicDropdown("", "duty.dd.dutyJobs"));

    public override Task<object> Handler(object? data)
    {
        if (data is not GetWorkersDataSettings settings || !Guid.TryParse(settings.JobGuid, out var jobGuid))
            return Task.FromResult<object>("???");
        
        var dutyPlanService = IAppHost.GetService<IDutyPlanService>();
        return Task.FromResult<object>(dutyPlanService.GetWorkersContent(jobGuid, new FallbackSettings()));
    }
}