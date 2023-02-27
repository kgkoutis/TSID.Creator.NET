using System.Diagnostics;

namespace TSID.Creator.NET;

///<summary>
/// A factory that actually generates Time-Sorted Unique Identifiers (TSID).
/// <para />
/// This factory is used by the <see cref="TsidCreator"/> utility.
/// <para />
/// Most people just need <see cref="TsidCreator"/>. However, you can use this class if
/// you need to make some customizations, for example changing the default
/// <see cref="System.Security.Cryptography.RandomNumberGenerator"/> random generator to a faster pseudo-random generator.
/// Make sure to read the documentation of <see cref="RandomGenerators"/> to learn how to do that.
/// <para />
/// If a system property "tsidcreator.node" or environment variable
/// "TSIDCREATOR_NODE" is defined, its value is utilized as node identifier. One
/// of them <b>should</b> be defined to embed a machine ID in the generated TSID
/// in order to avoid TSID collisions. Using that property or variable is
/// <b>highly recommended</b>. If no property or variable is defined, a random
/// node ID is generated at initialization.
/// <para />
/// If a system property "tsidcreator.node.count" or environment variable
/// "TSIDCREATOR_NODE_COUNT" is defined, its value is utilized by the
/// constructors of this class to adjust the amount of bits needed to embed the
/// node ID. For example, if the number 50 is given, the node bit amount is
/// adjusted to 6, which is the minimum number of bits to accommodate 50 nodes.
/// If no property or variable is defined, the number of bits reserved for node
/// ID is set to 10, which can accommodate 1024 nodes.
/// <para />
/// This class <b>should</b> be used as a singleton. Make sure that you create
/// and reuse a single instance of <see cref="TsidFactory"/> per node in your
/// distributed system.
/// <para />
///</summary>
public sealed class TsidFactory
{
    private int _counter;
    private long _lastTime;

    private readonly int _node;

    private readonly int _nodeBits;
    private readonly int _counterBits;

    private readonly int _nodeMask;
    private readonly int _counterMask;

    private readonly Clock _clock;
    private readonly long _customEpoch;

    private readonly IRandom _random;
    private readonly int _randomBytes;

    internal const int NodeBits256 = 8;
    internal const int NodeBits1024 = 10;
    internal const int NodeBits4096 = 12;

    private readonly object _lock = new object();
    private readonly object _lock2 = new object();

    // ******************************
    // Constructors
    // ******************************


    /// <summary>
    /// It builds a new factory.
    /// <para />
    /// The node identifier provided by the "tsidcreator.node" system property or the
    /// "TSIDCREATOR_NODE" environment variable is embedded in the generated TSIDs in
    /// order to avoid collisions. It is <b>highly recommended</b> defining that
    /// property or variable. Otherwise the node identifier will be randomly chosen.
    /// <para />
    /// If a system property "tsidcreator.node.count" or environment variable
    /// "TSIDCREATOR_NODE_COUNT" is defined, its value is used to adjust the node
    /// bits amount.
    ///	</summary>
    public TsidFactory() : this(GetBuilder())
    {
    }

    /// <summary>
    /// It builds a new factory.
    ///<para>
    /// The node identifier provided by parameter is embedded in the generated TSIDs
    /// in order to avoid collisions.
    /// </para>
    /// If a system property "tsidcreator.node.count" or environment variable
    /// "TSIDCREATOR_NODE_COUNT" is defined, its value is used to adjust the node
    /// bits amount.
    /// <param name="node"> the node identifier </param>
    /// </summary>
    public TsidFactory(int node) : this(GetBuilder().WithNode(node))
    {
    }

    /// <summary>
    /// It builds a generator with the given builder.
    /// <param name="builder"> a <see cref="Builder"/> instance </param>
    /// </summary>
    private TsidFactory(Builder builder)
    {
        // setup node bits, custom epoch and random function
        _customEpoch = builder.GetCustomEpoch();
        _nodeBits = builder.GetNodeBits();
        _random = builder.GetRandom();
        _clock = builder.GetClock();

        // setup constants that depend on node bits
        _counterBits = Tsid.RandomBits - _nodeBits;
        _counterMask = Tsid.RandomMask >>> _nodeBits;
        _nodeMask = Tsid.RandomMask >>> _counterBits;

        // setup how many bytes to get from the random function
        _randomBytes = ((_counterBits - 1) / 8) + 1;

        // setup the node identifier
        _node = builder.GetNode() & _nodeMask;

        // finally, initialize internal state
        _lastTime = _clock.FrozenMilliSeconds ?? 
                    DateTimeOffset.UtcNow.ToOffset(_clock.TimeZoneInfo.BaseUtcOffset).ToUnixTimeMilliseconds();
        
        _counter = GetRandomCounter();
    }

    /// <summary>
    /// Returns a new factory for up to 256 nodes and 16384 ID/ms.
    ///
    /// It is equivalent to <code>new TsidFactory()</code>
    /// <returns> a <see cref="TsidFactory"/> </returns>
    /// </summary>
    public static TsidFactory NewInstance256()
    {
        return GetBuilder().WithNodeBits(NodeBits256).Build();
    }

    /// <summary>
    /// Returns a new factory for up to 256 nodes and 16384 ID/ms.
    /// 
    /// It is equivalent to <code>new TsidFactory()</code>
    /// <param name="node"> the node identifier </param>
    /// <returns> a <see cref="TsidFactory"/> </returns>
    /// </summary>
    public static TsidFactory NewInstance256(int node)
    {
        return GetBuilder().WithNodeBits(NodeBits256).WithNode(node).Build();
    }

    /// <summary>
    /// Returns a new factory for up to 1024 nodes and 4096 ID/ms.
    ///
    /// It is equivalent to <code>new TsidFactory()</code>
    /// <param name="node"> the node identifier </param>
    /// <returns> a <see cref="TsidFactory"/> </returns>
    /// </summary>
    public static TsidFactory NewInstance1024()
    {
        return GetBuilder().WithNodeBits(NodeBits1024).Build();
    }

    /// <summary>
    /// Returns a new factory for up to 1024 nodes and 4096 ID/ms.
    /// It is equivalent to <code>new TsidFactory(int)</code>
    /// <param name="node"> the node identifier </param>
    /// <returns> a <see cref="TsidFactory"/> </returns>
    /// </summary>
    public static TsidFactory NewInstance1024(int node)
    {
        return GetBuilder().WithNodeBits(NodeBits1024).WithNode(node).Build();
    }

    /// <summary>
    /// Returns a new factory for up to 4096 nodes and 1024 ID/ms.
    /// <returns> a <see cref="TsidFactory"/> </returns>
    /// </summary>
    public static TsidFactory NewInstance4096()
    {
        return GetBuilder().WithNodeBits(NodeBits4096).Build();
    }

    /// <summary>
    /// Returns a new factory for up to 4096 nodes and 1024 ID/ms.
    ///
    /// <param name="node"> the node identifier </param>
    /// <returns> a <see cref="TsidFactory"/> </returns>
    /// </summary>
    public static TsidFactory NewInstance4096(int node)
    {
        return GetBuilder().WithNodeBits(NodeBits4096).WithNode(node).Build();
    }

    // ******************************
    // Public methods
    // ******************************

    /// <summary>
    /// Returns a Tsid.
    /// <returns> a <see cref="Tsid" /> </returns>
    /// </summary>
    public Tsid Create()
    {
        lock (_lock)
        {
            var time = GetTime() << Tsid.RandomBits;
            var node = (long)_node << _counterBits;
            var counter = (long)_counter & _counterMask;

            var number = time | node | counter;
            // Debug.Assert(number >= 0, "number is negative: " + number);
            return new Tsid(number);
        }
    }

    /// <summary>
    /// <para>
    /// Returns the current time.
    /// </para>
    /// <para>
    /// If the current time is equal to the previous time, the counter is incremented
    /// by one. Otherwise the counter is reset to a random value.
    /// </para>
    /// <para>
    /// The maximum number of increment operations depend on the counter bits. For
    /// example, if the counter bits is 12, the maximum number of increment
    /// operations is 2^12 = 4096.
    /// </para>
    /// <returns> the current time </returns>
    /// </summary>
    private long GetTime()
    {
        var time  = _clock.FrozenMilliSeconds ?? 
                    DateTimeOffset.UtcNow.ToOffset(_clock.TimeZoneInfo.BaseUtcOffset).ToUnixTimeMilliseconds();

        if (time <= _lastTime)
        {
            _counter++;
            // Carry is 1 if an overflow occurs after ++.
            var carry = _counter >>> _counterBits;
            _counter = _counter & _counterMask;
            time = _lastTime + carry; // increment time
        }
        else
        {
            // If the system clock has advanced as expected,
            // simply reset the counter to a new random value.
            _counter = GetRandomCounter();
        }

        // save current time
        _lastTime = time;

        // adjust to the custom epoch
        return time - _customEpoch;
    }
    
    /// <summary>
    /// <para>
    /// Returns a random counter value from 0 to 0x3fffff (2^22-1 = 4,194,303).
    /// </para>
    /// <para>
    /// The counter maximum value depends on the node identifier bits. For example,
    /// if the node identifier has 10 bits, the counter has 12 bits.
    /// </para>
    /// <returns> a number </returns>
    /// </summary>
    private int GetRandomCounter()
    {
        lock (_lock2)
        {
            if (_random is ByteRandom)
            {
                var bytes = _random.NextBytes(_randomBytes);

                switch (bytes.Length)
                {
                    case 1:
                        return (bytes[0] & 0xff) & _counterMask;
                    case 2:
                        return (((bytes[0] & 0xff) << 8) | (bytes[1] & 0xff)) & _counterMask;
                    default:
                        return (((bytes[0] & 0xff) << 16) | ((bytes[1] & 0xff) << 8) | (bytes[2] & 0xff)) &
                               _counterMask;
                }
            }
            else
            {
                return _random.NextInt() & _counterMask;
            }
        }
    }
    
    /// <summary>
    /// Returns a <see cref="Builder"/> object.
    /// <para>
    /// It is used to build a custom <see cref="TsidFactory"/>.
    /// </para>
    /// </summary>
    public static Builder GetBuilder()
    {
        return new Builder();
    }

    // ******************************
    // Package-private inner classes
    // ******************************
    
    /// <summary>
    /// <para>
    /// A nested class that builds custom Tsid factories.
    /// </para>
    /// <para>
    /// It is used to setup a custom <see cref="TsidFactory"/>.
    /// </para>
    /// </summary>
    public class Builder
    {
        private int? _node;
        private int? _nodeBits;
        private long? _customEpoch;
        private IRandom _random;
        private Clock _clock;

        public Builder WithNode(int node)
        {
            _node = node;
            return this;
        }

        public Builder WithNodeBits(int nodeBits)
        {
            _nodeBits = nodeBits;
            return this;
        }

        public Builder WithCustomEpoch(DateTimeOffset customEpoch)
        {
            _customEpoch = customEpoch.ToUnixTimeMilliseconds();
            return this;
        }

        public Builder WithRandom(RandomGenerators randoms)
        {
            if (randoms != null)
            {
                if (randoms.IsCryptographicallySecure())
                {
                    _random = new ByteRandom(randoms);
                }
                else
                {
                    _random = new IntRandom(randoms);
                }
            }

            return this;
        }

        public Builder WithRandomFunction(Func<int> randomFunction)
        {
            _random = new IntRandom(randomFunction);
            return this;
        }

        public Builder WithRandomFunction(Func<int, byte[]> randomFunction)
        {
            _random = new ByteRandom(randomFunction);
            return this;
        }

        public Builder WithClock(Clock clock)
        {
            _clock = clock;
            return this;
        }

        public int GetNode()
        {
            var max = (1 << _nodeBits) - 1;

            if (_node == null)
            {
                if (Settings.GetNode() != null)
                {
                    // use property or variable
                    _node = Settings.GetNode();
                }
                else
                {
                    // use random node identifier
                    _node = _random.NextInt() & max;
                }
            }

            if (_node < 0 || _node > max)
            {
                throw new ArgumentException($"Node ID out of range [0, {max}]: {_node}");
            }

            return (int)_node;
        }

        /// <summary>
        ///  Get the node identifier bits length within the range 0 to 20.
        /// <returns> a number </returns>
        /// <exception cref="ArgumentException"> throws if the node bits are out of range </exception>
        /// </summary>
        public int GetNodeBits()
        {
            if (_nodeBits == null)
            {
                if (Settings.GetNodeCount() != null)
                {
                    // use property or variable
                    _nodeBits = (int)Math.Ceiling(Math.Log((double)Settings.GetNodeCount()) / Math.Log(2));
                }
                else
                {
                    // use default bit length: 10 bits
                    _nodeBits = NodeBits1024;
                }
            }

            if (_nodeBits < 0 || _nodeBits > 20)
            {
                throw new ArgumentException($"Node bits out of range [0, 20]: {_nodeBits}");
            }

            return (int)_nodeBits;
        }

        /// <summary>
        /// Gets the custom epoch.
        /// <returns> a number </returns>
        /// </summary>
        public long GetCustomEpoch()
        {
            if (_customEpoch == null)
            {
                _customEpoch = Tsid.TsidEpoch; // 2020-01-01
            }

            return (long)_customEpoch;
        }
        
        /// <summary>
        /// Gets the random generator.
        /// <returns> a random generator </returns>
        /// </summary>
        public IRandom GetRandom()
        {
            if (_random == null)
            {
                WithRandom(RandomGenerators.OfCryptographicallySecureRandom());
            }

            return _random;
        }

        /// <summary>
        /// Gets the clock to be used in tests.
        /// <returns> a clock of type <see cref="Clock"/> </returns>
        /// </summary>
        public Clock GetClock()
        {
            if (_clock == null)
            {
                WithClock(new Clock(TimeZoneInfo.Utc));
            }

            return _clock;
        }

        /// <summary>
        /// Returns a build TSID factory.
        /// <returns> a <see cref="TsidFactory"/> </returns>
        /// <exception cref="ArgumentException"> throws if the node is out of range </exception>
        /// <exception cref="ArgumentException"> throws if if the node is out of range</exception>
        /// </summary>
        public TsidFactory Build()
        {
            return new TsidFactory(this);
        }
    }

    public interface IRandom
    {
        public int NextInt();

        public byte[] NextBytes(int length);
    }

    internal class IntRandom : IRandom
    {
        private readonly Func<int> _randomFunction;

        public IntRandom() : this((Func<int>)null)
        {
        }

        public IntRandom(RandomGenerators random) : this(NewRandomFunction(random))
        {
        }

        public IntRandom(Func<int> randomFunction)
        {
            _randomFunction = randomFunction ?? NewRandomFunction(null);
        }

        public int NextInt()
        {
            return _randomFunction();
        }

        public byte[] NextBytes(int length)
        {
            var shift = 0;
            var random = 0;
            var bytes = new byte[length];

            for (var i = 0; i < length; i++)
            {
                if (shift < 8) // Byte.SIZE
                {
                    shift = 32; // Integer.SIZE
                    random = _randomFunction();
                }

                shift -= 8; // Byte.SIZE
                bytes[i] = (byte)(random >> shift);
            }

            return bytes;
        }

        private static Func<int> NewRandomFunction(RandomGenerators randoms)
        {
            var entropy = randoms ?? RandomGenerators.OfCryptographicallySecureRandom();
            return () => entropy.NextInt();
        }
    }

    internal class ByteRandom : IRandom
    {
        private readonly Func<int, byte[]> _randomFunction;

        public ByteRandom() : this(NewRandomFunction(null))
        {
        }

        public ByteRandom(RandomGenerators randoms) : this(NewRandomFunction(randoms))
        {
        }

        public ByteRandom(Func<int, byte[]> randomFunction)
        {
            _randomFunction = randomFunction ?? NewRandomFunction(null);
        }

        public int NextInt()
        {
            var number = 0;
            var bytes = _randomFunction.Invoke(4);
            for (var i = 0; i < 4; i++) // Integer.SIZE
            {
                number = (number << 8) | (bytes[i] & 0xff);
            }

            return number;
        }

        public byte[] NextBytes(int length)
        {
            return _randomFunction.Invoke(length);
        }

        private static Func<int, byte[]> NewRandomFunction(RandomGenerators randoms)
        {
            var entropy = randoms ?? RandomGenerators.OfCryptographicallySecureRandom();
            return length => entropy.NextBytes(length);
        }
    }

    internal static class Settings
    {
        public const string Node = "tsidcreator.node";
        public const string NodeCount = "tsidcreator.node.count";
        
        public static int? GetNode()
        {
            return GetPropertyAsInteger(Node);
        }

        public static int? GetNodeCount()
        {
            return GetPropertyAsInteger(NodeCount);
        }

        private static int? GetPropertyAsInteger(string property)
        {
            try
            {
                return int.Parse(GetProperty(property));
            }
            catch (FormatException e)
            {
                return null;
            }
            catch (ArgumentNullException e)
            {
                return null;
            }
        }

        private static string GetProperty(string name)
        {
            var property = Environment.GetEnvironmentVariable(name.ToUpper().Replace(".", "_"));
            if (!string.IsNullOrEmpty(property))
            {
                return property;
            }

            property = Environment.GetEnvironmentVariable(name);
            if (!string.IsNullOrEmpty(property))
            {
                return property;
            }

            return null;
        }
    }
}