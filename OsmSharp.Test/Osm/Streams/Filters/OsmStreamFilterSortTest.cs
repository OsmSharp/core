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
using NUnit.Framework;
using OsmSharp.Collections.Tags;
using OsmSharp.Osm;
using OsmSharp.Osm.Streams.Filters;

namespace OsmSharp.Test.Osm.Streams.Filters
{
    /// <summary>
    /// Contains tests for the OsmStreamFilterSort.
    /// </summary>
    [TestFixture]
    public class OsmStreamFilterSortTests
    {
        /// <summary>
        /// Tests simple to complete node conversion.
        /// </summary>
        [Test]
        public void TestFilterSort()
        {
            // execute
            var filter = new OsmStreamFilterSort();
            filter.RegisterSource(new OsmGeo[] {
                Node.Create(1, 0, 0),
                Node.Create(2, 1, 0),
                Node.Create(3, 0, 1),
                Way.Create(1, 1, 2, 3)});
            var list = new List<OsmGeo>(
                filter);

            // verify.
            Assert.IsNotNull(list);
            Assert.AreEqual(4, list.Count);
            Assert.AreEqual(1, list[0].Id);
            Assert.AreEqual(2, list[1].Id);
            Assert.AreEqual(3, list[2].Id);
            Assert.AreEqual(1, list[3].Id);

            // reset.
            filter.Reset();
            list = new List<OsmGeo>(
                filter);

            // verify.
            Assert.IsNotNull(list);
            Assert.AreEqual(4, list.Count);
            Assert.AreEqual(1, list[0].Id);
            Assert.AreEqual(2, list[1].Id);
            Assert.AreEqual(3, list[2].Id);
            Assert.AreEqual(1, list[3].Id);

            // execute
            filter = new OsmStreamFilterSort();
            filter.RegisterSource(new OsmGeo[] {
                Node.Create(1, 0, 0),
                Node.Create(2, 1, 0),
                Way.Create(1, 1, 2, 3),
                Node.Create(3, 0, 1)});
            list = new List<OsmGeo>(
                filter);

            // verify.
            Assert.IsNotNull(list);
            Assert.AreEqual(4, list.Count);
            Assert.AreEqual(1, list[0].Id);
            Assert.AreEqual(2, list[1].Id);
            Assert.AreEqual(3, list[2].Id);
            Assert.AreEqual(1, list[3].Id);

            // execute
            filter = new OsmStreamFilterSort();
            filter.RegisterSource(new OsmGeo[] {
                Node.Create(1, 0, 0),
                Node.Create(2, 1, 0),
                Way.Create(1, 1, 2, 3),
                Relation.Create(1, 
                    new TagsCollection(
                        Tag.Create("type", "multipolygon")),
                    RelationMember.Create(1, "outer", OsmGeoType.Way)),
                Node.Create(3, 0, 1)});
            list = new List<OsmGeo>(
                filter);

            // verify.
            Assert.IsNotNull(list);
            Assert.AreEqual(5, list.Count);
            Assert.AreEqual(1, list[0].Id);
            Assert.AreEqual(2, list[1].Id);
            Assert.AreEqual(3, list[2].Id);
            Assert.AreEqual(1, list[3].Id);
            Assert.AreEqual(1, list[4].Id);

            // reset.
            filter.Reset();
            list = new List<OsmGeo>(
                filter);

            // verify.
            Assert.IsNotNull(list);
            Assert.AreEqual(5, list.Count);
            Assert.AreEqual(1, list[0].Id);
            Assert.AreEqual(2, list[1].Id);
            Assert.AreEqual(3, list[2].Id);
            Assert.AreEqual(1, list[3].Id);
            Assert.AreEqual(1, list[4].Id);
        }
    }
}