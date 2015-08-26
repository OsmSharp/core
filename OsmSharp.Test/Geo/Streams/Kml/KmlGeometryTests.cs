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

using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using OsmSharp.Geo.Geometries;
using OsmSharp.Geo.Streams.Kml;
using OsmSharp.Geo.Features;

namespace OsmSharp.Test.Streams.IO.Kml
{
    /// <summary>
    /// Contains test to read/write Kml files using geometry streams.
    /// </summary>
    [TestFixture]
    public class GpxGeometryTests
    {
        /// <summary>
        /// Test reads an embedded Kml files and converts it to geometries.
        /// </summary>
        [Test]
        public void KmlReadGeometryv2_0()
        {
            // initialize the geometry source.
            var kmlSource = new KmlFeatureStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Test.data.test.v2.0.kml"));

            // pull all the objects from the stream into the given collection.
            var kmlCollection = new FeatureCollection(kmlSource);
            var features = new List<Feature>(kmlCollection);

            // test collection contents.
            Assert.AreEqual(1, features.Count);
            Assert.IsInstanceOf(typeof(Point), features[0].Geometry);
        }
        
        /// <summary>
        /// Test reads an embedded Kml files and converts it to geometries.
        /// </summary>
        [Test]
        public void KmlReadGeometryv2_0_response()
        {
            // initialize the geometry source.
            var kmlSource = new KmlFeatureStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Test.data.test.v2.0.response.kml"));

            // pull all the objects from the stream into the given collection.
            var kmlCollection = new FeatureCollection(kmlSource);
            var geometries = new List<Feature>(kmlCollection);

            // test collection contents.
            Assert.AreEqual(7, geometries.Count);
            Assert.IsInstanceOf(typeof(Point), geometries[0].Geometry);
            Assert.IsInstanceOf(typeof(Point), geometries[1].Geometry);
            Assert.IsInstanceOf(typeof(Point), geometries[2].Geometry);
            Assert.IsInstanceOf(typeof(Point), geometries[3].Geometry);
            Assert.IsInstanceOf(typeof(Point), geometries[4].Geometry);
            Assert.IsInstanceOf(typeof(Point), geometries[5].Geometry);
            Assert.IsInstanceOf(typeof(Point), geometries[6].Geometry);
        }

        /// <summary>
        /// Test reads an embedded Kml files and converts it to geometries.
        /// </summary>
        [Test]
        public void KmlReadGeometryv2_1()
        {
            // initialize the geometry source.
            var kmlSource = new KmlFeatureStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Test.data.test.v2.1.kml"));

            // pull all the objects from the stream into the given collection.
            var kmlCollection = new FeatureCollection(kmlSource);
            var features = new List<Feature>(kmlCollection);

            // test collection contents.
            Assert.AreEqual(23, features.Count);
            Assert.IsInstanceOf(typeof(LineString), features[0].Geometry);
            Assert.IsInstanceOf(typeof(LineString), features[1].Geometry);
            Assert.IsInstanceOf(typeof(LineString), features[2].Geometry);
            Assert.IsInstanceOf(typeof(LineString), features[3].Geometry);
            Assert.IsInstanceOf(typeof(LineString), features[4].Geometry);
            Assert.IsInstanceOf(typeof(LineString), features[5].Geometry);
            Assert.IsInstanceOf(typeof(LineString), features[6].Geometry);
            Assert.IsInstanceOf(typeof(LineString), features[7].Geometry);
            Assert.IsInstanceOf(typeof(LineString), features[8].Geometry);
            Assert.IsInstanceOf(typeof(LineString), features[9].Geometry);
            Assert.IsInstanceOf(typeof(LineString), features[10].Geometry);
            Assert.IsInstanceOf(typeof(LineString), features[11].Geometry);
            Assert.IsInstanceOf(typeof(LineString), features[12].Geometry);
            Assert.IsInstanceOf(typeof(LineString), features[13].Geometry);
            Assert.IsInstanceOf(typeof(LineString), features[14].Geometry);
            Assert.IsInstanceOf(typeof(LineString), features[15].Geometry);
            Assert.IsInstanceOf(typeof(LineString), features[16].Geometry);
            Assert.IsInstanceOf(typeof(LineString), features[17].Geometry);
            Assert.IsInstanceOf(typeof(LineString), features[18].Geometry);
            Assert.IsInstanceOf(typeof(LineString), features[19].Geometry);
            Assert.IsInstanceOf(typeof(LineString), features[20].Geometry);
            Assert.IsInstanceOf(typeof(LineString), features[21].Geometry);
            Assert.IsInstanceOf(typeof(LineString), features[22].Geometry);
        }
    }
}
