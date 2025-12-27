using ClassIsland.Core.Models.Notification;
using ClassIsland.Shared;
using DutyIsland.Models.Notification;
using DutyIsland.Models.Worker;
using DutyIsland.Services;
using DutyIsland.Shared;

namespace DutyIsland.Extensions;

public static class AutoNotificationInfosExtension
{
    public static List<NotificationRequest> ToRequests(this List<AutoNotificationInfo> infos)
    {
        if (infos.Count < 0)
        {
            return [];
        }

        var dutyPlanService = IAppHost.GetService<DutyPlanService>();
        
        var maskNotificationSettings = infos[0].NotificationSettings;
        List<NotificationRequest> requests = [
            new()
            {
                MaskContent = NotificationContent.CreateTwoIconsMask(
                    maskNotificationSettings.Title, hasRightIcon: true, rightIcon: "\uE31E",
                    factory: x =>
                    {
                        x.Duration = TimeSpanHelper.FromSecondsSafe(maskNotificationSettings.TitleDuration);
                        x.IsSpeechEnabled = maskNotificationSettings.TitleEnableSpeech;
                    })
            }
        ];
        
        requests.AddRange(from info in infos
            let notificationSettings = info.NotificationSettings
            let workersText = dutyPlanService.GetWorkersContent(info.Guid, new FallbackSettings { Enabled = false })
            let text = DutyPlanService.FormatString(info.NotificationSettings.Text, workersText, info.TemplateItem)
            where !string.IsNullOrEmpty(text) && !(notificationSettings.TextDuration <= 0)
            select new NotificationRequest
            {
                MaskContent = NotificationContent.CreateSimpleTextContent(string.Empty, factory: x =>
                {
                    x.Duration = TimeSpan.FromTicks(1);
                    x.IsSpeechEnabled = false;
                }),
                OverlayContent = NotificationContent.CreateSimpleTextContent(text, factory: x =>
                {
                    x.Duration = TimeSpanHelper.FromSecondsSafe(notificationSettings.TextDuration);
                    x.IsSpeechEnabled = notificationSettings.TextEnableSpeech;
                })
            });

        return requests;
    }
}