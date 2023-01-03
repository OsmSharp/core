﻿// The MIT License (MIT)

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
                TimeStamp = DateTime.UnixEpoch,
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

                Assert.IsNotNull(resultObjects);
                Assert.AreEqual(1, resultObjects.Count);

                Assert.AreEqual(sourceObjects[0].Id, resultObjects[0].Id);
                Assert.AreEqual(1092, resultObjects[0].ChangeSetId);
                Assert.AreEqual(OsmSharp.IO.PBF.PBFExtensions.FromUnixTime(0), resultObjects[0].TimeStamp);
                Assert.AreEqual(9034, resultObjects[0].UserId);
                Assert.AreEqual(sourceObjects[0].UserName, resultObjects[0].UserName);
                Assert.AreEqual(12, resultObjects[0].Version);

                var resultNode = resultObjects[0] as Node;
                Assert.AreEqual(sourceNode.Latitude.Value, resultNode.Latitude.Value, .0001f);
                Assert.AreEqual(sourceNode.Longitude.Value, resultNode.Longitude.Value, .0001f);
            }

            // build source stream.
            sourceNode = new Node()
            {
                Id = 1,
                Latitude = 1.1f,
                Longitude = 1.2f,
                ChangeSetId = 1092,
                TimeStamp = DateTime.UnixEpoch,
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

                Assert.IsNotNull(resultObjects);
                Assert.AreEqual(1, resultObjects.Count);

                Assert.AreEqual(sourceObjects[0].Id, resultObjects[0].Id);
                Assert.AreEqual(1092, resultObjects[0].ChangeSetId);
                Assert.AreEqual(OsmSharp.IO.PBF.PBFExtensions.FromUnixTime(0), resultObjects[0].TimeStamp);
                Assert.AreEqual(9034, resultObjects[0].UserId);
                Assert.AreEqual(string.Empty, resultObjects[0].UserName);
                Assert.AreEqual(12, resultObjects[0].Version);
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
                Longitude = 1.2f,
                ChangeSetId = 1092,
                TimeStamp = DateTime.UnixEpoch,
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

            sourceObjects = new OsmGeo[] {
                new Node()
                {
                    Id = 1,
                    Latitude = 1f,
                    Longitude = 1f,
                    ChangeSetId = 1092,
                    TimeStamp = DateTime.UnixEpoch,
                    UserId = 9034,
                    Version = 12
                },
                new Node()
                {
                    Id = 2,
                    Latitude = 2f,
                    Longitude = 2f,
                    ChangeSetId = 1093,
                    TimeStamp = DateTime.Now,
                    UserId = 9035,
                    Version = 13
                },
                new Node()
                {
                    Id = 3,
                    Latitude = 3f,
                    Longitude = 3f,
                    ChangeSetId = 1094,
                    TimeStamp = DateTime.UtcNow,
                    UserId = 9036,
                    Version = 14
                }
            };

            // build PBF stream target.
            using (var stream = new MemoryStream())
            {
                var target = new PBFOsmStreamTarget(stream);
                target.RegisterSource(sourceObjects);
                target.Pull();

                stream.Seek(0, SeekOrigin.Begin);
                var resultObjects = new List<OsmGeo>(new PBFOsmStreamSource(stream));

                AreEqual(sourceObjects, resultObjects);
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

            // build PBF stream target.
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

            // build PBF stream target.
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

            // build PBF stream target.
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

            // build PBF stream target.
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

            // build PBF stream target.
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

            // build PBF stream target.
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

                Assert.AreEqual(1715, new PBFOsmStreamSource(memoryStream).Count(n => n is Node));
            }
        }


        /// <summary>
        /// Test helper to test if 2 OsmGeo arrays are equal.
        /// </summary>
        private static void AreEqual(IEnumerable<OsmGeo> sourceObjects, IEnumerable<OsmGeo> resultObjects)
        {
            Assert.IsNotNull(resultObjects);
            var sourceArray = sourceObjects.ToArray();
            var resultArray = resultObjects.ToArray();
            Assert.AreEqual(sourceArray.Length, resultArray.Length);
            for (int i = 0; i < sourceArray.Length; i++)
            {
                AreEqual(sourceArray[i], resultArray[i]);
            }
        }

        /// <summary>
        /// Test helper to test if 2 OsmGeos are equal.
        /// </summary>
        private static void AreEqual(OsmGeo sourceObject, OsmGeo resultObject)
        {
            Assert.AreEqual(sourceObject.Id, resultObject.Id);
            Assert.AreEqual(sourceObject.ChangeSetId, resultObject.ChangeSetId);
            Assert.AreEqual(sourceObject.TimeStamp.Value.Ticks, resultObject.TimeStamp.Value.Ticks, 10000000);
            Assert.AreEqual(sourceObject.UserId, resultObject.UserId);
            Assert.AreEqual(sourceObject.UserName, resultObject.UserName);
            Assert.AreEqual(sourceObject.Version, resultObject.Version);
            Assert.AreEqual(sourceObject.Tags?.Count ?? 0, resultObject.Tags?.Count ?? 0);
            foreach (var sourceTag in sourceObject.Tags ?? new TagsCollection())
            {
                Assert.IsTrue(resultObject.Tags.Contains(sourceTag));
            }
            switch (sourceObject)
            {
                case Node sourceNode:
                    var resultNode = (Node)resultObject;
                    Assert.AreEqual(sourceNode.Latitude.Value, resultNode.Latitude.Value, .0001f);
                    Assert.AreEqual(sourceNode.Longitude.Value, resultNode.Longitude.Value, .0001f);
                    break;
                case Way sourceWay:
                    var resultWay = (Way)resultObject;
                    Assert.IsTrue(sourceWay.Nodes.SequenceEqual(resultWay.Nodes));
                    break;
                case Relation sourceRelation:
                    var resultRelation = (Relation)resultObject;
                    Assert.AreEqual(sourceRelation.Members.Length, resultRelation.Members.Length);
                    for (int i = 0; i < sourceRelation.Members.Length; i++)
                    {
                        Assert.AreEqual(sourceRelation.Members[i].Type, resultRelation.Members[i].Type);
                        Assert.AreEqual(sourceRelation.Members[i].Id, resultRelation.Members[i].Id);
                        Assert.AreEqual(sourceRelation.Members[i].Role, resultRelation.Members[i].Role);
                    }
                    break;
            }
        }

    }
}