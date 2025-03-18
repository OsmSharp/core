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

            Assert.That(JsonSerializer.Serialize(tagsCollection), Is.EqualTo("{}"));
        }
        
        [Test]
        public void TagsCollection_ToJson_OneTag_ShouldReturnJsonWithOneTag()
        {
            var tagsCollection = new TagsCollection();
            tagsCollection.AddOrReplace("highway", "residential");

            Assert.That(JsonSerializer.Serialize(tagsCollection), Is.EqualTo("{\"highway\":\"residential\"}"));
        }
        
        [Test]
        public void TagsCollection_FromJson_Empty_ShouldReturnEmpty()
        {
            var tagsCollection = JsonSerializer.Deserialize<TagsCollectionBase>("{}");

            Assert.That(tagsCollection.Count, Is.EqualTo(0));
        }
        
        [Test]
        public void TagsCollection_FromJson_OneTag_ShouldReturnOneTag()
        {
            var tagsCollection = JsonSerializer.Deserialize<TagsCollectionBase>("{\"highway\": \"residential\"}");

            Assert.That(tagsCollection.Count, Is.EqualTo(1));
            Assert.That(tagsCollection.First().Key, Is.EqualTo("highway"));
            Assert.That(tagsCollection.First().Value, Is.EqualTo("residential"));
        }
    }
}