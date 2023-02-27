using System.Collections.Concurrent;
using Apache.NMS.Util;

namespace TSID.Creator.NET.Tests.Integration.uniq;

/// <summary>
/// <p> This may take a long time to finish </p>
/// </summary>
public class UniquenessTest
{
    private readonly int _tasksCount;
    private readonly int _requestCount;

    private readonly ConcurrentDictionary<long, byte> _set = new();

    private readonly bool _verbose;

    /// <summary>
    /// <p> Initialize the test. </p>
    /// <param name="tasksCount">Number of tasks to run in parallel</param>
    /// <param name="requestCount">Number of requests for task</param>
    /// <param name="verbose">Show progress or not</param>
    /// </summary>
    private UniquenessTest(int tasksCount, int requestCount, bool verbose)
    {
        _tasksCount = tasksCount;
        _requestCount = requestCount;
        _verbose = verbose;
    }

    /// <summary>
    /// Initialize and start the test.
    /// </summary>
    private void Start()
    {
        // Instantiate and start many threads
        var endLatch = new CountDownLatch(_tasksCount);
        Parallel.For(0, _tasksCount, j =>
        {
            var task = new UniquenessTestRunner(j, _verbose);
            task.Run(_requestCount, _set);
            endLatch.countDown();
        });

        endLatch.await();
    }

    public static void Init()
    {
        const int tasksCount = 1024; // 2^10 (node bit length)
        const int requestCount = 4096; // 2^12 (counter bit length)
        const bool verbose = true;
        var test = new UniquenessTest(tasksCount, requestCount, verbose);
        test.Start();
    }
}

public class UniquenessTestRunner
{
    private readonly int _id;
    private readonly bool _verbose;

    private readonly TsidFactory _factory;

    public UniquenessTestRunner(int id, bool verbose)
    {
        _id = id;
        _verbose = verbose;
        _factory = TsidFactory.GetBuilder()
            .WithNode(id)
            .WithRandom(RandomGenerators.OfSimpleRandomNumberGenerator())
            .Build();
    }

    /// <summary>
    /// <p> Run the test. </p>
    /// <param name="requestCount">Number of requests for task</param>
    /// <param name="set">Set holding the generated ID's</param>
    /// </summary>
    public void Run(int requestCount, ConcurrentDictionary<long, byte> set)
    {
        var progress = 0;

        for (var i = 0; i < requestCount; i++)
        {
            // Request a TSID
            var tsid = _factory.Create().ToLong();

            if (_verbose && i % (requestCount / 100) == 0)
            {
                // Calculate and show progress
                progress = (int)(i * 1.0 / requestCount * 100);
                Console.WriteLine(string.Format($"[Thread {_id:000000}] {tsid} {i} {progress}"));
            }

            // Insert the value in cache, if it does not exist in it.
            if (!set.TryAdd(tsid, byte.MinValue))
            {
                Console.WriteLine(string.Format($"[Thread {_id:000000}] {tsid} {i} {progress} [DUPLICATE]"));
            }
        }

        if (_verbose)
        {
            // Finished
            Console.WriteLine(string.Format($"[Thread {_id:000000}] Done."));
        }
    }
}