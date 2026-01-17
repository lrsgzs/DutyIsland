namespace DutyIsland.Interface.Shared;

/// <summary>
/// TimeSpan 助手
/// </summary>
public static class TimeSpanHelper
{
    /// <summary>
    /// 最大的 TimeSpan 秒数
    /// </summary>
    public const double MaxTimeSpanSeconds = 2147483.0;

    public static TimeSpan FromSecondsSafe(double seconds)
    {
        return !double.IsRealNumber(seconds) ? TimeSpan.Zero : TimeSpan.FromSeconds(Math.Max(0, Math.Min(MaxTimeSpanSeconds, seconds)));
    }
    
    /// <summary>
    /// 比较两个 TimeSpan
    /// </summary>
    /// <param name="ts1">第一个 TimeSpan</param>
    /// <param name="ts2">第二个 TimeSpan</param>
    /// <returns>布尔值，表示两个 TimeSpan 是否相等</returns>
    public static bool IsTimeSpanEqual(TimeSpan ts1, TimeSpan ts2)
    {
        return ts1.Hours == ts2.Hours && ts1.Minutes == ts2.Minutes && ts1.Seconds == ts2.Seconds;
    }
}