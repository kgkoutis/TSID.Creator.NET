using System.Numerics;

namespace TSID.Creator.NET.Extensions;

public static class ByteArrayExtensions
{
    public static BigInteger ToUnsignedBigEndianBigInteger(this byte[] bytes)
    {
#if NETSTANDARD2_1_OR_GREATER
            return new BigInteger(bytes, true, true);
#elif NETSTANDARD2_0
            return new BigInteger(bytes.Reverse().Concat(new byte[] { 0x00 }).ToArray());
#endif
    }
}