// The MIT License (MIT)

// Copyright (c) 2017 Ben Abelshausen

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

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using OsmSharp.IO.Xml;

namespace OsmSharp.API
{
    /// <summary>
    /// Represents gpx_file object.
    /// </summary>
    [XmlRoot("gpx_file")]
    public partial class GpxFile : IXmlSerializable
    {
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            this.Id = reader.GetAttributeInt64("id") ?? 0;
            this.Name = reader.GetAttribute("name");
            this.Lat = reader.GetAttributeDouble("lat");
            this.Lon = reader.GetAttributeDouble("lon");
            this.User = reader.GetAttribute("user");
            this.Visibility = reader.GetAttributeEnum<Visibility>("visibility");
            this.Pending = reader.GetAttributeBool("pending") ?? false;
            this.TimeStamp = reader.GetAttributeDateTime("timestamp") ?? DateTime.Now;
            var tags = new List<string>();
            reader.GetElements(
                new Tuple<string, Action>(
                    "description", () =>
                    {
                        this.Description = reader.ReadElementContentAsString();
                    }),
                new Tuple<string, Action>(
                    "tag", () =>
                    {
                        tags.Add(reader.ReadElementContentAsString());
                    }));
            this.Tags = tags.ToArray();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("id", this.Id.ToString());
            writer.WriteAttributeString("name", this.Name);
            if (this.Lat.HasValue)
            {
                writer.WriteAttributeString("lat", this.Lat.Value.ToInvariantString());
            }
            if (this.Lon.HasValue)
            {
                writer.WriteAttributeString("lon", this.Lon.Value.ToInvariantString());
            }
            writer.WriteAttributeString("user", this.User);
            writer.WriteAttributeString("visibility", this.Visibility.ToString().ToLower());
            writer.WriteAttributeString("pending", this.Pending.ToString());
            writer.WriteAttribute("timestamp", this.TimeStamp);
            writer.WriteElementString("description", this.Description);
            if(this.Tags != null)
            {
                foreach (string tag in this.Tags)
                {
                    writer.WriteElementString("tag", tag);
                }
            }
        }
    }
}
