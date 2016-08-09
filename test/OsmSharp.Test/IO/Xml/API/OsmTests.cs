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
using OsmSharp.API;
using OsmSharp.IO.Xml;
using System.IO;
using System.Xml.Serialization;

namespace OsmSharp.Test.IO.Xml.API
{
    /// <summary>
    /// Contains tests for the osm class.
    /// </summary>
    [TestFixture]
    public class OsmTests
    {
        /// <summary>
        /// Tests serialization.
        /// </summary>
        [Test]
        public void TestSerialize()
        {
            var osm = new Osm()
            {
                Api = new Capabilities()
                {
                    Version = new Version()
                    {
                        Maximum = 0.6,
                        Minimum = 0.6
                    },
                    Area = new Area()
                    {
                        Maximum = 0.25
                    },
                    Changesets = new OsmSharp.API.Changesets()
                    {
                        MaximumElements = 50000
                    },
                    Status = new Status()
                    {
                        Api = "online",
                        Database = "online",
                        Gpx = "online"
                    },
                    Timeout = new Timeout()
                    {
                        Seconds = 300
                    },
                    Tracepoints = new Tracepoints()
                    {
                        PerPage = 5000
                    },
                    WayNodes = new WayNodes()
                    {
                        Maximum = 2000
                    }
                }
            };

            Assert.AreEqual("<osm><api><version minimum=\"0.6\" maximum=\"0.6\" /><area maximum=\"0.25\" /><tracepoints per_page=\"5000\" /><waynodes maximum=\"2000\" /><changesets maximum_elements=\"50000\" /><timeout seconds=\"300\" /><status api=\"online\" database=\"online\" gpx=\"online\" /></api></osm>", 
                osm.SerializeToXml());
        }

        /// <summary>
        /// Test deserialization.
        /// </summary>
        [Test]
        public void TestDeserialize()
        {
            var serializer = new XmlSerializer(typeof(Osm));

            var osm = serializer.Deserialize(
                new StringReader("<osm><api><version minimum=\"0.6\" maximum=\"0.6\" /><area maximum=\"0.25\" /><tracepoints per_page=\"5000\" /><waynodes maximum=\"2000\" /><changesets maximum_elements=\"50000\" /><timeout seconds=\"300\" /><status api=\"online\" database=\"online\" gpx=\"online\" /></api></osm>")) 
                    as Osm;
            Assert.IsNotNull(osm);
            var capabilities = osm.Api;
            Assert.IsNotNull(capabilities);
            Assert.IsNotNull(capabilities.Version);
            Assert.AreEqual(0.6, capabilities.Version.Minimum);
            Assert.AreEqual(0.6, capabilities.Version.Maximum);
            Assert.IsNotNull(capabilities.Area);
            Assert.AreEqual(0.25, capabilities.Area.Maximum);
            Assert.IsNotNull(capabilities.Changesets);
            Assert.AreEqual(50000, capabilities.Changesets.MaximumElements);
            Assert.IsNotNull(capabilities.Status);
            Assert.AreEqual("online", capabilities.Status.Api);
            Assert.AreEqual("online", capabilities.Status.Database);
            Assert.AreEqual("online", capabilities.Status.Gpx);
            Assert.IsNotNull(capabilities.Timeout);
            Assert.AreEqual(300, capabilities.Timeout.Seconds);
            Assert.IsNotNull(capabilities.Tracepoints);
            Assert.AreEqual(5000, capabilities.Tracepoints.PerPage);
            Assert.IsNotNull(capabilities.WayNodes);
            Assert.AreEqual(2000, capabilities.WayNodes.Maximum);
        }
    }
}