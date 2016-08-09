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

using NUnit.Framework;
using OsmSharp.Streams;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace OsmSharp.Test.Stream
{
    /// <summary>
    /// Contains tests for the osm xml source stream.
    /// </summary>
    [TestFixture]
    public class XmlOsmStreamSourceTests
    {
        /// <summary>
        /// Test reading one node.
        /// </summary>
        [Test]
        public void TestReadNode()
        {
            // build the source.
            var source = new XmlOsmStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "OsmSharp.Test.data.xml.node.osm"));

            // read.
            var result = new List<OsmGeo>(source);

            // check results.
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.IsInstanceOf<Node>(result[0]);
            var node = result[0] as Node;
            Assert.AreEqual(471625991, node.Id);
            Assert.AreEqual(51.2704712f, node.Latitude);
            Assert.AreEqual(4.8006659f, node.Longitude);
            Assert.AreEqual("marc12", node.UserName);
            Assert.AreEqual(540527, node.UserId);
            Assert.AreEqual(true, node.Visible);
            Assert.AreEqual(3, node.Version);
            Assert.AreEqual(9797840, node.ChangeSetId);
            Assert.AreEqual(new DateTime(2011, 11, 11, 16, 43, 47), node.TimeStamp.Value.ToUniversalTime());
            Assert.IsNotNull(node.Tags);
            Assert.AreEqual(3, node.Tags.Count);
            Assert.IsTrue(node.Tags.Contains("alt_name", "Lille"));
            Assert.IsTrue(node.Tags.Contains("name", "Wechelderzande"));
            Assert.IsTrue(node.Tags.Contains("traffic_sign", "city_limit"));
        }

        /// <summary>
        /// Test reading one way.
        /// </summary>
        [Test]
        public void TestReadWay()
        {
            // build the source.
            var source = new XmlOsmStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "OsmSharp.Test.data.xml.way.osm"));

            // read.
            var result = new List<OsmGeo>(source);

            // check results.
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.IsInstanceOf<Way>(result[0]);
            var way = result[0] as Way;
            Assert.AreEqual(88310427, way.Id);
            Assert.AreEqual("Ben Abelshausen", way.UserName);
            Assert.AreEqual(137772, way.UserId);
            Assert.AreEqual(true, way.Visible);
            Assert.AreEqual(1, way.Version);
            Assert.AreEqual(6570367, way.ChangeSetId);
            Assert.AreEqual(new DateTime(2010, 12, 06, 23, 58, 37), way.TimeStamp.Value.ToUniversalTime());
            Assert.IsNotNull(way.Tags);
            Assert.AreEqual(1, way.Tags.Count);
            Assert.IsTrue(way.Tags.Contains("building", "yes"));
            Assert.IsNotNull(way.Nodes);
            Assert.AreEqual(5, way.Nodes.Length);
            Assert.AreEqual(1025709357, way.Nodes[0]);
            Assert.AreEqual(1025709360, way.Nodes[1]);
            Assert.AreEqual(1025709358, way.Nodes[2]);
            Assert.AreEqual(1025709344, way.Nodes[3]);
            Assert.AreEqual(1025709357, way.Nodes[4]);
        }

        /// <summary>
        /// Test reading one relation.
        /// </summary>
        [Test]
        public void TestReadRelation()
        {
            // build the source.
            var source = new XmlOsmStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "OsmSharp.Test.data.xml.relation.osm"));

            // read.
            var result = new List<OsmGeo>(source);

            // check results.
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.IsInstanceOf<Relation>(result[0]);
            var relation = result[0] as Relation;
            Assert.AreEqual(214314, relation.Id);
            Assert.AreEqual("marc12", relation.UserName);
            Assert.AreEqual(540527, relation.UserId);
            Assert.AreEqual(true, relation.Visible);
            Assert.AreEqual(18, relation.Version);
            Assert.AreEqual(9797825, relation.ChangeSetId);
            Assert.AreEqual(new DateTime(2011, 11, 11, 16, 42, 26), relation.TimeStamp.Value.ToUniversalTime());
            Assert.IsNotNull(relation.Tags);
            Assert.AreEqual(4, relation.Tags.Count);
            Assert.IsTrue(relation.Tags.Contains("network", "rcn"));
            Assert.IsTrue(relation.Tags.Contains("note", "53-80"));
            Assert.IsTrue(relation.Tags.Contains("route", "bicycle"));
            Assert.IsTrue(relation.Tags.Contains("type", "route"));
            Assert.IsNotNull(relation.Members);
            Assert.AreEqual(13, relation.Members.Length);

            Assert.AreEqual(string.Empty, relation.Members[0].Role);
            Assert.AreEqual(OsmGeoType.Way, relation.Members[0].Type);
            Assert.AreEqual(37294428, relation.Members[0].Id);

            Assert.AreEqual("forward", relation.Members[1].Role);
            Assert.AreEqual(OsmGeoType.Way, relation.Members[1].Type);
            Assert.AreEqual(87492000, relation.Members[1].Id);

            Assert.AreEqual("forward", relation.Members[2].Role);
            Assert.AreEqual(OsmGeoType.Way, relation.Members[2].Type);
            Assert.AreEqual(37682837, relation.Members[2].Id);

            Assert.AreEqual("forward", relation.Members[3].Role);
            Assert.AreEqual(OsmGeoType.Way, relation.Members[3].Type);
            Assert.AreEqual(88614492, relation.Members[3].Id);

            Assert.AreEqual(string.Empty, relation.Members[4].Role);
            Assert.AreEqual(OsmGeoType.Way, relation.Members[4].Type);
            Assert.AreEqual(88614520, relation.Members[4].Id);

            Assert.AreEqual(string.Empty, relation.Members[5].Role);
            Assert.AreEqual(OsmGeoType.Way, relation.Members[5].Type);
            Assert.AreEqual(39448130, relation.Members[5].Id);

            Assert.AreEqual(string.Empty, relation.Members[6].Role);
            Assert.AreEqual(OsmGeoType.Way, relation.Members[6].Type);
            Assert.AreEqual(39364233, relation.Members[6].Id);

            Assert.AreEqual(string.Empty, relation.Members[7].Role);
            Assert.AreEqual(OsmGeoType.Way, relation.Members[7].Type);
            Assert.AreEqual(52285585, relation.Members[7].Id);

            Assert.AreEqual(string.Empty, relation.Members[8].Role);
            Assert.AreEqual(OsmGeoType.Way, relation.Members[8].Type);
            Assert.AreEqual(39364232, relation.Members[8].Id);

            Assert.AreEqual(string.Empty, relation.Members[9].Role);
            Assert.AreEqual(OsmGeoType.Way, relation.Members[9].Type);
            Assert.AreEqual(136621092, relation.Members[9].Id);

            Assert.AreEqual(string.Empty, relation.Members[10].Role);
            Assert.AreEqual(OsmGeoType.Way, relation.Members[10].Type);
            Assert.AreEqual(88195311, relation.Members[10].Id);

            Assert.AreEqual(string.Empty, relation.Members[11].Role);
            Assert.AreEqual(OsmGeoType.Way, relation.Members[11].Type);
            Assert.AreEqual(88195309, relation.Members[11].Id);

            Assert.AreEqual(string.Empty, relation.Members[12].Role);
            Assert.AreEqual(OsmGeoType.Way, relation.Members[12].Type);
            Assert.AreEqual(88195313, relation.Members[12].Id);
        }

        /// <summary>
        /// A regression test in resetting an XML data source.
        /// </summary>
        [Test]
        public void TestReset()
        {
            // generate the source.
            var source = new XmlOsmStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "OsmSharp.Test.data.xml.api.osm"));

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
    }
}
