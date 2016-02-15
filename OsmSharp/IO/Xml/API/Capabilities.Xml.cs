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
    /// Represents the API capabilities.
    /// </summary>
    [XmlRoot("api")]
    public partial class Capabilities : IXmlSerializable
    {
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.GetElements(
                new Tuple<string, Action>(
                    "version", () =>
                    {
                        this.Version = new Version();
                        (this.Version as IXmlSerializable).ReadXml(reader);
                    }),
                new Tuple<string, Action>(
                    "area", () =>
                    {
                        this.Area = new Area();
                        (this.Area as IXmlSerializable).ReadXml(reader);
                    }),
                new Tuple<string, Action>(
                    "tracepoints", () =>
                    {
                        this.Tracepoints = new Tracepoints();
                        (this.Tracepoints as IXmlSerializable).ReadXml(reader);
                    }),
                new Tuple<string, Action>(
                    "waynodes", () =>
                    {
                        this.WayNodes = new WayNodes();
                        (this.WayNodes as IXmlSerializable).ReadXml(reader);
                    }),
                new Tuple<string, Action>(
                    "changesets", () =>
                    {
                        this.Changesets = new Changesets();
                        (this.Changesets as IXmlSerializable).ReadXml(reader);
                    }),
                new Tuple<string, Action>(
                    "timeout", () =>
                    {
                        this.Timeout = new Timeout();
                        (this.Timeout as IXmlSerializable).ReadXml(reader);
                    }),
                new Tuple<string, Action>(
                    "status", () =>
                    {
                        this.Status = new Status();
                        (this.Status as IXmlSerializable).ReadXml(reader);
                    }));
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteElement("version", this.Version);
            writer.WriteElement("area", this.Area);
            writer.WriteElement("tracepoints", this.Tracepoints);
            writer.WriteElement("waynodes", this.WayNodes);
            writer.WriteElement("changesets", this.Changesets);
            writer.WriteElement("timeout", this.Timeout);
            writer.WriteElement("status", this.Status);
        }
    }

    /// <summary>
    /// Represents the API-version capabilities.
    /// </summary>
    [XmlRoot("version")]
    public partial class Version : IXmlSerializable
    {
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.Minimum = reader.GetAttributeDouble("minimum");
            this.Maximum = reader.GetAttributeDouble("maximum");
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttribute("minimum", this.Minimum);
            writer.WriteAttribute("maximum", this.Maximum);
        }
    }

    /// <summary>
    /// Represents the API-area capability.
    /// </summary>
    [XmlRoot("area")]
    public partial class Area : IXmlSerializable
    {
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.Maximum = reader.GetAttributeDouble("maximum");
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttribute("maximum", this.Maximum);
        }
    }

    /// <summary>
    /// Represents the API-tracepoints capability.
    /// </summary>
    [XmlRoot("tracepoints")]
    public partial class Tracepoints : IXmlSerializable
    {
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.PerPage = reader.GetAttributeInt32("per_page");
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttribute("per_page", this.PerPage);
        }
    }

    /// <summary>
    /// Represents the API-waynodes capability.
    /// </summary>
    [XmlRoot("waynodes")]
    public partial class WayNodes : IXmlSerializable
    {
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.Maximum = reader.GetAttributeInt32("maximum");
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttribute("maximum", this.Maximum);
        }
    }

    /// <summary>
    /// Represents the API-changesets capabilitiy.
    /// </summary>
    [XmlRoot("changesets")]
    public partial class Changesets : IXmlSerializable
    {
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.MaximumElements = reader.GetAttributeInt32("maximum_elements");
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttribute("maximum_elements", this.MaximumElements);
        }
    }

    /// <summary>
    /// Represents the API-timeout capability.
    /// </summary>
    [XmlRoot("timeout")]
    public partial class Timeout : IXmlSerializable
    {
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.Seconds = reader.GetAttributeInt32("seconds");
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttribute("seconds", this.Seconds);
        }
    }

    /// <summary>
    /// Represents the API-status.
    /// </summary>
    [XmlRoot("status")]
    public partial class Status : IXmlSerializable
    {
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.Api = reader.GetAttribute("api");
            this.Database = reader.GetAttribute("database");
            this.Gpx = reader.GetAttribute("gpx");
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttribute("api", this.Api);
            writer.WriteAttribute("database", this.Database);
            writer.WriteAttribute("gpx", this.Gpx);
        }
    }
}
