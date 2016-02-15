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
using System.Linq;

namespace OsmSharp.Test.IO.Xml
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
                Members = new RelationMember[]
                {
                    new RelationMember(1, "role1", OsmGeoType.Node),
                    new RelationMember(10, "role2", OsmGeoType.Way),
                    new RelationMember(100, "role3", OsmGeoType.Relation)
                }
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
            Assert.AreEqual(new System.DateTime(2008, 09, 12, 21, 37, 45), relation.TimeStamp.Value.ToUniversalTime());
            Assert.IsNotNull(relation.Tags);
            Assert.IsTrue(relation.Tags.Contains("amenity", "something"));
            Assert.IsTrue(relation.Tags.Contains("key", "some_value"));

            relation = serializer.Deserialize(
                new StringReader("<relation id=\"1\" user=\"ben\" uid=\"1\" version=\"1\" timestamp=\"2008-09-12T21:37:45Z\"><member type=\"node\" ref=\"1\" role=\"role1\" /><member type=\"way\" ref=\"10\" role=\"role2\" /><member type=\"relation\" ref=\"100\" role=\"role3\" /><tag k=\"amenity\" v=\"something\" /><tag k=\"key\" v=\"some_value\" /></relation>")) as Relation;
            Assert.IsNotNull(relation);
            Assert.AreEqual(1, relation.Id);
            Assert.AreEqual("ben", relation.UserName);
            Assert.AreEqual(1, relation.UserId);
            Assert.AreEqual(1, relation.Version);
            Assert.AreEqual(new System.DateTime(2008, 09, 12, 21, 37, 45), relation.TimeStamp.Value.ToUniversalTime());
            Assert.IsNotNull(relation.Tags);
            Assert.IsTrue(relation.Tags.Contains("amenity", "something"));
            Assert.IsTrue(relation.Tags.Contains("key", "some_value"));
            Assert.IsNotNull(relation.Members);
            Assert.IsTrue(relation.Members.Any(x => x.Id == 1 && x.Role == "role1" && x.Type == OsmGeoType.Node));
            Assert.IsTrue(relation.Members.Any(x => x.Id == 10 && x.Role == "role2" && x.Type == OsmGeoType.Way));
            Assert.IsTrue(relation.Members.Any(x => x.Id == 100 && x.Role == "role3" && x.Type == OsmGeoType.Relation));
        }
    }
}