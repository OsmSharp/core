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
using OsmSharp.Geo.Attributes;
using OsmSharp.Geo.Features;
using OsmSharp.Geo.Geometries;
using OsmSharp.Geo.Streams.GeoJson;
using OsmSharp.Math.Geo;
using System.Collections.Generic;

namespace OsmSharp.Test.Geo.Streams.GeoJson
{
    /// <summary>
    /// Tests the GeoJson converter.
    /// </summary>
    [TestFixture]
    public class GeoJsonConverterTests
    {
        /// <summary>
        /// Tests serializing a lineair ring.
        /// </summary>
        [Test]
        public void TestLineairRingSerialization()
        {
            var geometry = new LineairRing(
                new GeoCoordinate[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(0, 1),
                    new GeoCoordinate(1, 1),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(0, 0)
                });

            var serialized = geometry.ToGeoJson();
            serialized = serialized.RemoveWhitespace();

            Assert.AreEqual("{\"type\":\"Polygon\",\"coordinates\":[[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0],[0.0,0.0]]]}", 
                serialized);
        }

        /// <summary>
        /// Tests serializing a polygon.
        /// </summary>
        [Test]
        public void TestPolygonSerialization()
        { 
            // polygon, no holes.
            var geometry = new Polygon(new LineairRing(
                new GeoCoordinate[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(0, 1),
                    new GeoCoordinate(1, 1),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(0, 0)
                }));

            var serialized = geometry.ToGeoJson();
            serialized = serialized.RemoveWhitespace();

            Assert.AreEqual("{\"type\":\"Polygon\",\"coordinates\":[[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0],[0.0,0.0]]]}",
                serialized);

            // polygons, one hole.
            geometry = new Polygon(new LineairRing(
                new GeoCoordinate[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(0, 1),
                    new GeoCoordinate(1, 1),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(0, 0)
                }),
                new LineairRing[]
                {
                    new LineairRing(
                        new GeoCoordinate[]
                        {
                            new GeoCoordinate(0.25, 0.25),
                            new GeoCoordinate(0.25, 0.75),
                            new GeoCoordinate(0.75, 0.75),
                            new GeoCoordinate(0.75, 0.25),
                            new GeoCoordinate(0.25, 0.25)
                        })
                });

            serialized = geometry.ToGeoJson();
            serialized = serialized.RemoveWhitespace();

            Assert.AreEqual("{\"type\":\"Polygon\",\"coordinates\":[[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0],[0.0,0.0]],[[0.25,0.25],[0.75,0.25],[0.75,0.75],[0.25,0.75],[0.25,0.25]]]}",
                serialized);
        }

        /// <summary>
        /// Tests deserializing a polygon.
        /// </summary>
        [Test]
        public void TestPolygonDeserialization()
        {
            var geometry = "{\"type\":\"Polygon\",\"coordinates\":[[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0],[0.0,0.0]]]}".ToGeometry();

            Assert.IsNotNull(geometry);
            Assert.IsInstanceOf<Polygon>(geometry);
            var polygon = geometry as Polygon;
            Assert.AreEqual(5, polygon.Ring.Coordinates.Count);
            Assert.AreEqual(0, polygon.Ring.Coordinates[0].Latitude);
            Assert.AreEqual(0, polygon.Ring.Coordinates[0].Longitude);
            Assert.AreEqual(0, polygon.Ring.Coordinates[1].Latitude);
            Assert.AreEqual(1, polygon.Ring.Coordinates[1].Longitude);
            Assert.AreEqual(1, polygon.Ring.Coordinates[2].Latitude);
            Assert.AreEqual(1, polygon.Ring.Coordinates[2].Longitude);
            Assert.AreEqual(1, polygon.Ring.Coordinates[3].Latitude);
            Assert.AreEqual(0, polygon.Ring.Coordinates[3].Longitude);
            Assert.AreEqual(0, polygon.Ring.Coordinates[4].Latitude);
            Assert.AreEqual(0, polygon.Ring.Coordinates[4].Longitude);

            geometry = "{\"coordinates\":[[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0],[0.0,0.0]]],\"type\":\"Polygon\"}".ToGeometry();

            Assert.IsNotNull(geometry);
            Assert.IsInstanceOf<Polygon>(geometry);
            polygon = geometry as Polygon;
            Assert.AreEqual(5, polygon.Ring.Coordinates.Count);
            Assert.AreEqual(0, polygon.Ring.Coordinates[0].Latitude);
            Assert.AreEqual(0, polygon.Ring.Coordinates[0].Longitude);
            Assert.AreEqual(0, polygon.Ring.Coordinates[1].Latitude);
            Assert.AreEqual(1, polygon.Ring.Coordinates[1].Longitude);
            Assert.AreEqual(1, polygon.Ring.Coordinates[2].Latitude);
            Assert.AreEqual(1, polygon.Ring.Coordinates[2].Longitude);
            Assert.AreEqual(1, polygon.Ring.Coordinates[3].Latitude);
            Assert.AreEqual(0, polygon.Ring.Coordinates[3].Longitude);
            Assert.AreEqual(0, polygon.Ring.Coordinates[4].Latitude);
            Assert.AreEqual(0, polygon.Ring.Coordinates[4].Longitude);

            geometry = "{\"type\":\"Polygon\",\"coordinates\":[[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0],[0.0,0.0]],[[0.25,0.25],[0.75,0.25],[0.75,0.75],[0.25,0.75],[0.25,0.25]]]}".ToGeometry();

            Assert.IsNotNull(geometry);
            Assert.IsInstanceOf<Polygon>(geometry);
            polygon = geometry as Polygon;
            Assert.AreEqual(5, polygon.Ring.Coordinates.Count);
            Assert.AreEqual(0, polygon.Ring.Coordinates[0].Latitude);
            Assert.AreEqual(0, polygon.Ring.Coordinates[0].Longitude);
            Assert.AreEqual(0, polygon.Ring.Coordinates[1].Latitude);
            Assert.AreEqual(1, polygon.Ring.Coordinates[1].Longitude);
            Assert.AreEqual(1, polygon.Ring.Coordinates[2].Latitude);
            Assert.AreEqual(1, polygon.Ring.Coordinates[2].Longitude);
            Assert.AreEqual(1, polygon.Ring.Coordinates[3].Latitude);
            Assert.AreEqual(0, polygon.Ring.Coordinates[3].Longitude);
            Assert.AreEqual(0, polygon.Ring.Coordinates[4].Latitude);
            Assert.AreEqual(0, polygon.Ring.Coordinates[4].Longitude);
            Assert.IsNotNull(polygon.Holes);
            var holes = new List<LineairRing>(polygon.Holes);
            Assert.AreEqual(1, holes.Count);
            Assert.AreEqual(5, holes[0].Coordinates.Count);
            Assert.AreEqual(0.25, holes[0].Coordinates[0].Latitude);
            Assert.AreEqual(0.25, holes[0].Coordinates[0].Longitude);
            Assert.AreEqual(0.25, holes[0].Coordinates[1].Latitude);
            Assert.AreEqual(0.75, holes[0].Coordinates[1].Longitude);
            Assert.AreEqual(0.75, holes[0].Coordinates[2].Latitude);
            Assert.AreEqual(0.75, holes[0].Coordinates[2].Longitude);
            Assert.AreEqual(0.75, holes[0].Coordinates[3].Latitude);
            Assert.AreEqual(0.25, holes[0].Coordinates[3].Longitude);
            Assert.AreEqual(0.25, holes[0].Coordinates[4].Latitude);
            Assert.AreEqual(0.25, holes[0].Coordinates[4].Longitude);

            geometry = "{\"coordinates\":[[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0],[0.0,0.0]],[[0.25,0.25],[0.75,0.25],[0.75,0.75],[0.25,0.75],[0.25,0.25]]],\"type\":\"Polygon\"}".ToGeometry();

            Assert.IsNotNull(geometry);
            Assert.IsInstanceOf<Polygon>(geometry);
            polygon = geometry as Polygon;
            Assert.AreEqual(5, polygon.Ring.Coordinates.Count);
            Assert.AreEqual(0, polygon.Ring.Coordinates[0].Latitude);
            Assert.AreEqual(0, polygon.Ring.Coordinates[0].Longitude);
            Assert.AreEqual(0, polygon.Ring.Coordinates[1].Latitude);
            Assert.AreEqual(1, polygon.Ring.Coordinates[1].Longitude);
            Assert.AreEqual(1, polygon.Ring.Coordinates[2].Latitude);
            Assert.AreEqual(1, polygon.Ring.Coordinates[2].Longitude);
            Assert.AreEqual(1, polygon.Ring.Coordinates[3].Latitude);
            Assert.AreEqual(0, polygon.Ring.Coordinates[3].Longitude);
            Assert.AreEqual(0, polygon.Ring.Coordinates[4].Latitude);
            Assert.AreEqual(0, polygon.Ring.Coordinates[4].Longitude);
            Assert.IsNotNull(polygon.Holes);
            holes = new List<LineairRing>(polygon.Holes);
            Assert.AreEqual(1, holes.Count);
            Assert.AreEqual(5, holes[0].Coordinates.Count);
            Assert.AreEqual(0.25, holes[0].Coordinates[0].Latitude);
            Assert.AreEqual(0.25, holes[0].Coordinates[0].Longitude);
            Assert.AreEqual(0.25, holes[0].Coordinates[1].Latitude);
            Assert.AreEqual(0.75, holes[0].Coordinates[1].Longitude);
            Assert.AreEqual(0.75, holes[0].Coordinates[2].Latitude);
            Assert.AreEqual(0.75, holes[0].Coordinates[2].Longitude);
            Assert.AreEqual(0.75, holes[0].Coordinates[3].Latitude);
            Assert.AreEqual(0.25, holes[0].Coordinates[3].Longitude);
            Assert.AreEqual(0.25, holes[0].Coordinates[4].Latitude);
            Assert.AreEqual(0.25, holes[0].Coordinates[4].Longitude);
        }

        /// <summary>
        /// Tests serializing a multipolygon.
        /// </summary>
        [Test]
        public void TestMultiPolygonSerialization()
        {
            var geometry1 = new Polygon(new LineairRing(
                new GeoCoordinate[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(0, 1),
                    new GeoCoordinate(1, 1),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(0, 0)
                }));
            var geometry2 = new Polygon(new LineairRing(
                new GeoCoordinate[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(0, 2),
                    new GeoCoordinate(2, 2),
                    new GeoCoordinate(2, 0),
                    new GeoCoordinate(0, 0)
                }));
            var geometry3 = new Polygon(new LineairRing(
                new GeoCoordinate[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(0, 3),
                    new GeoCoordinate(3, 3),
                    new GeoCoordinate(3, 0),
                    new GeoCoordinate(0, 0)
                }));
            var geometryCollection = new MultiPolygon(new Polygon[] { geometry1, geometry2, geometry3 });

            var serialized = geometryCollection.ToGeoJson();
            serialized = serialized.RemoveWhitespace();

            Assert.AreEqual("{\"type\":\"MultiPolygon\",\"coordinates\":[[[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0],[0.0,0.0]]],[[[0.0,0.0],[2.0,0.0],[2.0,2.0],[0.0,2.0],[0.0,0.0]]],[[[0.0,0.0],[3.0,0.0],[3.0,3.0],[0.0,3.0],[0.0,0.0]]]]}",
                serialized);
        }

        /// <summary>
        /// Tests deserializing a multipolygon.
        /// </summary>
        [Test]
        public void TestMultiPolygonDeserialization()
        {
            var geometry = "{\"type\":\"MultiPolygon\",\"coordinates\":[[[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0],[0.0,0.0]]],[[[0.0,0.0],[2.0,0.0],[2.0,2.0],[0.0,2.0],[0.0,0.0]]],[[[0.0,0.0],[3.0,0.0],[3.0,3.0],[0.0,3.0],[0.0,0.0]]]]}".ToGeometry();

            Assert.IsNotNull(geometry);
            Assert.IsInstanceOf<MultiPolygon>(geometry);
            var polygons = new List<Polygon>(geometry as MultiPolygon);
            Assert.AreEqual(3, polygons.Count);
            Assert.AreEqual(5, polygons[0].Ring.Coordinates.Count);
            Assert.AreEqual(0, polygons[0].Ring.Coordinates[0].Latitude);
            Assert.AreEqual(0, polygons[0].Ring.Coordinates[0].Longitude);
            Assert.AreEqual(0, polygons[0].Ring.Coordinates[1].Latitude);
            Assert.AreEqual(1, polygons[0].Ring.Coordinates[1].Longitude);
            Assert.AreEqual(1, polygons[0].Ring.Coordinates[2].Latitude);
            Assert.AreEqual(1, polygons[0].Ring.Coordinates[2].Longitude);
            Assert.AreEqual(1, polygons[0].Ring.Coordinates[3].Latitude);
            Assert.AreEqual(0, polygons[0].Ring.Coordinates[3].Longitude);
            Assert.AreEqual(0, polygons[0].Ring.Coordinates[4].Latitude);
            Assert.AreEqual(0, polygons[0].Ring.Coordinates[4].Longitude);
            Assert.AreEqual(5, polygons[1].Ring.Coordinates.Count);
            Assert.AreEqual(0, polygons[1].Ring.Coordinates[0].Latitude);
            Assert.AreEqual(0, polygons[1].Ring.Coordinates[0].Longitude);
            Assert.AreEqual(0, polygons[1].Ring.Coordinates[1].Latitude);
            Assert.AreEqual(2, polygons[1].Ring.Coordinates[1].Longitude);
            Assert.AreEqual(2, polygons[1].Ring.Coordinates[2].Latitude);
            Assert.AreEqual(2, polygons[1].Ring.Coordinates[2].Longitude);
            Assert.AreEqual(2, polygons[1].Ring.Coordinates[3].Latitude);
            Assert.AreEqual(0, polygons[1].Ring.Coordinates[3].Longitude);
            Assert.AreEqual(0, polygons[1].Ring.Coordinates[4].Latitude);
            Assert.AreEqual(0, polygons[1].Ring.Coordinates[4].Longitude);
            Assert.AreEqual(5, polygons[2].Ring.Coordinates.Count);
            Assert.AreEqual(0, polygons[2].Ring.Coordinates[0].Latitude);
            Assert.AreEqual(0, polygons[2].Ring.Coordinates[0].Longitude);
            Assert.AreEqual(0, polygons[2].Ring.Coordinates[1].Latitude);
            Assert.AreEqual(3, polygons[2].Ring.Coordinates[1].Longitude);
            Assert.AreEqual(3, polygons[2].Ring.Coordinates[2].Latitude);
            Assert.AreEqual(3, polygons[2].Ring.Coordinates[2].Longitude);
            Assert.AreEqual(3, polygons[2].Ring.Coordinates[3].Latitude);
            Assert.AreEqual(0, polygons[2].Ring.Coordinates[3].Longitude);
            Assert.AreEqual(0, polygons[2].Ring.Coordinates[4].Latitude);
            Assert.AreEqual(0, polygons[2].Ring.Coordinates[4].Longitude);

            geometry = "{\"coordinates\":[[[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0],[0.0,0.0]]],[[[0.0,0.0],[2.0,0.0],[2.0,2.0],[0.0,2.0],[0.0,0.0]]],[[[0.0,0.0],[3.0,0.0],[3.0,3.0],[0.0,3.0],[0.0,0.0]]]],\"type\":\"MultiPolygon\"}".ToGeometry();

            Assert.IsNotNull(geometry);
            Assert.IsInstanceOf<MultiPolygon>(geometry);
            polygons = new List<Polygon>(geometry as MultiPolygon);
            Assert.AreEqual(3, polygons.Count);
            Assert.AreEqual(5, polygons[0].Ring.Coordinates.Count);
            Assert.AreEqual(0, polygons[0].Ring.Coordinates[0].Latitude);
            Assert.AreEqual(0, polygons[0].Ring.Coordinates[0].Longitude);
            Assert.AreEqual(0, polygons[0].Ring.Coordinates[1].Latitude);
            Assert.AreEqual(1, polygons[0].Ring.Coordinates[1].Longitude);
            Assert.AreEqual(1, polygons[0].Ring.Coordinates[2].Latitude);
            Assert.AreEqual(1, polygons[0].Ring.Coordinates[2].Longitude);
            Assert.AreEqual(1, polygons[0].Ring.Coordinates[3].Latitude);
            Assert.AreEqual(0, polygons[0].Ring.Coordinates[3].Longitude);
            Assert.AreEqual(0, polygons[0].Ring.Coordinates[4].Latitude);
            Assert.AreEqual(0, polygons[0].Ring.Coordinates[4].Longitude);
            Assert.AreEqual(5, polygons[1].Ring.Coordinates.Count);
            Assert.AreEqual(0, polygons[1].Ring.Coordinates[0].Latitude);
            Assert.AreEqual(0, polygons[1].Ring.Coordinates[0].Longitude);
            Assert.AreEqual(0, polygons[1].Ring.Coordinates[1].Latitude);
            Assert.AreEqual(2, polygons[1].Ring.Coordinates[1].Longitude);
            Assert.AreEqual(2, polygons[1].Ring.Coordinates[2].Latitude);
            Assert.AreEqual(2, polygons[1].Ring.Coordinates[2].Longitude);
            Assert.AreEqual(2, polygons[1].Ring.Coordinates[3].Latitude);
            Assert.AreEqual(0, polygons[1].Ring.Coordinates[3].Longitude);
            Assert.AreEqual(0, polygons[1].Ring.Coordinates[4].Latitude);
            Assert.AreEqual(0, polygons[1].Ring.Coordinates[4].Longitude);
            Assert.AreEqual(5, polygons[2].Ring.Coordinates.Count);
            Assert.AreEqual(0, polygons[2].Ring.Coordinates[0].Latitude);
            Assert.AreEqual(0, polygons[2].Ring.Coordinates[0].Longitude);
            Assert.AreEqual(0, polygons[2].Ring.Coordinates[1].Latitude);
            Assert.AreEqual(3, polygons[2].Ring.Coordinates[1].Longitude);
            Assert.AreEqual(3, polygons[2].Ring.Coordinates[2].Latitude);
            Assert.AreEqual(3, polygons[2].Ring.Coordinates[2].Longitude);
            Assert.AreEqual(3, polygons[2].Ring.Coordinates[3].Latitude);
            Assert.AreEqual(0, polygons[2].Ring.Coordinates[3].Longitude);
            Assert.AreEqual(0, polygons[2].Ring.Coordinates[4].Latitude);
            Assert.AreEqual(0, polygons[2].Ring.Coordinates[4].Longitude);
        }

        /// <summary>
        /// Tests serializing a linestring.
        /// </summary>
        [Test]
        public void TestLineStringSerialization()
        {
            var geometry = new LineString(
                new GeoCoordinate[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(0, 1),
                    new GeoCoordinate(1, 1),
                    new GeoCoordinate(1, 0)
                });

            var serialized = geometry.ToGeoJson();
            serialized = serialized.RemoveWhitespace();

            Assert.AreEqual("{\"type\":\"LineString\",\"coordinates\":[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0]]}",
                serialized);
        }

        /// <summary>
        /// Tests deserializing a linestring.
        /// </summary>
        [Test]
        public void TestLineStringDeserialization()
        {
            var geometry = "{\"type\":\"LineString\",\"coordinates\":[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0]]}".ToGeometry();

            Assert.IsNotNull(geometry);
            Assert.IsInstanceOf<LineString>(geometry);
            var linestring = geometry as LineString;
            Assert.AreEqual(4, linestring.Coordinates.Count);
            Assert.AreEqual(0, linestring.Coordinates[0].Latitude);
            Assert.AreEqual(0, linestring.Coordinates[0].Longitude);
            Assert.AreEqual(0, linestring.Coordinates[1].Latitude);
            Assert.AreEqual(1, linestring.Coordinates[1].Longitude);
            Assert.AreEqual(1, linestring.Coordinates[2].Latitude);
            Assert.AreEqual(1, linestring.Coordinates[2].Longitude);
            Assert.AreEqual(1, linestring.Coordinates[3].Latitude);
            Assert.AreEqual(0, linestring.Coordinates[3].Longitude);

            geometry = "{\"coordinates\":[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0]],\"type\":\"LineString\"}".ToGeometry();

            Assert.IsNotNull(geometry);
            Assert.IsInstanceOf<LineString>(geometry);
            linestring = geometry as LineString;
            Assert.AreEqual(4, linestring.Coordinates.Count);
            Assert.AreEqual(0, linestring.Coordinates[0].Latitude);
            Assert.AreEqual(0, linestring.Coordinates[0].Longitude);
            Assert.AreEqual(0, linestring.Coordinates[1].Latitude);
            Assert.AreEqual(1, linestring.Coordinates[1].Longitude);
            Assert.AreEqual(1, linestring.Coordinates[2].Latitude);
            Assert.AreEqual(1, linestring.Coordinates[2].Longitude);
            Assert.AreEqual(1, linestring.Coordinates[3].Latitude);
            Assert.AreEqual(0, linestring.Coordinates[3].Longitude);
        }

        /// <summary>
        /// Tests serializing a multilinestring.
        /// </summary>
        [Test]
        public void TestMultiLineStringSerialization()
        {
            var geometry1 = new LineString(
                new GeoCoordinate[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(0, 1),
                    new GeoCoordinate(1, 1),
                    new GeoCoordinate(1, 0)
                });
            var geometry2 = new LineString(
                new GeoCoordinate[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(0, 2),
                    new GeoCoordinate(2, 2),
                    new GeoCoordinate(2, 0)
                });
            var geometry3 = new LineString(
                new GeoCoordinate[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(0, 3),
                    new GeoCoordinate(3, 3),
                    new GeoCoordinate(3, 0)
                });
            var geometryCollection = new MultiLineString(new LineString[] { geometry1, geometry2, geometry3 });

            var serialized = geometryCollection.ToGeoJson();
            serialized = serialized.RemoveWhitespace();

            Assert.AreEqual("{\"type\":\"MultiLineString\",\"coordinates\":[[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0]],[[0.0,0.0],[2.0,0.0],[2.0,2.0],[0.0,2.0]],[[0.0,0.0],[3.0,0.0],[3.0,3.0],[0.0,3.0]]]}",
                serialized);
        }

        /// <summary>
        /// Test deserializing a multilinestring.
        /// </summary>
        [Test]
        public void TestMultiLineStringDeserialization()
        {
            var geometry = "{\"type\":\"MultiLineString\",\"coordinates\":[[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0]],[[0.0,0.0],[2.0,0.0],[2.0,2.0],[0.0,2.0]],[[0.0,0.0],[3.0,0.0],[3.0,3.0],[0.0,3.0]]]}".ToGeometry();

            Assert.IsNotNull(geometry);
            Assert.IsInstanceOf<MultiLineString>(geometry);
            var linestrings = new List<LineString>(geometry as MultiLineString);
            Assert.AreEqual(3, linestrings.Count);
            Assert.AreEqual(4, linestrings[0].Coordinates.Count);
            Assert.AreEqual(0, linestrings[0].Coordinates[0].Latitude);
            Assert.AreEqual(0, linestrings[0].Coordinates[0].Longitude);
            Assert.AreEqual(0, linestrings[0].Coordinates[1].Latitude);
            Assert.AreEqual(1, linestrings[0].Coordinates[1].Longitude);
            Assert.AreEqual(1, linestrings[0].Coordinates[2].Latitude);
            Assert.AreEqual(1, linestrings[0].Coordinates[2].Longitude);
            Assert.AreEqual(1, linestrings[0].Coordinates[3].Latitude);
            Assert.AreEqual(0, linestrings[0].Coordinates[3].Longitude);
            Assert.AreEqual(4, linestrings[1].Coordinates.Count);
            Assert.AreEqual(0, linestrings[1].Coordinates[0].Latitude);
            Assert.AreEqual(0, linestrings[1].Coordinates[0].Longitude);
            Assert.AreEqual(0, linestrings[1].Coordinates[1].Latitude);
            Assert.AreEqual(2, linestrings[1].Coordinates[1].Longitude);
            Assert.AreEqual(2, linestrings[1].Coordinates[2].Latitude);
            Assert.AreEqual(2, linestrings[1].Coordinates[2].Longitude);
            Assert.AreEqual(2, linestrings[1].Coordinates[3].Latitude);
            Assert.AreEqual(0, linestrings[1].Coordinates[3].Longitude);
            Assert.AreEqual(4, linestrings[2].Coordinates.Count);
            Assert.AreEqual(0, linestrings[2].Coordinates[0].Latitude);
            Assert.AreEqual(0, linestrings[2].Coordinates[0].Longitude);
            Assert.AreEqual(0, linestrings[2].Coordinates[1].Latitude);
            Assert.AreEqual(3, linestrings[2].Coordinates[1].Longitude);
            Assert.AreEqual(3, linestrings[2].Coordinates[2].Latitude);
            Assert.AreEqual(3, linestrings[2].Coordinates[2].Longitude);
            Assert.AreEqual(3, linestrings[2].Coordinates[3].Latitude);
            Assert.AreEqual(0, linestrings[2].Coordinates[3].Longitude);

            geometry = "{\"coordinates\":[[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0]],[[0.0,0.0],[2.0,0.0],[2.0,2.0],[0.0,2.0]],[[0.0,0.0],[3.0,0.0],[3.0,3.0],[0.0,3.0]]],\"type\":\"MultiLineString\"}".ToGeometry();

            Assert.IsNotNull(geometry);
            Assert.IsInstanceOf<MultiLineString>(geometry);
            linestrings = new List<LineString>(geometry as MultiLineString);
            Assert.AreEqual(3, linestrings.Count);
            Assert.AreEqual(4, linestrings[0].Coordinates.Count);
            Assert.AreEqual(0, linestrings[0].Coordinates[0].Latitude);
            Assert.AreEqual(0, linestrings[0].Coordinates[0].Longitude);
            Assert.AreEqual(0, linestrings[0].Coordinates[1].Latitude);
            Assert.AreEqual(1, linestrings[0].Coordinates[1].Longitude);
            Assert.AreEqual(1, linestrings[0].Coordinates[2].Latitude);
            Assert.AreEqual(1, linestrings[0].Coordinates[2].Longitude);
            Assert.AreEqual(1, linestrings[0].Coordinates[3].Latitude);
            Assert.AreEqual(0, linestrings[0].Coordinates[3].Longitude);
            Assert.AreEqual(4, linestrings[1].Coordinates.Count);
            Assert.AreEqual(0, linestrings[1].Coordinates[0].Latitude);
            Assert.AreEqual(0, linestrings[1].Coordinates[0].Longitude);
            Assert.AreEqual(0, linestrings[1].Coordinates[1].Latitude);
            Assert.AreEqual(2, linestrings[1].Coordinates[1].Longitude);
            Assert.AreEqual(2, linestrings[1].Coordinates[2].Latitude);
            Assert.AreEqual(2, linestrings[1].Coordinates[2].Longitude);
            Assert.AreEqual(2, linestrings[1].Coordinates[3].Latitude);
            Assert.AreEqual(0, linestrings[1].Coordinates[3].Longitude);
            Assert.AreEqual(4, linestrings[2].Coordinates.Count);
            Assert.AreEqual(0, linestrings[2].Coordinates[0].Latitude);
            Assert.AreEqual(0, linestrings[2].Coordinates[0].Longitude);
            Assert.AreEqual(0, linestrings[2].Coordinates[1].Latitude);
            Assert.AreEqual(3, linestrings[2].Coordinates[1].Longitude);
            Assert.AreEqual(3, linestrings[2].Coordinates[2].Latitude);
            Assert.AreEqual(3, linestrings[2].Coordinates[2].Longitude);
            Assert.AreEqual(3, linestrings[2].Coordinates[3].Latitude);
            Assert.AreEqual(0, linestrings[2].Coordinates[3].Longitude);
        }

        /// <summary>
        /// Tests serializing a point.
        /// </summary>
        [Test]
        public void TestPointSerialization()
        {
            var geometry = new Point(new GeoCoordinate(0, 1));

            var serialized = geometry.ToGeoJson();
            serialized = serialized.RemoveWhitespace();

            Assert.AreEqual("{\"type\":\"Point\",\"coordinates\":[1.0,0.0]}",
                serialized);
        }
        
        /// <summary>
        /// Tests deserializing a point.
        /// </summary>
        [Test]
        public void TestPointDeserialization()
        {
            var geometry = "{\"type\":\"Point\",\"coordinates\":[1.0,0.0]}".ToGeometry();

            Assert.IsNotNull(geometry);
            Assert.IsInstanceOf<Point>(geometry);
            var point = geometry as Point;
            Assert.AreEqual(0, point.Coordinate.Latitude);
            Assert.AreEqual(1, point.Coordinate.Longitude);

            geometry = "{\"coordinates\":[1.0,0.0],\"type\":\"Point\"}".ToGeometry();

            Assert.IsNotNull(geometry);
            Assert.IsInstanceOf<Point>(geometry);
            point = geometry as Point;
            Assert.AreEqual(0, point.Coordinate.Latitude);
            Assert.AreEqual(1, point.Coordinate.Longitude);
        }

        /// <summary>
        /// Tests serializing a geometrycollection.
        /// </summary>
        [Test]
        public void TestGeometryCollectionSerialization()
        {
            var geometry1 = new LineString(
                new GeoCoordinate[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(0, 1),
                    new GeoCoordinate(1, 1),
                    new GeoCoordinate(1, 0)
                });
            var geometry2 = new LineString(
                new GeoCoordinate[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(0, 2),
                    new GeoCoordinate(2, 2),
                    new GeoCoordinate(2, 0)
                });
            var geometry3 = new LineString(
                new GeoCoordinate[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(0, 3),
                    new GeoCoordinate(3, 3),
                    new GeoCoordinate(3, 0)
                });
            var geometry4 = new LineairRing(
                new GeoCoordinate[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(0, 1),
                    new GeoCoordinate(1, 1),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(0, 0)
                });
            var geometry5 = new Polygon(new LineairRing(
                new GeoCoordinate[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(0, 1),
                    new GeoCoordinate(1, 1),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(0, 0)
                }));
            var geometry6 = new MultiPolygon(geometry5, new Polygon(new LineairRing(
                new GeoCoordinate[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(0, 2),
                    new GeoCoordinate(2, 2),
                    new GeoCoordinate(2, 0),
                    new GeoCoordinate(0, 0)
                })));
            var geometry7 = new Point(new GeoCoordinate(0, 1));
            var geometry8 = new MultiPoint(geometry7, new Point(new GeoCoordinate(0, 2)));
            var geometryCollection = new GeometryCollection(
                geometry1, geometry2, geometry3,
                geometry4, geometry5, geometry6,
                geometry7, geometry8);

            var serialized = geometryCollection.ToGeoJson();
            serialized = serialized.RemoveWhitespace();

            Assert.AreEqual("{\"type\":\"GeometryCollection\",\"geometries\":[{\"type\":\"LineString\",\"coordinates\":[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0]]},{\"type\":\"LineString\",\"coordinates\":[[0.0,0.0],[2.0,0.0],[2.0,2.0],[0.0,2.0]]},{\"type\":\"LineString\",\"coordinates\":[[0.0,0.0],[3.0,0.0],[3.0,3.0],[0.0,3.0]]},{\"type\":\"Polygon\",\"coordinates\":[[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0],[0.0,0.0]]]},{\"type\":\"Polygon\",\"coordinates\":[[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0],[0.0,0.0]]]},{\"type\":\"MultiPolygon\",\"coordinates\":[[[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0],[0.0,0.0]]],[[[0.0,0.0],[2.0,0.0],[2.0,2.0],[0.0,2.0],[0.0,0.0]]]]},{\"type\":\"Point\",\"coordinates\":[1.0,0.0]},{\"type\":\"MultiPoint\",\"coordinates\":[[1.0,0.0],[2.0,0.0]]}]}",
                serialized);
        }

        /// <summary>
        /// Tests deserializing a geometrycollection.
        /// </summary>
        [Test]
        public void TestGeometryCollectionDeserialization()
        {
            var geometry = "{\"type\":\"GeometryCollection\",\"geometries\":[{\"type\":\"LineString\",\"coordinates\":[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0]]},{\"type\":\"LineString\",\"coordinates\":[[0.0,0.0],[2.0,0.0],[2.0,2.0],[0.0,2.0]]},{\"type\":\"LineString\",\"coordinates\":[[0.0,0.0],[3.0,0.0],[3.0,3.0],[0.0,3.0]]},{\"type\":\"Polygon\",\"coordinates\":[[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0],[0.0,0.0]]]},{\"type\":\"Polygon\",\"coordinates\":[[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0],[0.0,0.0]]]},{\"type\":\"MultiPolygon\",\"coordinates\":[[[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0],[0.0,0.0]]],[[[0.0,0.0],[2.0,0.0],[2.0,2.0],[0.0,2.0],[0.0,0.0]]]]},{\"type\":\"Point\",\"coordinates\":[1.0,0.0]},{\"type\":\"MultiPoint\",\"coordinates\":[[1.0,0.0],[2.0,0.0]]}]}".ToGeometry();

            Assert.IsNotNull(geometry);
            Assert.IsInstanceOf<GeometryCollection>(geometry);
            var geometries = new List<Geometry>(geometry as GeometryCollection);
            Assert.AreEqual(8, geometries.Count);
        }

        /// <summary>
        /// Tests serializing a multipoint.
        /// </summary>
        [Test]
        public void TestMultiPointSerialization()
        {
            var geometry1 = new Point(new GeoCoordinate(0, 1));
            var geometry2 = new Point(new GeoCoordinate(1, 1));
            var geometry3 = new Point(new GeoCoordinate(1, 0));
            var geometryCollection = new MultiPoint(new Point[] { geometry1, geometry2, geometry3 });

            var serialized = geometryCollection.ToGeoJson();
            serialized = serialized.RemoveWhitespace();

            Assert.AreEqual("{\"type\":\"MultiPoint\",\"coordinates\":[[1.0,0.0],[1.0,1.0],[0.0,1.0]]}",
                serialized);
        }

        /// <summary>
        /// Tests deserializing a multipoint.
        /// </summary>
        [Test]
        public void TestMultiPointDeserialization()
        {
            var geometry = "{\"type\":\"MultiPoint\",\"coordinates\":[[1.0,0.0],[1.0,1.0],[0.0,1.0]]}".ToGeometry();

            Assert.IsNotNull(geometry);
            Assert.IsInstanceOf<MultiPoint>(geometry);
            var multiPoint = geometry as MultiPoint;
            var points = new List<Point>(multiPoint);
            Assert.AreEqual(0, points[0].Coordinate.Latitude);
            Assert.AreEqual(1, points[0].Coordinate.Longitude);
            Assert.AreEqual(1, points[1].Coordinate.Latitude);
            Assert.AreEqual(1, points[1].Coordinate.Longitude);
            Assert.AreEqual(1, points[2].Coordinate.Latitude);
            Assert.AreEqual(0, points[2].Coordinate.Longitude);

            geometry = "{\"coordinates\":[[1.0,0.0],[1.0,1.0],[0.0,1.0]],\"type\":\"MultiPoint\"}".ToGeometry();

            Assert.IsNotNull(geometry);
            Assert.IsInstanceOf<MultiPoint>(geometry);
            multiPoint = geometry as MultiPoint;
            points = new List<Point>(multiPoint);
            Assert.AreEqual(0, points[0].Coordinate.Latitude);
            Assert.AreEqual(1, points[0].Coordinate.Longitude);
            Assert.AreEqual(1, points[1].Coordinate.Latitude);
            Assert.AreEqual(1, points[1].Coordinate.Longitude);
            Assert.AreEqual(1, points[2].Coordinate.Latitude);
            Assert.AreEqual(0, points[2].Coordinate.Longitude);
        }

        /// <summary>
        /// Tests serializing a feature.
        /// </summary>
        [Test]
        public void TestFeatureSerialization()
        {
            // a feature with a point.
            var geometry = (Geometry)new Point(new GeoCoordinate(0, 1));
            var feature = new Feature(geometry);

            var serialized = feature.ToGeoJson();
            serialized = serialized.RemoveWhitespace();

            Assert.AreEqual("{\"type\":\"Feature\",\"properties\":{},\"geometry\":{\"type\":\"Point\",\"coordinates\":[1.0,0.0]}}",
                serialized);

            feature = new Feature(geometry, new SimpleGeometryAttributeCollection(new GeometryAttribute[] 
            {
                new GeometryAttribute()
                {
                    Key = "key1",
                    Value = "value1"
                }
            }));

            serialized = feature.ToGeoJson();
            serialized = serialized.RemoveWhitespace();

            Assert.AreEqual("{\"type\":\"Feature\",\"properties\":{\"key1\":\"value1\"},\"geometry\":{\"type\":\"Point\",\"coordinates\":[1.0,0.0]}}",
                serialized);

            // a feature with a linestring.
            geometry = new LineString(
                new GeoCoordinate[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(0, 1),
                    new GeoCoordinate(1, 1),
                    new GeoCoordinate(1, 0)
                });
            feature = new Feature(geometry);

            serialized = feature.ToGeoJson();
            serialized = serialized.RemoveWhitespace();

            Assert.AreEqual("{\"type\":\"Feature\",\"properties\":{},\"geometry\":{\"type\":\"LineString\",\"coordinates\":[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0]]}}",
                serialized);

            // a featurer with a linearring.
            geometry = new LineairRing(
                new GeoCoordinate[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(0, 1),
                    new GeoCoordinate(1, 1),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(0, 0)
                });
            feature = new Feature(geometry);

            serialized = feature.ToGeoJson();
            serialized = serialized.RemoveWhitespace();

            Assert.AreEqual("{\"type\":\"Feature\",\"properties\":{},\"geometry\":{\"type\":\"Polygon\",\"coordinates\":[[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0],[0.0,0.0]]]}}",
                serialized);

            // a featurer with a polygon.
            geometry = new Polygon(new LineairRing(
                new GeoCoordinate[]
                {
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(0, 1),
                    new GeoCoordinate(1, 1),
                    new GeoCoordinate(1, 0),
                    new GeoCoordinate(0, 0)
                }));
            feature = new Feature(geometry);

            serialized = feature.ToGeoJson();
            serialized = serialized.RemoveWhitespace();

            Assert.AreEqual("{\"type\":\"Feature\",\"properties\":{},\"geometry\":{\"type\":\"Polygon\",\"coordinates\":[[[0.0,0.0],[1.0,0.0],[1.0,1.0],[0.0,1.0],[0.0,0.0]]]}}",
                serialized);
        }

        /// <summary>
        /// Tests deserializing a feature.
        /// </summary>
        [Test]
        public void TestFeatureDeserialization()
        {
            var feature = "{\"type\":\"Feature\",\"properties\":{},\"geometry\":{\"type\":\"Point\",\"coordinates\":[1.0,0.0]}}".ToFeature();

            Assert.IsNotNull(feature);
            Assert.IsInstanceOf<Feature>(feature);
            Assert.AreEqual(0, feature.Attributes.Count);
            var geometry = feature.Geometry;
            Assert.IsNotNull(geometry);
            Assert.IsInstanceOf<Point>(geometry);

            feature = "{\"type\":\"Feature\",\"properties\":{\"key1\":\"value1\"},\"geometry\":{\"type\":\"Point\",\"coordinates\":[1.0,0.0]}}".ToFeature();
            Assert.IsNotNull(feature);
            Assert.IsInstanceOf<Feature>(feature);
            Assert.AreEqual(1, feature.Attributes.Count);
            Assert.IsTrue(feature.Attributes.ContainsKeyValue("key1", "value1"));
            geometry = feature.Geometry;
            Assert.IsNotNull(geometry);
            Assert.IsInstanceOf<Point>(geometry);
        }

        /// <summary>
        /// Tests serializing a feature collection.
        /// </summary>
        [Test]
        public void TestFeatureCollectionSerialization()
        {
            var geometry = new Point(new GeoCoordinate(0, 1));
            var feature1 = new Feature(geometry);
            var feature2 = new Feature(geometry, new SimpleGeometryAttributeCollection(new GeometryAttribute[] 
            {
                new GeometryAttribute()
                {
                    Key = "key1",
                    Value = "value1"
                }
            }));

            var featureCollection = new FeatureCollection(new Feature[] { feature1, feature2 });

            var serialized = featureCollection.ToGeoJson();
            serialized = serialized.RemoveWhitespace();

            Assert.AreEqual("{\"type\":\"FeatureCollection\",\"features\":[" + 
                "{\"type\":\"Feature\",\"properties\":{},\"geometry\":{\"type\":\"Point\",\"coordinates\":[1.0,0.0]}}," +
                "{\"type\":\"Feature\",\"properties\":{\"key1\":\"value1\"},\"geometry\":{\"type\":\"Point\",\"coordinates\":[1.0,0.0]}}" + 
                "]}",
                serialized);
        }

        /// <summary>
        /// Tests deserializing a feature collection.
        /// </summary>
        [Test]
        public void TestFeatureCollectionDeserialization()
        {
            var featureCollection = ("{\"type\":\"FeatureCollection\",\"features\":[" +
                "{\"type\":\"Feature\",\"properties\":{},\"geometry\":{\"type\":\"Point\",\"coordinates\":[1.0,0.0]}}," +
                "{\"type\":\"Feature\",\"properties\":{\"key1\":\"value1\"},\"geometry\":{\"type\":\"Point\",\"coordinates\":[1.0,0.0]}}" +
                "]}").ToFeatureCollection();

            Assert.IsNotNull(featureCollection);
            Assert.IsInstanceOf<FeatureCollection>(featureCollection);
            Assert.AreEqual(2, featureCollection.Count);
        }

        /// <summary>
        /// Tests deserializing an empty feature collection.
        /// </summary>
        [Test]
        public void TestEmptyFeatureCollection()
        {
            var featureCollection = ("{\"type\":\"FeatureCollection\",\"features\":[]}").ToFeatureCollection();

            Assert.IsNotNull(featureCollection);
            Assert.IsInstanceOf<FeatureCollection>(featureCollection);
            Assert.AreEqual(0, featureCollection.Count);
        }
    }
}