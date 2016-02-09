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

using NUnit.Framework;
using OsmSharp.IO.PBF;
using System;
using System.Collections.Generic;

namespace OsmSharp.Test.IO.PBF
{
    /// <summary>
    /// Tests the PBF encoder.
    /// </summary>
    [TestFixture]
    public class EncoderTests
    {
        /// <summary>
        /// Tests encoding a lat/lon values.
        /// </summary>
        [Test]
        public void TestEncodeLatLon()
        {
            Assert.AreEqual(0, Encoder.EncodeLatLon(0, 0, 100));
            Assert.AreEqual(900000000, Encoder.EncodeLatLon(90, 0, 100));
            Assert.AreEqual(-900000000, Encoder.EncodeLatLon(-90, 0, 100));

            Assert.AreEqual(0, Encoder.EncodeLatLon(0, 0, 100));
            Assert.AreEqual(1800000000, Encoder.EncodeLatLon(180, 0, 100));
            Assert.AreEqual(-1800000000, Encoder.EncodeLatLon(-180, 0, 100));
        }

        /// <summary>
        /// Tests decoding lat/lon values.
        /// </summary>
        [Test]
        public void TestDecodeLatLon()
        {
            Assert.AreEqual(0, Encoder.DecodeLatLon(0, 0, 100));
            Assert.AreEqual(90, Encoder.DecodeLatLon(900000000, 0, 100));
            Assert.AreEqual(-90, Encoder.DecodeLatLon(-900000000, 0, 100));

            Assert.AreEqual(0, Encoder.DecodeLatLon(0, 0, 100));
            Assert.AreEqual(180, Encoder.DecodeLatLon(1800000000, 0, 100));
            Assert.AreEqual(-180, Encoder.DecodeLatLon(-1800000000, 0, 100));
        }

        /// <summary>
        /// Tests encoding strings.
        /// </summary>
        [Test]
        public void TestEncodeStrings()
        {
            var block = new PrimitiveBlock();
            var reverseStringTable = new Dictionary<string, int>();
            Assert.AreEqual(1, Encoder.EncodeString(block, reverseStringTable, "Ben"));
            Assert.AreEqual(2, Encoder.EncodeString(block, reverseStringTable, "highway"));
            Assert.AreEqual(3, Encoder.EncodeString(block, reverseStringTable, "residential"));
            Assert.AreEqual(1, Encoder.EncodeString(block, reverseStringTable, "Ben"));
            Assert.AreEqual(2, Encoder.EncodeString(block, reverseStringTable, "highway"));
            Assert.AreEqual(4, Encoder.EncodeString(block, reverseStringTable, "Some other string"));
            Assert.AreEqual(5, Encoder.EncodeString(block, reverseStringTable, "Ban"));
        }

        /// <summary>
        /// Tests decoding a node.
        /// </summary>
        [Test]
        public void TestDecodeNode()
        {
            var block = new PrimitiveBlock();
            block.date_granularity = 1000;
            block.granularity = 100;
            block.lat_offset = 0;
            block.lon_offset = 0;
            block.stringtable = new StringTable();
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes(string.Empty)); // always encode empty string as '0'.
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes("highway"));
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes("residential"));
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes("Ben"));

            var pbfNode = new OsmSharp.IO.PBF.Node()
            {
                id = 1,
                info = new Info()
                {
                    changeset = 10,
                    timestamp = 10,
                    uid = 100,
                    user_sid = 3,
                    version = 2
                },
                lat = Encoder.EncodeLatLon(10.9f, block.lat_offset, block.granularity),
                lon = Encoder.EncodeLatLon(11.0f, block.lat_offset, block.granularity)
            };
            pbfNode.keys.Add(1);
            pbfNode.vals.Add(2);

            var node = Encoder.DecodeNode(block, pbfNode);
            Assert.IsNotNull(node);
            Assert.AreEqual(1, node.Id);
            Assert.AreEqual(10, node.ChangeSetId);
            Assert.AreEqual(10.9f, node.Latitude);
            Assert.AreEqual(11.0f, node.Longitude);
            Assert.AreEqual(PBFExtensions.FromUnixTime(10000), node.TimeStamp);
            Assert.AreEqual(OsmSharp.OsmGeoType.Node, node.Type);
            Assert.AreEqual(100, node.UserId);
            Assert.AreEqual("Ben", node.UserName);
            Assert.AreEqual(2, node.Version);
        }

        /// <summary>
        /// Tests encoding a node.
        /// </summary>
        [Test]
        public void TestEncodeNode()
        {
            var block = new PrimitiveBlock();
            block.date_granularity = 1000;
            block.granularity = 100;
            block.lat_offset = 0;
            block.lon_offset = 0;

            var node = new OsmSharp.Node();
            node.Id = 1;
            node.ChangeSetId = 1;
            node.Latitude = 10;
            node.Longitude = 11;
            node.Tags = new OsmSharp.Tags.TagsCollection();
            node.Tags.Add("name", "Ben");
            node.TimeStamp = DateTime.Now;
            node.UserId = 1;
            node.UserName = "Ben";
            node.Version = 1;
            node.Visible = true;

            var pbfNode = Encoder.EncodeNode(block, new Dictionary<string, int>(), node);
            Assert.IsNotNull(pbfNode);
            Assert.AreEqual(1, pbfNode.id);
            Assert.AreEqual(Encoder.EncodeLatLon(10, block.lat_offset, block.granularity), pbfNode.lat);
            Assert.AreEqual(Encoder.EncodeLatLon(11, block.lon_offset, block.granularity), pbfNode.lon);
            Assert.AreEqual(1, pbfNode.info.changeset);
            Assert.AreEqual(Encoder.EncodeTimestamp(node.TimeStamp.Value, block.date_granularity), pbfNode.info.timestamp);
            Assert.AreEqual(1, pbfNode.info.uid);
            Assert.AreEqual("Ben", System.Text.Encoding.UTF8.GetString(block.stringtable.s[(int)pbfNode.info.user_sid]));
            Assert.AreEqual(1, pbfNode.info.version);
            Assert.AreEqual(1, pbfNode.keys.Count);
            Assert.AreEqual("name", System.Text.Encoding.UTF8.GetString(block.stringtable.s[(int)pbfNode.keys[0]]));
            Assert.AreEqual(1, pbfNode.vals.Count);
            Assert.AreEqual("Ben", System.Text.Encoding.UTF8.GetString(block.stringtable.s[(int)pbfNode.vals[0]]));
        }

        /// <summary>
        /// Tests decoding a way.
        /// </summary>
        [Test]
        public void TestDecodeWay()
        {
            var block = new PrimitiveBlock();
            block.date_granularity = 1000;
            block.granularity = 100;
            block.lat_offset = 0;
            block.lon_offset = 0;
            block.stringtable = new StringTable();
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes(string.Empty)); // always encode empty string as '0'.
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes("highway"));
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes("residential"));
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes("Ben"));

            var pbfWay = new OsmSharp.IO.PBF.Way()
            {
                id = 1,
                info = new Info()
                {
                    changeset = 10,
                    timestamp = 10,
                    uid = 100,
                    user_sid = 3,
                    version = 2
                }
            };
            pbfWay.keys.Add(1);
            pbfWay.vals.Add(2);
            pbfWay.refs.Add(0);
            pbfWay.refs.Add(1);

            var way = Encoder.DecodeWay(block, pbfWay);
            Assert.IsNotNull(way);
            Assert.AreEqual(1, way.Id);
            Assert.AreEqual(10, way.ChangeSetId);
            Assert.AreEqual(PBFExtensions.FromUnixTime(10000), way.TimeStamp);
            Assert.AreEqual(OsmSharp.OsmGeoType.Way, way.Type);
            Assert.AreEqual(100, way.UserId);
            Assert.AreEqual("Ben", way.UserName);
            Assert.AreEqual(2, way.Version);
            Assert.AreEqual(2, way.Nodes.Length);
            Assert.AreEqual(0, way.Nodes[0]);
            Assert.AreEqual(1, way.Nodes[1]);
        }

        /// <summary>
        /// Tests encoding a way.
        /// </summary>
        [Test]
        public void TestEncodeWay()
        {
            var block = new PrimitiveBlock();
            block.date_granularity = 1000;
            block.granularity = 100;
            block.lat_offset = 0;
            block.lon_offset = 0;
            block.stringtable = new StringTable();
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes(string.Empty)); // always encode empty string as '0'.
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes("highway"));
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes("residential"));
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes("Ben"));

            var way = new OsmSharp.Way();
            way.Id = 1;
            way.ChangeSetId = 1;
            way.Tags = new OsmSharp.Tags.TagsCollection();
            way.Tags.Add("name", "Ben");
            way.TimeStamp = DateTime.Now;
            way.UserId = 1;
            way.UserName = "Ben";
            way.Version = 1;
            way.Visible = true;
            way.Nodes = new long[] { 1, 2 };

            var pbfWay = Encoder.EncodeWay(block, new Dictionary<string, int>(), way);
            Assert.IsNotNull(pbfWay);
            Assert.AreEqual(1, pbfWay.id);
            Assert.AreEqual(2, pbfWay.refs.Count);
            Assert.AreEqual(1, pbfWay.refs[0]);
            Assert.AreEqual(1, pbfWay.refs[1]);
            Assert.AreEqual(1, pbfWay.info.changeset);
            Assert.AreEqual(Encoder.EncodeTimestamp(way.TimeStamp.Value, block.date_granularity), pbfWay.info.timestamp);
            Assert.AreEqual(1, pbfWay.info.uid);
            Assert.AreEqual("Ben", System.Text.Encoding.UTF8.GetString(block.stringtable.s[(int)pbfWay.info.user_sid]));
            Assert.AreEqual(1, pbfWay.info.version);
            Assert.AreEqual(1, pbfWay.keys.Count);
            Assert.AreEqual("name", System.Text.Encoding.UTF8.GetString(block.stringtable.s[(int)pbfWay.keys[0]]));
            Assert.AreEqual(1, pbfWay.vals.Count);
            Assert.AreEqual("Ben", System.Text.Encoding.UTF8.GetString(block.stringtable.s[(int)pbfWay.vals[0]]));
        }

        /// <summary>
        /// Tests decoding a relation.
        /// </summary>
        [Test]
        public void TestDecodeRelation()
        {
            var block = new PrimitiveBlock();
            block.date_granularity = 1000;
            block.granularity = 100;
            block.lat_offset = 0;
            block.lon_offset = 0;
            block.stringtable = new StringTable();
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes(string.Empty)); // always encode empty string as '0'.
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes("highway"));
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes("residential"));
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes("Ben"));
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes("fake role"));

            var pbfRelation = new OsmSharp.IO.PBF.Relation()
            {
                id = 1,
                info = new Info()
                {
                    changeset = 10,
                    timestamp = 10,
                    uid = 100,
                    user_sid = 3,
                    version = 2
                }
            };
            pbfRelation.keys.Add(1);
            pbfRelation.vals.Add(2);
            pbfRelation.memids.Add(10);
            pbfRelation.memids.Add(1); // delta-encoding.
            pbfRelation.roles_sid.Add(4);
            pbfRelation.roles_sid.Add(4);
            pbfRelation.types.Add(OsmSharp.IO.PBF.Relation.MemberType.NODE);
            pbfRelation.types.Add(OsmSharp.IO.PBF.Relation.MemberType.WAY);

            var relation = Encoder.DecodeRelation(block, pbfRelation);
            Assert.IsNotNull(relation);
            Assert.AreEqual(1, relation.Id);
            Assert.AreEqual(10, relation.ChangeSetId);
            Assert.AreEqual(PBFExtensions.FromUnixTime(10000), relation.TimeStamp);
            Assert.AreEqual(OsmSharp.OsmGeoType.Relation, relation.Type);
            Assert.AreEqual(100, relation.UserId);
            Assert.AreEqual("Ben", relation.UserName);
            Assert.AreEqual(2, relation.Version);
            Assert.AreEqual(2, relation.Members.Length);
            Assert.AreEqual(10, relation.Members[0].Id);
            Assert.AreEqual(OsmSharp.OsmGeoType.Node, relation.Members[0].Type);
            Assert.AreEqual("fake role", relation.Members[0].Role);
            Assert.AreEqual(11, relation.Members[1].Id);
            Assert.AreEqual(OsmSharp.OsmGeoType.Way, relation.Members[1].Type);
            Assert.AreEqual("fake role", relation.Members[1].Role);
        }

        /// <summary>
        /// Tests encoding a relation.
        /// </summary>
        [Test]
        public void TestEncodeRelation()
        {
            var block = new PrimitiveBlock();
            block.date_granularity = 1000;
            block.granularity = 100;
            block.lat_offset = 0;
            block.lon_offset = 0;
            block.stringtable = new StringTable();
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes(string.Empty)); // always encode empty string as '0'.
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes("highway"));
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes("residential"));
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes("Ben"));

            var relation = new OsmSharp.Relation();
            relation.Id = 1;
            relation.ChangeSetId = 1;
            relation.Tags = new OsmSharp.Tags.TagsCollection();
            relation.Tags.Add("name", "Ben");
            relation.TimeStamp = DateTime.Now;
            relation.UserId = 1;
            relation.UserName = "Ben";
            relation.Version = 1;
            relation.Visible = true;
            relation.Members = new RelationMember[] 
            {
                new OsmSharp.RelationMember()
                {
                    Id = 1,
                    Role = "fake role1",
                    Type = OsmSharp.OsmGeoType.Node
                },
                new OsmSharp.RelationMember()
                {
                    Id = 2,
                    Role = "fake role2",
                    Type = OsmSharp.OsmGeoType.Relation
                }
            };

            var pbfRelation = Encoder.EncodeRelation(block, new Dictionary<string, int>(), relation);
            Assert.IsNotNull(pbfRelation);
            Assert.AreEqual(1, pbfRelation.id);
            Assert.AreEqual(2, pbfRelation.memids.Count);
            Assert.AreEqual(1, pbfRelation.memids[0]);
            Assert.AreEqual(1, pbfRelation.memids[1]);
            Assert.AreEqual(2, pbfRelation.roles_sid.Count);
            Assert.AreEqual("fake role1", System.Text.Encoding.UTF8.GetString(block.stringtable.s[(int)pbfRelation.roles_sid[0]]));
            Assert.AreEqual("fake role2", System.Text.Encoding.UTF8.GetString(block.stringtable.s[(int)pbfRelation.roles_sid[1]]));
            Assert.AreEqual(2, pbfRelation.types.Count);
            Assert.AreEqual(OsmSharp.IO.PBF.Relation.MemberType.NODE, pbfRelation.types[0]);
            Assert.AreEqual(OsmSharp.IO.PBF.Relation.MemberType.RELATION, pbfRelation.types[1]);
            Assert.AreEqual(1, pbfRelation.info.changeset);
            Assert.AreEqual(Encoder.EncodeTimestamp(relation.TimeStamp.Value, block.date_granularity), pbfRelation.info.timestamp);
            Assert.AreEqual(1, pbfRelation.info.uid);
            Assert.AreEqual("Ben", System.Text.Encoding.UTF8.GetString(block.stringtable.s[(int)pbfRelation.info.user_sid]));
            Assert.AreEqual(1, pbfRelation.info.version);
            Assert.AreEqual(1, pbfRelation.keys.Count);
            Assert.AreEqual("name", System.Text.Encoding.UTF8.GetString(block.stringtable.s[(int)pbfRelation.keys[0]]));
            Assert.AreEqual(1, pbfRelation.vals.Count);
            Assert.AreEqual("Ben", System.Text.Encoding.UTF8.GetString(block.stringtable.s[(int)pbfRelation.vals[0]]));
        }

        /// <summary>
        /// Tests decoding a block with one with one node.
        /// </summary>
        [Test]
        public void TestDecodeBlockWithNode()
        {
            var block = new PrimitiveBlock();
            block.date_granularity = 1000;
            block.granularity = 100;
            block.lat_offset = 0;
            block.lon_offset = 0;
            block.stringtable = new StringTable();
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes("highway"));
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes("residential"));
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes("Ben"));

            var node = new OsmSharp.IO.PBF.Node()
            {
                id = 1,
                info = new Info()
                {
                    changeset = 10,
                    timestamp = 10,
                    uid = 100,
                    user_sid = 2,
                    version = 2
                },
                lat = Encoder.EncodeLatLon(10.9f, block.lat_offset, block.granularity),
                lon = Encoder.EncodeLatLon(11.0f, block.lat_offset, block.granularity)
            };
            node.keys.Add(0);
            node.vals.Add(1);

            var primitiveGroup = new PrimitiveGroup();
            primitiveGroup.nodes.Add(node);
            block.primitivegroup.Add(primitiveGroup);

            var primitivesConsumer = new PrimitivesConsumerMock();
            block.Decode(primitivesConsumer, false, false, false);

            Assert.AreEqual(1, primitivesConsumer.Nodes.Count);
            Assert.AreEqual(0, primitivesConsumer.Ways.Count);
            Assert.AreEqual(0, primitivesConsumer.Relations.Count);
        }

        /// <summary>
        /// Tests decoding a block with one with several dense nodes.
        /// </summary>
        [Test]
        public void TestDecodeBlockWithDenseNodes()
        {
            var block = new PrimitiveBlock();
            block.date_granularity = 1000;
            block.granularity = 100;
            block.lat_offset = 0;
            block.lon_offset = 0;
            block.stringtable = new StringTable();
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes(string.Empty)); // always encode empty string as '0'.
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes("highway")); // 1
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes("residential")); // 2
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes("Ben")); // 3
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes("track")); // 4
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes("trunk")); // 5

            var primitiveGroup = new PrimitiveGroup();
            primitiveGroup.dense = new DenseNodes();
            primitiveGroup.dense.denseinfo = new DenseInfo();
            primitiveGroup.dense.denseinfo.changeset.Add(10);
            primitiveGroup.dense.denseinfo.changeset.Add(1);
            primitiveGroup.dense.denseinfo.changeset.Add(1);

            primitiveGroup.dense.denseinfo.timestamp.Add(10);
            primitiveGroup.dense.denseinfo.timestamp.Add(1);
            primitiveGroup.dense.denseinfo.timestamp.Add(1);

            primitiveGroup.dense.denseinfo.uid.Add(1);
            primitiveGroup.dense.denseinfo.uid.Add(0);
            primitiveGroup.dense.denseinfo.uid.Add(0);

            primitiveGroup.dense.denseinfo.user_sid.Add(3);
            primitiveGroup.dense.denseinfo.user_sid.Add(0);
            primitiveGroup.dense.denseinfo.user_sid.Add(0);

            primitiveGroup.dense.denseinfo.version.Add(1);
            primitiveGroup.dense.denseinfo.version.Add(1);
            primitiveGroup.dense.denseinfo.version.Add(1);

            primitiveGroup.dense.id.Add(1);
            primitiveGroup.dense.id.Add(1);
            primitiveGroup.dense.id.Add(1);

            primitiveGroup.dense.keys_vals.Add(1);
            primitiveGroup.dense.keys_vals.Add(2);
            primitiveGroup.dense.keys_vals.Add(0); // highway=residential.
            primitiveGroup.dense.keys_vals.Add(1);
            primitiveGroup.dense.keys_vals.Add(4);
            primitiveGroup.dense.keys_vals.Add(0); // highway=track.
            primitiveGroup.dense.keys_vals.Add(0); // empty.

            primitiveGroup.dense.lat.Add(Encoder.EncodeLatLon(10.0f, block.lat_offset, block.granularity));
            primitiveGroup.dense.lat.Add(Encoder.EncodeLatLon(11.0f, block.lat_offset, block.granularity)
                - primitiveGroup.dense.lat[primitiveGroup.dense.lat.Count - 1]);
            primitiveGroup.dense.lat.Add(Encoder.EncodeLatLon(12.0f, block.lat_offset, block.granularity)
                - primitiveGroup.dense.lat[primitiveGroup.dense.lat.Count - 1]);

            primitiveGroup.dense.lon.Add(Encoder.EncodeLatLon(100.0f, block.lon_offset, block.granularity));
            primitiveGroup.dense.lon.Add(Encoder.EncodeLatLon(110.0f, block.lon_offset, block.granularity)
                - primitiveGroup.dense.lon[primitiveGroup.dense.lon.Count - 1]);
            primitiveGroup.dense.lon.Add(Encoder.EncodeLatLon(120.0f, block.lon_offset, block.granularity)
                - primitiveGroup.dense.lon[primitiveGroup.dense.lon.Count - 1]);

            block.primitivegroup.Add(primitiveGroup);

            var primitivesConsumer = new PrimitivesConsumerMock();
            block.Decode(primitivesConsumer, false, false, false);

            Assert.AreEqual(3, primitivesConsumer.Nodes.Count);
            Assert.AreEqual(0, primitivesConsumer.Ways.Count);
            Assert.AreEqual(0, primitivesConsumer.Relations.Count);

            var node = primitivesConsumer.Nodes[0];
            Assert.IsNotNull(node);
            Assert.AreEqual(1, node.id);
            Assert.AreEqual(10, node.info.changeset);
            Assert.AreEqual(10, node.info.timestamp);
            Assert.AreEqual(1, node.info.uid);
            Assert.AreEqual(3, node.info.user_sid);
            Assert.AreEqual(1, node.info.version);
            Assert.AreEqual(1, node.keys.Count);
            Assert.AreEqual(1, node.keys[0]);
            Assert.AreEqual(1, node.vals.Count);
            Assert.AreEqual(2, node.vals[0]);

            node = primitivesConsumer.Nodes[1];
            Assert.IsNotNull(node);
            Assert.AreEqual(2, node.id);
            Assert.AreEqual(11, node.info.changeset);
            Assert.AreEqual(11, node.info.timestamp);
            Assert.AreEqual(1, node.info.uid);
            Assert.AreEqual(3, node.info.user_sid);
            Assert.AreEqual(2, node.info.version);
            Assert.AreEqual(1, node.keys.Count);
            Assert.AreEqual(1, node.keys[0]);
            Assert.AreEqual(1, node.vals.Count);
            Assert.AreEqual(4, node.vals[0]);

            node = primitivesConsumer.Nodes[2];
            Assert.IsNotNull(node);
            Assert.AreEqual(3, node.id);
            Assert.AreEqual(12, node.info.changeset);
            Assert.AreEqual(12, node.info.timestamp);
            Assert.AreEqual(1, node.info.uid);
            Assert.AreEqual(3, node.info.user_sid);
            Assert.AreEqual(3, node.info.version);
            Assert.AreEqual(0, node.keys.Count);
            Assert.AreEqual(0, node.vals.Count);
        }

        /// <summary>
        /// Tests decoding a block with one with one way.
        /// </summary>
        [Test]
        public void TestDecodeBlockWithWay()
        {
            var block = new PrimitiveBlock();
            block.date_granularity = 1000;
            block.granularity = 100;
            block.lat_offset = 0;
            block.lon_offset = 0;
            block.stringtable = new StringTable();
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes("highway"));
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes("residential"));
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes("Ben"));

            var pbfWay = new OsmSharp.IO.PBF.Way()
            {
                id = 1,
                info = new Info()
                {
                    changeset = 10,
                    timestamp = 10,
                    uid = 100,
                    user_sid = 2,
                    version = 2
                }
            };
            pbfWay.keys.Add(0);
            pbfWay.vals.Add(1);
            pbfWay.refs.Add(0);
            pbfWay.refs.Add(1);

            var primitiveGroup = new PrimitiveGroup();
            primitiveGroup.ways.Add(pbfWay);
            block.primitivegroup.Add(primitiveGroup);

            var primitivesConsumer = new PrimitivesConsumerMock();
            block.Decode(primitivesConsumer, false, false, false);

            Assert.AreEqual(0, primitivesConsumer.Nodes.Count);
            Assert.AreEqual(1, primitivesConsumer.Ways.Count);
            Assert.AreEqual(0, primitivesConsumer.Relations.Count);
        }

        /// <summary>
        /// Tests decoding a block with one with one relation.
        /// </summary>
        [Test]
        public void TestDecodeBlockWithRelation()
        {
            var block = new PrimitiveBlock();
            block.date_granularity = 1000;
            block.granularity = 100;
            block.lat_offset = 0;
            block.lon_offset = 0;
            block.stringtable = new StringTable();
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes("highway"));
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes("residential"));
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes("Ben"));

            var pbfRelation = new OsmSharp.IO.PBF.Relation()
            {
                id = 1,
                info = new Info()
                {
                    changeset = 10,
                    timestamp = 10,
                    uid = 100,
                    user_sid = 2,
                    version = 2
                }
            };
            pbfRelation.keys.Add(0);
            pbfRelation.vals.Add(1);
            pbfRelation.memids.Add(10);
            pbfRelation.memids.Add(1); // delta-encoding.
            pbfRelation.roles_sid.Add(3);
            pbfRelation.roles_sid.Add(3);
            pbfRelation.types.Add(OsmSharp.IO.PBF.Relation.MemberType.NODE);
            pbfRelation.types.Add(OsmSharp.IO.PBF.Relation.MemberType.WAY);

            var primitiveGroup = new PrimitiveGroup();
            primitiveGroup.relations.Add(pbfRelation);
            block.primitivegroup.Add(primitiveGroup);

            var primitivesConsumer = new PrimitivesConsumerMock();
            block.Decode(primitivesConsumer, false, false, false);

            Assert.AreEqual(0, primitivesConsumer.Nodes.Count);
            Assert.AreEqual(0, primitivesConsumer.Ways.Count);
            Assert.AreEqual(1, primitivesConsumer.Relations.Count);
        }
    }
}