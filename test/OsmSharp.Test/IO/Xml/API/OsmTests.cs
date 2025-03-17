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

            Assert.That(osm.SerializeToXml(), Is.EqualTo("<osm><api><version minimum=\"0.6\" maximum=\"0.6\" /><area maximum=\"0.25\" /><tracepoints per_page=\"5000\" /><waynodes maximum=\"2000\" /><changesets maximum_elements=\"50000\" /><timeout seconds=\"300\" /><status api=\"online\" database=\"online\" gpx=\"online\" /></api></osm>"));
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
            Assert.That(osm, Is.Not.Null);
            var capabilities = osm.Api;
            Assert.That(capabilities, Is.Not.Null);
            Assert.That(capabilities.Version, Is.Not.Null);
            Assert.That(capabilities.Version.Minimum, Is.EqualTo(0.6));
            Assert.That(capabilities.Version.Maximum, Is.EqualTo(0.6));
            Assert.That(capabilities.Area, Is.Not.Null);
            Assert.That(capabilities.Area.Maximum, Is.EqualTo(0.25));
            Assert.That(capabilities.Changesets, Is.Not.Null);
            Assert.That(capabilities.Changesets.MaximumElements, Is.EqualTo(50000));
            Assert.That(capabilities.Status, Is.Not.Null);
            Assert.That(capabilities.Status.Api, Is.EqualTo(Status.ServiceStatus.online));
            Assert.That(capabilities.Status.Database, Is.EqualTo(Status.ServiceStatus.online));
            Assert.That(capabilities.Status.Gpx, Is.EqualTo(Status.ServiceStatus.online));
            Assert.That(capabilities.Timeout, Is.Not.Null);
            Assert.That(capabilities.Timeout.Seconds, Is.EqualTo(300));
            Assert.That(capabilities.Tracepoints, Is.Not.Null);
            Assert.That(capabilities.Tracepoints.PerPage, Is.EqualTo(5000));
            Assert.That(capabilities.WayNodes, Is.Not.Null);
            Assert.That(capabilities.WayNodes.Maximum, Is.EqualTo(2000));
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
            Assert.That(osm, Is.Not.Null);
            Assert.That(osm.Version, Is.EqualTo(.6));
            Assert.That(osm.Generator, Is.EqualTo("Overpass API 0.7.55.7 8b86ff77"));

            Assert.That(osm.Ways, Is.Null);
            Assert.That(osm.Relations, Is.Null);
            Assert.That(osm.User, Is.Null);
            Assert.That(osm.GpxFiles, Is.Null);
            Assert.That(osm.Bounds, Is.Null);
            Assert.That(osm.Api, Is.Null);
            Assert.That(osm.Notes, Is.Null);

            Assert.That(osm.Nodes, Is.Not.Null);
            Assert.That(osm.Nodes.Length, Is.EqualTo(1));
            var node = osm.Nodes[0];
            Assert.That(node.Id, Is.EqualTo(1));
            Assert.That(node.Latitude, Is.EqualTo(111));
            Assert.That(node.Longitude, Is.EqualTo(-70.111));
            Assert.That(node.Tags, Is.Not.Null);
            Assert.That(node.Tags.Count, Is.EqualTo(2));
            Assert.True(node.Tags.ContainsKey("addr:housenumber"));
            Assert.That(node.Tags["addr:housenumber"], Is.EqualTo("11"));
            Assert.True(node.Tags.ContainsKey("addr:street"));
            Assert.That(node.Tags["addr:street"], Is.EqualTo("Main Street"));
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
            Assert.That(osm, Is.Not.Null);
            Assert.That(osm.Version, Is.EqualTo(.6));
            Assert.That(osm.Generator, Is.EqualTo("CGImap 0.7.5 (5035 errol.openstreetmap.org)"));

            Assert.That(osm.Relations, Is.Null);
            Assert.That(osm.User, Is.Null);
            Assert.That(osm.GpxFiles, Is.Null);
            Assert.That(osm.Api, Is.Null);

            Assert.That(osm.Bounds, Is.Not.Null);
            Assert.That(osm.Bounds.MinLatitude, Is.EqualTo(float.Parse("38.9070200", DefaultCultureInfo)));
            Assert.That(osm.Bounds.MinLongitude, Is.EqualTo(float.Parse("-77.0371900", DefaultCultureInfo)));
            Assert.That(osm.Bounds.MaxLatitude, Is.EqualTo(float.Parse("38.9077300", DefaultCultureInfo)));
            Assert.That(osm.Bounds.MaxLongitude, Is.EqualTo(float.Parse("-77.0360000", DefaultCultureInfo)));

            Assert.That(osm.Nodes, Is.Not.Null);
            Assert.That(osm.Nodes.Length, Is.EqualTo(2));
            var node = osm.Nodes[0];
            Assert.That(node.Id, Is.EqualTo(8549479));
            Assert.That(node.Visible, Is.EqualTo(true));
            Assert.That(node.Version, Is.EqualTo(6));
            Assert.That(node.ChangeSetId, Is.EqualTo(17339));
            Assert.That(node.TimeStamp, Is.EqualTo(parseToUniversalTime("2013-01-20T06:31:24Z")));
            Assert.That(node.UserName, Is.EqualTo("samanbb"));
            Assert.That(node.UserId, Is.EqualTo(933));
            Assert.That(node.Latitude, Is.EqualTo(38.8921989));
            Assert.That(node.Longitude, Is.EqualTo(-77.0503034));
            Assert.That(node.Tags, Is.Null);
            node = osm.Nodes[1];
            Assert.That(node.Id, Is.EqualTo(8549530));
            Assert.That(node.Visible, Is.EqualTo(false));
            Assert.That(node.Version, Is.EqualTo(2));
            Assert.That(node.ChangeSetId, Is.EqualTo(17248));
            Assert.That(node.TimeStamp, Is.EqualTo(parseToUniversalTime("2013-01-17T15:24:35Z")));
            Assert.That(node.UserName, Is.EqualTo("ideditor"));
            Assert.That(node.UserId, Is.EqualTo(912));
            Assert.That(node.Latitude, Is.EqualTo(38.9065506));
            Assert.That(node.Longitude, Is.EqualTo(-77.0345080));
            Assert.That(node.Tags, Is.Null);

            Assert.That(osm.Ways, Is.Not.Null);
            Assert.That(osm.Ways.Length, Is.EqualTo(1));
            var way = osm.Ways[0];
            Assert.That(way.Id, Is.EqualTo(538868));
            Assert.That(way.Visible, Is.EqualTo(true));
            Assert.That(way.Version, Is.EqualTo(5));
            Assert.That(way.ChangeSetId, Is.EqualTo(23710));
            Assert.That(way.TimeStamp, Is.EqualTo(parseToUniversalTime("2013-05-28T17:45:26Z")));
            Assert.That(way.UserName, Is.EqualTo("Kate"));
            Assert.That(way.UserId, Is.EqualTo(1163));
            Assert.That(way.Nodes, Is.Not.Null);
            Assert.That(way.Nodes.Length, Is.EqualTo(2));
            Assert.That(way.Nodes[0], Is.EqualTo(4294969195));
            Assert.That(way.Nodes[1], Is.EqualTo(4294969575));
            Assert.That(way.Tags, Is.Not.Null);
            Assert.That(way.Tags.Count, Is.EqualTo(2));
            Assert.True(way.Tags.ContainsKey("highway"));
            Assert.That(way.Tags["highway"], Is.EqualTo("residential"));
            Assert.True(way.Tags.ContainsKey("maxspeed:practical"));
            Assert.That(way.Tags["maxspeed:practical"], Is.EqualTo("12.910093541777924"));
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
            Assert.That(osm, Is.Not.Null);
            Assert.That(osm.Version, Is.EqualTo(.6));
            Assert.That(osm.Generator, Is.EqualTo("OpenStreetMap server"));

            Assert.That(osm.Relations, Is.Null);
            Assert.That(osm.User, Is.Null);
            Assert.That(osm.GpxFiles, Is.Null);
            Assert.That(osm.Nodes, Is.Null);
            Assert.That(osm.Ways, Is.Null);
            Assert.That(osm.Bounds, Is.Null);

            Assert.That(osm.Api, Is.Not.Null);
            Assert.That(osm.Api.Version, Is.Not.Null);
            Assert.That(osm.Api.Version.Maximum, Is.EqualTo(.6));
            Assert.That(osm.Api.Version.Minimum, Is.EqualTo(.6));
            Assert.That(osm.Api.Area, Is.Not.Null);
            Assert.That(osm.Api.Area.Maximum, Is.EqualTo(.25));
            Assert.That(osm.Api.NoteArea, Is.Not.Null);
            Assert.That(osm.Api.NoteArea.Maximum, Is.EqualTo(25));
            Assert.That(osm.Api.Tracepoints, Is.Not.Null);
            Assert.That(osm.Api.Tracepoints.PerPage, Is.EqualTo(5000));
            Assert.That(osm.Api.WayNodes, Is.Not.Null);
            Assert.That(osm.Api.WayNodes.Maximum, Is.EqualTo(2000));
            Assert.That(osm.Api.Changesets, Is.Not.Null);
            Assert.That(osm.Api.Changesets.MaximumElements, Is.EqualTo(10000));
            Assert.That(osm.Api.Timeout, Is.Not.Null);
            Assert.That(osm.Api.Timeout.Seconds, Is.EqualTo(300));
            Assert.That(osm.Api.Status, Is.Not.Null);
            Assert.That(osm.Api.Status.Database, Is.EqualTo(Status.ServiceStatus.online));
            Assert.That(osm.Api.Status.Api, Is.EqualTo(Status.ServiceStatus.online));
            Assert.That(osm.Api.Status.Gpx, Is.EqualTo(Status.ServiceStatus.online));

            Assert.That(osm.Policy, Is.Not.Null);
            Assert.That(osm.Policy.Imagery, Is.Not.Null);
            Assert.That(osm.Policy.Imagery.Blacklists, Is.Not.Null);
            Assert.That(osm.Policy.Imagery.Blacklists.Length, Is.EqualTo(3));
            Assert.That(osm.Policy.Imagery.Blacklists[0].Regex, Is.EqualTo(@".*\.google(apis)?\..*/(vt|kh)[\?/].*([xyz]=.*){3}.*"));
            Assert.That(osm.Policy.Imagery.Blacklists[1].Regex, Is.EqualTo(@"http://xdworld\.vworld\.kr:8080/.*"));
            Assert.That(osm.Policy.Imagery.Blacklists[2].Regex, Is.EqualTo(@".*\.here\.com[/:].*"));
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

            Assert.That(osm, Is.Not.Null);
            Assert.That(osm.Api, Is.Not.Null);
            Assert.That(osm.Api.Version, Is.Not.Null);
            Assert.That(osm.Api.Version.Maximum, Is.EqualTo(.6));
            Assert.That(osm.Api.Version.Maximum, Is.EqualTo(.6));
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

            Assert.That(osm, Is.Not.Null);
            Assert.That(osm.Permissions, Is.Not.Null);
            Assert.That(osm.Permissions.UserPermission, Is.Not.Null);
            Assert.That(osm.Permissions.UserPermission.Length, Is.EqualTo(3));
            Assert.That(osm.Permissions.UserPermission[0], Is.EqualTo(Permissions.Permission.allow_read_prefs));
            Assert.That(osm.Permissions.UserPermission[1], Is.EqualTo(Permissions.Permission.allow_read_gpx));
            Assert.That(osm.Permissions.UserPermission[2], Is.EqualTo(Permissions.Permission.allow_write_gpx));
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

            Assert.That(osm, Is.Not.Null);
            Assert.That(osm.Preferences, Is.Not.Null);
            Assert.That(osm.Preferences.UserPreferences, Is.Not.Null);
            Assert.That(osm.Preferences.UserPreferences.Length, Is.EqualTo(2));
            Assert.That(osm.Preferences.UserPreferences[0].Key, Is.EqualTo("gps.trace.visibility"));
            Assert.That(osm.Preferences.UserPreferences[0].Value, Is.EqualTo("public"));
            Assert.That(osm.Preferences.UserPreferences[1].Key, Is.EqualTo("color"));
            Assert.That(osm.Preferences.UserPreferences[1].Value, Is.EqualTo("red"));
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

            Assert.That(osm, Is.Not.Null);
            Assert.That(osm.Notes, Is.Not.Null);
            Assert.That(osm.Notes.Length, Is.EqualTo(1));

            var note = osm.Notes[0];
            Assert.That(note.Latitude, Is.EqualTo(51));
            Assert.That(note.Longitude, Is.EqualTo(0.1));
            Assert.That(note.Id, Is.EqualTo(16659));
            Assert.That(note.Url, Is.EqualTo("https://master.apis.dev.openstreetmap.org/api/0.6/notes/16659"));
            Assert.That(note.CommentUrl, Is.EqualTo("https://master.apis.dev.openstreetmap.org/api/0.6/notes/16659/comment"));
            Assert.That(note.CloseUrl, Is.EqualTo("https://master.apis.dev.openstreetmap.org/api/0.6/notes/16659/close"));
            Assert.That(note.DateCreated, Is.EqualTo(Note.ParseNoteDate("2019-06-15 08:26:04 UTC")));
            Assert.That(note.Status, Is.EqualTo(Note.NoteStatus.Open));
            Assert.That(note.Comments, Is.Not.Null);
            Assert.That(note.Comments.Comments, Is.Not.Null);
            Assert.That(note.Comments.Comments.Length, Is.EqualTo(1));

            var comment = note.Comments.Comments[0];
            Assert.That(comment.Date, Is.EqualTo(Note.ParseNoteDate("2019-06-15 08:26:04 UTC")));
            Assert.That(comment.UserId, Is.EqualTo(1234));
            Assert.That(comment.UserName, Is.EqualTo("userName"));
            Assert.That(comment.UserUrl, Is.EqualTo("https://master.apis.dev.openstreetmap.org/user/userName"));
            Assert.That(comment.Action, Is.EqualTo(Note.Comment.CommentAction.Opened));
            Assert.That(comment.Text, Is.EqualTo("ThisIsANote"));
            Assert.That(comment.HTML, Is.EqualTo("<p>ThisIsANote</p>"));
        }
    }
}