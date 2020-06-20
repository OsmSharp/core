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
    /// Contains tests for the complete way class.
    /// </summary>
    [TestFixture]
    public class CompleteWayTests
    {
        /// <summary>
        /// Tests to simple.
        /// </summary>
        [Test]
        public void TestToSimple()
        {
            var completeWay = new CompleteWay()
            {
                ChangeSetId = 1,
                Id = 10,
                Nodes = new Node[]
                {
                    new Node()
                    {
                        Id = 1
                    },
                    new Node()
                    {
                        Id = 2
                    },
                    new Node()
                    {
                        Id = 3
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

            var osmGeo = completeWay.ToSimple();
            Assert.IsNotNull(osmGeo);
            Assert.IsInstanceOf<Way>(osmGeo);

            var way = osmGeo as Way;
            Assert.AreEqual(completeWay.Id, way.Id);
            Assert.AreEqual(completeWay.ChangeSetId, way.ChangeSetId);
            Assert.AreEqual(completeWay.TimeStamp, way.TimeStamp);
            Assert.AreEqual(completeWay.UserName, way.UserName);
            Assert.AreEqual(completeWay.UserId, way.UserId);
            Assert.AreEqual(completeWay.Version, way.Version);
            Assert.AreEqual(completeWay.Visible, way.Visible);
            Assert.IsNotNull(way.Nodes);
            Assert.AreEqual(completeWay.Nodes.Length, way.Nodes.Length);
            for (var i = 0; i < completeWay.Nodes.Length; i++)
            {
                Assert.AreEqual(completeWay.Nodes[i].Id, way.Nodes[i]);
            }
        }

        /// <summary>
        /// Tests to simple with children.
        /// </summary>
        [Test]
        public void TestToSimpleWithChildren()
        {
            var completeWay = new CompleteWay()
            {
                ChangeSetId = 1,
                Id = 10,
                Nodes = new Node[]
                {
                    new Node()
                    {
                        Id = 1,
                        Version = 1,
                        Tags = new TagsCollection(new Tag("id", "1")),
                        Latitude = 1,
                        Longitude = 1,
                        UserId = 1
                    },
                    new Node()
                    {
                        Id = 2,
                        Version = 2,
                        Tags = new TagsCollection(new Tag("id", "2")),
                        Latitude = 2,
                        Longitude = 2,
                        UserId = 2
                    },
                    new Node()
                    {
                        Id = 3,
                        Version = 3,
                        Tags = new TagsCollection(new Tag("id", "3")),
                        Latitude = 3,
                        Longitude = 3,
                        UserId = 3
                    },
                    new Node() // testing a closed way, ensuring node isn't duplicated.
                    {
                        Id = 1,
                        Version = 1,
                        Tags = new TagsCollection(new Tag("id", "1")),
                        Latitude = 1,
                        Longitude = 1,
                        UserId = 1
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

            var osmGeos = completeWay.ToSimpleWithChildren();
            Assert.IsNotNull(osmGeos);

            var ways = osmGeos.OfType<Way>().ToArray();
            Assert.AreEqual(1, ways.Length);
            var way = ways[0];
            Assert.AreEqual(completeWay.Id, way.Id);
            Assert.AreEqual(completeWay.ChangeSetId, way.ChangeSetId);
            Assert.AreEqual(completeWay.TimeStamp, way.TimeStamp);
            Assert.AreEqual(completeWay.UserName, way.UserName);
            Assert.AreEqual(completeWay.UserId, way.UserId);
            Assert.AreEqual(completeWay.Version, way.Version);
            Assert.AreEqual(completeWay.Visible, way.Visible);
            Assert.IsNotNull(way.Nodes);
            Assert.AreEqual(completeWay.Nodes.Length, way.Nodes.Length);
            for (var i = 0; i < completeWay.Nodes.Length; i++)
            {
                Assert.AreEqual(completeWay.Nodes[i].Id, way.Nodes[i]);
            }

            var nodes = osmGeos.OfType<Node>().ToArray();
            Assert.AreEqual(3, nodes.Length);
            for (int i = 0; i < 3; i++)
            {
                var node = nodes[i];
                var expected = i + 1;
                Assert.AreEqual(expected, node.Id);
                Assert.AreEqual(expected, node.Version);
                Assert.AreEqual(expected, node.Latitude);
                Assert.AreEqual(expected, node.Longitude);
                Assert.AreEqual(expected, node.UserId);
                Assert.IsNotNull(node.Tags);
                Assert.AreEqual(1, node.Tags.Count);
                Assert.IsTrue(node.Tags.ContainsKey("id"));
                Assert.AreEqual(node.Tags["id"], expected.ToString());
            }

            var others = osmGeos.Except(ways).Except(nodes).ToArray();
            CollectionAssert.IsEmpty(others);
        }
    }
}