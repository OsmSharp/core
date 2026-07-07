// The MIT License (MIT)

// Copyright (c) 2016 Ben Abelshausen

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using OsmSharp.Streams;

namespace OsmSharp.Test.Stream;

/// <summary>
/// Contains tests for the PBF osm stream source.
/// </summary>
[TestFixture]
internal class PBFOsmStreamSourceTests
{
    private static System.IO.Stream OpenWechel() =>
        Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Test.data.pbf.wechel.osm.pbf");

    /// <summary>
    /// A regression test on resetting a PBF osm stream.
    /// </summary>
    [Test]
    public void PBFOsmStreamReaderReset()
    {
        // generate the source.
        var source = new PBFOsmStreamSource(
            Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "OsmSharp.Test.data.pbf.api.osm.pbf"));

        // pull the data out.
        var target = new OsmStreamTargetEmpty();
        target.RegisterSource(source);
        target.Pull();

        // reset the source.
        if (source.CanReset)
        {
            source.Reset();

            // pull the data again.
            target.Pull();
        }
    }

    /// <summary>
    /// A regression test on initializing a stream.
    /// </summary>
    [Test]
    public void MoveNextWayRegression1()
    {
        using (var fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "OsmSharp.Test.data.pbf.api.osm.pbf"))
        {
            using (var reader = new PBFOsmStreamSource(fileStream))
            {
                var counter = 0;
                while (reader.MoveNextWay())
                {
                    if (counter++ % 10000 == 0)
                    {

                    }
                }
            }
        }
    }

    /// <summary>
    /// Tests reading an actual OSM-PBF file.
    /// </summary>
    [Test]
    public void ReadRealPBF()
    {
        using (var fileStream = this.GetType().Assembly.GetManifestResourceStream(
            "OsmSharp.Test.data.pbf.wechel.osm.pbf"))
        {
            var wechel = new List<OsmGeo>();
            using (var reader = new PBFOsmStreamSource(fileStream))
            {
                wechel.AddRange(reader);
            }

            Assert.That(wechel.Count, Is.EqualTo(13978));
        }
    }

    /// <summary>
    /// Tests reading an actual OSM-PBF file from a non-seekable stream.
    /// </summary>
    [Test]
    public void ReadRealPBFNonSeekable()
    {
        using (var fileStream = new NonSeekableStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(
            "OsmSharp.Test.data.pbf.wechel.osm.pbf")))
        {
            var wechel = new List<OsmGeo>();
            using (var reader = new PBFOsmStreamSource(fileStream))
            {
                wechel.AddRange(reader);
            }

            Assert.That(wechel.Count, Is.EqualTo(13978));
        }
    }

    /// <summary>
    /// Tests reading an actual OSM-PBF file from a non-seekable stream.
    /// </summary>
    [Test]
    public void ReadRealPBFNotAtBeginning()
    {
        var offsetMemoryStream = new MemoryStream();
        offsetMemoryStream.Write(new byte[235], 0, 235);
        using (var fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
            "OsmSharp.Test.data.pbf.wechel.osm.pbf"))
        {
            fileStream.CopyTo(offsetMemoryStream);
        }
        offsetMemoryStream.Seek(235, SeekOrigin.Begin);

        var wechel = new List<OsmGeo>();
        using (var reader = new PBFOsmStreamSource(offsetMemoryStream))
        {
            wechel.AddRange(reader);
        }

        Assert.That(wechel.Count, Is.EqualTo(13978));
    }

    /// <summary>
    /// Tests reading an actual OSM-PBF file from a non-seekable stream.
    /// </summary>
    [Test]
    public void ReadRealPBFNonSeekableNotAtBeginning()
    {
        var offsetMemoryStream = new MemoryStream();
        offsetMemoryStream.Write(new byte[235], 0, 235);
        using (var fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
            "OsmSharp.Test.data.pbf.wechel.osm.pbf"))
        {
            fileStream.CopyTo(offsetMemoryStream);
        }
        offsetMemoryStream.Seek(235, SeekOrigin.Begin);

        var wechel = new List<OsmGeo>();
        using (var reader = new PBFOsmStreamSource(new NonSeekableStream(offsetMemoryStream)))
        {
            wechel.AddRange(reader);
        }

        Assert.That(wechel.Count, Is.EqualTo(13978));
    }

    /// <summary>
    /// Tests reading from a stream where position is not available.
    /// </summary>
    [Test]
    public void PBFOsmStreamSource_ShouldBeAbleToReadFromStreamWithPositionNotAvailable()
    {
        using (var fileStream = new DeflateMockStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(
            "OsmSharp.Test.data.pbf.wechel.osm.pbf")))
        {
            var wechel = new List<OsmGeo>();
            using (var reader = new PBFOsmStreamSource(fileStream))
            {
                wechel.AddRange(reader);
            }

            Assert.That(wechel.Count, Is.EqualTo(13978));
        }
    }

    /// <summary>
    /// MoveNext(type, minId) yields elements of the requested type in id-ascending order,
    /// each with Id &gt;= the requested minId. Successive calls that request "next id after
    /// current + 1" must reproduce the full ordered list of that type.
    /// </summary>
    [Test]
    public void MoveNext_TypedMinId_MatchesFullWayEnumeration()
    {
        List<Way> waysViaFullWalk;
        using (var stream = OpenWechel())
        using (var reader = new PBFOsmStreamSource(stream))
        {
            waysViaFullWalk = reader.OfType<Way>().ToList();
        }

        List<Way> waysViaTypedMoveNext;
        using (var stream = OpenWechel())
        using (var reader = new PBFOsmStreamSource(stream))
        {
            waysViaTypedMoveNext = new List<Way>();
            long minId = long.MinValue;
            while (reader.MoveNext(OsmGeoType.Way, minId))
            {
                var w = (Way)reader.Current();
                waysViaTypedMoveNext.Add(w);
                minId = w.Id.Value + 1;
            }
        }

        Assert.That(waysViaTypedMoveNext.Count, Is.EqualTo(waysViaFullWalk.Count),
            "typed MoveNext yielded a different way count than the full walk");
        for (var i = 0; i < waysViaFullWalk.Count; i++)
        {
            Assert.That(waysViaTypedMoveNext[i].Id, Is.EqualTo(waysViaFullWalk[i].Id),
                $"way at index {i} differs");
        }
    }

    /// <summary>
    /// Same idea for nodes. Confirms the block-index Tier-2 skip doesn't drop node ids
    /// on a cold walk.
    /// </summary>
    [Test]
    public void MoveNext_TypedMinId_MatchesFullNodeEnumeration()
    {
        List<Node> nodesViaFullWalk;
        using (var stream = OpenWechel())
        using (var reader = new PBFOsmStreamSource(stream))
        {
            nodesViaFullWalk = reader.OfType<Node>().ToList();
        }

        List<Node> nodesViaTypedMoveNext;
        using (var stream = OpenWechel())
        using (var reader = new PBFOsmStreamSource(stream))
        {
            nodesViaTypedMoveNext = new List<Node>();
            long minId = long.MinValue;
            while (reader.MoveNext(OsmGeoType.Node, minId))
            {
                var n = (Node)reader.Current();
                nodesViaTypedMoveNext.Add(n);
                minId = n.Id.Value + 1;
            }
        }

        Assert.That(nodesViaTypedMoveNext.Count, Is.EqualTo(nodesViaFullWalk.Count));
        for (var i = 0; i < nodesViaFullWalk.Count; i++)
        {
            Assert.That(nodesViaTypedMoveNext[i].Id, Is.EqualTo(nodesViaFullWalk[i].Id));
        }
    }

    /// <summary>
    /// MoveNext(type, minId) with minId equal to a known way id in the fixture must yield
    /// that way (or the next one if the id doesn't exist). Verifies id-lower-bound semantics.
    /// </summary>
    [Test]
    public void MoveNext_TypedMinId_SeeksToRequestedId()
    {
        long midWayId;
        using (var stream = OpenWechel())
        using (var reader = new PBFOsmStreamSource(stream))
        {
            var ways = reader.OfType<Way>().ToList();
            Assert.That(ways.Count, Is.GreaterThan(1));
            midWayId = ways[ways.Count / 2].Id.Value;
        }

        using (var stream = OpenWechel())
        using (var reader = new PBFOsmStreamSource(stream))
        {
            Assert.IsTrue(reader.MoveNext(OsmGeoType.Way, midWayId));
            var first = (Way)reader.Current();
            Assert.That(first.Id, Is.EqualTo(midWayId));
        }
    }

    /// <summary>
    /// Forward-only contract: once a typed MoveNext has yielded an id, a subsequent typed
    /// MoveNext for the same type with a smaller minId must throw.
    /// </summary>
    [Test]
    public void MoveNext_TypedMinId_ThrowsWhenCurrentIsPastRequestedId()
    {
        using (var stream = OpenWechel())
        using (var reader = new PBFOsmStreamSource(stream))
        {
            Assert.IsTrue(reader.MoveNext(OsmGeoType.Way, long.MinValue));
            var firstId = reader.Current().Id.Value;

            // advance a few more so the current id is clearly past long.MinValue
            for (var i = 0; i < 3 && reader.MoveNext(OsmGeoType.Way, reader.Current().Id.Value + 1); i++) { }
            Assert.That(reader.Current().Id.Value, Is.GreaterThan(firstId));

            Assert.Throws<InvalidOperationException>(
                () => reader.MoveNext(OsmGeoType.Way, firstId),
                "expected forward-only guard to throw when asked to rewind");
        }
    }

    /// <summary>
    /// Forward-only across types: once we've read a way, asking for a node must throw,
    /// because nodes come before ways in PBF ordering and can't be reached without a rewind.
    /// </summary>
    [Test]
    public void MoveNext_TypedMinId_ThrowsWhenCurrentIsLaterTypeThanRequested_WayThenNode()
    {
        using (var stream = OpenWechel())
        using (var reader = new PBFOsmStreamSource(stream))
        {
            Assert.IsTrue(reader.MoveNextWay(), "fixture should have at least one way");
            Assert.That(reader.Current().Type, Is.EqualTo(OsmGeoType.Way));

            Assert.Throws<InvalidOperationException>(
                () => reader.MoveNext(OsmGeoType.Node, long.MinValue),
                "expected forward-only guard to throw when asking for a node while current is a way");
        }
    }

    /// <summary>
    /// Same, but relation-then-way and relation-then-node.
    /// </summary>
    [Test]
    public void MoveNext_TypedMinId_ThrowsWhenCurrentIsLaterTypeThanRequested_RelationThenEarlier()
    {
        using (var stream = OpenWechel())
        using (var reader = new PBFOsmStreamSource(stream))
        {
            if (!reader.MoveNextRelation())
            {
                Assert.Ignore("fixture has no relations, skipping");
                return;
            }
            Assert.That(reader.Current().Type, Is.EqualTo(OsmGeoType.Relation));

            Assert.Throws<InvalidOperationException>(
                () => reader.MoveNext(OsmGeoType.Node, long.MinValue),
                "relation → node should throw");
            Assert.Throws<InvalidOperationException>(
                () => reader.MoveNext(OsmGeoType.Way, long.MinValue),
                "relation → way should throw");
        }
    }

    /// <summary>
    /// Warm-path: after a full walk populates the internal block index, a second walk
    /// (post-Reset) must produce identical output. Regression check that block-index skip
    /// doesn't drop primitives.
    /// </summary>
    [Test]
    public void WarmPath_ResetAndRewalk_YieldsIdenticalOutput()
    {
        using (var stream = OpenWechel())
        using (var reader = new PBFOsmStreamSource(stream))
        {
            var first = reader.Select(g => (g.Type, g.Id)).ToList();
            Assert.IsTrue(reader.CanReset);
            reader.Reset();
            var second = reader.Select(g => (g.Type, g.Id)).ToList();

            Assert.That(second.Count, Is.EqualTo(first.Count));
            for (var i = 0; i < first.Count; i++)
            {
                Assert.That(second[i], Is.EqualTo(first[i]));
            }
        }
    }

    /// <summary>
    /// Warm-path: after a full walk, a subsequent nodes-only walk (post-Reset) must yield
    /// exactly the same nodes as the first walk did, in the same order — the block-index
    /// Tier-1 skip must not drop node-bearing blobs.
    /// </summary>
    [Test]
    public void WarmPath_NodesOnlyAfterFullWalk_YieldsSameNodes()
    {
        using (var stream = OpenWechel())
        using (var reader = new PBFOsmStreamSource(stream))
        {
            var nodesFirst = reader.OfType<Node>().Select(n => n.Id.Value).ToList();

            Assert.IsTrue(reader.CanReset);
            reader.Reset();

            var nodesSecond = new List<long>();
            while (reader.MoveNext(ignoreNodes: false, ignoreWays: true, ignoreRelations: true))
            {
                var n = (Node)reader.Current();
                nodesSecond.Add(n.Id.Value);
            }

            Assert.That(nodesSecond.Count, Is.EqualTo(nodesFirst.Count));
            for (var i = 0; i < nodesFirst.Count; i++)
            {
                Assert.That(nodesSecond[i], Is.EqualTo(nodesFirst[i]));
            }
        }
    }

    /// <summary>
    /// Warm-path: after a full walk, a subsequent ways-only walk (post-Reset) must yield
    /// exactly the same ways. Also exercises the Tier-1 skip for the (mostly node) blobs
    /// at the start of the file.
    /// </summary>
    [Test]
    public void WarmPath_WaysOnlyAfterFullWalk_YieldsSameWays()
    {
        using (var stream = OpenWechel())
        using (var reader = new PBFOsmStreamSource(stream))
        {
            var waysFirst = reader.OfType<Way>().Select(w => w.Id.Value).ToList();

            Assert.IsTrue(reader.CanReset);
            reader.Reset();

            var waysSecond = new List<long>();
            while (reader.MoveNextWay())
            {
                var w = (Way)reader.Current();
                waysSecond.Add(w.Id.Value);
            }

            Assert.That(waysSecond.Count, Is.EqualTo(waysFirst.Count));
            for (var i = 0; i < waysFirst.Count; i++)
            {
                Assert.That(waysSecond[i], Is.EqualTo(waysFirst[i]));
            }
        }
    }

    /// <summary>
    /// Warm-path with typed MoveNext: after a full walk populates Tier-2 (id ranges per
    /// blob), a subsequent typed walk requesting way ids past the max in the file must
    /// return false without misbehaving.
    /// </summary>
    [Test]
    public void WarmPath_TypedMoveNext_PastMaxId_ReturnsFalse()
    {
        using (var stream = OpenWechel())
        using (var reader = new PBFOsmStreamSource(stream))
        {
            long maxWayId = 0;
            foreach (var g in reader.OfType<Way>())
            {
                if (g.Id.Value > maxWayId) maxWayId = g.Id.Value;
            }
            Assert.That(maxWayId, Is.GreaterThan(0));

            reader.Reset();
            Assert.IsFalse(reader.MoveNext(OsmGeoType.Way, maxWayId + 1));
        }
    }
}
