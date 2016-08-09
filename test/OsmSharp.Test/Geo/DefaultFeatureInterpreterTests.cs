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

using NetTopologySuite.Geometries;
using NUnit.Framework;
using OsmSharp.Db;
using OsmSharp.Geo;
using OsmSharp.Tags;
using System.Linq;
using OsmSharp.Db.Impl;

namespace OsmSharp.Test.Geo
{

    /// <summary>
    /// Contains tests for the default feature interpreter class testing as many of the openstreetmap tags ->  geometry logic as possible.
    /// </summary>
    [TestFixture]
    public class DefaultFeatureInterpreterTests
    {
        /// <summary>
        /// Tests the interpretation of an area.
        /// Way(area=yes) => area
        /// 
        /// http://wiki.openstreetmap.org/wiki/Area#A_simple_area
        /// </summary>
        [Test]
        public void TestWayAreaIsYesArea()
        {
            var node1 = new Node();
            node1.Id = 1;
            node1.Latitude = 0;
            node1.Longitude = 0;
            var node2 = new Node();
            node2.Id = 2;
            node2.Latitude = 1;
            node2.Longitude = 0;
            var node3 = new Node();
            node3.Id = 3;
            node3.Latitude = 0;
            node3.Longitude = 1;

            var way = new Way();
            way.Id = 1;
            way.Nodes = new long[]
            {
                1, 2, 3, 1
            };
            way.Tags = new TagsCollection();
            way.Tags.Add("area", "yes");

            var source = (new MemorySnapshotDb()).CreateSnapshotDb();
            source.AddOrUpdate(node1);
            source.AddOrUpdate(node2);
            source.AddOrUpdate(node3);
            source.AddOrUpdate(way);

            // the use of natural=water implies an area-type.
            var interpreter = new DefaultFeatureInterpreter();
            var features = interpreter.Interpret(way, source);

            Assert.IsNotNull(features);
            Assert.AreEqual(1, features.Count);
            var feature = features[0];
            Assert.IsInstanceOf<LinearRing>(feature.Geometry);
            Assert.IsTrue(feature.Attributes.Contains("area", "yes"));
        }

        /// <summary>
        /// Tests the interpretation of a water area.
        /// Way(natural=water) => area
        /// 
        /// http://wiki.openstreetmap.org/wiki/Tag:natural=water
        /// </summary>
        [Test]
        public void TestWayNaturalIsWaterArea()
        {
            var node1 = new Node();
            node1.Id = 1;
            node1.Latitude = 0;
            node1.Longitude = 0;
            var node2 = new Node();
            node2.Id = 2;
            node2.Latitude = 1;
            node2.Longitude = 0;
            var node3 = new Node();
            node3.Id = 3;
            node3.Latitude = 0;
            node3.Longitude = 1;

            var way = new Way();
            way.Id = 1;
            way.Nodes = new long[]
            {
                1, 2, 3, 1
            };
            way.Tags = new TagsCollection();
            way.Tags.Add("natural", "water");

            var source = (new MemorySnapshotDb()).CreateSnapshotDb();
            source.AddOrUpdate(node1);
            source.AddOrUpdate(node2);
            source.AddOrUpdate(node3);
            source.AddOrUpdate(way);

            // the use of natural=water implies an area-type.
            var interpreter = new DefaultFeatureInterpreter();
            var features = interpreter.Interpret(way, source);

            Assert.IsNotNull(features);
            Assert.AreEqual(1, features.Count);
            var feature = features[0];
            Assert.IsInstanceOf<LinearRing>(feature.Geometry);
            Assert.IsTrue(feature.Attributes.Contains("natural", "water"));
        }

        /// <summary>
        /// Tests the interpretation of a multipolygon relation.
        /// Relation(type=multipolygon) => area
        /// 
        /// http://wiki.openstreetmap.org/wiki/Relation:multipolygon
        /// </summary>
        [Test]
        public void TestRelationMultipolygonAreaOneOuter()
        {
            // tests a multipolygon containing one 'outer' member.
            var source = new MemorySnapshotDb(
                new Node()
                {
                    Id = 1,
                    Latitude = 0,
                    Longitude = 0
                },
                new Node()
                {
                    Id = 2,
                    Latitude = 1,
                    Longitude = 0
                },
                new Node()
                {
                    Id = 3,
                    Latitude = 0,
                    Longitude = 1
                },
                new Way()
                {
                    Id = 1,
                    Nodes = new long[]
                    {
                        1, 2, 3, 1
                    }
                },
                new Relation()
                {
                    Id = 1,
                    Tags = new TagsCollection(
                        new Tag("type", "multipolygon")),
                    Members = new RelationMember[]
                    {
                        new RelationMember()
                        {
                            Id = 1,
                            Role = "outer",
                            Type = OsmGeoType.Way
                        }
                    }
                }).CreateSnapshotDb();

            var interpreter = new DefaultFeatureInterpreter();
            var features = interpreter.Interpret(source.GetRelation(1), source);
            Assert.IsNotNull(features);
            Assert.AreEqual(1, features.Count);
            var feature = features[0];
            Assert.IsInstanceOf<LinearRing>(feature.Geometry);
            Assert.IsTrue(feature.Attributes.Contains("type", "multipolygon"));
        }

        /// <summary>
        /// Tests the interpretation of a multipolygon containing one 'outer' member and one 'inner' member.
        /// Relation(type=multipolygon) => area
        /// 
        /// http://wiki.openstreetmap.org/wiki/Relation:multipolygon
        /// </summary>
        [Test]
        public void TestRelationMultipolygonAreaOneOuterOneInner()
        {
            var source = new MemorySnapshotDb(
                new Node()
                {
                    Id = 1,
                    Latitude = 0,
                    Longitude = 0
                },
                new Node()
                {
                    Id = 2,
                    Latitude = 0,
                    Longitude = 1
                },
                new Node()
                {
                    Id = 3,
                    Latitude = 1,
                    Longitude = 1
                },
                new Node()
                {
                    Id = 4,
                    Latitude = 1,
                    Longitude = 0
                },
                new Node()
                {
                    Id = 5,
                    Latitude = 0.25f,
                    Longitude = 0.25f
                },
                new Node()
                {
                    Id = 6,
                    Latitude = 0.25f,
                    Longitude = 0.40f
                },
                new Node()
                {
                    Id = 7,
                    Latitude = 0.40f,
                    Longitude = 0.40f
                },
                new Node()
                {
                    Id = 8,
                    Latitude = 0.40f,
                    Longitude = 0.25f
                },
                new Way()
                {
                    Id = 1,
                    Nodes = new long[]
                    {
                        1, 2, 3, 4, 1
                    }
                },
                new Way()
                {
                    Id = 2,
                    Nodes = new long[]
                    {
                        5, 6, 7, 8, 5
                    }
                },
                new Relation()
                {
                    Id = 1,
                    Tags = new TagsCollection(
                        new Tag("type", "multipolygon")),
                    Members = new RelationMember[]
                    {
                        new RelationMember()
                        {
                            Id = 1,
                            Role = "outer",
                            Type = OsmGeoType.Way
                        },
                        new RelationMember()
                        {
                            Id = 2,
                            Role = "inner",
                            Type = OsmGeoType.Way
                        }
                    }
                }).CreateSnapshotDb();

            var interpreter = new DefaultFeatureInterpreter();
            var features = interpreter.Interpret(source.GetRelation(1), source);
            Assert.IsNotNull(features);
            Assert.AreEqual(1, features.Count);
            var feature = features[0];
            Assert.IsInstanceOf<Polygon>(feature.Geometry);
            var polygon = feature.Geometry as Polygon;
            Assert.IsNotNull(polygon.Holes);
            Assert.AreEqual(1, polygon.Holes.Count());
            Assert.IsTrue(feature.Attributes.Contains("type", "multipolygon"));
        }

        /// <summary>
        /// Tests the interpretation of a multipolygon containing one 'outer' member and two 'inner' members.
        /// Relation(type=multipolygon) => area
        /// 
        /// http://wiki.openstreetmap.org/wiki/Relation:multipolygon
        /// </summary>
        [Test]
        public void TestRelationMultipolygonAreaOneOuterTwoInners()
        {
            var source = new MemorySnapshotDb(
                new Node()
                {
                    Id = 1,
                    Latitude = 0,
                    Longitude = 0
                },
                new Node()
                {
                    Id = 2,
                    Latitude = 0,
                    Longitude = 1
                },
                new Node()
                {
                    Id = 3,
                    Latitude = 1,
                    Longitude = 1
                },
                new Node()
                {
                    Id = 4,
                    Latitude = 1,
                    Longitude = 0
                },
                new Node()
                {
                    Id = 5,
                    Latitude = 0.25f,
                    Longitude = 0.25f
                },
                new Node()
                {
                    Id = 6,
                    Latitude = 0.25f,
                    Longitude = 0.40f
                },
                new Node()
                {
                    Id = 7,
                    Latitude = 0.40f,
                    Longitude = 0.40f
                },
                new Node()
                {
                    Id = 8,
                    Latitude = 0.40f,
                    Longitude = 0.25f
                },
                new Node()
                {
                    Id = 9,
                    Latitude = 0.60f,
                    Longitude = 0.25f
                },
                new Node()
                {
                    Id = 10,
                    Latitude = 0.60f,
                    Longitude = 0.40f
                },
                new Node()
                {
                    Id = 11,
                    Latitude = 0.75f,
                    Longitude = 0.40f
                },
                new Node()
                {
                    Id = 12,
                    Latitude = 0.75f,
                    Longitude = 0.25f
                },
                new Way()
                {
                    Id = 1,
                    Nodes = new long[]
                    {
                        1, 2, 3, 4, 1
                    }
                },
                new Way()
                {
                    Id = 2,
                    Nodes = new long[]
                    {
                        5, 6, 7, 8, 5
                    }
                },
                new Way()
                {
                    Id = 3,
                    Nodes = new long[]
                    {
                        9, 10, 11, 12, 9
                    }
                },
                new Relation()
                {
                    Id = 1,
                    Tags = new TagsCollection(
                        new Tag("type", "multipolygon")),
                    Members = new RelationMember[]
                    {
                        new RelationMember()
                        {
                            Id = 1,
                            Role = "outer",
                            Type = OsmGeoType.Way
                        },
                        new RelationMember()
                        {
                            Id = 2,
                            Role = "inner",
                            Type = OsmGeoType.Way
                        },
                        new RelationMember()
                        {
                            Id = 3,
                            Role = "inner",
                            Type = OsmGeoType.Way
                        }
                    }
                }).CreateSnapshotDb();

            var interpreter = new DefaultFeatureInterpreter();
            var features = interpreter.Interpret(source.GetRelation(1), source);
            Assert.IsNotNull(features);
            Assert.AreEqual(1, features.Count);
            var feature = features[0];
            Assert.IsInstanceOf<Polygon>(feature.Geometry);
            Polygon polygon = feature.Geometry as Polygon;
            Assert.IsNotNull(polygon.Holes);
            Assert.AreEqual(2, polygon.Holes.Count());
            Assert.IsTrue(feature.Attributes.Contains("type", "multipolygon"));
        }

        /// <summary>
        /// Tests the interpretation of a multipolygon containing one 'outer' member and two partial 'inner' member.
        /// Relation(type=multipolygon) => area
        /// 
        /// http://wiki.openstreetmap.org/wiki/Relation:multipolygon
        /// </summary>
        [Test]
        public void TestRelationMultipolygonAreaOneOuterTwoPartialInners()
        {
            var source = new MemorySnapshotDb(
                new Node()
                {
                    Id = 1,
                    Latitude = 0,
                    Longitude = 0
                },
                new Node()
                {
                    Id = 2,
                    Latitude = 0,
                    Longitude = 1
                },
                new Node()
                {
                    Id = 3,
                    Latitude = 1,
                    Longitude = 1
                },
                new Node()
                {
                    Id = 4,
                    Latitude = 1,
                    Longitude = 0
                },
                new Node()
                {
                    Id = 5,
                    Latitude = 0.25f,
                    Longitude = 0.25f
                },
                new Node()
                {
                    Id = 6,
                    Latitude = 0.25f,
                    Longitude = 0.40f
                },
                new Node()
                {
                    Id = 7,
                    Latitude = 0.40f,
                    Longitude = 0.40f
                },
                new Node()
                {
                    Id = 8,
                    Latitude = 0.40f,
                    Longitude = 0.25f
                },
                new Way()
                {
                    Id = 1,
                    Nodes = new long[]
                    {
                        1, 2, 3, 4, 1
                    }
                },
                new Way()
                {
                    Id = 2,
                    Nodes = new long[]
                    {
                         5, 6, 7
                    }
                },
                new Way()
                {
                    Id = 3,
                    Nodes = new long[]
                    {
                        7, 8, 5
                    }
                },
                new Relation()
                {
                    Id = 1,
                    Tags = new TagsCollection(
                        new Tag("type", "multipolygon")),
                    Members = new RelationMember[]
                    {
                        new RelationMember()
                        {
                            Id = 1,
                            Role = "outer",
                            Type = OsmGeoType.Way
                        },
                        new RelationMember()
                        {
                            Id = 2,
                            Role = "inner",
                            Type = OsmGeoType.Way
                        },
                        new RelationMember()
                        {
                            Id = 3,
                            Role = "inner",
                            Type = OsmGeoType.Way
                        }
                    }
                }).CreateSnapshotDb();

            var interpreter = new DefaultFeatureInterpreter();
            var features = interpreter.Interpret(source.GetRelation(1), source);
            Assert.IsNotNull(features);
            Assert.AreEqual(1, features.Count);
            var feature = features[0];
            Assert.IsInstanceOf<Polygon>(feature.Geometry);
            Polygon polygon = feature.Geometry as Polygon;
            Assert.IsNotNull(polygon.Holes);
            Assert.AreEqual(1, polygon.Holes.Count());
            Assert.IsTrue(feature.Attributes.Contains("type", "multipolygon"));
        }
    }
}