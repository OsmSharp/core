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

using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using NUnit.Framework;
using OsmSharp.API;
using OsmSharp.IO.Xml;

namespace OsmSharp.Test.IO.Xml.API;


/// <summary>
/// Contains tests for the gpx_file object.
/// </summary>
[TestFixture]
public class GpxFileTests
{
    [Test]
    public void TestSerialize()
    {
        var osm = new Osm
        {
            Version = 0.6,
            Generator = "OpenStreetMap server",
            GpxFiles = new[] {new GpxFile
                {
                    Id = 1,
                    Name = "Name",
                    Lat = 1.1,
                    Lon = 2.2,
                    User = "User",
                    Visibility = Visibility.Private,
                    Pending = false,
                    TimeStamp = new DateTime(1947, 11, 29, 12, 0, 0, DateTimeKind.Utc),
                    Description = "Description",
                    Tags = new [] {"tag1", "tag2"},
                }
            }
        };
        var osmString = osm.SerializeToXml();

        Assert.That(
            osmString, Is.EqualTo("<osm version=\"0.6\" generator=\"OpenStreetMap server\"><gpx_file id=\"1\" name=\"Name\" lat=\"1.1\" lon=\"2.2\" user=\"User\" visibility=\"private\" pending=\"False\" timestamp=\"1947-11-29T12:00:00Z\"><description>Description</description><tag>tag1</tag><tag>tag2</tag></gpx_file></osm>"));
    }

    [Test]
    public void TestDeserialize()
    {
        var serializer = new XmlSerializer(typeof(Osm));
        var osm = serializer.Deserialize(
                new StringReader(
                    "<osm version=\"0.6\" generator=\"OpenStreetMap server\"><gpx_file id=\"1\" name=\"Name\" lat=\"1.1\" lon=\"2.2\" user=\"User\" visibility=\"private\" pending=\"False\" timestamp=\"1947-11-29T10:00:00Z\"><description>Description</description><tag>tag1</tag><tag>tag2</tag></gpx_file></osm>"))
            as Osm;
        Assert.IsNotNull(osm.GpxFiles);
        var gpxFile = osm.GpxFiles.First();
        Assert.That(gpxFile.Id, Is.EqualTo(1));
        Assert.That(gpxFile.Name, Is.EqualTo("Name"));
        Assert.That(gpxFile.Lat, Is.EqualTo(1.1));
        Assert.That(gpxFile.Lon, Is.EqualTo(2.2));
        Assert.That(gpxFile.User, Is.EqualTo("User"));
        Assert.That(gpxFile.Visibility, Is.EqualTo(Visibility.Private));
        Assert.That(gpxFile.Pending, Is.EqualTo(false));
        Assert.That(gpxFile.TimeStamp.Year, Is.EqualTo(1947));
        Assert.That(gpxFile.Description, Is.EqualTo("Description"));
        Assert.That(gpxFile.Tags.Length, Is.EqualTo(2));
        Assert.That(gpxFile.Tags.First(), Is.EqualTo("tag1"));
    }
}
