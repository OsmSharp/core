// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2016 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using NUnit.Framework;
using System.IO;
using System.Xml.Serialization;
using OsmSharp.Osm;
using OsmSharp.Collections.Tags;

namespace OsmSharp.Test.Osm.Xml
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
                Nodes = new System.Collections.Generic.List<long>(new long[]
                {
                    1, 2, 3
                })
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
                new StringReader("<way id=\"1\" latitude=\"54.1\" longitude=\"12.2\" user=\"ben\" uid=\"1\" version=\"1\" />")) as Way;
            Assert.IsNotNull(way);
            Assert.AreEqual(1, way.Id);
            Assert.AreEqual("ben", way.UserName);
            Assert.AreEqual(1, way.UserId);
            Assert.AreEqual(1, way.Version);

            way = serializer.Deserialize(
                new StringReader("<way id=\"1\" latitude=\"54.1\" longitude=\"12.2\" user=\"ben\" uid=\"1\" version=\"1\" timestamp=\"2008-09-12T21:37:45Z\"><tag k=\"amenity\" v=\"something\" /><tag k=\"key\" v=\"some_value\" /></way>")) as Way;
            Assert.IsNotNull(way);
            Assert.AreEqual(1, way.Id);
            Assert.AreEqual("ben", way.UserName);
            Assert.AreEqual(1, way.UserId);
            Assert.AreEqual(1, way.Version);
            Assert.AreEqual(new System.DateTime(2008, 09, 12, 21, 37, 45), way.TimeStamp);
            Assert.IsNotNull(way.Tags);
            Assert.IsTrue(way.Tags.ContainsKeyValue("amenity", "something"));
            Assert.IsTrue(way.Tags.ContainsKeyValue("key", "some_value"));

            way = serializer.Deserialize(
                new StringReader("<way id=\"1\" user=\"ben\" uid=\"1\" version=\"1\" timestamp=\"2008-09-12T21:37:45Z\"><nd ref=\"1\" /><nd ref=\"2\" /><nd ref=\"3\" /><tag k=\"amenity\" v=\"something\" /><tag k=\"key\" v=\"some_value\" /></way>")) as Way;
            Assert.IsNotNull(way);
            Assert.AreEqual(1, way.Id);
            Assert.AreEqual("ben", way.UserName);
            Assert.AreEqual(1, way.UserId);
            Assert.AreEqual(1, way.Version);
            Assert.AreEqual(new System.DateTime(2008, 09, 12, 21, 37, 45), way.TimeStamp);
            Assert.IsNotNull(way.Tags);
            Assert.IsTrue(way.Tags.ContainsKeyValue("amenity", "something"));
            Assert.IsTrue(way.Tags.ContainsKeyValue("key", "some_value"));
            Assert.IsNotNull(way.Nodes);
            Assert.AreEqual(3, way.Nodes.Count);
            Assert.AreEqual(1, way.Nodes[0]);
            Assert.AreEqual(2, way.Nodes[1]);
            Assert.AreEqual(3, way.Nodes[2]);
        }
    }
}