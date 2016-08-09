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
using System.IO;

namespace OsmSharp.Test.Stream
{
    /// <summary>
    /// Contains tests for the OSM-XML target stream.
    /// </summary>
    [TestFixture]
    public class XmlOsmStreamTargetTests
    {
        /// <summary>
        /// Tests writing one node.
        /// </summary>
        [Test]
        public void TestWriteNode()
        {
            var source = new OsmGeo[]
            {
                new Node()
                {
                    Id = 1
                }
            };

            using (var memoryStream = new MemoryStream())
            {
                var target = new XmlOsmStreamTarget(memoryStream);
                target.RegisterSource(source);
                target.Pull();

                memoryStream.Seek(0, SeekOrigin.Begin);
                var result = (new StreamReader(memoryStream)).ReadToEnd();
                Assert.AreEqual("<?xml version=\"1.0\" encoding=\"UTF-8\"?><osm version=\"0.6\" generator=\"OsmSharp\"><node id=\"1\" /></osm>",
                    result);
            }
        }

        /// <summary>
        /// Tests writing one way.
        /// </summary>
        [Test]
        public void TestWriteWay()
        {
            var source = new OsmGeo[]
            {
                new Way()
                {
                    Id = 1
                }
            };

            using (var memoryStream = new MemoryStream())
            {
                var target = new XmlOsmStreamTarget(memoryStream);
                target.RegisterSource(source);
                target.Pull();

                memoryStream.Seek(0, SeekOrigin.Begin);
                var result = (new StreamReader(memoryStream)).ReadToEnd();
                Assert.AreEqual("<?xml version=\"1.0\" encoding=\"UTF-8\"?><osm version=\"0.6\" generator=\"OsmSharp\"><way id=\"1\" /></osm>",
                    result);
            }
        }

        /// <summary>
        /// Tests writing one relation.
        /// </summary>
        [Test]
        public void TestWriteRelation()
        {
            var source = new OsmGeo[]
            {
                new Relation()
                {
                    Id = 1
                }
            };

            using (var memoryStream = new MemoryStream())
            {
                var target = new XmlOsmStreamTarget(memoryStream);
                target.RegisterSource(source);
                target.Pull();

                memoryStream.Seek(0, SeekOrigin.Begin);
                var result = (new StreamReader(memoryStream)).ReadToEnd();
                Assert.AreEqual("<?xml version=\"1.0\" encoding=\"UTF-8\"?><osm version=\"0.6\" generator=\"OsmSharp\"><relation id=\"1\" /></osm>",
                    result);
            }
        }

        /// <summary>
        /// Tests writing a couple of nodes, ways and relations.
        /// </summary>
        [Test]
        public void TestWrite()
        {
            var source = new OsmGeo[]
            {
                new Node()
                {
                    Id = 1,
                    Latitude = 1.0f,
                    Longitude = 1.1f
                },
                new Node()
                {
                    Id = 2,
                    Latitude = 2.0f,
                    Longitude = 2.1f
                },
                new Node()
                {
                    Id = 3,
                    Latitude = 3.0f,
                    Longitude = 3.1f
                },
                new Way()
                {
                    Id = 1,
                    Nodes = new long[]
                    {
                        1, 2, 3
                    }
                },
                new Relation()
                {
                    Id = 1,
                    Members = new RelationMember[]
                    {
                        new RelationMember(1, string.Empty, OsmGeoType.Node)
                    }
                }
            };

            using (var memoryStream = new MemoryStream())
            {
                var target = new XmlOsmStreamTarget(memoryStream);
                target.RegisterSource(source);
                target.Pull();

                memoryStream.Seek(0, SeekOrigin.Begin);
                var result = (new StreamReader(memoryStream)).ReadToEnd();
                Assert.AreEqual("<?xml version=\"1.0\" encoding=\"UTF-8\"?><osm version=\"0.6\" generator=\"OsmSharp\"><node id=\"1\" lat=\"1\" lon=\"1.1\" /><node id=\"2\" lat=\"2\" lon=\"2.1\" /><node id=\"3\" lat=\"3\" lon=\"3.1\" /><way id=\"1\"><nd ref=\"1\" /><nd ref=\"2\" /><nd ref=\"3\" /></way><relation id=\"1\"><member type=\"node\" ref=\"1\" role=\"\" /></relation></osm>",
                    result);
            }
        }
    }
}