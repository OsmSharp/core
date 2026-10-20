#nullable enable
using System.Threading.Tasks;

namespace OsmSharp.Replication;

/// <summary>
/// A replication changeset enumerator.
/// </summary>
/// <remarks>
/// Enumerates all diff until it reaches the latest diff. When this MoveNext is called when the replication at the latest false is returned.
/// </remarks>
public class ReplicationDiffEnumerator : IReplicationDiffEnumerator
{
    internal ReplicationDiffEnumerator(ReplicationConfig config)
    {
        this.Config = config;
        _lastReturned = -1;
    }

    private long _lastReturned;
    private long _highestLatest = -1;

    /// <summary>
    /// Moves this enumerator to the given sequence number.
    /// </summary>
    /// <param name="sequenceNumber">The sequence number.</param>
    /// <returns>True if the move was a success, false otherwise. Throw an exception on anything but a 404 from the server.</returns>
    internal async Task<bool> MoveTo(long sequenceNumber)
    {
        var state = await this.Config.GetReplicationState(sequenceNumber);
        if (state == null) return false;

        if (_highestLatest < 0 ||
            _lastReturned == _highestLatest ||
            state.SequenceNumber >= _highestLatest)
        {
            // make sure the latest is up to date.
            var latest = await this.Config.LatestReplicationState();
            _highestLatest = latest.SequenceNumber;
        }

        _lastReturned = sequenceNumber;
        this.State = state;


        this.CurrentIsLatest = (_lastReturned == _highestLatest);

        return true;
    }

    /// <summary>
    /// Moves to the next diff, returns true when it's available.
    /// </summary>
    /// <returns>True when there is new data, false when latest was reached before.</returns>
    public async Task<bool> MoveNext()
    {
        if (_highestLatest < 0 ||
            _lastReturned == _highestLatest)
        {
            // make sure the latest is up to date.
            var latest = await this.Config.LatestReplicationState();
            _highestLatest = latest.SequenceNumber;
        }

        if (_lastReturned < 0)
        { // start from the latest.
            _lastReturned = _highestLatest;
        }
        else
        {
            // there is a sequence number, try to increase.
            var next = _lastReturned + 1;

            if (next > _highestLatest)
            {
                return false; // next is higher, latest has been reached.
            }

            _lastReturned = next;
        }

        // download all the things.
        var state = await this.Config.GetReplicationState(_lastReturned);
        if (state == null) return false;   // nothing published there, so nothing to move to

        this.State = state;
        this.CurrentIsLatest = (_lastReturned == _highestLatest);
        return true;
    }

    /// <summary>
    /// Gets the replication config.
    /// </summary>
    public ReplicationConfig Config { get; }

    /// <summary>
    /// Gets the replication state.
    /// </summary>
    /// <summary>
    /// The state of the diff this enumerator is on, once it has moved onto one.
    /// </summary>
    /// <remarks>
    /// Only valid once a move has succeeded: an enumerator that has not moved is not on a
    /// diff. Declared non-null because a move returning true is what guarantees it, and saying
    /// so once here beats a null check at every use — annotating it nullable put the
    /// suppression at a dozen call sites instead.
    /// </remarks>
    public ReplicationState State { get; private set; } = null!;

    /// <summary>
    /// Returns true if the current state is the latest.
    /// </summary>
    /// <remarks>
    /// This reflects the state at the time of move to or move next, not at the time this property is accessed.
    /// </remarks>
    public bool CurrentIsLatest { get; private set; }
}
