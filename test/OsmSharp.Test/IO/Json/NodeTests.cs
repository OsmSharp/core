using System;
using System.Linq;
using System.Text.Json;
using NUnit.Framework;
using OsmSharp.Tags;

namespace OsmSharp.Test.IO.Json
{
    [TestFixture]
    public class NodeTests
    {
        [Test]
        public void Node_ToJson_CompleteNode_ShouldReturnCompleteJson()
        {
            var n = new Node
            {
                Id = 100000,
                Latitude = 58.4215544,
                Longitude = 15.6182983,
                TimeStamp = new DateTime(2017, 04, 17, 18, 31, 21, DateTimeKind.Utc),
                Version = 3,
                ChangeSetId = 47881103,
                UserName = "riiga",
                UserId = 83501,
                Tags = new TagsCollection(new Tag("highway", "residential"))
            };

            var serialized = JsonSerializer.Serialize(n);
            Assert.That(serialized, Is.EqualTo("{\"type\":\"node\",\"lat\":58.4215544,\"lon\":15.6182983,\"id\":100000,\"tags\":{\"highway\":\"residential\"},\"timestamp\":\"2017-04-17T18:31:21Z\",\"version\":3,\"changeset\":47881103,\"user\":\"riiga\",\"uid\":83501}"));
        }
        
        [Test]
        public void Node_FromJson_Empty_ShouldReturnEmptyNode()
        {
            var n = JsonSerializer.Deserialize<Node>("{\"type\":\"node\"}");
            
            Assert.That(n.Id, Is.EqualTo(null));
        }
        
        [Test]
        public void Node_FromJson_IdOnly_ShouldSetId()
        {
            var n = JsonSerializer.Deserialize<Node>("{\"type\":\"node\",\"id\": 15441}");
            
            Assert.That(n.Id, Is.EqualTo(15441));
        }

        [Test] public void Node_FromJson_CompleteNode_ShouldSetAll()
        {
            var n = JsonSerializer.Deserialize<Node>("{\"type\":\"node\",\"id\":100000,\"lat\":58.4215544,\"lon\":15.6182983,\"tags\":{\"highway\": \"residential\"},\"timestamp\":\"2017-04-17T18:31:21Z\",\"version\":3,\"changeset\":47881103,\"user\":\"riiga\",\"uid\":83501}");
            
            Assert.That(n.Id, Is.EqualTo(100000));
            Assert.That(n.Latitude, Is.EqualTo(58.4215544));
            Assert.That(n.Longitude, Is.EqualTo(15.6182983));
            Assert.That(n.TimeStamp, Is.EqualTo(new DateTime(2017,04,17,18,31,21, DateTimeKind.Utc)));
            Assert.That(n.Version, Is.EqualTo(3));
            Assert.That(n.ChangeSetId, Is.EqualTo(47881103));
            Assert.That(n.UserName, Is.EqualTo("riiga"));
            Assert.That(n.UserId, Is.EqualTo(83501));
            Assert.That(n.Tags, Is.Not.Null);
            Assert.That(n.Tags.Count, Is.EqualTo(1));
            Assert.That(n.Tags.First().Key, Is.EqualTo("highway"));
            Assert.That(n.Tags.First().Value, Is.EqualTo("residential"));
        }
    }
}