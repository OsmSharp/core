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
using OsmSharp.Collections.Tags.Index;
using OsmSharp.IO.MemoryMappedFiles;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Test.Unittests.Collections.Tags
{
    /// <summary>
    /// Contains tests for the SimpleTagsCollectionIndexTests.
    /// </summary>
    [TestFixture]
    public class TagsCollectionIndexTests
    {
        /// <summary>
        /// Tests the tags collection index.
        /// </summary>
        [Test]
        public void TestTagsIndex()
        {
            this.TestTagIndex(new TagsIndex());
        }

        /// <summary>
        /// Tests the tags collection index but memory mapped.
        /// </summary>
        [Test]
        public void TestTagsIndexMemoryMapped()
        {
            this.TestTagIndex(new TagsIndex(new MemoryMappedStream(new MemoryStream())));
        }

        /// <summary>
        /// Tests a simple tag serialization.
        /// </summary>
        [Test]
        public void TestTagIndexSerializaton()
        {
            // set the seed manually.
            OsmSharp.Math.Random.StaticRandomGenerator.Set(116542346);

            // build a tags index and keep what was added.
            var tagsIndex = new TagsIndex(new MemoryMappedStream(new MemoryStream()));
            var addedTags = new List<KeyValuePair<uint, TagsCollectionBase>>();
            for (int i = 0; i < 100; i++)
            {
                var tagsCollection = new TagsCollection();
                var tagCollectionSize = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(3) + 1;
                for (int idx = 0; idx < tagCollectionSize; idx++)
                {
                    var tagValue = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(3);
                    tagsCollection.Add(
                        string.Format("key_{0}", tagValue),
                        string.Format("value_{0}", tagValue));
                }
                var addCount = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(2) + 1;
                for (int idx = 0; idx < addCount; idx++)
                {
                    var tagsId = tagsIndex.Add(tagsCollection);
                    addedTags.Add(new KeyValuePair<uint, TagsCollectionBase>(tagsId, tagsCollection));

                    var indexTags = tagsIndex.Get(tagsId);
                    Assert.AreEqual(tagsCollection.Count, indexTags.Count);
                    foreach (var tag in tagsCollection)
                    {
                        Assert.IsTrue(indexTags.ContainsKeyValue(tag.Key, tag.Value));
                    }
                }
            }

            // serialize/deserialize.
            var deserializedTagsIndex = this.SerializeDeserialize(tagsIndex);

            // verify if what was added is still there.
            this.TestTagIndexContent(deserializedTagsIndex, addedTags);
        }

        /// <summary>
        /// Tests the given tags collection index.
        /// </summary>
        /// <param name="tagsCollectionIndex"></param>
        protected void TestTagIndex(ITagsIndex tagsCollectionIndex)
        {
            // set the seed manually.
            OsmSharp.Math.Random.StaticRandomGenerator.Set(116542346);

            var addedTags = new List<KeyValuePair<uint, TagsCollectionBase>>();
            for (int i = 0; i < 100; i++)
            {
                var tagsCollection = new TagsCollection();
                var tagCollectionSize = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(3) + 1;
                for (int idx = 0; idx < tagCollectionSize; idx++)
                {
                    var tagValue = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(3);
                    tagsCollection.Add(
                        string.Format("key_{0}", tagValue),
                        string.Format("value_{0}", tagValue));
                }
                var addCount = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(2) + 1;
                for (int idx = 0; idx < addCount; idx++)
                {
                    var tagsId = tagsCollectionIndex.Add(tagsCollection);
                    addedTags.Add(new KeyValuePair<uint, TagsCollectionBase>(tagsId, tagsCollection));

                    var indexTags = tagsCollectionIndex.Get(tagsId);
                    Assert.AreEqual(tagsCollection.Count, indexTags.Count);
                    foreach (var tag in tagsCollection)
                    {
                        Assert.IsTrue(indexTags.ContainsKeyValue(tag.Key, tag.Value));
                    }
                }
            }

            // test complete content.
            this.TestTagIndexContent(tagsCollectionIndex, addedTags);
        }

        /// <summary>
        /// Verifies the content of the given tags index agains the expected content list.
        /// </summary>
        /// <param name="tagsCollectionIndex"></param>
        /// <param name="expectedContent"></param>
        private void TestTagIndexContent(ITagsIndex tagsCollectionIndex, List<KeyValuePair<uint, TagsCollectionBase>> expectedContent)
        {
            // check the index.
            foreach (var pair in expectedContent)
            {
                var indexTags = tagsCollectionIndex.Get(pair.Key);
                Assert.AreEqual(pair.Value.Count, indexTags.Count);
                foreach (var tag in pair.Value)
                {
                    Assert.IsTrue(indexTags.ContainsKeyValue(tag.Key, tag.Value));
                }
                foreach (var tag in indexTags)
                {
                    Assert.IsTrue(pair.Value.ContainsKeyValue(tag.Key, tag.Value));
                }
            }
        }

        /// <summary>
        /// Serialize/deserialize index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private TagsIndex SerializeDeserialize(TagsIndex index)
        {
            var stream = new MemoryStream();
            index.Serialize(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return TagsIndex.Deserialize(stream);
        }
    }
}