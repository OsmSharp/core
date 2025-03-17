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
using OsmSharp.API;

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

            Assert.That(way.SerializeToXml(), Is.EqualTo("<way id=\"1\" />"));

            way = new Way()
            {
                Id = 1,
                Version = 1,
                UserName = "ben",
                UserId = 1
            };
            Assert.That(way.SerializeToXml(), Is.EqualTo("<way id=\"1\" user=\"ben\" uid=\"1\" version=\"1\" />"));
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
            Assert.That(way.SerializeToXml(), Is.EqualTo("<way id=\"1\" user=\"ben\" uid=\"1\" version=\"1\" timestamp=\"2008-09-12T21:37:45Z\"><nd ref=\"1\" /><nd ref=\"2\" /><nd ref=\"3\" /><tag k=\"amenity\" v=\"something\" /><tag k=\"key\" v=\"some_value\" /></way>"));
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
            Assert.That(way, Is.Not.Null);
            Assert.That(way.Id, Is.EqualTo(1));

            way = serializer.Deserialize(
                new StringReader("<way id=\"1\" user=\"ben\" uid=\"1\" version=\"1\" />")) as Way;
            Assert.That(way, Is.Not.Null);
            Assert.That(way.Id, Is.EqualTo(1));
            Assert.That(way.UserName, Is.EqualTo("ben"));
            Assert.That(way.UserId, Is.EqualTo(1));
            Assert.That(way.Version, Is.EqualTo(1));

            way = serializer.Deserialize(
                new StringReader("<way id=\"1\" user=\"ben\" uid=\"1\" version=\"1\" timestamp=\"2008-09-12T21:37:45Z\"><tag k=\"amenity\" v=\"something\" /><tag k=\"key\" v=\"some_value\" /></way>")) as Way;
            Assert.That(way, Is.Not.Null);
            Assert.That(way.Id, Is.EqualTo(1));
            Assert.That(way.UserName, Is.EqualTo("ben"));
            Assert.That(way.UserId, Is.EqualTo(1));
            Assert.That(way.Version, Is.EqualTo(1));
            Assert.That(way.TimeStamp.Value.ToUniversalTime(), Is.EqualTo(new System.DateTime(2008, 09, 12, 21, 37, 45)));
            Assert.That(way.Tags, Is.Not.Null);
            Assert.That(way.Tags.Contains("amenity", "something"), Is.True);
            Assert.That(way.Tags.Contains("key", "some_value"), Is.True);

            way = serializer.Deserialize(
                new StringReader("<way id=\"1\" user=\"ben\" uid=\"1\" version=\"1\" timestamp=\"2008-09-12T21:37:45Z\"><nd ref=\"1\" /><nd ref=\"2\" /><nd ref=\"3\" /><tag k=\"amenity\" v=\"something\" /><tag k=\"key\" v=\"some_value\" /></way>")) as Way;
            Assert.That(way, Is.Not.Null);
            Assert.That(way.Id, Is.EqualTo(1));
            Assert.That(way.UserName, Is.EqualTo("ben"));
            Assert.That(way.UserId, Is.EqualTo(1));
            Assert.That(way.Version, Is.EqualTo(1));
            Assert.That(way.TimeStamp.Value.ToUniversalTime(), Is.EqualTo(new System.DateTime(2008, 09, 12, 21, 37, 45)));
            Assert.That(way.Tags, Is.Not.Null);
            Assert.That(way.Tags.Contains("amenity", "something"), Is.True);
            Assert.That(way.Tags.Contains("key", "some_value"), Is.True);
            Assert.That(way.Nodes, Is.Not.Null);
            Assert.That(way.Nodes.Length, Is.EqualTo(3));
            Assert.That(way.Nodes[0], Is.EqualTo(1));
            Assert.That(way.Nodes[1], Is.EqualTo(2));
            Assert.That(way.Nodes[2], Is.EqualTo(3));
        }

        /// <summary>
        /// Test deserialization of multiple ways.
        /// </summary>
        [Test]
        public void TestDeserializeMulti()
        {
            var serializer = new XmlSerializer(typeof(Osm));

            var osm = serializer.Deserialize(
                new StringReader("<osm>" +
                                     "<way id=\"1\" user=\"ben\" uid=\"1\" version=\"1\" />" +
                                     "<way id=\"2\" user=\"ben\" uid=\"1\" version=\"1\" />" +
                                 "</osm>")) as Osm;

            Assert.That(osm, Is.Not.Null);
            Assert.That(osm.Ways, Is.Not.Null);
            Assert.That(osm.Ways.Length, Is.EqualTo(2));

            osm = serializer.Deserialize(
                new StringReader("<?xml version=\"1.0\" encoding=\"utf - 8\"?><osm version=\"0.6\"><way id=\"1\"><nd ref=\"11\" /><nd ref=\"12\" /><nd ref=\"13\" /></way><way id=\"2\"><nd ref=\"21\" /><nd ref=\"22\" /><nd ref=\"23\" /></way><way id=\"3\"><nd ref=\"31\" /><nd ref=\"32\" /><nd ref=\"33\" /></way></osm>")) as Osm;
            
            Assert.That(osm, Is.Not.Null);
            Assert.That(osm.Ways, Is.Not.Null);
            Assert.That(osm.Ways.Length, Is.EqualTo(3));
        }
    }
}