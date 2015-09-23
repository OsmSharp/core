// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using NUnit.Framework;
using OsmSharp.Collections.Tags;
using OsmSharp.Osm;
using OsmSharp.Osm.PBF.Streams;
using OsmSharp.Osm.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OsmSharp.Test.Osm.PBF.Streams
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
            var sourceNode = Node.Create(1, 1.1, 1.2);
            var sourceObjects = new OsmGeo[] {
                sourceNode
            };
            var source = sourceObjects.ToOsmStreamSource();

            // build pfb stream target.
            using(var stream = new MemoryStream())
            {
                var target = new PBFOsmStreamTarget(stream);
                target.RegisterSource(source);
                target.Pull();

                stream.Seek(0, SeekOrigin.Begin);
                var resultObjects = new List<OsmGeo>(new PBFOsmStreamSource(stream));

                Assert.IsNotNull(resultObjects);
                Assert.AreEqual(1, resultObjects.Count);

                Assert.AreEqual(sourceObjects[0].Id, resultObjects[0].Id);
                Assert.AreEqual(0, resultObjects[0].ChangeSetId);
                Assert.AreEqual(Utilities.FromUnixTime(0), resultObjects[0].TimeStamp);
                Assert.AreEqual(0, resultObjects[0].UserId);
                Assert.AreEqual(sourceObjects[0].UserName, resultObjects[0].UserName);
                Assert.AreEqual(0, resultObjects[0].Version);

                var resultNode = resultObjects[0] as Node;
                Assert.AreEqual(sourceNode.Latitude.Value, resultNode.Latitude.Value, .0001f);
                Assert.AreEqual(sourceNode.Longitude.Value, resultNode.Longitude.Value, .0001f);
            }

            // build source stream.
            sourceNode = Node.Create(1, 1.1, 1.2);
            sourceNode.Tags = new TagsCollection();
            sourceNode.Tags.Add("highway", "residential");
            sourceObjects = new OsmGeo[] {
                sourceNode
            };
            source = sourceObjects.ToOsmStreamSource();

            // build pfb stream target.
            using (var stream = new MemoryStream())
            {
                var target = new PBFOsmStreamTarget(stream);
                target.RegisterSource(source);
                target.Pull();

                stream.Seek(0, SeekOrigin.Begin);
                var resultObjects = new List<OsmGeo>(new PBFOsmStreamSource(stream));

                Assert.IsNotNull(resultObjects);
                Assert.AreEqual(1, resultObjects.Count);

                Assert.AreEqual(sourceObjects[0].Id, resultObjects[0].Id);
                Assert.AreEqual(0, resultObjects[0].ChangeSetId);
                Assert.AreEqual(Utilities.FromUnixTime(0), resultObjects[0].TimeStamp);
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
            sourceNode = Node.Create(1, 1.1, 1.2);
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
            source = sourceObjects.ToOsmStreamSource();

            // build pfb stream target.
            using (var stream = new MemoryStream())
            {
                var target = new PBFOsmStreamTarget(stream);
                target.RegisterSource(source);
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
            var sourceWay = Way.Create(1, 1, 2);
            var sourceObjects = new OsmGeo[] {
                sourceWay
            };

            // build pfb stream target.
            using (var stream = new MemoryStream())
            {
                var target = new PBFOsmStreamTarget(stream);
                target.RegisterSource(sourceObjects.ToOsmStreamSource());
                target.Pull();

                stream.Seek(0, SeekOrigin.Begin);
                var resultObjects = new List<OsmGeo>(new PBFOsmStreamSource(stream));

                Assert.IsNotNull(resultObjects);
                Assert.AreEqual(1, resultObjects.Count);

                Assert.AreEqual(sourceObjects[0].Id, resultObjects[0].Id);
                Assert.AreEqual(0, resultObjects[0].ChangeSetId);
                Assert.AreEqual(Utilities.FromUnixTime(0), resultObjects[0].TimeStamp);
                Assert.AreEqual(0, resultObjects[0].UserId);
                Assert.AreEqual(sourceObjects[0].UserName, resultObjects[0].UserName);
                Assert.AreEqual(0, resultObjects[0].Version);

                var resultWay = resultObjects[0] as Way;
                Assert.AreEqual(sourceWay.Nodes.Count, resultWay.Nodes.Count);
                Assert.AreEqual(sourceWay.Nodes[0], resultWay.Nodes[0]);
                Assert.AreEqual(sourceWay.Nodes[1], resultWay.Nodes[1]);
            }

            // build source stream.
            sourceWay = Way.Create(1, 1, 2);
            sourceWay.Tags = new TagsCollection();
            sourceWay.Tags.Add("highway", "residential");
            sourceObjects = new OsmGeo[] {
                sourceWay
            };

            // build pfb stream target.
            using (var stream = new MemoryStream())
            {
                var target = new PBFOsmStreamTarget(stream);
                target.RegisterSource(sourceObjects.ToOsmStreamSource());
                target.Pull();

                stream.Seek(0, SeekOrigin.Begin);
                var resultObjects = new List<OsmGeo>(new PBFOsmStreamSource(stream));

                Assert.IsNotNull(resultObjects);
                Assert.AreEqual(1, resultObjects.Count);

                Assert.AreEqual(sourceObjects[0].Id, resultObjects[0].Id);
                Assert.AreEqual(0, resultObjects[0].ChangeSetId);
                Assert.AreEqual(Utilities.FromUnixTime(0), resultObjects[0].TimeStamp);
                Assert.AreEqual(0, resultObjects[0].UserId);
                Assert.AreEqual(string.Empty, resultObjects[0].UserName);
                Assert.AreEqual(0, resultObjects[0].Version);
                Assert.AreEqual(sourceObjects[0].Tags.Count, resultObjects[0].Tags.Count);
                Assert.IsTrue(resultObjects[0].Tags.Contains(sourceObjects[0].Tags.First<Tag>()));

                var resultWay = resultObjects[0] as Way;
                Assert.AreEqual(sourceWay.Nodes.Count, resultWay.Nodes.Count);
                Assert.AreEqual(sourceWay.Nodes[0], resultWay.Nodes[0]);
                Assert.AreEqual(sourceWay.Nodes[1], resultWay.Nodes[1]);
            }

            // build source stream.
            sourceWay = Way.Create(1, 1, 2);
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
                target.RegisterSource(sourceObjects.ToOsmStreamSource());
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
                Assert.AreEqual(sourceWay.Nodes.Count, resultWay.Nodes.Count);
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
            var sourceRelation = Relation.Create(1, new RelationMember()
                {
                    MemberId = 1,
                    MemberRole = "fake role",
                    MemberType = OsmGeoType.Node
                }, new RelationMember()
                {
                    MemberId = 1,
                    MemberRole = "fake role",
                    MemberType = OsmGeoType.Way
                });
            var sourceObjects = new OsmGeo[] {
                sourceRelation
            };
            var source = sourceObjects.ToOsmStreamSource();

            // build pfb stream target.
            using (var stream = new MemoryStream())
            {
                var target = new PBFOsmStreamTarget(stream);
                target.RegisterSource(source);
                target.Pull();

                stream.Seek(0, SeekOrigin.Begin);
                var resultObjects = new List<OsmGeo>(new PBFOsmStreamSource(stream));

                Assert.IsNotNull(resultObjects);
                Assert.AreEqual(1, resultObjects.Count);

                Assert.AreEqual(sourceObjects[0].Id, resultObjects[0].Id);
                Assert.AreEqual(0, resultObjects[0].ChangeSetId);
                Assert.AreEqual(Utilities.FromUnixTime(0), resultObjects[0].TimeStamp);
                Assert.AreEqual(0, resultObjects[0].UserId);
                Assert.AreEqual(string.Empty, resultObjects[0].UserName);
                Assert.AreEqual(0, resultObjects[0].Version);

                var resultRelation = resultObjects[0] as Relation;
                Assert.AreEqual(sourceRelation.Members.Count, resultRelation.Members.Count);
                Assert.AreEqual(sourceRelation.Members[0].MemberId, resultRelation.Members[0].MemberId);
                Assert.AreEqual(sourceRelation.Members[0].MemberRole, resultRelation.Members[0].MemberRole);
                Assert.AreEqual(sourceRelation.Members[0].MemberType, resultRelation.Members[0].MemberType);
                Assert.AreEqual(sourceRelation.Members[1].MemberId, resultRelation.Members[1].MemberId);
                Assert.AreEqual(sourceRelation.Members[1].MemberRole, resultRelation.Members[1].MemberRole);
                Assert.AreEqual(sourceRelation.Members[1].MemberType, resultRelation.Members[1].MemberType);
            }

            // build source stream.
            sourceRelation = Relation.Create(1, new RelationMember()
                {
                    MemberId = 1,
                    MemberRole = "fake role",
                    MemberType = OsmGeoType.Node
                }, new RelationMember()
                {
                    MemberId = 1,
                    MemberRole = "fake role",
                    MemberType = OsmGeoType.Way
                });
            sourceRelation.Tags = new TagsCollection();
            sourceRelation.Tags.Add("highway", "residential");
            sourceObjects = new OsmGeo[] {
                sourceRelation
            };

            // build pfb stream target.
            using (var stream = new MemoryStream())
            {
                var target = new PBFOsmStreamTarget(stream);
                target.RegisterSource(sourceObjects.ToOsmStreamSource());
                target.Pull();

                stream.Seek(0, SeekOrigin.Begin);
                var resultObjects = new List<OsmGeo>(new PBFOsmStreamSource(stream));

                Assert.IsNotNull(resultObjects);
                Assert.AreEqual(1, resultObjects.Count);

                Assert.AreEqual(sourceObjects[0].Id, resultObjects[0].Id);
                Assert.AreEqual(0, resultObjects[0].ChangeSetId);
                Assert.AreEqual(Utilities.FromUnixTime(0), resultObjects[0].TimeStamp);
                Assert.AreEqual(0, resultObjects[0].UserId);
                Assert.AreEqual(string.Empty, resultObjects[0].UserName);
                Assert.AreEqual(0, resultObjects[0].Version);
                Assert.AreEqual(sourceObjects[0].Tags.Count, resultObjects[0].Tags.Count);
                Assert.IsTrue(resultObjects[0].Tags.Contains(sourceObjects[0].Tags.First<Tag>()));

                var resultRelation = resultObjects[0] as Relation;
                Assert.AreEqual(sourceRelation.Members.Count, resultRelation.Members.Count);
                Assert.AreEqual(sourceRelation.Members[0].MemberId, resultRelation.Members[0].MemberId);
                Assert.AreEqual(sourceRelation.Members[0].MemberRole, resultRelation.Members[0].MemberRole);
                Assert.AreEqual(sourceRelation.Members[0].MemberType, resultRelation.Members[0].MemberType);
                Assert.AreEqual(sourceRelation.Members[1].MemberId, resultRelation.Members[1].MemberId);
                Assert.AreEqual(sourceRelation.Members[1].MemberRole, resultRelation.Members[1].MemberRole);
                Assert.AreEqual(sourceRelation.Members[1].MemberType, resultRelation.Members[1].MemberType);
            }

            // build source stream.
            sourceRelation = Relation.Create(1, new RelationMember()
                {
                    MemberId = 1,
                    MemberRole = "fake role",
                    MemberType = OsmGeoType.Node
                }, new RelationMember()
                {
                    MemberId = 1,
                    MemberRole = "fake role",
                    MemberType = OsmGeoType.Way
                });
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
                target.RegisterSource(sourceObjects.ToOsmStreamSource());
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
                Assert.AreEqual(sourceRelation.Members.Count, resultRelation.Members.Count);
                Assert.AreEqual(sourceRelation.Members[0].MemberId, resultRelation.Members[0].MemberId);
                Assert.AreEqual(sourceRelation.Members[0].MemberRole, resultRelation.Members[0].MemberRole);
                Assert.AreEqual(sourceRelation.Members[0].MemberType, resultRelation.Members[0].MemberType);
                Assert.AreEqual(sourceRelation.Members[1].MemberId, resultRelation.Members[1].MemberId);
                Assert.AreEqual(sourceRelation.Members[1].MemberRole, resultRelation.Members[1].MemberRole);
                Assert.AreEqual(sourceRelation.Members[1].MemberType, resultRelation.Members[1].MemberType);
            }
        }

        /// <summary>
        /// Tests writing a stream of different objects.
        /// </summary>
        [Test]
        public void TestWriteMix()
        {
            var sourceNode = Node.Create(1, 1.1, 1.2);
            sourceNode.Tags = new TagsCollection();
            sourceNode.Tags.Add("highway", "residential");
            sourceNode.ChangeSetId = 1;
            sourceNode.TimeStamp = DateTime.Now;
            sourceNode.UserId = 1;
            sourceNode.UserName = "ben";
            sourceNode.Version = 3;
            sourceNode.Visible = true;

            var sourceWay = Way.Create(1, 1, 2);
            sourceWay.Tags = new TagsCollection();
            sourceWay.Tags.Add("highway", "residential");
            sourceWay.ChangeSetId = 1;
            sourceWay.TimeStamp = DateTime.Now;
            sourceWay.UserId = 1;
            sourceWay.UserName = "ben";
            sourceWay.Version = 3;
            sourceWay.Visible = true;

            var sourceRelation = Relation.Create(1, new RelationMember()
            {
                MemberId = 1,
                MemberRole = "fake role",
                MemberType = OsmGeoType.Node
            }, new RelationMember()
            {
                MemberId = 1,
                MemberRole = "fake role",
                MemberType = OsmGeoType.Way
            });
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
                target.RegisterSource(sourceObjects.ToOsmStreamSource());
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
                Assert.AreEqual(sourceWay.Nodes.Count, resultWay.Nodes.Count);
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
                Assert.AreEqual(sourceRelation.Members.Count, resultRelation.Members.Count);
                Assert.AreEqual(sourceRelation.Members[0].MemberId, resultRelation.Members[0].MemberId);
                Assert.AreEqual(sourceRelation.Members[0].MemberRole, resultRelation.Members[0].MemberRole);
                Assert.AreEqual(sourceRelation.Members[0].MemberType, resultRelation.Members[0].MemberType);
                Assert.AreEqual(sourceRelation.Members[1].MemberId, resultRelation.Members[1].MemberId);
                Assert.AreEqual(sourceRelation.Members[1].MemberRole, resultRelation.Members[1].MemberRole);
                Assert.AreEqual(sourceRelation.Members[1].MemberType, resultRelation.Members[1].MemberType);
            }
        }
    }
}