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
using OsmSharp.Osm;
using OsmSharp.Osm.Streams;

namespace OsmSharp.Test.Osm.Streams.Filters
{
    /// <summary>
    /// Contains tests for the OsmStreamFilterBase.
    /// </summary>
    [TestFixture]
    public class OsmStreamFilterBaseTests
    {
        /// <summary>
        /// Tests simple to complete node conversion.
        /// </summary>
        [Test]
        public void TestSimpleToCompleteNode()
        {
            // execute
            List<OsmGeo> filtered = this.Filter(new OsmGeo[] {
                Node.Create(1, 0, 0),
                Node.Create(2, 1, 0),
                Node.Create(3, 0, 1) });

            // verify.
            Assert.IsNotNull(filtered);
            Assert.AreEqual(2, filtered.Count);
            Assert.IsTrue(filtered.Any(x => (x.Id == 1 &&
                (x as Node).Latitude == 0 &&
                (x as Node).Longitude == 0)));
            Assert.IsTrue(filtered.Any(x => (x.Id == 3 &&
                (x as Node).Latitude == 0 &&
                (x as Node).Longitude == 1)));
        }

        /// <summary>
        /// Tests simple to complete way conversion.
        /// </summary>
        [Test]
        public void TestSimpleToCompleteWay()
        {
            // execute
            List<OsmGeo> completeList = this.Filter(new OsmGeo[] {
                Node.Create(1, 0, 0),
                Node.Create(2, 1, 0),
                Node.Create(3, 0, 1),
                Way.Create(1, 1, 2, 3)});

            // verify.
            Assert.IsNotNull(completeList);
            Assert.AreEqual(4, completeList.Count);
            Assert.IsTrue(completeList.Any(x => (x.Id == 1 && (x is Node) &&
                (x as Node).Latitude == 0 &&
                (x as Node).Longitude == 0)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 2 && (x is Node) &&
                (x as Node).Latitude == 1 &&
                (x as Node).Longitude == 0)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 3 && (x is Node) &&
                (x as Node).Latitude == 0 &&
                (x as Node).Longitude == 1)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 1 && (x is Way) &&
                (x as Way).Nodes.Count == 3 &&
                (x as Way).Nodes[0] == 1 &&
                (x as Way).Nodes[1] == 2 &&
                (x as Way).Nodes[2] == 3)));
        }

        /// <summary>
        /// Tests simple to complete relation conversion.
        /// </summary>
        [Test]
        public void TestSimpleToCompleteRelation()
        {
            // execute
            List<OsmGeo> completeList = this.Filter(new OsmGeo[] {
                Node.Create(1, 0, 0),
                Node.Create(2, 1, 0),
                Node.Create(3, 0, 1),
                Way.Create(1, 1, 2, 3),
                Relation.Create(1, 
                    RelationMember.Create(1, "way", OsmGeoType.Way))});

            // verify.
            Assert.IsNotNull(completeList);
            Assert.AreEqual(5, completeList.Count);
            Assert.IsTrue(completeList.Any(x => (x.Id == 1 && (x is Node) &&
                (x as Node).Latitude == 0 &&
                (x as Node).Longitude == 0)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 2 && (x is Node) &&
                (x as Node).Latitude == 1 &&
                (x as Node).Longitude == 0)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 3 && (x is Node) &&
                (x as Node).Latitude == 0 &&
                (x as Node).Longitude == 1)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 1 && (x is Way) &&
                (x as Way).Nodes.Count == 3 &&
                (x as Way).Nodes[0] == 1 &&
                (x as Way).Nodes[1] == 2 &&
                (x as Way).Nodes[2] == 3)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 1 && (x is Relation) &&
                (x as Relation).Members.Count == 1)));
        }

        /// <summary>
        /// Tests simple to complete relation conversion.
        /// </summary>
        [Test]
        public void TestSimpleToCompleteRelationCascade()
        {
            // execute
            List<OsmGeo> completeList = this.Filter(new OsmGeo[] {
                Node.Create(1, 0, 0),
                Node.Create(2, 1, 0),
                Node.Create(3, 0, 1),
                Way.Create(2, 1, 2, 3),
                Relation.Create(1, 
                    RelationMember.Create(2, "way", OsmGeoType.Way))});

            // verify.
            Assert.IsNotNull(completeList);
            Assert.AreEqual(5, completeList.Count);
            Assert.IsTrue(completeList.Any(x => (x.Id == 1 && (x is Node) &&
                (x as Node).Latitude == 0 &&
                (x as Node).Longitude == 0)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 2 && (x is Node) &&
                (x as Node).Latitude == 1 &&
                (x as Node).Longitude == 0)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 3 && (x is Node) &&
                (x as Node).Latitude == 0 &&
                (x as Node).Longitude == 1)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 2 && (x is Way) &&
                (x as Way).Nodes.Count == 3 &&
                (x as Way).Nodes[0] == 1 &&
                (x as Way).Nodes[1] == 2 &&
                (x as Way).Nodes[2] == 3)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 1 && (x is Relation) &&
                (x as Relation).Members.Count == 1)));
        }


        /// <summary>
        /// Execute the simple-to-complete code.
        /// </summary>
        /// <param name="osmGeoList"></param>
        /// <returns></returns>
        private List<OsmGeo> Filter(IEnumerable<OsmGeo> osmGeoList)
        {
            OsmStreamFilter filter = new OsmStreamFilterReference();
            filter.RegisterSource(osmGeoList.ToOsmStreamSource());
            return new List<OsmGeo>(filter); // create the basic stream.
        }
    }
}
