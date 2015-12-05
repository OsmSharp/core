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
using OsmSharp.Osm;
using OsmSharp.Osm.Streams.Filters;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Test.Osm.Streams.Filters
{
    /// <summary>
    /// Tests the stream filter that excludes objects from other streams.
    /// </summary>
    [TestFixture]
    public class OsmStreamFilterExcludeTests
    {
        /// <summary>
        /// Tests the exlude filtering with one node.
        /// </summary>
        [Test]
        public void TestFilterOneNode()
        {
            // execute
            var filtered = this.Filter(
                    new OsmGeo[] {
                Node.Create(1, new TagsCollection(
                    Tag.Create("tag1", "value1")), 0, 0),
                Node.Create(2, new TagsCollection(
                    Tag.Create("tag2", "value2")), 1, 0),
                Node.Create(3, new TagsCollection(
                    Tag.Create("tag3", "value3")), 0, 1) }, 
                    new OsmGeo[] {
                Node.Create(1, new TagsCollection(
                    Tag.Create("tag1", "value1")), 0, 0) });

            // verify.
            Assert.IsNotNull(filtered);
            Assert.AreEqual(2, filtered.Count);
            Assert.IsFalse(filtered.Any(x => (x.Id == 1 &&
                x.Tags.Count == 1)));
            Assert.IsTrue(filtered.Any(x => (x.Id == 2 &&
                x.Tags.Count == 1)));
            Assert.IsTrue(filtered.Any(x => (x.Id == 3 &&
                x.Tags.Count == 1)));

            filtered = this.Filter(
                    new OsmGeo[] {
                Node.Create(1, new TagsCollection(
                    Tag.Create("tag1", "value1")), 0, 0),
                Node.Create(2, new TagsCollection(
                    Tag.Create("tag2", "value2")), 1, 0),
                Node.Create(3, new TagsCollection(
                    Tag.Create("tag3", "value3")), 0, 1) },
                    new OsmGeo[] {
                Node.Create(1, new TagsCollection(
                    Tag.Create("tag1", "value1")), 0, 0) },
                    false, true, true);

            // verify.
            Assert.IsNotNull(filtered);
            Assert.AreEqual(3, filtered.Count);
            Assert.IsTrue(filtered.Any(x => (x.Id == 1 &&
                x.Tags.Count == 1)));
            Assert.IsTrue(filtered.Any(x => (x.Id == 2 &&
                x.Tags.Count == 1)));
            Assert.IsTrue(filtered.Any(x => (x.Id == 3 &&
                x.Tags.Count == 1)));
        }
        
        /// <summary>
        /// Tests the exlude filtering with one way.
        /// </summary>
        [Test]
        public void TestFilterOneWay()
        {
            // execute
            var filtered = this.Filter(
                    new OsmGeo[] {
                Way.Create(1, new TagsCollection(
                    Tag.Create("tag1", "value1")), 0, 0),
                Way.Create(2, new TagsCollection(
                    Tag.Create("tag2", "value2")), 1, 0),
                Way.Create(3, new TagsCollection(
                    Tag.Create("tag3", "value3")), 0, 1) },
                    new OsmGeo[] {
                Way.Create(1, new TagsCollection(
                    Tag.Create("tag1", "value1")), 0, 0) });

            // verify.
            Assert.IsNotNull(filtered);
            Assert.AreEqual(2, filtered.Count);
            Assert.IsFalse(filtered.Any(x => (x.Id == 1 &&
                x.Tags.Count == 1)));
            Assert.IsTrue(filtered.Any(x => (x.Id == 2 &&
                x.Tags.Count == 1)));
            Assert.IsTrue(filtered.Any(x => (x.Id == 3 &&
                x.Tags.Count == 1)));

            filtered = this.Filter(
                    new OsmGeo[] {
                Way.Create(1, new TagsCollection(
                    Tag.Create("tag1", "value1")), 0, 0),
                Way.Create(2, new TagsCollection(
                    Tag.Create("tag2", "value2")), 1, 0),
                Way.Create(3, new TagsCollection(
                    Tag.Create("tag3", "value3")), 0, 1) },
                    new OsmGeo[] {
                Way.Create(1, new TagsCollection(
                    Tag.Create("tag1", "value1")), 0, 0) },
                    true, false, true);

            // verify.
            Assert.IsNotNull(filtered);
            Assert.AreEqual(3, filtered.Count);
            Assert.IsTrue(filtered.Any(x => (x.Id == 1 &&
                x.Tags.Count == 1)));
            Assert.IsTrue(filtered.Any(x => (x.Id == 2 &&
                x.Tags.Count == 1)));
            Assert.IsTrue(filtered.Any(x => (x.Id == 3 &&
                x.Tags.Count == 1)));
        }
        /// <summary>
        /// Tests the exlude filtering with one relation
        /// </summary>
        [Test]
        public void TestFilterOneRelation()
        {
            // execute
            var filtered = this.Filter(
                    new OsmGeo[] {
                Relation.Create(1, new TagsCollection(
                    Tag.Create("tag1", "value1"))),
                Relation.Create(2, new TagsCollection(
                    Tag.Create("tag2", "value2"))),
                Relation.Create(3, new TagsCollection(
                    Tag.Create("tag3", "value3"))) },
                    new OsmGeo[] {
                Relation.Create(1, new TagsCollection(
                    Tag.Create("tag1", "value1"))) });

            // verify.
            Assert.IsNotNull(filtered);
            Assert.AreEqual(2, filtered.Count);
            Assert.IsFalse(filtered.Any(x => (x.Id == 1 &&
                x.Tags.Count == 1)));
            Assert.IsTrue(filtered.Any(x => (x.Id == 2 &&
                x.Tags.Count == 1)));
            Assert.IsTrue(filtered.Any(x => (x.Id == 3 &&
                x.Tags.Count == 1)));

            filtered = this.Filter(
                    new OsmGeo[] {
                Relation.Create(1, new TagsCollection(
                    Tag.Create("tag1", "value1"))),
                Relation.Create(2, new TagsCollection(
                    Tag.Create("tag2", "value2"))),
                Relation.Create(3, new TagsCollection(
                    Tag.Create("tag3", "value3"))) },
                    new OsmGeo[] {
                Relation.Create(1, new TagsCollection(
                    Tag.Create("tag1", "value1"))) },
                    true, true, false);

            // verify.
            Assert.IsNotNull(filtered);
            Assert.AreEqual(3, filtered.Count);
            Assert.IsTrue(filtered.Any(x => (x.Id == 1 &&
                x.Tags.Count == 1)));
            Assert.IsTrue(filtered.Any(x => (x.Id == 2 &&
                x.Tags.Count == 1)));
            Assert.IsTrue(filtered.Any(x => (x.Id == 3 &&
                x.Tags.Count == 1)));
        }

        /// <summary>
        /// Does the filtering.
        /// </summary>
        /// <param name="osmGeos"></param>
        /// <param name="toExclude"></param>
        private List<OsmGeo> Filter(OsmGeo[] osmGeos, OsmGeo[] toExclude)
        {
            return this.Filter(osmGeos, toExclude, true, true, true);
        }

        /// <summary>
        /// Does the filtering.
        /// </summary>
        /// <param name="osmGeos"></param>
        /// <param name="toExclude"></param>
        /// <param name="excludeNodes"></param>
        /// <param name="excludeWays"></param>
        /// <param name="excludeRelations"></param>
        private List<OsmGeo> Filter(OsmGeo[] osmGeos, OsmGeo[] toExclude, bool excludeNodes, bool excludeWays, bool excludeRelations)
        {
            var excludeFilter = new OsmStreamFilterExclude(excludeNodes, excludeWays, excludeRelations);
            excludeFilter.RegisterSource(osmGeos);
            excludeFilter.RegisterSource(toExclude);
            return new List<OsmGeo>(excludeFilter);
        }
    }
}