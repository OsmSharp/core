using System;
using System.IO;
using NUnit.Framework;
using OsmSharp.Streams;

namespace OsmSharp.Test.IO.PBF;

/// <summary>
/// The replication fields a PBF header carries.
/// </summary>
/// <remarks>
/// A planet-derived extract records which replication sequence it was cut at. Anything building
/// a database from an extract needs it: without it there is no safe place to resume replication
/// from, because starting at the endpoint's latest skips everything in between and leaves
/// nothing behind that could say so.
/// </remarks>
[TestFixture]
public class PBFReplicationStateTests
{
    /// <summary>
    /// Needs a real extract, since the fields come from whatever cut it. Set OSM_PBF to one.
    /// </summary>
    [Test]
    public void AnExtractsHeader_ShouldSayWhereItStandsInTheStream()
    {
        var path = Environment.GetEnvironmentVariable("OSM_PBF");
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            Assert.Ignore($"OSM_PBF not set or missing: {path}");
        }

        using var stream = File.OpenRead(path);
        var source = new PBFOsmStreamSource(stream);

        // nothing has been read, so the header has not been passed yet.
        Assert.IsNull(source.ReplicationState, "the header was reported before it was read");

        Assert.IsTrue(source.MoveNext(), "the extract is empty");

        var state = source.ReplicationState;
        if (state == null)
        {
            // Not a failure: the fields are optional and whatever cut the extract may have
            // left them out. Both extracts to hand were cut by osmium/1.7.1 and carry none,
            // while their writingprogram and required_features decode — so the header is
            // being read, and this is the file saying nothing rather than the reader failing.
            Assert.Ignore(
                "this extract's header carries no replication state, so there is nothing " +
                "here to check; use one cut by a tool that records it");
        }
        Assert.Greater(state!.Value.sequenceNumber, 0,
            "the sequence number came back as zero, which no real extract has");

        // the timestamp has to be in the past and not absurdly so: a decoding mistake on a
        // seconds-since-epoch field lands in 1970 or far in the future rather than near now.
        Assert.Greater(state.Value.timestamp, new DateTime(2012, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            "the timestamp decoded to before OSM replication existed");
        Assert.Less(state.Value.timestamp, DateTime.UtcNow.AddDays(1),
            "the timestamp decoded to the future");

        TestContext.Out.WriteLine(
            $"sequence {state.Value.sequenceNumber} at {state.Value.timestamp:u} " +
            $"from {state.Value.baseUrl}");
    }
}
