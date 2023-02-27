namespace TSID.Creator.NET.Tests.Unit;

public class TsidFormatTest
{
    [Fact]
    public void TestFormat()
    {
        var tsid = Tsid.Fast();

        string[][] str =
        {
            //
            new[] { "HEAD", "TAIL" }, //
            new[] { "HEAD", "" }, //
            new[] { "", "TAIL" }, //
            new[] { "", "" } //
        };

        string format;
        string formatted;

        // '%S': upper case
        for (var i = 0; i < str.Length; i++)
        {
            var head = str[i][0];
            var tail = str[i][1];

            // '%S': canonical string in upper case
            format = head + "%S" + tail;
            formatted = head + tsid.ToString() + tail;
            formatted.Should().BeEquivalentTo(tsid.Format(format));

            // '%s': canonical string in lower case
            format = head + "%s" + tail;
            formatted = head + tsid.ToLower() + tail;
            formatted.Should().BeEquivalentTo(tsid.Format(format));

            // '%X': hexadecimal in upper case
            format = head + "%X" + tail;
            formatted = head + Tsid.BaseN.Encode(tsid, 16) + tail;
            formatted.Should().BeEquivalentTo(tsid.Format(format));

            // '%x': hexadecimal in lower case
            format = head + "%x" + tail;
            formatted = head + Tsid.BaseN.Encode(tsid, 16).ToLower() + tail;
            formatted.Should().BeEquivalentTo(tsid.Format(format));

            // '%d': base-10
            format = head + "%d" + tail;
            formatted = head + Tsid.BaseN.Encode(tsid, 10) + tail;
            formatted.Should().BeEquivalentTo(tsid.Format(format));

            // '%z': base-62
            format = head + "%z" + tail;
            formatted = head + Tsid.BaseN.Encode(tsid, 62) + tail;
            formatted.Should().BeEquivalentTo(tsid.Format(format));
        }
    }

    [Fact]
    public void TestUnformat()
    {
        var tsid = Tsid.Fast();

        string[][] str =
        {
            //
            new[] { "HEAD", "TAIL" }, //
            new[] { "HEAD", "" }, //
            new[] { "", "TAIL" }, //
            new[] { "", "" } //
        };

        string format;
        string formatted;

        for (var i = 0; i < str.Length; i++)
        {
            var head = str[i][0];
            var tail = str[i][1];

            // '%S': canonical string in upper case
            format = head + "%S" + tail;
            formatted = head + tsid + tail;
            tsid.Should().BeEquivalentTo(Tsid.Unformat(formatted, format));

            // '%s': canonical string in lower case
            format = head + "%s" + tail;
            formatted = head + tsid.ToLower() + tail;
            tsid.Should().BeEquivalentTo(Tsid.Unformat(formatted, format));

            // '%X': hexadecimal in upper case
            format = head + "%X" + tail;
            var ss = Tsid.BaseN.Encode(tsid, 16);
            formatted = head + ss + tail;
            var res = Tsid.Unformat(formatted, format);
            tsid.Should().BeEquivalentTo(res);

            // '%x': hexadecimal in lower case
            format = head + "%x" + tail;
            formatted = head + Tsid.BaseN.Encode(tsid, 16).ToLower() + tail;
            tsid.Should().BeEquivalentTo(Tsid.Unformat(formatted, format));

            // '%d': base-10
            format = head + "%d" + tail;
            formatted = head + Tsid.BaseN.Encode(tsid, 10) + tail;
            tsid.Should().BeEquivalentTo(Tsid.Unformat(formatted, format));

            // '%z': base-62
            format = head + "%z" + tail;
            formatted = head + Tsid.BaseN.Encode(tsid, 62) + tail;
            tsid.Should().BeEquivalentTo(Tsid.Unformat(formatted, format));
        }
    }

    [Fact]
    public void TestArgumentException()
    {
        var act = () =>
        {
            var str = Tsid.Fast().Format("%z");
            Tsid.Unformat(str, "%z");
        }; 
        act.Should().NotThrow<ArgumentException>();

        act = () => { Tsid.Fast().Format((string)null); };
        act.Should().Throw<ArgumentException>();
        
        act = () => { Tsid.Fast().Format(""); };
        act.Should().Throw<ArgumentException>();
        
        act = () => { Tsid.Fast().Format("%"); };
        act.Should().Throw<ArgumentException>();
        
        act = () => { Tsid.Fast().Format("%a"); };
        act.Should().Throw<ArgumentException>();
        
        act = () => { Tsid.Fast().Format("INVALID"); };
        act.Should().Throw<ArgumentException>();
        
        act = () => { Tsid.Fast().Format("INVALID%"); };
        act.Should().Throw<ArgumentException>();

        act = () => { Tsid.Unformat(null, "%s"); };
        act.Should().Throw<ArgumentException>();

        act = () => { Tsid.Unformat("", null); };
        act.Should().Throw<ArgumentException>();

        act = () => { Tsid.Unformat("", ""); };
        act.Should().Throw<ArgumentException>();

        act = () => { Tsid.Unformat("", "%s"); };
        act.Should().Throw<ArgumentException>();

        act = () => { Tsid.Unformat("INVALID", "%s"); };
        act.Should().Throw<ArgumentException>();

        act = () => { Tsid.Unformat("HEAD" + Tsid.Fast() + "TAIL", "HEAD%STOES"); };
        act.Should().Throw<ArgumentException>();

        act = () => { Tsid.Unformat("HEAD" + Tsid.Fast() + "TAIL", "BANG%STAIL"); };
        act.Should().Throw<ArgumentException>();

        act = () => { Tsid.Unformat("" + Tsid.Fast(), "%a"); };
        act.Should().Throw<ArgumentException>();

        act = () => { Tsid.Unformat("INVALID" + Tsid.Fast(), "INVALID%"); };
        act.Should().Throw<ArgumentException>();

        act = () => { Tsid.Unformat("HEADzzzTAIL", "HEAD%STAIL"); };
        act.Should().Throw<ArgumentException>();

        act = () => { Tsid.Unformat("HEADTAIL", "HEAD%STAIL"); };
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void testFormatAndUnformat()
    {
        var tsid = Tsid.Fast();

        string[][] str =
        {
            //
            new[] { "HEAD", "TAIL" }, //
            new[] { "HEAD", "" }, //
            new[] { "", "TAIL" }, //
            new[] { "", "" } //
        };

        string format;
        string formatted;

        for (var i = 0; i < str.Length; i++)
        {
            var head = str[i][0];
            var tail = str[i][1];

            // '%S': canonical string in upper case
            format = head + "%S" + tail;
            formatted = head + tsid.ToString() + tail;
            formatted.Should().BeEquivalentTo(Tsid.Unformat(formatted, format).Format(format));
            tsid.Should().BeEquivalentTo(Tsid.Unformat(tsid.Format(format), format));

            // '%s': canonical string in lower case
            format = head + "%s" + tail;
            formatted = head + tsid.ToLower() + tail;
            formatted.Should().BeEquivalentTo(Tsid.Unformat(formatted, format).Format(format));
            tsid.Should().BeEquivalentTo(Tsid.Unformat(tsid.Format(format), format));

            // '%X': hexadecimal in upper case
            format = head + "%X" + tail;
            formatted = head + Tsid.BaseN.Encode(tsid, 16) + tail;
            formatted.Should().BeEquivalentTo(Tsid.Unformat(formatted, format).Format(format));
            tsid.Should().BeEquivalentTo(Tsid.Unformat(tsid.Format(format), format));

            // '%x': hexadecimal in lower case
            format = head + "%x" + tail;
            formatted = head + Tsid.BaseN.Encode(tsid, 16).ToLower() + tail;
            formatted.Should().BeEquivalentTo(Tsid.Unformat(formatted, format).Format(format));
            tsid.Should().BeEquivalentTo(Tsid.Unformat(tsid.Format(format), format));

            // '%z': base-62
            format = head + "%z" + tail;
            formatted = head + Tsid.BaseN.Encode(tsid, 62) + tail;
            formatted.Should().BeEquivalentTo(Tsid.Unformat(formatted, format).Format(format));
            tsid.Should().BeEquivalentTo(Tsid.Unformat(tsid.Format(format), format));

            // '%d': base-10
            format = head + "%d" + tail;
            formatted = head + Tsid.BaseN.Encode(tsid, 10) + tail;
            formatted.Should().BeEquivalentTo(Tsid.Unformat(formatted, format).Format(format));
            tsid.Should().BeEquivalentTo(Tsid.Unformat(tsid.Format(format), format));
        }
    }
}