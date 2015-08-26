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
using OsmSharp.Collections.Tags;
using OsmSharp.Geo.Geometries;
using OsmSharp.Osm;
using OsmSharp.Osm.Streams;
using OsmSharp.Osm.Data.Memory;
using OsmSharp.Osm.Geo.Interpreter;
using System.Collections.Generic;
using System.Linq;
using OsmSharp.Osm.Geo.Streams;
using OsmSharp.Geo.Features;

namespace OsmSharp.Test.Osm.Geo.Interpreter
{
    /// <summary>
    /// Contains tests for the default feature interpreter class testing as many of the openstreetmap tags ->  geometry logic as possible.
    /// </summary>
    [TestFixture]
    public class OsmFeatureStreamSourceTests
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
            way.Nodes = new List<long>();
            way.Nodes.Add(1);
            way.Nodes.Add(2);
            way.Nodes.Add(3);
            way.Nodes.Add(1);
            way.Tags = new TagsCollection();
            way.Tags.Add("area", "yes");

            var source = new List<OsmGeo>();
            source.Add(node1);
            source.Add(node2);
            source.Add(node3);
            source.Add(way);

            // create source stream.
            var sourceStream = new OsmSharp.Osm.Streams.Complete.OsmSimpleCompleteStreamSource(source.ToOsmStreamSource());

            // create features source.
            var featuresSourceStream = new OsmFeatureStreamSource(sourceStream);

            // pull stream.
            var features = new List<Feature>(featuresSourceStream);

            Assert.IsNotNull(features);
            Assert.AreEqual(1, features.Count);
            var feature = features[0];
            Assert.IsInstanceOf<LineairRing>(feature.Geometry);
            Assert.IsTrue(feature.Attributes.ContainsKeyValue("area", "yes"));
        }
    }
}