// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2016 Abelshausen Ben
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
using System.Linq;
using OsmSharp.Osm;
using OsmSharp.Osm.Streams.Filters;
using OsmSharp.Collections.Tags;

namespace OsmSharp.Test.Osm.Streams.Filters
{
    /// <summary>
    /// Holds tests for the tags filter.
    /// </summary>
    [TestFixture]
    class OsmStreamFilterTagsFilterTest
    {
        /// <summary>
        /// Tests the tags filtering without any filtering.
        /// </summary>
        [Test]
        public void TestFilterTagsFilterNoFiltering()
        {
            // execute
            var filtered = this.Filter(new OsmGeo[] {
                new Node(1, 0, 0, new TagsCollection(
                    Tag.Create("tag1", "value1"))),
                new Node(2, 1, 0, new TagsCollection(
                    Tag.Create("tag2", "value2"))),
                new Node(3, 0, 1, new TagsCollection(
                    Tag.Create("tag3", "value3"))) },
                ((TagsCollectionBase tags) => {}));

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
        /// Tests the tags filtering with simple filtering.
        /// </summary>
        [Test]
        public void TestFilterTagsFilterFiltering()
        {
            // execute
            var filtered = this.Filter(new OsmGeo[] {
                new Node(1, 0, 0, new TagsCollection(
                    Tag.Create("tag1", "value1"))),
                new Node(2, 1, 0, new TagsCollection(
                    Tag.Create("tag2", "value2"))),
                new Node(3, 0, 1, new TagsCollection(
                    Tag.Create("tag3", "value3"))) },
                ((TagsCollectionBase tags) => {
                    tags.RemoveKey("tag2");
                }));

            // verify.
            Assert.IsNotNull(filtered);
            Assert.AreEqual(3, filtered.Count);
            Assert.IsTrue(filtered.Any(x => (x.Id == 1 &&
                x.Tags.Count == 1)));
            Assert.IsTrue(filtered.Any(x => (x.Id == 2 &&
                x.Tags.Count == 0)));
            Assert.IsTrue(filtered.Any(x => (x.Id == 3 &&
                x.Tags.Count == 1)));
        }

        /// <summary>
        /// Does the filtering.
        /// </summary>
        private List<OsmGeo> Filter(OsmGeo[] osmGeos,
            OsmStreamFilterTagsFilter.TagsFilterDelegate filter)
        {
            var tagsFilter = new OsmStreamFilterTagsFilter(filter);
            tagsFilter.RegisterSource(osmGeos);
            return new List<OsmGeo>(tagsFilter);
        }
    }
}