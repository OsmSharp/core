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
using OsmSharp.IO.Xml;
using System.IO;
using System.Xml.Serialization;

namespace OsmSharp.Test.IO.Xml.Changesets
{
    /// <summary>
    /// Contains tests for the osm change class.
    /// </summary>
    [TestFixture]
    public class OsmChangeTests
    {
        /// <summary>
        /// Tests serialization.
        /// </summary>
        [Test]
        public void TestSerialize()
        {
            var osmChange = new OsmChange()
            {
                Version = 0.6,
                Generator = "OsmSharp",
                Create = new OsmGeo[]
                {
                    new Node()
                    {
                        Id = 1
                    },
                    new Way()
                    {
                        Id = 10
                    },
                    new Relation()
                    {
                        Id = 100
                    }
                },
                Modify = new OsmGeo[]
                {
                    new Node()
                    {
                        Id = 2
                    },
                    new Way()
                    {
                        Id = 20
                    },
                    new Relation()
                    {
                        Id = 200
                    }
                },
                Delete = new OsmGeo[]
                {
                    new Node()
                    {
                        Id = 3
                    },
                    new Way()
                    {
                        Id = 30
                    },
                    new Relation()
                    {
                        Id = 300
                    }
                }
            };

            var result = osmChange.SerializeToXml();
            Assert.AreEqual("<osmChange generator=\"OsmSharp\" version=\"0.6\"><create><node id=\"1\" /><way id=\"10\" /><relation id=\"100\" /></create><modify><node id=\"2\" /><way id=\"20\" /><relation id=\"200\" /></modify><delete><node id=\"3\" /><way id=\"30\" /><relation id=\"300\" /></delete></osmChange>",
                result);
        }

        /// <summary>
        /// Tests deserialization.
        /// </summary>
        [Test]
        public void TestDeserialize()
        {
            var serializer = new XmlSerializer(typeof(OsmChange));

            var osmChange = serializer.Deserialize(
                new StringReader("<osmChange version=\"0.6\"></osmChange>")) as OsmChange;
            Assert.IsNotNull(osmChange);
            Assert.IsNull(osmChange.Create);
            Assert.IsNull(osmChange.Delete);
            Assert.IsNull(osmChange.Modify);
            Assert.AreEqual(0.6, osmChange.Version);
            Assert.IsNull(osmChange.Generator);

            osmChange = serializer.Deserialize(
                new StringReader("<osmChange generator=\"OsmSharp\" version=\"0.6\"></osmChange>")) as OsmChange;
            Assert.IsNotNull(osmChange);
            Assert.IsNull(osmChange.Create);
            Assert.IsNull(osmChange.Delete);
            Assert.IsNull(osmChange.Modify);
            Assert.AreEqual(0.6, osmChange.Version);
            Assert.AreEqual("OsmSharp", osmChange.Generator);

            osmChange = serializer.Deserialize(
                new StringReader("<osmChange generator=\"OsmSharp\" version=\"0.6\"><create><node id=\"1\" /><way id=\"10\" /><relation id=\"100\" /></create><modify><node id=\"2\" /><way id=\"20\" /><relation id=\"200\" /></modify><delete><node id=\"3\" /><way id=\"30\" /><relation id=\"300\" /></delete></osmChange>")) as OsmChange;
            Assert.IsNotNull(osmChange);

            Assert.IsNotNull(osmChange.Create);
            Assert.AreEqual(3, osmChange.Create.Length);
            Assert.AreEqual(1, osmChange.Create[0].Id);
            Assert.AreEqual(OsmGeoType.Node, osmChange.Create[0].Type);
            Assert.AreEqual(10, osmChange.Create[1].Id);
            Assert.AreEqual(OsmGeoType.Way, osmChange.Create[1].Type);
            Assert.AreEqual(100, osmChange.Create[2].Id);
            Assert.AreEqual(OsmGeoType.Relation, osmChange.Create[2].Type);

            Assert.IsNotNull(osmChange.Modify);
            Assert.AreEqual(3, osmChange.Modify.Length);
            Assert.AreEqual(2, osmChange.Modify[0].Id);
            Assert.AreEqual(OsmGeoType.Node, osmChange.Modify[0].Type);
            Assert.AreEqual(20, osmChange.Modify[1].Id);
            Assert.AreEqual(OsmGeoType.Way, osmChange.Modify[1].Type);
            Assert.AreEqual(200, osmChange.Modify[2].Id);
            Assert.AreEqual(OsmGeoType.Relation, osmChange.Modify[2].Type);

            Assert.IsNotNull(osmChange.Delete);
            Assert.AreEqual(3, osmChange.Delete.Length);
            Assert.AreEqual(3, osmChange.Delete[0].Id);
            Assert.AreEqual(OsmGeoType.Node, osmChange.Delete[0].Type);
            Assert.AreEqual(30, osmChange.Delete[1].Id);
            Assert.AreEqual(OsmGeoType.Way, osmChange.Delete[1].Type);
            Assert.AreEqual(300, osmChange.Delete[2].Id);
            Assert.AreEqual(OsmGeoType.Relation, osmChange.Delete[2].Type);

            Assert.AreEqual(0.6, osmChange.Version);
            Assert.AreEqual("OsmSharp", osmChange.Generator);
        }
    }
}