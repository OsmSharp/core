#nullable enable
using System;

namespace OsmSharp.Replication;

/// <summary>
/// Keeps replication state.
/// </summary>
public class ReplicationState
{
    internal static string SequenceNumberKey = "sequenceNumber";
    internal static string TimestampKey = "timestamp";

    /// <summary>
    /// Creates a new replication state.
    /// </summary>
    /// <param name="config">The replication config.</param>
    /// <param name="sequenceNumber">The sequence number.</param>
    /// <param name="endTimestamp">The timestamp.</param>
    internal ReplicationState(ReplicationConfig config, long sequenceNumber, DateTime endTimestamp)
    {
        this.Config = config;
        this.SequenceNumber = sequenceNumber;
        this.EndTimestamp = endTimestamp;
    }

    /// <summary>
    /// Gets the replication config.
    /// </summary>
    public ReplicationConfig Config { get; }

    /// <summary>
    /// Gets the sequence number.
    /// </summary>
    public long SequenceNumber { get; }

    /// <summary>
    /// Gets the start timestamp (included).
    /// </summary>
    public DateTime StartTimestamp => this.EndTimestamp.AddSeconds(-this.Config.Period);

    /// <summary>
    /// Gets the end timestamp (excluded).
    /// </summary>
    public DateTime EndTimestamp { get; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{this.SequenceNumber} @ [{this.StartTimestamp}->{this.EndTimestamp}[ UTC for {this.Config}";
    }
}
