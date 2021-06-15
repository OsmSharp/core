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
            Assert.AreEqual("{\"type\":\"node\",\"lat\":58.4215544,\"lon\":15.6182983,\"id\":100000,\"tags\":{\"highway\":\"residential\"},\"timestamp\":\"2017-04-17T18:31:21Z\",\"version\":3,\"changeset\":47881103,\"user\":\"riiga\",\"uid\":83501}",
                serialized);
        }
        
        [Test]
        public void Node_FromJson_Empty_ShouldReturnEmptyNode()
        {
            var n = JsonSerializer.Deserialize<Node>("{\"type\":\"node\"}");
            
            Assert.AreEqual(null, n.Id);
        }
        
        [Test]
        public void Node_FromJson_IdOnly_ShouldSetId()
        {
            var n = JsonSerializer.Deserialize<Node>("{\"type\":\"node\",\"id\": 15441}");
            
            Assert.AreEqual(15441, n.Id);
        }

        [Test] public void Node_FromJson_CompleteNode_ShouldSetAll()
        {
            var n = JsonSerializer.Deserialize<Node>("{\"type\":\"node\",\"id\":100000,\"lat\":58.4215544,\"lon\":15.6182983,\"tags\":{\"highway\": \"residential\"},\"timestamp\":\"2017-04-17T18:31:21Z\",\"version\":3,\"changeset\":47881103,\"user\":\"riiga\",\"uid\":83501}");
            
            Assert.AreEqual(100000, n.Id);
            Assert.AreEqual(58.4215544, n.Latitude);
            Assert.AreEqual(15.6182983, n.Longitude);
            Assert.AreEqual(new DateTime(2017,04,17,18,31,21, DateTimeKind.Utc), n.TimeStamp);
            Assert.AreEqual(3, n.Version);
            Assert.AreEqual(47881103, n.ChangeSetId);
            Assert.AreEqual("riiga", n.UserName);
            Assert.AreEqual(83501, n.UserId);
            Assert.NotNull(n.Tags);
            Assert.AreEqual(1, n.Tags.Count);
            Assert.AreEqual("highway", n.Tags.First().Key);
            Assert.AreEqual("residential", n.Tags.First().Value);
        }
    }
}