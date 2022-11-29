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
using System;
using System.Globalization;
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
        private static readonly CultureInfo DefaultCultureInfo = new CultureInfo("en-US");

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
                    Version = new OsmSharp.API.Version()
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
                        Api = Status.ServiceStatus.online,
                        Database = Status.ServiceStatus.online,
                        Gpx = Status.ServiceStatus.online
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
            Assert.AreEqual(Status.ServiceStatus.online, capabilities.Status.Api);
            Assert.AreEqual(Status.ServiceStatus.online, capabilities.Status.Database);
            Assert.AreEqual(Status.ServiceStatus.online, capabilities.Status.Gpx);
            Assert.IsNotNull(capabilities.Timeout);
            Assert.AreEqual(300, capabilities.Timeout.Seconds);
            Assert.IsNotNull(capabilities.Tracepoints);
            Assert.AreEqual(5000, capabilities.Tracepoints.PerPage);
            Assert.IsNotNull(capabilities.WayNodes);
            Assert.AreEqual(2000, capabilities.WayNodes.Maximum);
        }

        /// <summary>
        /// Test deserialization of XML that contains unexpected elements (for example 'note' and 'meta' from an OverpassApi result).
        /// </summary>
        [Test]
        public void TestDeserializeSkippingUnexpectedElements()
        {
            var xml =
                @"<?xml version=""1.0"" encoding=""UTF-8""?>
                <osm version=""0.6"" generator=""Overpass API 0.7.55.7 8b86ff77"">
                    <note>This is just a note</note>
                    <meta osm_base=""2019-07-27T00:04:02Z"" areas=""2019-07-26T23:48:03Z""/>
                    <node id=""1"" lat=""111"" lon=""-70.111"">
                        <tag k=""addr:housenumber"" v=""11""/>
                        <tag k=""addr:street"" v=""Main Street""/>
                    </node>
                </osm>";
            
            var serializer = new XmlSerializer(typeof(Osm));
            var osm = serializer.Deserialize(new StringReader(xml)) as Osm;
            Assert.IsNotNull(osm);
            Assert.AreEqual(.6, osm.Version);
            Assert.AreEqual("Overpass API 0.7.55.7 8b86ff77", osm.Generator);

            Assert.IsNull(osm.Ways);
            Assert.IsNull(osm.Relations);
            Assert.IsNull(osm.User);
            Assert.IsNull(osm.GpxFiles);
            Assert.IsNull(osm.Bounds);
            Assert.IsNull(osm.Api);
            Assert.IsNull(osm.Notes);

            Assert.IsNotNull(osm.Nodes);
            Assert.AreEqual(1, osm.Nodes.Length);
            var node = osm.Nodes[0];
            Assert.AreEqual(1, node.Id);
            Assert.AreEqual(111, node.Latitude);
            Assert.AreEqual(-70.111, node.Longitude);
            Assert.NotNull(node.Tags);
            Assert.AreEqual(2, node.Tags.Count);
            Assert.True(node.Tags.ContainsKey("addr:housenumber"));
            Assert.AreEqual("11", node.Tags["addr:housenumber"]);
            Assert.True(node.Tags.ContainsKey("addr:street"));
            Assert.AreEqual("Main Street", node.Tags["addr:street"]);
        }

        /// <summary>
        /// Test deserialization of XML that contains bounds.
        /// </summary>
        [Test]
        public void TestDeserializeWithBoundsElement()
        {
            var xml =
                @"<?xml version=""1.0"" encoding=""UTF-8""?>
                <osm version=""0.6"" generator=""CGImap 0.7.5 (5035 errol.openstreetmap.org)"" copyright=""OpenStreetMap and contributors"" attribution=""http://www.openstreetmap.org/copyright"" license=""http://opendatacommons.org/licenses/odbl/1-0/"">
                 <bounds minlat=""38.9070200"" minlon=""-77.0371900"" maxlat=""38.9077300"" maxlon=""-77.0360000""/>
                 <node id=""8549479"" visible=""true"" version=""6"" changeset=""17339"" timestamp=""2013-01-20T06:31:24Z"" user=""samanbb"" uid=""933"" lat=""38.8921989"" lon=""-77.0503034""/>
                 <node id=""8549530"" visible=""false"" version=""2"" changeset=""17248"" timestamp=""2013-01-17T15:24:35Z"" user=""ideditor"" uid=""912"" lat=""38.9065506"" lon=""-77.0345080""/>
                 <way id=""538868"" visible=""true"" version=""5"" changeset=""23710"" timestamp=""2013-05-28T17:45:26Z"" user=""Kate"" uid=""1163"">
                  <nd ref=""4294969195""/>
                  <nd ref=""4294969575""/>
                  <tag k=""highway"" v=""residential""/>
                  <tag k=""maxspeed:practical"" v=""12.910093541777924""/>
                 </way>
                </osm>
                ";

            Func<string, DateTime> parseToUniversalTime =
                t => DateTime.Parse(t, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            var serializer = new XmlSerializer(typeof(Osm));
            var osm = serializer.Deserialize(new StringReader(xml)) as Osm;
            Assert.IsNotNull(osm);
            Assert.AreEqual(.6, osm.Version);
            Assert.AreEqual("CGImap 0.7.5 (5035 errol.openstreetmap.org)", osm.Generator);

            Assert.IsNull(osm.Relations);
            Assert.IsNull(osm.User);
            Assert.IsNull(osm.GpxFiles);
            Assert.IsNull(osm.Api);

            Assert.IsNotNull(osm.Bounds);
            Assert.AreEqual(float.Parse("38.9070200", DefaultCultureInfo), osm.Bounds.MinLatitude);
            Assert.AreEqual(float.Parse("-77.0371900", DefaultCultureInfo), osm.Bounds.MinLongitude);
            Assert.AreEqual(float.Parse("38.9077300", DefaultCultureInfo), osm.Bounds.MaxLatitude);
            Assert.AreEqual(float.Parse("-77.0360000", DefaultCultureInfo), osm.Bounds.MaxLongitude);

            Assert.IsNotNull(osm.Nodes);
            Assert.AreEqual(2, osm.Nodes.Length);
            var node = osm.Nodes[0];
            Assert.AreEqual(8549479, node.Id);
            Assert.AreEqual(true, node.Visible);
            Assert.AreEqual(6, node.Version);
            Assert.AreEqual(17339, node.ChangeSetId);
            Assert.AreEqual(parseToUniversalTime("2013-01-20T06:31:24Z"), node.TimeStamp);
            Assert.AreEqual("samanbb", node.UserName);
            Assert.AreEqual(933, node.UserId);
            Assert.AreEqual(38.8921989, node.Latitude);
            Assert.AreEqual(-77.0503034, node.Longitude);
            Assert.IsNull(node.Tags);
            node = osm.Nodes[1];
            Assert.AreEqual(8549530, node.Id);
            Assert.AreEqual(false, node.Visible);
            Assert.AreEqual(2, node.Version);
            Assert.AreEqual(17248, node.ChangeSetId);
            Assert.AreEqual(parseToUniversalTime("2013-01-17T15:24:35Z"), node.TimeStamp);
            Assert.AreEqual("ideditor", node.UserName);
            Assert.AreEqual(912, node.UserId);
            Assert.AreEqual(38.9065506, node.Latitude);
            Assert.AreEqual(-77.0345080, node.Longitude);
            Assert.IsNull(node.Tags);

            Assert.IsNotNull(osm.Ways);
            Assert.AreEqual(1, osm.Ways.Length);
            var way = osm.Ways[0];
            Assert.AreEqual(538868, way.Id);
            Assert.AreEqual(true, way.Visible);
            Assert.AreEqual(5, way.Version);
            Assert.AreEqual(23710, way.ChangeSetId);
            Assert.AreEqual(parseToUniversalTime("2013-05-28T17:45:26Z"), way.TimeStamp);
            Assert.AreEqual("Kate", way.UserName);
            Assert.AreEqual(1163, way.UserId);
            Assert.NotNull(way.Nodes);
            Assert.AreEqual(2, way.Nodes.Length);
            Assert.AreEqual(4294969195, way.Nodes[0]);
            Assert.AreEqual(4294969575, way.Nodes[1]);
            Assert.NotNull(way.Tags);
            Assert.AreEqual(2, way.Tags.Count);
            Assert.True(way.Tags.ContainsKey("highway"));
            Assert.AreEqual("residential", way.Tags["highway"]);
            Assert.True(way.Tags.ContainsKey("maxspeed:practical"));
            Assert.AreEqual("12.910093541777924", way.Tags["maxspeed:practical"]);
        }

        /// <summary>
        /// Test deserialization of XML that contains api-capabilities and policies.
        /// </summary>
        [Test]
        public void TestDeserializeWithCapabilitiesAndPolicies()
        {
            var xml =
                @"<?xml version=""1.0"" encoding=""UTF-8""?>
                <osm version=""0.6"" generator=""OpenStreetMap server"" copyright=""OpenStreetMap and contributors"" attribution=""http://www.openstreetmap.org/copyright"" license=""http://opendatacommons.org/licenses/odbl/1-0/"">
                  <api>
                    <version minimum=""0.6"" maximum=""0.6""/>
                    <area maximum=""0.25""/>
                    <note_area maximum=""25""/>
                    <tracepoints per_page=""5000""/>
                    <waynodes maximum=""2000""/>
                    <changesets maximum_elements=""10000""/>
                    <timeout seconds=""300""/>
                    <status database=""online"" api=""online"" gpx=""online""/>
                  </api>
                  <policy>
                    <imagery>
                      <blacklist regex="".*\.google(apis)?\..*/(vt|kh)[\?/].*([xyz]=.*){3}.*""/>
                      <blacklist regex=""http://xdworld\.vworld\.kr:8080/.*""/>
                      <blacklist regex="".*\.here\.com[/:].*""/>
                    </imagery>
                  </policy>
                </osm>
                ";

            var serializer = new XmlSerializer(typeof(Osm));
            var osm = serializer.Deserialize(new StringReader(xml)) as Osm;
            Assert.IsNotNull(osm);
            Assert.AreEqual(.6, osm.Version);
            Assert.AreEqual("OpenStreetMap server", osm.Generator);

            Assert.IsNull(osm.Relations);
            Assert.IsNull(osm.User);
            Assert.IsNull(osm.GpxFiles);
            Assert.IsNull(osm.Nodes);
            Assert.IsNull(osm.Ways);
            Assert.IsNull(osm.Bounds);

            Assert.IsNotNull(osm.Api);
            Assert.IsNotNull(osm.Api.Version);
            Assert.AreEqual(.6, osm.Api.Version.Maximum);
            Assert.AreEqual(.6, osm.Api.Version.Minimum);
            Assert.IsNotNull(osm.Api.Area);
            Assert.AreEqual(.25, osm.Api.Area.Maximum);
            Assert.IsNotNull(osm.Api.NoteArea);
            Assert.AreEqual(25, osm.Api.NoteArea.Maximum);
            Assert.IsNotNull(osm.Api.Tracepoints);
            Assert.AreEqual(5000, osm.Api.Tracepoints.PerPage);
            Assert.IsNotNull(osm.Api.WayNodes);
            Assert.AreEqual(2000, osm.Api.WayNodes.Maximum);
            Assert.IsNotNull(osm.Api.Changesets);
            Assert.AreEqual(10000, osm.Api.Changesets.MaximumElements);
            Assert.IsNotNull(osm.Api.Timeout);
            Assert.AreEqual(300, osm.Api.Timeout.Seconds);
            Assert.IsNotNull(osm.Api.Status);
            Assert.AreEqual(Status.ServiceStatus.online, osm.Api.Status.Database);
            Assert.AreEqual(Status.ServiceStatus.online, osm.Api.Status.Api);
            Assert.AreEqual(Status.ServiceStatus.online, osm.Api.Status.Gpx);

            Assert.IsNotNull(osm.Policy);
            Assert.IsNotNull(osm.Policy.Imagery);
            Assert.IsNotNull(osm.Policy.Imagery.Blacklists);
            Assert.AreEqual(3, osm.Policy.Imagery.Blacklists.Length);
            Assert.AreEqual(@".*\.google(apis)?\..*/(vt|kh)[\?/].*([xyz]=.*){3}.*", osm.Policy.Imagery.Blacklists[0].Regex);
            Assert.AreEqual(@"http://xdworld\.vworld\.kr:8080/.*", osm.Policy.Imagery.Blacklists[1].Regex);
            Assert.AreEqual(@".*\.here\.com[/:].*", osm.Policy.Imagery.Blacklists[2].Regex);
        }

        /// <summary>
        /// Test deserialization of XML that contains the version as a single value (as apposed to attributes).
        /// </summary>
        [Test]
        public void TestDeserializeVersionAsValue()
        {
            var xml =
                @"<?xml version=""1.0"" encoding=""UTF-8""?>
                <osm>
                  <api>
                    <version>0.6</version>
                  </api>
                </osm>
                ";

            var serializer = new XmlSerializer(typeof(Osm));
            var osm = serializer.Deserialize(new StringReader(xml)) as Osm;

            Assert.IsNotNull(osm);
            Assert.IsNotNull(osm.Api);
            Assert.IsNotNull(osm.Api.Version);
            Assert.AreEqual(.6, osm.Api.Version.Maximum);
            Assert.AreEqual(.6, osm.Api.Version.Maximum);
        }

        /// <summary>
        /// Test deserialization of XML that contains permissions.
        /// </summary>
        [Test]
        public void TestDeserializePermissions()
        {
            var xml =
                @"<?xml version=""1.0"" encoding=""UTF-8""?>
                <osm version=""0.6"" generator=""OpenStreetMap server"">
                  <permissions>
                    <permission name=""allow_read_prefs""/>
                    <permission name=""allow_read_gpx""/>
                    <permission name=""allow_write_gpx""/>
                  </permissions>
                </osm>
                ";

            var serializer = new XmlSerializer(typeof(Osm));
            var osm = serializer.Deserialize(new StringReader(xml)) as Osm;

            Assert.IsNotNull(osm);
            Assert.IsNotNull(osm.Permissions);
            Assert.IsNotNull(osm.Permissions.UserPermission);
            Assert.AreEqual(3, osm.Permissions.UserPermission.Length);
            Assert.AreEqual(Permissions.Permission.allow_read_prefs, osm.Permissions.UserPermission[0]);
            Assert.AreEqual(Permissions.Permission.allow_read_gpx, osm.Permissions.UserPermission[1]);
            Assert.AreEqual(Permissions.Permission.allow_write_gpx, osm.Permissions.UserPermission[2]);
        }

        /// <summary>
        /// Test deserialization of XML that contains preferences.
        /// </summary>
        [Test]
        public void TestDeserializePreferences()
        {
            var xml =
                @"<?xml version=""1.0"" encoding=""UTF -8"" ?>
                <osm>
                    <preferences>
                        <preference k=""gps.trace.visibility"" v=""public"" />
                        <preference k=""color"" v=""red"" />
                    </preferences>
                </osm>
                ";

            var serializer = new XmlSerializer(typeof(Osm));
            var osm = serializer.Deserialize(new StringReader(xml)) as Osm;

            Assert.IsNotNull(osm);
            Assert.IsNotNull(osm.Preferences);
            Assert.IsNotNull(osm.Preferences.UserPreferences);
            Assert.AreEqual(2, osm.Preferences.UserPreferences.Length);
            Assert.AreEqual("gps.trace.visibility", osm.Preferences.UserPreferences[0].Key);
            Assert.AreEqual("public", osm.Preferences.UserPreferences[0].Value);
            Assert.AreEqual("color", osm.Preferences.UserPreferences[1].Key);
            Assert.AreEqual("red", osm.Preferences.UserPreferences[1].Value);
        }

        /// <summary>
        /// Test deserialization of XML that contains Notes.
        /// </summary>
        [Test]
        public void TestDeserializeNotes()
        {
            var xml =
                @"<?xml version=""1.0"" encoding=""UTF-8""?>
                <osm>
                    <note>This note should be skipped because it has no ID</note>
                    <note lon=""0.1000000"" lat=""51.0000000"">
                        <id>16659</id>
                        <url>https://master.apis.dev.openstreetmap.org/api/0.6/notes/16659</url>
                        <comment_url>https://master.apis.dev.openstreetmap.org/api/0.6/notes/16659/comment</comment_url>
                        <close_url>https://master.apis.dev.openstreetmap.org/api/0.6/notes/16659/close</close_url>
                        <date_created>2019-06-15 08:26:04 UTC</date_created>
                        <status>open</status>
                        <comments>
                            <comment>
                                <date>2019-06-15 08:26:04 UTC</date>
                                <uid>1234</uid>
                                <user>userName</user>
                                <user_url>https://master.apis.dev.openstreetmap.org/user/userName</user_url>
                                <action>opened</action>
                                <text>ThisIsANote</text>
                                <html>&lt;p&gt;ThisIsANote&lt;/p&gt;</html>
                            </comment>
                        </comments>
                    </note>
                </osm>
                ";

            var serializer = new XmlSerializer(typeof(Osm));
            var osm = serializer.Deserialize(new StringReader(xml)) as Osm;

            Assert.IsNotNull(osm);
            Assert.IsNotNull(osm.Notes);
            Assert.AreEqual(1, osm.Notes.Length);

            var note = osm.Notes[0];
            Assert.AreEqual(51, note.Latitude);
            Assert.AreEqual(0.1, note.Longitude);
            Assert.AreEqual(16659, note.Id);
            Assert.AreEqual("https://master.apis.dev.openstreetmap.org/api/0.6/notes/16659", note.Url);
            Assert.AreEqual("https://master.apis.dev.openstreetmap.org/api/0.6/notes/16659/comment", note.CommentUrl);
            Assert.AreEqual("https://master.apis.dev.openstreetmap.org/api/0.6/notes/16659/close", note.CloseUrl);
            Assert.AreEqual(Note.ParseNoteDate("2019-06-15 08:26:04 UTC"), note.DateCreated);
            Assert.AreEqual(Note.NoteStatus.Open, note.Status);
            Assert.IsNotNull(note.Comments);
            Assert.IsNotNull(note.Comments.Comments);
            Assert.AreEqual(1, note.Comments.Comments.Length);

            var comment = note.Comments.Comments[0];
            Assert.AreEqual(Note.ParseNoteDate("2019-06-15 08:26:04 UTC"), comment.Date);
            Assert.AreEqual(1234, comment.UserId);
            Assert.AreEqual("userName", comment.UserName);
            Assert.AreEqual("https://master.apis.dev.openstreetmap.org/user/userName", comment.UserUrl);
            Assert.AreEqual(Note.Comment.CommentAction.Opened, comment.Action);
            Assert.AreEqual("ThisIsANote", comment.Text);
            Assert.AreEqual("<p>ThisIsANote</p>", comment.HTML);
        }
    }
}