#nullable enable
using System.Threading.Tasks;
using OsmSharp.Changesets;

namespace OsmSharp.Replication
{
    /// <summary>
    /// Contains extension methods for the replication diff enumerator.
    /// </summary>
    public static class IReplicationDiffEnumeratorExtensions
    {
        /// <summary>
        /// Downloads the diff based on the current state of the enumerator.
        /// </summary>
        /// <param name="enumerator">The enumerator.</param>
        /// <returns>The diff.</returns>
        public static async Task<OsmChange?> Diff(this IReplicationDiffEnumerator enumerator)
        {
            return await enumerator.Config.DownloadDiff(enumerator.State.SequenceNumber);
        }
    }
}