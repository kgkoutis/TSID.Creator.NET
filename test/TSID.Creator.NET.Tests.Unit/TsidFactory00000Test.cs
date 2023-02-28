namespace TSID.Creator.NET.Tests.Unit;

public abstract class TsidFactory00000Test {
	private const int TsidLength = 13;

	protected const int LoopMax = 10_000;

	protected static readonly RandomGenerators Random = RandomGenerators.OfSimpleRandomNumberGenerator();

	protected static readonly int MultiplePasses = AvailableProcessors();

	protected static bool CheckNullOrInvalid(long[] list) {
		foreach (var tsid in list) {
			tsid.Should().NotBe(0);
		}
		return true; // success
	}

	protected static bool CheckNullOrInvalid(string[] list) {
		foreach (var tsid in list) {
			tsid.Should().NotBeNull();
			tsid.Should().NotBeEmpty();
			tsid.Should().NotBeNullOrWhiteSpace();
			tsid.Length.Should().Be(TsidLength);
			Tsid.IsValid(tsid).Should().BeTrue();
		}
		return true; // success
	}

	protected static bool CheckUniqueness(long[] list) {

		var set = new HashSet<long>();

		foreach (var tsid in list)
		{
			set.Add(tsid).Should().BeTrue();
		}

		set.Count.Should().Be(list.Length);
		return true; // success
	}

	protected static bool CheckUniqueness(string[] list) {

		var set = new HashSet<string>();

		foreach (var tsid in list) {
			set.Add(tsid).Should().BeTrue();
		}

		set.Count.Should().Be(list.Length);
		return true; // success
	}

	protected static bool CheckOrdering(long[] list) {
		// copy the list
		var other = new long[list.Length];
		Array.Copy(list, 0, other, 0, list.Length);
		Array.Sort(other);
		
		for (var i = 0; i < list.Length; i++) {
			list[i].Should().Be(other[i]);
		}
		return true; // success
	}

	protected static bool CheckOrdering(string[] list) {
		var other = new string[list.Length];
		Array.Copy(list, 0, other, 0, list.Length);
		Array.Sort(other);
		
		for (var i = 0; i < list.Length; i++) {
			list[i].Should().Be(other[i]);
		}
		return true; // success
	}

	protected static bool CheckMaximumPerMs(long[] list, int max) {
		var dict = new Dictionary<long, List<long>>();

		foreach (var tsid in list) {
			var key = Tsid.From(tsid).GetTime();
			if (!dict.ContainsKey(key)) {
				dict.Add(key, new List<long>());
			}

			dict[key].Add(tsid);
			var size = dict[key].Count;


			var notTooManyTsiDsPerMillisecond = size <= max;
			notTooManyTsiDsPerMillisecond.Should().BeTrue();
		}

		return true; // success
	}

	protected bool CheckMaximumPerMs(string[] list, int max) {
		var dict = new Dictionary<long, List<string>>();

		foreach (var tsid in list) {
			var key = Tsid.From(tsid).GetTime();
			if (!dict.ContainsKey(key)) {
				dict.Add(key, new List<string>());
			}

			dict[key].Add(tsid);
			var size = dict[key].Count;


			var notTooManyTsiDsPerMillisecond = size <= max;
			notTooManyTsiDsPerMillisecond.Should().BeTrue();
		}

		return true; // success
	}

	protected static bool CheckCreationTime(long[] list, long startTime, long endTime) {

		(startTime <= endTime).Should().BeTrue();

		foreach (var tsid in list) {
			var creationTime = Tsid.From(tsid).GetDateTimeOffset().ToUnixTimeMilliseconds();
			(creationTime >= startTime).Should().BeTrue();
			(creationTime <= endTime + LoopMax).Should().BeTrue();
		}
		return true; // success
	}

	protected static bool CheckCreationTime(string[] list, long startTime, long endTime) {

		(startTime <= endTime).Should().BeTrue();

		foreach (var tsid in list) {
			var creationTime = Tsid.From(tsid).GetDateTimeOffset().ToUnixTimeMilliseconds();
			(creationTime >= startTime).Should().BeTrue();
			(creationTime <= endTime + LoopMax).Should().BeTrue();
		}
		return true; // success
	}

	private static int AvailableProcessors() {
		var processors = Environment.ProcessorCount;
		if (processors < 4) {
			processors = 4;
		}
		return processors;
	}
}
