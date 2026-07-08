using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using OsmSharp;
using OsmSharp.Streams;
using Sample.PBFBlockIndex.Staging;

namespace Sample.PBFBlockIndex;

/// <summary>
/// Demonstrates the persistent block-index sidecar on
/// <see cref="OsmSharp.Streams.PBFOsmStreamSource"/>. Runs the same targeted
/// <c>MoveNext(OsmGeoType.Relation, 0)</c> jump twice — once with no sidecar (cold),
/// once with the sidecar loaded (warm) — and prints wall time for each. The warm case
/// skips every node and way blob using the loaded Tier-1 info.
/// </summary>
internal class Program
{
    private const string DefaultPbfUrl = "http://planet.anyways.eu/planet/europe/luxembourg/luxembourg-latest.osm.pbf";
    private const string DefaultPbfFile = "luxembourg-latest.osm.pbf";

    private static async Task Main(string[] args)
    {
        var pbf = args.Length > 0 ? args[0] : DefaultPbfFile;
        if (pbf == DefaultPbfFile)
        {
            await Download.ToFile(DefaultPbfUrl, DefaultPbfFile);
        }
        else if (!File.Exists(pbf))
        {
            Console.Error.WriteLine($"PBF file not found: {pbf}");
            Environment.Exit(1);
        }

        var idx = pbf + ".blockindex";
        var pbfBytes = new FileInfo(pbf).Length;
        Console.WriteLine($"PBF:      {pbf} ({pbfBytes / 1024 / 1024:N0} MB)");
        Console.WriteLine($"Sidecar:  {idx}");
        Console.WriteLine();

        // Pass A — cold typed jump. Ensure no sidecar exists; the source has no Tier-1
        // info about any blob, so it must decode every node and way blob before finding
        // the first relation.
        if (File.Exists(idx)) File.Delete(idx);
        var (coldElapsed, coldFirstId) = TimeFirstRelationJump(pbf);
        Console.WriteLine($"Pass A (cold, no sidecar)");
        Console.WriteLine($"  first relation id: {coldFirstId}");
        Console.WriteLine($"  wall time:         {coldElapsed.TotalMilliseconds:F0} ms");
        Console.WriteLine();

        // Pass B — build the sidecar by doing one full walk with persistence enabled.
        // This is the one-time cost you pay so subsequent runs can jump quickly.
        Console.WriteLine("Pass B (build sidecar, full walk with persistBlockIndex: true)");
        var sw = Stopwatch.StartNew();
        long walked = 0;
        using (var src = new PBFOsmStreamSource(pbf, persistBlockIndex: true))
        {
            foreach (var _ in src) walked++;
        }
        sw.Stop();
        var idxBytes = new FileInfo(idx).Length;
        Console.WriteLine($"  elements walked:   {walked:N0}");
        Console.WriteLine($"  wall time:         {sw.Elapsed.TotalSeconds:F2} s");
        Console.WriteLine($"  sidecar written:   {idxBytes:N0} bytes ({(double)idxBytes / pbfBytes:P4} of PBF)");
        Console.WriteLine();

        // Pass C — warm typed jump. The ctor auto-loads the sidecar, which contains
        // Tier-1 info for every blob. Node blobs are skipped without decompression;
        // way blobs likewise. The source stops at the first blob that HasRelations = true.
        var (warmElapsed, warmFirstId) = TimeFirstRelationJump(pbf);
        Console.WriteLine("Pass C (warm, sidecar loaded)");
        Console.WriteLine($"  first relation id: {warmFirstId}");
        Console.WriteLine($"  wall time:         {warmElapsed.TotalMilliseconds:F0} ms");
        Console.WriteLine();

        // Both jumps should land on the same relation. If they diverge, something is
        // wrong with the load path — surface it loudly.
        if (coldFirstId != warmFirstId)
        {
            Console.Error.WriteLine($"MISMATCH: cold={coldFirstId} vs warm={warmFirstId}");
            Environment.Exit(2);
        }

        Console.WriteLine("───── Summary ─────");
        Console.WriteLine($"Cold typed jump:  {coldElapsed.TotalMilliseconds,7:F0} ms");
        Console.WriteLine($"Warm typed jump:  {warmElapsed.TotalMilliseconds,7:F0} ms");
        if (warmElapsed.TotalMilliseconds > 0)
        {
            Console.WriteLine($"Speedup:          {coldElapsed.TotalMilliseconds / warmElapsed.TotalMilliseconds,7:F1}×");
        }
    }

    /// <summary>
    /// Constructs a fresh source over <paramref name="pbf"/> (auto-loads sidecar if
    /// present), jumps to the first relation with <c>MoveNext(Relation, 0)</c>, and
    /// returns elapsed time and the id landed on.
    /// </summary>
    private static (TimeSpan Elapsed, long? RelationId) TimeFirstRelationJump(string pbf)
    {
        var sw = Stopwatch.StartNew();
        using var src = new PBFOsmStreamSource(pbf);
        var found = src.MoveNext(OsmGeoType.Relation, 0);
        sw.Stop();
        return (sw.Elapsed, found ? src.Current().Id : null);
    }
}
