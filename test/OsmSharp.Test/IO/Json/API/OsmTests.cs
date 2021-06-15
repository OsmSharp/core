using System;
using System.Text.Json;
using NUnit.Framework;
using OsmSharp.API;
using OsmSharp.Tags;

namespace OsmSharp.Test.IO.Json.API
{
    [TestFixture]
    public class OsmTests
    {
        [Test]
        public void Osm_ToJson_Complete_ShouldWriteCompleteJson()
        {
            var osm = new Osm()
            {
                Version = 0.6,
                Generator = "OsmSharp",
                Nodes = new []{new Node
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
                }},
                Ways = new []{new Way()
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
                }},
                Relations = new []{new Relation()
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
                }}
            };

            var serialized = JsonSerializer.Serialize(osm);
            Assert.AreEqual("{\"version\":0.6,\"generator\":\"OsmSharp\",\"elements\":[{\"type\":\"node\",\"lat\":58.4215544,\"lon\":15.6182983,\"id\":100000,\"tags\":{\"highway\":\"residential\"},\"timestamp\":\"2017-04-17T18:31:21Z\",\"version\":3,\"changeset\":47881103,\"user\":\"riiga\",\"uid\":83501},{\"type\":\"way\",\"nodes\":[1,2,3],\"id\":1,\"tags\":{\"amenity\":\"something\",\"key\":\"some_value\"},\"timestamp\":\"2008-09-12T21:37:45\",\"version\":1,\"user\":\"ben\",\"uid\":1},{\"type\":\"relation\",\"members\":[{\"type\":\"node\",\"ref\":1,\"role\":\"role1\"},{\"type\":\"way\",\"ref\":10,\"role\":\"role2\"},{\"type\":\"relation\",\"ref\":100,\"role\":\"role3\"}],\"id\":1,\"tags\":{\"amenity\":\"something\",\"key\":\"some_value\"},\"timestamp\":\"2008-09-12T21:37:45\",\"version\":1,\"user\":\"ben\",\"uid\":1}]}", 
                serialized);
        }

        [Test]
        public void Osm_FromJson_Complete_ShouldReadCompleteJson()
        {
            var osm = JsonSerializer.Deserialize<Osm>(
                "{\"version\":0.6,\"generator\":\"OsmSharp\",\"elements\":[{\"type\":\"node\",\"lat\":58.4215544,\"lon\":15.6182983,\"id\":100000,\"tags\":{\"highway\":\"residential\"},\"timestamp\":\"2017-04-17T18:31:21Z\",\"version\":3,\"changeset\":47881103,\"user\":\"riiga\",\"uid\":83501},{\"type\":\"way\",\"nodes\":[1,2,3],\"id\":1,\"tags\":{\"amenity\":\"something\",\"key\":\"some_value\"},\"timestamp\":\"2008-09-12T21:37:45\",\"version\":1,\"user\":\"ben\",\"uid\":1},{\"type\":\"relation\",\"members\":[{\"type\":\"node\",\"ref\":1,\"role\":\"role1\"},{\"type\":\"way\",\"ref\":10,\"role\":\"role2\"},{\"type\":\"relation\",\"ref\":100,\"role\":\"role3\"}],\"id\":1,\"tags\":{\"amenity\":\"something\",\"key\":\"some_value\"},\"timestamp\":\"2008-09-12T21:37:45\",\"version\":1,\"user\":\"ben\",\"uid\":1}]}");
            
            Assert.NotNull(osm);
            Assert.AreEqual(0.6, osm.Version);
            Assert.AreEqual("OsmSharp", osm.Generator);

            Assert.AreEqual(1, osm.Nodes.Length);   
            Assert.AreEqual(1, osm.Ways.Length);   
            Assert.AreEqual(1, osm.Relations.Length);   
        }
    }
}