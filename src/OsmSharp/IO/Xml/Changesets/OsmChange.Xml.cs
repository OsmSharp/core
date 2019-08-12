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
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace OsmSharp.Changesets
{
    /// <summary>
    /// Represents an OSM change.
    /// </summary>
    [XmlRoot("osmChange")]
    public partial class OsmChange : IXmlSerializable
    {
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.MoveToContent();

            this.Generator = reader.GetAttribute("generator");
            this.Version = reader.GetAttributeDouble("version");
            this.Copyright = reader.GetAttribute("copyright");
            this.Attribution = reader.GetAttribute("attribution");
            this.License = reader.GetAttribute("license");

            List<OsmGeo> creates = new List<OsmGeo>(); ;
            List<OsmGeo> modifies = new List<OsmGeo>(); ;
            List<OsmGeo> deletes = new List<OsmGeo>(); ;

            reader.GetElements(
                new Tuple<string, Action>(
                    "create", () =>
                    {
						if (reader.IsEmptyElement)
						{
							reader.Read();
							return;
						}
                        reader.Read();
                        while ((reader.Name == "node" ||
                             reader.Name == "way" ||
                             reader.Name == "relation"))
                        {
                            creates.Add(OsmChange.ReadOsmGeo(reader));
                            if (reader.NodeType == XmlNodeType.EndElement && (reader.Name == "node" ||
                                 reader.Name == "way" ||
                                 reader.Name == "relation"))
                            {
                                reader.Read();
                            }
                        }
                        reader.Read();
                    }),
                new Tuple<string, Action>(
                    "modify", () =>
                    {
						if (reader.IsEmptyElement)
						{
							reader.Read();
							return;
						}
                        reader.Read();
                        while ((reader.Name == "node" ||
                             reader.Name == "way" ||
                             reader.Name == "relation"))
                        {
                            modifies.Add(OsmChange.ReadOsmGeo(reader));
                            if (reader.NodeType == XmlNodeType.EndElement && (reader.Name == "node" ||
                                 reader.Name == "way" ||
                                 reader.Name == "relation"))
                            {
                                reader.Read();
                            }
                        }
                        reader.Read();
                    }),
                new Tuple<string, Action>(
                    "delete", () =>
                    {
						if (reader.IsEmptyElement)
						{
							reader.Read();
							return;
						}
                        reader.Read();
                        while ((reader.Name == "node" ||
                             reader.Name == "way" ||
                             reader.Name == "relation"))
                        {
                            deletes.Add(OsmChange.ReadOsmGeo(reader));
                            if (reader.NodeType == XmlNodeType.EndElement && (reader.Name == "node" ||
                                 reader.Name == "way" ||
                                 reader.Name == "relation"))
                            {
                                reader.Read();
                            }
                        }
                        reader.Read();
                    }));

            this.Create = creates.ToArray();
            this.Modify = modifies.ToArray();
            this.Delete = deletes.ToArray();
        }

        private static OsmGeo ReadOsmGeo(XmlReader reader)
        {
            OsmGeo osmGeo = null;
            if (reader.Name == "node")
            {
                osmGeo = new Node();
            }
            else if (reader.Name == "way")
            {
                osmGeo = new Way();
            }
            else if (reader.Name == "relation")
            {
                osmGeo = new Relation();
            }
            (osmGeo as IXmlSerializable).ReadXml(reader);
            return osmGeo;
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttribute("generator", this.Generator);
            writer.WriteAttribute("version", this.Version);
            writer.WriteAttribute("copyright", this.Copyright);
            writer.WriteAttribute("attribution", this.Attribution);
            writer.WriteAttribute("license", this.License);

            if (this.Create != null)
            {
                writer.WriteStartElement("create");
                for (var i = 0; i < this.Create.Length; i++)
                {
                    OsmChange.WriteOsmGeo(writer, this.Create[i]);
                }
                writer.WriteEndElement();
            }
            if (this.Modify != null)
            {
                writer.WriteStartElement("modify");
                for (var i = 0; i < this.Modify.Length; i++)
                {
                    OsmChange.WriteOsmGeo(writer, this.Modify[i]);
                }
                writer.WriteEndElement();
            }
            if (this.Delete != null)
            {
                writer.WriteStartElement("delete");
                for (var i = 0; i < this.Delete.Length; i++)
                {
                    OsmChange.WriteOsmGeo(writer, this.Delete[i]);
                }
                writer.WriteEndElement();
            }
        }

        private static void WriteOsmGeo(XmlWriter writer, OsmGeo osmGeo)
        {
            if (osmGeo == null) { throw new ArgumentNullException("osmGeo"); }

            switch (osmGeo.Type)
            {
                case OsmGeoType.Node:
                    writer.WriteStartElement("node");
                    break;
                case OsmGeoType.Way:
                    writer.WriteStartElement("way");
                    break;
                case OsmGeoType.Relation:
                    writer.WriteStartElement("relation");
                    break;
            }

            (osmGeo as IXmlSerializable).WriteXml(writer);

            writer.WriteEndElement();
        }
    }
}