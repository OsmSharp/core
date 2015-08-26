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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OsmSharp.Geo.Geometries;
using OsmSharp.Math.Geo;

namespace OsmSharp.Test.Geo.Geometries
{
    /// <summary>
    /// Containts tests for the polygons.
    /// </summary>
    [TestFixture]
    public class PolygonTests
    {
        /// <summary>
        /// Tests the contains function for a point in a polygon.
        /// </summary>
        [Test]
        public void TestPolygonContainsPoint()
        {
            LineairRing outer = new LineairRing(new GeoCoordinate(0, 0),
                new GeoCoordinate(5, 0), new GeoCoordinate(5, 5), new GeoCoordinate(0, 5), new GeoCoordinate(0, 0));
            LineairRing inner = new LineairRing(new GeoCoordinate(2, 2),
                new GeoCoordinate(4, 2), new GeoCoordinate(4, 4), new GeoCoordinate(2, 4),new GeoCoordinate(2, 2));

            Polygon polygon = new Polygon(outer, new LineairRing[] { inner });

            foreach (GeoCoordinate ringCoordinate in outer.Coordinates)
            {
                Assert.IsTrue(polygon.Contains(ringCoordinate));
            }
            foreach (GeoCoordinate ringCoordinate in inner.Coordinates)
            {
                Assert.IsFalse(polygon.Contains(ringCoordinate));
            }

            GeoCoordinate coordinate = new GeoCoordinate(1, 1);
            Assert.IsTrue(polygon.Contains(coordinate));
            coordinate = new GeoCoordinate(3, 3);
            Assert.IsFalse(polygon.Contains(coordinate));
            coordinate = new GeoCoordinate(-1, 1);
            Assert.IsFalse(polygon.Contains(coordinate));
        }

        /// <summary>
        /// Tests the contains function for a lineair ring against another lineair ring.
        /// </summary>
        [Test]
        public void TestLineairRingContainsRing()
        {
            LineairRing outer = new LineairRing(new GeoCoordinate(0, 0),
                new GeoCoordinate(5, 0), new GeoCoordinate(5, 5), new GeoCoordinate(0, 5), new GeoCoordinate(0, 0));
            LineairRing inner = new LineairRing(new GeoCoordinate(1, 1),
                new GeoCoordinate(2, 1), new GeoCoordinate(2, 2), new GeoCoordinate(1, 2), new GeoCoordinate(1, 1));

            LineairRing test = new LineairRing(new GeoCoordinate(1, 3),
                new GeoCoordinate(2, 3), new GeoCoordinate(2, 4), new GeoCoordinate(1, 4), new GeoCoordinate(1, 3));
            Polygon polygon = new Polygon(outer, new LineairRing[] { inner });

            Assert.IsTrue(polygon.Contains(test));

            outer = new LineairRing(new GeoCoordinate(0, 0),
                new GeoCoordinate(5, 0), new GeoCoordinate(5, 5), new GeoCoordinate(0, 5), new GeoCoordinate(0, 0));
            inner = new LineairRing(new GeoCoordinate(1, 1),
                new GeoCoordinate(4, 1), new GeoCoordinate(4, 4), new GeoCoordinate(1, 4), new GeoCoordinate(1, 1));
            test = new LineairRing(new GeoCoordinate(2, 2),
                new GeoCoordinate(3, 2), new GeoCoordinate(3, 3), new GeoCoordinate(2, 3), new GeoCoordinate(2, 2));
            polygon = new Polygon(outer, new LineairRing[] { inner });

            Assert.IsFalse(polygon.Contains(test));
        }
    }
}