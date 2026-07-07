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

using System;
using System.Collections.Generic;
using NUnit.Framework;
using OsmSharp.IO.PBF;

namespace OsmSharp.Test.IO.PBF;

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
        Assert.That(Encoder.EncodeLatLon(0, 0, 100), Is.EqualTo(0));
        Assert.That(Encoder.EncodeLatLon(90, 0, 100), Is.EqualTo(900000000));
        Assert.That(Encoder.EncodeLatLon(-90, 0, 100), Is.EqualTo(-900000000));

        Assert.That(Encoder.EncodeLatLon(0, 0, 100), Is.EqualTo(0));
        Assert.That(Encoder.EncodeLatLon(180, 0, 100), Is.EqualTo(1800000000));
        Assert.That(Encoder.EncodeLatLon(-180, 0, 100), Is.EqualTo(-1800000000));
    }

    /// <summary>
    /// Tests decoding lat/lon values.
    /// </summary>
    [Test]
    public void TestDecodeLatLon()
    {
        Assert.That(Encoder.DecodeLatLon(0, 0, 100), Is.EqualTo(0));
        Assert.That(Encoder.DecodeLatLon(900000000, 0, 100), Is.EqualTo(90));
        Assert.That(Encoder.DecodeLatLon(-900000000, 0, 100), Is.EqualTo(-90));

        Assert.That(Encoder.DecodeLatLon(0, 0, 100), Is.EqualTo(0));
        Assert.That(Encoder.DecodeLatLon(1800000000, 0, 100), Is.EqualTo(180));
        Assert.That(Encoder.DecodeLatLon(-1800000000, 0, 100), Is.EqualTo(-180));
    }

    /// <summary>
    /// Tests encoding strings.
    /// </summary>
    [Test]
    public void TestEncodeStrings()
    {
        var block = new PrimitiveBlock();
        var reverseStringTable = new Dictionary<string, int>();
        Assert.That(Encoder.EncodeString(block, reverseStringTable, "Ben"), Is.EqualTo(1));
        Assert.That(Encoder.EncodeString(block, reverseStringTable, "highway"), Is.EqualTo(2));
        Assert.That(Encoder.EncodeString(block, reverseStringTable, "residential"), Is.EqualTo(3));
        Assert.That(Encoder.EncodeString(block, reverseStringTable, "Ben"), Is.EqualTo(1));
        Assert.That(Encoder.EncodeString(block, reverseStringTable, "highway"), Is.EqualTo(2));
        Assert.That(Encoder.EncodeString(block, reverseStringTable, "Some other string"), Is.EqualTo(4));
        Assert.That(Encoder.EncodeString(block, reverseStringTable, "Ban"), Is.EqualTo(5));
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
            lat = Encoder.EncodeLatLon(10.9, block.lat_offset, block.granularity),
            lon = Encoder.EncodeLatLon(11.0, block.lat_offset, block.granularity)
        };
        pbfNode.keys.Add(1);
        pbfNode.vals.Add(2);

        var node = Encoder.DecodeNode(block, pbfNode);
        Assert.IsNotNull(node);
        Assert.That(node.Id, Is.EqualTo(1));
        Assert.That(node.ChangeSetId, Is.EqualTo(10));
        Assert.That(node.Latitude, Is.EqualTo(10.9));
        Assert.That(node.Longitude, Is.EqualTo(11.0));
        Assert.That(node.TimeStamp, Is.EqualTo(PBFExtensions.FromUnixTime(10000)));
        Assert.That(node.Type, Is.EqualTo(OsmSharp.OsmGeoType.Node));
        Assert.That(node.UserId, Is.EqualTo(100));
        Assert.That(node.UserName, Is.EqualTo("Ben"));
        Assert.That(node.Version, Is.EqualTo(2));
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
        Assert.That(pbfNode.id, Is.EqualTo(1));
        Assert.That(pbfNode.lat, Is.EqualTo(Encoder.EncodeLatLon(10, block.lat_offset, block.granularity)));
        Assert.That(pbfNode.lon, Is.EqualTo(Encoder.EncodeLatLon(11, block.lon_offset, block.granularity)));
        Assert.That(pbfNode.info.changeset, Is.EqualTo(1));
        Assert.That(pbfNode.info.timestamp, Is.EqualTo(Encoder.EncodeTimestamp(node.TimeStamp.Value, block.date_granularity)));
        Assert.That(pbfNode.info.uid, Is.EqualTo(1));
        Assert.That(System.Text.Encoding.UTF8.GetString(block.stringtable.s[(int)pbfNode.info.user_sid]), Is.EqualTo("Ben"));
        Assert.That(pbfNode.info.version, Is.EqualTo(1));
        Assert.That(pbfNode.keys.Count, Is.EqualTo(1));
        Assert.That(System.Text.Encoding.UTF8.GetString(block.stringtable.s[(int)pbfNode.keys[0]]), Is.EqualTo("name"));
        Assert.That(pbfNode.vals.Count, Is.EqualTo(1));
        Assert.That(System.Text.Encoding.UTF8.GetString(block.stringtable.s[(int)pbfNode.vals[0]]), Is.EqualTo("Ben"));
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
        Assert.That(way.Id, Is.EqualTo(1));
        Assert.That(way.ChangeSetId, Is.EqualTo(10));
        Assert.That(way.TimeStamp, Is.EqualTo(PBFExtensions.FromUnixTime(10000)));
        Assert.That(way.Type, Is.EqualTo(OsmSharp.OsmGeoType.Way));
        Assert.That(way.UserId, Is.EqualTo(100));
        Assert.That(way.UserName, Is.EqualTo("Ben"));
        Assert.That(way.Version, Is.EqualTo(2));
        Assert.That(way.Nodes.Length, Is.EqualTo(2));
        Assert.That(way.Nodes[0], Is.EqualTo(0));
        Assert.That(way.Nodes[1], Is.EqualTo(1));
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
        Assert.That(pbfWay.id, Is.EqualTo(1));
        Assert.That(pbfWay.refs.Count, Is.EqualTo(2));
        Assert.That(pbfWay.refs[0], Is.EqualTo(1));
        Assert.That(pbfWay.refs[1], Is.EqualTo(1));
        Assert.That(pbfWay.info.changeset, Is.EqualTo(1));
        Assert.That(pbfWay.info.timestamp, Is.EqualTo(Encoder.EncodeTimestamp(way.TimeStamp.Value, block.date_granularity)));
        Assert.That(pbfWay.info.uid, Is.EqualTo(1));
        Assert.That(System.Text.Encoding.UTF8.GetString(block.stringtable.s[(int)pbfWay.info.user_sid]), Is.EqualTo("Ben"));
        Assert.That(pbfWay.info.version, Is.EqualTo(1));
        Assert.That(pbfWay.keys.Count, Is.EqualTo(1));
        Assert.That(System.Text.Encoding.UTF8.GetString(block.stringtable.s[(int)pbfWay.keys[0]]), Is.EqualTo("name"));
        Assert.That(pbfWay.vals.Count, Is.EqualTo(1));
        Assert.That(System.Text.Encoding.UTF8.GetString(block.stringtable.s[(int)pbfWay.vals[0]]), Is.EqualTo("Ben"));
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
        Assert.That(relation.Id, Is.EqualTo(1));
        Assert.That(relation.ChangeSetId, Is.EqualTo(10));
        Assert.That(relation.TimeStamp, Is.EqualTo(PBFExtensions.FromUnixTime(10000)));
        Assert.That(relation.Type, Is.EqualTo(OsmSharp.OsmGeoType.Relation));
        Assert.That(relation.UserId, Is.EqualTo(100));
        Assert.That(relation.UserName, Is.EqualTo("Ben"));
        Assert.That(relation.Version, Is.EqualTo(2));
        Assert.That(relation.Members.Length, Is.EqualTo(2));
        Assert.That(relation.Members[0].Id, Is.EqualTo(10));
        Assert.That(relation.Members[0].Type, Is.EqualTo(OsmSharp.OsmGeoType.Node));
        Assert.That(relation.Members[0].Role, Is.EqualTo("fake role"));
        Assert.That(relation.Members[1].Id, Is.EqualTo(11));
        Assert.That(relation.Members[1].Type, Is.EqualTo(OsmSharp.OsmGeoType.Way));
        Assert.That(relation.Members[1].Role, Is.EqualTo("fake role"));
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
        Assert.That(pbfRelation.id, Is.EqualTo(1));
        Assert.That(pbfRelation.memids.Count, Is.EqualTo(2));
        Assert.That(pbfRelation.memids[0], Is.EqualTo(1));
        Assert.That(pbfRelation.memids[1], Is.EqualTo(1));
        Assert.That(pbfRelation.roles_sid.Count, Is.EqualTo(2));
        Assert.That(System.Text.Encoding.UTF8.GetString(block.stringtable.s[(int)pbfRelation.roles_sid[0]]), Is.EqualTo("fake role1"));
        Assert.That(System.Text.Encoding.UTF8.GetString(block.stringtable.s[(int)pbfRelation.roles_sid[1]]), Is.EqualTo("fake role2"));
        Assert.That(pbfRelation.types.Count, Is.EqualTo(2));
        Assert.That(pbfRelation.types[0], Is.EqualTo(OsmSharp.IO.PBF.Relation.MemberType.NODE));
        Assert.That(pbfRelation.types[1], Is.EqualTo(OsmSharp.IO.PBF.Relation.MemberType.RELATION));
        Assert.That(pbfRelation.info.changeset, Is.EqualTo(1));
        Assert.That(pbfRelation.info.timestamp, Is.EqualTo(Encoder.EncodeTimestamp(relation.TimeStamp.Value, block.date_granularity)));
        Assert.That(pbfRelation.info.uid, Is.EqualTo(1));
        Assert.That(System.Text.Encoding.UTF8.GetString(block.stringtable.s[(int)pbfRelation.info.user_sid]), Is.EqualTo("Ben"));
        Assert.That(pbfRelation.info.version, Is.EqualTo(1));
        Assert.That(pbfRelation.keys.Count, Is.EqualTo(1));
        Assert.That(System.Text.Encoding.UTF8.GetString(block.stringtable.s[(int)pbfRelation.keys[0]]), Is.EqualTo("name"));
        Assert.That(pbfRelation.vals.Count, Is.EqualTo(1));
        Assert.That(System.Text.Encoding.UTF8.GetString(block.stringtable.s[(int)pbfRelation.vals[0]]), Is.EqualTo("Ben"));
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

        Assert.That(primitivesConsumer.Nodes.Count, Is.EqualTo(1));
        Assert.That(primitivesConsumer.Ways.Count, Is.EqualTo(0));
        Assert.That(primitivesConsumer.Relations.Count, Is.EqualTo(0));
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
        primitiveGroup.dense.denseinfo.version.Add(3);

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

        Assert.That(primitivesConsumer.Nodes.Count, Is.EqualTo(3));
        Assert.That(primitivesConsumer.Ways.Count, Is.EqualTo(0));
        Assert.That(primitivesConsumer.Relations.Count, Is.EqualTo(0));

        var node = primitivesConsumer.Nodes[0];
        Assert.IsNotNull(node);
        Assert.That(node.id, Is.EqualTo(1));
        Assert.That(node.info.changeset, Is.EqualTo(10));
        Assert.That(node.info.timestamp, Is.EqualTo(10));
        Assert.That(node.info.uid, Is.EqualTo(1));
        Assert.That(node.info.user_sid, Is.EqualTo(3));
        Assert.That(node.info.version, Is.EqualTo(1));
        Assert.That(node.keys.Count, Is.EqualTo(1));
        Assert.That(node.keys[0], Is.EqualTo(1));
        Assert.That(node.vals.Count, Is.EqualTo(1));
        Assert.That(node.vals[0], Is.EqualTo(2));

        node = primitivesConsumer.Nodes[1];
        Assert.IsNotNull(node);
        Assert.That(node.id, Is.EqualTo(2));
        Assert.That(node.info.changeset, Is.EqualTo(11));
        Assert.That(node.info.timestamp, Is.EqualTo(11));
        Assert.That(node.info.uid, Is.EqualTo(1));
        Assert.That(node.info.user_sid, Is.EqualTo(3));
        Assert.That(node.info.version, Is.EqualTo(1));
        Assert.That(node.keys.Count, Is.EqualTo(1));
        Assert.That(node.keys[0], Is.EqualTo(1));
        Assert.That(node.vals.Count, Is.EqualTo(1));
        Assert.That(node.vals[0], Is.EqualTo(4));

        node = primitivesConsumer.Nodes[2];
        Assert.IsNotNull(node);
        Assert.That(node.id, Is.EqualTo(3));
        Assert.That(node.info.changeset, Is.EqualTo(12));
        Assert.That(node.info.timestamp, Is.EqualTo(12));
        Assert.That(node.info.uid, Is.EqualTo(1));
        Assert.That(node.info.user_sid, Is.EqualTo(3));
        Assert.That(node.info.version, Is.EqualTo(3));
        Assert.That(node.keys.Count, Is.EqualTo(0));
        Assert.That(node.vals.Count, Is.EqualTo(0));
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

        Assert.That(primitivesConsumer.Nodes.Count, Is.EqualTo(0));
        Assert.That(primitivesConsumer.Ways.Count, Is.EqualTo(1));
        Assert.That(primitivesConsumer.Relations.Count, Is.EqualTo(0));
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

        Assert.That(primitivesConsumer.Nodes.Count, Is.EqualTo(0));
        Assert.That(primitivesConsumer.Ways.Count, Is.EqualTo(0));
        Assert.That(primitivesConsumer.Relations.Count, Is.EqualTo(1));
    }
}
