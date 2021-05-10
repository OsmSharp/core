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
using System;
using System.Collections.Generic;

namespace OsmSharp.API
{
    /// <summary>
    /// Represents the OSM base object.
    /// </summary>
    [XmlRoot("user")]
    public partial class User : IXmlSerializable
    {
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.Id = reader.GetAttributeInt64("id") ?? 0;
            this.DisplayName = reader.GetAttribute("display_name");
            this.AccountCreated = reader.GetAttributeDateTime("account_created").Value;

            reader.GetElements(
                new Tuple<string, Action>(
                    "description", () =>
                    {
                        this.Description = reader.ReadElementContentAsString();
                    }),
                new Tuple<string, Action>(
                    "contributor-terms", () =>
                    {
                        this.ContributorTermsAgreed = reader.GetAttributeBool("agreed") ?? false;
                        this.ContributorTermsPublicDomain = reader.GetAttributeBool("pd") ?? false;
                        reader.Read();
                    }),
                new Tuple<string, Action>(
                    "img", () =>
                    {
                        this.Image = reader.GetAttribute("href");
                        reader.Read();
                    }),
                new Tuple<string, Action>(
                    "roles", () =>
                    {
                        reader.Read();
                    }),
                new Tuple<string, Action>(
                    "changesets", () =>
                    {
                        this.ChangeSetCount = reader.GetAttributeInt32("count") ?? 0;
                        reader.Read();
                    }),
                new Tuple<string, Action>(
                    "traces", () =>
                    {
                        this.TraceCount = reader.GetAttributeInt32("count") ?? 0;
                        reader.Read();
                    }),
                new Tuple<string, Action>(
                    "blocks", () =>
                    {
                        var blocks = new List<Block>();
                        reader.GetElements(new Tuple<string, Action>(
                            "received", () =>
                            {
                                var block = new Block();
                                block.ReadXml(reader);
                                reader.Read();
                                blocks.Add(block);
                            }));
                        this.BlocksReceived = blocks.ToArray();
                    }),
                new Tuple<string, Action>(
                    "home", () =>
                    {
                        this.Home = new Home();
                        this.Home.ReadXml(reader);
                        reader.Read();
                    }),
                new Tuple<string, Action>(
                    "languages", () =>
                    {
                        var languages = new List<string>();
                        reader.GetElements(new Tuple<string, Action>(
                            "lang", () =>
                            {
                                languages.Add(reader.ReadElementContentAsString());
                            }));
                        this.Languages = languages.ToArray();
                        //reader.Read();
                    }),
                new Tuple<string, Action>(
                    "messages", () =>
                    {
                        this.Messages = new Messages();
                        this.Messages.ReadXml(reader);
                        reader.Read();
                    })
            );

        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttribute("id", this.Id);
            writer.WriteAttribute("display_name", this.DisplayName);
            writer.WriteAttribute("account_created", this.AccountCreated);

            writer.WriteElementString("description", this.Description);
            writer.WriteStartElement("contributor-terms");
            writer.WriteAttribute("agreed", this.ContributorTermsAgreed);
            writer.WriteAttribute("pd", this.ContributorTermsPublicDomain);
            writer.WriteEndElement();
            writer.WriteStartElement("img");
            writer.WriteAttribute("href", this.Image);
            writer.WriteEndElement();
            writer.WriteStartElement("roles");
            writer.WriteFullEndElement();
            writer.WriteStartElement("changesets");
            writer.WriteAttribute("count", this.ChangeSetCount);
            writer.WriteEndElement();
            writer.WriteStartElement("traces");
            writer.WriteAttribute("count", this.TraceCount);
            writer.WriteEndElement();
            writer.WriteStartElement("blocks");
            if (this.BlocksReceived != null)
            {
                foreach (var block in this.BlocksReceived)
                {
                    writer.WriteElement("received", block);
                }
            }
            writer.WriteEndElement();
            writer.WriteElement("home", this.Home);
            writer.WriteStartElement("languages");
            if (this.Languages != null)
            {
                foreach (var language in this.Languages)
                {
                    writer.WriteElementString("lang", language);
                }
            }
            writer.WriteEndElement();
            writer.WriteElement("messages", this.Messages);
        }
    }

    [XmlRoot("received")]
    public partial class Block : IXmlSerializable
    {
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            this.Count = reader.GetAttributeInt32("count") ?? 0;
            this.Active = reader.GetAttributeInt32("active") ?? 0;
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttribute("count", this.Count);
            writer.WriteAttribute("active", this.Active);
        }
    }

    [XmlRoot("home")]
    public partial class Home : IXmlSerializable
    {
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            this.Latitude = reader.GetAttributeSingle("lat") ?? 0;
            this.Longitude = reader.GetAttributeSingle("lon") ?? 0;
            this.Zoom = reader.GetAttributeSingle("zoom") ?? 0;
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttribute("lat", this.Latitude);
            writer.WriteAttribute("lon", this.Longitude);
            writer.WriteAttribute("zoom", this.Zoom);
        }
    }

    [XmlRoot("messages")]
    public partial class Messages : IXmlSerializable
    {
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement("messages");
            this.Received = reader.GetAttributeInt32("count") ?? 0;
            this.Unread = reader.GetAttributeInt32("unread") ?? 0;
            reader.ReadStartElement("received");
            this.Sent = reader.GetAttributeInt32("count") ?? 0;
            reader.ReadStartElement("sent");
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("received");
            writer.WriteAttribute("count", this.Received);
            writer.WriteAttribute("unread", this.Unread);
            writer.WriteEndElement();
            writer.WriteStartElement("sent");
            writer.WriteAttribute("count", this.Sent);
            writer.WriteEndElement();
        }
    }
}