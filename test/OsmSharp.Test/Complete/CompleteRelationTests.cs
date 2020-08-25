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

using NUnit.Framework;
using OsmSharp.Complete;
using OsmSharp.Tags;
using System;
using System.Linq;

namespace OsmSharp.Test.Complete
{
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
            Assert.AreEqual(completeRelation.Id, relation.Id);
            Assert.AreEqual(completeRelation.ChangeSetId, relation.ChangeSetId);
            Assert.AreEqual(completeRelation.TimeStamp, relation.TimeStamp);
            Assert.AreEqual(completeRelation.UserName, relation.UserName);
            Assert.AreEqual(completeRelation.UserId, relation.UserId);
            Assert.AreEqual(completeRelation.Version, relation.Version);
            Assert.AreEqual(completeRelation.Visible, relation.Visible);
            Assert.IsNotNull(relation.Members);
            Assert.AreEqual(completeRelation.Members.Length, relation.Members.Length);
            for (var i = 0; i < completeRelation.Members.Length; i++)
            {
                Assert.AreEqual(completeRelation.Members[i].Member.Id, relation.Members[i].Id);
                Assert.AreEqual(completeRelation.Members[i].Member.Type, relation.Members[i].Type);
                Assert.AreEqual(completeRelation.Members[i].Role, relation.Members[i].Role);
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
                Version= 1,
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
                Assert.AreEqual(1, osmGeo.Tags.Count);
                Assert.True(osmGeo.Tags.ContainsKey("type"));
                Assert.AreEqual(osmGeo.Type.ToString(), osmGeo.Tags["type"]);
            }

            var nodes = osmGeos.OfType<Node>().ToArray();
            Assert.AreEqual(1, nodes.Length);
            var resultNode = nodes[0];
            Assert.AreEqual(expectedNode.Id, resultNode.Id);
            Assert.AreEqual(expectedNode.Version, resultNode.Version);

            var ways = osmGeos.OfType<Way>().ToArray();
            Assert.AreEqual(1, ways.Length);
            var resultWay = ways[0];
            Assert.AreEqual(expectedWay.Id, resultWay.Id);
            Assert.AreEqual(expectedWay.Version, resultWay.Version);
            Assert.IsNotNull(resultWay.Nodes);
            CollectionAssert.AreEqual(expectedWay.Nodes.Select(n => n.Id), resultWay.Nodes);

            var relations = osmGeos.OfType<Relation>().Where(r => !r.Members.Any(m => m.Type == OsmGeoType.Relation)).ToArray();
            Assert.AreEqual(1, relations.Length);
            var resultRelation = relations[0];
            Assert.AreEqual(expectedRelation.Id, resultRelation.Id);
            Assert.AreEqual(expectedRelation.Version, resultRelation.Version);
            Assert.AreEqual(expectedRelation.UserId, resultRelation.UserId);
            Assert.AreEqual(expectedRelation.Members.Length, resultRelation.Members.Length);
            for (int i = 0; i < expectedRelation.Members.Length; i++)
            {
                Assert.AreEqual(expectedRelation.Members[i].Role, resultRelation.Members[i].Role);
                Assert.AreEqual(expectedRelation.Members[i].Member.Id, resultRelation.Members[i].Id);
                Assert.AreEqual(expectedRelation.Members[i].Member.Type, resultRelation.Members[i].Type);
            }

            var superRelations = osmGeos.OfType<Relation>().Where(r => r.Members.Any(m => m.Type == OsmGeoType.Relation)).ToArray();
            Assert.AreEqual(1, superRelations.Length);
            var resultSuperRelation = superRelations[0];
            Assert.AreEqual(expectedSuperRelation.Id, resultSuperRelation.Id);
            Assert.AreEqual(expectedSuperRelation.Version, resultSuperRelation.Version);
            Assert.AreEqual(expectedSuperRelation.UserId, resultSuperRelation.UserId);
            Assert.AreEqual(expectedSuperRelation.Members.Length, resultSuperRelation.Members.Length);
            for (int i = 0; i < expectedSuperRelation.Members.Length; i++)
            {
                Assert.AreEqual(expectedSuperRelation.Members[i].Member.Id, resultSuperRelation.Members[i].Id);
                Assert.AreEqual(expectedSuperRelation.Members[i].Member.Type, resultSuperRelation.Members[i].Type);
                Assert.AreEqual(expectedSuperRelation.Members[i].Role, resultSuperRelation.Members[i].Role);
            }

            var others = osmGeos.Except(nodes).Except(ways).Except(relations).Except(superRelations).ToArray();
            CollectionAssert.IsEmpty(others);
        }
    }
}