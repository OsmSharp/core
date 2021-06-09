using System;
using System.Linq;
using System.Text.Json;
using NUnit.Framework;
using OsmSharp.Tags;

namespace OsmSharp.Test.IO.Json
{
    [TestFixture]
    public class WayTests
    {
        [Test]
        public void Way_FromJson_Empty_ShouldReturnEmptyWay()
        {
            var w = JsonSerializer.Deserialize<Way>("{\"type\":\"way\"}");
            
            Assert.AreEqual(null, w.Id);
        }

        [Test]
        public void Way_FromJson_CompleteWay_ShouldReturnCompleteWay()
        {
            var w = JsonSerializer.Deserialize<Way>("{\"type\":\"way\",\"id\":41494454,\"timestamp\":\"2013-06-22T15:17:51Z\"," +
                                                    "\"version\":3,\"changeset\":16657760,\"user\":\"joakimfors\",\"uid\":306096," +
                                                    "\"nodes\":[507916537,507920041]," +
                                                    "\"tags\":{\"highway\":\"residential\",\"name\":\"Husargatan\"}}");
            
            Assert.AreEqual(41494454, w.Id);
            Assert.AreEqual(new DateTime(2013,06,22,15,17,51, DateTimeKind.Utc), w.TimeStamp);
            Assert.AreEqual(3, w.Version);
            Assert.AreEqual(16657760, w.ChangeSetId);
            Assert.AreEqual("joakimfors", w.UserName);
            Assert.AreEqual(306096, w.UserId);
            Assert.NotNull(w.Tags);
            Assert.AreEqual(2, w.Tags.Count);
            Assert.AreEqual("highway", w.Tags.ToArray()[0].Key);
            Assert.AreEqual("residential", w.Tags.ToArray()[0].Value);
            Assert.AreEqual("name", w.Tags.ToArray()[1].Key);
            Assert.AreEqual("Husargatan", w.Tags.ToArray()[1].Value);
            Assert.AreEqual(new[] {507916537,507920041}, w.Nodes);
        }

        [Test]
        public void Way_ToJson_CompleteWay_ShouldReturnCompleteJson()
        {
            var w = new Way()
            {
                Id = 1,
                Version = 1,
                UserName = "ben",
                UserId = 1,
                TimeStamp = new System.DateTime(2008, 09, 12, 21, 37, 45),
                Tags = new TagsCollection(
                    new Tag("amenity", "something"),
                    new Tag("key", "some_value")),
                Nodes = new long[]
                {
                    1, 2, 3
                }
            };

            var serialized = JsonSerializer.Serialize(w);
            Assert.AreEqual("{\"type\":\"way\",\"nodes\":[1,2,3],\"id\":1,\"tags\":{\"amenity\":\"something\",\"key\":\"some_value\"},\"timestamp\":\"2008-09-12T21:37:45\",\"version\":1,\"user\":\"ben\",\"uid\":1}", serialized);
        }
    }
}