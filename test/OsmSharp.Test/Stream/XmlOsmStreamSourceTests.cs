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
using System.IO;
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
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0], Is.InstanceOf<Node>());
            var node = result[0] as Node;
            Assert.That(node.Id, Is.EqualTo(471625991));
            Assert.That(node.Latitude, Is.EqualTo(51.2704712));
            Assert.That(node.Longitude, Is.EqualTo(4.8006659));
            Assert.That(node.UserName, Is.EqualTo("marc12"));
            Assert.That(node.UserId, Is.EqualTo(540527));
            Assert.That(node.Visible, Is.EqualTo(true));
            Assert.That(node.Version, Is.EqualTo(3));
            Assert.That(node.ChangeSetId, Is.EqualTo(9797840));
            Assert.That(node.TimeStamp.Value.ToUniversalTime(), Is.EqualTo(new DateTime(2011, 11, 11, 16, 43, 47)));
            Assert.That(node.Tags, Is.Not.Null);
            Assert.That(node.Tags.Count, Is.EqualTo(3));
            Assert.That(node.Tags.Contains("alt_name", "Lille"), Is.True);
            Assert.That(node.Tags.Contains("name", "Wechelderzande"), Is.True);
            Assert.That(node.Tags.Contains("traffic_sign", "city_limit"), Is.True);
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
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0], Is.InstanceOf<Way>());
            var way = result[0] as Way;
            Assert.That(way.Id, Is.EqualTo(88310427));
            Assert.That(way.UserName, Is.EqualTo("Ben Abelshausen"));
            Assert.That(way.UserId, Is.EqualTo(137772));
            Assert.That(way.Visible, Is.EqualTo(true));
            Assert.That(way.Version, Is.EqualTo(1));
            Assert.That(way.ChangeSetId, Is.EqualTo(6570367));
            Assert.That(way.TimeStamp.Value.ToUniversalTime(), Is.EqualTo(new DateTime(2010, 12, 06, 23, 58, 37)));
            Assert.That(way.Tags, Is.Not.Null);
            Assert.That(way.Tags.Count, Is.EqualTo(1));
            Assert.That(way.Tags.Contains("building", "yes"), Is.True);
            Assert.That(way.Nodes, Is.Not.Null);
            Assert.That(way.Nodes.Length, Is.EqualTo(5));
            Assert.That(way.Nodes[0], Is.EqualTo(1025709357));
            Assert.That(way.Nodes[1], Is.EqualTo(1025709360));
            Assert.That(way.Nodes[2], Is.EqualTo(1025709358));
            Assert.That(way.Nodes[3], Is.EqualTo(1025709344));
            Assert.That(way.Nodes[4], Is.EqualTo(1025709357));
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
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0], Is.InstanceOf<Relation>());
            var relation = result[0] as Relation;
            Assert.That(relation.Id, Is.EqualTo(214314));
            Assert.That(relation.UserName, Is.EqualTo("marc12"));
            Assert.That(relation.UserId, Is.EqualTo(540527));
            Assert.That(relation.Visible, Is.EqualTo(true));
            Assert.That(relation.Version, Is.EqualTo(18));
            Assert.That(relation.ChangeSetId, Is.EqualTo(9797825));
            Assert.That(relation.TimeStamp.Value.ToUniversalTime(), Is.EqualTo(new DateTime(2011, 11, 11, 16, 42, 26)));
            Assert.That(relation.Tags, Is.Not.Null);
            Assert.That(relation.Tags.Count, Is.EqualTo(4));
            Assert.That(relation.Tags.Contains("network", "rcn"), Is.True);
            Assert.That(relation.Tags.Contains("note", "53-80"), Is.True);
            Assert.That(relation.Tags.Contains("route", "bicycle"), Is.True);
            Assert.That(relation.Tags.Contains("type", "route"), Is.True);
            Assert.That(relation.Members, Is.Not.Null);
            Assert.That(relation.Members.Length, Is.EqualTo(13));

            Assert.That(relation.Members[0].Role, Is.EqualTo(string.Empty));
            Assert.That(relation.Members[0].Type, Is.EqualTo(OsmGeoType.Way));
            Assert.That(relation.Members[0].Id, Is.EqualTo(37294428));

            Assert.That(relation.Members[1].Role, Is.EqualTo("forward"));
            Assert.That(relation.Members[1].Type, Is.EqualTo(OsmGeoType.Way));
            Assert.That(relation.Members[1].Id, Is.EqualTo(87492000));

            Assert.That(relation.Members[2].Role, Is.EqualTo("forward"));
            Assert.That(relation.Members[2].Type, Is.EqualTo(OsmGeoType.Way));
            Assert.That(relation.Members[2].Id, Is.EqualTo(37682837));

            Assert.That(relation.Members[3].Role, Is.EqualTo("forward"));
            Assert.That(relation.Members[3].Type, Is.EqualTo(OsmGeoType.Way));
            Assert.That(relation.Members[3].Id, Is.EqualTo(88614492));

            Assert.That(relation.Members[4].Role, Is.EqualTo(string.Empty));
            Assert.That(relation.Members[4].Type, Is.EqualTo(OsmGeoType.Way));
            Assert.That(relation.Members[4].Id, Is.EqualTo(88614520));

            Assert.That(relation.Members[5].Role, Is.EqualTo(string.Empty));
            Assert.That(relation.Members[5].Type, Is.EqualTo(OsmGeoType.Way));
            Assert.That(relation.Members[5].Id, Is.EqualTo(39448130));

            Assert.That(relation.Members[6].Role, Is.EqualTo(string.Empty));
            Assert.That(relation.Members[6].Type, Is.EqualTo(OsmGeoType.Way));
            Assert.That(relation.Members[6].Id, Is.EqualTo(39364233));

            Assert.That(relation.Members[7].Role, Is.EqualTo(string.Empty));
            Assert.That(relation.Members[7].Type, Is.EqualTo(OsmGeoType.Way));
            Assert.That(relation.Members[7].Id, Is.EqualTo(52285585));

            Assert.That(relation.Members[8].Role, Is.EqualTo(string.Empty));
            Assert.That(relation.Members[8].Type, Is.EqualTo(OsmGeoType.Way));
            Assert.That(relation.Members[8].Id, Is.EqualTo(39364232));

            Assert.That(relation.Members[9].Role, Is.EqualTo(string.Empty));
            Assert.That(relation.Members[9].Type, Is.EqualTo(OsmGeoType.Way));
            Assert.That(relation.Members[9].Id, Is.EqualTo(136621092));

            Assert.That(relation.Members[10].Role, Is.EqualTo(string.Empty));
            Assert.That(relation.Members[10].Type, Is.EqualTo(OsmGeoType.Way));
            Assert.That(relation.Members[10].Id, Is.EqualTo(88195311));

            Assert.That(relation.Members[11].Role, Is.EqualTo(string.Empty));
            Assert.That(relation.Members[11].Type, Is.EqualTo(OsmGeoType.Way));
            Assert.That(relation.Members[11].Id, Is.EqualTo(88195309));

            Assert.That(relation.Members[12].Role, Is.EqualTo(string.Empty));
            Assert.That(relation.Members[12].Type, Is.EqualTo(OsmGeoType.Way));
            Assert.That(relation.Members[12].Id, Is.EqualTo(88195313));
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

        /// <summary>
        /// Reads a real OSM-XML file.
        /// </summary>
        [Test]
        public void ReadRealXML()
        {
            using (var fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "OsmSharp.Test.data.xml.wechel.osm"))
            {
                var wechel = new List<OsmGeo>();
                using (var reader = new XmlOsmStreamSource(fileStream))
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
        public void ReadRealXMLNonSeekable()
        {
            using (var fileStream = new NonSeekableStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "OsmSharp.Test.data.xml.wechel.osm")))
            {
                var wechel = new List<OsmGeo>();
                using (var reader = new XmlOsmStreamSource(fileStream))
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
        public void ReadRealXMLNotAtBeginning()
        {
            var offsetMemoryStream = new MemoryStream();
            offsetMemoryStream.Write(new byte[235], 0, 235);
            using (var fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "OsmSharp.Test.data.xml.wechel.osm"))
            {
                fileStream.CopyTo(offsetMemoryStream);
            }
            offsetMemoryStream.Seek(235, SeekOrigin.Begin);

            var wechel = new List<OsmGeo>();
            using (var reader = new XmlOsmStreamSource(offsetMemoryStream))
            {
                wechel.AddRange(reader);
            }

            Assert.That(wechel.Count, Is.EqualTo(13978));
        }

        /// <summary>
        /// Tests reading an actual OSM-XML file from a non-seekable stream.
        /// </summary>
        [Test]
        public void ReadRealXMLNonSeekableNotAtBeginning()
        {
            var offsetMemoryStream = new MemoryStream();
            offsetMemoryStream.Write(new byte[235], 0, 235);
            using (var fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "OsmSharp.Test.data.xml.wechel.osm"))
            {
                fileStream.CopyTo(offsetMemoryStream);
            }
            offsetMemoryStream.Seek(235, SeekOrigin.Begin);

            var wechel = new List<OsmGeo>();
            using (var reader = new XmlOsmStreamSource(new NonSeekableStream(offsetMemoryStream)))
            {
                wechel.AddRange(reader);
            }

            Assert.That(wechel.Count, Is.EqualTo(13978));
        }

        /// <summary>
        /// Tests reading from a stream where position is not available.
        /// </summary>
        [Test]
        public void XmlOsmStreamSource_ShouldBeAbleToReadFromStreamWithPositionNotAvailable()
        {
            using (var fileStream = new DeflateMockStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "OsmSharp.Test.data.xml.wechel.osm")))
            {
                var wechel = new List<OsmGeo>();
                using (var reader = new XmlOsmStreamSource(fileStream))
                {
                    wechel.AddRange(reader);
                }

                Assert.That(wechel.Count, Is.EqualTo(13978));
            }
        }
    }
}
