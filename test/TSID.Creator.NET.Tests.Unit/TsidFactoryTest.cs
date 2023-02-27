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
			var random = StaticRandom.NextLong(min: 0, max: bound);
			var millis = random + Tsid.TsidEpoch; // avoid dates before 2020
			var clock = new Clock(TimeZoneInfo.Utc, millis); // simulate a frozen clock  
			var randomFunction = (int x) => new byte[x]; // force to reinitialize the counter to ZERO
			var factory = TsidFactory.GetBuilder().WithClock(clock).WithRandomFunction(randomFunction).Build();

			var result = factory.Create().GetDateTimeOffset().ToUnixTimeMilliseconds();
			millis.Should().Be(result); // "The current instant is incorrect"
		}
	}

	[Fact]
	public void TestGetUnixMillisecondsWithClock() {

		long bound = (long) Math.Pow(2, 42);

		for (int i = 0; i < LoopMax; i++) {

			// instantiate a factory with a Clock that returns a fixed value
			var random = StaticRandom.NextLong(min: 0, max: bound);
			var millis = random + Tsid.TsidEpoch; // avoid dates before 2020
			var clock = new Clock(TimeZoneInfo.Utc, millis); // simulate a frozen clock 
			var randomFunction = (int x) => new byte[x]; // force to reinitialize the counter to ZERO
			var factory = TsidFactory.GetBuilder().WithClock(clock).WithRandomFunction(randomFunction).Build();

			var result = factory.Create().GetUnixMilliseconds();
			millis.Should().Be(result); // "The current instant is incorrect"
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
				var node = StaticRandom.Rand() & mask;
				var factory = new TsidFactory(node);
				(node == ((factory.Create().GetRandom() >>> shif) & mask)).Should().BeTrue();
			}
		}
		{
			for (var i = 0; i <= 20; i++) {
				const int bits = TsidFactory.NodeBits1024;
				var shif = Tsid.RandomBits - bits;
				var mask = ((1 << bits) - 1);
				var node = StaticRandom.Rand() & mask;
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
				var node = StaticRandom.Rand() & mask;
				var factory = TsidFactory.GetBuilder().WithNode(node).Build();
				(node == ((factory.Create().GetRandom() >>> shif) & mask)).Should().BeTrue();
			}
		}
		{
			for (var i = 0; i <= 20; i++) {
				const int bits = TsidFactory.NodeBits256;
				var shif = Tsid.RandomBits - bits;
				var mask = ((1 << bits) - 1);
				var node = StaticRandom.Rand() & mask;
				var factory = TsidFactory.NewInstance256(node);
				(node == ((factory.Create().GetRandom() >>> shif) & mask)).Should().BeTrue();
			}
		}
		{
			for (var i = 0; i <= 20; i++) {
				const int bits = TsidFactory.NodeBits1024;
				var shif = Tsid.RandomBits - bits;
				var mask = ((1 << bits) - 1);
				var node = StaticRandom.Rand() & mask;
				var factory = TsidFactory.NewInstance1024(node);
				(node == ((factory.Create().GetRandom() >>> shif) & mask)).Should().BeTrue();
			}
		}
		{
			for (var i = 0; i <= 20; i++) {
				const int bits = TsidFactory.NodeBits4096;
				var shif = Tsid.RandomBits - bits;
				var mask = ((1 << bits) - 1);
				var node = StaticRandom.Rand() & mask;
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
	 		TsidFactory factory1 = TsidFactory.GetBuilder().WithRandomFunction(function1).Build();
	        factory1.Create().Should().NotBeNull();
	    
	
	 		Func<int, byte[]> function2 = (int length) => {
	 			byte[] bytes = new byte[length];
	 			StaticRandom.RandBytes(bytes);
	 			return bytes;
			};
	 		TsidFactory factory2 = TsidFactory.GetBuilder().WithRandomFunction(function2).Build();
	        factory2.Create().Should().NotBeNull();
	}
	
	[Fact]
	 public void TestWithRandomFunctionNull() {
		 TsidFactory factory1 = TsidFactory.GetBuilder().WithRandomFunction((Func<int>)null).Build();
		 factory1.Create().Should().NotBeNull();
	 	
		 TsidFactory factory2 = TsidFactory.GetBuilder().WithRandomFunction((Func<int, byte[]>)null).Build();
		 factory2.Create().Should().NotBeNull();
	 }
	
	[Fact]
	public void TestWithRandomFunctionReturningZero() {
	
	 	// a random function that returns a fixed array filled with ZEROS
	    byte[] RandomFunction(int x) => new byte[x];

	    TsidFactory factory = TsidFactory.GetBuilder().WithRandomFunction((Func<int,  byte[]>)RandomFunction).Build();
	
	 	long mask = 0b111111111111; // counter bits: 12
	
	 	// test it 5 times, waiting 1ms each time
	 	for (int i = 0; i < 5; i++) {
	 		Thread.Sleep(1); // wait 1ms
			long expected = 0;
			long counter = factory.Create().GetRandom() & mask;
			counter.Should().Be(expected); // "The counter should be equal to ZERO when the ms changes"
	    }
	}
	
	[Fact]
	public void TestWithRandomFunctionReturningNonZero() {
	
	 	// a random function that returns a fixed array
	 	byte[] @fixed = { 0, 0, 0, 0, 0, 0, 0, 127 };
	    byte[] RandomFunction(int x) => @fixed;

	    TsidFactory factory = TsidFactory.GetBuilder().WithRandomFunction((Func<int,byte[]>)RandomFunction).Build();
	
	 	long mask = 0b111111111111; // counter bits: 12
	
	 	// test it 5 times, waiting 1ms each time
	 	for (int i = 0; i < 5; i++) {
	 		Thread.Sleep(1); // wait 1ms
	 		long expected = @fixed[2];
	 		long counter = factory.Create().GetRandom() & mask;
			counter.Should().Be(expected); // "The counter should be equal to a fixed value when the ms changes"
	    }
	}
	
	// [Fact]
	// public void TestMonotonicityAfterClockDrift() {
	//
	// 	long diff = 10_000;
	// 	long time = DateTimeOffset.Parse("2021-12-31T23:59:59.000Z").ToUnixTimeMilliseconds();
	// 	long[] times = { -1, time, time + 0, time + 1, time + 2, time + 3 - diff, time + 4 - diff, time + 5 };
	//
	// 	Clock clock = new Clock() {
	// 		private int i;
	//
	// 		@Override
	// 		public long millis() {
	// 			return times[i++ % times.length];
	// 		}
	//
	// 		@Override
	// 		public ZoneId getZone() {
	// 			return null;
	// 		}
	//
	// 		@Override
	// 		public Clock withZone(ZoneId zone) {
	// 			return null;
	// 		}
	//
	// 		@Override
	// 		public Instant instant() {
	// 			return null;
	// 		}
	// 	};
	//
	//  	// a function that forces the clock to restart to ZERO
	// 	Func<int,byte[]>  randomFunction = (int x) => new byte[x];
	//
	//  	TsidFactory factory = TsidFactory.GetBuilder().WithClock(clock).WithRandomFunction(randomFunction).Build();
	//
	// 	long ms1 = factory.Create().GetUnixMilliseconds(); // time
	// 	long ms2 = factory.Create().GetUnixMilliseconds(); // time + 0
	// 	long ms3 = factory.Create().GetUnixMilliseconds(); // time + 1
	// 	long ms4 = factory.Create().GetUnixMilliseconds(); // time + 2
	// 	long ms5 = factory.Create().GetUnixMilliseconds(); // time + 3 - 10000 (CLOCK DRIFT)
	// 	long ms6 = factory.Create().GetUnixMilliseconds(); // time + 4 - 10000 (CLOCK DRIFT)
	// 	long ms7 = factory.Create().GetUnixMilliseconds(); // time + 5
	// 	assertEquals(ms1 + 0, ms2); // clock repeats.
	// 	assertEquals(ms1 + 1, ms3); // clock advanced.
	// 	assertEquals(ms1 + 2, ms4); // clock advanced.
	// 	assertEquals(ms1 + 2, ms5); // CLOCK DRIFT! DON'T MOVE BACKWARDS!
	// 	assertEquals(ms1 + 2, ms6); // CLOCK DRIFT! DON'T MOVE BACKWARDS!
	// 	assertEquals(ms1 + 5, ms7); // clock advanced.
	// }
	
	// [Fact]
	// public void TestMonotonicityAfterLeapSecond() {
	//
	// 	long second = Instant.parse("2021-12-31T23:59:59.000Z").getEpochSecond();
	// 	long leapSecond = second - 1; // simulate a leap second
	// 	long times[] = { second, leapSecond };
	//
	// 	Clock clock = new Clock() {
	// 		private int i;
	//
	// 		@Override
	// 		public long millis() {
	// 			return times[i++ % times.length] * 1000;
	// 		}
	//
	// 		@Override
	// 		public ZoneId getZone() {
	// 			return null;
	// 		}
	//
	// 		@Override
	// 		public Clock withZone(ZoneId zone) {
	// 			return null;
	// 		}
	//
	// 		@Override
	// 		public Instant instant() {
	// 			return null;
	// 		}
	// 	};
	//
	// 	// a function that forces the clock to restart to ZERO
	// 	IntFunction<byte[]> randomFunction = x -> new byte[x];
	//
	// 	TsidFactory factory = TsidFactory.builder().withClock(clock).withRandomFunction(randomFunction).build();
	//
	// 	long ms1 = factory.create().getUnixMilliseconds(); // second
	// 	long ms2 = factory.create().getUnixMilliseconds(); // leap second
	//
	// 	assertEquals(ms1, ms2); // LEAP SECOND! DON'T MOVE BACKWARDS!
	// }
	//
	[Fact]
	public void TestByteRandomNextInt() {
	
		for (int i = 0; i < 10; i++)
		{
			byte[] bytes = new byte[IntegerBytes];
			StaticRandom.RandBytes(bytes);
			int number = BitConverter.ToInt32(bytes.Reverse().ToArray());
			TsidFactory.IRandom random = new TsidFactory.ByteRandom((x) => bytes);
			random.NextInt().Should().Be(number);
		}

		for (int i = 0; i < 10; i++)
		{
			int ints = 10;
			int size = IntegerBytes * ints;

			byte[] bytes = new byte[size];
			StaticRandom.RandBytes(bytes);
			using var buffer1 = new MemoryStream(bytes.Reverse().ToArray());
			using var buffer2 = new MemoryStream(bytes.Reverse().ToArray());

			TsidFactory.IRandom random = new TsidFactory.ByteRandom((x) =>
			{
				byte[] octects = new byte[x];
				buffer1.Read(octects, 0, x);
				return octects;
			});

			for (int j = 0; j < ints; j++)
			{
				byte[] intBytes = new byte[IntegerBytes];
				buffer2.Read(intBytes, 0, IntegerBytes);
				int expectedInt = BitConverter.ToInt32(intBytes.Reverse().ToArray());
				random.NextInt().Should().Be(expectedInt);
			}
		}
	}
	
	 [Fact]
	 public void TestByteRandomNextBytes() {
	 	for (int i = 0; i < 10; i++) {
	 		byte[] bytes = new byte[IntegerBytes];
	        StaticRandom.RandBytes(bytes);
	 		TsidFactory.IRandom random = new TsidFactory.ByteRandom((_) => bytes);
	        byte[] nextBytes = random.NextBytes(IntegerBytes);
	        Arrays.ToString(bytes).Should().Be(Arrays.ToString(nextBytes));
	 	}
	
	 	for (int i = 0; i < 10; i++) {
	
	 		int ints = 10;
	 		int size = IntegerBytes * ints;
	
	 		byte[] bytes = new byte[size];
	        StaticRandom.RandBytes(bytes);
	        using var buffer1 = new MemoryStream(bytes); 
	        using var buffer2 = new MemoryStream(bytes);
	        TsidFactory.IRandom random = new TsidFactory.ByteRandom((x) =>
	        {
		        byte[] octects = new byte[x];
		        buffer1.Read(octects, 0, x);
		        return octects;
	        });
	
	 		for (int j = 0; j < ints; j++) {
		        byte[] octects = new byte[IntegerBytes];
		        buffer2.Read(octects, 0, IntegerBytes);

		        byte[]? nextBytes = random.NextBytes(IntegerBytes);
		        Arrays.ToString(octects).Should().Be(Arrays.ToString(nextBytes));
	 		}
	 	}
	 }
	
	[Fact]
	public void TestLogRandomNextInt() {
	
		for (int i = 0; i < 10; i++) {
			byte[] bytes = new byte[IntegerBytes];
			StaticRandom.RandBytes(bytes);
			int number = BitConverter.ToInt32(bytes.Reverse().ToArray());
			TsidFactory.IRandom random = new TsidFactory.IntRandom(() => number);
			number.Should().Be(random.NextInt());
		}
	
		for (int i = 0; i < 10; i++) {
	
			int ints = 10;
			int size = IntegerBytes * ints;
	
			byte[] bytes = new byte[size];
			StaticRandom.RandBytes(bytes);
			using var buffer1 = new MemoryStream(bytes.Reverse().ToArray());
			using var buffer2 = new MemoryStream(bytes.Reverse().ToArray());
	
			TsidFactory.IRandom random = new TsidFactory.IntRandom(() =>
			{
				byte[] octects = new byte[IntegerBytes];
				buffer1.Read(octects, 0, IntegerBytes);
				return BitConverter.ToInt32(octects);
				// return buffer1.getInt();
			});
	
			for (int j = 0; j < ints; j++) {
				byte[] octects = new byte[IntegerBytes];
				buffer2.Read(octects, 0, IntegerBytes);
				BitConverter.ToInt32(octects).Should().Be(random.NextInt());
			}
		}
	}
	
	[Fact]
	public void TestLogRandomNextBytes() {
	
		for (int i = 0; i < 10; i++) {
			byte[] bytes = new byte[IntegerBytes];
			StaticRandom.RandBytes(bytes);
			int number = BitConverter.ToInt32(bytes.Reverse().ToArray());
			TsidFactory.IRandom random = new TsidFactory.IntRandom(() => number);
			Arrays.ToString(bytes).Should().Be(Arrays.ToString(random.NextBytes(IntegerBytes)));
		}
	
		for (int i = 0; i < 10; i++) {
	
			int ints = 10;
			int size = IntegerBytes * ints;
	
			byte[] bytes = new byte[size];
			StaticRandom.RandBytes(bytes);
			using var buffer1 = new MemoryStream(bytes.Reverse().ToArray());
			using var buffer2 = new MemoryStream(bytes.Reverse().ToArray());
	
			TsidFactory.IRandom random = new TsidFactory.IntRandom(() =>
			{
				byte[] octects = new byte[IntegerBytes];
				buffer1.Read(octects, 0, IntegerBytes);
				octects = octects.Reverse().ToArray();
				return BitConverter.ToInt32(octects);
			});
	
			for (int j = 0; j < ints; j++) {
				byte[] octects = new byte[IntegerBytes];
				buffer2.Read(octects, 0, IntegerBytes);
				var nextBytes = random.NextBytes(IntegerBytes);
				Arrays.ToString(octects).Should().Be(Arrays.ToString(nextBytes));
			}
		}
	}
	
	[Fact]
	public void TestSettingsGetNode() {
	 	for (int i = 0; i < 100; i++) {
	 		long number = StaticRandom.Rand();
	 		Environment.SetEnvironmentVariable(TsidFactory.Settings.Node, number.ToString());
	 		long result = (long)TsidFactory.Settings.GetNode();
	        Environment.SetEnvironmentVariable(TsidFactory.Settings.Node, null);
	 		number.Should().Be(result);
	 	}
	}
	
	[Fact]
	public void TestSettingsGetNodeCount() {
	 	for (int i = 0; i < 100; i++) {
		    long number = StaticRandom.Rand();
		    Environment.SetEnvironmentVariable(TsidFactory.Settings.NodeCount, number.ToString());
	        long result = (long)TsidFactory.Settings.GetNodeCount();
	        Environment.SetEnvironmentVariable(TsidFactory.Settings.NodeCount, null);
	        number.Should().Be(result);
	 	}
	}
	
	[Fact]
	public void TestSettingsGetNodeInvalid() {
		string str = "0xx11223344"; // typo
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
