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
using OsmSharp.Changesets;

namespace OsmSharp.Test.Changesets
{
    /// <summary>
    /// Contains tests for the OsmChange extensions.
    /// </summary>
    [TestFixture]
    public class OsmChangeExtensionsTests
    {
        /// <summary>
        /// Tests squashing a single changeset.
        /// </summary>
        [Test]
        public void TestSquashOneSet()
        {
            var changeset = new OsmChange()
            {
                Create = new OsmGeo[]
                {
                    new Node()
                    {
                        Id = 1,
                        Version = 1
                    }
                },
                Delete = new OsmGeo[]
                {
                    new Way()
                    {
                        Id = 1,
                        Version = 1
                    }
                },
                Modify = new OsmGeo[]
                {
                    new Relation()
                    {
                        Id = 1,
                        Version = 2
                    }
                },
                Generator = "OsmSharp",
                Version = System.Version.Parse("0.6")
            };

            // doing the squashing, nothing should happen.
            var squashed = new[] { changeset }.Squash();

            Assert.That(squashed.Create, Is.Not.Null);
            Assert.That(squashed.Create.Length, Is.EqualTo(1));
            Assert.That(squashed.Create[0].Type, Is.EqualTo(OsmGeoType.Node));
            Assert.That(squashed.Delete, Is.Not.Null);
            Assert.That(squashed.Delete.Length, Is.EqualTo(1));
            Assert.That(squashed.Delete[0].Type, Is.EqualTo(OsmGeoType.Way));
            Assert.That(squashed.Modify, Is.Not.Null);
            Assert.That(squashed.Modify.Length, Is.EqualTo(1));
            Assert.That(squashed.Modify[0].Type, Is.EqualTo(OsmGeoType.Relation));

            Assert.That(squashed.Generator, Is.EqualTo("OsmSharp"));
            Assert.That(squashed.Version, Is.EqualTo(System.Version.Parse("0.6")));
        }

        /// <summary>
        /// Tests modifying a creation.
        /// </summary>
        [Test]
        public void TestSquashModifyCreation()
        {
            var changeset1 = new OsmChange()
            {
                Create = new OsmGeo[]
                {
                    new Node()
                    {
                        Id = 1,
                        Version = 1
                    }
                },
                Generator = "OsmSharp",
                Version = System.Version.Parse("0.6")
            };
            var changeset2 = new OsmChange()
            {
                Modify = new OsmGeo[]
                {
                    new Node()
                    {
                        Id = 1,
                        Version = 3
                    }
                },
                Generator = "OsmSharp",
                Version = System.Version.Parse("0.6")
            };

            // doing the squashing, should modify the creation.
            var squashed = new[] { changeset1, changeset2 }.Squash();

            Assert.That(squashed.Create, Is.Not.Null);
            Assert.That(squashed.Create.Length, Is.EqualTo(1));
            Assert.That(squashed.Create[0].Type, Is.EqualTo(OsmGeoType.Node));
            Assert.That(squashed.Create[0].Id, Is.EqualTo(1));
            Assert.That(squashed.Create[0].Version, Is.EqualTo(3));
            Assert.That(squashed.Delete, Is.Not.Null);
            Assert.That(squashed.Delete.Length, Is.EqualTo(0));
            Assert.That(squashed.Modify, Is.Not.Null);
            Assert.That(squashed.Modify.Length, Is.EqualTo(0));

            Assert.That(squashed.Generator, Is.EqualTo("OsmSharp"));
            Assert.That(squashed.Version, Is.EqualTo(System.Version.Parse("0.6")));
        }

        /// <summary>
        /// Tests deleting a creation.
        /// </summary>
        [Test]
        public void TestSquashDeleteCreation()
        {
            var changeset1 = new OsmChange()
            {
                Create = new OsmGeo[]
                {
                    new Node()
                    {
                        Id = 1,
                        Version = 1
                    }
                },
                Generator = "OsmSharp",
                Version = System.Version.Parse("0.6")
            };
            var changeset2 = new OsmChange()
            {
                Delete = new OsmGeo[]
                {
                    new Node()
                    {
                        Id = 1,
                        Version = 1
                    }
                },
                Generator = "OsmSharp",
                Version = System.Version.Parse("0.6")
            };

            // doing the squashing, should undo the creation.
            var squashed = new[] { changeset1, changeset2 }.Squash();

            Assert.That(squashed.Create, Is.Not.Null);
            Assert.That(squashed.Create.Length, Is.EqualTo(0));
            Assert.That(squashed.Delete, Is.Not.Null);
            Assert.That(squashed.Delete.Length, Is.EqualTo(0));
            Assert.That(squashed.Modify, Is.Not.Null);
            Assert.That(squashed.Modify.Length, Is.EqualTo(0));

            Assert.That(squashed.Generator, Is.EqualTo("OsmSharp"));
            Assert.That(squashed.Version, Is.EqualTo(System.Version.Parse("0.6")));
        }

        /// <summary>
        /// Tests deleting a modified creation.
        /// </summary>
        [Test]
        public void TestSquashDeleteModifiedCreation()
        {
            var changeset1 = new OsmChange()
            {
                Create = new OsmGeo[]
                {
                    new Node()
                    {
                        Id = 1,
                        Version = 1
                    }
                },
                Generator = "OsmSharp",
                Version = System.Version.Parse("0.6")
            };
            var changeset2 = new OsmChange()
            {
                Modify = new OsmGeo[]
                {
                    new Node()
                    {
                        Id = 1,
                        Version = 3
                    }
                },
                Generator = "OsmSharp",
                Version = System.Version.Parse("0.6")
            };
            var changeset3 = new OsmChange()
            {
                Delete = new OsmGeo[]
                {
                    new Node()
                    {
                        Id = 1,
                        Version = 3
                    }
                },
                Generator = "OsmSharp",
                Version = System.Version.Parse("0.6")
            };

            // doing the squashing, should undo the creation that was modified.
            var squashed = new[] { changeset1, changeset2, changeset3 }.Squash();

            Assert.That(squashed.Create, Is.Not.Null);
            Assert.That(squashed.Create.Length, Is.EqualTo(0));
            Assert.That(squashed.Delete, Is.Not.Null);
            Assert.That(squashed.Delete.Length, Is.EqualTo(0));
            Assert.That(squashed.Modify, Is.Not.Null);
            Assert.That(squashed.Modify.Length, Is.EqualTo(0));

            Assert.That(squashed.Generator, Is.EqualTo("OsmSharp"));
            Assert.That(squashed.Version, Is.EqualTo(System.Version.Parse("0.6")));
        }

        /// <summary>
        /// Tests modifying a modification.
        /// </summary>
        [Test]
        public void TestSquashModifyModification()
        {
            var changeset1 = new OsmChange()
            {
                Modify = new OsmGeo[]
                {
                    new Node()
                    {
                        Id = 1,
                        Version = 2
                    },
                    new Node()
                    {
                        Id = 1,
                        Version = 3
                    }
                },
                Generator = "OsmSharp",
                Version = System.Version.Parse("0.6")
            };

            // doing the squashing, should modify the creation.
            var squashed = new[] { changeset1 }.Squash();

            Assert.That(squashed.Modify, Is.Not.Null);
            Assert.That(squashed.Modify.Length, Is.EqualTo(1));
            Assert.That(squashed.Modify[0].Type, Is.EqualTo(OsmGeoType.Node));
            Assert.That(squashed.Modify[0].Id, Is.EqualTo(1));
            Assert.That(squashed.Modify[0].Version, Is.EqualTo(3));
            Assert.That(squashed.Delete, Is.Not.Null);
            Assert.That(squashed.Delete.Length, Is.EqualTo(0));
            Assert.That(squashed.Create, Is.Not.Null);
            Assert.That(squashed.Create.Length, Is.EqualTo(0));

            Assert.That(squashed.Generator, Is.EqualTo("OsmSharp"));
            Assert.That(squashed.Version, Is.EqualTo(System.Version.Parse("0.6")));
        }
    }
}