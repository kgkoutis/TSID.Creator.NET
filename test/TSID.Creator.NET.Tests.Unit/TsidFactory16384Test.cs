﻿using System.Collections.Concurrent;

namespace TSID.Creator.NET.Tests.Unit;

public class TsidFactory16384Test : TsidFactory00000Test {

	private static readonly int NODE_BITS = 14;
	private static readonly int COUNTER_BITS = 8;

	private static readonly int NODE_MAX = (int) Math.Pow(2, NODE_BITS);
	private static readonly int COUNTER_MAX = (int) Math.Pow(2, COUNTER_BITS);

	[Fact]
	public void TestGetTsid16384()
	{
		long startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

		TsidFactory factory = TsidFactory.GetBuilder().WithNodeBits(NODE_BITS).WithRandom(random).Build();

		long[] list = new long[LOOP_MAX];
		for (int i = 0; i < LOOP_MAX; i++) {
			list[i] = factory.Create().ToLong();
		}

		long endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

		CheckNullOrInvalid(list).Should().BeTrue();
		CheckUniqueness(list).Should().BeTrue();
		CheckOrdering(list).Should().BeTrue();
		CheckMaximumPerMs(list, COUNTER_MAX).Should().BeTrue();
		CheckCreationTime(list, startTime, endTime).Should().BeTrue();
	}

	[Fact]
	public void TestGetTsid16384WithNode() {

		long startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

		int node;
		do
		{
			node = random.NextInt();	
		} while(node >= NODE_MAX);
		
		TsidFactory factory = TsidFactory.GetBuilder().WithNode(node).WithNodeBits(NODE_BITS).WithRandom(random).Build();

		long[] list = new long[LOOP_MAX];
		for (int i = 0; i < LOOP_MAX; i++) {
			list[i] = factory.Create().ToLong();
		}

		long endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

		CheckNullOrInvalid(list).Should().BeTrue();
		CheckUniqueness(list).Should().BeTrue();
		CheckOrdering(list).Should().BeTrue();
		CheckMaximumPerMs(list, COUNTER_MAX).Should().BeTrue();
		CheckCreationTime(list, startTime, endTime).Should().BeTrue();
	}

	[Fact]
	public void TestGetTsidString16384() {

		long startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

		TsidFactory factory = TsidFactory.GetBuilder().WithNodeBits(NODE_BITS).WithRandom(random).Build();

		string[] list = new string[LOOP_MAX];
		for (int i = 0; i < LOOP_MAX; i++) {
			list[i] = factory.Create().ToString();
		}

		long endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

		CheckNullOrInvalid(list).Should().BeTrue();
		CheckUniqueness(list).Should().BeTrue();
		CheckOrdering(list).Should().BeTrue();
		CheckMaximumPerMs(list, COUNTER_MAX).Should().BeTrue();
		CheckCreationTime(list, startTime, endTime).Should().BeTrue();
	}

	[Fact]
	public void TestGetTsidString16384WithNode() {

		long startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

		int node;
		do
		{
			node = random.NextInt();	
		} while(node >= NODE_MAX);
		
		TsidFactory factory = TsidFactory.GetBuilder().WithNode(node).WithNodeBits(NODE_BITS).WithRandom(random).Build();

		string[] list = new string[LOOP_MAX];
		for (int i = 0; i < LOOP_MAX; i++) {
			list[i] = factory.Create().ToString();
		}

		long endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

		CheckNullOrInvalid(list).Should().BeTrue();
		CheckUniqueness(list).Should().BeTrue();
		CheckOrdering(list).Should().BeTrue();
		CheckMaximumPerMs(list, COUNTER_MAX).Should().BeTrue();
		CheckCreationTime(list, startTime, endTime).Should().BeTrue();
	}

	[Fact]
	public void TestGetTsid16384Parallel() {
		ConcurrentDictionary<string, byte>[] sets = new ConcurrentDictionary<string, byte>[MultiplePasses];
		int counterMax = COUNTER_MAX / MultiplePasses;
		
		// Instantiate and start many threads
		for (int i = 0; i < MultiplePasses; i++) {
			TsidFactory factory = TsidFactory.GetBuilder().WithNode(1).WithNodeBits(NODE_BITS).WithRandom(random).Build();
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