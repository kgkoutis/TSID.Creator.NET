namespace TSID.Creator.NET.Tests.Unit;

public abstract class TsidFactory00000Test {
	protected const int TSID_LENGTH = 13;

	protected static readonly int LOOP_MAX = 10_000;

	protected static RandomGenerators random = RandomGenerators.OfSimpleRandomNumberGenerator();

	protected static readonly int MultiplePasses = AvailableProcessors();

	protected bool CheckNullOrInvalid(long[] list) {
		foreach (var tsid in list) {
			tsid.Should().NotBe(0);
		}
		return true; // success
	}

	protected bool CheckNullOrInvalid(string[] list) {
		foreach (var tsid in list) {
			tsid.Should().NotBeNull();
			tsid.Should().NotBeEmpty();
			tsid.Should().NotBeNullOrWhiteSpace();
			tsid.Length.Should().Be(TSID_LENGTH);
			Tsid.IsValid(tsid).Should().BeTrue();
		}
		return true; // success
	}

	protected bool CheckUniqueness(long[] list) {

		HashSet<long> set = new HashSet<long>();

		foreach (var tsid in list)
		{
			set.Add(tsid).Should().BeTrue();
		}

		set.Count.Should().Be(list.Length);
		return true; // success
	}

	protected bool CheckUniqueness(string[] list) {

		HashSet<string> set = new HashSet<string>();

		foreach (var tsid in list) {
			set.Add(tsid).Should().BeTrue();
		}

		set.Count.Should().Be(list.Length);
		return true; // success
	}

	protected bool CheckOrdering(long[] list) {
		// copy the list
		var other = new long[list.Length];
		Array.Copy(list, 0, other, 0, list.Length);
		Array.Sort(other);
		
		for (var i = 0; i < list.Length; i++) {
			list[i].Should().Be(other[i]);
		}
		return true; // success
	}

	protected bool CheckOrdering(string[] list) {
		var other = new string[list.Length];
		Array.Copy(list, 0, other, 0, list.Length);
		Array.Sort(other);
		
		for (var i = 0; i < list.Length; i++) {
			list[i].Should().Be(other[i]);
		}
		return true; // success
	}

	protected bool CheckMaximumPerMs(long[] list, int max) {
		var dict = new Dictionary<long, List<long>>();

		foreach (long tsid in list) {
			var key = Tsid.From(tsid).GetTime();
			if (!dict.ContainsKey(key)) {
				dict.Add(key, new List<long>());
			}

			dict[key].Add(tsid);
			int size = dict[key].Count;


			var notTooManyTsiDsPerMillisecond = size <= max;
			notTooManyTsiDsPerMillisecond.Should().BeTrue();
		}

		return true; // success
	}

	protected bool CheckMaximumPerMs(string[] list, int max) {
		var dict = new Dictionary<long, List<string>>();

		foreach (string tsid in list) {
			var key = Tsid.From(tsid).GetTime();
			if (!dict.ContainsKey(key)) {
				dict.Add(key, new List<string>());
			}

			dict[key].Add(tsid);
			int size = dict[key].Count;


			var notTooManyTsiDsPerMillisecond = size <= max;
			notTooManyTsiDsPerMillisecond.Should().BeTrue();
		}

		return true; // success
	}

	protected bool CheckCreationTime(long[] list, long startTime, long endTime) {

		(startTime <= endTime).Should().BeTrue();

		foreach (long tsid in list) {
			long creationTime = Tsid.From(tsid).GetDateTimeOffset().ToUnixTimeMilliseconds();
			(creationTime >= startTime).Should().BeTrue();
			(creationTime <= endTime + LOOP_MAX).Should().BeTrue();
		}
		return true; // success
	}

	protected bool CheckCreationTime(string[] list, long startTime, long endTime) {

		(startTime <= endTime).Should().BeTrue();

		foreach (var tsid in list) {
			long creationTime = Tsid.From(tsid).GetDateTimeOffset().ToUnixTimeMilliseconds();
			(creationTime >= startTime).Should().BeTrue();
			(creationTime <= endTime + LOOP_MAX).Should().BeTrue();
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
