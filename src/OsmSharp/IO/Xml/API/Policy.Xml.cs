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
    /// Represents the Policy object.
    /// </summary>
    [XmlRoot("policy")]
    public partial class Policy : IXmlSerializable
    {
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.GetElements(
                new Tuple<string, Action>(
                    "imagery", () =>
                    {
                        var imagery = new Imagery();
                        imagery.ReadXml(reader);
                        reader.Read();
                        this.Imagery = imagery;
                    })
            );
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("imagery");
            writer.WriteElement("imagery", this.Imagery);
            writer.WriteEndElement();
        }
    }

    [XmlRoot("imagery")]
    public partial class Imagery : IXmlSerializable
    {
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            var blacklists = new List<Blacklist>();

            reader.GetElements(
                new Tuple<string, Action>(
                    "blacklist", () =>
                    {
                        var blacklist = new Blacklist();
                        blacklist.ReadXml(reader);
                        blacklists.Add(blacklist);
                        reader.Read();
                    })
            );

            this.Blacklists = blacklists.ToArray();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElements("blacklist", this.Blacklists);
        }
    }

    [XmlRoot("blacklist")]
    public partial class Blacklist : IXmlSerializable
    {
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            this.Regex = reader.GetAttribute("regex");
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("blacklist");
            writer.WriteAttribute("regex", this.Regex);
            writer.WriteEndElement();
        }
    }
}