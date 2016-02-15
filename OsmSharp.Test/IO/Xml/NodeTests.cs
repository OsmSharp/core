// The MIT License (MIT)

// Copyright (c) 2016 Ben Abelshausen

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using NUnit.Framework;
using OsmSharp.Tags;
using OsmSharp.IO.Xml;
using System.IO;
using System.Xml.Serialization;

namespace OsmSharp.Test.IO.Xml
{
    /// <summary>
    /// Contains tests for the node class.
    /// </summary>
    [TestFixture]
    public class NodeTests
    {
        /// <summary>
        /// Tests serialization.
        /// </summary>
        [Test]
        public void TestSerialize()
        {
            var node = new Node()
            {
                Id = 1
            };

            Assert.AreEqual("<node id=\"1\" />", node.SerializeToXml());

            node = new Node()
            {
                Id = 1,
                Version = 1,
                Latitude = 54.10f,
                Longitude = 12.2f,
                UserName = "ben",
                UserId = 1
            };
            Assert.AreEqual("<node id=\"1\" lat=\"54.1\" lon=\"12.2\" user=\"ben\" uid=\"1\" version=\"1\" />",
                node.SerializeToXml());
            node = new Node()
            {
                Id = 1,
                Version = 1,
                Latitude = 54.10f,
                Longitude = 12.2f,
                UserName = "ben",
                UserId = 1,
                TimeStamp = new System.DateTime(2008, 09, 12, 21, 37, 45),
                Tags = new TagsCollection(
                    new Tag("amenity", "something"),
                    new Tag("key", "some_value"))
            };
            Assert.AreEqual("<node id=\"1\" lat=\"54.1\" lon=\"12.2\" user=\"ben\" uid=\"1\" version=\"1\" timestamp=\"2008-09-12T21:37:45Z\"><tag k=\"amenity\" v=\"something\" /><tag k=\"key\" v=\"some_value\" /></node>",
                node.SerializeToXml());
        }

        /// <summary>
        /// Test deserialization.
        /// </summary>
        [Test]
        public void TestDeserialize()
        {
            var serializer = new XmlSerializer(typeof(Node));

            var node = serializer.Deserialize(
                new StringReader("<node id=\"1\" />")) as Node;
            Assert.IsNotNull(node);
            Assert.AreEqual(1, node.Id);

            node = serializer.Deserialize(
                new StringReader("<node id=\"1\" lat=\"54.1\" lon=\"12.2\" user=\"ben\" uid=\"1\" version=\"1\" />")) as Node;
            Assert.IsNotNull(node);
            Assert.AreEqual(1, node.Id);
            Assert.AreEqual(54.1f, node.Latitude);
            Assert.AreEqual(12.2f, node.Longitude);
            Assert.AreEqual("ben", node.UserName);
            Assert.AreEqual(1, node.UserId);
            Assert.AreEqual(1, node.Version);

            node = serializer.Deserialize(
                new StringReader("<node id=\"1\" lat=\"54.1\" lon=\"12.2\" user=\"ben\" uid=\"1\" version=\"1\" timestamp=\"2008-09-12T21:37:45Z\"><tag k=\"amenity\" v=\"something\" /><tag k=\"key\" v=\"some_value\" /></node>")) as Node;
            Assert.IsNotNull(node);
            Assert.AreEqual(1, node.Id);
            Assert.AreEqual(54.1f, node.Latitude);
            Assert.AreEqual(12.2f, node.Longitude);
            Assert.AreEqual("ben", node.UserName);
            Assert.AreEqual(1, node.UserId);
            Assert.AreEqual(1, node.Version);
            Assert.AreEqual(new System.DateTime(2008, 09, 12, 21, 37, 45), node.TimeStamp.Value.ToUniversalTime());
            Assert.IsNotNull(node.Tags);
            Assert.IsTrue(node.Tags.Contains("amenity", "something"));
            Assert.IsTrue(node.Tags.Contains("key", "some_value"));
        }
    }
}