// The MIT License (MIT)

// Copyright (c) 2017 Ben Abelshausen

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

using System;
using System.Linq;
using NUnit.Framework;
using OsmSharp.Complete;
using OsmSharp.Tags;

namespace OsmSharp.Test.Complete;

/// <summary>
/// Contains tests for the complete relation class.
/// </summary>
[TestFixture]
public class CompleteRelationTests
{
    /// <summary>
    /// Tests to simple.
    /// </summary>
    [Test]
    public void TestToSimple()
    {
        var completeRelation = new CompleteRelation()
        {
            ChangeSetId = 1,
            Id = 10,
            Members = new CompleteRelationMember[]
            {
                new CompleteRelationMember()
                {
                    Member = new Node()
                    {
                        Id = 1
                    },
                    Role = "node"
                },
                new CompleteRelationMember()
                {
                    Member = new CompleteWay()
                    {
                        Id = 2
                    },
                    Role = "way"
                },
                new CompleteRelationMember()
                {
                    Member = new CompleteRelation()
                    {
                        Id = 3
                    },
                    Role = "relation"
                }
            },
            Tags = new Tags.TagsCollection(
                new Tags.Tag("tag1", "value1"),
                new Tags.Tag("tag2", "value2")),
            TimeStamp = DateTime.Now,
            UserName = "Ben",
            UserId = 1,
            Version = 23,
            Visible = true
        };

        var osmGeo = completeRelation.ToSimple();
        Assert.IsNotNull(osmGeo);
        Assert.IsInstanceOf<Relation>(osmGeo);

        var relation = osmGeo as Relation;
        Assert.That(relation.Id, Is.EqualTo(completeRelation.Id));
        Assert.That(relation.ChangeSetId, Is.EqualTo(completeRelation.ChangeSetId));
        Assert.That(relation.TimeStamp, Is.EqualTo(completeRelation.TimeStamp));
        Assert.That(relation.UserName, Is.EqualTo(completeRelation.UserName));
        Assert.That(relation.UserId, Is.EqualTo(completeRelation.UserId));
        Assert.That(relation.Version, Is.EqualTo(completeRelation.Version));
        Assert.That(relation.Visible, Is.EqualTo(completeRelation.Visible));
        Assert.IsNotNull(relation.Members);
        Assert.That(relation.Members.Length, Is.EqualTo(completeRelation.Members.Length));
        for (var i = 0; i < completeRelation.Members.Length; i++)
        {
            Assert.That(relation.Members[i].Id, Is.EqualTo(completeRelation.Members[i].Member.Id));
            Assert.That(relation.Members[i].Type, Is.EqualTo(completeRelation.Members[i].Member.Type));
            Assert.That(relation.Members[i].Role, Is.EqualTo(completeRelation.Members[i].Role));
        }
    }

    /// <summary>
    /// Tests to simple with children.
    /// Nodes, ways, relations
    /// Duplicate nodes, duplicate ways, circular references
    /// </summary>
    [Test]
    public void TestToSimpleWithChildren()
    {
        var expectedNode = new Node()
        {
            Id = 1,
            Version = 1,
            UserId = 1,
            Tags = new TagsCollection(new Tag("type", "Node"))
        };

        var expectedWay = new CompleteWay()
        {
            Id = 2,
            Version = 2,
            UserId = 2,
            Nodes = new Node[] { expectedNode, expectedNode }, // duplicate elements
            Tags = new TagsCollection(new Tag("type", "Way"))
        };

        var expectedRelation = new CompleteRelation()
        {
            Id = 3,
            Version = 3,
            UserId = 3,
            Tags = new TagsCollection(new Tag("type", "Relation")),
            Members = new CompleteRelationMember[]
            {
                new CompleteRelationMember()
                {
                    Member = expectedNode,
                    Role = "Node"
                },
                new CompleteRelationMember()
                {
                    Member = expectedWay,
                    Role = "Way"
                }
            }
        };

        var expectedSuperRelation = new CompleteRelation()
        {
            Id = 4,
            Version = 4,
            UserId = 4,
            Tags = new TagsCollection(new Tag("type", "Relation"))
        };

        expectedSuperRelation.Members = new CompleteRelationMember[]
            {
                new CompleteRelationMember()
                {
                    Member = expectedNode,
                    Role = "Node"
                },
                new CompleteRelationMember()
                {
                    Member = expectedWay,
                    Role = "Way"
                },
                new CompleteRelationMember()
                {
                    Member = expectedRelation,
                    Role = "Relation"
                },
                new CompleteRelationMember()
                {
                    Member = expectedSuperRelation, // Circular reference
                    Role = "SuperRelation"
                }
            };

        var osmGeos = expectedSuperRelation.ToSimpleWithChildren();
        Assert.IsNotNull(osmGeos);

        foreach (var osmGeo in osmGeos)
        {
            Assert.IsNotNull(osmGeo.Tags);
            Assert.That(osmGeo.Tags.Count, Is.EqualTo(1));
            Assert.True(osmGeo.Tags.ContainsKey("type"));
            Assert.That(osmGeo.Tags["type"], Is.EqualTo(osmGeo.Type.ToString()));
        }

        var nodes = osmGeos.OfType<Node>().ToArray();
        Assert.That(nodes.Length, Is.EqualTo(1));
        var resultNode = nodes[0];
        Assert.That(resultNode.Id, Is.EqualTo(expectedNode.Id));
        Assert.That(resultNode.Version, Is.EqualTo(expectedNode.Version));

        var ways = osmGeos.OfType<Way>().ToArray();
        Assert.That(ways.Length, Is.EqualTo(1));
        var resultWay = ways[0];
        Assert.That(resultWay.Id, Is.EqualTo(expectedWay.Id));
        Assert.That(resultWay.Version, Is.EqualTo(expectedWay.Version));
        Assert.IsNotNull(resultWay.Nodes);
        CollectionAssert.AreEqual(expectedWay.Nodes.Select(n => n.Id), resultWay.Nodes);

        var relations = osmGeos.OfType<Relation>().Where(r => !r.Members.Any(m => m.Type == OsmGeoType.Relation)).ToArray();
        Assert.That(relations.Length, Is.EqualTo(1));
        var resultRelation = relations[0];
        Assert.That(resultRelation.Id, Is.EqualTo(expectedRelation.Id));
        Assert.That(resultRelation.Version, Is.EqualTo(expectedRelation.Version));
        Assert.That(resultRelation.UserId, Is.EqualTo(expectedRelation.UserId));
        Assert.That(resultRelation.Members.Length, Is.EqualTo(expectedRelation.Members.Length));
        for (int i = 0; i < expectedRelation.Members.Length; i++)
        {
            Assert.That(resultRelation.Members[i].Role, Is.EqualTo(expectedRelation.Members[i].Role));
            Assert.That(resultRelation.Members[i].Id, Is.EqualTo(expectedRelation.Members[i].Member.Id));
            Assert.That(resultRelation.Members[i].Type, Is.EqualTo(expectedRelation.Members[i].Member.Type));
        }

        var superRelations = osmGeos.OfType<Relation>().Where(r => r.Members.Any(m => m.Type == OsmGeoType.Relation)).ToArray();
        Assert.That(superRelations.Length, Is.EqualTo(1));
        var resultSuperRelation = superRelations[0];
        Assert.That(resultSuperRelation.Id, Is.EqualTo(expectedSuperRelation.Id));
        Assert.That(resultSuperRelation.Version, Is.EqualTo(expectedSuperRelation.Version));
        Assert.That(resultSuperRelation.UserId, Is.EqualTo(expectedSuperRelation.UserId));
        Assert.That(resultSuperRelation.Members.Length, Is.EqualTo(expectedSuperRelation.Members.Length));
        for (int i = 0; i < expectedSuperRelation.Members.Length; i++)
        {
            Assert.That(resultSuperRelation.Members[i].Id, Is.EqualTo(expectedSuperRelation.Members[i].Member.Id));
            Assert.That(resultSuperRelation.Members[i].Type, Is.EqualTo(expectedSuperRelation.Members[i].Member.Type));
            Assert.That(resultSuperRelation.Members[i].Role, Is.EqualTo(expectedSuperRelation.Members[i].Role));
        }

        var others = osmGeos.Except(nodes).Except(ways).Except(relations).Except(superRelations).ToArray();
        CollectionAssert.IsEmpty(others);
    }
}
