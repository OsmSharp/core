using System;
using System.Linq;
using System.Text.Json;
using NUnit.Framework;
using OsmSharp.Tags;

namespace OsmSharp.Test.IO.Json;

[TestFixture]
public class RelationTests
{
    [Test]
    public void Relation_FromJson_Empty_ShouldReturnEmptyRelation()
    {
        var r = JsonSerializer.Deserialize<Relation>("{\"type\":\"relation\"}");

        Assert.That(r.Id, Is.EqualTo(null));
    }

    [Test]
    public void Relation_FromJson_Complete_ShouldReturnComplete()
    {
        var r = JsonSerializer.Deserialize<Relation>("{\"type\":\"relation\",\"id\":2361924,\"timestamp\":\"2021-05-03T15:06:29Z\"," +
                                                     "\"version\":67,\"changeset\":104065346,\"user\":\"pointseven\",\"uid\":4355310," +
                                                     "\"members\":[" +
                                                         "{\"type\":\"node\",\"ref\":4442684,\"role\":\"node-role\"}," +
                                                         "{\"type\":\"way\",\"ref\":211334447,\"role\":\"way-role\"}," +
                                                         "{\"type\":\"relation\",\"ref\":211334453,\"role\":\"relation-role\"}]," +
                                                     "\"tags\":" +
                                                     "  {\"distance\":\"126\"," +
                                                     "  \"network\":\"nwn\"," +
                                                     "  \"ref\":\"GR 565\"," +
                                                     "  \"route\":\"hiking\"," +
                                                     "  \"type\":\"route\"}}");

        Assert.That(r.Id, Is.EqualTo(2361924));
        Assert.That(r.TimeStamp, Is.EqualTo(new DateTime(2021, 05, 03, 15, 06, 29, DateTimeKind.Utc)));
        Assert.That(r.Version, Is.EqualTo(67));
        Assert.That(r.ChangeSetId, Is.EqualTo(104065346));
        Assert.That(r.UserName, Is.EqualTo("pointseven"));
        Assert.That(r.UserId, Is.EqualTo(4355310));
        Assert.NotNull(r.Tags);
        Assert.That(r.Tags.Count, Is.EqualTo(5));
        Assert.That(r.Tags.ToArray()[0].Key, Is.EqualTo("distance"));
        Assert.That(r.Tags.ToArray()[0].Value, Is.EqualTo("126"));
        Assert.That(r.Tags.ToArray()[1].Key, Is.EqualTo("network"));
        Assert.That(r.Tags.ToArray()[1].Value, Is.EqualTo("nwn"));
        Assert.That(r.Tags.ToArray()[2].Key, Is.EqualTo("ref"));
        Assert.That(r.Tags.ToArray()[2].Value, Is.EqualTo("GR 565"));
        Assert.That(r.Tags.ToArray()[3].Key, Is.EqualTo("route"));
        Assert.That(r.Tags.ToArray()[3].Value, Is.EqualTo("hiking"));
        Assert.That(r.Tags.ToArray()[4].Key, Is.EqualTo("type"));
        Assert.That(r.Tags.ToArray()[4].Value, Is.EqualTo("route"));
        Assert.NotNull(r.Members);
        Assert.That(r.Members.Length, Is.EqualTo(3));
        Assert.That(r.Members[0].Type, Is.EqualTo(OsmGeoType.Node));
        Assert.That(r.Members[0].Role, Is.EqualTo("node-role"));
        Assert.That(r.Members[0].Id, Is.EqualTo(4442684));
        Assert.That(r.Members[1].Type, Is.EqualTo(OsmGeoType.Way));
        Assert.That(r.Members[1].Role, Is.EqualTo("way-role"));
        Assert.That(r.Members[1].Id, Is.EqualTo(211334447));
        Assert.That(r.Members[2].Type, Is.EqualTo(OsmGeoType.Relation));
        Assert.That(r.Members[2].Role, Is.EqualTo("relation-role"));
        Assert.That(r.Members[2].Id, Is.EqualTo(211334453));
    }

    [Test]
    public void Relation_ToJson_Complete_ShouldReturnCompleteJson()
    {
        var relation = new Relation()
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

        var serialized = JsonSerializer.Serialize(relation);
        Assert.That(serialized, Is.EqualTo("{\"type\":\"relation\",\"members\":[{\"type\":\"node\",\"ref\":1,\"role\":\"role1\"},{\"type\":\"way\",\"ref\":10,\"role\":\"role2\"},{\"type\":\"relation\",\"ref\":100,\"role\":\"role3\"}],\"id\":1,\"tags\":{\"amenity\":\"something\",\"key\":\"some_value\"},\"timestamp\":\"2008-09-12T21:37:45\",\"version\":1,\"user\":\"ben\",\"uid\":1}"));
    }
}
