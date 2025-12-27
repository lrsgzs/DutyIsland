using ClassIsland.Core.Abstractions.Services.NotificationProviders;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Models.Notification;
using DutyIsland.Models.Notification;
using DutyIsland.Models.Worker;
using DutyIsland.Shared;

namespace DutyIsland.Services.NotificationProviders;

[NotificationProviderInfo(GlobalConstants.DutyNotificationProviderGuid, "值日人员提醒", "\uE31E", "提醒值日人员打扫。")]
[NotificationChannelInfo(GlobalConstants.DutyActionNotificationChannelGuid, "值日行动提醒", "\uE314", description:"通过行动发出的提醒。")]
[NotificationChannelInfo(GlobalConstants.DutyAutoNotificationChannelGuid, "值日自动提醒", "\uE31C", description:"通过值日表模板中设置的自动提醒发出的提醒。")]
[NotificationChannelInfo(GlobalConstants.DutyTaskBarNotificationChannelGuid, "值日托盘提醒", "\uE312", description:"通过任务栏托盘中手动触发发出的提醒。")]
public class DutyNotificationProvider : NotificationProviderBase
{
    private DutyPlanService DutyPlanService { get; }

    public DutyNotificationProvider(DutyPlanService dutyPlanService)
    {
        DutyPlanService = dutyPlanService;
        
        DutyPlanService.OnDutyJobAutoNotificationEvent +=
            (sender, info) => _ = OnAutoNotification(sender, info);
    }
    
    public async Task ShowActionNotification(NotificationRequest request)
    {
        await Channel(GlobalConstants.DutyActionNotificationChannelGuid).ShowNotificationAsync(request);
    }

    private async Task OnAutoNotification(object? sender, AutoNotificationInfo info)
    {
        await AutoNotification(info);
    }

    public async Task AutoNotification(AutoNotificationInfo info)
    {
        var notificationSettings = info.NotificationSettings;
        var workersText = DutyPlanService.GetWorkersContent(info.Guid, new FallbackSettings { Enabled = false });
        var text = DutyPlanService.FormatString(info.NotificationSettings.Text, workersText, info.TemplateItem);
        
        var request = new NotificationRequest
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
        
        await Channel(GlobalConstants.DutyAutoNotificationChannelGuid).ShowNotificationAsync(request);
        info.TemplateItem.IsActivated = false;
    }
}