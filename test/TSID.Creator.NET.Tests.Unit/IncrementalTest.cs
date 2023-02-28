namespace TSID.Creator.NET.Tests.Unit;

public class IncrementalTest {
    private const int TenSeconds = 10_000;

    private class ClockMock : Clock
    {
        private static readonly DateTimeOffset BeginningOfTime = new DateTimeOffset(DateTime.SpecifyKind(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), DateTimeKind.Utc), TimeSpan.Zero);

        public void IncrementMillis(long millis)
        {
            var prev = FrozenMilliSeconds!.Invoke();
            FrozenMilliSeconds = () => prev + millis;
        }

        public void DecrementMillis(long millis) {
            var prev = FrozenMilliSeconds!.Invoke();
            FrozenMilliSeconds = () => prev - millis;
        }

        public ClockMock() : this(TimeZoneInfo.Utc, BeginningOfTime.ToUnixTimeMilliseconds())
        {
        }

        private ClockMock(TimeZoneInfo timeZoneInfo, long fixedDateTimeOffset) : base(timeZoneInfo, () => fixedDateTimeOffset)
        {
        }
    }

    [Fact]
    public void ShouldGenerateIncrementalValuesInCaseOfBackwardAGlitch() {
        var clock = new ClockMock();

        var factory = TsidFactory.GetBuilder()
                .WithNodeBits(20)
                .WithNode(0)
                .WithClock(clock)
                .Build();

        var prev = factory.Create();

        clock.DecrementMillis(TenSeconds - 1);

        var next = factory.Create();

        AssertIncremental(prev, next);
    }

    [Fact]
    public void ShouldGenerateIncrementalValuesInCaseOfForwardAGlitch() {
        var clock = new ClockMock();

        var factory = TsidFactory.GetBuilder()
                .WithNodeBits(20)
                .WithNode(0)
                .WithClock(clock)
                .Build();

        var prev = factory.Create();

        clock.IncrementMillis(TenSeconds - 1);

        var next = factory.Create();

        AssertIncremental(prev, next);
    }

    [Fact]
    public void ShouldManageAGlitch() {
    	
        var factory = TsidFactory.GetBuilder()
                .WithRandomFunction(() => 0)
                .WithClock(new ClockMock())
                .WithNodeBits(20)
                .WithNode(0)
                .Build();

        const int advanceTimeUpToDriftTolerance = TenSeconds * 4 - 1;

        var last = long.MinValue;
        for (var i=0; i < advanceTimeUpToDriftTolerance; i++) {
            var tsid = factory.Create().ToLong();
            (last < tsid).Should().BeTrue();
            last = tsid;
        }
        
        var prev = factory.Create();
        (last < prev.ToLong()).Should().BeTrue();
        
        var next = factory.Create();
        AssertIncremental(prev, next);
    }

     [Fact]
    public void ShouldAlwaysBeIncremental() {
        var factory = TsidFactory.GetBuilder()
                .WithNodeBits(20)
                .WithNode(0)
                .Build();

        long last = 0;
        for (var i=0; i<1_000_000; i++) {
            var tsid = factory.Create().ToLong();
            (last != 0 && tsid < last).Should().BeFalse();
            last = tsid;
        }
    }

    private static void AssertIncremental(Tsid prev, Tsid next) {
        (prev.ToLong() < next.ToLong()).Should().BeTrue();
    }
}