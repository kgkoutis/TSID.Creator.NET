namespace TSID.Creator.NET.Extensions;


public class StaticRandom : Random
{
    public static StaticRandom Instance { get; } = new();
    
    private static int _seed = Environment.TickCount;

    private static readonly ThreadLocal<Random> Random = new(() => new Random(Interlocked.Increment(ref _seed)));

    public int Next()
    {
        return Random.Value.Next();
    }

    public void NextBytes(byte[] bytes)
    {
        Random.Value.NextBytes(bytes);
    }
    public long NextLong(long? min = default, long? max = default)
    {
        return min switch
        {
            null when max == default => Random.Value.NextLong(long.MinValue, long.MaxValue),
            null => Random.Value.NextLong(long.MinValue, long.MaxValue),
            _ => Random.Value.NextLong((long)min, (long)max)
        };
    }
}