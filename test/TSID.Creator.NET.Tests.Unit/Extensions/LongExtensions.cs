namespace TSID.Creator.NET.Tests.Unit.Extensions;

public static class LongExtensions
{
    public static string ToUnsignedStringJavaStyle(this long i)
    {
        const string digits = "0123456789abcdefghijklmnopqrstuvwxyz";
        if (i >= 0)
        {
            var i1 = i;
            var buf = new char[65];
            var charPos = 64;

            i1 = -i1;
            
            while (i1 <= -32)
            {
                buf[charPos--] = digits[(int)-(i1 % 32)];
                i1 /= 32;
            }

            buf[charPos] = digits[(int)-i1];
            
            return new string(buf, charPos, 65 - charPos);
        }

        var chars = Math.Max((64 - NumberOfLeadingZeros(i) + 4) / 5, 1);
        var buf1 = new char[chars];

        var val = i;
        var charPos1 = chars;
        const int mask = 31;
        do
        {
            buf1[0 + --charPos1] = digits[(int)val & mask];
            val >>>= 5;
        } while (val != 0 && charPos1 > 0);

        return new string(buf1);
    }

    private static int NumberOfLeadingZeros(long i)
    {
        var n = 1;
        var x = (int)(i >>> 32);
        if (x == 0) { n += 32; x = (int)i; }
        if (x >>> 16 == 0) { n += 16; x <<= 16; }
        if (x >>> 24 == 0) { n += 8; x <<= 8; }
        if (x >>> 28 == 0) { n += 4; x <<= 4; }
        if (x >>> 30 == 0) { n += 2; x <<= 2; }
        n -= x >>> 31;
        return n;
    }
}