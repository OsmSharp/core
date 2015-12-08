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
using OsmSharp.Units.Distance;
using OsmSharp.Math.Geo.Meta;

namespace OsmSharp.Test.Geo
{
    /// <summary>
    /// Contains tests for the GeoCoordinate class.
    /// </summary>
    [TestFixture]
    public class GeoCoordinateTests
    {
        /// <summary>
        /// Tests the offset distance estimate.
        /// </summary>
        [Test]
        public void TestGeoCoordinateOffsetEstimate()
        {
            GeoCoordinate coord1 = new GeoCoordinate(51, 4.8);

            Meter distance = 10000;

            GeoCoordinate coord3 = coord1.OffsetWithDistances(distance);
            GeoCoordinate coord3_lat = new GeoCoordinate(coord3.Latitude, coord1.Longitude);
            GeoCoordinate coord3_lon = new GeoCoordinate(coord1.Latitude, coord3.Longitude);

            Meter distance_lat = coord3_lat.DistanceReal(coord1);
            Meter distance_lon = coord3_lon.DistanceReal(coord1);

            Assert.AreEqual(distance.Value, distance_lat.Value, 0.001);
            Assert.AreEqual(distance.Value, distance_lon.Value, 0.001);
        }

        /// <summary>
        /// Tests the random offset 
        /// </summary>
        [Test]
        public void TestGeoCoordinateOffsetRandom()
        {
            var generator = new OsmSharp.Math.Random.RandomGenerator(10124613);

            for (int idx = 0; idx < 1000; idx++)
            {
                GeoCoordinate start = new GeoCoordinate(51, 4.8);
                GeoCoordinate offset = start.OffsetRandom(generator, 20);

                double distance = offset.DistanceReal(start).Value;
                Assert.IsTrue(distance <= 20.001);
            }
        }

        /// <summary>
        /// Tests the random offset 
        /// </summary>
        [Test]
        public void TestGeoCoordinateOffsetWithDirection()
        {
            var distance = 1000; // 1km
            var start = new GeoCoordinate(53.32056, 1.72972);

            var offset = start.OffsetWithDirection(distance, DirectionEnum.North);
            Assert.AreEqual(53.32950, offset.Latitude, 0.0001);
            Assert.AreEqual(1.72970, offset.Longitude, 0.0001);

            offset = start.OffsetWithDirection(distance, DirectionEnum.NorthEast);
            Assert.AreEqual(53.32690, offset.Latitude, 0.0001);
            Assert.AreEqual(1.74040, offset.Longitude, 0.0001);

            start = new GeoCoordinate(0, 0);

            offset = start.OffsetWithDirection(distance, DirectionEnum.West);
            Assert.AreEqual(0, offset.Latitude, 0.0001);
            Assert.AreEqual(-0.008984, offset.Longitude, 0.0001);

            offset = start.OffsetWithDirection(distance, DirectionEnum.East);
            Assert.AreEqual(0, offset.Latitude, 0.0001);
            Assert.AreEqual(0.008984, offset.Longitude, 0.0001);

            offset = start.OffsetWithDirection(distance, DirectionEnum.North);
            Assert.AreEqual(0.008896, offset.Latitude, 0.0001);
            Assert.AreEqual(0, offset.Longitude, 0.0001);

            offset = start.OffsetWithDirection(distance, DirectionEnum.South);
            Assert.AreEqual(-0.008896, offset.Latitude, 0.0001);
            Assert.AreEqual(0, offset.Longitude, 0.0001);
        }
    }
}