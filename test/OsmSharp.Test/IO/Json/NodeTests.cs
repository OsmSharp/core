using System;
using System.Text.Json;
using NUnit.Framework;

namespace OsmSharp.Test.IO.Json
{
    [TestFixture]
    public class NodeTests
    {
        [Test]
        public void Node_Empty_ShouldReturnEmptyNode()
        {
            var n = JsonSerializer.Deserialize<Node>("{}");
            
            Assert.AreEqual(null, n.Id);
        }
        
        [Test]
        public void Node_FromJson_IdOnly_ShouldSetId()
        {
            var n = JsonSerializer.Deserialize<Node>("{\"id\": 15441}");
            
            Assert.AreEqual(15441, n.Id);
        }

        [Test] public void Node_FromJson_CompleteNode_ShouldSetAll()
        {
            var n = JsonSerializer.Deserialize<Node>("{\"type\":\"node\",\"id\":100000,\"lat\":58.4215544,\"lon\":15.6182983,\"timestamp\":\"2017-04-17T18:31:21Z\",\"version\":3,\"changeset\":47881103,\"user\":\"riiga\",\"uid\":83501}");
            
            Assert.AreEqual(100000, n.Id);
            Assert.AreEqual(58.4215544, n.Latitude);
            Assert.AreEqual(15.6182983, n.Longitude);
            Assert.AreEqual(new DateTime(2017,04,17,18,31,21, DateTimeKind.Utc), n.TimeStamp);
            Assert.AreEqual(3, n.Version);
            Assert.AreEqual(47881103, n.ChangeSetId);
            Assert.AreEqual("riiga", n.UserName);
            Assert.AreEqual(83501, n.UserId);
        }
    }
}