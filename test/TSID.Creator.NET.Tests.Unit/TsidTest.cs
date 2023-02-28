using TSID.Creator.NET.Tests.Unit.Extensions;

namespace TSID.Creator.NET.Tests.Unit;

public class TsidTest
{
    private const int TimeBits = 42;
    private const int RandomBits = 22;
    private const int LoopMax = 1_000;

    private static readonly char[] AlphabetCrockford = "0123456789ABCDEFGHJKMNPQRSTVWXYZ".ToCharArray();
    private static readonly char[] AlphabetCsharp = "0123456789abcdefghijklmnopqrstuv".ToCharArray();


    [Fact]
    public void TestFromBytes()
    {
        for (var i = 0; i < LoopMax; i++)
        {
            var number0 = StaticRandom.NextLong();
            var bytes = new byte[8];
            using (var stream = new MemoryStream())
            {
                using (var writer = new EndianBinaryWriter(stream))
                {
                    writer.Write(number0);
                    bytes = stream.ToArray();
                }
            }

            var number1 = Tsid.From(bytes).ToLong();
            number0.Should().Be(number1);
        }
    }

    [Fact]
    public void TestToBytes()
    {
        for (var i = 0; i < LoopMax; i++)
        {
            var number = StaticRandom.NextLong(0, long.MaxValue);
            var bytes = new byte[8];
            using (var stream = new MemoryStream())
            {
                using (var writer = new EndianBinaryWriter(stream))
                {
                    writer.Write(number);
                    bytes = stream.ToArray();
                }
            }

            var string0 = ToString(number);
            var bytes1 = Tsid.From(string0).ToBytes();
            bytes.Should().BeEquivalentTo(bytes1);
        }
    }

    [Fact]
    public void TestFromString()
    {
        for (var i = 0; i < LoopMax; i++)
        {
            var number0 = StaticRandom.NextLong();
            var string0 = ToString(number0);
            var number1 = Tsid.From(string0).ToLong();
            number0.Should().Be(number1);
        }
    }

    [Fact]
    public void TestToString()
    {
        for (var i = 0; i < LoopMax; i++)
        {
            var number = StaticRandom.NextLong();
            var string0 = ToString(number);
            var string1 = Tsid.From(number).ToString();
            string0.Should().Be(string1);
        }
    }

    [Fact]
    public void TestToLowerCase()
    {
        for (var i = 0; i < LoopMax; i++)
        {
            var number = StaticRandom.NextLong();
            var string0 = ToString(number).ToLower();
            var string1 = Tsid.From(number).ToLower();
            string0.Should().Be(string1);
        }
    }

    [Fact]
    public void TestFromString2()
    {
        const long tsid1 = unchecked((long)0xFFFFFFFFFFFFFFFF); // -1
        const string string1 = "FZZZZZZZZZZZZ";
        var result1 = Tsid.From(string1).ToLong();
        tsid1.Should().Be(result1);


        const long tsid2 = 0x000000000000000aL; // 10
        const string string2 = "000000000000A";
        var result2 = Tsid.From(string2).ToLong();
        tsid2.Should().Be(result2);


        const string string3 = "G000000000000";
        Assert.Throws<ArgumentException>(() => { Tsid.From(string3); });
    }

    [Fact]
    public void TestToString2()
    {
        const long tsid1 = unchecked((long)0xFFFFFFFFFFFFFFFF); // -1
        const string string1 = "FZZZZZZZZZZZZ";
        var result1 = Tsid.From(tsid1).ToString();
        string1.Should().Be(result1);

        const long tsid2 = 0x000000000000000aL; // 10
        const string string2 = "000000000000A";
        var result2 = Tsid.From(tsid2).ToString();
        string2.Should().Be(result2);
    }

    [Fact]
    public void TestToLowerCase2()
    {
        const long tsid1 = unchecked((long)0xFFFFFFFFFFFFFFFF); // -1
        var string1 = "FZZZZZZZZZZZZ".ToLower();
        var result1 = Tsid.From(tsid1).ToLower();
        string1.Should().Be(result1);

        const long tsid2 = 0x000000000000000aL; // 10
        var string2 = "000000000000A".ToLower();
        var result2 = Tsid.From(tsid2).ToLower();
        string2.Should().Be(result2);
    }

    [Fact]
    public void TestGetUnixMilliseconds()
    {
        var start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var tsid = TsidCreator.GetTsid1024();
        var middle = tsid.GetUnixMilliseconds();
        var end = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        (start <= middle).Should().BeTrue();
        (middle <= end + 1).Should().BeTrue();
    }

    [Fact]
    public void TestGetUnixMillisecondsMinimum()
    {
        // 2020-01-01T00:00:00.000Z
        var expected = Tsid.TsidEpoch;

        var time1 = 0; // the same as 2^42
        var tsid1 = time1 << TsidTest.RandomBits;
        Tsid.From(tsid1).GetUnixMilliseconds().Should().Be(expected);

        var time2 = (long)Math.Pow(2, 42);
        var tsid2 = time2 << TsidTest.RandomBits;
        Tsid.From(tsid2).GetUnixMilliseconds().Should().Be(expected);
    }

    [Fact]
    public void TestGetUnixMillisecondsMaximum()
    {
        // 2159-05-15T07:35:11.103Z
        var expected = Tsid.TsidEpoch + (long)Math.Pow(2, 42) - 1;

        var time1 = (long)Math.Pow(2, 42) - 1;
        var tsid1 = time1 << TsidTest.RandomBits;
        Tsid.From(tsid1).GetUnixMilliseconds().Should().Be(expected);

        const long time2 = -1; // the same as 2^42-1
        var tsid2 = time2 << TsidTest.RandomBits;
        Tsid.From(tsid2).GetUnixMilliseconds().Should().Be(expected);
    }

    [Fact]
    public void TestGetDateTimeOffset()
    {
        var start = DateTimeOffset.Now;
        var tsid = TsidCreator.GetTsid1024();
        var middle = tsid.GetDateTimeOffset();
        var end = DateTimeOffset.Now;

        (start.ToUnixTimeMilliseconds() <= middle.ToUnixTimeMilliseconds()).Should().BeTrue();
        (middle.ToUnixTimeMilliseconds() <= end.ToUnixTimeMilliseconds()).Should().BeTrue();
    }

    [Fact]
    public void TestGetDateTimeOffsetMinimum()
    {
        var expected = DateTimeOffset.Parse("2020-01-01T00:00:00.000Z");

        long time1 = 0; // the same as 2^42
        var tsid1 = time1 << TsidTest.RandomBits;
        Tsid.From(tsid1).GetDateTimeOffset().Should().Be(expected);

        var time2 = (long)Math.Pow(2, 42);
        var tsid2 = time2 << TsidTest.RandomBits;
        Tsid.From(tsid2).GetDateTimeOffset().Should().Be(expected);
    }

    [Fact]
    public void TestGetDateTimeOffsetMaximum()
    {
        var expected = DateTimeOffset.Parse("2159-05-15T07:35:11.103Z");

        var time1 = (long)Math.Pow(2, 42) - 1;
        var tsid1 = time1 << TsidTest.RandomBits;
        Tsid.From(tsid1).GetDateTimeOffset().Should().Be(expected);

        long time2 = -1; // the same as 2^42-1
        var tsid2 = time2 << TsidTest.RandomBits;
        Tsid.From(tsid2).GetDateTimeOffset().Should().Be(expected);
    }

    [Fact]
    public void TestGetTime()
    {
        for (var i = 0; i < LoopMax; i++)
        {
            var number = StaticRandom.NextLong();
            var tsid = Tsid.From(number);

            var time0 = number >>> RandomBits;
            var time1 = tsid.GetTime();

            time0.Should().Be(time1);
        }
    }

    [Fact]
    public void TestGetRandom()
    {
        for (var i = 0; i < LoopMax; i++)
        {
            var number = StaticRandom.NextLong();
            var tsid = Tsid.From(number);

            var random0 = number << TimeBits >>> TimeBits;
            var random1 = tsid.GetRandom();

            random0.Should().Be(random1);
        }
    }

    [Fact]
    public void TestFastTime()
    {
        for (var i = 0; i < LoopMax; i++)
        {
            var a = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var tsid = Tsid.Fast();
            var b = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var time = tsid.GetUnixMilliseconds();
            (time >= a).Should().BeTrue();
            (time <= b + 1).Should().BeTrue();
        }
    }

    [Fact]
    public void TestFastMonotonicity()
    {
        long prev = 0;
        for (var i = 0; i < LoopMax; i++)
        {
            var tsid = Tsid.Fast();
            // minus to ignore counter rollover
            var next = tsid.ToLong() - LoopMax;
            next.Should().BeGreaterThan(prev);
            prev = next;
        }
    }

    [Fact]
    public void TestIsValid()
    {
        string tsid = null; // Null
        Tsid.IsValid(tsid).Should().BeFalse("Null tsid should be invalid.");

        tsid = ""; // length: 0
        Tsid.IsValid(tsid).Should().BeFalse("tsid with empty string should be invalid.");

        tsid = "0123456789ABC"; // All upper case
        Tsid.IsValid(tsid).Should().BeTrue("tsid in upper case should be valid.");

        tsid = "0123456789abc"; // All lower case
        Tsid.IsValid(tsid).Should().BeTrue("tsid in lower case should be valid.");

        tsid = "0123456789AbC"; // Mixed case
        Tsid.IsValid(tsid).Should().BeTrue("tsid in upper and lower case should be valid.");

        tsid = "0123456789AB"; // length: 12
        Tsid.IsValid(tsid).Should().BeFalse("tsid length lower than 13 should be invalid.");

        tsid = "0123456789ABCC"; // length: 14
        Tsid.IsValid(tsid).Should().BeFalse("tsid length greater than 13 should be invalid.");

        tsid = "0123456789ABi"; // Letter I
        Tsid.IsValid(tsid).Should().BeTrue("tsid with 'i' or 'I' should be valid.");

        tsid = "0123456789ABl"; // Letter L
        Tsid.IsValid(tsid).Should().BeTrue("tsid with 'i' or 'L' should be valid.");

        tsid = "0123456789ABo"; // Letter O
        Tsid.IsValid(tsid).Should().BeTrue("tsid with 'o' or 'O' should be valid.");

        tsid = "0123456789ABu"; // Letter U
        Tsid.IsValid(tsid).Should().BeFalse("tsid with 'u' or 'U' should be invalid.");

        tsid = "0123456789AB#"; // Special char
        Tsid.IsValid(tsid).Should().BeFalse("tsid with special chars should be invalid.");

        Assert.Throws<ArgumentException>(() =>
        {
            tsid = null;
            Tsid.From(tsid);
        });

        for (var i = 0; i < LoopMax; i++)
        {
            var str = TsidCreator.GetTsid1024().ToString();
            Tsid.IsValid(str).Should().BeTrue();
        }
    }

    [Fact]
    public void TestTsidMax256()
    {
        const int maxTsid = 16384;
        const int maxLoop = 20000;

        var list = new Tsid[maxLoop];

        for (var i = 0; i < maxLoop; i++)
        {
            // can generate up to 16384 per msec
            list[i] = TsidCreator.GetTsid256();
        }

        var n = 0;
        long prevTime = 0;
        for (var i = 0; i < maxLoop; i++)
        {
            var time = list[i].GetTime();
            if (time != prevTime)
            {
                n = 0;
            }

            n++;
            prevTime = time;
            (n > maxTsid).Should().BeFalse("Too many TSIDs: " + n);
        }
    }

    [Fact]
    public void TestTsidMax1024()
    {
        const int maxTsid = 4096;
        const int maxLoop = 10000;

        var list = new Tsid[maxLoop];

        for (var i = 0; i < maxLoop; i++)
        {
            // can generate up to 4096 per msec
            list[i] = TsidCreator.GetTsid1024();
        }

        var n = 0;
        long prevTime = 0;
        for (var i = 0; i < maxLoop; i++)
        {
            var time = list[i].GetTime();
            if (time != prevTime)
            {
                n = 0;
            }

            n++;
            prevTime = time;
            (n > maxTsid).Should().BeFalse("Too many TSIDs: " + n);
        }
    }

    [Fact]
    public void TestEquals()
    {
        var bytes = new byte[Tsid.TsidBytes];

        for (var i = 0; i < LoopMax; i++)
        {
            StaticRandom.RandBytes(bytes);
            var tsid1 = Tsid.From(bytes);
            var tsid2 = Tsid.From(bytes);
            tsid1.Should().BeEquivalentTo(tsid2);
            tsid1.ToString().Should().BeEquivalentTo(tsid2.ToString());
            Arrays.ToString(tsid1.ToBytes()).Should().BeEquivalentTo(Arrays.ToString(tsid2.ToBytes()));

            // change all bytes
            for (var j = 0; j < bytes.Length; j++)
            {
                bytes[j]++;
            }

            var tsid3 = Tsid.From(bytes);

            tsid1.Equals(tsid3).Should().BeFalse();
            tsid3.ToString().Equals(tsid1.ToString()).Should().BeFalse();
            Arrays.ToString(tsid1.ToBytes()).Equals(Arrays.ToString(tsid3.ToBytes())).Should().BeFalse();
        }
    }

    [Fact]
    public void TestCompareTo()
    {
        var bytes = new byte[Tsid.TsidBytes];

        for (var i = 0; i < LoopMax; i++)
        {
            StaticRandom.RandBytes(bytes);
            var tsid1 = Tsid.From(bytes);
            var vOut = BitConverter.ToInt64(bytes, 0);
            var number1 = new BigInteger(bytes, true, true);

            StaticRandom.RandBytes(bytes);
            var tsid2 = Tsid.From(bytes);
            var tsid3 = Tsid.From(bytes);
            var number2 = new BigInteger(bytes, true, true);
            var number3 = new BigInteger(bytes, true, true);

            // // compare numerically
            (number1.CompareTo(number2) > 0).Should().Be(tsid1.CompareTo(tsid2) > 0);
            (number1.CompareTo(number2) < 0).Should().Be(tsid1.CompareTo(tsid2) < 0);
            (number2.CompareTo(number3) == 0).Should().Be(tsid2.CompareTo(tsid3) == 0);

            // // compare lexicographically
            (number1.CompareTo(number2) > 0).Should()
                .Be(string.Compare(tsid1.ToString(), tsid2.ToString(), StringComparison.Ordinal) > 0);
            (number1.CompareTo(number2) < 0).Should()
                .Be(string.Compare(tsid1.ToString(), tsid2.ToString(), StringComparison.Ordinal) < 0);
            (number2.CompareTo(number2) == 0).Should()
                .Be(string.Compare(tsid2.ToString(), tsid3.ToString(), StringComparison.Ordinal) == 0);
        }
    }

    [Fact]
    public void TestHashCode()
    {
        // invoked on the same object
        for (var i = 0; i < LoopMax; i++)
        {
            var number = StaticRandom.NextLong();
            var tsid1 = Tsid.From(number);
            tsid1.GetHashCode().Should().Be(tsid1.GetHashCode());
        }

        // invoked on two equal objects
        for (var i = 0; i < LoopMax; i++)
        {
            var number = StaticRandom.NextLong();
            var tsid1 = Tsid.From(number);
            var tsid2 = Tsid.From(number);
            tsid1.GetHashCode().Should().Be(tsid2.GetHashCode());
        }
    }

    [Fact]
    public void TestTsidMax4096()
    {
        const int maxTsid = 1024;
        const int maxLoop = 10000;

        var list = new Tsid[maxLoop];

        for (var i = 0; i < maxLoop; i++)
        {
            // can generate up to 1024 per msec
            list[i] = TsidCreator.GetTsid4096();
        }

        var n = 0;
        long prevTime = 0;
        for (var i = 0; i < maxLoop; i++)
        {
            var time = list[i].GetTime();
            if (time != prevTime)
            {
                n = 0;
            }

            n++;
            prevTime = time;
            (n > maxTsid).Should().BeFalse("Too many TSIDs: " + n);
        }
    }

    private static string ToString(long tsid)
    {
        const string zero = "0000000000000";

        var number = tsid.ToUnsignedStringJavaStyle();
        number = zero.Substring(0, zero.Length - number.Length) + number;

        return Transliterate(number, AlphabetCsharp, AlphabetCrockford);
    }

    private static string Transliterate(string str, char[] alphabet1, char[] alphabet2)
    {
        var output = str.ToCharArray();
        for (var i = 0; i < output.Length; i++)
        {
            for (var j = 0; j < alphabet1.Length; j++)
            {
                if (output[i] == alphabet1[j])
                {
                    output[i] = alphabet2[j];
                    break;
                }
            }
        }

        return new string(output);
    }
}