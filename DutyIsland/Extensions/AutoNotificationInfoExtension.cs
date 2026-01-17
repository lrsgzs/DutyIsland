using ClassIsland.Core.Models.Notification;
using ClassIsland.Shared;
using DutyIsland.Interface.Models.Notification;
using DutyIsland.Interface.Models.Worker;
using DutyIsland.Interface.Services;
using DutyIsland.Interface.Shared;

namespace DutyIsland.Extensions;

public static class AutoNotificationInfoExtension
{
    public static List<NotificationRequest> ToRequests(this List<AutoNotificationInfo> infos)
    {
        if (infos.Count < 0)
        {
            return [];
        }

        var dutyPlanService = IAppHost.GetService<IDutyPlanService>();
        
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
            let text = IDutyPlanService.FormatString(info.NotificationSettings.Text, workersText, info.TemplateItem)
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
    
    public static NotificationRequest GenerateNotificationRequest(this AutoNotificationInfo info)
    {
        var dutyPlanService = IAppHost.GetService<IDutyPlanService>();
        var workersText = dutyPlanService.GetWorkersContent(info.Guid, new FallbackSettings { Enabled = false });
        var text = IDutyPlanService.FormatString(info.NotificationSettings.Text, workersText, info.TemplateItem);
        
        return new NotificationRequest
        {
            MaskContent = NotificationContent.CreateTwoIconsMask(
                info.NotificationSettings.Title, hasRightIcon: true, rightIcon: "\uE31E",
                factory: x =>
                {
                    x.Duration = TimeSpanHelper.FromSecondsSafe(info.NotificationSettings.TitleDuration);
                    x.IsSpeechEnabled = info.NotificationSettings.TitleEnableSpeech;
                }),
            OverlayContent = string.IsNullOrEmpty(text) || info.NotificationSettings.TextDuration <= 0
                ? null
                : NotificationContent.CreateSimpleTextContent(text,
                    factory: x =>
                    {
                        x.Duration = TimeSpanHelper.FromSecondsSafe(info.NotificationSettings.TextDuration);
                        x.IsSpeechEnabled = info.NotificationSettings.TextEnableSpeech;
                    })
        };
    }
}