# Sample.CompleteBuildings

Demonstrates a memory-bounded way of reading complete ways (way + fully resolved node
geometry) from a PBF file, without ever building a global node coordinate cache.

## What it does

Reads Luxembourg's PBF and emits every `building=*` way as an
`OsmSharp.Complete.CompleteWay` — with its OSM id, tags, and an ordered
`Node[]` of fully materialized child nodes (each carrying id, coord, tags and
metadata). Prints per-batch stats and a running count.

```bash
dotnet run --project samples/Sample.CompleteBuildings                # default 50k node budget
dotnet run --project samples/Sample.CompleteBuildings -- 500000      # 500k node budget (bigger batch, fewer)
```

The default budget is deliberately small (50k unique node ids) so Luxembourg's
~220k buildings split into ~20 batches — enough to clearly show what the
mechanism is doing. Production consumers pick a budget based on available RAM;
the pattern is the same regardless.

## Why the approach matters

The established way to resolve child nodes for ways in OSM tooling is a **two-pass
scan** over the PBF:

1. First pass iterates ways to collect the set of node ids referenced by all wanted
   ways (`wantedNodeIds`).
2. Second pass iterates nodes and keeps only those in that set, into a
   `long → (lon,lat)` dictionary.
3. Then iterate the wanted ways (or replay them from a buffer) and look up each node.

That works and is memory-honest — you never hold coords for nodes you won't need. But
the coord dictionary at the end still contains **every wanted node**, all at once. For
a broad filter (say `building=*` on a planet PBF, with hundreds of millions of nodes)
that's still gigabytes of resident state.

This sample adds a **batched version** of the same two-pass idea. Instead of one big
`wantedNodeIds` set + one big coord dictionary, we process a fixed number of unique
node ids at a time:

- Collect ways forward until the union of their node ids hits `nodeBudget`.
- Run a mini second-pass over the nodes for that batch only.
- Emit the batch's complete ways.
- Discard everything, move on to the next range of ways.

Peak memory is bounded by `nodeBudget` regardless of file size. The trade is wall time
— we walk the nodes section once per batch instead of once total — but the block index
inside `PBFOsmStreamSource` makes each of those walks cheap.

The two properties of the post-block-index `PBFOsmStreamSource` that make this
practical:

1. **The block index accretes automatically as the source decodes blobs**, and it
   persists across `Reset()`. Only the stream position resets — the map of
   `(offset → tile-1 presence flags, tile-2 id ranges)` stays.
2. **`MoveNext(OsmGeoType type, long minId)` uses the block index to skip whole blobs**
   whose contents can't include what we're asking for. After the first pass through the
   file, subsequent random-access lookups only touch the blobs whose id ranges actually
   overlap the batch.

## Example output

```
Reading luxembourg-latest.osm.pbf (node budget = 50,000).

Per-batch:
  batch     ways     nodes    coords   collect     fetch
      1    7,117    50,004    50,004     921ms    1310ms
      2    8,962    50,004    50,004      80ms    1019ms
      3    8,386    50,007    50,007      81ms    1053ms
   …
     23    9,647    41,643    41,643     184ms    1045ms

Total buildings:  220,725
Wall time:        26.42s
Per building:     0.12 ms
```

Batch 1 has a higher collect time (~920ms) because that pass walks the entire
nodes section on its way to the first way — that's what warms the block index.
Every batch after that sees ~80-180ms collect (just iterating ways forward) and
~1s fetch (Reset + monotonic node walk with warm skip). Fetch time stays
essentially flat regardless of batch number.

## The batching pattern

```csharp
foreach (var way in PBFCompleteWayReader.Read(
    stream,
    filter: w => w.Tags?.ContainsKey("building") == true,
    nodeBudget: 50_000,
    onBatchFlushed: stats => Console.WriteLine($"batch {stats.BatchNumber} done")))
{
    // way.Id, way.Tags, way.Coordinates are all fully materialized
}
```

Inside the reader:

1. `MoveNext(OsmGeoType.Way, lastId + 1)` — jump to the next way past the last one we
   emitted. On the very first batch, this walks the entire node section, populating
   the block index for those blobs as a side effect.
2. Iterate ways forward, keeping the ones matching `filter`. For each, buffer its
   metadata + tags + node id list and add the node ids to a per-batch `HashSet<long>`.
3. When that set reaches `nodeBudget`, stop accumulating.
4. `source.Reset()` + `MoveNext(OsmGeoType.Node, sortedId)` — walk the batch's node ids
   monotonically. With the warm block index, this touches only the node blobs whose id
   ranges overlap this batch. Each fetched `Node` is stashed in a
   `Dictionary<long, Node>`.
5. Build each `CompleteWay` from the buffered way records + the node dictionary, yield
   it.
6. Drop the batch's state entirely (way buffer, node id set, node dictionary). Loop
   back to step 1 for the next range of ways.

Peak in-flight state per batch: the buffered `WayRecord[]` (roughly
`ways × (30 + 8×avg_nodes)` bytes) plus the fetched `Node` dictionary (roughly
`nodeBudget × ~100` bytes since `Node` carries id + coord + tags + metadata). Nothing
survives across iterations except the source itself and its block index.

## What to look at

- **`PBFCompleteWayReader.cs`** — the whole idea in ~100 lines of iterator body.
  Yields `OsmSharp.Complete.CompleteWay` instances with fully materialized
  `Node[]` members (id + coord + tags + metadata).
- **`Program.cs`** — top-level demo: download PBF, iterate, print stats.

## Notes and extensions

- Same pattern works for `MoveNext(OsmGeoType.Relation, id)`. Building complete
  multipolygons is one extra layer: relation → member way ids → each way's node ids →
  fetch node coords. The source's block index stays warm across all three levels.
- Bigger `nodeBudget` = fewer batches, so less redundant node blob decompression. Tune
  it based on available RAM.
- The reader silently drops ways whose nodes aren't fully present in the file (e.g.
  filtered lifecycle-annotated nodes). Add error handling in your own consumer if you
  need to detect that.
- The `TagsCollection` on each returned `CompleteWay` is a copy — the source recycles
  its `Way` instance across iterations, so tags are captured up front rather than
  referenced from the source's buffer.
- Node instances in `CompleteWay.Nodes` are not recycled: `PBFOsmStreamSource` returns
  a fresh `Node` for every `MoveNext(Node, id)` call, so retaining them (as the reader
  does across the batch's yield phase) is safe.
