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
using System;
using System.Globalization;
using OsmSharp.API;

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
            Assert.That(result, Is.EqualTo("<changeset id=\"10\" user=\"fred\" uid=\"123\" created_at=\"2008-11-08T19:07:39Z\" open=\"true\" min_lon=\"7.0191822\" min_lat=\"49.2785416\" max_lon=\"7.0197487\" max_lat=\"49.2793083\"><tag k=\"created_by\" v=\"JOSM 1.61\" /><tag k=\"comment\" v=\"Just adding some streetnames\" /></changeset>"));
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
            Assert.That(changeset, Is.Not.Null);
            Assert.That(changeset.Id, Is.EqualTo(10));

            changeset = serializer.Deserialize(
                new StringReader("<changeset id=\"10\" user=\"fred\" uid=\"123\" created_at=\"2008-11-08T19:07:39Z\" open=\"true\" min_lon=\"7.019182\" min_lat=\"49.27854\" max_lon=\"7.019749\" max_lat=\"49.27931\"><tag k=\"created_by\" v=\"JOSM 1.61\" /><tag k=\"comment\" v=\"Just adding some streetnames\" /></changeset>")) as Changeset;
            Assert.That(changeset, Is.Not.Null);
            Assert.That(changeset.Id, Is.EqualTo(10));
            Assert.That(changeset.UserId, Is.EqualTo(123));
            Assert.That(changeset.UserName, Is.EqualTo("fred"));
            Assert.That(changeset.CreatedAt.Value.ToUniversalTime(), Is.EqualTo(new System.DateTime(2008, 11, 08, 19, 07, 39)));
            Assert.That(changeset.ClosedAt, Is.Null);
            Assert.That(changeset.Open, Is.EqualTo(true));
            Assert.That(changeset.MinLongitude, Is.EqualTo(7.0191821f).Within(0.00001f));
            Assert.That(changeset.MinLatitude, Is.EqualTo(49.2785426f).Within(0.00001f));
            Assert.That(changeset.MaxLongitude, Is.EqualTo(7.0197485f).Within(0.00001f));
            Assert.That(changeset.MaxLatitude, Is.EqualTo(49.27931011f).Within(0.00001f));

            Assert.That(changeset.Tags, Is.Not.Null);
            Assert.That(changeset.Tags.Count, Is.EqualTo(2));
            Assert.That(changeset.Tags.Contains("created_by", "JOSM 1.61"), Is.True);
            Assert.That(changeset.Tags.Contains("comment", "Just adding some streetnames"), Is.True);

            Assert.That(changeset.Discussion, Is.Null);
        }

        /// <summary>
        /// Tests deserialization of a changeset that has a discussion.
        /// </summary>
        [Test]
        public void TestDeserializeDiscussion()
        {
            var xml =
@"<osm>
  <changeset id=""10"" user=""fred"" uid=""123"" created_at=""2008-11-08T19:07:39+01:00"" open=""true"" min_lon=""7.0191821"" min_lat=""49.2785426"" max_lon=""7.0197485"" max_lat=""49.2793101"" comments_count=""2"" changes_count=""5"">
    <tag k=""created_by"" v=""JOSM 1.61""/>
    <tag k=""comment"" v=""Just adding some streetnames""/>
    <discussion>
     <comment date=""2015-01-01T18:56:48Z"" uid=""1841"" user=""metaodi"">
       <text>Did you verify those street names?</text>
     </comment>
     <comment date=""2015-01-01T18:58:03Z"" uid=""123"" user=""fred"">
       <text>sure!</text>
     </comment>
   </discussion>
 </changeset>
</osm>
";
            var serializer = new XmlSerializer(typeof(Osm));

            var osm = serializer.Deserialize(new StringReader(xml)) as Osm;
            Func<string, DateTime> parseToUniversalTime =
                t => DateTime.Parse(t, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

            Assert.That(osm.Changesets, Is.Not.Null);
            Assert.That(osm.Changesets.Length, Is.EqualTo(1));
            var changeset = osm.Changesets[0];
            Assert.That(changeset, Is.Not.Null);
            Assert.That(changeset.Id, Is.EqualTo(10));
            Assert.That(changeset.UserName, Is.EqualTo("fred"));
            Assert.That(changeset.UserId, Is.EqualTo(123));
            Assert.That(changeset.CreatedAt, Is.EqualTo(parseToUniversalTime("2008-11-08T19:07:39+01:00")));
            Assert.That(changeset.Open, Is.EqualTo(true));
            Assert.That(changeset.MinLongitude, Is.EqualTo(7.0191821f).Within(0.00001f));
            Assert.That(changeset.MinLatitude, Is.EqualTo(49.2785426f).Within(0.00001f));
            Assert.That(changeset.MaxLongitude, Is.EqualTo(7.0197485f).Within(0.00001f));
            Assert.That(changeset.MaxLatitude, Is.EqualTo(49.27931011f).Within(0.00001f));
            Assert.That(changeset.CommentsCount, Is.EqualTo(2));
            Assert.That(changeset.ChangesCount, Is.EqualTo(5));
            Assert.That(changeset.ClosedAt, Is.Null);

            Assert.That(changeset.Tags, Is.Not.Null);
            Assert.That(changeset.Tags.Count, Is.EqualTo(2));
            Assert.That(changeset.Tags.Contains("created_by", "JOSM 1.61"), Is.True);
            Assert.That(changeset.Tags.Contains("comment", "Just adding some streetnames"), Is.True);

            Assert.That(changeset.Discussion, Is.Not.Null);
            Assert.That(changeset.Discussion.Comments, Is.Not.Null);
            var comments = changeset.Discussion.Comments;
            Assert.That(comments.Length, Is.EqualTo(2));
            Assert.That(comments[0].Date, Is.EqualTo(parseToUniversalTime("2015-01-01T18:56:48Z")));
            Assert.That(comments[0].UserId, Is.EqualTo(1841));
            Assert.That(comments[0].UserName, Is.EqualTo("metaodi"));
            Assert.That(comments[0].Text, Is.EqualTo("Did you verify those street names?"));
            Assert.That(comments[1].Date, Is.EqualTo(parseToUniversalTime("2015-01-01T18:58:03Z")));
            Assert.That(comments[1].UserId, Is.EqualTo(123));
            Assert.That(comments[1].UserName, Is.EqualTo("fred"));
            Assert.That(comments[1].Text, Is.EqualTo("sure!"));
        }
    }
}