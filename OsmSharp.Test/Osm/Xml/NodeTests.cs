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
using System.Xml;
using System.Xml.Serialization;
using OsmSharp.Osm;
using OsmSharp.Collections.Tags;

namespace OsmSharp.Test.Osm.Xml
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
            Assert.AreEqual("<node id=\"1\" latitude=\"54.1\" longitude=\"12.2\" user=\"ben\" uid=\"1\" version=\"1\" />", 
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
            Assert.AreEqual("<node id=\"1\" latitude=\"54.1\" longitude=\"12.2\" user=\"ben\" uid=\"1\" version=\"1\" timestamp=\"2008-09-12T21:37:45Z\"><tag k=\"amenity\" v=\"something\" /><tag k=\"key\" v=\"some_value\" /></node>",
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
                new StringReader("<node id=\"1\" latitude=\"54.1\" longitude=\"12.2\" user=\"ben\" uid=\"1\" version=\"1\" />")) as Node;
            Assert.IsNotNull(node);
            Assert.AreEqual(1, node.Id);
            Assert.AreEqual(54.1f, node.Latitude);
            Assert.AreEqual(12.2f, node.Longitude);
            Assert.AreEqual("ben", node.UserName);
            Assert.AreEqual(1, node.UserId);
            Assert.AreEqual(1, node.Version);

            node = serializer.Deserialize(
                new StringReader("<node id=\"1\" latitude=\"54.1\" longitude=\"12.2\" user=\"ben\" uid=\"1\" version=\"1\" timestamp=\"2008-09-12T21:37:45Z\"><tag k=\"amenity\" v=\"something\" /><tag k=\"key\" v=\"some_value\" /></node>")) as Node;
            Assert.IsNotNull(node);
            Assert.AreEqual(1, node.Id);
            Assert.AreEqual(54.1f, node.Latitude);
            Assert.AreEqual(12.2f, node.Longitude);
            Assert.AreEqual("ben", node.UserName);
            Assert.AreEqual(1, node.UserId);
            Assert.AreEqual(1, node.Version);
            Assert.AreEqual(new System.DateTime(2008, 09, 12, 21, 37, 45), node.TimeStamp);
            Assert.IsNotNull(node.Tags);
            Assert.IsTrue(node.Tags.ContainsKeyValue("amenity", "something"));
            Assert.IsTrue(node.Tags.ContainsKeyValue("key", "some_value"));
        }
    }
}