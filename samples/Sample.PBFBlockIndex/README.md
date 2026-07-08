# Sample.PBFBlockIndex

Shows the persistent block-index sidecar on `PBFOsmStreamSource`. Runs the same
targeted `MoveNext(OsmGeoType.Relation, 0)` twice — once cold (no sidecar), once
warm (sidecar loaded) — and prints wall time for each.

## What the sidecar contains

For each blob in the PBF, the block index records:

- **Tier-0**: file offset + end offset (where the blob starts and ends on disk).
- **Tier-1**: which primitive types are present (`HasNodes` / `HasWays` / `HasRelations`).
- **Tier-2** (when known): min/max id per type present.

The index accretes for free as any walk decodes blobs. Persisting it means the
next process gets the same skip information without having to re-walk.

## The three passes

- **Pass A — cold typed jump**: sidecar deleted. `MoveNext(Relation, 0)` on a
  freshly-constructed source must decode every node and way blob to reach the
  first relation.
- **Pass B — build the sidecar**: one full walk with `persistBlockIndex: true`.
  On `Dispose()` the accreted index is written to `{pbfPath}.blockindex`. This is
  the one-time cost you pay so future runs get the fast path.
- **Pass C — warm typed jump**: same jump as A. The ctor auto-loads the sidecar,
  so Tier-1 info tells the source that node and way blobs contain no relations —
  their bytes are seeked past without decompression.

## Running

```bash
dotnet run -c Release
```

Downloads Luxembourg (~52 MB) to the working directory on first run, reuses it
after. Pass a path to run against your own PBF:

```bash
dotnet run -c Release -- /path/to/some.osm.pbf
```

## API used

```csharp
// Persist the accreted block index on Dispose() and periodically during the walk.
using var src = new PBFOsmStreamSource(pbf, persistBlockIndex: true);
foreach (var geo in src) { /* ... */ }

// Auto-load the sidecar if it exists; do not write anything back.
using var src2 = new PBFOsmStreamSource(pbf);
src2.MoveNext(OsmGeoType.Relation, 0);

// Explicit flush at a checkpoint (throws when the source was built from a raw Stream).
src.SaveBlockIndex();
```

## Sidecar validation

The sidecar records the PBF's length and mtime at the time of writing. On load
those must match the current PBF exactly — any mismatch discards the sidecar
silently and the source starts cold. Writes are atomic via a temp file + rename,
so a crash mid-write never leaves a torn sidecar.
