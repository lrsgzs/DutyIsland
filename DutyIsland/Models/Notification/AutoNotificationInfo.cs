using ClassIsland.Core.Models.Notification;
using ClassIsland.Shared;
using DutyIsland.Models.Duty;
using DutyIsland.Models.Worker;
using DutyIsland.Services;
using DutyIsland.Shared;

namespace DutyIsland.Models.Notification;

public record AutoNotificationInfo
{
    public required Guid Guid { get; init; }
    public required DutyPlanItem Item { get; init; }
    public required DutyPlanTemplateItem TemplateItem { get; init; }
    public required NotificationSettings NotificationSettings { get; init; }

    public NotificationRequest GenerateNotificationRequest()
    {
        var dutyPlanService = IAppHost.GetService<DutyPlanService>();
        var workersText = dutyPlanService.GetWorkersContent(Guid, new FallbackSettings { Enabled = false });
        var text = DutyPlanService.FormatString(NotificationSettings.Text, workersText, TemplateItem);
        
        return new NotificationRequest
        {
            MaskContent = NotificationContent.CreateTwoIconsMask(
                NotificationSettings.Title, hasRightIcon: true, rightIcon: "\uE31E",
                factory: x =>
                {
                    x.Duration = TimeSpanHelper.FromSecondsSafe(NotificationSettings.TitleDuration);
                    x.IsSpeechEnabled = NotificationSettings.TitleEnableSpeech;
                }),
            OverlayContent = string.IsNullOrEmpty(text) || NotificationSettings.TextDuration <= 0
                ? null
                : NotificationContent.CreateSimpleTextContent(text,
                    factory: x =>
                    {
                        x.Duration = TimeSpanHelper.FromSecondsSafe(NotificationSettings.TextDuration);
                        x.IsSpeechEnabled = NotificationSettings.TextEnableSpeech;
                    })
        };
    }
}