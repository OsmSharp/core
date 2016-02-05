// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2016 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;
using OsmSharp.Collections.Tags;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace OsmSharp.Osm
{
    /// <summary>
    /// Represents a simple relation.
    /// </summary>
    [XmlRoot("relation")]
    public class Relation : OsmGeo, IXmlSerializable
    {
        /// <summary>
        /// Creates new simple relation.
        /// </summary>
        public Relation()
        {
            this.Type = OsmGeoType.Relation;
        }

        /// <summary>
        /// The relation members.
        /// </summary>
        public List<RelationMember> Members { get; set; }

        /// <summary>
        /// Returns a description of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string tags = "{no tags}";
            if (this.Tags != null && this.Tags.Count > 0)
            {
                tags = this.Tags.ToString();
            }
            if (!this.Id.HasValue)
            {
                return string.Format("Relation[null]{0}", tags);
            }
            return string.Format("Relation[{0}]{1}", this.Id.Value, tags);
        }

        /// <summary>
        /// Creates a new relation.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="members"></param>
        /// <returns></returns>
        public static Relation Create(long id, params RelationMember[] members)
        {
            Relation relation = new Relation();
            relation.Id = id;
            relation.Members = new List<RelationMember>(members);
            return relation;
        }

        /// <summary>
        /// Creates a new relation.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tags"></param>
        /// <param name="members"></param>
        /// <returns></returns>
        public static Relation Create(long id, TagsCollectionBase tags, params RelationMember[] members)
        {
            Relation relation = new Relation();
            relation.Id = id;
            relation.Members = new List<RelationMember>(members);
            relation.Tags = tags;
            return relation;
        }

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
            while (reader.Read())
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
                    if (this.Members == null)
                    {
                        this.Members = new List<RelationMember>();
                    }
                    var member = new RelationMember();
                    (member as IXmlSerializable).ReadXml(reader);
                    this.Members.Add(member);
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
            writer.WriteAttribute("visible", this.Visible);
            writer.WriteAttribute("version", this.Version);
            writer.WriteAttribute("changeset", this.ChangeSetId);
            writer.WriteAttribute("timestamp", this.TimeStamp);

            if (this.Members != null)
            {
                for (var i = 0; i < this.Members.Count; i++)
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
}