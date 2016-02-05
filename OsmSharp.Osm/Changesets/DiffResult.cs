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
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace OsmSharp.Osm.Changesets
{
    /// <summary>
    /// A diff results after applying a changeset.
    /// </summary>
    [XmlRoot("diffResult")]
    public class DiffResult : IXmlSerializable
    {
        /// <summary>
        /// Gets or sets the generator.
        /// </summary>
        public string Generator { get; set; }

        /// <summary>
        /// Gets or sets the version #.
        /// </summary>
        public double? Version { get; set; }

        /// <summary>
        /// Gets or sets the results array.
        /// </summary>
        public OsmGeoResult[] Results { get; set; }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.MoveToContent();

            this.Version = reader.GetAttributeDouble("version");
            this.Generator = reader.GetAttribute("generator");

            List<OsmGeoResult> results = null;
            while (reader.Read())
            {
                if (reader.Name == "node")
                {
                    if (results == null)
                    {
                        results = new List<OsmGeoResult>();
                    }
                    var nodeResult = new NodeResult();
                    (nodeResult as IXmlSerializable).ReadXml(reader);
                    results.Add(nodeResult);
                }
                else if (reader.Name == "way")
                {
                    if (results == null)
                    {
                        results = new List<OsmGeoResult>();
                    }
                    var wayResult = new WayResult();
                    (wayResult as IXmlSerializable).ReadXml(reader);
                    results.Add(wayResult);
                }
                else if (reader.Name == "relation")
                {
                    if (results == null)
                    {
                        results = new List<OsmGeoResult>();
                    }
                    var relationResult = new RelationResult();
                    (relationResult as IXmlSerializable).ReadXml(reader);
                    results.Add(relationResult);
                }
                else
                {
                    if (results == null)
                    {
                        results = new List<OsmGeoResult>();
                    }
                    return;
                }
            }
            if(results != null)
            {
                this.Results = results.ToArray();
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (!string.IsNullOrWhiteSpace(this.Generator))
            {
                writer.WriteAttributeString("generator", this.Generator);
            }
            if (this.Version.HasValue)
            {
                writer.WriteAttributeString("version", this.Version.Value.ToInvariantString());
            }

            if(this.Results != null)
            {
                for(var i = 0; i < this.Results.Length; i++)
                {
                    var result = this.Results[0] as IXmlSerializable;
                    if(result is NodeResult)
                    {
                        writer.WriteStartElement("node");
                        result.WriteXml(writer);
                        writer.WriteEndElement();
                    }
                    else if (result is WayResult)
                    {
                        writer.WriteStartElement("way");
                        result.WriteXml(writer);
                        writer.WriteEndElement();
                    }
                    else if (result is RelationResult)
                    {
                        writer.WriteStartElement("relation");
                        result.WriteXml(writer);
                        writer.WriteEndElement();
                    }
                }
            }
        }
    }

    /// <summary>
    /// An osmgeo result.
    /// </summary>
    public abstract class OsmGeoResult : IXmlSerializable
    {
        /// <summary>
        /// Gets or sets the old id.
        /// </summary>
        public long? OldId { get; set; }

        /// <summary>
        /// Gets or sets the new id.
        /// </summary>
        public long? NewId { get; set; }

        /// <summary>
        /// Gets or sets the new version #.
        /// </summary>
        public int? NewVersion { get; set; }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.MoveToContent();

            var newIdString = reader.GetAttribute("new_id");
            long newId = 0;
            if (!string.IsNullOrWhiteSpace(newIdString) &&
               long.TryParse(newIdString, NumberStyles.Any, CultureInfo.InvariantCulture, out newId))
            {
                this.NewId = newId;
            }

            var oldIdString = reader.GetAttribute("old_id");
            long oldId = 0;
            if (!string.IsNullOrWhiteSpace(oldIdString) &&
               long.TryParse(oldIdString, NumberStyles.Any, CultureInfo.InvariantCulture, out oldId))
            {
                this.OldId = oldId;
            }

            var versionString = reader.GetAttribute("new_version");
            int version = 0;
            if (!string.IsNullOrWhiteSpace(versionString) &&
               int.TryParse(versionString, NumberStyles.Any, CultureInfo.InvariantCulture, out version))
            {
                this.NewVersion = version;
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (this.OldId.HasValue)
            {
                writer.WriteAttributeString("old_id", this.OldId.Value.ToInvariantString());
            }
            if (this.NewId.HasValue)
            {
                writer.WriteAttributeString("new_id", this.NewId.Value.ToInvariantString());
            }
            if (this.NewVersion.HasValue)
            {
                writer.WriteAttributeString("new_version", this.NewVersion.Value.ToInvariantString());
            }
        }
    }

    /// <summary>
    /// A node result.
    /// </summary>
    public class NodeResult : OsmGeoResult
    {

    }

    /// <summary>
    /// A way result.
    /// </summary>
    public class WayResult : OsmGeoResult
    {

    }

    /// <summary>
    /// A relation result.
    /// </summary>
    public class RelationResult : OsmGeoResult
    {

    }
}
