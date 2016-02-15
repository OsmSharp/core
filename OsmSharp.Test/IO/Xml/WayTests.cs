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
using System.IO;
using OsmSharp.IO.Xml;
using System.Xml.Serialization;

namespace OsmSharp.Test.IO.Xml
{
    /// <summary>
    /// Contains tests for the way class.
    /// </summary>
    [TestFixture]
    public class WayTests
    {
        /// <summary>
        /// Tests serialization.
        /// </summary>
        [Test]
        public void TestSerialize()
        {
            var way = new Way()
            {
                Id = 1
            };

            Assert.AreEqual("<way id=\"1\" />", way.SerializeToXml());

            way = new Way()
            {
                Id = 1,
                Version = 1,
                UserName = "ben",
                UserId = 1
            };
            Assert.AreEqual("<way id=\"1\" user=\"ben\" uid=\"1\" version=\"1\" />",
                way.SerializeToXml());
            way = new Way()
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
            Assert.AreEqual("<way id=\"1\" user=\"ben\" uid=\"1\" version=\"1\" timestamp=\"2008-09-12T21:37:45Z\"><nd ref=\"1\" /><nd ref=\"2\" /><nd ref=\"3\" /><tag k=\"amenity\" v=\"something\" /><tag k=\"key\" v=\"some_value\" /></way>",
                way.SerializeToXml());
        }

        /// <summary>
        /// Test deserialization.
        /// </summary>
        [Test]
        public void TestDeserialize()
        {
            var serializer = new XmlSerializer(typeof(Way));

            var way = serializer.Deserialize(
                new StringReader("<way id=\"1\" />")) as Way;
            Assert.IsNotNull(way);
            Assert.AreEqual(1, way.Id);

            way = serializer.Deserialize(
                new StringReader("<way id=\"1\" user=\"ben\" uid=\"1\" version=\"1\" />")) as Way;
            Assert.IsNotNull(way);
            Assert.AreEqual(1, way.Id);
            Assert.AreEqual("ben", way.UserName);
            Assert.AreEqual(1, way.UserId);
            Assert.AreEqual(1, way.Version);

            way = serializer.Deserialize(
                new StringReader("<way id=\"1\" user=\"ben\" uid=\"1\" version=\"1\" timestamp=\"2008-09-12T21:37:45Z\"><tag k=\"amenity\" v=\"something\" /><tag k=\"key\" v=\"some_value\" /></way>")) as Way;
            Assert.IsNotNull(way);
            Assert.AreEqual(1, way.Id);
            Assert.AreEqual("ben", way.UserName);
            Assert.AreEqual(1, way.UserId);
            Assert.AreEqual(1, way.Version);
            Assert.AreEqual(new System.DateTime(2008, 09, 12, 21, 37, 45), way.TimeStamp.Value.ToUniversalTime());
            Assert.IsNotNull(way.Tags);
            Assert.IsTrue(way.Tags.Contains("amenity", "something"));
            Assert.IsTrue(way.Tags.Contains("key", "some_value"));

            way = serializer.Deserialize(
                new StringReader("<way id=\"1\" user=\"ben\" uid=\"1\" version=\"1\" timestamp=\"2008-09-12T21:37:45Z\"><nd ref=\"1\" /><nd ref=\"2\" /><nd ref=\"3\" /><tag k=\"amenity\" v=\"something\" /><tag k=\"key\" v=\"some_value\" /></way>")) as Way;
            Assert.IsNotNull(way);
            Assert.AreEqual(1, way.Id);
            Assert.AreEqual("ben", way.UserName);
            Assert.AreEqual(1, way.UserId);
            Assert.AreEqual(1, way.Version);
            Assert.AreEqual(new System.DateTime(2008, 09, 12, 21, 37, 45), way.TimeStamp.Value.ToUniversalTime());
            Assert.IsNotNull(way.Tags);
            Assert.IsTrue(way.Tags.Contains("amenity", "something"));
            Assert.IsTrue(way.Tags.Contains("key", "some_value"));
            Assert.IsNotNull(way.Nodes);
            Assert.AreEqual(3, way.Nodes.Length);
            Assert.AreEqual(1, way.Nodes[0]);
            Assert.AreEqual(2, way.Nodes[1]);
            Assert.AreEqual(3, way.Nodes[2]);
        }
    }
}