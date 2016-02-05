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
using System.Linq;
using OsmSharp.Osm;
using OsmSharp.Collections.Tags;

namespace OsmSharp.Test.Osm.IO.Xml
{
    /// <summary>
    /// Contains tests for the relation class.
    /// </summary>
    [TestFixture]
    public class RelationTests
    {
        /// <summary>
        /// Tests serialization.
        /// </summary>
        [Test]
        public void TestSerialize()
        {
            var relation = new Relation()
            {
                Id = 1
            };

            Assert.AreEqual("<relation id=\"1\" />", relation.SerializeToXml());

            relation = new Relation()
            {
                Id = 1,
                Version = 1,
                UserName = "ben",
                UserId = 1
            };
            Assert.AreEqual("<relation id=\"1\" user=\"ben\" uid=\"1\" version=\"1\" />",
                relation.SerializeToXml());

            relation = new Relation()
            {
                Id = 1,
                Version = 1,
                UserName = "ben",
                UserId = 1,
                TimeStamp = new System.DateTime(2008, 09, 12, 21, 37, 45),
                Tags = new TagsCollection(
                    new Tag("amenity", "something"),
                    new Tag("key", "some_value"))
            };
            Assert.AreEqual("<relation id=\"1\" user=\"ben\" uid=\"1\" version=\"1\" timestamp=\"2008-09-12T21:37:45Z\"><tag k=\"amenity\" v=\"something\" /><tag k=\"key\" v=\"some_value\" /></relation>",
                relation.SerializeToXml());

            relation = new Relation()
            {
                Id = 1,
                Version = 1,
                UserName = "ben",
                UserId = 1,
                TimeStamp = new System.DateTime(2008, 09, 12, 21, 37, 45),
                Tags = new TagsCollection(
                    new Tag("amenity", "something"),
                    new Tag("key", "some_value")),
                Members = new System.Collections.Generic.List<RelationMember>(new RelationMember[]
                {
                    RelationMember.Create(1, "role1", OsmGeoType.Node),
                    RelationMember.Create(10, "role2", OsmGeoType.Way),
                    RelationMember.Create(100, "role3", OsmGeoType.Relation)
                })
            };
            Assert.AreEqual("<relation id=\"1\" user=\"ben\" uid=\"1\" version=\"1\" timestamp=\"2008-09-12T21:37:45Z\"><member type=\"node\" ref=\"1\" role=\"role1\" /><member type=\"way\" ref=\"10\" role=\"role2\" /><member type=\"relation\" ref=\"100\" role=\"role3\" /><tag k=\"amenity\" v=\"something\" /><tag k=\"key\" v=\"some_value\" /></relation>",
                relation.SerializeToXml());
        }

        /// <summary>
        /// Test deserialization.
        /// </summary>
        [Test]
        public void TestDeserialize()
        {
            var serializer = new XmlSerializer(typeof(Relation));

            var relation = serializer.Deserialize(
                new StringReader("<relation id=\"1\" />")) as Relation;
            Assert.IsNotNull(relation);
            Assert.AreEqual(1, relation.Id);

            relation = serializer.Deserialize(
                new StringReader("<relation id=\"1\" user=\"ben\" uid=\"1\" version=\"1\" />")) as Relation;
            Assert.IsNotNull(relation);
            Assert.AreEqual(1, relation.Id);
            Assert.AreEqual("ben", relation.UserName);
            Assert.AreEqual(1, relation.UserId);
            Assert.AreEqual(1, relation.Version);

            relation = serializer.Deserialize(
                new StringReader("<relation id=\"1\" user=\"ben\" uid=\"1\" version=\"1\" timestamp=\"2008-09-12T21:37:45Z\"><tag k=\"amenity\" v=\"something\" /><tag k=\"key\" v=\"some_value\" /></relation>")) as Relation;
            Assert.IsNotNull(relation);
            Assert.AreEqual(1, relation.Id);
            Assert.AreEqual("ben", relation.UserName);
            Assert.AreEqual(1, relation.UserId);
            Assert.AreEqual(1, relation.Version);
            Assert.AreEqual(new System.DateTime(2008, 09, 12, 21, 37, 45), relation.TimeStamp);
            Assert.IsNotNull(relation.Tags);
            Assert.IsTrue(relation.Tags.ContainsKeyValue("amenity", "something"));
            Assert.IsTrue(relation.Tags.ContainsKeyValue("key", "some_value"));

            relation = serializer.Deserialize(
                new StringReader("<relation id=\"1\" user=\"ben\" uid=\"1\" version=\"1\" timestamp=\"2008-09-12T21:37:45Z\"><member type=\"node\" ref=\"1\" role=\"role1\" /><member type=\"way\" ref=\"10\" role=\"role2\" /><member type=\"relation\" ref=\"100\" role=\"role3\" /><tag k=\"amenity\" v=\"something\" /><tag k=\"key\" v=\"some_value\" /></relation>")) as Relation;
            Assert.IsNotNull(relation);
            Assert.AreEqual(1, relation.Id);
            Assert.AreEqual("ben", relation.UserName);
            Assert.AreEqual(1, relation.UserId);
            Assert.AreEqual(1, relation.Version);
            Assert.AreEqual(new System.DateTime(2008, 09, 12, 21, 37, 45), relation.TimeStamp);
            Assert.IsNotNull(relation.Tags);
            Assert.IsTrue(relation.Tags.ContainsKeyValue("amenity", "something"));
            Assert.IsTrue(relation.Tags.ContainsKeyValue("key", "some_value"));
            Assert.IsNotNull(relation.Members);
            Assert.IsTrue(relation.Members.Any(x => x.MemberId == 1 && x.MemberRole == "role1" && x.MemberType == OsmGeoType.Node));
            Assert.IsTrue(relation.Members.Any(x => x.MemberId == 10 && x.MemberRole == "role2" && x.MemberType == OsmGeoType.Way));
            Assert.IsTrue(relation.Members.Any(x => x.MemberId == 100 && x.MemberRole == "role3" && x.MemberType == OsmGeoType.Relation));
        }
    }
}