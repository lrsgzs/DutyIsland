using DutyIsland.Interface.Models.Duty;

namespace DutyIsland.Interface.Models.Notification;

/// <summary>
/// 自动提醒记录
/// </summary>
public record AutoNotificationInfo
{
    /// <summary>
    /// 任务 Guid
    /// </summary>
    public required Guid Guid { get; init; }
    
    /// <summary>
    /// 值日表项
    /// </summary>
    public required DutyPlanItem Item { get; init; }
    
    /// <summary>
    /// 值日表模板项
    /// </summary>
    public required DutyPlanTemplateItem TemplateItem { get; init; }
    
    /// <summary>
    /// 提醒设置
    /// </summary>
    public required NotificationSettings NotificationSettings { get; init; }
}