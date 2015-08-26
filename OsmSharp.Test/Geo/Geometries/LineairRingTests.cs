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
using OsmSharp.Math.Primitives;

namespace OsmSharp.Test.Geo.Geometries
{
    /// <summary>
    /// Containts tests for the lineair ring.
    /// </summary>
    [TestFixture]
    public class LineairRingTests
    {
        /// <summary>
        /// Tests the contains function for a point on a lineair ring.
        /// </summary>
        [Test]
        public void TestLineairRingContainsPoint()
        {
            LineairRing ring = new LineairRing(new GeoCoordinate(0, 0),
                new GeoCoordinate(3, 0), new GeoCoordinate(0, 3), new GeoCoordinate(0, 0));

            foreach (GeoCoordinate ringCoordinate in ring.Coordinates)
            {
                Assert.IsTrue(ring.Contains(ringCoordinate));
            }

            GeoCoordinate coordinate = new GeoCoordinate(1, 1);
            Assert.IsTrue(ring.Contains(coordinate));
            coordinate = new GeoCoordinate(2, 2);
            Assert.IsFalse(ring.Contains(coordinate));
            coordinate = new GeoCoordinate(-1, 1);
            Assert.IsFalse(ring.Contains(coordinate));
            coordinate = new GeoCoordinate(0, 1);
            Assert.IsTrue(ring.Contains(coordinate));
            coordinate = new GeoCoordinate(1, 0);
            Assert.IsTrue(ring.Contains(coordinate));
        }

        /// <summary>
        /// Tests the contains function for a lineair ring against another lineair ring.
        /// </summary>
        [Test]
        public void TestLineairRingContainsRing()
        {
            LineairRing inner = new LineairRing(new GeoCoordinate(1, 1),
                new GeoCoordinate(1, 0.2), new GeoCoordinate(0.2, 1), new GeoCoordinate(1, 1));
            LineairRing outer = new LineairRing(new GeoCoordinate(0, 0),
                new GeoCoordinate(2, 0), new GeoCoordinate(2, 2), new GeoCoordinate(0, 2), new GeoCoordinate(0, 0));

            Assert.IsTrue(outer.Contains(inner));
        }
    }
}
