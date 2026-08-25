using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using DutyIsland.Interface.Models.Worker;
using DutyIsland.Interface.Services;
using DutyIsland.Models.DataSettings;
using DutyIsland.Services.Automations.Blocks;
using DutyIsland.Shared;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using Field = SuperAutoIsland.Interface.Metadata.Field;

namespace DutyIsland.Services.Automations;

public static class AutomationsRegister
{
    public static void RegisterSai()
    {
        var dutyPlanService = IAppHost.GetService<IDutyPlanService>();
        var saiServerService = IAppHost.GetService<ISaiServer>();

        saiServerService.RegisterBlocks("DutyIsland", it =>
        {
            it.AddBlock(new BlockMetadata("duty.actions.notifyDuty")
            {
                Kind = BlockKind.Action,
                Name = "提醒值日人员",
                Icon = ("日历助手", "\uE314"),
                Fields = new Dictionary<string, Field>
                {
                    ["JobGuid"] = BasicFields.DynamicDropdown("", "duty.dd.dutyJobs"),
                    ["FallbackSettings.Enabled"] = BasicFields.CheckBox("启用回滚?"),
                    ["FallbackSettings.JobName"] = BasicFields.Text("回滚任务名称"),

                    ["UseCustomNotificationSettings"] = BasicFields.CheckBox("启用自定义提醒设置?"),
                    ["CustomNotificationSettings.Title"] = BasicFields.Text("遮罩内容", "值日提醒"),
                    ["CustomNotificationSettings.TitleDuration"] = BasicFields.Number("遮罩时长", 3),
                    ["CustomNotificationSettings.TitleEnableSpeech"] = BasicFields.Boolean("启用遮罩语音?", true),
                    ["CustomNotificationSettings.Text"] = BasicFields.Text("正文内容", "请 %n 进行 %j，谢谢配合。"),
                    ["dummy1"] = BasicFields.Dummy("格式化变量说明:"),
                    ["dummy2"] = BasicFields.Dummy("%n - 履行该任务的成员"),
                    ["dummy3"] = BasicFields.Dummy("%j - 该任务名称"),
                    ["CustomNotificationSettings.TextDuration"] = BasicFields.Number("正文时长", 10),
                    ["CustomNotificationSettings.TextEnableSpeech"] = BasicFields.Boolean("启用正文语音?", true)
                }
            });

            if (GlobalConstants.Config!.Data.EnableExperimentFeature)
            {
                it.AddBlock(new BlockMetadata("duty.actions.notifyAllJob")
                {
                    Kind = BlockKind.Action,
                    Name = "提醒所有值日人员",
                    Icon = ("日历助手", "\uE314"),
                    Fields = []
                });
            }

            it.AddBlock<GetWorkersBlock>();
        });

        saiServerService.RegisterDynamicDropdown("duty.dd.dutyJobs", () => Task.FromResult(
            Plugin.EnsureListHasItemOrDefaultListItem(
                dutyPlanService.CurrentDutyPlan?.Template?.WorkerTemplateDictionary
                    .Select(kvp => (kvp.Value.Name, kvp.Key.ToString()))
                    .ToList(),
                new ValueTuple<string, string>("???", GlobalConstants.Guids.NullTemplate.ToString()))));
    }
}