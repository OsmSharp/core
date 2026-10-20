#nullable enable
using System.Threading.Tasks;
using OsmSharp.Changesets;

namespace OsmSharp.Replication
{
    /// <summary>
    /// Abstract representation of a replication diff enumerator.
    /// </summary>
    public interface IReplicationDiffEnumerator
    {
        /// <summary>
        /// Moves to the next diff, returns true when it's available.
        /// </summary>
        /// <returns></returns>
        Task<bool> MoveNext();

        /// <summary>
        /// Gets the replication state.
        /// </summary>
        ReplicationState State { get; }

        /// <summary>
        /// Gets the replication config.
        /// </summary>
        ReplicationConfig Config { get; }
    }
}