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
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using OsmSharp.IO.Xml;
using OsmSharp.Tags;

namespace OsmSharp.Changesets
{
    /// <summary>
    /// Represents a changeset.
    /// </summary>
    [XmlRoot("changeset")]
    public partial class Changeset : IXmlSerializable
    {
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.Id = reader.GetAttributeInt64("id");
            this.UserName = reader.GetAttribute("user");
            this.UserId = reader.GetAttributeInt64("uid");
            this.CreatedAt = reader.GetAttributeDateTime("created_at");
            this.ClosedAt = reader.GetAttributeDateTime("closed_at");
            this.Open = reader.GetAttributeBool("open");
            this.MinLongitude = reader.GetAttributeSingle("min_lon");
            this.MinLatitude = reader.GetAttributeSingle("min_lat");
            this.MaxLongitude = reader.GetAttributeSingle("max_lon");
            this.MaxLatitude = reader.GetAttributeSingle("max_lat");
            this.CommentsCount = reader.GetAttributeInt32("comments_count");
            this.ChangesCount = reader.GetAttributeInt32("changes_count");

            TagsCollection tags = null;
            while (reader.Read() &&
                reader.MoveToContent() != XmlNodeType.None)
            {
                if (reader.Name == "tag")
                {
                    if (tags == null)
                    {
                        tags = new TagsCollection();
                    }
                    tags.Add(new Tag()
                    {
                        Key = reader.GetAttribute("k"),
                        Value = reader.GetAttribute("v")
                    });
                }
                else if (reader.Name == "discussion")
                {
                    this.Discussion = new Discussion();
                    (this.Discussion as IXmlSerializable).ReadXml(reader);
                }
                else
                {
                    if (tags != null)
                    {
                        this.Tags = tags;
                    }
                    return;
                }
            }
            if (tags != null)
            {
                this.Tags = tags;
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttribute("id", this.Id);
            writer.WriteAttribute("user", this.UserName);
            writer.WriteAttribute("uid", this.UserId);
            writer.WriteAttribute("created_at", this.CreatedAt);
            writer.WriteAttribute("closed_at", this.ClosedAt);
            writer.WriteAttribute("open", this.Open);
            writer.WriteAttribute("min_lon", this.MinLongitude);
            writer.WriteAttribute("min_lat", this.MinLatitude);
            writer.WriteAttribute("max_lon", this.MaxLongitude);
            writer.WriteAttribute("max_lat", this.MaxLatitude);
            writer.WriteAttribute("comments_count", this.CommentsCount);
            writer.WriteAttribute("changes_count", this.ChangesCount);

            if (this.Tags != null)
            {
                foreach (var tag in this.Tags)
                {
                    writer.WriteStartElement("tag");
                    writer.WriteAttributeString("k", tag.Key);
                    writer.WriteAttributeString("v", tag.Value);
                    writer.WriteEndElement();
                }
            }

            writer.WriteElement("discussion", this.Discussion);
        }
    }

    /// <summary>
    /// Represents a Discussion.
    /// </summary>
    [XmlRoot("discussion")]
    public partial class Discussion : IXmlSerializable
    {
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            var comments = new List<Comment>();

            reader.GetElements(
               new Tuple<string, Action>(
                   "comment", () =>
                   {
                       var comment = new Comment();
                       (comment as IXmlSerializable).ReadXml(reader);
                       comments.Add(comment);
                   })
               );

           this.Comments = comments.ToArray();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteElements("comment", this.Comments);
        }
    }

    /// <summary>
    /// Represents a Comment.
    /// </summary>
    [XmlRoot("comment")]
    public partial class Comment : IXmlSerializable
    {
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.Date = reader.GetAttributeDateTime("date");
            this.UserId = reader.GetAttributeInt64("uid");
            this.UserName = reader.GetAttribute("user");

            reader.GetElements(
               new Tuple<string, Action>(
                   "text", () =>
                   {
                       reader.Read();
                       this.Text = reader.Value;
                       reader.Read();
                   })
               );
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttribute("date", this.Date);
            writer.WriteAttribute("uid", this.UserId);
            writer.WriteAttribute("user", this.UserName);

            writer.WriteStartElement("text");
            writer.WriteString(this.Text);
            writer.WriteEndElement();
        }
    }
}