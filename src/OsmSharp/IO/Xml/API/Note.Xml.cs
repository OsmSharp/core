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

using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using OsmSharp.IO.Xml;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace OsmSharp.API
{
    /// <summary>
    /// Represents the Note object.
    /// </summary>
    [XmlRoot("note")]
    public partial class Note : IXmlSerializable
    {
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.Latitude = reader.GetAttributeDouble("lat");
            this.Longitude = reader.GetAttributeDouble("lon");

            reader.GetElements(
                new Tuple<string, Action>(
                    "id", () =>
                    {
                        this.Id = reader.ReadElementContentAsLong();
                    }),
                new Tuple<string, Action>(
                    "url", () =>
                    {
                        this.Url = reader.ReadElementContentAsString();
                    }),
                new Tuple<string, Action>(
                    "comment_url", () =>
                    {
                        this.CommentUrl = reader.ReadElementContentAsString();
                    }),
                new Tuple<string, Action>(
                    "close_url", () =>
                    {
                        this.CloseUrl = reader.ReadElementContentAsString();
                    }),
                new Tuple<string, Action>(
                    "date_created", () =>
                    {
                        var valueString = reader.ReadElementContentAsString();
                        this.DateCreated = ParseNoteDate(valueString);
                    }),
                new Tuple<string, Action>(
                    "status", () =>
                    {
                        this.Status = reader.ReadElementContentAsEnum<NoteStatus>();
                    }),
                new Tuple<string, Action>(
                    "comments", () =>
                    {
                        this.Comments = new CommentsContainer();
                        (this.Comments as IXmlSerializable).ReadXml(reader);
                    })
            );
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttribute("lat", this.Latitude);
            writer.WriteAttribute("lon", this.Longitude);
            writer.WriteStartAndEndElementWithContent("id", this.Id?.ToString());
            writer.WriteStartAndEndElementWithContent("url", this.Url);
            writer.WriteStartAndEndElementWithContent("comment_url", this.CommentUrl);
            writer.WriteStartAndEndElementWithContent("close_url", this.CloseUrl);
            writer.WriteStartAndEndElementWithContent("date_created", this.DateCreated?.ToString());
            writer.WriteStartAndEndElementWithContent("status", this.Status?.ToString().ToLower());
            writer.WriteElement("comments", this.Comments);
        }

        [XmlRoot("comments")]
        public partial class CommentsContainer : IXmlSerializable
        {
            public XmlSchema GetSchema()
            {
                return null;
            }

            public void ReadXml(XmlReader reader)
            {
                var comments = new List<Comment>();

                reader.GetElements(
                    new Tuple<string, Action>(
                        "comment", () =>
                        {
                            var comment = new Comment();
                            (comment as IXmlSerializable).ReadXml(reader);
                            comments.Add(comment);
                        }));

                if (comments.Count > 0)
                {
                    this.Comments = comments.ToArray();
                }
            }

            public void WriteXml(XmlWriter writer)
            {
                writer.WriteElements("comment", this.Comments);
            }
        }

        [XmlRoot("comment")]
        public partial class Comment : IXmlSerializable
        {
            public XmlSchema GetSchema()
            {
                return null;
            }

            public void ReadXml(XmlReader reader)
            {
                reader.GetElements(
                    new Tuple<string, Action>(
                        "date", () =>
                        {
                            var valueString = reader.ReadElementContentAsString();
                            this.Date = ParseNoteDate(valueString);
                        }),
                    new Tuple<string, Action>(
                        "uid", () =>
                        {
                            this.UserId = reader.ReadElementContentAsLong();
                        }),
                    new Tuple<string, Action>(
                        "user", () =>
                        {
                            this.UserName = reader.ReadElementContentAsString();
                        }),
                    new Tuple<string, Action>(
                        "user_url", () =>
                        {
                            this.UserUrl = reader.ReadElementContentAsString();
                        }),
                    new Tuple<string, Action>(
                        "action", () =>
                        {
                            this.Action = reader.ReadElementContentAsEnum<CommentAction>();
                        }),
                    new Tuple<string, Action>(
                        "text", () =>
                        {
                            this.Text = reader.ReadElementContentAsString();
                        }),
                    new Tuple<string, Action>(
                        "html", () =>
                        {
                            this.HTML = reader.ReadElementContentAsString();
                        }));
            }

            public void WriteXml(XmlWriter writer)
            {
                writer.WriteStartAndEndElementWithContent("date", this.Date?.ToString());
                writer.WriteStartAndEndElementWithContent("uid", this.UserId?.ToString());
                writer.WriteStartAndEndElementWithContent("user", this.UserName?.ToString());
                writer.WriteStartAndEndElementWithContent("user_url", this.UserUrl?.ToString());
                writer.WriteStartAndEndElementWithContent("action", this.Action?.ToString().ToLower());
                writer.WriteStartAndEndElementWithContent("text", this.Text);
                writer.WriteStartAndEndElementWithContent("html", this.HTML);
            }
        }

        // Note dates are in their own special format.
        public static DateTime? ParseNoteDate(string dateString)
        {
            if (DateTime.TryParseExact(dateString, "yyyy-MM-dd HH:mm:ss' UTC'",
                CultureInfo.CurrentCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out DateTime date))
            {
                return date;
            }
            return null;
        }
    }
}