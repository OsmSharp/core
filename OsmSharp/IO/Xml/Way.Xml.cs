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
using OsmSharp.Tags;
using System.Collections.Generic;

namespace OsmSharp
{
    /// <summary>
    /// Represents a way.
    /// </summary>
    [XmlRoot("way")]
    public partial class Way : IXmlSerializable
    {
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.Id = reader.GetAttributeInt64("id");
            this.Version = reader.GetAttributeInt32("version");
            this.ChangeSetId = reader.GetAttributeInt64("changeset");
            this.TimeStamp = reader.GetAttributeDateTime("timestamp");
            this.UserId = reader.GetAttributeInt32("uid");
            this.UserName = reader.GetAttribute("user");
            this.Visible = reader.GetAttributeBool("visible");

            TagsCollection tags = null;
            var nodes = new List<long>();
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
                else if (reader.Name == "nd")
                {
                    if (nodes == null)
                    {
                        nodes = new List<long>();
                    }
                    nodes.Add(reader.GetAttributeInt64("ref").Value);
                }
                else
                {
                    if (tags != null)
                    {
                        this.Tags = tags;
                    }
                    if (nodes != null)
                    {
                        this.Nodes = nodes.ToArray();
                    }
                    return;
                }
            }
            if (tags != null)
            {
                this.Tags = tags;
            }
            if (nodes != null)
            {
                this.Nodes = nodes.ToArray();
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttribute("id", this.Id);
            writer.WriteAttribute("user", this.UserName);
            writer.WriteAttribute("uid", this.UserId);
            writer.WriteAttribute("visible", this.Visible);
            writer.WriteAttribute("version", this.Version);
            writer.WriteAttribute("changeset", this.ChangeSetId);
            writer.WriteAttribute("timestamp", this.TimeStamp);

            if (this.Nodes != null)
            {
                for (var i = 0; i < this.Nodes.Length; i++)
                {
                    writer.WriteStartElement("nd");
                    writer.WriteAttribute("ref", this.Nodes[i]);
                    writer.WriteEndElement();
                }
            }

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
        }
    }
}
