using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using OsmSharp;
using OsmSharp.Complete;
using OsmSharp.Streams;
using OsmSharp.Tags;

namespace Sample.CompleteBuildings;

/// <summary>
/// Reads complete ways (<see cref="OsmSharp.Complete.CompleteWay"/>) from a PBF file
/// in batches bounded by a caller-supplied node budget.
///
/// The trick this sample demonstrates: <see cref="PBFOsmStreamSource"/> accretes an
/// internal block index as blobs get decoded, and preserves it across
/// <see cref="PBFOsmStreamSource.Reset"/>. That lets a single source ping-pong between
/// walking the ways section forward and doing random-access node lookups, all without
/// pre-indexing every node up front.
///
/// Sequence of events for a batch:
/// <list type="number">
///   <item>Advance to the next way past the last one we emitted, via
///     <c>MoveNext(OsmGeoType.Way, lastWayId + 1)</c>. On the very first batch this
///     also warms the block index for the node blobs the source walks past.</item>
///   <item>Accumulate matching ways until the union of their node ids reaches the
///     budget. This is the real memory bound — multiple ways share nodes.</item>
///   <item>Sort the batch's node ids, call <c>source.Reset()</c>, then walk them
///     monotonically with <c>MoveNext(OsmGeoType.Node, sortedId)</c>. The warm block
///     index makes this cheap: Tier-1 skips non-node blobs, Tier-2 skips node blobs
///     whose max id is below the current target.</item>
///   <item>Build <see cref="OsmSharp.Complete.CompleteWay"/> instances from the
///     buffered records + fetched nodes.</item>
///   <item>Discard the batch's state entirely. Loop to next batch.</item>
/// </list>
///
/// Nothing survives between batches except the source itself (and its block index).
/// </summary>
public static class PBFCompleteWayReader
{
    /// <summary>
    /// Enumerates complete ways matching <paramref name="filter"/> from the PBF at
    /// <paramref name="pbfStream"/>. Peak in-flight memory is roughly proportional to
    /// <paramref name="nodeBudget"/>.
    /// </summary>
    /// <param name="pbfStream">A seekable stream over a PBF file. The caller owns disposal.</param>
    /// <param name="filter">Predicate to decide which ways are wanted (e.g. <c>w => w.Tags?.ContainsKey("building") == true</c>).</param>
    /// <param name="nodeBudget">Upper bound on the number of unique node ids collected before
    /// a batch is flushed. Bigger budget = fewer batches, more RAM per batch.</param>
    /// <param name="onBatchFlushed">Optional callback fired once per batch, right before its ways
    /// are yielded. Useful for demonstrating that batching is actually happening.</param>
    public static IEnumerable<CompleteWay> Read(
        Stream pbfStream,
        Func<Way, bool> filter,
        int nodeBudget = 50_000,
        Action<BatchStats>? onBatchFlushed = null)
    {
        if (pbfStream == null) throw new ArgumentNullException(nameof(pbfStream));
        if (filter == null) throw new ArgumentNullException(nameof(filter));
        if (nodeBudget <= 0) throw new ArgumentOutOfRangeException(nameof(nodeBudget));

        var source = new PBFOsmStreamSource(pbfStream);

        // OSM way ids are positive, so starting from 0 finds the first way regardless
        // of what its id happens to be.
        long nextMinId = 0;
        var atEnd = false;
        var batchNumber = 0;

        while (!atEnd)
        {
            // Buffered way records: everything we need to build a CompleteWay after
            // we've fetched the node coords. Way tags and metadata are captured now
            // because the source recycles its Way instance across MoveNext calls.
            var batchWays = new List<BufferedWay>();
            var batchNodeIds = new HashSet<long>();
            var swBatchCollect = Stopwatch.StartNew();

            // 1) Advance to the next way at or after nextMinId. On the first batch,
            //    this walk goes through the entire nodes section; block index Tier-1
            //    for node blobs gets populated as a side effect.
            if (!source.MoveNext(OsmGeoType.Way, nextMinId))
            {
                atEnd = true;
                break;
            }

            // 2) Accumulate matching ways until the node budget is hit or we run out.
            long lastWayId;
            do
            {
                var way = (Way)source.Current();
                lastWayId = way.Id ?? throw new InvalidDataException("Way with null id in PBF");

                if (way.Nodes != null && way.Nodes.Length >= 2 && filter(way))
                {
                    batchWays.Add(new BufferedWay
                    {
                        Id = lastWayId,
                        Version = way.Version,
                        ChangeSetId = way.ChangeSetId,
                        TimeStamp = way.TimeStamp,
                        UserId = way.UserId,
                        UserName = way.UserName,
                        Visible = way.Visible,
                        Tags = CopyTags(way.Tags),
                        NodeIds = way.Nodes,
                    });
                    foreach (var nid in way.Nodes) batchNodeIds.Add(nid);
                }

                if (batchNodeIds.Count >= nodeBudget) break;
            } while (source.MoveNext(OsmGeoType.Way, lastWayId + 1));

            nextMinId = lastWayId + 1;
            swBatchCollect.Stop();

            // Skip flush + emit if this batch happened to contain no matches.
            if (batchWays.Count == 0)
            {
                // Loop's own MoveNext at the top will detect true end-of-ways.
                continue;
            }

            // 3) Sort the batch's node ids and fetch full node records via random-access
            //    lookups into the same source. Reset() rewinds the stream but keeps the
            //    block index warm, so subsequent MoveNext(Node, id) calls hit only the
            //    node blobs whose id ranges overlap this batch.
            //
            //    We keep the full OsmSharp.Node (not just coords) so consumers of the
            //    yielded CompleteWay get real node metadata / tags for the members.
            var swBatchFetch = Stopwatch.StartNew();
            var sortedIds = new long[batchNodeIds.Count];
            batchNodeIds.CopyTo(sortedIds);
            Array.Sort(sortedIds);

            var nodeById = new Dictionary<long, Node>(sortedIds.Length);
            source.Reset();
            var i = 0;
            while (i < sortedIds.Length && source.MoveNext(OsmGeoType.Node, sortedIds[i]))
            {
                var n = (Node)source.Current();
                var currentId = n.Id ?? throw new InvalidDataException("Node with null id in PBF");

                // The source may return a node past our target when the target id
                // simply doesn't exist in the file. Advance the pointer accordingly.
                while (i < sortedIds.Length && sortedIds[i] < currentId) i++;
                if (i >= sortedIds.Length) break;

                if (sortedIds[i] == currentId)
                {
                    // The source doesn't recycle Node instances (each MoveNext returns
                    // a fresh instance via Encoder.DecodeNode), so it's safe to retain.
                    nodeById[currentId] = n;
                    i++;
                }
            }
            swBatchFetch.Stop();

            batchNumber++;
            onBatchFlushed?.Invoke(new BatchStats(
                BatchNumber: batchNumber,
                WayCount: batchWays.Count,
                UniqueNodeIds: batchNodeIds.Count,
                CoordsFetched: nodeById.Count,
                CollectElapsed: swBatchCollect.Elapsed,
                FetchElapsed: swBatchFetch.Elapsed));

            // 4) Build CompleteWay instances. Ways whose nodes aren't all present get
            //    silently skipped — either the PBF is incomplete or a node was filtered
            //    upstream (lifecycle-annotated, etc.).
            foreach (var bw in batchWays)
            {
                var nodes = new Node[bw.NodeIds.Length];
                var complete = true;
                for (var j = 0; j < bw.NodeIds.Length; j++)
                {
                    if (!nodeById.TryGetValue(bw.NodeIds[j], out var node))
                    {
                        complete = false;
                        break;
                    }
                    nodes[j] = node;
                }
                if (!complete) continue;

                yield return new CompleteWay
                {
                    Id = bw.Id,
                    Version = bw.Version,
                    ChangeSetId = bw.ChangeSetId,
                    TimeStamp = bw.TimeStamp,
                    UserId = bw.UserId,
                    UserName = bw.UserName,
                    Visible = bw.Visible,
                    Tags = bw.Tags,
                    Nodes = nodes,
                };
            }

            // 5) Batch state is discarded when it goes out of scope on the next iteration.
        }
    }

    private static TagsCollection CopyTags(TagsCollectionBase? source)
    {
        var target = new TagsCollection();
        if (source == null) return target;
        foreach (var t in source) target.Add(t.Key, t.Value);
        return target;
    }

    /// <summary>
    /// In-flight record used while a batch is being collected. Captures the way's
    /// metadata + tags immediately (the source recycles the Way instance across
    /// MoveNext calls) but defers materializing the Node[] until Phase 4 has fetched
    /// coords.
    /// </summary>
    private sealed class BufferedWay
    {
        public long Id;
        public long? Version;
        public long? ChangeSetId;
        public DateTime? TimeStamp;
        public long? UserId;
        public string? UserName;
        public bool? Visible;
        public TagsCollection Tags = new();
        public long[] NodeIds = Array.Empty<long>();
    }
}

/// <summary>
/// Per-batch stats reported to <see cref="PBFCompleteWayReader.Read"/>'s
/// <c>onBatchFlushed</c> callback, right before the batch's ways are yielded.
/// </summary>
public sealed record BatchStats(
    int BatchNumber,
    int WayCount,
    int UniqueNodeIds,
    int CoordsFetched,
    TimeSpan CollectElapsed,
    TimeSpan FetchElapsed);
