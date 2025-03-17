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
using OsmSharp.API;

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

            Assert.That(node.SerializeToXml(), Is.EqualTo("<node id=\"1\" />"));

            node = new Node()
            {
                Id = 1,
                Version = 1,
                Latitude = 54.10f,
                Longitude = 12.2f,
                UserName = "ben",
                UserId = 1
            };
            Assert.That(node.SerializeToXml(), Is.EqualTo("<node id=\"1\" lat=\"54.0999985\" lon=\"12.1999998\" user=\"ben\" uid=\"1\" version=\"1\" />"));
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
            Assert.That(node.SerializeToXml(), Is.EqualTo("<node id=\"1\" lat=\"54.0999985\" lon=\"12.1999998\" user=\"ben\" uid=\"1\" version=\"1\" timestamp=\"2008-09-12T21:37:45Z\"><tag k=\"amenity\" v=\"something\" /><tag k=\"key\" v=\"some_value\" /></node>"));
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
            Assert.That(node, Is.Not.Null);
            Assert.That(node.Id, Is.EqualTo(1));

            node = serializer.Deserialize(
                new StringReader("<node id=\"1\" lat=\"54.1\" lon=\"12.2\" user=\"ben\" uid=\"1\" version=\"1\" />")) as Node;
            Assert.That(node, Is.Not.Null);
            Assert.That(node.Id, Is.EqualTo(1));
            Assert.That(node.Latitude, Is.EqualTo(54.1).Within(float.Epsilon));
            Assert.That(node.Longitude, Is.EqualTo(12.2).Within(float.Epsilon));
            Assert.That(node.UserName, Is.EqualTo("ben"));
            Assert.That(node.UserId, Is.EqualTo(1));
            Assert.That(node.Version, Is.EqualTo(1));

            node = serializer.Deserialize(
                new StringReader("<node id=\"1\" lat=\"54.1\" lon=\"12.2\" user=\"ben\" uid=\"1\" version=\"1\" timestamp=\"2008-09-12T21:37:45Z\"><tag k=\"amenity\" v=\"something\" /><tag k=\"key\" v=\"some_value\" /></node>")) as Node;
            Assert.That(node, Is.Not.Null);
            Assert.That(node.Id, Is.EqualTo(1));
            Assert.That(node.Latitude, Is.EqualTo(54.1).Within(float.Epsilon));
            Assert.That(node.Longitude, Is.EqualTo(12.2).Within(float.Epsilon));
            Assert.That(node.UserName, Is.EqualTo("ben"));
            Assert.That(node.UserId, Is.EqualTo(1));
            Assert.That(node.Version, Is.EqualTo(1));
            Assert.That(node.TimeStamp.Value.ToUniversalTime(), Is.EqualTo(new System.DateTime(2008, 09, 12, 21, 37, 45)));
            Assert.That(node.Tags, Is.Not.Null);
            Assert.That(node.Tags.Contains("amenity", "something"), Is.True);
            Assert.That(node.Tags.Contains("key", "some_value"), Is.True);
        }

        /// <summary>
        /// Test deserialization of multiple nodes.
        /// </summary>
        [Test]
        public void TestDeserializeMulti()
        {
            var serializer = new XmlSerializer(typeof(Osm));

            var osm = serializer.Deserialize(
                new StringReader("<osm>" +
                                     "<node id=\"1\" lat=\"54.1\" lon=\"12.2\" user=\"ben\" uid=\"1\" version=\"1\" />" +
                                     "<node id=\"2\" lat=\"54.1\" lon=\"12.2\" user=\"ben\" uid=\"2\" version=\"1\" />" +
                                 "</osm>")) as Osm;

            Assert.That(osm, Is.Not.Null);
            Assert.That(osm.Nodes, Is.Not.Null);
            Assert.That(osm.Nodes.Length, Is.EqualTo(2));
        }
    }
}