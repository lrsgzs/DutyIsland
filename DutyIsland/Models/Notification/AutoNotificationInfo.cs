using DutyIsland.Models.Duty;

namespace DutyIsland.Models.Notification;

public record AutoNotificationInfo
{
    public required Guid Guid { get; init; }
    public required DutyPlanItem Item { get; init; }
    public required DutyPlanTemplateItem TemplateItem { get; init; }
    public required NotificationSettings NotificationSettings { get; init; }
}