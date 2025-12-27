using ClassIsland.Core.Abstractions.Services.NotificationProviders;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Models.Notification;
using DutyIsland.Models.Notification;
using DutyIsland.Shared;
using DutyIsland.Shared.Logger;

namespace DutyIsland.Services.NotificationProviders;

[NotificationProviderInfo(GlobalConstants.DutyNotificationProviderGuid, "值日人员提醒", "\uE31E", "提醒值日人员打扫。")]
[NotificationChannelInfo(GlobalConstants.DutyActionNotificationChannelGuid, "值日行动提醒", "\uE314", description:"通过行动发出的提醒。")]
[NotificationChannelInfo(GlobalConstants.DutyAutoNotificationChannelGuid, "值日自动提醒", "\uE31C", description:"通过值日表模板中设置的自动提醒发出的提醒。")]
[NotificationChannelInfo(GlobalConstants.DutyTaskBarNotificationChannelGuid, "值日托盘提醒", "\uE312", description:"通过任务栏托盘中手动触发发出的提醒。")]
public class DutyNotificationProvider : NotificationProviderBase
{
    private Logger<DutyNotificationProvider> Logger { get; } = new();
    private DutyPlanService DutyPlanService { get; }

    public DutyNotificationProvider(DutyPlanService dutyPlanService)
    {
        DutyPlanService = dutyPlanService;
        
        DutyPlanService.OnDutyJobAutoNotificationEvent +=
            (sender, info) => _ = OnAutoNotification(sender, info);
    }
    
    private async Task OnAutoNotification(object? sender, AutoNotificationInfo info)
    {
        await ShowAutoNotification(info.GenerateNotificationRequest());
        info.TemplateItem.IsActivated = false;
    }
    
    public async Task ShowActionNotification(NotificationRequest request)
    {
        await Channel(GlobalConstants.DutyActionNotificationChannelGuid).ShowNotificationAsync(request);
    }
    
    public async Task ShowTaskBarNotification(NotificationRequest request)
    {
        await Channel(GlobalConstants.DutyTaskBarNotificationChannelGuid).ShowNotificationAsync(request);
    }

    public async Task ShowAutoNotification(NotificationRequest request)
    {
        await Channel(GlobalConstants.DutyAutoNotificationChannelGuid).ShowNotificationAsync(request);
    }

    public async Task ShowTaskBarChainedNotification(NotificationRequest[] requests)
    {
        await Channel(GlobalConstants.DutyTaskBarNotificationChannelGuid).ShowChainedNotificationsAsync(requests);
    }

    public async Task TestUnwelcomedChainedNotification()
    {
        Logger.Debug("触发「测试不受欢迎的链式提醒」");
        
        await ShowChainedNotificationsAsync([
            new NotificationRequest
            {
                MaskContent = NotificationContent.CreateTwoIconsMask(
                    "Title", hasRightIcon: true, rightIcon: "\uE31E",
                    factory: x =>
                    {
                        x.Duration = TimeSpanHelper.FromSecondsSafe(3);
                        x.IsSpeechEnabled = false;
                    }),
            },
            new NotificationRequest
            {
                MaskContent = NotificationContent.CreateSimpleTextContent(string.Empty, factory: x =>
                {
                    x.Duration = TimeSpan.FromTicks(1);
                    x.IsSpeechEnabled = false;
                }),
                OverlayContent = NotificationContent.CreateSimpleTextContent(
                    "1",
                    factory: x =>
                    {
                        x.Duration = TimeSpanHelper.FromSecondsSafe(3);
                        x.IsSpeechEnabled = false;
                    })
            },
            new NotificationRequest
            {
                MaskContent = NotificationContent.CreateSimpleTextContent(string.Empty, factory: x =>
                {
                    x.Duration = TimeSpan.FromTicks(1);
                    x.IsSpeechEnabled = false;
                }),
                OverlayContent = NotificationContent.CreateSimpleTextContent(
                    "2",
                    factory: x =>
                    {
                        x.Duration = TimeSpanHelper.FromSecondsSafe(3);
                        x.IsSpeechEnabled = false;
                    })
            },
            new NotificationRequest
            {
                MaskContent = NotificationContent.CreateSimpleTextContent(string.Empty, factory: x =>
                {
                    x.Duration = TimeSpan.FromTicks(1);
                    x.IsSpeechEnabled = false;
                }),
                OverlayContent = NotificationContent.CreateSimpleTextContent(
                    "3",
                    factory: x =>
                    {
                        x.Duration = TimeSpanHelper.FromSecondsSafe(3);
                        x.IsSpeechEnabled = false;
                    })
            }
        ]);
    }
}