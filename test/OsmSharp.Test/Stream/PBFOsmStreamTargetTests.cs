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
                Longitude = 1.2f
            };
            var sourceObjects = new OsmGeo[] {
                sourceNode
            };

            // build pfb stream target.
            using (var stream = new MemoryStream())
            {
                var target = new PBFOsmStreamTarget(stream);
                target.RegisterSource(sourceObjects);
                target.Pull();

                stream.Seek(0, SeekOrigin.Begin);
                var resultObjects = new List<OsmGeo>(new PBFOsmStreamSource(stream));

                Assert.IsNotNull(resultObjects);
                Assert.AreEqual(1, resultObjects.Count);

                Assert.AreEqual(sourceObjects[0].Id, resultObjects[0].Id);
                Assert.AreEqual(0, resultObjects[0].ChangeSetId);
                Assert.AreEqual(OsmSharp.IO.PBF.PBFExtensions.FromUnixTime(0), resultObjects[0].TimeStamp);
                Assert.AreEqual(0, resultObjects[0].UserId);
                Assert.AreEqual(sourceObjects[0].UserName, resultObjects[0].UserName);
                Assert.AreEqual(0, resultObjects[0].Version);

                var resultNode = resultObjects[0] as Node;
                Assert.AreEqual(sourceNode.Latitude.Value, resultNode.Latitude.Value, .0001f);
                Assert.AreEqual(sourceNode.Longitude.Value, resultNode.Longitude.Value, .0001f);
            }

            // build source stream.
            sourceNode = new Node()
            {
                Id = 1,
                Latitude = 1.1f,
                Longitude = 1.2f
            };
            sourceNode.Tags = new TagsCollection();
            sourceNode.Tags.Add("highway", "residential");
            sourceObjects = new OsmGeo[] {
                sourceNode
            };

            // build pfb stream target.
            using (var stream = new MemoryStream())
            {
                var target = new PBFOsmStreamTarget(stream);
                target.RegisterSource(sourceObjects);
                target.Pull();

                stream.Seek(0, SeekOrigin.Begin);
                var resultObjects = new List<OsmGeo>(new PBFOsmStreamSource(stream));

                Assert.IsNotNull(resultObjects);
                Assert.AreEqual(1, resultObjects.Count);

                Assert.AreEqual(sourceObjects[0].Id, resultObjects[0].Id);
                Assert.AreEqual(0, resultObjects[0].ChangeSetId);
                Assert.AreEqual(OsmSharp.IO.PBF.PBFExtensions.FromUnixTime(0), resultObjects[0].TimeStamp);
                Assert.AreEqual(0, resultObjects[0].UserId);
                Assert.AreEqual(string.Empty, resultObjects[0].UserName);
                Assert.AreEqual(0, resultObjects[0].Version);
                Assert.AreEqual(sourceObjects[0].Tags.Count, resultObjects[0].Tags.Count);
                Assert.IsTrue(resultObjects[0].Tags.Contains(sourceObjects[0].Tags.First<Tag>()));

                var resultNode = resultObjects[0] as Node;
                Assert.AreEqual(sourceNode.Latitude.Value, resultNode.Latitude.Value, .0001f);
                Assert.AreEqual(sourceNode.Longitude.Value, resultNode.Longitude.Value, .0001f);
            }

            // build source stream.
            sourceNode = new Node()
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
            sourceObjects = new OsmGeo[] {
                sourceNode
            };

            // build pfb stream target.
            using (var stream = new MemoryStream())
            {
                var target = new PBFOsmStreamTarget(stream);
                target.RegisterSource(sourceObjects);
                target.Pull();

                stream.Seek(0, SeekOrigin.Begin);
                var resultObjects = new List<OsmGeo>(new PBFOsmStreamSource(stream));

                Assert.IsNotNull(resultObjects);
                Assert.AreEqual(1, resultObjects.Count);

                Assert.AreEqual(sourceObjects[0].Id, resultObjects[0].Id);
                Assert.AreEqual(sourceObjects[0].ChangeSetId, resultObjects[0].ChangeSetId);
                Assert.AreEqual(sourceObjects[0].TimeStamp.Value.Ticks, resultObjects[0].TimeStamp.Value.Ticks, 10000000);
                Assert.AreEqual(sourceObjects[0].UserId, resultObjects[0].UserId);
                Assert.AreEqual(sourceObjects[0].UserName, resultObjects[0].UserName);
                Assert.AreEqual(sourceObjects[0].Version, resultObjects[0].Version);
                Assert.AreEqual(sourceObjects[0].Tags.Count, resultObjects[0].Tags.Count);
                Assert.IsTrue(resultObjects[0].Tags.Contains(sourceObjects[0].Tags.First<Tag>()));

                var resultNode = resultObjects[0] as Node;
                Assert.AreEqual(sourceNode.Latitude.Value, resultNode.Latitude.Value, .0001f);
                Assert.AreEqual(sourceNode.Longitude.Value, resultNode.Longitude.Value, .0001f);
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

            // build pfb stream target.
            using (var stream = new MemoryStream())
            {
                var target = new PBFOsmStreamTarget(stream);
                target.RegisterSource(sourceObjects);
                target.Pull();

                stream.Seek(0, SeekOrigin.Begin);
                var resultObjects = new List<OsmGeo>(new PBFOsmStreamSource(stream));

                Assert.IsNotNull(resultObjects);
                Assert.AreEqual(1, resultObjects.Count);

                Assert.AreEqual(sourceObjects[0].Id, resultObjects[0].Id);
                Assert.AreEqual(0, resultObjects[0].ChangeSetId);
                Assert.AreEqual(OsmSharp.IO.PBF.PBFExtensions.FromUnixTime(0), resultObjects[0].TimeStamp);
                Assert.AreEqual(0, resultObjects[0].UserId);
                Assert.AreEqual(sourceObjects[0].UserName, resultObjects[0].UserName);
                Assert.AreEqual(0, resultObjects[0].Version);

                var resultWay = resultObjects[0] as Way;
                Assert.AreEqual(sourceWay.Nodes.Length, resultWay.Nodes.Length);
                Assert.AreEqual(sourceWay.Nodes[0], resultWay.Nodes[0]);
                Assert.AreEqual(sourceWay.Nodes[1], resultWay.Nodes[1]);
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

            // build pfb stream target.
            using (var stream = new MemoryStream())
            {
                var target = new PBFOsmStreamTarget(stream);
                target.RegisterSource(sourceObjects);
                target.Pull();

                stream.Seek(0, SeekOrigin.Begin);
                var resultObjects = new List<OsmGeo>(new PBFOsmStreamSource(stream));

                Assert.IsNotNull(resultObjects);
                Assert.AreEqual(1, resultObjects.Count);

                Assert.AreEqual(sourceObjects[0].Id, resultObjects[0].Id);
                Assert.AreEqual(0, resultObjects[0].ChangeSetId);
                Assert.AreEqual(OsmSharp.IO.PBF.PBFExtensions.FromUnixTime(0), resultObjects[0].TimeStamp);
                Assert.AreEqual(0, resultObjects[0].UserId);
                Assert.AreEqual(string.Empty, resultObjects[0].UserName);
                Assert.AreEqual(0, resultObjects[0].Version);
                Assert.AreEqual(sourceObjects[0].Tags.Count, resultObjects[0].Tags.Count);
                Assert.IsTrue(resultObjects[0].Tags.Contains(sourceObjects[0].Tags.First<Tag>()));

                var resultWay = resultObjects[0] as Way;
                Assert.AreEqual(sourceWay.Nodes.Length, resultWay.Nodes.Length);
                Assert.AreEqual(sourceWay.Nodes[0], resultWay.Nodes[0]);
                Assert.AreEqual(sourceWay.Nodes[1], resultWay.Nodes[1]);
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

            // build pfb stream target.
            using (var stream = new MemoryStream())
            {
                var target = new PBFOsmStreamTarget(stream);
                target.RegisterSource(sourceObjects);
                target.Pull();

                stream.Seek(0, SeekOrigin.Begin);
                var resultObjects = new List<OsmGeo>(new PBFOsmStreamSource(stream));

                Assert.IsNotNull(resultObjects);
                Assert.AreEqual(1, resultObjects.Count);

                Assert.AreEqual(sourceObjects[0].Id, resultObjects[0].Id);
                Assert.AreEqual(sourceObjects[0].ChangeSetId, resultObjects[0].ChangeSetId);
                Assert.AreEqual(sourceObjects[0].TimeStamp.Value.Ticks, resultObjects[0].TimeStamp.Value.Ticks, 10000000);
                Assert.AreEqual(sourceObjects[0].UserId, resultObjects[0].UserId);
                Assert.AreEqual(sourceObjects[0].UserName, resultObjects[0].UserName);
                Assert.AreEqual(sourceObjects[0].Version, resultObjects[0].Version);
                Assert.AreEqual(sourceObjects[0].Tags.Count, resultObjects[0].Tags.Count);
                Assert.IsTrue(resultObjects[0].Tags.Contains(sourceObjects[0].Tags.First<Tag>()));

                var resultWay = resultObjects[0] as Way;
                Assert.AreEqual(sourceWay.Nodes.Length, resultWay.Nodes.Length);
                Assert.AreEqual(sourceWay.Nodes[0], resultWay.Nodes[0]);
                Assert.AreEqual(sourceWay.Nodes[1], resultWay.Nodes[1]);
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

            // build pfb stream target.
            using (var stream = new MemoryStream())
            {
                var target = new PBFOsmStreamTarget(stream);
                target.RegisterSource(sourceObjects);
                target.Pull();

                stream.Seek(0, SeekOrigin.Begin);
                var resultObjects = new List<OsmGeo>(new PBFOsmStreamSource(stream));

                Assert.IsNotNull(resultObjects);
                Assert.AreEqual(1, resultObjects.Count);

                Assert.AreEqual(sourceObjects[0].Id, resultObjects[0].Id);
                Assert.AreEqual(0, resultObjects[0].ChangeSetId);
                Assert.AreEqual(OsmSharp.IO.PBF.PBFExtensions.FromUnixTime(0), resultObjects[0].TimeStamp);
                Assert.AreEqual(0, resultObjects[0].UserId);
                Assert.AreEqual(string.Empty, resultObjects[0].UserName);
                Assert.AreEqual(0, resultObjects[0].Version);

                var resultRelation = resultObjects[0] as Relation;
                Assert.AreEqual(sourceRelation.Members.Length, resultRelation.Members.Length);
                Assert.AreEqual(sourceRelation.Members[0].Id, resultRelation.Members[0].Id);
                Assert.AreEqual(sourceRelation.Members[0].Role, resultRelation.Members[0].Role);
                Assert.AreEqual(sourceRelation.Members[0].Type, resultRelation.Members[0].Type);
                Assert.AreEqual(sourceRelation.Members[1].Id, resultRelation.Members[1].Id);
                Assert.AreEqual(sourceRelation.Members[1].Role, resultRelation.Members[1].Role);
                Assert.AreEqual(sourceRelation.Members[1].Type, resultRelation.Members[1].Type);
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

            // build pfb stream target.
            using (var stream = new MemoryStream())
            {
                var target = new PBFOsmStreamTarget(stream);
                target.RegisterSource(sourceObjects);
                target.Pull();

                stream.Seek(0, SeekOrigin.Begin);
                var resultObjects = new List<OsmGeo>(new PBFOsmStreamSource(stream));

                Assert.IsNotNull(resultObjects);
                Assert.AreEqual(1, resultObjects.Count);

                Assert.AreEqual(sourceObjects[0].Id, resultObjects[0].Id);
                Assert.AreEqual(0, resultObjects[0].ChangeSetId);
                Assert.AreEqual(OsmSharp.IO.PBF.PBFExtensions.FromUnixTime(0), resultObjects[0].TimeStamp);
                Assert.AreEqual(0, resultObjects[0].UserId);
                Assert.AreEqual(string.Empty, resultObjects[0].UserName);
                Assert.AreEqual(0, resultObjects[0].Version);
                Assert.AreEqual(sourceObjects[0].Tags.Count, resultObjects[0].Tags.Count);
                Assert.IsTrue(resultObjects[0].Tags.Contains(sourceObjects[0].Tags.First<Tag>()));

                var resultRelation = resultObjects[0] as Relation;
                Assert.AreEqual(sourceRelation.Members.Length, resultRelation.Members.Length);
                Assert.AreEqual(sourceRelation.Members[0].Id, resultRelation.Members[0].Id);
                Assert.AreEqual(sourceRelation.Members[0].Role, resultRelation.Members[0].Role);
                Assert.AreEqual(sourceRelation.Members[0].Type, resultRelation.Members[0].Type);
                Assert.AreEqual(sourceRelation.Members[1].Id, resultRelation.Members[1].Id);
                Assert.AreEqual(sourceRelation.Members[1].Role, resultRelation.Members[1].Role);
                Assert.AreEqual(sourceRelation.Members[1].Type, resultRelation.Members[1].Type);
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

            // build pfb stream target.
            using (var stream = new MemoryStream())
            {
                var target = new PBFOsmStreamTarget(stream);
                target.RegisterSource(sourceObjects);
                target.Pull();

                stream.Seek(0, SeekOrigin.Begin);
                var resultObjects = new List<OsmGeo>(new PBFOsmStreamSource(stream));

                Assert.IsNotNull(resultObjects);
                Assert.AreEqual(1, resultObjects.Count);

                Assert.AreEqual(sourceObjects[0].Id, resultObjects[0].Id);
                Assert.AreEqual(sourceObjects[0].ChangeSetId, resultObjects[0].ChangeSetId);
                Assert.AreEqual(sourceObjects[0].TimeStamp.Value.Ticks, resultObjects[0].TimeStamp.Value.Ticks, 10000000);
                Assert.AreEqual(sourceObjects[0].UserId, resultObjects[0].UserId);
                Assert.AreEqual(sourceObjects[0].UserName, resultObjects[0].UserName);
                Assert.AreEqual(sourceObjects[0].Version, resultObjects[0].Version);
                Assert.AreEqual(sourceObjects[0].Tags.Count, resultObjects[0].Tags.Count);
                Assert.IsTrue(resultObjects[0].Tags.Contains(sourceObjects[0].Tags.First<Tag>()));

                var resultRelation = resultObjects[0] as Relation;
                Assert.AreEqual(sourceRelation.Members.Length, resultRelation.Members.Length);
                Assert.AreEqual(sourceRelation.Members[0].Id, resultRelation.Members[0].Id);
                Assert.AreEqual(sourceRelation.Members[0].Role, resultRelation.Members[0].Role);
                Assert.AreEqual(sourceRelation.Members[0].Type, resultRelation.Members[0].Type);
                Assert.AreEqual(sourceRelation.Members[1].Id, resultRelation.Members[1].Id);
                Assert.AreEqual(sourceRelation.Members[1].Role, resultRelation.Members[1].Role);
                Assert.AreEqual(sourceRelation.Members[1].Type, resultRelation.Members[1].Type);
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

            // build pfb stream target.
            using (var stream = new MemoryStream())
            {
                var target = new PBFOsmStreamTarget(stream);
                target.RegisterSource(sourceObjects);
                target.Pull();

                stream.Seek(0, SeekOrigin.Begin);
                var resultObjects = new List<OsmGeo>(new PBFOsmStreamSource(stream));

                Assert.IsNotNull(resultObjects);
                Assert.AreEqual(3, resultObjects.Count);

                Assert.AreEqual(sourceObjects[0].Id, resultObjects[0].Id);
                Assert.AreEqual(sourceObjects[0].ChangeSetId, resultObjects[0].ChangeSetId);
                Assert.AreEqual(sourceObjects[0].TimeStamp.Value.Ticks, resultObjects[0].TimeStamp.Value.Ticks, 10000000);
                Assert.AreEqual(sourceObjects[0].UserId, resultObjects[0].UserId);
                Assert.AreEqual(sourceObjects[0].UserName, resultObjects[0].UserName);
                Assert.AreEqual(sourceObjects[0].Version, resultObjects[0].Version);
                Assert.AreEqual(sourceObjects[0].Tags.Count, resultObjects[0].Tags.Count);
                Assert.IsTrue(resultObjects[0].Tags.Contains(sourceObjects[0].Tags.First<Tag>()));

                var resultNode = resultObjects[0] as Node;
                Assert.AreEqual(sourceNode.Latitude.Value, resultNode.Latitude.Value, .0001f);
                Assert.AreEqual(sourceNode.Longitude.Value, resultNode.Longitude.Value, .0001f);

                Assert.AreEqual(sourceObjects[1].Id, resultObjects[1].Id);
                Assert.AreEqual(sourceObjects[1].ChangeSetId, resultObjects[1].ChangeSetId);
                Assert.AreEqual(sourceObjects[1].TimeStamp.Value.Ticks, resultObjects[1].TimeStamp.Value.Ticks, 10000000);
                Assert.AreEqual(sourceObjects[1].UserId, resultObjects[1].UserId);
                Assert.AreEqual(sourceObjects[1].UserName, resultObjects[1].UserName);
                Assert.AreEqual(sourceObjects[1].Version, resultObjects[1].Version);
                Assert.AreEqual(sourceObjects[1].Tags.Count, resultObjects[1].Tags.Count);
                Assert.IsTrue(resultObjects[1].Tags.Contains(sourceObjects[1].Tags.First<Tag>()));

                var resultWay = resultObjects[1] as Way;
                Assert.AreEqual(sourceWay.Nodes.Length, resultWay.Nodes.Length);
                Assert.AreEqual(sourceWay.Nodes[0], resultWay.Nodes[0]);
                Assert.AreEqual(sourceWay.Nodes[1], resultWay.Nodes[1]);

                Assert.AreEqual(sourceObjects[2].Id, resultObjects[2].Id);
                Assert.AreEqual(sourceObjects[2].ChangeSetId, resultObjects[2].ChangeSetId);
                Assert.AreEqual(sourceObjects[2].TimeStamp.Value.Ticks, resultObjects[2].TimeStamp.Value.Ticks, 10000000);
                Assert.AreEqual(sourceObjects[2].UserId, resultObjects[2].UserId);
                Assert.AreEqual(sourceObjects[2].UserName, resultObjects[2].UserName);
                Assert.AreEqual(sourceObjects[2].Version, resultObjects[2].Version);
                Assert.AreEqual(sourceObjects[2].Tags.Count, resultObjects[2].Tags.Count);
                Assert.IsTrue(resultObjects[2].Tags.Contains(sourceObjects[2].Tags.First<Tag>()));

                var resultRelation = resultObjects[2] as Relation;
                Assert.AreEqual(sourceRelation.Members.Length, resultRelation.Members.Length);
                Assert.AreEqual(sourceRelation.Members[0].Id, resultRelation.Members[0].Id);
                Assert.AreEqual(sourceRelation.Members[0].Role, resultRelation.Members[0].Role);
                Assert.AreEqual(sourceRelation.Members[0].Type, resultRelation.Members[0].Type);
                Assert.AreEqual(sourceRelation.Members[1].Id, resultRelation.Members[1].Id);
                Assert.AreEqual(sourceRelation.Members[1].Role, resultRelation.Members[1].Role);
                Assert.AreEqual(sourceRelation.Members[1].Type, resultRelation.Members[1].Type);
            }
        }
    }
}