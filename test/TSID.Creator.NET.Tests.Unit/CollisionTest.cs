using System.Collections.Concurrent;
using TSID.Creator.NET.Tests.Unit.Extensions;

namespace TSID.Creator.NET.Tests.Unit;

public class CollisionTest
{
    private static TsidFactory NewFactory(int nodeBits)
    {
        return TsidFactory.GetBuilder().WithRandomFunction(StaticRandom.Rand)
            .WithNodeBits(nodeBits) // 8 bits: 256 nodes; 10 bits: 1024 nodes...
            .Build();
    }

    [Fact]
    public void TestCollision()
    {
        int nodeBits = 8;
        int threadCount = 16;
        int iterationCount = 100_000;

        int clashes = 0;
        CountDownLatch endLatch = new CountDownLatch(threadCount);
        var tsidMap = new ConcurrentDictionary<long, int>();

        // one generator shared by ALL THREADS
        TsidFactory factory = NewFactory(nodeBits);

        for (int i = 0; i < threadCount; i++)
        {
            int threadId = i;

            new Thread(() =>
            {
                for (int j = 0; j < iterationCount; j++)
                {
                    long tsid = factory.Create().ToLong();
                    if (tsidMap.ContainsKey(tsid))
                    {
                        Interlocked.Increment(ref clashes);
                        break;
                    }
                    var success = tsidMap.TryAdd(
                        tsid,
                        threadId * iterationCount + j
                    );
                    if (!success)
                    {
                        Interlocked.Increment(ref clashes);
                        break;
                    }
                }

                endLatch.countDown();
            }).Start();
        }

        endLatch.await();

        // assertFalse("Collisions detected!", clashes.intValue() != 0);
        clashes.Should().Be(0);
    }
}