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
    /// Represents a relation.
    /// </summary>
    [XmlRoot("relation")]
    public partial class Relation : IXmlSerializable
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
            var members = new List<RelationMember>();
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
                else if (reader.Name == "member")
                {
                    if (members == null)
                    {
                        members = new List<RelationMember>();
                    }
                    var member = new RelationMember();
                    (member as IXmlSerializable).ReadXml(reader);
                    members.Add(member);
                }
                else
                {
                    if (tags != null)
                    {
                        this.Tags = tags;
                    }
                    if (members != null)
                    {
                        this.Members = members.ToArray();
                    }
                    return;
                }
            }
            if (tags != null)
            {
                this.Tags = tags;
            }
            if (members != null)
            {
                this.Members = members.ToArray();
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

            if (this.Members != null)
            {
                for (var i = 0; i < this.Members.Length; i++)
                {
                    writer.WriteStartElement("member");
                    (this.Members[i] as IXmlSerializable).WriteXml(writer);
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

    /// <summary>
    /// Represents a relation member.
    /// </summary>
    public partial class RelationMember : IXmlSerializable
    {
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.MoveToContent();

            this.Id = reader.GetAttributeInt64("ref").Value;
            this.Role = reader.GetAttribute("role");
            var type = reader.GetAttribute("type");
            switch (type)
            {
                case "node":
                    this.Type = OsmGeoType.Node;
                    break;
                case "way":
                    this.Type = OsmGeoType.Way;
                    break;
                case "relation":
                    this.Type = OsmGeoType.Relation;
                    break;
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            switch (this.Type)
            {
                case OsmGeoType.Node:
                    writer.WriteAttribute("type", "node");
                    break;
                case OsmGeoType.Way:
                    writer.WriteAttribute("type", "way");
                    break;
                case OsmGeoType.Relation:
                    writer.WriteAttribute("type", "relation");
                    break;
            }
            writer.WriteAttribute("ref", this.Id);
            writer.WriteAttribute("role", this.Role);
        }
    }
}