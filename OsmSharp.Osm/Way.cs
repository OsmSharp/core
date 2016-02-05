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
    /// Represents a simple way.
    /// </summary>
    [XmlRoot("way")]
    public class Way : OsmGeo, IXmlSerializable
    {
        /// <summary>
        /// Creates a new simple way.
        /// </summary>
        public Way()
        {
            this.Type = OsmGeoType.Way;
        }

        /// <summary>
        /// Holds the list of nodes.
        /// </summary>
        public List<long>  Nodes { get; set; }

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
                return string.Format("Way[null]{0}", tags);
            }
            return string.Format("Way[{0}]{1}", this.Id.Value, tags);
        }

        /// <summary>
        /// Creates a new way.
        /// </summary>
        public static Way Create(long id, params long[] nodes)
        {
            Way way = new Way();
            way.Id = id;
            way.Nodes = new List<long>(nodes);
            return way;
        }

        /// <summary>
        /// Creates a new way.
        /// </summary>
        public static Way Create(long id, TagsCollectionBase tags, params long[] nodes)
        {
            Way way = new Way();
            way.Id = id;
            way.Nodes = new List<long>(nodes);
            way.Tags = tags;
            return way;
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
                else if(reader.Name == "nd")
                {
                    if(this.Nodes == null)
                    {
                        this.Nodes = new List<long>();
                    }
                    this.Nodes.Add(reader.GetAttributeInt64("ref").Value);
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

            if (this.Nodes != null)
            {
                for (var i  = 0; i < this.Nodes.Count; i++)
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
