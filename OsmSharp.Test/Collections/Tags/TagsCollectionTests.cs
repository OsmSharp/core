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

namespace OsmSharp.Test.Collections.Tags
{
    /// <summary>
    /// Contains tests for tags collections.
    /// </summary>
    [TestFixture]
    public class TagsCollectionTests
    {
        /// <summary>
        /// Tests a simple tags collection.
        /// </summary>
        [Test]
        public void TestSimpleTagsCollectionEmpty()
        {
            var collection = new TagsCollection();

            Assert.AreEqual(0, collection.Count);
        }

        /// <summary>
        /// Tests a simple tags collection.
        /// </summary>
        [Test]
        public void TestSimpleTagsCollectionSimple()
        {
            var collection = new TagsCollection();

            collection["simple"] = "yes";

            Assert.IsTrue(collection.ContainsKey("simple"));
            Assert.IsTrue(collection.ContainsKeyValue("simple", "yes"));
            Assert.AreEqual("yes", collection["simple"]);
            Assert.AreEqual(1, collection.Count);
        }
    }
}