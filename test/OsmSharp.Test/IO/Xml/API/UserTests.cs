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

using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using NUnit.Framework;
using OsmSharp.API;
using OsmSharp.IO.Xml;

namespace OsmSharp.Test.IO.Xml.API;

/// <summary>
/// Contains tests for the user object.
/// </summary>
[TestFixture]
public class UserTests
{
    /// <summary>
    /// Tests serializing the user object.
    /// </summary>
    [Test]
    public void TestSerialize()
    {
        var osm = new Osm
        {
            Version = 0.6,
            Generator = "OpenStreetMap server",
            User = new User()
            {
                Id = 1,
                AccountCreated = new DateTime(2000, 1, 1),
                ChangeSetCount = 2,
                DisplayName = "DisplayName",
                Description = "Description",
                Home = new Home { Latitude = 3, Longitude = 4, Zoom = 5.5f },
                Image = "Image",
                TraceCount = 6,
                ContributorTermsAgreed = false,
                ContributorTermsPublicDomain = true,
                BlocksReceived = new[]
                {
                    new Block
                    {
                        Active = 7,
                        Count = 8
                    }
                },
                Languages = new[]
                {
                    "en-US"
                },
                Messages = new Messages
                {
                    Received = 9,
                    Unread = 10,
                    Sent = 11
                }
            }
        };
        var osmString = osm.SerializeToXml();

        Assert.That(osmString, Is.EqualTo("<osm version=\"0.6\" generator=\"OpenStreetMap server\"><user id=\"1\" display_name=\"DisplayName\" account_created=\"2000-01-01T00:00:00Z\"><description>Description</description><contributor-terms agreed=\"false\" pd=\"true\" /><img href=\"Image\" /><roles></roles><changesets count=\"2\" /><traces count=\"6\" /><blocks><received count=\"8\" active=\"7\" /></blocks><home lat=\"3\" lon=\"4\" zoom=\"5.5\" /><languages><lang>en-US</lang></languages><messages><received count=\"9\" unread=\"10\" /><sent count=\"11\" /></messages></user></osm>"));
    }

    /// <summary>
    /// Tests deserializing the user object.
    /// </summary>
    [Test]
    public void TestDeserialize()
    {
        var serializer = new XmlSerializer(typeof(Osm));
        var osm = serializer.Deserialize(
            new StringReader("<osm version=\"0.6\" generator=\"OpenStreetMap server\"><user id=\"111\" display_name=\"Test\" account_created=\"2000-01-01T00:00:00Z\"><description>Hello</description><contributor-terms agreed=\"true\" pd=\"false\"/><img href=\"Image\"/><roles></roles><changesets count=\"1\"/><traces count=\"2\"/><blocks><received count=\"3\" active=\"4\"/></blocks><home lat=\"5.5\" lon=\"6.6\" zoom=\"7.7\"/><languages><lang>he</lang><lang>en-US</lang><lang>en</lang></languages><messages><received count=\"8\" unread=\"9\"/><sent count=\"10\"/></messages></user></osm>"))
                as Osm;
        Assert.IsNotNull(osm.User);
        Assert.That(osm.User.Id, Is.EqualTo(111));
        Assert.That(osm.User.DisplayName, Is.EqualTo("Test"));
        Assert.That(osm.User.AccountCreated, Is.EqualTo(new DateTime(2000, 1, 1, 0, 0, 0)));
        Assert.That(osm.User.Description, Is.EqualTo("Hello"));
        Assert.IsTrue(osm.User.ContributorTermsAgreed);
        Assert.IsFalse(osm.User.ContributorTermsPublicDomain);
        Assert.That(osm.User.Image, Is.EqualTo("Image"));
        Assert.That(osm.User.ChangeSetCount, Is.EqualTo(1));
        Assert.That(osm.User.TraceCount, Is.EqualTo(2));
        Assert.That(osm.User.BlocksReceived.Length, Is.EqualTo(1));
        Assert.That(osm.User.BlocksReceived.First().Count, Is.EqualTo(3));
        Assert.That(osm.User.BlocksReceived.First().Active, Is.EqualTo(4));
        Assert.That(osm.User.Home.Latitude, Is.EqualTo(5.5f));
        Assert.That(osm.User.Home.Longitude, Is.EqualTo(6.6f));
        Assert.That(osm.User.Home.Zoom, Is.EqualTo(7.7f));
        Assert.That(osm.User.Languages.Length, Is.EqualTo(3));
        Assert.That(osm.User.Languages.First(), Is.EqualTo("he"));
        Assert.That(osm.User.Messages.Received, Is.EqualTo(8));
        Assert.That(osm.User.Messages.Unread, Is.EqualTo(9));
        Assert.That(osm.User.Messages.Sent, Is.EqualTo(10));
    }
}
