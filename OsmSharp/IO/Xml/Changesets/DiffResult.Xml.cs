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

using OsmSharp.IO.Xml;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System;

namespace OsmSharp.Changesets
{
    /// <summary>
    /// Represents a diff result after applying a changeset.
    /// </summary>
    [XmlRoot("diffResult")]
    public partial class DiffResult : IXmlSerializable
    {
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
            while (reader.Read() &&
                reader.MoveToContent() != XmlNodeType.None)
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
            if (results != null)
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

            if (this.Results != null)
            {
                for (var i = 0; i < this.Results.Length; i++)
                {
                    var result = this.Results[0] as IXmlSerializable;
                    if (result is NodeResult)
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
    public abstract partial class OsmGeoResult : IXmlSerializable
    {
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

        /// <summary>
        /// Creates a modification.
        /// </summary>
        public static OsmGeoResult CreateModification(OsmGeo modify, int newVersion)
        {
            switch(modify.Type)
            {
                case OsmGeoType.Node:
                    return new NodeResult()
                    {
                        NewId = modify.Id,
                        OldId = modify.Id,
                        NewVersion = newVersion
                    };
                case OsmGeoType.Way:
                    return new WayResult()
                    {
                        NewId = modify.Id,
                        OldId = modify.Id,
                        NewVersion = newVersion
                    };
                case OsmGeoType.Relation:
                    return new RelationResult()
                    {
                        NewId = modify.Id,
                        OldId = modify.Id,
                        NewVersion = newVersion
                    };
            }
            throw new Exception("Invalid OsmGeo type.");
        }

        /// <summary>
        /// Creates a creation.
        /// </summary>
        public static OsmGeoResult CreateCreation(OsmGeo create, long newId)
        {
            switch (create.Type)
            {
                case OsmGeoType.Node:
                    return new NodeResult()
                    {
                        NewId = newId,
                        OldId = create.Id,
                        NewVersion = 1
                    };
                case OsmGeoType.Way:
                    return new WayResult()
                    {
                        NewId = newId,
                        OldId = create.Id,
                        NewVersion = 1
                    };
                case OsmGeoType.Relation:
                    return new RelationResult()
                    {
                        NewId = newId,
                        OldId = create.Id,
                        NewVersion = 1
                    };
            }
            throw new Exception("Invalid OsmGeo type.");
        }

        /// <summary>
        /// Creates a deletion
        /// </summary>
        public static OsmGeoResult CreateDeletion(OsmGeo delete)
        {
            switch (delete.Type)
            {
                case OsmGeoType.Node:
                    return new NodeResult()
                    {
                        NewId = null,
                        OldId = delete.Id.Value,
                        NewVersion = null
                    };
                case OsmGeoType.Way:
                    return new WayResult()
                    {
                        NewId = null,
                        OldId = delete.Id.Value,
                        NewVersion = null
                    };
                case OsmGeoType.Relation:
                    return new RelationResult()
                    {
                        NewId = null,
                        OldId = delete.Id.Value,
                        NewVersion = null
                    };
            }
            throw new Exception("Invalid OsmGeo type.");
        }
    }
}