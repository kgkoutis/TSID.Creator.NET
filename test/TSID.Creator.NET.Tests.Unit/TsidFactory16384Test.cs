using System.Collections.Concurrent;

namespace TSID.Creator.NET.Tests.Unit;

public class TsidFactory16384Test : TsidFactory00000Test {
	private const int NodeBits = 14;
	private const int CounterBits = 8;

	private static readonly int NodeMax = (int) Math.Pow(2, NodeBits);
	private static readonly int CounterMax = (int) Math.Pow(2, CounterBits);

	[Fact]
	public void TestGetTsid16384()
	{
		var startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

		var factory = TsidFactory.GetBuilder().WithNodeBits(NodeBits).WithRandom(Random).Build();

		var list = new long[LoopMax];
		for (var i = 0; i < LoopMax; i++) {
			list[i] = factory.Create().ToLong();
		}

		var endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

		CheckNullOrInvalid(list).Should().BeTrue();
		CheckUniqueness(list).Should().BeTrue();
		CheckOrdering(list).Should().BeTrue();
		CheckMaximumPerMs(list, CounterMax).Should().BeTrue();
		CheckCreationTime(list, startTime, endTime).Should().BeTrue();
	}

	[Fact]
	public void TestGetTsid16384WithNode() {

		var startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

		var node = Random.NextInt(NodeMax);
		
		var factory = TsidFactory.GetBuilder().WithNode(node).WithNodeBits(NodeBits).WithRandom(Random).Build();

		var list = new long[LoopMax];
		for (var i = 0; i < LoopMax; i++) {
			list[i] = factory.Create().ToLong();
		}

		var endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

		CheckNullOrInvalid(list).Should().BeTrue();
		CheckUniqueness(list).Should().BeTrue();
		CheckOrdering(list).Should().BeTrue();
		CheckMaximumPerMs(list, CounterMax).Should().BeTrue();
		CheckCreationTime(list, startTime, endTime).Should().BeTrue();
	}

	[Fact]
	public void TestGetTsidString16384() {

		var startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

		var factory = TsidFactory.GetBuilder().WithNodeBits(NodeBits).WithRandom(Random).Build();

		var list = new string[LoopMax];
		for (var i = 0; i < LoopMax; i++) {
			list[i] = factory.Create().ToString();
		}

		var endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

		CheckNullOrInvalid(list).Should().BeTrue();
		CheckUniqueness(list).Should().BeTrue();
		CheckOrdering(list).Should().BeTrue();
		CheckMaximumPerMs(list, CounterMax).Should().BeTrue();
		CheckCreationTime(list, startTime, endTime).Should().BeTrue();
	}

	[Fact]
	public void TestGetTsidString16384WithNode() {

		var startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

		var node = Random.NextInt(NodeMax);
		
		var factory = TsidFactory.GetBuilder().WithNode(node).WithNodeBits(NodeBits).WithRandom(Random).Build();

		var list = new string[LoopMax];
		for (var i = 0; i < LoopMax; i++) {
			list[i] = factory.Create().ToString();
		}

		var endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

		CheckNullOrInvalid(list).Should().BeTrue();
		CheckUniqueness(list).Should().BeTrue();
		CheckOrdering(list).Should().BeTrue();
		CheckMaximumPerMs(list, CounterMax).Should().BeTrue();
		CheckCreationTime(list, startTime, endTime).Should().BeTrue();
	}

	[Fact]
	public void TestGetTsid16384Parallel() {
		var sets = new ConcurrentDictionary<string, byte>[MultiplePasses];
		var counterMax = CounterMax / MultiplePasses;
		
		// Instantiate and start many threads
		for (var i = 0; i < MultiplePasses; i++) {
			var factory = TsidFactory.GetBuilder().WithNode(1).WithNodeBits(NodeBits).WithRandom(Random).Build();
			sets[i] = new ConcurrentDictionary<string, byte>();
			Parallel.For(0, counterMax, j => {
				sets[i].TryAdd(factory.Create().ToString(), 0);
			});
		}

	 	// Check if the quantity of unique UUIDs is correct
	    var sum = sets.Select(x => x.Count).Aggregate((a, b) => a + b);
	    (counterMax * MultiplePasses).Should().Be(sum);
	}
}