using ClassIsland.Core.Controls;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using DutyIsland.Interface.Models.Worker;
using DutyIsland.Interface.Services;
using DutyIsland.Models.DataSettings;
using DutyIsland.Shared;
using SuperAutoIsland.Interface;
using SuperAutoIsland.Interface.MetaData;
using SuperAutoIsland.Interface.MetaData.ArgsType;
using SuperAutoIsland.Interface.Services;

namespace DutyIsland.Services.Automations;

public static class AutomationsRegister
{
    public static void RegisterSai()
    {
        var dutyPlanService = IAppHost.GetService<IDutyPlanService>();
        var saiServerService = IAppHost.GetService<ISaiServer>();

        saiServerService.RegisterBlocks("DutyIsland", new RegisterData
        {
            Actions =
            [
                new BlockMetadata
                {
                    Id = "duty.actions.notifyDuty",
                    Name = "提醒值日人员",
                    Icon = ("日历助手", "\uE314"),
                    Args = new Dictionary<string, MetaArgsBase>
                    {
                        ["JobGuid"] = new DynamicDropdownMetaArgs
                        {
                            Name = "",
                            Type = MetaType.dynamic_dropdown,
                            Id = "duty.dd.dutyJobs"
                        },
                        ["FallbackSettings.Enabled"] = new CheckboxMetaArgs
                        {
                            Name = "启用回滚?",
                            Type = MetaType.checkbox,
                            DefaultValue = false
                        },
                        ["FallbackSettings.JobName"] = new CommonMetaArgs
                        {
                            Name = "回滚任务名称",
                            Type = MetaType.text
                        },
                        ["UseCustomNotificationSettings"] = new CheckboxMetaArgs
                        {
                            Name = "启用自定义提醒设置?",
                            Type = MetaType.checkbox,
                            DefaultValue = false
                        },
                        ["CustomNotificationSettings.Title"] = new CommonMetaArgs
                        {
                            Name = "遮罩内容",
                            Type = MetaType.text
                        },
                        ["CustomNotificationSettings.TitleDuration"] = new CommonMetaArgs
                        {
                            Name = "遮罩时长",
                            Type = MetaType.number
                        },
                        ["CustomNotificationSettings.TitleEnableSpeech"] = new CommonMetaArgs
                        {
                            Name = "启用遮罩语音?",
                            Type = MetaType.boolean
                        },
                        ["CustomNotificationSettings.Text"] = new CommonMetaArgs
                        {
                            Name = "正文内容",
                            Type = MetaType.text
                        },
                        ["_string0"] = new CommonMetaArgs
                        {
                            Name = "说明:",
                            Type = MetaType.dummy
                        },
                        ["_string1"] = new CommonMetaArgs
                        {
                            Name = "%n - 履行该任务的成员",
                            Type = MetaType.dummy
                        },
                        ["_string2"] = new CommonMetaArgs
                        {
                            Name = "%j - 该任务名称",
                            Type = MetaType.dummy
                        },
                        ["CustomNotificationSettings.TextDuration"] = new CommonMetaArgs
                        {
                            Name = "正文时长",
                            Type = MetaType.number
                        },
                        ["CustomNotificationSettings.TextEnableSpeech"] = new CommonMetaArgs
                        {
                            Name = "启用正文语音?",
                            Type = MetaType.boolean
                        }
                    },
                    DropdownUseNumbers = false,
                    InlineField = false,
                    InlineBlock = false
                },
                .. GlobalConstants.Config!.Data.EnableExperimentFeature
                    ?
                    [
                        new BlockMetadata
                        {
                            Id = "duty.actions.notifyAllJob",
                            Name = "提醒所有值日人员",
                            Icon = ("日历助手", "\uE314"),
                            Args = new Dictionary<string, MetaArgsBase>(),
                            DropdownUseNumbers = false,
                            InlineField = false,
                            InlineBlock = false
                        }
                    ]
                    : Array.Empty<BlockMetadata>()
            ],
            Rules = [],
            Data =
            [
                new BlockMetadata
                {
                    Id = "duty.data.getWorkers",
                    Name = "获取任务成员",
                    Icon = ("人们", FluentIcons.PeopleRegular),
                    Args = new Dictionary<string, MetaArgsBase>
                    {
                        ["JobGuid"] = new DynamicDropdownMetaArgs
                        {
                            Name = "",
                            Type = MetaType.dynamic_dropdown,
                            Id = "duty.dd.dutyJobs"
                        },
                    },
                    DropdownUseNumbers = false,
                    InlineField = false,
                    InlineBlock = false
                }
            ]
        });

        saiServerService.RegisterDynamicDropdown("duty.dd.dutyJobs", () => Task.FromResult(
            Plugin.EnsureListHasItemOrDefaultListItem(
                dutyPlanService.CurrentDutyPlan?.Template?.WorkerTemplateDictionary
                    .Select(kvp => (kvp.Value.Name, kvp.Key.ToString()))
                    .ToList(),
                new ValueTuple<string, string>("???", GlobalConstants.Guids.NullTemplate.ToString()))));
        
        saiServerService.RegisterDataGetter<GetWorkersDataSettings>("duty.data.getWorkers", data =>
        {
            if (data is not GetWorkersDataSettings settings || !Guid.TryParse(settings.JobGuid, out var jobGuid))
                return Task.FromResult("???");
            return Task.FromResult(dutyPlanService.GetWorkersContent(jobGuid, new FallbackSettings()));
        });
    }
}