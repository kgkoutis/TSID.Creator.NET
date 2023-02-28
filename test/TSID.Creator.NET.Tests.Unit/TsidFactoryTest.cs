using TSID.Creator.NET.Extensions;
using TSID.Creator.NET.Tests.Unit.Extensions;

namespace TSID.Creator.NET.Tests.Unit;

public class TsidFactoryTest : IDisposable {
	private const int LoopMax = 1_000;
	private const int IntegerBytes = 4;

	public TsidFactoryTest()
	{
		// clear properties
		Environment.SetEnvironmentVariable(TsidFactory.Settings.Node, "");
		Environment.SetEnvironmentVariable(TsidFactory.Settings.NodeCount, "");	
	}

	public void Dispose()
	{
		// clear properties
		Environment.SetEnvironmentVariable(TsidFactory.Settings.Node, "");
		Environment.SetEnvironmentVariable(TsidFactory.Settings.NodeCount, "");
	}

	[Fact]
	public void TestGetInstant()
	{
		var start = DateTimeOffset.UtcNow;
		var tsid = TsidCreator.GetTsid();
		var middle = tsid.GetDateTimeOffset();
		var end = DateTimeOffset.UtcNow;

		start.ToUnixTimeMilliseconds().Should().BeLessOrEqualTo(middle.ToUnixTimeMilliseconds());
		middle.ToUnixTimeMilliseconds().Should().BeLessOrEqualTo(end.ToUnixTimeMilliseconds() + 1);
	}

	[Fact]
	public void TestGetUnixMilliseconds() {

		var start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		var tsid = new TsidFactory().Create();
		var middle = tsid.GetUnixMilliseconds();
		var end = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

		start.Should().BeLessOrEqualTo(middle);
		middle.Should().BeLessOrEqualTo(end + 1);
	}

	[Fact]
	public void TestGetInstantWithClock() {

		var bound = (long) Math.Pow(2, 42);

		for (var i = 0; i < LoopMax; i++) {

			// instantiate a factory with a Clock that returns a fixed value
			var random = StaticRandom.Instance.NextLong(min: 0, max: bound);
			var fixedRandomNumber = random + Tsid.TsidEpoch; // avoid dates before 2020
			long Millis() => fixedRandomNumber;
			var clock = new Clock(TimeZoneInfo.Utc, Millis); // simulate a frozen clock  
			var randomFunction = (int x) => new byte[x]; // force to reinitialize the counter to ZERO
			var factory = TsidFactory.GetBuilder().WithClock(clock).WithRandomFunction(randomFunction).Build();

			var result = factory.Create().GetDateTimeOffset().ToUnixTimeMilliseconds();
			Millis().Should().Be(result); // "The current instant is incorrect"
		}
	}

	[Fact]
	public void TestGetUnixMillisecondsWithClock() {

		var bound = (long) Math.Pow(2, 42);

		for (var i = 0; i < LoopMax; i++) {

			// instantiate a factory with a Clock that returns a fixed value
			var random = StaticRandom.Instance.NextLong(min: 0, max: bound);
			var fixedRandomNumber = random + Tsid.TsidEpoch; // avoid dates before 2020
			long Millis() => fixedRandomNumber;
			var clock = new Clock(TimeZoneInfo.Utc, Millis); // simulate a frozen clock 
			var randomFunction = (int x) => new byte[x]; // force to reinitialize the counter to ZERO
			var factory = TsidFactory.GetBuilder().WithClock(clock).WithRandomFunction(randomFunction).Build();

			var result = factory.Create().GetUnixMilliseconds();
			Millis().Should().Be(result); // "The current instant is incorrect"
		}
	}

	[Fact]
	public void TestGetInstantWithCustomEpoch()
	{
		var customEpoch = DateTimeOffset.Parse("2015-10-23T00:00:00Z");

		var start = DateTimeOffset.Now;
		var tsid = TsidFactory.GetBuilder().WithCustomEpoch(customEpoch).Build().Create();
		var middle = tsid.GetDateTimeOffset(customEpoch);
		var end = DateTimeOffset.Now;

		start.ToUnixTimeMilliseconds().Should().BeLessOrEqualTo(middle.ToUnixTimeMilliseconds());
		middle.ToUnixTimeMilliseconds().Should().BeLessOrEqualTo(end.ToUnixTimeMilliseconds());
	}

	[Fact]
	public void TestGetUnixMillisecondsWithCustomEpoch() 
	{
		var customEpoch = DateTimeOffset.Parse("1984-01-01T00:00:00Z");

		var start =  DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		var tsid = TsidFactory.GetBuilder().WithCustomEpoch(customEpoch).Build().Create();
		var middle = tsid.GetDateTimeOffset(customEpoch).ToUnixTimeMilliseconds();
		var end =  DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

		start.Should().BeLessOrEqualTo(middle);
		middle.Should().BeLessOrEqualTo(end);
	}

	[Fact]
	public void TestWithNode() {
		{
			for (var i = 0; i <= 20; i++) {
				const int bits = TsidFactory.NodeBits1024;
				var shif = Tsid.RandomBits - bits;
				var mask = ((1 << bits) - 1);
				var node = StaticRandom.Instance.Next() & mask;
				var factory = new TsidFactory(node);
				(node == ((factory.Create().GetRandom() >>> shif) & mask)).Should().BeTrue();
			}
		}
		{
			for (var i = 0; i <= 20; i++) {
				const int bits = TsidFactory.NodeBits1024;
				var shif = Tsid.RandomBits - bits;
				var mask = ((1 << bits) - 1);
				var node = StaticRandom.Instance.Next() & mask;
				Environment.SetEnvironmentVariable(TsidFactory.Settings.Node, node.ToString());
				var factory = new TsidFactory();
				(node == ((factory.Create().GetRandom() >>> shif) & mask)).Should().BeTrue();
			}
		}
		{
			for (var i = 0; i <= 20; i++) {
				const int bits = TsidFactory.NodeBits1024;
				var shif = Tsid.RandomBits - bits;
				var mask = ((1 << bits) - 1);
				var node = StaticRandom.Instance.Next() & mask;
				var factory = TsidFactory.GetBuilder().WithNode(node).Build();
				(node == ((factory.Create().GetRandom() >>> shif) & mask)).Should().BeTrue();
			}
		}
		{
			for (var i = 0; i <= 20; i++) {
				const int bits = TsidFactory.NodeBits256;
				var shif = Tsid.RandomBits - bits;
				var mask = ((1 << bits) - 1);
				var node = StaticRandom.Instance.Next() & mask;
				var factory = TsidFactory.NewInstance256(node);
				(node == ((factory.Create().GetRandom() >>> shif) & mask)).Should().BeTrue();
			}
		}
		{
			for (var i = 0; i <= 20; i++) {
				const int bits = TsidFactory.NodeBits1024;
				var shif = Tsid.RandomBits - bits;
				var mask = ((1 << bits) - 1);
				var node = StaticRandom.Instance.Next() & mask;
				var factory = TsidFactory.NewInstance1024(node);
				(node == ((factory.Create().GetRandom() >>> shif) & mask)).Should().BeTrue();
			}
		}
		{
			for (var i = 0; i <= 20; i++) {
				const int bits = TsidFactory.NodeBits4096;
				var shif = Tsid.RandomBits - bits;
				var mask = ((1 << bits) - 1);
				var node = StaticRandom.Instance.Next() & mask;
				var factory = TsidFactory.NewInstance4096(node);
				(node == ((factory.Create().GetRandom() >>> shif) & mask)).Should().BeTrue();
			}
		}
	}

	[Fact]
	public void TestWithNodeBits()
	{
		const int randomBits = 22;
		// test all allowed values of node bits
		for (var i = 0; i <= 20; i++) {
			var nodeBits = i;
			var counterBits = randomBits - nodeBits;
			var node = (1 << nodeBits) - 1; // max: 2^nodeBits - 1
			var tsid = TsidFactory.GetBuilder().WithNodeBits(nodeBits).WithNode(node).Build().Create();
			var actual = (int) tsid.GetRandom() >>> counterBits;
			node.Should().Be(actual);
		}
	}

	[Fact]
	public void TestWithNodeCount()
	{
		const int randomBits = 22;
		// test all allowed values of node bits
		for (var i = 0; i <= 20; i++) {
			var nodeBits = i;
			var counterBits = randomBits - nodeBits;
			var node = (1 << nodeBits) - 1; // max: 2^nodeBits - 1
			var nodeCount = (int) Math.Pow(2, nodeBits);
			Environment.SetEnvironmentVariable(TsidFactory.Settings.NodeCount, nodeCount.ToString());
			var tsid = TsidFactory.GetBuilder().WithNode(node).Build().Create();
			var actual = (int) tsid.GetRandom() >>> counterBits;
			node.Should().Be(actual);
		}
	}

	[Fact]
	public void TestWithRandom()
	{
		var randomNumberGenerator = RandomGenerators.OfSimpleRandomNumberGenerator();
		var factory = TsidFactory.GetBuilder().WithRandom(randomNumberGenerator).Build();
		factory.Create().Should().NotBeNull();
	}

	[Fact]
	public void TestWithRandomNull() {
		var factory = TsidFactory.GetBuilder().WithRandom(null).Build();
		factory.Create().Should().NotBeNull();
	}
	
	[Fact]
	public void TestWithRandomFunction() {
	 		var random = new Random();
	 		var function1 = () => random.Next();
	 		var factory1 = TsidFactory.GetBuilder().WithRandomFunction(function1).Build();
	        factory1.Create().Should().NotBeNull();
	    
	
	 		Func<int, byte[]> function2 = (int length) => {
	 			var bytes = new byte[length];
	 			StaticRandom.Instance.NextBytes(bytes);
	 			return bytes;
			};
	 		var factory2 = TsidFactory.GetBuilder().WithRandomFunction(function2).Build();
	        factory2.Create().Should().NotBeNull();
	}
	
	[Fact]
	 public void TestWithRandomFunctionNull() {
		 var factory1 = TsidFactory.GetBuilder().WithRandomFunction((Func<int>)null).Build();
		 factory1.Create().Should().NotBeNull();
	 	
		 var factory2 = TsidFactory.GetBuilder().WithRandomFunction((Func<int, byte[]>)null).Build();
		 factory2.Create().Should().NotBeNull();
	 }
	
	[Fact]
	public void TestWithRandomFunctionReturningZero() {
	
	 	// a random function that returns a fixed array filled with ZEROS
	    byte[] RandomFunction(int x) => new byte[x];

	    var factory = TsidFactory.GetBuilder().WithRandomFunction((Func<int,  byte[]>)RandomFunction).Build();
	
	 	long mask = 0b111111111111; // counter bits: 12
	
	 	// test it 5 times, waiting 1ms each time
	 	for (var i = 0; i < 5; i++) {
	 		Thread.Sleep(1); // wait 1ms
			long expected = 0;
			var counter = factory.Create().GetRandom() & mask;
			counter.Should().Be(expected); // "The counter should be equal to ZERO when the ms changes"
	    }
	}
	
	[Fact]
	public void TestWithRandomFunctionReturningNonZero() {
	
	 	// a random function that returns a fixed array
	 	byte[] @fixed = { 0, 0, 0, 0, 0, 0, 0, 127 };
	    byte[] RandomFunction(int x) => @fixed;

	    var factory = TsidFactory.GetBuilder().WithRandomFunction((Func<int,byte[]>)RandomFunction).Build();
	
	 	long mask = 0b111111111111; // counter bits: 12
	
	 	// test it 5 times, waiting 1ms each time
	 	for (var i = 0; i < 5; i++) {
	 		Thread.Sleep(1); // wait 1ms
	 		long expected = @fixed[2];
	 		var counter = factory.Create().GetRandom() & mask;
			counter.Should().Be(expected); // "The counter should be equal to a fixed value when the ms changes"
	    }
	}
	
	[Fact]
	public void TestMonotonicityAfterClockDrift() {
	
		const long diff = 10000;
		var time = DateTimeOffset.Parse("2021-12-31T23:59:59.000Z").ToUnixTimeMilliseconds();
		long[] times = { -1, time, time + 0, time + 1, time + 2, time + 3 - diff, time + 4 - diff, time + 5 };

		// a function that forces the clock to restart to ZERO
		byte[] RandomFunction(int x) => new byte[x];
		int[] sillyIndexHolder = { 0 };
		var clock = new Clock(TimeZoneInfo.Utc, () => times[sillyIndexHolder[0]]);
	 	var factory = TsidFactory.GetBuilder().WithClock(clock).WithRandomFunction((Func<int,byte[]>)RandomFunction).Build();
	
		sillyIndexHolder[0] = 1;
		var ms1 = factory.Create().GetUnixMilliseconds(); // time
		sillyIndexHolder[0] = 2;
		var ms2 = factory.Create().GetUnixMilliseconds(); // time + 0
		sillyIndexHolder[0] = 3;
		var ms3 = factory.Create().GetUnixMilliseconds(); // time + 1
		sillyIndexHolder[0] = 4;
		var ms4 = factory.Create().GetUnixMilliseconds(); // time + 2
		sillyIndexHolder[0] = 5;
		var ms5 = factory.Create().GetUnixMilliseconds(); // time + 3 - 10000 (CLOCK DRIFT)
		sillyIndexHolder[0] = 6;
		var ms6 = factory.Create().GetUnixMilliseconds(); // time + 4 - 10000 (CLOCK DRIFT)
		sillyIndexHolder[0] = 7;
		var ms7 = factory.Create().GetUnixMilliseconds(); // time + 5
		(ms1 + 0).Should().Be(ms2); // clock repeats.
		(ms1 + 1).Should().Be(ms3); // clock advanced.
		(ms1 + 2).Should().Be(ms4); // clock advanced.
		(ms1 + 2).Should().Be(ms5); // CLOCK DRIFT! DON'T MOVE BACKWARDS!
		(ms1 + 2).Should().Be(ms6); // CLOCK DRIFT! DON'T MOVE BACKWARDS!
		(ms1 + 5).Should().Be(ms7); // clock advanced.
	}
	
	[Fact]
	public void TestMonotonicityAfterLeapSecond() 
	{
		var second = DateTimeOffset.Parse("2021-12-31T23:59:59.000Z").ToUnixTimeSeconds();
		// long second = Instant.parse("2021-12-31T23:59:59.000Z").getEpochSecond();
		var leapSecond = second - 1; // simulate a leap second
		long[] times = { second, leapSecond };

		// a function that forces the clock to restart to ZERO
		byte[] RandomFunction(int x) => new byte[x];
		int[] sillyIndexHolder = { 0 };
		var clock = new Clock(TimeZoneInfo.Utc, () => times[sillyIndexHolder[0]]);
		var factory = TsidFactory.GetBuilder().WithClock(clock).WithRandomFunction((Func<int, byte[]>)RandomFunction).Build();
	
		var ms1 = factory.Create().GetUnixMilliseconds(); // second
		sillyIndexHolder[0] = 1;
		var ms2 = factory.Create().GetUnixMilliseconds(); // leap second
	
		ms1.Should().Be(ms2); // LEAP SECOND! DON'T MOVE BACKWARDS!
	}
	
	[Fact]
	public void TestByteRandomNextInt() {
	
		for (var i = 0; i < 10; i++)
		{
			var bytes = new byte[IntegerBytes];
			StaticRandom.Instance.NextBytes(bytes);
			var number = BitConverter.ToInt32(bytes.Reverse().ToArray());
			TsidFactory.IRandom random = new TsidFactory.ByteRandom((x) => bytes);
			random.NextInt().Should().Be(number);
		}

		for (var i = 0; i < 10; i++)
		{
			var ints = 10;
			var size = IntegerBytes * ints;

			var bytes = new byte[size];
			StaticRandom.Instance.NextBytes(bytes);
			using var buffer1 = new MemoryStream(bytes.Reverse().ToArray());
			using var buffer2 = new MemoryStream(bytes.Reverse().ToArray());

			TsidFactory.IRandom random = new TsidFactory.ByteRandom((x) =>
			{
				var octects = new byte[x];
				buffer1.Read(octects, 0, x);
				return octects;
			});

			for (var j = 0; j < ints; j++)
			{
				var intBytes = new byte[IntegerBytes];
				buffer2.Read(intBytes, 0, IntegerBytes);
				var expectedInt = BitConverter.ToInt32(intBytes.Reverse().ToArray());
				random.NextInt().Should().Be(expectedInt);
			}
		}
	}
	
	 [Fact]
	 public void TestByteRandomNextBytes() {
	 	for (var i = 0; i < 10; i++) {
	 		var bytes = new byte[IntegerBytes];
	        StaticRandom.Instance.NextBytes(bytes);
	 		TsidFactory.IRandom random = new TsidFactory.ByteRandom((_) => bytes);
	        var nextBytes = random.NextBytes(IntegerBytes);
	        Arrays.ToString(bytes).Should().Be(Arrays.ToString(nextBytes));
	 	}
	
	 	for (var i = 0; i < 10; i++) {
	
	 		var ints = 10;
	 		var size = IntegerBytes * ints;
	
	 		var bytes = new byte[size];
	        StaticRandom.Instance.NextBytes(bytes);
	        using var buffer1 = new MemoryStream(bytes); 
	        using var buffer2 = new MemoryStream(bytes);
	        TsidFactory.IRandom random = new TsidFactory.ByteRandom((x) =>
	        {
		        var octects = new byte[x];
		        buffer1.Read(octects, 0, x);
		        return octects;
	        });
	
	 		for (var j = 0; j < ints; j++) {
		        var octects = new byte[IntegerBytes];
		        buffer2.Read(octects, 0, IntegerBytes);

		        var nextBytes = random.NextBytes(IntegerBytes);
		        Arrays.ToString(octects).Should().Be(Arrays.ToString(nextBytes));
	 		}
	 	}
	 }
	
	[Fact]
	public void TestLogRandomNextInt() {
	
		for (var i = 0; i < 10; i++) {
			var bytes = new byte[IntegerBytes];
			StaticRandom.Instance.NextBytes(bytes);
			var number = BitConverter.ToInt32(bytes.Reverse().ToArray());
			TsidFactory.IRandom random = new TsidFactory.IntRandom(() => number);
			number.Should().Be(random.NextInt());
		}
	
		for (var i = 0; i < 10; i++) {
	
			var ints = 10;
			var size = IntegerBytes * ints;
	
			var bytes = new byte[size];
			StaticRandom.Instance.NextBytes(bytes);
			using var buffer1 = new MemoryStream(bytes.Reverse().ToArray());
			using var buffer2 = new MemoryStream(bytes.Reverse().ToArray());
	
			TsidFactory.IRandom random = new TsidFactory.IntRandom(() =>
			{
				var octects = new byte[IntegerBytes];
				buffer1.Read(octects, 0, IntegerBytes);
				return BitConverter.ToInt32(octects);
			});
	
			for (var j = 0; j < ints; j++) {
				var octects = new byte[IntegerBytes];
				buffer2.Read(octects, 0, IntegerBytes);
				BitConverter.ToInt32(octects).Should().Be(random.NextInt());
			}
		}
	}
	
	[Fact]
	public void TestLogRandomNextBytes() {
	
		for (var i = 0; i < 10; i++) {
			var bytes = new byte[IntegerBytes];
			StaticRandom.Instance.NextBytes(bytes);
			var number = BitConverter.ToInt32(bytes.Reverse().ToArray());
			TsidFactory.IRandom random = new TsidFactory.IntRandom(() => number);
			Arrays.ToString(bytes).Should().Be(Arrays.ToString(random.NextBytes(IntegerBytes)));
		}
	
		for (var i = 0; i < 10; i++) {
	
			var ints = 10;
			var size = IntegerBytes * ints;
	
			var bytes = new byte[size];
			StaticRandom.Instance.NextBytes(bytes);
			using var buffer1 = new MemoryStream(bytes.Reverse().ToArray());
			using var buffer2 = new MemoryStream(bytes.Reverse().ToArray());
	
			TsidFactory.IRandom random = new TsidFactory.IntRandom(() =>
			{
				var octects = new byte[IntegerBytes];
				buffer1.Read(octects, 0, IntegerBytes);
				octects = octects.Reverse().ToArray();
				return BitConverter.ToInt32(octects);
			});
	
			for (var j = 0; j < ints; j++) {
				var octects = new byte[IntegerBytes];
				buffer2.Read(octects, 0, IntegerBytes);
				var nextBytes = random.NextBytes(IntegerBytes);
				Arrays.ToString(octects).Should().Be(Arrays.ToString(nextBytes));
			}
		}
	}
	
	[Fact]
	public void TestSettingsGetNode() {
	 	for (var i = 0; i < 100; i++) {
	 		long number = StaticRandom.Instance.Next();
	 		Environment.SetEnvironmentVariable(TsidFactory.Settings.Node, number.ToString());
	 		var result = (long)TsidFactory.Settings.GetNode();
	        Environment.SetEnvironmentVariable(TsidFactory.Settings.Node, null);
	 		number.Should().Be(result);
	 	}
	}
	
	[Fact]
	public void TestSettingsGetNodeCount() {
	 	for (var i = 0; i < 100; i++) {
		    long number = StaticRandom.Instance.Next();
		    Environment.SetEnvironmentVariable(TsidFactory.Settings.NodeCount, number.ToString());
	        var result = (long)TsidFactory.Settings.GetNodeCount();
	        Environment.SetEnvironmentVariable(TsidFactory.Settings.NodeCount, null);
	        number.Should().Be(result);
	 	}
	}
	
	[Fact]
	public void TestSettingsGetNodeInvalid() {
		var str = "0xx11223344"; // typo
		Environment.SetEnvironmentVariable(TsidFactory.Settings.Node, str);
		var result = TsidFactory.Settings.GetNode();
		Environment.SetEnvironmentVariable(TsidFactory.Settings.Node, null);
		result.Should().BeNull();

		str = " 0x11223344"; // space
		Environment.SetEnvironmentVariable(TsidFactory.Settings.Node, str);
		result = TsidFactory.Settings.GetNode();
		Environment.SetEnvironmentVariable(TsidFactory.Settings.Node, null);
		result.Should().BeNull();
	
		str = "0x112233zz"; // non hexadecimal
		Environment.SetEnvironmentVariable(TsidFactory.Settings.Node, str);
		result = TsidFactory.Settings.GetNode();
		Environment.SetEnvironmentVariable(TsidFactory.Settings.Node, null);
		result.Should().BeNull();
	
		str = ""; // empty
		Environment.SetEnvironmentVariable(TsidFactory.Settings.Node, str);
		result = TsidFactory.Settings.GetNode();
		Environment.SetEnvironmentVariable(TsidFactory.Settings.Node, null);
		result.Should().BeNull();
	
		str = " "; // blank
		Environment.SetEnvironmentVariable(TsidFactory.Settings.Node, str);
		result = TsidFactory.Settings.GetNode();
		Environment.SetEnvironmentVariable(TsidFactory.Settings.Node, null);
		result.Should().BeNull();
	}
	
	[Fact]
	public void TestSettingsGetNodeCountInvalid() {
		var str = "0xx11223344"; // typo
		Environment.SetEnvironmentVariable(TsidFactory.Settings.NodeCount, str);
		var result = TsidFactory.Settings.GetNodeCount();
		Environment.SetEnvironmentVariable(TsidFactory.Settings.NodeCount, null);
		result.Should().BeNull();
	
		str = " 0x11223344"; // space
		Environment.SetEnvironmentVariable(TsidFactory.Settings.NodeCount, str);
		result = TsidFactory.Settings.GetNodeCount();
		Environment.SetEnvironmentVariable(TsidFactory.Settings.NodeCount, null);
		result.Should().BeNull();
	
		str = "0x112233zz"; // non hexadecimal
		Environment.SetEnvironmentVariable(TsidFactory.Settings.NodeCount, str);
		result = TsidFactory.Settings.GetNodeCount();
		Environment.SetEnvironmentVariable(TsidFactory.Settings.NodeCount, null);
		result.Should().BeNull();
	
		str = ""; // empty
		Environment.SetEnvironmentVariable(TsidFactory.Settings.NodeCount, str);
		result = TsidFactory.Settings.GetNodeCount();
		Environment.SetEnvironmentVariable(TsidFactory.Settings.NodeCount, null);
		result.Should().BeNull();
	
		str = " "; // blank
		Environment.SetEnvironmentVariable(TsidFactory.Settings.NodeCount, str);
		result = TsidFactory.Settings.GetNodeCount();
		Environment.SetEnvironmentVariable(TsidFactory.Settings.NodeCount, null);
		result.Should().BeNull();
	}
}