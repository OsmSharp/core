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

namespace OsmSharp.Test.IO.Xml.API
{
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
                    Home = new Home {Latitude = 3, Longitude = 4, Zoom = 5.5f},
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
                    Languages = new []
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

            Assert.AreEqual("<osm version=\"0.6\" generator=\"OpenStreetMap server\"><user id=\"1\" display_name=\"DisplayName\" account_created=\"2000-01-01T00:00:00Z\"><description>Description</description><contributor-terms agreed=\"false\" pd=\"true\" /><img href=\"Image\" /><roles></roles><changesets count=\"2\" /><traces count=\"6\" /><blocks><received count=\"8\" active=\"7\" /></blocks><home lat=\"3\" lon=\"4\" zoom=\"5.5\" /><languages><lang>en-US</lang></languages><messages><received count=\"9\" unread=\"10\" /><sent count=\"11\" /></messages></user></osm>",
                osmString);
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
            Assert.AreEqual(111, osm.User.Id);
            Assert.AreEqual("Test", osm.User.DisplayName);
            Assert.AreEqual(new DateTime(2000, 1, 1, 1, 0, 0), osm.User.AccountCreated);
            Assert.AreEqual("Hello", osm.User.Description);
            Assert.IsTrue(osm.User.ContributorTermsAgreed);
            Assert.IsFalse(osm.User.ContributorTermsPublicDomain);
            Assert.AreEqual("Image", osm.User.Image);
            Assert.AreEqual(1, osm.User.ChangeSetCount);
            Assert.AreEqual(2, osm.User.TraceCount);
            Assert.AreEqual(1, osm.User.BlocksReceived.Length);
            Assert.AreEqual(3, osm.User.BlocksReceived.First().Count);
            Assert.AreEqual(4, osm.User.BlocksReceived.First().Active);
            Assert.AreEqual(5.5f, osm.User.Home.Latitude);
            Assert.AreEqual(6.6f, osm.User.Home.Longitude);
            Assert.AreEqual(7.7f, osm.User.Home.Zoom);
            Assert.AreEqual(3, osm.User.Languages.Length);
            Assert.AreEqual("he", osm.User.Languages.First());
            Assert.AreEqual(8, osm.User.Messages.Received);
            Assert.AreEqual(9, osm.User.Messages.Unread);
            Assert.AreEqual(10, osm.User.Messages.Sent);
        }
    }
}
