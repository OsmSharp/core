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
using OsmSharp.Changesets;
using OsmSharp.Db;
using OsmSharp.Db.Impl;

namespace OsmSharp.Test.Db
{
    /// <summary>
    /// Contains tests for the history db.
    /// </summary>
    [TestFixture]
    public class HistoryDbTests
    { 
        /// <summary>
        /// Tests applying a changeset that contains a new way with new nodes.
        /// </summary>
        [Test]
        public void TestApplyChangesetNewNodesInNewWay()
        {
            var historyDb = new HistoryDb(new MemoryHistoryDb());
            historyDb.Add(new OsmGeo[] {
                new Node()
                {
                    Id = 1,
                    Version = 1
                },
                new Node()
                {
                    Id = 2,
                    Version = 1
                }
            });

            var osmChange = new OsmChange();
            osmChange.Create = new OsmGeo[]
            {
                new Node()
                {
                    Id = -1,
                    Version = 1
                },
                new Node()
                {
                    Id = -2,
                    Version = 1
                },
                new Way()
                {
                    Id = -1,
                    Version = 1,
                    Nodes = new long[]
                    {
                        1, -1, -2, 2
                    }
                }
            };

            var results = historyDb.ApplyChangeset(1, osmChange);

            Assert.That(results, Is.Not.Null);
            Assert.That(results.Result, Is.Not.Null);
            Assert.That(results.Result.Results, Is.Not.Null);
            Assert.That(results.Result.Results.Length, Is.EqualTo(3));
            var result = results.Result.Results[0];
            Assert.IsInstanceOf<NodeResult>(result);
            Assert.That(result.NewId, Is.EqualTo(3));
            Assert.That(result.OldId, Is.EqualTo(-1));
            Assert.That(result.NewVersion, Is.EqualTo(1));
            result = results.Result.Results[1];
            Assert.IsInstanceOf<NodeResult>(result);
            Assert.That(result.NewId, Is.EqualTo(4));
            Assert.That(result.OldId, Is.EqualTo(-2));
            Assert.That(result.NewVersion, Is.EqualTo(1));
            result = results.Result.Results[2];
            Assert.IsInstanceOf<WayResult>(result);
            Assert.That(result.NewId, Is.EqualTo(1));
            Assert.That(result.OldId, Is.EqualTo(-1));
            Assert.That(result.NewVersion, Is.EqualTo(1));

            var way = historyDb.Get(OsmGeoType.Way, 1) as Way;
            Assert.That(way, Is.Not.Null);
            Assert.That(way.Nodes, Is.Not.Null);
            Assert.That(way.Nodes.Length, Is.EqualTo(4));
            Assert.That(way.Nodes[0], Is.EqualTo(1));
            Assert.That(way.Nodes[1], Is.EqualTo(3));
            Assert.That(way.Nodes[2], Is.EqualTo(4));
            Assert.That(way.Nodes[3], Is.EqualTo(2));
        }

        /// <summary>
        /// Tests applying a changeset that contains a modified way with new nodes.
        /// </summary>
        [Test]
        public void TestApplyChangesetNewNodesInModifiedWay()
        {
            var historyDb = new HistoryDb(new MemoryHistoryDb());
            historyDb.Add(new OsmGeo[] {
                new Node()
                {
                    Id = 1,
                    Version = 1
                },
                new Node()
                {
                    Id = 2,
                    Version = 1
                },
                new Way()
                {
                    Id = 1,
                    Version = 1,
                    Nodes = new long[]
                    {
                        1, 2
                    }
                }
            });

            var osmChange = new OsmChange();
            osmChange.Create = new OsmGeo[]
            {
                new Node()
                {
                    Id = -1,
                    Version = 1
                },
                new Node()
                {
                    Id = -2,
                    Version = 1
                }
            };
            osmChange.Modify = new OsmGeo[]
            {
                new Way()
                {
                    Id = 1,
                    Version = 1,
                    Nodes = new long[]
                    {
                        1, -1, -2, 2
                    }
                }
            };

            var results = historyDb.ApplyChangeset(1, osmChange);

            Assert.That(results, Is.Not.Null);
            Assert.That(results.Result, Is.Not.Null);
            Assert.That(results.Result.Results, Is.Not.Null);
            Assert.That(results.Result.Results.Length, Is.EqualTo(3));
            var result = results.Result.Results[0];
            Assert.IsInstanceOf<NodeResult>(result);
            Assert.That(result.NewId, Is.EqualTo(3));
            Assert.That(result.OldId, Is.EqualTo(-1));
            Assert.That(result.NewVersion, Is.EqualTo(1));
            result = results.Result.Results[1];
            Assert.IsInstanceOf<NodeResult>(result);
            Assert.That(result.NewId, Is.EqualTo(4));
            Assert.That(result.OldId, Is.EqualTo(-2));
            Assert.That(result.NewVersion, Is.EqualTo(1));
            result = results.Result.Results[2];
            Assert.IsInstanceOf<WayResult>(result);
            Assert.That(result.NewId, Is.EqualTo(1));
            Assert.That(result.OldId, Is.EqualTo(1));
            Assert.That(result.NewVersion, Is.EqualTo(2));

            var way = historyDb.Get(OsmGeoType.Way, 1) as Way;
            Assert.That(way, Is.Not.Null);
            Assert.That(way.Nodes, Is.Not.Null);
            Assert.That(way.Nodes.Length, Is.EqualTo(4));
            Assert.That(way.Nodes[0], Is.EqualTo(1));
            Assert.That(way.Nodes[1], Is.EqualTo(3));
            Assert.That(way.Nodes[2], Is.EqualTo(4));
            Assert.That(way.Nodes[3], Is.EqualTo(2));
        }
    }
}