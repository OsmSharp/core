// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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
using OsmSharp.Math.Geo;
using OsmSharp.Osm;
using OsmSharp.Osm.Data.Memory;
using System;
using System.Collections.Generic;

namespace OsmSharp.Test.Osm.Data
{
    /// <summary>
    /// Does some raw data memory tests.
    /// </summary>
    [TestFixture]
    public class MemoryDataSourceTests
    {
        /// <summary>
        /// Tests adding a null node.
        /// </summary>
        [Test]
        public void TestAddNodeNull()
        {
            var dataSource = new MemoryDataSource();
            Assert.Catch<ArgumentNullException>(() => dataSource.AddNode(null));
        }

        /// <summary>
        /// Tests adding a null way.
        /// </summary>
        [Test]
        public void TestAddWayNull()
        {
            var dataSource = new MemoryDataSource();
            Assert.Catch<ArgumentNullException>(() => dataSource.AddWay(null));
        }

        /// <summary>
        /// Tests adding a null relation.
        /// </summary>
        [Test]
        public void TestAddRelationNull()
        {
            var dataSource = new MemoryDataSource();
            Assert.Catch<ArgumentNullException>(() => dataSource.AddRelation(null));
        }

        /// <summary>
        /// Tests adding a node without an id.
        /// </summary>
        [Test]
        public void TestAddNodeNoId()
        {
            var dataSource = new MemoryDataSource();
            Assert.Catch<ArgumentException>(() => dataSource.AddNode(new Node()));
        }

        /// <summary>
        /// Tests adding a node without valid lat/lon.
        /// </summary>
        [Test]
        public void TestAddNodeNoLocation()
        {
            var node = new Node();
            node.Id = 1;

            var dataSource = new MemoryDataSource();
            Assert.Catch<ArgumentException>(() => dataSource.AddNode(node));
        }

        /// <summary>
        /// Tests adding a way without an id.
        /// </summary>
        [Test]
        public void TestAddWayNoId()
        {
            var dataSource = new MemoryDataSource();
            Assert.Catch<ArgumentException>(() => dataSource.AddWay(new Way()));
        }

        /// <summary>
        /// Tests adding a relation without an id.
        /// </summary>
        [Test]
        public void TestAddRelationNoId()
        {
            var dataSource = new MemoryDataSource();
            Assert.Catch<ArgumentException>(() => dataSource.AddRelation(new Relation()));
        }
       
        /// <summary>
        /// Tests adding nodes and the resulting boundingbox.
        /// </summary>
        [Test]
        public void TestBoundingBox()
        {
            var dataSource = new MemoryDataSource();

            Node node = new Node();
            node.Id = 1;
            node.Longitude = -2;
            node.Latitude = -1;
            dataSource.AddNode(node);

            node = new Node();
            node.Id = 2;
            node.Longitude = 2;
            node.Latitude = 1;
            dataSource.AddNode(node);

            Assert.IsTrue(dataSource.HasBoundinBox);
            GeoCoordinateBox box = dataSource.BoundingBox;
            Assert.AreEqual(1, box.MaxLat);
            Assert.AreEqual(2, box.MaxLon);
            Assert.AreEqual(-1, box.MinLat);
            Assert.AreEqual(-2, box.MinLon);
        }

        /// <summary>
        /// Tests adding a node to the memory source.
        /// </summary>
        [Test]
        public void TestAddNode()
        {
            Node testNode =new Node();
            testNode.Id = -1;
            testNode.Latitude = 0;
            testNode.Longitude = 0;

            var source = new MemoryDataSource();
            source.AddNode(testNode);

            // test if the node is actually there.
            Assert.AreEqual(testNode, source.GetNode(-1));

            // test if the node was not remove after getting it.
            Assert.AreEqual(testNode, source.GetNodes(new List<long>() { -1 })[0]);

            // test if the node is in the list of nodes.
            Assert.AreEqual(testNode, new List<Node>(source.GetNodes())[0]);

            // test if the node will be retrieved using a list of ids.
            var ids = new List<long>();
            ids.Add(-1);
            IList<Node> nodes = source.GetNodes(ids);
            Assert.IsNotNull(nodes);
            Assert.AreEqual(1, nodes.Count);
            Assert.AreEqual(testNode, nodes[0]);
        }

        /// <summary>
        /// Tests removing a node.
        /// </summary>
        [Test]
        public void TestRemoveNode()
        {
            Node testNode = new Node();
            testNode.Id = -1;
            testNode.Latitude = 0;
            testNode.Longitude = 0;

            var source = new MemoryDataSource();
            source.AddNode(testNode);

            // test if the node is actually there.
            Assert.AreEqual(testNode, source.GetNode(-1));

            // remove the node.
            source.RemoveNode(-1);

            // test if the node is actually gone.
            Assert.IsNull(source.GetNode(-1));
        }

        /// <summary>
        /// Tests adding a way to the memory source.
        /// </summary>
        [Test]
        public void TestAddWay()
        {
            Way testWay = new Way();
            testWay.Id = -1;
            var source = new MemoryDataSource();
            source.AddWay(testWay);

            // test if the way is actually there.
            Assert.AreEqual(testWay, source.GetWay(-1));

            // test if the way was not remove after getting it.
            Assert.AreEqual(testWay, source.GetWays(new List<long>() { -1 })[0]);

            // test if the way is in the list of ways.
            Assert.AreEqual(testWay, new List<Way>(source.GetWays())[0]);

            // test if the way will be retrieved using a list of ids.
            var ids = new List<long>();
            ids.Add(-1);
            IList<Way> ways = source.GetWays(ids);
            Assert.IsNotNull(ways);
            Assert.AreEqual(1, ways.Count);
            Assert.AreEqual(testWay, ways[0]);
        }

        /// <summary>
        /// Tests removing a way.
        /// </summary>
        [Test]
        public void TestRemoveWay()
        {
            Way testWay = new Way();
            testWay.Id = -1;
            var source = new MemoryDataSource();
            source.AddWay(testWay);

            // test if the way is actually there.
            Assert.AreEqual(testWay, source.GetWay(-1));

            // remove the way.
            source.RemoveWay(-1);

            // test if the way is actually gone.
            Assert.IsNull(source.GetWay(-1));
        }

        /// <summary>
        /// Tests adding a relation to the memory source.
        /// </summary>
        [Test]
        public void TestAddRelation()
        {
            Relation testRelation = new Relation();
            testRelation.Id = -1;
            var source = new MemoryDataSource();
            source.AddRelation(testRelation);

            // test if the relation is actually there.
            Assert.AreEqual(testRelation, source.GetRelation(-1));

            // test if the relation was not remove after getting it.
            Assert.AreEqual(testRelation, source.GetRelations(new List<long>() { -1 })[0]);

            // test if the relation is in the list of relations.
            Assert.AreEqual(testRelation, new List<Relation>(source.GetRelations())[0]);

            // test if the relation will be retrieved using a list of ids.
            List<long> ids = new List<long>();
            ids.Add(-1);
            IList<Relation> relations = source.GetRelations(ids);
            Assert.IsNotNull(relations);
            Assert.AreEqual(1, relations.Count);
            Assert.AreEqual(testRelation, relations[0]);
        }

        /// <summary>
        /// Tests removing a relation.
        /// </summary>
        [Test]
        public void TestRemoveRelation()
        {
            Relation testRelation = new Relation();
            testRelation.Id = -1;
            var source = new MemoryDataSource();
            source.AddRelation(testRelation);

            // test if the relation is actually there.
            Assert.AreEqual(testRelation, source.GetRelation(-1));

            // remove the relation.
            source.RemoveRelation(-1);

            // test if the relation is actually gone.
            Assert.IsNull(source.GetRelation(-1));
        }

        /// <summary>
        /// Tests adding a way and retrieving it by it's nodes.
        /// </summary>
        [Test]
        public void TestAddWayAndNodes()
        {
            Way testWay = new Way();
            testWay.Id = 1;
            testWay.Nodes = new List<long>();
            testWay.Nodes.Add(1);
            testWay.Nodes.Add(2);

            Node node1 = new Node();
            node1.Id = 1;
            node1.Longitude = 0;
            node1.Latitude = 0;
            Node node2 = new Node();
            node2.Id = 2;
            node2.Longitude = 0;
            node2.Latitude = 0;

            var source = new MemoryDataSource();
            source.AddWay(testWay);

            IList<Way> resultWays = source.GetWaysFor(node1);
            Assert.IsNotNull(resultWays);
            Assert.AreEqual(1, resultWays.Count);
            Assert.AreEqual(testWay, resultWays[0]);

            // test if the way is actually there.
            Assert.AreEqual(testWay, source.GetWay(1));

            // test if the way was not removed after getting it.
            Assert.AreEqual(testWay, source.GetWay(1));
        }

        /// <summary>
        /// Tests adding a relation and retrieving it by it's members.
        /// </summary>
        [Test]
        public void TestAddRelationAndMembers()
        {
            Way testWay = new Way();
            testWay.Id = 1;
            testWay.Nodes = new List<long>();
            testWay.Nodes.Add(1);
            testWay.Nodes.Add(2);

            Node node1 = new Node();
            node1.Id = 1;
            node1.Longitude = 0;
            node1.Latitude = 0;
            Node node2 = new Node();
            node2.Id = 2;
            node2.Longitude = 0;
            node2.Latitude = 0;

            Relation relationAsMember = new Relation();
            relationAsMember.Id = 2;

            Relation relation = new Relation();
            relation.Id = 1;
            relation.Members = new List<RelationMember>();
            relation.Members.Add(new RelationMember()
            {
                MemberId = 1,
                MemberRole = "node",
                MemberType = OsmGeoType.Node
            });
            relation.Members.Add(new RelationMember()
            {
                MemberId = 2,
                MemberRole = "node",
                MemberType = OsmGeoType.Node
            });
            relation.Members.Add(new RelationMember()
            {
                MemberId = 1,
                MemberRole = "way",
                MemberType = OsmGeoType.Way
            });
            relation.Members.Add(new RelationMember()
            {
                MemberId = 2,
                MemberRole = "relation",
                MemberType = OsmGeoType.Relation
            });

            var source = new MemoryDataSource();
            source.AddRelation(relation);

            // test positive cases.
            IList<Relation> resultRelations = source.GetRelationsFor(node1);
            Assert.IsNotNull(resultRelations);
            Assert.AreEqual(1, resultRelations.Count);
            Assert.AreEqual(relation, resultRelations[0]);
            resultRelations = source.GetRelationsFor(node2);
            Assert.IsNotNull(resultRelations);
            Assert.AreEqual(1, resultRelations.Count);
            Assert.AreEqual(relation, resultRelations[0]);
            resultRelations = source.GetRelationsFor(testWay);
            Assert.IsNotNull(resultRelations);
            Assert.AreEqual(1, resultRelations.Count);
            Assert.AreEqual(relation, resultRelations[0]);
            resultRelations = source.GetRelationsFor(relationAsMember);
            Assert.IsNotNull(resultRelations);
            Assert.AreEqual(1, resultRelations.Count);
            Assert.AreEqual(relation, resultRelations[0]);

            // test negative cases.
            resultRelations = source.GetRelationsFor(OsmGeoType.Node, 10000);
            Assert.IsNotNull(resultRelations);
            Assert.AreEqual(0, resultRelations.Count);
            resultRelations = source.GetRelationsFor(OsmGeoType.Way, 10000);
            Assert.IsNotNull(resultRelations);
            Assert.AreEqual(0, resultRelations.Count);
            resultRelations = source.GetRelationsFor(OsmGeoType.Relation, 10000);
            Assert.IsNotNull(resultRelations);
            Assert.AreEqual(0, resultRelations.Count);
        }

        /// <summary>
        /// Tests queries by bounding box.
        /// </summary>
        [Test]
        public void TestGetBoundingBox()
        {
            var dataSource = new MemoryDataSource();

            // test nodes.
            Node node = new Node();
            node.Id = 1;
            node.Longitude = -2;
            node.Latitude = -1;
            dataSource.AddNode(node);

            node = new Node();
            node.Id = 2;
            node.Longitude = 2;
            node.Latitude = 1;
            dataSource.AddNode(node);

            GeoCoordinateBox box = dataSource.BoundingBox;

            IList<OsmGeo> boxResults = dataSource.Get(box, null);
            Assert.IsNotNull(boxResults);
            Assert.AreEqual(1, boxResults.Count);

            boxResults = dataSource.Get(box.Resize(0.1), null);
            Assert.IsNotNull(boxResults);
            Assert.AreEqual(2, boxResults.Count);

            node = new Node();
            node.Id = 3;
            node.Latitude = 10;
            node.Longitude = 10;
            dataSource.AddNode(node);

            node = new Node();
            node.Id = 4;
            node.Latitude = -10;
            node.Longitude = -10;
            dataSource.AddNode(node);

            boxResults = dataSource.Get(box, null);
            Assert.IsNotNull(boxResults);
            Assert.AreEqual(1, boxResults.Count);

            boxResults = dataSource.Get(box.Resize(0.1), null);
            Assert.IsNotNull(boxResults);
            Assert.AreEqual(2, boxResults.Count);

            // test ways.
            Way positive = new Way();
            positive.Id = 1;
            positive.Nodes = new List<long>();
            positive.Nodes.Add(1);
            positive.Nodes.Add(2);
            dataSource.AddWay(positive);

            Way halfPositive = new Way();
            halfPositive.Id = 2;
            halfPositive.Nodes = new List<long>();
            halfPositive.Nodes.Add(1);
            halfPositive.Nodes.Add(3);
            dataSource.AddWay(halfPositive);

            Way negative = new Way();
            negative.Id = 3;
            negative.Nodes = new List<long>();
            negative.Nodes.Add(3);
            negative.Nodes.Add(4);
            dataSource.AddWay(negative);
            
            HashSet<OsmGeo> boxResultWithWays = new HashSet<OsmGeo>(dataSource.Get(box, null));
            Assert.IsTrue(boxResultWithWays.Contains(positive));
            Assert.IsTrue(boxResultWithWays.Contains(halfPositive));
            Assert.IsFalse(boxResultWithWays.Contains(negative));

            // test relations.
            Relation positiveRelation1 = new Relation();
            positiveRelation1.Id = 1;
            positiveRelation1.Members = new List<RelationMember>();
            positiveRelation1.Members.Add(new RelationMember()
            {
                MemberId = 1,
                MemberType = OsmGeoType.Node,
                MemberRole = "node"
            });
            dataSource.AddRelation(positiveRelation1);

            Relation positiveRelation2 = new Relation();
            positiveRelation2.Id = 2;
            positiveRelation2.Members = new List<RelationMember>();
            positiveRelation2.Members.Add(new RelationMember()
            {
                MemberId = 1,
                MemberType = OsmGeoType.Way,
                MemberRole = "way"
            });
            dataSource.AddRelation(positiveRelation2);

            Relation negativeRelation3 = new Relation();
            negativeRelation3.Id = 3;
            negativeRelation3.Members = new List<RelationMember>();
            negativeRelation3.Members.Add(new RelationMember()
            {
                MemberId = 3,
                MemberType = OsmGeoType.Way,
                MemberRole = "way"
            });
            dataSource.AddRelation(positiveRelation2);

            HashSet<OsmGeo> boxResultWithWaysAndRelations = new HashSet<OsmGeo>(dataSource.Get(box, null));
            Assert.IsTrue(boxResultWithWaysAndRelations.Contains(positiveRelation1));
            Assert.IsTrue(boxResultWithWaysAndRelations.Contains(positiveRelation2));
            Assert.IsFalse(boxResultWithWaysAndRelations.Contains(negativeRelation3));

            // test recursive relations.
            Relation recusive1 = new Relation();
            recusive1.Id = 10;
            recusive1.Members = new List<RelationMember>();
            recusive1.Members.Add(new RelationMember()
            {
                MemberId = 1,
                MemberType = OsmGeoType.Relation,
                MemberRole = "relation"
            });
            dataSource.AddRelation(recusive1);
            Relation recusive2 = new Relation();
            recusive2.Id = 11;
            recusive2.Members = new List<RelationMember>();
            recusive2.Members.Add(new RelationMember()
            {
                MemberId = 10,
                MemberType = OsmGeoType.Relation,
                MemberRole = "relation"
            });
            dataSource.AddRelation(recusive2);
            Relation recusive3 = new Relation();
            recusive3.Id = 12;
            recusive3.Members = new List<RelationMember>();
            recusive3.Members.Add(new RelationMember()
            {
                MemberId = 11,
                MemberType = OsmGeoType.Relation,
                MemberRole = "relation"
            });
            dataSource.AddRelation(recusive3);

            boxResultWithWaysAndRelations = new HashSet<OsmGeo>(dataSource.Get(box, null));
            Assert.IsTrue(boxResultWithWaysAndRelations.Contains(recusive1));
            Assert.IsTrue(boxResultWithWaysAndRelations.Contains(recusive2));
            Assert.IsTrue(boxResultWithWaysAndRelations.Contains(recusive3));
        }
    }
}