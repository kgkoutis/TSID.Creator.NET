namespace TSID.Creator.NET.Tests.Unit.Extensions;

public static class StaticRandom
{
    private static int _seed = Environment.TickCount;

    private static readonly ThreadLocal<Random> Random = new(() => new Random(Interlocked.Increment(ref _seed)));

    public static int Rand()
    {
        return Random.Value.Next();
    }

    public static void RandBytes(byte[] bytes)
    {
        Random.Value.NextBytes(bytes);
    }
    public static long NextLong(long? min = default, long? max = default)
    {
        return min switch
        {
            null when max == default => Random.Value.NextLong(long.MinValue, long.MaxValue),
            null => Random.Value.NextLong(long.MinValue, long.MaxValue),
            _ => Random.Value.NextLong((long)min, (long)max)
        };
    }
}