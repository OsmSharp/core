using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Sample.CompleteBuildings.Staging;

namespace Sample.CompleteBuildings;

/// <summary>
/// Downloads a small country PBF and prints stats about every <c>building=*</c> way
/// reconstructed as an <see cref="OsmSharp.Complete.CompleteWay"/> (id + tags + fully
/// materialized <c>Node[]</c>). Demonstrates that on-demand node coord fetching via
/// <see cref="OsmSharp.Streams.PBFOsmStreamSource"/>'s block index + <c>MoveNext(type,
/// minId)</c> is fast enough to skip the "index every node" pre-pass entirely.
///
/// Small node budget by default so batching actually kicks in on Luxembourg. The
/// pattern is the same regardless of size — bigger PBFs just get more batches.
/// </summary>
internal class Program
{
    private const string DefaultPbfUrl = "http://planet.anyways.eu/planet/europe/luxembourg/luxembourg-latest.osm.pbf";
    private const string DefaultPbfFile = "luxembourg-latest.osm.pbf";

    private static async Task Main(string[] args)
    {
        // Node budget: peak in-flight unique node ids per batch. Kept small on purpose so
        // Luxembourg splits into ~20 batches and you can see the mechanism in action.
        // Real workloads scale this up (few hundred k to a few million).
        var nodeBudget = args.Length > 0 && int.TryParse(args[0], out var b) ? b : 50_000;

        // Optional second arg: path to an existing PBF. When omitted we download
        // Luxembourg to a local file.
        var pbfFile = args.Length > 1 ? args[1] : DefaultPbfFile;
        if (pbfFile == DefaultPbfFile)
        {
            await Download.ToFile(DefaultPbfUrl, DefaultPbfFile);
        }
        else if (!File.Exists(pbfFile))
        {
            Console.Error.WriteLine($"PBF file not found: {pbfFile}");
            Environment.Exit(1);
        }

        Console.WriteLine($"Reading {pbfFile} (node budget = {nodeBudget:N0}).");
        Console.WriteLine();
        Console.WriteLine("Per-batch:");
        Console.WriteLine($"  {"batch",5}  {"ways",7}  {"nodes",8}  {"coords",8}  {"collect",8}  {"fetch",8}");

        await using var pbfStream = File.OpenRead(pbfFile);

        var total = 0;
        var closed = 0;
        var open = 0;
        var totalNodes = 0L;
        var sw = Stopwatch.StartNew();

        foreach (var way in PBFCompleteWayReader.Read(
            pbfStream,
            filter: w => w.Tags?.ContainsKey("building") == true,
            nodeBudget: nodeBudget,
            onBatchFlushed: stats => Console.WriteLine(
                $"  {stats.BatchNumber,5}  {stats.WayCount,7:N0}  {stats.UniqueNodeIds,8:N0}  " +
                $"{stats.CoordsFetched,8:N0}  {stats.CollectElapsed.TotalMilliseconds,7:F0}ms  " +
                $"{stats.FetchElapsed.TotalMilliseconds,7:F0}ms")))
        {
            total++;
            totalNodes += way.Nodes.Length;
            if (IsClosedRing(way))
                closed++;
            else
                open++;
        }

        sw.Stop();
        Console.WriteLine();
        Console.WriteLine("───── Done ─────");
        Console.WriteLine($"Total buildings:  {total:N0}");
        Console.WriteLine($"  closed rings:   {closed:N0}");
        Console.WriteLine($"  open ways:      {open:N0}");
        Console.WriteLine($"Total node refs:  {totalNodes:N0}");
        Console.WriteLine($"Wall time:        {sw.Elapsed.TotalSeconds:F2}s");
        if (total > 0)
        {
            Console.WriteLine($"Per building:     {sw.Elapsed.TotalMilliseconds / total:F2} ms");
        }
    }

    /// <summary>
    /// A way is a closed ring when its first and last nodes have the same id — the
    /// standard OSM convention for polygon-shaped ways.
    /// </summary>
    private static bool IsClosedRing(OsmSharp.Complete.CompleteWay way) =>
        way.Nodes.Length >= 4 && way.Nodes[0].Id == way.Nodes[^1].Id;
}
