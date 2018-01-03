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
using OsmSharp.Changesets;

namespace OsmSharp.Test.Stream.Filters
{
    /// <summary>
    /// Contains apply changesets filter tests.
    /// </summary>
    [TestFixture]
    public class OsmStreamFilterApplyChangesetTests
    {
        /// <summary>
        /// Tests creating objects.
        /// </summary>
        [Test]
        public void TestCreate()
        {
            var source = new OsmGeo[]
            {
                new Node()
                {
                    Id = 2,
                    Version = 1
                },
                new Way()
                {
                    Id = 3,
                    Version = 2
                },
                new Relation()
                {
                    Id = 3,
                    Version = 2
                }
            };
            var changeset = new OsmChange()
            {
                Create = new OsmGeo[]
                {
                    new Node()
                    {
                        Id = 1,
                        Version = 1
                    },
                    new Node()
                    {
                        Id = 3,
                        Version = 1
                    },
                    new Way()
                    {
                        Id = 2,
                        Version = 1
                    },
                    new Relation()
                    {
                        Id = 4,
                        Version = 1
                    }
                }
            };

            var filter = new OsmStreamFilterApplyChangeset(changeset);
            filter.RegisterSource(source);

            var result = new List<OsmGeo>(filter);
            Assert.AreEqual(7, result.Count);
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual(OsmGeoType.Node, result[0].Type);
            Assert.AreEqual(2, result[1].Id);
            Assert.AreEqual(OsmGeoType.Node, result[1].Type);
            Assert.AreEqual(3, result[2].Id);
            Assert.AreEqual(OsmGeoType.Node, result[2].Type);

            Assert.AreEqual(2, result[3].Id);
            Assert.AreEqual(OsmGeoType.Way, result[3].Type);
            Assert.AreEqual(3, result[4].Id);
            Assert.AreEqual(OsmGeoType.Way, result[4].Type);

            Assert.AreEqual(3, result[5].Id);
            Assert.AreEqual(OsmGeoType.Relation, result[5].Type);
            Assert.AreEqual(4, result[6].Id);
            Assert.AreEqual(OsmGeoType.Relation, result[6].Type);
        }

        /// <summary>
        /// Tests modifying objects.
        /// </summary>
        [Test]
        public void TestModify()
        {
            var source = new OsmGeo[]
            {
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
                new Node()
                {
                    Id = 3,
                    Version = 1
                },
                new Way()
                {
                    Id = 2,
                    Version = 1
                },
                new Way()
                {
                    Id = 3,
                    Version = 1
                },
                new Relation()
                {
                    Id = 3,
                    Version = 1
                },
                new Relation()
                {
                    Id = 4,
                    Version = 1
                }
            };
            var changeset = new OsmChange()
            {
                Modify = new OsmGeo[]
                {
                    new Node()
                    {
                        Id = 3,
                        Version = 2
                    },
                    new Way()
                    {
                        Id = 2,
                        Version = 2
                    },
                    new Relation()
                    {
                        Id = 4,
                        Version = 2
                    }
                }
            };

            var filter = new OsmStreamFilterApplyChangeset(changeset);
            filter.RegisterSource(source);

            var result = new List<OsmGeo>(filter);
            Assert.AreEqual(7, result.Count);
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual(1, result[0].Version);
            Assert.AreEqual(OsmGeoType.Node, result[0].Type);
            Assert.AreEqual(2, result[1].Id);
            Assert.AreEqual(1, result[1].Version);
            Assert.AreEqual(OsmGeoType.Node, result[1].Type);
            Assert.AreEqual(3, result[2].Id);
            Assert.AreEqual(2, result[2].Version);
            Assert.AreEqual(OsmGeoType.Node, result[2].Type);

            Assert.AreEqual(2, result[3].Id);
            Assert.AreEqual(2, result[3].Version);
            Assert.AreEqual(OsmGeoType.Way, result[3].Type);
            Assert.AreEqual(3, result[4].Id);
            Assert.AreEqual(1, result[4].Version);
            Assert.AreEqual(OsmGeoType.Way, result[4].Type);

            Assert.AreEqual(3, result[5].Id);
            Assert.AreEqual(1, result[5].Version);
            Assert.AreEqual(OsmGeoType.Relation, result[5].Type);
            Assert.AreEqual(4, result[6].Id);
            Assert.AreEqual(2, result[6].Version);
            Assert.AreEqual(OsmGeoType.Relation, result[6].Type);
        }

        /// <summary>
        /// Tests deleting objects.
        /// </summary>
        [Test]
        public void TestDelete()
        {
            var source = new OsmGeo[]
            {
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
                new Node()
                {
                    Id = 3,
                    Version = 1
                },
                new Way()
                {
                    Id = 2,
                    Version = 1
                },
                new Way()
                {
                    Id = 3,
                    Version = 1
                },
                new Relation()
                {
                    Id = 3,
                    Version = 1
                },
                new Relation()
                {
                    Id = 4,
                    Version = 1
                }
            };
            var changeset = new OsmChange()
            {
                Delete = new OsmGeo[]
                {
                    new Node()
                    {
                        Id = 3,
                        Version = 1
                    },
                    new Way()
                    {
                        Id = 2,
                        Version = 1
                    },
                    new Relation()
                    {
                        Id = 4,
                        Version = 1
                    }
                }
            };

            var filter = new OsmStreamFilterApplyChangeset(changeset);
            filter.RegisterSource(source);

            var result = new List<OsmGeo>(filter);
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual(1, result[0].Version);
            Assert.AreEqual(OsmGeoType.Node, result[0].Type);
            Assert.AreEqual(2, result[1].Id);
            Assert.AreEqual(1, result[1].Version);
            Assert.AreEqual(OsmGeoType.Node, result[1].Type);
            
            Assert.AreEqual(3, result[2].Id);
            Assert.AreEqual(1, result[2].Version);
            Assert.AreEqual(OsmGeoType.Way, result[2].Type);

            Assert.AreEqual(3, result[3].Id);
            Assert.AreEqual(1, result[3].Version);
            Assert.AreEqual(OsmGeoType.Relation, result[3].Type);
        }
    }
}