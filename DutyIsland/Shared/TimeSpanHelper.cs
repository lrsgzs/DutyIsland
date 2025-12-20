namespace DutyIsland.Shared;

public static class TimeSpanHelper
{
    public const double MaxTimeSpanSeconds = 2147483.0;

    public static TimeSpan FromSecondsSafe(double seconds)
    {
        return !double.IsRealNumber(seconds) ? TimeSpan.Zero : TimeSpan.FromSeconds(Math.Max(0, Math.Min(MaxTimeSpanSeconds, seconds)));
    }
    
    public static bool IsTimeSpanEqual(TimeSpan ts1, TimeSpan ts2)
    {
        return ts1.Hours == ts2.Hours && ts1.Minutes == ts2.Minutes && ts1.Seconds == ts2.Seconds;
    }
}