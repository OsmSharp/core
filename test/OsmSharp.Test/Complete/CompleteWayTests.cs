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
            Assert.That(osmGeo, Is.Not.Null);
            Assert.That(osmGeo, Is.InstanceOf<Way>());

            var way = osmGeo as Way;
            Assert.That(way.Id, Is.EqualTo(completeWay.Id));
            Assert.That(way.ChangeSetId, Is.EqualTo(completeWay.ChangeSetId));
            Assert.That(way.TimeStamp, Is.EqualTo(completeWay.TimeStamp));
            Assert.That(way.UserName, Is.EqualTo(completeWay.UserName));
            Assert.That(way.UserId, Is.EqualTo(completeWay.UserId));
            Assert.That(way.Version, Is.EqualTo(completeWay.Version));
            Assert.That(way.Visible, Is.EqualTo(completeWay.Visible));
            Assert.That(way.Nodes, Is.Not.Null);
            Assert.That(way.Nodes.Length, Is.EqualTo(completeWay.Nodes.Length));
            for (var i = 0; i < completeWay.Nodes.Length; i++)
            {
                Assert.That(way.Nodes[i], Is.EqualTo(completeWay.Nodes[i].Id));
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
            Assert.That(osmGeos, Is.Not.Null);

            var ways = osmGeos.OfType<Way>().ToArray();
            Assert.That(ways.Length, Is.EqualTo(1));
            var way = ways[0];
            Assert.That(way.Id, Is.EqualTo(completeWay.Id));
            Assert.That(way.ChangeSetId, Is.EqualTo(completeWay.ChangeSetId));
            Assert.That(way.TimeStamp, Is.EqualTo(completeWay.TimeStamp));
            Assert.That(way.UserName, Is.EqualTo(completeWay.UserName));
            Assert.That(way.UserId, Is.EqualTo(completeWay.UserId));
            Assert.That(way.Version, Is.EqualTo(completeWay.Version));
            Assert.That(way.Visible, Is.EqualTo(completeWay.Visible));
            Assert.That(way.Nodes, Is.Not.Null);
            Assert.That(way.Nodes.Length, Is.EqualTo(completeWay.Nodes.Length));
            for (var i = 0; i < completeWay.Nodes.Length; i++)
            {
                Assert.That(way.Nodes[i], Is.EqualTo(completeWay.Nodes[i].Id));
            }

            var nodes = osmGeos.OfType<Node>().ToArray();
            Assert.That(nodes.Length, Is.EqualTo(3));
            for (int i = 0; i < 3; i++)
            {
                var node = nodes[i];
                var expected = i + 1;
                Assert.That(node.Id, Is.EqualTo(expected));
                Assert.That(node.Version, Is.EqualTo(expected));
                Assert.That(node.Latitude, Is.EqualTo(expected));
                Assert.That(node.Longitude, Is.EqualTo(expected));
                Assert.That(node.UserId, Is.EqualTo(expected));
                Assert.That(node.Tags, Is.Not.Null);
                Assert.That(node.Tags.Count, Is.EqualTo(1));
                Assert.That(node.Tags.ContainsKey("id"), Is.True);
                Assert.That(expected.ToString(), Is.EqualTo(node.Tags["id"]));
            }

            var others = osmGeos.Except(ways).Except(nodes).ToArray();
            CollectionAssert.IsEmpty(others);
        }
    }
}