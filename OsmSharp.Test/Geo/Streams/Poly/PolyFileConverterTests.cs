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
using OsmSharp.Geo.Streams.Poly;
using System.Linq;

namespace OsmSharp.Test.Geo.Streams.Poly
{
    /// <summary>
    /// Containts test for poly files.
    /// </summary>
    [TestFixture]
    class PolyFileConverterTests
    {
        /// <summary>
        /// Tests a basic poly file.
        /// </summary>
        [Test]
        public void TestPoly()
        {
            var poly = "australia_v\n" +
                "1\n" +
                    " 0.1446693E+03 -0.3826255E+02\n" +
                    " 0.1446627E+03 -0.3825661E+02\n" +
                    " 0.1446763E+03 -0.3824465E+02\n" +
                    " 0.1446813E+03 -0.3824343E+02\n" +
                    " 0.1446824E+03 -0.3824484E+02\n" +
                    " 0.1446826E+03 -0.3825356E+02\n" +
                    " 0.1446876E+03 -0.3825210E+02\n" +
                    " 0.1446919E+03 -0.3824719E+02\n" +
                    " 0.1447006E+03 -0.3824723E+02\n" +
                    " 0.1447042E+03 -0.3825078E+02\n" +
                    " 0.1446758E+03 -0.3826229E+02\n" +
                    " 0.1446693E+03 -0.3826255E+02\n" +
                "END\n" +
                "END";

            var feature = PolyFileConverter.ReadPolygon(poly);
            Assert.IsNotNull(feature);
            Assert.IsNotNull(feature.Attributes);
            Assert.IsInstanceOf<Polygon>(feature.Geometry);
            Assert.IsTrue(feature.Attributes.ContainsKey("name"));
            Assert.AreEqual("australia_v", feature.Attributes["name"]);
            var polygon = feature.Geometry as Polygon;
            Assert.IsNotNull(polygon.Ring);
            Assert.AreEqual(12, polygon.Ring.Coordinates.Count);
            Assert.AreEqual(-38.26255, polygon.Ring.Coordinates[0].Latitude);
            Assert.AreEqual(144.6693, polygon.Ring.Coordinates[0].Longitude);
            Assert.AreEqual(-38.25661, polygon.Ring.Coordinates[1].Latitude);
            Assert.AreEqual(144.6627, polygon.Ring.Coordinates[1].Longitude);
            Assert.AreEqual(-38.24465, polygon.Ring.Coordinates[2].Latitude);
            Assert.AreEqual(144.6763, polygon.Ring.Coordinates[2].Longitude);
            Assert.AreEqual(-38.24343, polygon.Ring.Coordinates[3].Latitude);
            Assert.AreEqual(144.6813, polygon.Ring.Coordinates[3].Longitude);
            Assert.AreEqual(-38.24484, polygon.Ring.Coordinates[4].Latitude);
            Assert.AreEqual(144.6824, polygon.Ring.Coordinates[4].Longitude);
            Assert.AreEqual(-38.25356, polygon.Ring.Coordinates[5].Latitude);
            Assert.AreEqual(144.6826, polygon.Ring.Coordinates[5].Longitude);
            Assert.AreEqual(-38.2521, polygon.Ring.Coordinates[6].Latitude);
            Assert.AreEqual(144.6876, polygon.Ring.Coordinates[6].Longitude);
            Assert.AreEqual(-38.24719, polygon.Ring.Coordinates[7].Latitude);
            Assert.AreEqual(144.6919, polygon.Ring.Coordinates[7].Longitude);
            Assert.AreEqual(-38.24723, polygon.Ring.Coordinates[8].Latitude);
            Assert.AreEqual(144.7006, polygon.Ring.Coordinates[8].Longitude);
            Assert.AreEqual(-38.25078, polygon.Ring.Coordinates[9].Latitude);
            Assert.AreEqual(144.7042, polygon.Ring.Coordinates[9].Longitude);
            Assert.AreEqual(-38.26229, polygon.Ring.Coordinates[10].Latitude);
            Assert.AreEqual(144.6758, polygon.Ring.Coordinates[10].Longitude);
            Assert.AreEqual(-38.26255, polygon.Ring.Coordinates[11].Latitude);
            Assert.AreEqual(144.6693, polygon.Ring.Coordinates[11].Longitude);
        }

        /// <summary>
        /// Tests a polyfile with a centroid.
        /// </summary>
        [Test]
        public void TestPolyCentroid()
        {
            var poly = "australia_v\n" +
                "1\n" +
                    " 0.1446763E+03 -0.3825659E+02\n" +
                    " 0.1446693E+03 -0.3826255E+02\n" +
                    " 0.1446627E+03 -0.3825661E+02\n" +
                    " 0.1446763E+03 -0.3824465E+02\n" +
                    " 0.1446813E+03 -0.3824343E+02\n" +
                    " 0.1446824E+03 -0.3824484E+02\n" +
                    " 0.1446826E+03 -0.3825356E+02\n" +
                    " 0.1446876E+03 -0.3825210E+02\n" +
                    " 0.1446919E+03 -0.3824719E+02\n" +
                    " 0.1447006E+03 -0.3824723E+02\n" +
                    " 0.1447042E+03 -0.3825078E+02\n" +
                    " 0.1446758E+03 -0.3826229E+02\n" +
                    " 0.1446693E+03 -0.3826255E+02\n" +
                "END\n" +
                "END";

            var feature = PolyFileConverter.ReadPolygon(poly);
            Assert.IsNotNull(feature);
            Assert.IsNotNull(feature.Attributes);
            Assert.IsInstanceOf<Polygon>(feature.Geometry);
            Assert.IsTrue(feature.Attributes.ContainsKey("name"));
            Assert.AreEqual("australia_v", feature.Attributes["name"]);
            var polygon = feature.Geometry as Polygon;
            Assert.IsNotNull(polygon.Ring);
            Assert.AreEqual(12, polygon.Ring.Coordinates.Count);
            Assert.AreEqual(-38.26255, polygon.Ring.Coordinates[0].Latitude);
            Assert.AreEqual(144.6693, polygon.Ring.Coordinates[0].Longitude);
            Assert.AreEqual(-38.25661, polygon.Ring.Coordinates[1].Latitude);
            Assert.AreEqual(144.6627, polygon.Ring.Coordinates[1].Longitude);
            Assert.AreEqual(-38.24465, polygon.Ring.Coordinates[2].Latitude);
            Assert.AreEqual(144.6763, polygon.Ring.Coordinates[2].Longitude);
            Assert.AreEqual(-38.24343, polygon.Ring.Coordinates[3].Latitude);
            Assert.AreEqual(144.6813, polygon.Ring.Coordinates[3].Longitude);
            Assert.AreEqual(-38.24484, polygon.Ring.Coordinates[4].Latitude);
            Assert.AreEqual(144.6824, polygon.Ring.Coordinates[4].Longitude);
            Assert.AreEqual(-38.25356, polygon.Ring.Coordinates[5].Latitude);
            Assert.AreEqual(144.6826, polygon.Ring.Coordinates[5].Longitude);
            Assert.AreEqual(-38.2521, polygon.Ring.Coordinates[6].Latitude);
            Assert.AreEqual(144.6876, polygon.Ring.Coordinates[6].Longitude);
            Assert.AreEqual(-38.24719, polygon.Ring.Coordinates[7].Latitude);
            Assert.AreEqual(144.6919, polygon.Ring.Coordinates[7].Longitude);
            Assert.AreEqual(-38.24723, polygon.Ring.Coordinates[8].Latitude);
            Assert.AreEqual(144.7006, polygon.Ring.Coordinates[8].Longitude);
            Assert.AreEqual(-38.25078, polygon.Ring.Coordinates[9].Latitude);
            Assert.AreEqual(144.7042, polygon.Ring.Coordinates[9].Longitude);
            Assert.AreEqual(-38.26229, polygon.Ring.Coordinates[10].Latitude);
            Assert.AreEqual(144.6758, polygon.Ring.Coordinates[10].Longitude);
            Assert.AreEqual(-38.26255, polygon.Ring.Coordinates[11].Latitude);
            Assert.AreEqual(144.6693, polygon.Ring.Coordinates[11].Longitude);
        }

        /// <summary>
        /// Tests a polyfile with a hole.
        /// </summary>
        [Test]
        public void TestPolyOneHole()
        {
            var poly = "australia_v\n" +
                "1\n" +
                    " 0.1446763E+03 -0.3825659E+02\n" +
                    " 0.1446693E+03 -0.3826255E+02\n" +
                    " 0.1446627E+03 -0.3825661E+02\n" +
                    " 0.1446763E+03 -0.3824465E+02\n" +
                    " 0.1446813E+03 -0.3824343E+02\n" +
                    " 0.1446824E+03 -0.3824484E+02\n" +
                    " 0.1446826E+03 -0.3825356E+02\n" +
                    " 0.1446876E+03 -0.3825210E+02\n" +
                    " 0.1446919E+03 -0.3824719E+02\n" +
                    " 0.1447006E+03 -0.3824723E+02\n" +
                    " 0.1447042E+03 -0.3825078E+02\n" +
                    " 0.1446758E+03 -0.3826229E+02\n" +
                    " 0.1446693E+03 -0.3826255E+02\n" +
                "END\n" +
                "!2\n" +
                    " 0.1422483E+03 -0.3839481E+02\n" +
                    " 0.1422436E+03 -0.3839315E+02\n" +
                    " 0.1422496E+03 -0.3839070E+02\n" +
                    " 0.1422543E+03 -0.3839025E+02\n" +
                    " 0.1422574E+03 -0.3839155E+02\n" +
                    " 0.1422467E+03 -0.3840065E+02\n" +
                    " 0.1422433E+03 -0.3840048E+02\n" +
                    " 0.1422420E+03 -0.3839857E+02\n" +
                    " 0.1422436E+03 -0.3839315E+02\n" +
                "END\n" +
                "END";

            var feature = PolyFileConverter.ReadPolygon(poly);
            Assert.IsNotNull(feature);
            Assert.IsNotNull(feature.Attributes);
            Assert.IsInstanceOf<Polygon>(feature.Geometry);
            Assert.IsTrue(feature.Attributes.ContainsKey("name"));
            Assert.AreEqual("australia_v", feature.Attributes["name"]);

            var polygon = feature.Geometry as Polygon;
            Assert.IsNotNull(polygon.Ring);
            Assert.AreEqual(12, polygon.Ring.Coordinates.Count);
            Assert.AreEqual(-38.26255, polygon.Ring.Coordinates[0].Latitude);
            Assert.AreEqual(144.6693, polygon.Ring.Coordinates[0].Longitude);
            Assert.AreEqual(-38.25661, polygon.Ring.Coordinates[1].Latitude);
            Assert.AreEqual(144.6627, polygon.Ring.Coordinates[1].Longitude);
            Assert.AreEqual(-38.24465, polygon.Ring.Coordinates[2].Latitude);
            Assert.AreEqual(144.6763, polygon.Ring.Coordinates[2].Longitude);
            Assert.AreEqual(-38.24343, polygon.Ring.Coordinates[3].Latitude);
            Assert.AreEqual(144.6813, polygon.Ring.Coordinates[3].Longitude);
            Assert.AreEqual(-38.24484, polygon.Ring.Coordinates[4].Latitude);
            Assert.AreEqual(144.6824, polygon.Ring.Coordinates[4].Longitude);
            Assert.AreEqual(-38.25356, polygon.Ring.Coordinates[5].Latitude);
            Assert.AreEqual(144.6826, polygon.Ring.Coordinates[5].Longitude);
            Assert.AreEqual(-38.2521, polygon.Ring.Coordinates[6].Latitude);
            Assert.AreEqual(144.6876, polygon.Ring.Coordinates[6].Longitude);
            Assert.AreEqual(-38.24719, polygon.Ring.Coordinates[7].Latitude);
            Assert.AreEqual(144.6919, polygon.Ring.Coordinates[7].Longitude);
            Assert.AreEqual(-38.24723, polygon.Ring.Coordinates[8].Latitude);
            Assert.AreEqual(144.7006, polygon.Ring.Coordinates[8].Longitude);
            Assert.AreEqual(-38.25078, polygon.Ring.Coordinates[9].Latitude);
            Assert.AreEqual(144.7042, polygon.Ring.Coordinates[9].Longitude);
            Assert.AreEqual(-38.26229, polygon.Ring.Coordinates[10].Latitude);
            Assert.AreEqual(144.6758, polygon.Ring.Coordinates[10].Longitude);
            Assert.AreEqual(-38.26255, polygon.Ring.Coordinates[11].Latitude);
            Assert.AreEqual(144.6693, polygon.Ring.Coordinates[11].Longitude);

            Assert.IsNotNull(polygon.Holes);
            Assert.AreEqual(1, polygon.Holes.Count());

            var inner = polygon.Holes.First();
            Assert.AreEqual(8, inner.Coordinates.Count);
            Assert.AreEqual(-38.39315, inner.Coordinates[0].Latitude);
            Assert.AreEqual(142.2436, inner.Coordinates[0].Longitude);
            Assert.AreEqual(-38.3907, inner.Coordinates[1].Latitude);
            Assert.AreEqual(142.2496, inner.Coordinates[1].Longitude);
            Assert.AreEqual(-38.39025, inner.Coordinates[2].Latitude);
            Assert.AreEqual(142.2543, inner.Coordinates[2].Longitude);
            Assert.AreEqual(-38.39155, inner.Coordinates[3].Latitude);
            Assert.AreEqual(142.2574, inner.Coordinates[3].Longitude);
            Assert.AreEqual(-38.40065, inner.Coordinates[4].Latitude);
            Assert.AreEqual(142.2467, inner.Coordinates[4].Longitude);
            Assert.AreEqual(-38.40048, inner.Coordinates[5].Latitude);
            Assert.AreEqual(142.2433, inner.Coordinates[5].Longitude);
            Assert.AreEqual(-38.39857, inner.Coordinates[6].Latitude);
            Assert.AreEqual(142.242, inner.Coordinates[6].Longitude);
            Assert.AreEqual(-38.39315, inner.Coordinates[7].Latitude);
            Assert.AreEqual(142.2436, inner.Coordinates[7].Longitude);
        }
    }
}