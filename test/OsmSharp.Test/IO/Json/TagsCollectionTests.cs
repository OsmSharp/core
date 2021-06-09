using System.Linq;
using System.Text.Json;
using NUnit.Framework;
using OsmSharp.Tags;

namespace OsmSharp.Test.IO.Json
{
    [TestFixture]
    public class TagsCollectionTests
    {
        [Test]
        public void TagsCollection_ToJson_Empty_ShouldReturnEmptyJson()
        {
            var tagsCollection = new TagsCollection();

            Assert.AreEqual("{}", JsonSerializer.Serialize(tagsCollection));
        }
        
        [Test]
        public void TagsCollection_FromJson_Empty_ShouldReturnEmpty()
        {
            var tagsCollection = JsonSerializer.Deserialize<TagsCollectionBase>("{}");

            Assert.AreEqual(0, tagsCollection.Count);
        }
        
        [Test]
        public void TagsCollection_FromJson_OneTag_ShouldReturnOneTag()
        {
            var tagsCollection = JsonSerializer.Deserialize<TagsCollectionBase>("{\"highway\": \"residential\"}");

            Assert.AreEqual(1, tagsCollection.Count);
            Assert.AreEqual("highway", tagsCollection.First().Key);
            Assert.AreEqual("residential", tagsCollection.First().Value);
        }
    }
}