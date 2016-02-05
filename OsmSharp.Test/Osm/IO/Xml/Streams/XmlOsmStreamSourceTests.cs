// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2016 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using System.Reflection;
using NUnit.Framework;
using OsmSharp.Osm.Streams;
using System.Collections.Generic;
using OsmSharp.Osm;
using System;

namespace OsmSharp.Test.Osm.IO.Xml.Streams
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
                    "OsmSharp.Test.data.node.osm"));

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
            Assert.AreEqual(new DateTime(2011, 11, 11, 16, 43, 47), node.TimeStamp);
            Assert.IsNotNull(node.Tags);
            Assert.AreEqual(3, node.Tags.Count);
            Assert.IsTrue(node.Tags.ContainsKeyValue("alt_name", "Lille"));
            Assert.IsTrue(node.Tags.ContainsKeyValue("name", "Wechelderzande"));
            Assert.IsTrue(node.Tags.ContainsKeyValue("traffic_sign", "city_limit"));
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
                    "OsmSharp.Test.data.way.osm"));

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
            Assert.AreEqual(new DateTime(2010, 12, 06, 23, 58, 37), way.TimeStamp);
            Assert.IsNotNull(way.Tags);
            Assert.AreEqual(1, way.Tags.Count);
            Assert.IsTrue(way.Tags.ContainsKeyValue("building", "yes"));
            Assert.IsNotNull(way.Nodes);
            Assert.AreEqual(5, way.Nodes.Count);
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
                    "OsmSharp.Test.data.relation.osm"));

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
            Assert.AreEqual(new DateTime(2011, 11, 11, 16, 42, 26), relation.TimeStamp);
            Assert.IsNotNull(relation.Tags);
            Assert.AreEqual(4, relation.Tags.Count);
            Assert.IsTrue(relation.Tags.ContainsKeyValue("network", "rcn"));
            Assert.IsTrue(relation.Tags.ContainsKeyValue("note", "53-80"));
            Assert.IsTrue(relation.Tags.ContainsKeyValue("route", "bicycle"));
            Assert.IsTrue(relation.Tags.ContainsKeyValue("type", "route"));
            Assert.IsNotNull(relation.Members);
            Assert.AreEqual(13, relation.Members.Count);

            Assert.AreEqual(string.Empty, relation.Members[0].MemberRole);
            Assert.AreEqual(OsmGeoType.Way, relation.Members[0].MemberType);
            Assert.AreEqual(37294428, relation.Members[0].MemberId);

            Assert.AreEqual("forward", relation.Members[1].MemberRole);
            Assert.AreEqual(OsmGeoType.Way, relation.Members[1].MemberType);
            Assert.AreEqual(87492000, relation.Members[1].MemberId);

            Assert.AreEqual("forward", relation.Members[2].MemberRole);
            Assert.AreEqual(OsmGeoType.Way, relation.Members[2].MemberType);
            Assert.AreEqual(37682837, relation.Members[2].MemberId);

            Assert.AreEqual("forward", relation.Members[3].MemberRole);
            Assert.AreEqual(OsmGeoType.Way, relation.Members[3].MemberType);
            Assert.AreEqual(88614492, relation.Members[3].MemberId);

            Assert.AreEqual(string.Empty, relation.Members[4].MemberRole);
            Assert.AreEqual(OsmGeoType.Way, relation.Members[4].MemberType);
            Assert.AreEqual(88614520, relation.Members[4].MemberId);

            Assert.AreEqual(string.Empty, relation.Members[5].MemberRole);
            Assert.AreEqual(OsmGeoType.Way, relation.Members[5].MemberType);
            Assert.AreEqual(39448130, relation.Members[5].MemberId);

            Assert.AreEqual(string.Empty, relation.Members[6].MemberRole);
            Assert.AreEqual(OsmGeoType.Way, relation.Members[6].MemberType);
            Assert.AreEqual(39364233, relation.Members[6].MemberId);

            Assert.AreEqual(string.Empty, relation.Members[7].MemberRole);
            Assert.AreEqual(OsmGeoType.Way, relation.Members[7].MemberType);
            Assert.AreEqual(52285585, relation.Members[7].MemberId);

            Assert.AreEqual(string.Empty, relation.Members[8].MemberRole);
            Assert.AreEqual(OsmGeoType.Way, relation.Members[8].MemberType);
            Assert.AreEqual(39364232, relation.Members[8].MemberId);

            Assert.AreEqual(string.Empty, relation.Members[9].MemberRole);
            Assert.AreEqual(OsmGeoType.Way, relation.Members[9].MemberType);
            Assert.AreEqual(136621092, relation.Members[9].MemberId);

            Assert.AreEqual(string.Empty, relation.Members[10].MemberRole);
            Assert.AreEqual(OsmGeoType.Way, relation.Members[10].MemberType);
            Assert.AreEqual(88195311, relation.Members[10].MemberId);

            Assert.AreEqual(string.Empty, relation.Members[11].MemberRole);
            Assert.AreEqual(OsmGeoType.Way, relation.Members[11].MemberType);
            Assert.AreEqual(88195309, relation.Members[11].MemberId);

            Assert.AreEqual(string.Empty, relation.Members[12].MemberRole);
            Assert.AreEqual(OsmGeoType.Way, relation.Members[12].MemberType);
            Assert.AreEqual(88195313, relation.Members[12].MemberId);
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
                    "OsmSharp.Test.data.api.osm"));

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