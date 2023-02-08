namespace TSID.Creator.NET;

/// <summary>
/// <para>
/// A utility that generates Time-Sorted Unique Identifiers (TSID).
/// </para>
/// <para>
/// It is <b>highly recommended</b> to use this class in conjunction with the
/// "tsidcreator.node" system property or the "TSIDCREATOR_NODE" environment
/// variable. This is a simple way to avoid collisions between identifiers
/// produced by more than one machine or application instance.
/// </para>
/// </summary>
public static class TsidCreator
{
    /// <summary>
    /// Returns a new TSID.
    /// <para>
    /// The node ID is is set by defining the system property "tsidcreator.node" or
    /// the environment variable "TSIDCREATOR_NODE". One of them <b>should</b> be
    /// used to embed a machine ID in the generated TSID in order to avoid TSID
    /// collisions. If that property or variable is not defined, the node ID is
    /// chosen randomly.
    /// </para>
    /// <para>
    /// The amount of nodes can be set by defining the system property
    /// "tsidcreator.node.count" or the environment variable
    /// "TSIDCREATOR_NODE_COUNT". That property or variable is used to adjust the
    /// minimum amount of bits to accommodate the node ID. If that property or
    /// variable is not defined, the default amount of nodes is 1024, which takes 10
    /// bits.
    /// </para>
    /// The amount of bits needed to accommodate the node ID is calculated by this
    /// pseudo-code formula: {@code node_bits = ceil(log(node_count)/log(2))}.
    /// <para>
    /// Random component settings:
    /// <ul>
    /// <li>Node bits: node_bits</li>
    /// <li>Counter bits: 22-node_bits</li>
    /// <li>Maximum node: 2^node_bits</li>
    /// <li>Maximum counter: 2^(22-node_bits)</li>
    /// </ul>
    /// </para>
    /// <para>
    /// The time component can be 1 ms or more ahead of the system time when
    /// necessary to maintain monotonicity and generation speed.
    /// </para>
    /// <returns>a <see cref="Tsid"/></returns>
    /// </summary>
    public static Tsid GetTsid() => FactoryHolder.Instance.Create();

    /// <summary>
    /// Returns a new TSID.
    /// <para>
    /// It supports up to 256 nodes.
    /// </para>
    /// It can generate up to 16,384 TSIDs per millisecond per node.
    /// <para>
    /// The node ID is is set by defining the system property "tsidcreator.node" or
    /// the environment variable "TSIDCREATOR_NODE". One of them <b>should</b> be
    /// used to embed a machine ID in the generated TSID in order to avoid TSID
    /// collisions. If that property or variable is not defined, the node ID is
    /// chosen randomly.
    /// </para>
    /// <para>
    /// Random component settings:
    /// <ul>
    /// <li>Node bits: 8 </li>
    /// <li>Counter bits: 14 </li>
    /// <li>Maximum node: 256 (2^8) </li>
    /// <li>Maximum counter: 16,384 (2^14) </li>
    /// </ul>
    /// </para>
    /// <para>
    /// The time component can be 1 ms or more ahead of the system time when
    /// necessary to maintain monotonicity and generation speed.
    /// </para>
    /// <returns>a <see cref="Tsid"/></returns>
    /// </summary>
    public static Tsid GetTsid256() => Factory256Holder.Instance.Create();

    /// <summary>
    /// <para>
    /// Returns a new TSID.
    /// </para>
    /// <para>
    /// It supports up to 1,024 nodes.
    /// </para>
    /// <para>
    /// It can generate up to 4,096 TSIDs per millisecond per node.
    /// </para>
    /// <para>
    /// The node ID is is set by defining the system property "tsidcreator.node" or
    /// the environment variable "TSIDCREATOR_NODE". One of them <b>should</b> be
    /// used to embed a machine ID in the generated TSID in order to avoid TSID
    /// collisions. If that property or variable is not defined, the node ID is
    /// chosen randomly.
    /// </para>
    /// <para>
    /// Random component settings:
    /// <ul>
    /// <li>Node bits: 10 </li>
    /// <li>Counter bits: 12 </li>
    /// <li>Maximum node: 1,024 (2^10) </li>
    /// <li>Maximum counter: 4,096 (2^12) </li>
    /// </ul>
    /// </para>
    /// <para>
    /// The time component can be 1 ms or more ahead of the system time when
    /// necessary to maintain monotonicity and generation speed.
    /// </para>
    /// <returns>a <see cref="Tsid"/></returns>
    /// </summary>
    public static Tsid GetTsid1024() => Factory1024Holder.Instance.Create();

    /// <summary>
    /// <para>
    /// Returns a new TSID.
    /// </para>
    /// <para>
    /// It supports up to 4,096 nodes.
    /// </para>
    /// <para>
    /// It can generate up to 1,024 TSIDs per millisecond per node.
    /// </para>
    /// <para>
    /// The node ID is is set by defining the system property "tsidcreator.node" or
    /// the environment variable "TSIDCREATOR_NODE". One of them <b>should</b> be
    /// used to embed a machine ID in the generated TSID in order to avoid TSID
    /// collisions. If that property or variable is not defined, the node ID is
    /// chosen randomly.
    /// </para>
    /// <para>
    /// Random component settings:
    /// <ul>
    /// <li>Node bits: 12 </li>
    /// <li>Counter bits: 10 </li>
    /// <li>Maximum node: 4,096 (2^12) </li>
    /// <li>Maximum counter: 1,024 (2^10) </li>
    /// </ul>
    /// </para>
    /// <para>
    /// The time component can be 1 ms or more ahead of the system time when
    /// necessary to maintain monotonicity and generation speed.
    /// </para>
    /// <returns>a <see cref="Tsid"/></returns>
    /// </summary>
    public static Tsid GetTsid4096() => Factory4096Holder.Instance.Create();

    private static class FactoryHolder
    {
        public static readonly TsidFactory Instance = new TsidFactory();
    }

    private static class Factory256Holder
    {
        public static readonly TsidFactory Instance = TsidFactory.NewInstance256();
    }

    private static class Factory1024Holder
    {
        public static readonly TsidFactory Instance = TsidFactory.NewInstance1024();
    }

    private static class Factory4096Holder
    {
        public static readonly TsidFactory Instance = TsidFactory.NewInstance4096();
    }
}