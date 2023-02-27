using TSID.Creator.NET.Tests.Integration.demo;
using TSID.Creator.NET.Tests.Integration.uniq;

namespace TSID.Creator.NET.Tests.Integration;

public static class Program
{
    public static void Main(string[] args)
    {
        UniquenessTest.Init();
        DemoTest.Init();
    }
}