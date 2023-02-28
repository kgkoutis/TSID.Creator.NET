namespace TSID.Creator.NET;

public class Clock
{
    public TimeZoneInfo TimeZoneInfo { get; }
    internal Func<long> FrozenMilliSeconds { get; set; } // used only for testing

    public Clock(TimeZoneInfo timeZoneInfo)
    {
        TimeZoneInfo = timeZoneInfo;
    }
    internal Clock(TimeZoneInfo timeZoneInfo, Func<long> fixedDateTimeOffset)
    {
        TimeZoneInfo = timeZoneInfo;
        FrozenMilliSeconds = fixedDateTimeOffset;
    }
}