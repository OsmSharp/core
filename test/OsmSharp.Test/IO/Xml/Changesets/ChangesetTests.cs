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
using OsmSharp.IO.Xml;
using OsmSharp.Changesets;
using System.Xml.Serialization;
using System.IO;
using OsmSharp.Tags;

namespace OsmSharp.Test.IO.Xml.Changesets
{
    /// <summary>
    /// Contains tests for the changeset class.
    /// </summary>
    [TestFixture]
    public class ChangesetTests
    {
        /// <summary>
        /// Tests serialization.
        /// </summary>
        [Test]
        public void TestSerialize()
        {
            var changeset = new Changeset()
            {
                Id = 10,
                MinLatitude = 49.2785426f,
                MinLongitude = 7.0191821f,
                MaxLatitude = 49.2793101f,
                MaxLongitude = 7.0197485f,
                Open = true,
                CreatedAt = new System.DateTime(2008, 11, 08, 19, 07, 39),
                UserId = 123,
                UserName = "fred",
                Tags = new TagsCollection(
                    new Tag("created_by", "JOSM 1.61"),
                    new Tag("comment", "Just adding some streetnames"))
            };

            var result = changeset.SerializeToXml();
            Assert.AreEqual("<changeset id=\"10\" user=\"fred\" uid=\"123\" created_at=\"2008-11-08T19:07:39Z\" open=\"true\" min_lon=\"7.019182\" min_lat=\"49.27854\" max_lon=\"7.019749\" max_lat=\"49.27931\"><tag k=\"created_by\" v=\"JOSM 1.61\" /><tag k=\"comment\" v=\"Just adding some streetnames\" /></changeset>",
                result);
        }

        /// <summary>
        /// Tests deserialization.
        /// </summary>
        [Test]
        public void TestDeserialize()
        {
            var serializer = new XmlSerializer(typeof(Changeset));

            var changeset = serializer.Deserialize(
                new StringReader("<changeset id=\"10\"></changeset>")) as Changeset;
            Assert.IsNotNull(changeset);
            Assert.AreEqual(10, changeset.Id);

            changeset = serializer.Deserialize(
                new StringReader("<changeset id=\"10\" user=\"fred\" uid=\"123\" created_at=\"2008-11-08T19:07:39Z\" open=\"true\" min_lon=\"7.019182\" min_lat=\"49.27854\" max_lon=\"7.019749\" max_lat=\"49.27931\"><tag k=\"created_by\" v=\"JOSM 1.61\" /><tag k=\"comment\" v=\"Just adding some streetnames\" /></changeset>")) as Changeset;
            Assert.IsNotNull(changeset);
            Assert.AreEqual(10, changeset.Id);
            Assert.AreEqual(123, changeset.UserId);
            Assert.AreEqual("fred", changeset.UserName);
            Assert.AreEqual(new System.DateTime(2008, 11, 08, 19, 07, 39), changeset.CreatedAt.Value.ToUniversalTime());
            Assert.IsNull(changeset.ClosedAt);
            Assert.AreEqual(true, changeset.Open);
            Assert.AreEqual(7.0191821f, changeset.MinLongitude, 0.00001f);
            Assert.AreEqual(49.2785426f, changeset.MinLatitude, 0.00001f);
            Assert.AreEqual(7.0197485f, changeset.MaxLongitude, 0.00001f);
            Assert.AreEqual(49.27931011f, changeset.MaxLatitude, 0.00001f);

            Assert.IsNotNull(changeset.Tags);
            Assert.AreEqual(2, changeset.Tags.Count);
            Assert.IsTrue(changeset.Tags.Contains("created_by", "JOSM 1.61"));
            Assert.IsTrue(changeset.Tags.Contains("comment", "Just adding some streetnames"));
        }
    }
}