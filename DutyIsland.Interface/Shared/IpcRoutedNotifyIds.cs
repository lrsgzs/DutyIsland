namespace DutyIsland.Interface.Shared;

/// <summary>
/// 存储了 DutyIsland 跨进程通信路由通知标识符。
/// </summary>
public static class IpcRoutedNotifyIds
{
    /// <summary>
    /// 值日表变化通知标识符
    /// </summary>
    public const string OnDutyPlanChanged = "dutyIsland.dutyPlanService.onDutyPlanChanged";
    
    /// <summary>
    /// 自动提醒通知标识符
    /// </summary>
    public const string OnDutyJobAutoNotificationEvent = "dutyIsland.dutyPlanService.onDutyJobAutoNotificationEvent";
}