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
using OsmSharp.Streams;
using OsmSharp.Tags;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OsmSharp.Test.Stream
{
    /// <summary>
    /// Contains tests for the PBF osm stream source.
    /// </summary>
    [TestFixture]
    class PBFOsmStreamTargetTests
    {
        /// <summary>
        /// Tests writing a node.
        /// </summary>
        [Test]
        public void TestWriteNode()
        {
            // build source stream.
            var sourceNode = new Node()
            {
                Id = 1,
                Latitude = 1.1f,
                Longitude = 1.2f,
                ChangeSetId = 1092,
                TimeStamp = DateTimeHelpers.UnixEpoch(),
                UserId = 9034,
                Version = 12
            };
            var sourceObjects = new OsmGeo[] {
                sourceNode
            };

            // build PBF stream target.
            using (var stream = new MemoryStream())
            {
                var target = new PBFOsmStreamTarget(stream);
                target.RegisterSource(sourceObjects);
                target.Pull();

                stream.Seek(0, SeekOrigin.Begin);
                var resultObjects = new List<OsmGeo>(new PBFOsmStreamSource(stream));

                Assert.That(resultObjects, Is.Not.Null);
                Assert.That(resultObjects.Count, Is.EqualTo(1));

                Assert.That(resultObjects[0].Id, Is.EqualTo(sourceObjects[0].Id));
                Assert.That(resultObjects[0].ChangeSetId, Is.EqualTo(1092));
                Assert.That(resultObjects[0].TimeStamp, Is.EqualTo(OsmSharp.IO.PBF.PBFExtensions.FromUnixTime(0)));
                Assert.That(resultObjects[0].UserId, Is.EqualTo(9034));
                Assert.That(resultObjects[0].UserName, Is.EqualTo(sourceObjects[0].UserName));
                Assert.That(resultObjects[0].Version, Is.EqualTo(12));

                var resultNode = resultObjects[0] as Node;
                Assert.That(resultNode.Latitude.Value, Is.EqualTo(sourceNode.Latitude.Value).Within(.0001f));
                Assert.That(resultNode.Longitude.Value, Is.EqualTo(sourceNode.Longitude.Value).Within(.0001f));
            }

            // build source stream.
            sourceNode = new Node()
            {
                Id = 1,
                Latitude = 1.1f,
                Longitude = 1.2f,
                ChangeSetId = 1092,
                TimeStamp = DateTimeHelpers.UnixEpoch(),
                UserId = 9034,
                Version = 12
            };
            sourceNode.Tags = new TagsCollection();
            sourceNode.Tags.Add("highway", "residential");
            sourceObjects = new OsmGeo[] {
                sourceNode
            };

            // build PBF stream target.
            using (var stream = new MemoryStream())
            {
                var target = new PBFOsmStreamTarget(stream);
                target.RegisterSource(sourceObjects);
                target.Pull();

                stream.Seek(0, SeekOrigin.Begin);
                var resultObjects = new List<OsmGeo>(new PBFOsmStreamSource(stream));

                Assert.That(resultObjects, Is.Not.Null);
                Assert.That(resultObjects.Count, Is.EqualTo(1));

                Assert.That(resultObjects[0].Id, Is.EqualTo(sourceObjects[0].Id));
                Assert.That(resultObjects[0].ChangeSetId, Is.EqualTo(1092));
                Assert.That(resultObjects[0].TimeStamp, Is.EqualTo(OsmSharp.IO.PBF.PBFExtensions.FromUnixTime(0)));
                Assert.That(resultObjects[0].UserId, Is.EqualTo(9034));
                Assert.That(resultObjects[0].UserName, Is.EqualTo(string.Empty));
                Assert.That(resultObjects[0].Version, Is.EqualTo(12));
                Assert.That(resultObjects[0].Tags.Count, Is.EqualTo(sourceObjects[0].Tags.Count));
                Assert.That(resultObjects[0].Tags.Contains(sourceObjects[0].Tags.First<Tag>()), Is.True);

                var resultNode = resultObjects[0] as Node;
                Assert.That(resultNode.Latitude.Value, Is.EqualTo(sourceNode.Latitude.Value).Within(.0001f));
                Assert.That(resultNode.Longitude.Value, Is.EqualTo(sourceNode.Longitude.Value).Within(.0001f));
            }

            // build source stream.
            sourceNode = new Node()
            {
                Id = 1,
                Latitude = 1.1f,
                Longitude = 1.2f,
                ChangeSetId = 1092,
                TimeStamp = DateTimeHelpers.UnixEpoch(),
                UserId = 9034,
                Version = 12
            };
            sourceNode.Tags = new TagsCollection();
            sourceNode.Tags.Add("highway", "residential");
            sourceNode.ChangeSetId = 1;
            sourceNode.TimeStamp = DateTime.Now;
            sourceNode.UserId = 1;
            sourceNode.UserName = "ben";
            sourceNode.Version = 3;
            sourceNode.Visible = true;
            sourceObjects = new OsmGeo[] {
                sourceNode
            };

            // build PBF stream target.
            using (var stream = new MemoryStream())
            {
                var target = new PBFOsmStreamTarget(stream);
                target.RegisterSource(sourceObjects);
                target.Pull();

                stream.Seek(0, SeekOrigin.Begin);
                var resultObjects = new List<OsmGeo>(new PBFOsmStreamSource(stream));

                Assert.That(resultObjects, Is.Not.Null);
                Assert.That(resultObjects.Count, Is.EqualTo(1));

                Assert.That(resultObjects[0].Id, Is.EqualTo(sourceObjects[0].Id));
                Assert.That(resultObjects[0].ChangeSetId, Is.EqualTo(sourceObjects[0].ChangeSetId));
                Assert.That(resultObjects[0].TimeStamp.Value.Ticks, Is.EqualTo(sourceObjects[0].TimeStamp.Value.Ticks).Within(10000000));
                Assert.That(resultObjects[0].UserId, Is.EqualTo(sourceObjects[0].UserId));
                Assert.That(resultObjects[0].UserName, Is.EqualTo(sourceObjects[0].UserName));
                Assert.That(resultObjects[0].Version, Is.EqualTo(sourceObjects[0].Version));
                Assert.That(resultObjects[0].Tags.Count, Is.EqualTo(sourceObjects[0].Tags.Count));
                Assert.That(resultObjects[0].Tags.Contains(sourceObjects[0].Tags.First<Tag>()), Is.True);

                var resultNode = resultObjects[0] as Node;
                Assert.That(resultNode.Latitude.Value, Is.EqualTo(sourceNode.Latitude.Value).Within(.0001f));
                Assert.That(resultNode.Longitude.Value, Is.EqualTo(sourceNode.Longitude.Value).Within(.0001f));
            }
        }

        /// <summary>
        /// Tests writing a way.
        /// </summary>
        [Test]
        public void TestWriteWay()
        {
            // build source stream.
            var sourceWay = new Way()
            {
                Id = 1,
                Nodes = new long[] { 1, 2 }
            };
            var sourceObjects = new OsmGeo[] {
                sourceWay
            };

            // build PBF stream target.
            using (var stream = new MemoryStream())
            {
                var target = new PBFOsmStreamTarget(stream);
                target.RegisterSource(sourceObjects);
                target.Pull();

                stream.Seek(0, SeekOrigin.Begin);
                var resultObjects = new List<OsmGeo>(new PBFOsmStreamSource(stream));

                Assert.That(resultObjects, Is.Not.Null);
                Assert.That(resultObjects.Count, Is.EqualTo(1));

                Assert.That(resultObjects[0].Id, Is.EqualTo(sourceObjects[0].Id));
                Assert.That(resultObjects[0].ChangeSetId, Is.EqualTo(0));
                Assert.That(resultObjects[0].TimeStamp, Is.EqualTo(OsmSharp.IO.PBF.PBFExtensions.FromUnixTime(0)));
                Assert.That(resultObjects[0].UserId, Is.EqualTo(0));
                Assert.That(resultObjects[0].UserName, Is.EqualTo(sourceObjects[0].UserName));
                Assert.That(resultObjects[0].Version, Is.EqualTo(0));

                var resultWay = resultObjects[0] as Way;
                Assert.That(resultWay.Nodes.Length, Is.EqualTo(sourceWay.Nodes.Length));
                Assert.That(resultWay.Nodes[0], Is.EqualTo(sourceWay.Nodes[0]));
                Assert.That(resultWay.Nodes[1], Is.EqualTo(sourceWay.Nodes[1]));
            }

            // build source stream.
            sourceWay = new Way()
            {
                Id = 1,
                Nodes = new long[] { 1, 2 }
            };
            sourceWay.Tags = new TagsCollection();
            sourceWay.Tags.Add("highway", "residential");
            sourceObjects = new OsmGeo[] {
                sourceWay
            };

            // build PBF stream target.
            using (var stream = new MemoryStream())
            {
                var target = new PBFOsmStreamTarget(stream);
                target.RegisterSource(sourceObjects);
                target.Pull();

                stream.Seek(0, SeekOrigin.Begin);
                var resultObjects = new List<OsmGeo>(new PBFOsmStreamSource(stream));

                Assert.That(resultObjects, Is.Not.Null);
                Assert.That(resultObjects.Count, Is.EqualTo(1));

                Assert.That(resultObjects[0].Id, Is.EqualTo(sourceObjects[0].Id));
                Assert.That(resultObjects[0].ChangeSetId, Is.EqualTo(0));
                Assert.That(resultObjects[0].TimeStamp, Is.EqualTo(OsmSharp.IO.PBF.PBFExtensions.FromUnixTime(0)));
                Assert.That(resultObjects[0].UserId, Is.EqualTo(0));
                Assert.That(resultObjects[0].UserName, Is.EqualTo(string.Empty));
                Assert.That(resultObjects[0].Version, Is.EqualTo(0));
                Assert.That(resultObjects[0].Tags.Count, Is.EqualTo(sourceObjects[0].Tags.Count));
                Assert.That(resultObjects[0].Tags.Contains(sourceObjects[0].Tags.First<Tag>()), Is.True);

                var resultWay = resultObjects[0] as Way;
                Assert.That(resultWay.Nodes.Length, Is.EqualTo(sourceWay.Nodes.Length));
                Assert.That(resultWay.Nodes[0], Is.EqualTo(sourceWay.Nodes[0]));
                Assert.That(resultWay.Nodes[1], Is.EqualTo(sourceWay.Nodes[1]));
            }

            // build source stream.
            sourceWay = new Way()
            {
                Id = 1,
                Nodes = new long[] { 1, 2 }
            };
            sourceWay.Tags = new TagsCollection();
            sourceWay.Tags.Add("highway", "residential");
            sourceWay.ChangeSetId = 1;
            sourceWay.TimeStamp = DateTime.Now;
            sourceWay.UserId = 1;
            sourceWay.UserName = "ben";
            sourceWay.Version = 3;
            sourceWay.Visible = true;
            sourceObjects = new OsmGeo[] {
                sourceWay
            };

            // build PBF stream target.
            using (var stream = new MemoryStream())
            {
                var target = new PBFOsmStreamTarget(stream);
                target.RegisterSource(sourceObjects);
                target.Pull();

                stream.Seek(0, SeekOrigin.Begin);
                var resultObjects = new List<OsmGeo>(new PBFOsmStreamSource(stream));

                Assert.That(resultObjects, Is.Not.Null);
                Assert.That(resultObjects.Count, Is.EqualTo(1));

                Assert.That(resultObjects[0].Id, Is.EqualTo(sourceObjects[0].Id));
                Assert.That(resultObjects[0].ChangeSetId, Is.EqualTo(sourceObjects[0].ChangeSetId));
                Assert.That(resultObjects[0].TimeStamp.Value.Ticks, Is.EqualTo(sourceObjects[0].TimeStamp.Value.Ticks).Within(10000000));
                Assert.That(resultObjects[0].UserId, Is.EqualTo(sourceObjects[0].UserId));
                Assert.That(resultObjects[0].UserName, Is.EqualTo(sourceObjects[0].UserName));
                Assert.That(resultObjects[0].Version, Is.EqualTo(sourceObjects[0].Version));
                Assert.That(resultObjects[0].Tags.Count, Is.EqualTo(sourceObjects[0].Tags.Count));
                Assert.That(resultObjects[0].Tags.Contains(sourceObjects[0].Tags.First<Tag>()), Is.True);

                var resultWay = resultObjects[0] as Way;
                Assert.That(resultWay.Nodes.Length, Is.EqualTo(sourceWay.Nodes.Length));
                Assert.That(resultWay.Nodes[0], Is.EqualTo(sourceWay.Nodes[0]));
                Assert.That(resultWay.Nodes[1], Is.EqualTo(sourceWay.Nodes[1]));
            }
        }

        /// <summary>
        /// Tests writing a relation.
        /// </summary>
        [Test]
        public void TestWriteRelation()
        {
            // build source stream.
            var sourceRelation = new Relation()
            {
                Id = 1,
                Members = new RelationMember[]
                {
                    new RelationMember()
                    {
                        Id = 1,
                        Role = "fake role",
                        Type = OsmGeoType.Node
                    },
                    new RelationMember()
                    {
                        Id = 1,
                        Role = "fake role",
                        Type = OsmGeoType.Way
                    }
                }
            };
            var sourceObjects = new OsmGeo[] {
                sourceRelation
            };

            // build PBF stream target.
            using (var stream = new MemoryStream())
            {
                var target = new PBFOsmStreamTarget(stream);
                target.RegisterSource(sourceObjects);
                target.Pull();

                stream.Seek(0, SeekOrigin.Begin);
                var resultObjects = new List<OsmGeo>(new PBFOsmStreamSource(stream));

                Assert.That(resultObjects, Is.Not.Null);
                Assert.That(resultObjects.Count, Is.EqualTo(1));

                Assert.That(resultObjects[0].Id, Is.EqualTo(sourceObjects[0].Id));
                Assert.That(resultObjects[0].ChangeSetId, Is.EqualTo(0));
                Assert.That(resultObjects[0].TimeStamp, Is.EqualTo(OsmSharp.IO.PBF.PBFExtensions.FromUnixTime(0)));
                Assert.That(resultObjects[0].UserId, Is.EqualTo(0));
                Assert.That(resultObjects[0].UserName, Is.EqualTo(string.Empty));
                Assert.That(resultObjects[0].Version, Is.EqualTo(0));

                var resultRelation = resultObjects[0] as Relation;
                Assert.That(resultRelation.Members.Length, Is.EqualTo(sourceRelation.Members.Length));
                Assert.That(resultRelation.Members[0].Id, Is.EqualTo(sourceRelation.Members[0].Id));
                Assert.That(resultRelation.Members[0].Role, Is.EqualTo(sourceRelation.Members[0].Role));
                Assert.That(resultRelation.Members[0].Type, Is.EqualTo(sourceRelation.Members[0].Type));
                Assert.That(resultRelation.Members[1].Id, Is.EqualTo(sourceRelation.Members[1].Id));
                Assert.That(resultRelation.Members[1].Role, Is.EqualTo(sourceRelation.Members[1].Role));
                Assert.That(resultRelation.Members[1].Type, Is.EqualTo(sourceRelation.Members[1].Type));
            }

            // build source stream.
            sourceRelation = new Relation()
            {
                Id = 1,
                Members = new RelationMember[]
                {
                    new RelationMember()
                    {
                        Id = 1,
                        Role = "fake role",
                        Type = OsmGeoType.Node
                    },
                    new RelationMember()
                    {
                        Id = 1,
                        Role = "fake role",
                        Type = OsmGeoType.Way
                    }
                }
            };
            sourceRelation.Tags = new TagsCollection();
            sourceRelation.Tags.Add("highway", "residential");
            sourceObjects = new OsmGeo[] {
                sourceRelation
            };

            // build PBF stream target.
            using (var stream = new MemoryStream())
            {
                var target = new PBFOsmStreamTarget(stream);
                target.RegisterSource(sourceObjects);
                target.Pull();

                stream.Seek(0, SeekOrigin.Begin);
                var resultObjects = new List<OsmGeo>(new PBFOsmStreamSource(stream));

                Assert.That(resultObjects, Is.Not.Null);
                Assert.That(resultObjects.Count, Is.EqualTo(1));

                Assert.That(resultObjects[0].Id, Is.EqualTo(sourceObjects[0].Id));
                Assert.That(resultObjects[0].ChangeSetId, Is.EqualTo(0));
                Assert.That(resultObjects[0].TimeStamp, Is.EqualTo(OsmSharp.IO.PBF.PBFExtensions.FromUnixTime(0)));
                Assert.That(resultObjects[0].UserId, Is.EqualTo(0));
                Assert.That(resultObjects[0].UserName, Is.EqualTo(string.Empty));
                Assert.That(resultObjects[0].Version, Is.EqualTo(0));
                Assert.That(resultObjects[0].Tags.Count, Is.EqualTo(sourceObjects[0].Tags.Count));
                Assert.That(resultObjects[0].Tags.Contains(sourceObjects[0].Tags.First<Tag>()), Is.True);

                var resultRelation = resultObjects[0] as Relation;
                Assert.That(resultRelation.Members.Length, Is.EqualTo(sourceRelation.Members.Length));
                Assert.That(resultRelation.Members[0].Id, Is.EqualTo(sourceRelation.Members[0].Id));
                Assert.That(resultRelation.Members[0].Role, Is.EqualTo(sourceRelation.Members[0].Role));
                Assert.That(resultRelation.Members[0].Type, Is.EqualTo(sourceRelation.Members[0].Type));
                Assert.That(resultRelation.Members[1].Id, Is.EqualTo(sourceRelation.Members[1].Id));
                Assert.That(resultRelation.Members[1].Role, Is.EqualTo(sourceRelation.Members[1].Role));
                Assert.That(resultRelation.Members[1].Type, Is.EqualTo(sourceRelation.Members[1].Type));
            }

            // build source stream.
            sourceRelation = new Relation()
            {
                Id = 1,
                Members = new RelationMember[]
                {
                    new RelationMember()
                    {
                        Id = 1,
                        Role = "fake role",
                        Type = OsmGeoType.Node
                    },
                    new RelationMember()
                    {
                        Id = 1,
                        Role = "fake role",
                        Type = OsmGeoType.Way
                    }
                }
            };
            sourceRelation.Tags = new TagsCollection();
            sourceRelation.Tags.Add("highway", "residential");
            sourceRelation.ChangeSetId = 1;
            sourceRelation.TimeStamp = DateTime.Now;
            sourceRelation.UserId = 1;
            sourceRelation.UserName = "ben";
            sourceRelation.Version = 3;
            sourceRelation.Visible = true;
            sourceObjects = new OsmGeo[] {
                sourceRelation
            };

            // build PBF stream target.
            using (var stream = new MemoryStream())
            {
                var target = new PBFOsmStreamTarget(stream);
                target.RegisterSource(sourceObjects);
                target.Pull();

                stream.Seek(0, SeekOrigin.Begin);
                var resultObjects = new List<OsmGeo>(new PBFOsmStreamSource(stream));

                Assert.That(resultObjects, Is.Not.Null);
                Assert.That(resultObjects.Count, Is.EqualTo(1));

                Assert.That(resultObjects[0].Id, Is.EqualTo(sourceObjects[0].Id));
                Assert.That(resultObjects[0].ChangeSetId, Is.EqualTo(sourceObjects[0].ChangeSetId));
                Assert.That(resultObjects[0].TimeStamp.Value.Ticks, Is.EqualTo(sourceObjects[0].TimeStamp.Value.Ticks).Within(10000000));
                Assert.That(resultObjects[0].UserId, Is.EqualTo(sourceObjects[0].UserId));
                Assert.That(resultObjects[0].UserName, Is.EqualTo(sourceObjects[0].UserName));
                Assert.That(resultObjects[0].Version, Is.EqualTo(sourceObjects[0].Version));
                Assert.That(resultObjects[0].Tags.Count, Is.EqualTo(sourceObjects[0].Tags.Count));
                Assert.That(resultObjects[0].Tags.Contains(sourceObjects[0].Tags.First<Tag>()), Is.True);

                var resultRelation = resultObjects[0] as Relation;
                Assert.That(resultRelation.Members.Length, Is.EqualTo(sourceRelation.Members.Length));
                Assert.That(resultRelation.Members[0].Id, Is.EqualTo(sourceRelation.Members[0].Id));
                Assert.That(resultRelation.Members[0].Role, Is.EqualTo(sourceRelation.Members[0].Role));
                Assert.That(resultRelation.Members[0].Type, Is.EqualTo(sourceRelation.Members[0].Type));
                Assert.That(resultRelation.Members[1].Id, Is.EqualTo(sourceRelation.Members[1].Id));
                Assert.That(resultRelation.Members[1].Role, Is.EqualTo(sourceRelation.Members[1].Role));
                Assert.That(resultRelation.Members[1].Type, Is.EqualTo(sourceRelation.Members[1].Type));
            }
        }

        /// <summary>
        /// Tests writing a stream of different objects.
        /// </summary>
        [Test]
        public void TestWriteMix()
        {
            var sourceNode = new Node()
            {
                Id = 1,
                Latitude = 1.1f,
                Longitude = 1.2f
            };
            sourceNode.Tags = new TagsCollection();
            sourceNode.Tags.Add("highway", "residential");
            sourceNode.ChangeSetId = 1;
            sourceNode.TimeStamp = DateTime.Now;
            sourceNode.UserId = 1;
            sourceNode.UserName = "ben";
            sourceNode.Version = 3;
            sourceNode.Visible = true;

            var sourceWay = new Way()
            {
                Id = 1,
                Nodes = new long[] { 1, 2 }
            };
            sourceWay.Tags = new TagsCollection();
            sourceWay.Tags.Add("highway", "residential");
            sourceWay.ChangeSetId = 1;
            sourceWay.TimeStamp = DateTime.Now;
            sourceWay.UserId = 1;
            sourceWay.UserName = "ben";
            sourceWay.Version = 3;
            sourceWay.Visible = true;

            var sourceRelation = new Relation()
            {
                Id = 1,
                Members = new RelationMember[]
                {
                    new RelationMember()
                    {
                        Id = 1,
                        Role = "fake role",
                        Type = OsmGeoType.Node
                    },
                    new RelationMember()
                    {
                        Id = 1,
                        Role = "fake role",
                        Type = OsmGeoType.Way
                    }
                }
            };
            sourceRelation.Tags = new TagsCollection();
            sourceRelation.Tags.Add("highway", "residential");
            sourceRelation.ChangeSetId = 1;
            sourceRelation.TimeStamp = DateTime.Now;
            sourceRelation.UserId = 1;
            sourceRelation.UserName = "ben";
            sourceRelation.Version = 3;
            sourceRelation.Visible = true;

            var sourceObjects = new OsmGeo[] {
                sourceNode,
                sourceWay,
                sourceRelation
            };

            // build PBF stream target.
            using (var stream = new MemoryStream())
            {
                var target = new PBFOsmStreamTarget(stream);
                target.RegisterSource(sourceObjects);
                target.Pull();

                stream.Seek(0, SeekOrigin.Begin);
                var resultObjects = new List<OsmGeo>(new PBFOsmStreamSource(stream));

                Assert.That(resultObjects, Is.Not.Null);
                Assert.That(resultObjects.Count, Is.EqualTo(3));

                Assert.That(resultObjects[0].Id, Is.EqualTo(sourceObjects[0].Id));
                Assert.That(resultObjects[0].ChangeSetId, Is.EqualTo(sourceObjects[0].ChangeSetId));
                Assert.That(resultObjects[0].TimeStamp.Value.Ticks, Is.EqualTo(sourceObjects[0].TimeStamp.Value.Ticks).Within(10000000));
                Assert.That(resultObjects[0].UserId, Is.EqualTo(sourceObjects[0].UserId));
                Assert.That(resultObjects[0].UserName, Is.EqualTo(sourceObjects[0].UserName));
                Assert.That(resultObjects[0].Version, Is.EqualTo(sourceObjects[0].Version));
                Assert.That(resultObjects[0].Tags.Count, Is.EqualTo(sourceObjects[0].Tags.Count));
                Assert.That(resultObjects[0].Tags.Contains(sourceObjects[0].Tags.First<Tag>()), Is.True);

                var resultNode = resultObjects[0] as Node;
                Assert.That(resultNode.Latitude.Value, Is.EqualTo(sourceNode.Latitude.Value).Within(.0001f));
                Assert.That(resultNode.Longitude.Value, Is.EqualTo(sourceNode.Longitude.Value).Within(.0001f));

                Assert.That(resultObjects[1].Id, Is.EqualTo(sourceObjects[1].Id));
                Assert.That(resultObjects[1].ChangeSetId, Is.EqualTo(sourceObjects[1].ChangeSetId));
                Assert.That(resultObjects[1].TimeStamp.Value.Ticks, Is.EqualTo(sourceObjects[1].TimeStamp.Value.Ticks).Within(10000000));
                Assert.That(resultObjects[1].UserId, Is.EqualTo(sourceObjects[1].UserId));
                Assert.That(resultObjects[1].UserName, Is.EqualTo(sourceObjects[1].UserName));
                Assert.That(resultObjects[1].Version, Is.EqualTo(sourceObjects[1].Version));
                Assert.That(resultObjects[1].Tags.Count, Is.EqualTo(sourceObjects[1].Tags.Count));
                Assert.That(resultObjects[1].Tags.Contains(sourceObjects[1].Tags.First<Tag>()), Is.True);

                var resultWay = resultObjects[1] as Way;
                Assert.That(resultWay.Nodes.Length, Is.EqualTo(sourceWay.Nodes.Length));
                Assert.That(resultWay.Nodes[0], Is.EqualTo(sourceWay.Nodes[0]));
                Assert.That(resultWay.Nodes[1], Is.EqualTo(sourceWay.Nodes[1]));

                Assert.That(resultObjects[2].Id, Is.EqualTo(sourceObjects[2].Id));
                Assert.That(resultObjects[2].ChangeSetId, Is.EqualTo(sourceObjects[2].ChangeSetId));
                Assert.That(resultObjects[2].TimeStamp.Value.Ticks, Is.EqualTo(sourceObjects[2].TimeStamp.Value.Ticks).Within(10000000));
                Assert.That(resultObjects[2].UserId, Is.EqualTo(sourceObjects[2].UserId));
                Assert.That(resultObjects[2].UserName, Is.EqualTo(sourceObjects[2].UserName));
                Assert.That(resultObjects[2].Version, Is.EqualTo(sourceObjects[2].Version));
                Assert.That(resultObjects[2].Tags.Count, Is.EqualTo(sourceObjects[2].Tags.Count));
                Assert.That(resultObjects[2].Tags.Contains(sourceObjects[2].Tags.First<Tag>()), Is.True);

                var resultRelation = resultObjects[2] as Relation;
                Assert.That(resultRelation.Members.Length, Is.EqualTo(sourceRelation.Members.Length));
                Assert.That(resultRelation.Members[0].Id, Is.EqualTo(sourceRelation.Members[0].Id));
                Assert.That(resultRelation.Members[0].Role, Is.EqualTo(sourceRelation.Members[0].Role));
                Assert.That(resultRelation.Members[0].Type, Is.EqualTo(sourceRelation.Members[0].Type));
                Assert.That(resultRelation.Members[1].Id, Is.EqualTo(sourceRelation.Members[1].Id));
                Assert.That(resultRelation.Members[1].Role, Is.EqualTo(sourceRelation.Members[1].Role));
                Assert.That(resultRelation.Members[1].Type, Is.EqualTo(sourceRelation.Members[1].Type));
            }
        }

        [Test]
        public void TestReadWriteCompressedRead_ShouldSucceed()
        {
            var source = new PBFOsmStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "OsmSharp.Test.data.pbf.api.osm.pbf"));

            using (var memoryStream = new MemoryStream())
            {
                var target = new PBFOsmStreamTarget(memoryStream, true);
                target.RegisterSource(source);
                target.Pull();
                memoryStream.Seek(0, 0);

                Assert.That(new PBFOsmStreamSource(memoryStream).Count(n => n is Node), Is.EqualTo(1715));
            }
        }
    }
}