using System.Numerics;

namespace TSID.Creator.NET.Extensions;

public static class BigIntegerExtensions
{
    public static long ToLongValueJavaStyle(this BigInteger bigInteger)
    {
        long longValue = (long) (ulong) (bigInteger & ulong.MaxValue);
        return longValue;
    }
    
    public static int ToIntValueJavaStyle(this BigInteger bigInteger)
    {
        int intValue  = (int) (uint) (bigInteger & uint.MaxValue);
        return intValue;
    }
}