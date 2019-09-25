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
            List<GpxFile> gpxFiles = null;
            List<User> users = null;
            List<Note> notes = null;
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
                        reader.Read();
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
                    }),
                new Tuple<string, Action>(
                    "user", () =>
                    {
                        var user = new User();
                        (user as IXmlSerializable).ReadXml(reader);
                        if (users == null)
                        {
                            users = new List<User>();
                        }
                        users.Add(user);
                    }),
                new Tuple<string, Action>(
                    "note", () =>
                    {
                        var note = new Note();
                        (note as IXmlSerializable).ReadXml(reader);
                        if (note.Id.HasValue) // Ignore Notes missing content.
                        {
                            if (notes == null)
                            {
                                notes = new List<Note>();
                            }
                            notes.Add(note);
                        }
                    }),
                new Tuple<string, Action>(
                    "policy", () =>
                    {
                        this.Policy = new Policy();
                        (this.Policy as IXmlSerializable).ReadXml(reader);
                    }),
                new Tuple<string, Action>(
                    "permissions", () =>
                    {
                        this.Permissions = new Permissions();
                        (this.Permissions as IXmlSerializable).ReadXml(reader);
                    }),
                new Tuple<string, Action>(
                    "preferences", () =>
                    {
                        this.Preferences = new Preferences();
                        (this.Preferences as IXmlSerializable).ReadXml(reader);
                    }),
                new Tuple<string, Action>(
                    "gpx_file", () =>
                    {
                        var gpxFile = new GpxFile();
                        (gpxFile as IXmlSerializable).ReadXml(reader);
                        if (gpxFiles == null)
                        {
                            gpxFiles = new List<GpxFile>();
                        }
                        gpxFiles.Add(gpxFile);
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
            if (gpxFiles != null)
            {
                this.GpxFiles = gpxFiles.ToArray();
            }
            if (users != null)
            {
                if (users.Count == 1)
                {
                    this.User = users[0];
                }
                else
                {
                    this.Users = users.ToArray();
                }
            }
            if (notes != null)
            {
                this.Notes = notes.ToArray();
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttribute("version", this.Version);
            writer.WriteAttribute("generator", this.Generator);

            writer.WriteElement("user", this.User);
            writer.WriteElements("user", this.Users);
            writer.WriteElement("bounds", this.Bounds);
            writer.WriteElement("api", this.Api);
            writer.WriteElement("policy", this.Policy);
            writer.WriteElement("permissions", this.Permissions);
            writer.WriteElement("preferences", this.Preferences);
            writer.WriteElements("note", this.Notes);

            writer.WriteElements("node", this.Nodes);
            writer.WriteElements("way", this.Ways);
            writer.WriteElements("relation", this.Relations);
            writer.WriteElements("changeset", this.Changesets);

            writer.WriteElements("gpx_file", this.GpxFiles);
        }
    }
}