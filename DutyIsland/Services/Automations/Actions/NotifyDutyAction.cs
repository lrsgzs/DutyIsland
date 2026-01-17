using Avalonia.Threading;
using ClassIsland.Core.Abstractions.Automation;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Models.Notification;
using ClassIsland.Shared;
using DutyIsland.Interface.Services;
using DutyIsland.Interface.Shared;
using DutyIsland.Models.ActionSettings;
using DutyIsland.Services.NotificationProviders;
using DutyIsland.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DutyIsland.Services.Automations.Actions;

[ActionInfo("duty.actions.notifyDuty", "提醒值日人员", "\uE314")]
public class NotifyDutyAction : ActionBase<NotifyDutyActionSettings>
{
    private IDutyPlanService DutyPlanService { get; }
    private DutyNotificationProvider DutyNotificationProvider { get; }
    private NotificationRequest? _request;

    public NotifyDutyAction(IDutyPlanService dutyPlanService)
    {
        DutyPlanService = dutyPlanService;
        DutyNotificationProvider = IAppHost.Host!.Services.GetServices<IHostedService>().OfType<DutyNotificationProvider>().First();
    }
    
    protected override async Task OnInvoke()
    {
        await base.OnInvoke();

        if (Settings.JobGuid == GlobalConstants.Guids.NullTemplate)
        {
            return;
        }
        
        if (DutyPlanService.CurrentDutyPlan == null
            || !DutyPlanService.CurrentDutyPlan.Template!.WorkerTemplateDictionary.ContainsKey(Settings.JobGuid)
            || !DutyPlanService.CurrentDutyPlan.WorkerDictionary.ContainsKey(Settings.JobGuid))
        {
            return;
        }
        
        var templateItem = DutyPlanService.GetTemplateItem(Settings.JobGuid, Settings.FallbackSettings);
        if (templateItem == null)
        {
            return;
        }
        
        var notificationSettings = Settings.UseCustomNotificationSettings
            ? Settings.CustomNotificationSettings
            : templateItem.NotificationSettings;
        var workersText = DutyPlanService.GetWorkersContent(Settings.JobGuid, Settings.FallbackSettings);
        var text = IDutyPlanService.FormatString(notificationSettings.Text, workersText, templateItem);

        _request = new NotificationRequest
        {
            MaskContent = NotificationContent.CreateTwoIconsMask(
                notificationSettings.Title, hasRightIcon: true, rightIcon: "\uE31E",
                factory: x =>
                {
                    x.Duration = TimeSpanHelper.FromSecondsSafe(notificationSettings.TitleDuration);
                    x.IsSpeechEnabled = notificationSettings.TitleEnableSpeech;
                }),
            OverlayContent = string.IsNullOrEmpty(text) || notificationSettings.TextDuration <= 0
                ? null
                : NotificationContent.CreateSimpleTextContent(text,
                    factory: x =>
                    {
                        x.Duration = TimeSpanHelper.FromSecondsSafe(notificationSettings.TextDuration);
                        x.IsSpeechEnabled = notificationSettings.TextEnableSpeech;
                    })
        };
        await DutyNotificationProvider.ShowActionNotification(_request);
    }
    
    protected override async Task OnInterrupted()
    {
        await base.OnInterrupted();
        
        _ = Dispatcher.UIThread.InvokeAsync(() =>
        {
            _request?.Cancel();
        });
    }
}