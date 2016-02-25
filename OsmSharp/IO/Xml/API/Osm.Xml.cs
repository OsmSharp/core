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
using System.Collections.Generic;
using System;
using OsmSharp.Changesets;

namespace OsmSharp.API
{
    /// <summary>
    /// Represents the OSM base object.
    /// </summary>
    [XmlRoot("osm")]
    public partial class Osm : IXmlSerializable
    {
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.Version = reader.GetAttributeDouble("version");
            this.Generator = reader.GetAttribute("generator");

            List<Node> nodes = null;
            List<Way> ways = null;
            List<Relation> relations = null;
            List<Changeset> changesets = null;
            reader.GetElements(
                new Tuple<string, Action>(
                    "api", () =>
                    {
                        this.Api = new Capabilities();
                        (this.Api as IXmlSerializable).ReadXml(reader);
                    }),
                new Tuple<string, Action>(
                    "bounds", () =>
                    {
                        this.Bounds = new Bounds();
                        (this.Bounds as IXmlSerializable).ReadXml(reader);
                    }),
                new Tuple<string, Action>(
                    "node", () =>
                    {
                        var node = new Node();
                        (node as IXmlSerializable).ReadXml(reader);
                        if (nodes == null)
                        {
                            nodes = new List<Node>();
                        }
                        nodes.Add(node);
                    }),
                new Tuple<string, Action>(
                    "way", () =>
                    {
                        var way = new Way();
                        (way as IXmlSerializable).ReadXml(reader);
                        if (ways == null)
                        {
                            ways = new List<Way>();
                        }
                        ways.Add(way);
                    }),
                new Tuple<string, Action>(
                    "relation", () =>
                    {
                        var relation = new Relation();
                        (relation as IXmlSerializable).ReadXml(reader);
                        if (relations == null)
                        {
                            relations = new List<Relation>();
                        }
                        relations.Add(relation);
                    }),
                new Tuple<string, Action>(
                    "changeset", () =>
                    {
                        var changeset = new Changeset();
                        (changeset as IXmlSerializable).ReadXml(reader);
                        if (changesets == null)
                        {
                            changesets = new List<Changeset>();
                        }
                        changesets.Add(changeset);
                    }));

            if (nodes != null)
            {
                this.Nodes = nodes.ToArray();
            }
            if (ways != null)
            {
                this.Ways = ways.ToArray();
            }
            if (relations != null)
            {
                this.Relations = relations.ToArray();
            }
            if (changesets != null)
            {
                this.Changesets = changesets.ToArray();
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttribute("version", this.Version);
            writer.WriteAttribute("generator", this.Generator);

            writer.WriteElement("bounds", this.Bounds);
            writer.WriteElement("api", this.Api);

            writer.WriteElements("node", this.Nodes);
            writer.WriteElements("way", this.Ways);
            writer.WriteElements("relation", this.Relations);
            writer.WriteElements("changeset", this.Changesets);
        }
    }
}