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

namespace OsmSharp.API
{
    /// <summary>
    /// Represents the root-object for all API-related communication.
    /// </summary>
    [XmlRoot("osm")]
    public class Osm : IXmlSerializable
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
        /// Gets or sets the capabilities.
        /// </summary>
        public Capabilities Api { get; set; }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.Version = reader.GetAttributeDouble("version");
            this.Generator = reader.GetAttribute("generator");

            reader.GetElements(
                new Tuple<string, Action>(
                    "api", () =>
                    {
                        this.Api = new Capabilities();
                        (this.Api as IXmlSerializable).ReadXml(reader);
                    }));
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttribute("version", this.Version);
            writer.WriteAttribute("generator", this.Generator);

            writer.WriteElement("api", this.Api);
        }
    }
}