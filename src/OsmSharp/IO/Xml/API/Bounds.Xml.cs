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

namespace OsmSharp.API
{
    /// <summary>
    /// Represents the API capabilities.
    /// </summary>
    [XmlRoot("bounds")]
    public partial class Bounds : IXmlSerializable
    {
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.MinLatitude = reader.GetAttributeSingle("minlat");
            this.MinLongitude = reader.GetAttributeSingle("minlon");
            this.MaxLatitude = reader.GetAttributeSingle("maxlat");
            this.MaxLongitude = reader.GetAttributeSingle("maxlon");
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttribute("minlat", this.MinLatitude);
            writer.WriteAttribute("minlon", this.MinLongitude);
            writer.WriteAttribute("maxlat", this.MaxLatitude);
            writer.WriteAttribute("maxlon", this.MaxLongitude);
        }
    }
}