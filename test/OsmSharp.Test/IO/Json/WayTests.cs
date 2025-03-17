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
            
            Assert.That(w.Id, Is.EqualTo(null));
        }

        [Test]
        public void Way_FromJson_CompleteWay_ShouldReturnCompleteWay()
        {
            var w = JsonSerializer.Deserialize<Way>("{\"type\":\"way\",\"id\":41494454,\"timestamp\":\"2013-06-22T15:17:51Z\"," +
                                                    "\"version\":3,\"changeset\":16657760,\"user\":\"joakimfors\",\"uid\":306096," +
                                                    "\"nodes\":[507916537,507920041]," +
                                                    "\"tags\":{\"highway\":\"residential\",\"name\":\"Husargatan\"}}");
            
            Assert.That(w.Id, Is.EqualTo(41494454));
            Assert.That(w.TimeStamp, Is.EqualTo(new DateTime(2013,06,22,15,17,51, DateTimeKind.Utc)));
            Assert.That(w.Version, Is.EqualTo(3));
            Assert.That(w.ChangeSetId, Is.EqualTo(16657760));
            Assert.That(w.UserName, Is.EqualTo("joakimfors"));
            Assert.That(w.UserId, Is.EqualTo(306096));
            Assert.That(w.Tags, Is.Not.Null);
            Assert.That(w.Tags.Count, Is.EqualTo(2));
            Assert.That(w.Tags.ToArray()[0].Key, Is.EqualTo("highway"));
            Assert.That(w.Tags.ToArray()[0].Value, Is.EqualTo("residential"));
            Assert.That(w.Tags.ToArray()[1].Key, Is.EqualTo("name"));
            Assert.That(w.Tags.ToArray()[1].Value, Is.EqualTo("Husargatan"));
            Assert.That(w.Nodes, Is.EqualTo(new[] {507916537,507920041}));
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
            Assert.That(serialized, Is.EqualTo("{\"type\":\"way\",\"nodes\":[1,2,3],\"id\":1,\"tags\":{\"amenity\":\"something\",\"key\":\"some_value\"},\"timestamp\":\"2008-09-12T21:37:45\",\"version\":1,\"user\":\"ben\",\"uid\":1}"));
        }
    }
}