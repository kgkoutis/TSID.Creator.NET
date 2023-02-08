namespace TSID.Creator.NET;

public static class DateTimeExtensions
{
    public static long ToUnixTimeMilliseconds(this DateTime @this)
    {
        // The value of DateTime.UnixEpochTicks in C# is 621355968000000000, which represents the number of ticks
        // (100-nanosecond intervals) between the Unix epoch (January 1, 1970, 00:00:00 UTC) and the value of DateTime.MinValue
        // (January 1, 0001, 00:00:00 UTC). This value can be used to convert between DateTime and Unix timestamp representations.
        // so...
        // long UnixEpochMilliseconds = DateTime.UnixEpochTicks / TimeSpan.TicksPerMillisecond; // 62,135,596,800,000
        const long unixEpochMilliseconds = 62135596800000;
        
        // Truncate sub-millisecond precision before offsetting by the Unix Epoch to avoid
        // the last digit being off by one for dates that result in negative Unix times
        var milliseconds = @this.Ticks / TimeSpan.TicksPerMillisecond;
        return milliseconds - unixEpochMilliseconds;
    }
}