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
using System;

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
    }
}