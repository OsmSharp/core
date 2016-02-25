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
using OsmSharp.Streams.Filters;
using System.Linq;
using System.Collections.Generic;

namespace OsmSharp.Test.Stream.Filters
{
    /// <summary>
    /// A merge filter tests.
    /// </summary>
    [TestFixture]
    public class OsmStreamFilterMergeTests
    {
        /// <summary>
        /// Tests merging nodes.
        /// </summary>
        [Test]
        public void TestMergeNodes()
        {
            var stream1 = new OsmGeo[]
            {
                new Node()
                {
                    Id = 1
                }
            };
            var stream2 = new OsmGeo[]
            {
                new Node()
                {
                    Id = 2
                }
            };

            var merge = new OsmStreamFilterMerge();
            merge.RegisterSource(stream1);
            merge.RegisterSource(stream2);

            var result = new List<OsmGeo>(merge);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual(2, result[1].Id);
        }

        /// <summary>
        /// Tests merging ways.
        /// </summary>
        [Test]
        public void TestMergeWays()
        {
            var stream1 = new OsmGeo[]
            {
                new Way()
                {
                    Id = 1
                }
            };
            var stream2 = new OsmGeo[]
            {
                new Way()
                {
                    Id = 2
                }
            };

            var merge = new OsmStreamFilterMerge();
            merge.RegisterSource(stream1);
            merge.RegisterSource(stream2);

            var result = new List<OsmGeo>(merge);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual(2, result[1].Id);
        }

        /// <summary>
        /// Tests merging relations.
        /// </summary>
        [Test]
        public void TestMergeRelations()
        {
            var stream1 = new OsmGeo[]
            {
                new Relation()
                {
                    Id = 1
                }
            };
            var stream2 = new OsmGeo[]
            {
                new Relation()
                {
                    Id = 2
                }
            };

            var merge = new OsmStreamFilterMerge();
            merge.RegisterSource(stream1);
            merge.RegisterSource(stream2);

            var result = new List<OsmGeo>(merge);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual(2, result[1].Id);
        }

        /// <summary>
        /// Tests not sorted.
        /// </summary>
        [Test]
        public void TestMergeNotSorted()
        {
            var stream1 = new OsmGeo[]
            {
                new Node()
                {
                    Id = 1
                }
            };
            var stream2 = new OsmGeo[]
            {
                new Node()
                {
                    Id = 4
                },
                new Node()
                {
                    Id = 2
                }
            };

            var merge = new OsmStreamFilterMerge();
            merge.RegisterSource(stream1);
            merge.RegisterSource(stream2);

            Assert.Catch<Streams.Exceptions.StreamNotSortedException>(() =>
            {
                merge.ToList();
            });
        }

        /// <summary>
        /// Test merging with an empty stream.
        /// </summary>
        [Test]
        public void TestMergeOneEmpty()
        {
            var stream1 = new OsmGeo[0];
            var stream2 = new OsmGeo[]
            {
                new Node()
                {
                    Id = 1
                },
                new Way()
                {
                    Id = 1
                },
                new Relation()
                {
                    Id = 1
                }
            };

            var merge = new OsmStreamFilterMerge();
            merge.RegisterSource(stream1);
            merge.RegisterSource(stream2);

            var result = new List<OsmGeo>(merge);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual(OsmGeoType.Node, result[0].Type);
            Assert.AreEqual(1, result[1].Id);
            Assert.AreEqual(OsmGeoType.Way, result[1].Type);
            Assert.AreEqual(1, result[2].Id);
            Assert.AreEqual(OsmGeoType.Relation, result[2].Type);
        }

        /// <summary>
        /// Tests merging a conflict.
        /// </summary>
        [Test]
        public void TestMergeConflict()
        {
            var stream1 = new OsmGeo[]
            {
                new Node()
                {
                    Id = 1,
                    Version = 1
                },
                new Way()
                {
                    Id = 1,
                    Version = 1
                },
                new Relation()
                {
                    Id = 1,
                    Version = 1
                }
            };
            var stream2 = new OsmGeo[]
            {
                new Node()
                {
                    Id = 1
                },
                new Node()
                {
                    Id = 2
                },
                new Way()
                {
                    Id = 1
                },
                new Way()
                {
                    Id = 2
                },
                new Relation()
                {
                    Id = 1
                },
                new Relation()
                {
                    Id = 2
                }
            };

            var merge = new OsmStreamFilterMerge();
            merge.RegisterSource(stream1);
            merge.RegisterSource(stream2);

            var result = new List<OsmGeo>(merge);
            Assert.AreEqual(6, result.Count);

            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual(1, result[0].Version);
            Assert.AreEqual(OsmGeoType.Node, result[0].Type);
            Assert.AreEqual(2, result[1].Id);
            Assert.AreEqual(OsmGeoType.Node, result[1].Type);

            Assert.AreEqual(1, result[2].Id);
            Assert.AreEqual(1, result[2].Version);
            Assert.AreEqual(OsmGeoType.Way, result[2].Type);
            Assert.AreEqual(2, result[3].Id);
            Assert.AreEqual(OsmGeoType.Way, result[3].Type);

            Assert.AreEqual(1, result[4].Id);
            Assert.AreEqual(1, result[4].Version);
            Assert.AreEqual(OsmGeoType.Relation, result[4].Type);
            Assert.AreEqual(2, result[5].Id);
            Assert.AreEqual(OsmGeoType.Relation, result[5].Type);
        }
    }
}