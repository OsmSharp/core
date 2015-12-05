// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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
using OsmSharp.Geo.Geometries;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Streams;
using OsmSharp.Osm;
using OsmSharp.Osm.Streams.Filters;
using System.Collections.Generic;

namespace OsmSharp.Test.Osm.Streams.Filters
{
    /// <summary>
    /// Contains tests for the poly filter.
    /// </summary>
    [TestFixture]
    public class OsmStreamFilterPolyTests
    {
        /// <summary>
        /// Tests filtering a stream that has a node after a way.
        /// </summary>
        [Test]
        public void TestNodeAfterWay()
        {
            var ring = new LineairRing(new GeoCoordinate(0, -1), new GeoCoordinate(1, 1),
                new GeoCoordinate(-1, 1), new GeoCoordinate(0, -1));

            var filter = new OsmStreamFilterPoly(ring);
            filter.RegisterSource(new OsmGeo[] {
                Way.Create(1, 1, 2),
                Node.Create(2, 10, 10) 
            });
            Assert.Catch<OsmStreamNotSortedException>(() =>
            {
                var list = new List<OsmGeo>(
                   filter);
            });
        }

        /// <summary>
        /// Tests filtering a stream that has a way after a relation.
        /// </summary>
        [Test]
        public void TestWayAfterRelation()
        {
            var ring = new LineairRing(new GeoCoordinate(0, -1), new GeoCoordinate(1, 1),
                new GeoCoordinate(-1, 1), new GeoCoordinate(0, -1));

            var filter = new OsmStreamFilterPoly(ring);
            filter.RegisterSource(new OsmGeo[] {
                Relation.Create(1, new RelationMember()
                {
                    MemberId = 1,
                    MemberType = OsmGeoType.Node,
                    MemberRole = string.Empty
                }),
                Way.Create(1, 1, 2)
            });
            Assert.Catch<OsmStreamNotSortedException>(() =>
            {
                var list = new List<OsmGeo>(
                   filter);
            });
        }

        /// <summary>
        /// Tests filtering one node.
        /// </summary>
        [Test]
        public void TestOneNode()
        {
            var ring = new LineairRing(new GeoCoordinate(0, -1), new GeoCoordinate(1, 1), 
                new GeoCoordinate(-1, 1), new GeoCoordinate(0, -1));

            var filter = new OsmStreamFilterPoly(ring);
            filter.RegisterSource(new OsmGeo[] {
                Node.Create(1, 0, 0),
                Node.Create(2, 10, 10) 
            });
            var list = new List<OsmGeo>(
               filter);

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(1, list[0].Id);
        }

        /// <summary>
        /// Tests filtering one node and a way.
        /// </summary>
        [Test]
        public void TestOneNodeAndOneWay()
        {
            var ring = new LineairRing(new GeoCoordinate(0, -1), new GeoCoordinate(1, 1),
                new GeoCoordinate(-1, 1), new GeoCoordinate(0, -1));

            var filter = new OsmStreamFilterPoly(ring);
            filter.RegisterSource(new OsmGeo[] {
                Node.Create(1, 0, 0),
                Node.Create(2, 10, 10),
                Way.Create(1, 1, 2)
            });
            var list = new List<OsmGeo>(
               filter);

            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(1, list[0].Id);
            Assert.AreEqual(OsmGeoType.Node, list[0].Type);
            Assert.AreEqual(1, list[1].Id);
            Assert.AreEqual(OsmGeoType.Way, list[1].Type);
        }

        /// <summary>
        /// Tests filtering one node and one relation.
        /// </summary>
        [Test]
        public void TestOneNodeAndOneRelation()
        {
            var ring = new LineairRing(new GeoCoordinate(0, -1), new GeoCoordinate(1, 1),
                new GeoCoordinate(-1, 1), new GeoCoordinate(0, -1));

            var filter = new OsmStreamFilterPoly(ring);
            filter.RegisterSource(new OsmGeo[] {
                Node.Create(1, 0, 0),
                Relation.Create(1, new RelationMember()
                {
                    MemberId = 1,
                    MemberType = OsmGeoType.Node,
                    MemberRole = string.Empty
                })
            });
            var list = new List<OsmGeo>(
               filter);

            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(1, list[0].Id);
            Assert.AreEqual(OsmGeoType.Node, list[0].Type);
            Assert.AreEqual(1, list[1].Id);
            Assert.AreEqual(OsmGeoType.Relation, list[1].Type);
        }

        /// <summary>
        /// Tests filtering one node, one way and one relation.
        /// </summary>
        [Test]
        public void TestOneNodeOneWayAndOneRelation()
        {
            var ring = new LineairRing(new GeoCoordinate(0, -1), new GeoCoordinate(1, 1),
                new GeoCoordinate(-1, 1), new GeoCoordinate(0, -1));

            var filter = new OsmStreamFilterPoly(ring);
            filter.RegisterSource(new OsmGeo[] {
                Node.Create(1, 0, 0),
                Way.Create(1, 1, 2),
                Relation.Create(1, new RelationMember()
                {
                    MemberId = 1,
                    MemberType = OsmGeoType.Node,
                    MemberRole = string.Empty
                })
            });
            var list = new List<OsmGeo>(
               filter);

            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(1, list[0].Id);
            Assert.AreEqual(OsmGeoType.Node, list[0].Type);
            Assert.AreEqual(1, list[1].Id);
            Assert.AreEqual(OsmGeoType.Way, list[1].Type);
            Assert.AreEqual(1, list[2].Id);
            Assert.AreEqual(OsmGeoType.Relation, list[2].Type);
        }

        /// <summary>
        /// Tests gettig meta-data.
        /// </summary>
        [Test]
        public void TestGetMetaData()
        {
            var ring = new LineairRing(new GeoCoordinate(0, -1), new GeoCoordinate(1, 1),
                new GeoCoordinate(-1, 1), new GeoCoordinate(0, -1));

            var source = (new OsmGeo[0]).ToOsmStreamSource();
            source.Meta.Add("source", "enumeration");
            var filter = new OsmStreamFilterPoly(ring);
            filter.RegisterSource(source);

            var meta = filter.GetAllMeta();

            Assert.IsTrue(meta.ContainsKeyValue("source", "enumeration"));
            Assert.IsTrue(meta.ContainsKeyValue("poly", OsmSharp.Geo.Streams.GeoJson.GeoJsonConverter.ToGeoJson(ring)));
        }
    }
}