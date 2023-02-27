namespace TSID.Creator.NET;

public class Clock
{
    public TimeZoneInfo TimeZoneInfo { get; }
    internal long? FrozenMilliSeconds { get; set; } // used for testing

    public Clock(TimeZoneInfo timeZoneInfo)
    {
        TimeZoneInfo = timeZoneInfo;
    }
    internal Clock(TimeZoneInfo timeZoneInfo, long? fixedDateTimeOffset)
    {
        TimeZoneInfo = timeZoneInfo;
        FrozenMilliSeconds = fixedDateTimeOffset;
    }
}