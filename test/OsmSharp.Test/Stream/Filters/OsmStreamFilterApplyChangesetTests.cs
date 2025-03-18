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
            Assert.That(result.Count, Is.EqualTo(7));
            Assert.That(result[0].Id, Is.EqualTo(1));
            Assert.That(result[0].Type, Is.EqualTo(OsmGeoType.Node));
            Assert.That(result[1].Id, Is.EqualTo(2));
            Assert.That(result[1].Type, Is.EqualTo(OsmGeoType.Node));
            Assert.That(result[2].Id, Is.EqualTo(3));
            Assert.That(result[2].Type, Is.EqualTo(OsmGeoType.Node));

            Assert.That(result[3].Id, Is.EqualTo(2));
            Assert.That(result[3].Type, Is.EqualTo(OsmGeoType.Way));
            Assert.That(result[4].Id, Is.EqualTo(3));
            Assert.That(result[4].Type, Is.EqualTo(OsmGeoType.Way));

            Assert.That(result[5].Id, Is.EqualTo(3));
            Assert.That(result[5].Type, Is.EqualTo(OsmGeoType.Relation));
            Assert.That(result[6].Id, Is.EqualTo(4));
            Assert.That(result[6].Type, Is.EqualTo(OsmGeoType.Relation));
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
            Assert.That(result.Count, Is.EqualTo(7));
            Assert.That(result[0].Id, Is.EqualTo(1));
            Assert.That(result[0].Version, Is.EqualTo(1));
            Assert.That(result[0].Type, Is.EqualTo(OsmGeoType.Node));
            Assert.That(result[1].Id, Is.EqualTo(2));
            Assert.That(result[1].Version, Is.EqualTo(1));
            Assert.That(result[1].Type, Is.EqualTo(OsmGeoType.Node));
            Assert.That(result[2].Id, Is.EqualTo(3));
            Assert.That(result[2].Version, Is.EqualTo(2));
            Assert.That(result[2].Type, Is.EqualTo(OsmGeoType.Node));

            Assert.That(result[3].Id, Is.EqualTo(2));
            Assert.That(result[3].Version, Is.EqualTo(2));
            Assert.That(result[3].Type, Is.EqualTo(OsmGeoType.Way));
            Assert.That(result[4].Id, Is.EqualTo(3));
            Assert.That(result[4].Version, Is.EqualTo(1));
            Assert.That(result[4].Type, Is.EqualTo(OsmGeoType.Way));

            Assert.That(result[5].Id, Is.EqualTo(3));
            Assert.That(result[5].Version, Is.EqualTo(1));
            Assert.That(result[5].Type, Is.EqualTo(OsmGeoType.Relation));
            Assert.That(result[6].Id, Is.EqualTo(4));
            Assert.That(result[6].Version, Is.EqualTo(2));
            Assert.That(result[6].Type, Is.EqualTo(OsmGeoType.Relation));
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
            Assert.That(result.Count, Is.EqualTo(4));
            Assert.That(result[0].Id, Is.EqualTo(1));
            Assert.That(result[0].Version, Is.EqualTo(1));
            Assert.That(result[0].Type, Is.EqualTo(OsmGeoType.Node));
            Assert.That(result[1].Id, Is.EqualTo(2));
            Assert.That(result[1].Version, Is.EqualTo(1));
            Assert.That(result[1].Type, Is.EqualTo(OsmGeoType.Node));
            
            Assert.That(result[2].Id, Is.EqualTo(3));
            Assert.That(result[2].Version, Is.EqualTo(1));
            Assert.That(result[2].Type, Is.EqualTo(OsmGeoType.Way));

            Assert.That(result[3].Id, Is.EqualTo(3));
            Assert.That(result[3].Version, Is.EqualTo(1));
            Assert.That(result[3].Type, Is.EqualTo(OsmGeoType.Relation));
        }
    }
}