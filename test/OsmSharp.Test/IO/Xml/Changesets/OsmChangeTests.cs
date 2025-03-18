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
                Version = System.Version.Parse("0.6"),
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
            Assert.That(result, Is.EqualTo("<osmChange generator=\"OsmSharp\" version=\"0.6\"><create><node id=\"1\" /><way id=\"10\" /><relation id=\"100\" /></create><modify><node id=\"2\" /><way id=\"20\" /><relation id=\"200\" /></modify><delete><node id=\"3\" /><way id=\"30\" /><relation id=\"300\" /></delete></osmChange>"));
        }

        /// <summary>
        /// Tests deserialization.
        /// </summary>
        [Test]
        public void TestDeserialize_Empty()
        {
            var serializer = new XmlSerializer(typeof(OsmChange));

            var osmChange = serializer.Deserialize(
                new StringReader("<osmChange version=\"0.6\"></osmChange>")) as OsmChange;
            Assert.That(osmChange, Is.Not.Null);
            Assert.That(osmChange.Create.Length, Is.EqualTo(0));
            Assert.That(osmChange.Delete.Length, Is.EqualTo(0));
            Assert.That(osmChange.Modify.Length, Is.EqualTo(0));
            Assert.That(osmChange.Version, Is.EqualTo(System.Version.Parse("0.6")));
            Assert.That(osmChange.Generator, Is.Null);
            Assert.That(osmChange.Copyright, Is.Null);
            Assert.That(osmChange.Attribution, Is.Null);
            Assert.That(osmChange.License, Is.Null);
        }

        [Test]
        public void TestDeserialize_WithAttributes()
        {
            var serializer = new XmlSerializer(typeof(OsmChange));
            var osmChange = serializer.Deserialize(
                new StringReader("<osmChange generator=\"OsmSharp\" version=\"0.6\" copyright=\"OpenStreetMap and contributors\" attribution=\"http://www.openstreetmap.org/copyright\" license=\"http://opendatacommons.org/licenses/odbl/1-0/\"></osmChange>")) as OsmChange;
            Assert.That(osmChange, Is.Not.Null);
            Assert.That(osmChange.Create.Length, Is.EqualTo(0));
            Assert.That(osmChange.Delete.Length, Is.EqualTo(0));
            Assert.That(osmChange.Modify.Length, Is.EqualTo(0));
            Assert.That(osmChange.Version, Is.EqualTo(System.Version.Parse("0.6")));
            Assert.That(osmChange.Generator, Is.EqualTo("OsmSharp"));
            Assert.That(osmChange.Copyright, Is.EqualTo("OpenStreetMap and contributors"));
            Assert.That(osmChange.Attribution, Is.EqualTo("http://www.openstreetmap.org/copyright"));
            Assert.That(osmChange.License, Is.EqualTo("http://opendatacommons.org/licenses/odbl/1-0/"));
        }

        [Test]
        public void TestDeserialize_Full()
        {
            var serializer = new XmlSerializer(typeof(OsmChange));

            var osmChange = serializer.Deserialize(
                new StringReader("<osmChange generator=\"OsmSharp\" version=\"0.6\"><create><node id=\"1\" /><way id=\"10\" /><relation id=\"100\" /></create><modify><node id=\"2\" /><way id=\"20\" /><relation id=\"200\" /></modify><delete><node id=\"3\" /><way id=\"30\" /><relation id=\"300\" /></delete></osmChange>")) as OsmChange;
            Assert.That(osmChange, Is.Not.Null);

            Assert.That(osmChange.Create, Is.Not.Null);
            Assert.That(osmChange.Create.Length, Is.EqualTo(3));
            Assert.That(osmChange.Create[0].Id, Is.EqualTo(1));
            Assert.That(osmChange.Create[0].Type, Is.EqualTo(OsmGeoType.Node));
            Assert.That(osmChange.Create[1].Id, Is.EqualTo(10));
            Assert.That(osmChange.Create[1].Type, Is.EqualTo(OsmGeoType.Way));
            Assert.That(osmChange.Create[2].Id, Is.EqualTo(100));
            Assert.That(osmChange.Create[2].Type, Is.EqualTo(OsmGeoType.Relation));

            Assert.That(osmChange.Modify, Is.Not.Null);
            Assert.That(osmChange.Modify.Length, Is.EqualTo(3));
            Assert.That(osmChange.Modify[0].Id, Is.EqualTo(2));
            Assert.That(osmChange.Modify[0].Type, Is.EqualTo(OsmGeoType.Node));
            Assert.That(osmChange.Modify[1].Id, Is.EqualTo(20));
            Assert.That(osmChange.Modify[1].Type, Is.EqualTo(OsmGeoType.Way));
            Assert.That(osmChange.Modify[2].Id, Is.EqualTo(200));
            Assert.That(osmChange.Modify[2].Type, Is.EqualTo(OsmGeoType.Relation));

            Assert.That(osmChange.Delete, Is.Not.Null);
            Assert.That(osmChange.Delete.Length, Is.EqualTo(3));
            Assert.That(osmChange.Delete[0].Id, Is.EqualTo(3));
            Assert.That(osmChange.Delete[0].Type, Is.EqualTo(OsmGeoType.Node));
            Assert.That(osmChange.Delete[1].Id, Is.EqualTo(30));
            Assert.That(osmChange.Delete[1].Type, Is.EqualTo(OsmGeoType.Way));
            Assert.That(osmChange.Delete[2].Id, Is.EqualTo(300));
            Assert.That(osmChange.Delete[2].Type, Is.EqualTo(OsmGeoType.Relation));

            Assert.That(osmChange.Version, Is.EqualTo(System.Version.Parse("0.6")));
            Assert.That(osmChange.Generator, Is.EqualTo("OsmSharp"));

        }

        [Test]
        public void TestDeserialize_IfUnused()
        {
            var serializer = new XmlSerializer(typeof(OsmChange));

            var osmChange = serializer.Deserialize(
				new StringReader("<osmChange version=\"0.6\" generator=\"iD\"><create/><modify><node id=\"1014872736\" lon=\"4.793814787696839\" lat=\"51.26403992993145\" version=\"1470\" changeset=\"2\"/></modify><delete if-unused=\"true\"/></osmChange>")) as OsmChange;
			Assert.That(osmChange, Is.Not.Null);

			Assert.That(osmChange.Create.Length, Is.EqualTo(0));
			Assert.That(osmChange.Modify, Is.Not.Null);
			Assert.That(osmChange.Modify.Length, Is.EqualTo(1));
			Assert.That(osmChange.Modify[0].Id, Is.EqualTo(1014872736));
			Assert.That(osmChange.Modify[0].Type, Is.EqualTo(OsmGeoType.Node));
			Assert.That(osmChange.Delete.Length, Is.EqualTo(0));
			Assert.That(osmChange.Version, Is.EqualTo(System.Version.Parse("0.6")));
			Assert.That(osmChange.Generator, Is.EqualTo("iD"));
        }

        [Test]
        public void TestDeserialize_DuplicateContainerTags()
        {
            var serializer = new XmlSerializer(typeof(OsmChange));

            var osmChange = serializer.Deserialize(
				new StringReader("<osmChange generator=\"OsmSharp\" version=\"0.6\"><create><node id=\"1\" /><way id=\"10\" /></create><create><relation id=\"100\" /></create><modify><node id=\"2\" /><way id=\"20\" /></modify><modify><relation id=\"200\" /></modify><delete><node id=\"3\" /></delete><delete><way id=\"30\" /><relation id=\"300\" /></delete></osmChange>")) as OsmChange;
			Assert.That(osmChange, Is.Not.Null);

            Assert.That(osmChange.Create, Is.Not.Null);
            Assert.That(osmChange.Create.Length, Is.EqualTo(3));
            Assert.That(osmChange.Create[0].Id, Is.EqualTo(1));
            Assert.That(osmChange.Create[0].Type, Is.EqualTo(OsmGeoType.Node));
            Assert.That(osmChange.Create[1].Id, Is.EqualTo(10));
            Assert.That(osmChange.Create[1].Type, Is.EqualTo(OsmGeoType.Way));
            Assert.That(osmChange.Create[2].Id, Is.EqualTo(100));
            Assert.That(osmChange.Create[2].Type, Is.EqualTo(OsmGeoType.Relation));

            Assert.That(osmChange.Modify, Is.Not.Null);
            Assert.That(osmChange.Modify.Length, Is.EqualTo(3));
            Assert.That(osmChange.Modify[0].Id, Is.EqualTo(2));
            Assert.That(osmChange.Modify[0].Type, Is.EqualTo(OsmGeoType.Node));
            Assert.That(osmChange.Modify[1].Id, Is.EqualTo(20));
            Assert.That(osmChange.Modify[1].Type, Is.EqualTo(OsmGeoType.Way));
            Assert.That(osmChange.Modify[2].Id, Is.EqualTo(200));
            Assert.That(osmChange.Modify[2].Type, Is.EqualTo(OsmGeoType.Relation));

            Assert.That(osmChange.Delete, Is.Not.Null);
            Assert.That(osmChange.Delete.Length, Is.EqualTo(3));
            Assert.That(osmChange.Delete[0].Id, Is.EqualTo(3));
            Assert.That(osmChange.Delete[0].Type, Is.EqualTo(OsmGeoType.Node));
            Assert.That(osmChange.Delete[1].Id, Is.EqualTo(30));
            Assert.That(osmChange.Delete[1].Type, Is.EqualTo(OsmGeoType.Way));
            Assert.That(osmChange.Delete[2].Id, Is.EqualTo(300));
            Assert.That(osmChange.Delete[2].Type, Is.EqualTo(OsmGeoType.Relation));
        }
    }
}