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
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Osm.Tiles;

namespace OsmSharp.Test.Osm.Tiles
{
    /// <summary>
    /// Tests for the webmercator projection.
    /// </summary>
    [TestFixture]
    public class WebMercatorTests
    {
        /// <summary>
        /// Tests simple web mercator projection projecting a tile nicely onto 256x256 squares.
        /// </summary>
        [Test]
        public void TestSimpleWebMercator()
        {
            // TODO: stabalize the webmercator projection numerically for lower zoom levels (0-9).
            var mercator = new WebMercator();
            for (int zoomLevel = 10; zoomLevel <= 25; zoomLevel++)
            {
                var tile = Tile.CreateAroundLocation(new GeoCoordinate(0, 0), zoomLevel);

                var topleft = mercator.ToPixel(tile.Box.TopLeft);
                var bottomright = mercator.ToPixel(tile.Box.BottomRight);

                var scaleFactor = mercator.ToZoomFactor(zoomLevel);

                Assert.AreEqual(-256, (topleft[0] - bottomright[0]) * scaleFactor, 0.01);
                Assert.AreEqual(-256, (topleft[1] - bottomright[1]) * scaleFactor, 0.01);
            }

            var coordinate = new GeoCoordinate(51.26337, 4.78739);
            var projected = mercator.ToPixel(coordinate);
            var reProjected = mercator.ToGeoCoordinates(projected[0], projected[1]);

            Assert.AreEqual(coordinate.Longitude, reProjected.Longitude, 0.0001);
            Assert.AreEqual(coordinate.Latitude, reProjected.Latitude, 0.0001);
        }

        /// <summary>
        /// Tests simple web mercator projection zoom level zoom factor conversion.
        /// </summary>
        [Test]
        public void TestSimpleWebMercatorZoomLevel()
        {
            var mercator = new WebMercator();

            for (int orignalLevel = 0; orignalLevel < 20; orignalLevel++)
            {
                double zoomFactor = mercator.ToZoomFactor(orignalLevel);
                double zoomLevel = mercator.ToZoomLevel(zoomFactor);

                Assert.AreEqual(orignalLevel, zoomLevel, 0.001);
            }
        }
    }
}