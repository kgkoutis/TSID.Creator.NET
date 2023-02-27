namespace TSID.Creator.NET.Tests.Integration.demo;

public static class DemoTest 
{
    private const string HorizontalLine = "----------------------------------------";

    private static void PrintList() {
        const int max = 100;

        Console.WriteLine(HorizontalLine);
        Console.WriteLine("### Demo Test: TSID number");
        Console.WriteLine(HorizontalLine);

        for (var i = 0; i < max; i++) {
            Console.WriteLine(TsidCreator.GetTsid1024().ToLong());
        }

        Console.WriteLine(HorizontalLine);
        Console.WriteLine("### TSID string");
        Console.WriteLine(HorizontalLine);

        for (var i = 0; i < max; i++) {
            Console.WriteLine(TsidCreator.GetTsid1024().ToString());
        }
    }

    public static void Init() => PrintList();
}
