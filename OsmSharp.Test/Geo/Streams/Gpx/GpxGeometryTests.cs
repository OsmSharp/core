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
using OsmSharp.Geo.Streams.Gpx;
using OsmSharp.Geo.Features;

namespace OsmSharp.Test.Geo.Streams.Gpx
{
    /// <summary>
    /// Contains test to read/write gpx files using geometry streams.
    /// </summary>
    [TestFixture]
    public class GpxGeometryTests
    {
        /// <summary>
        /// Test reads an embedded gpx files and converts it to geometries.
        /// </summary>
        [Test]
        public void GpxReadGeometryv1_0()
        {
            // initialize the geometry source.
            var gpxSource = new GpxFeatureStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Test.data.test.v1.0.gpx"),
                false);

            // pull all the objects from the stream into the given collection.
            var gpxCollection = new FeatureCollection(gpxSource);
            var features = new List<Feature>(gpxCollection);

            // test collection contents.
            Assert.AreEqual(1, features.Count);
            Assert.IsInstanceOf(typeof(LineString), features[0].Geometry);
            Assert.AreEqual(424, (features[0].Geometry as LineString).Coordinates.Count);
        }
    }
}
