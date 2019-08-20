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
    /// Represents the Preferences object.
    /// </summary>
    [XmlRoot("preferences")]
    public partial class Preferences : IXmlSerializable
    {
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            var userPreference = new List<Preference>();

            reader.GetElements(
                new Tuple<string, Action>(
                    "preference", () =>
                    {
                        var preference = new Preference();
                        (preference as IXmlSerializable).ReadXml(reader);
                        userPreference.Add(preference);
                        reader.Read();
                    })
            );

            this.UserPreferences = userPreference.ToArray();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("preferences");
            writer.WriteElements("preference", this.UserPreferences);
            writer.WriteEndElement();
        }
    }

    /// <summary>
    /// Represents the Preference object.
    /// </summary>
    [XmlRoot("preference")]
    public partial class Preference : IXmlSerializable
    {
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.Key = reader.GetAttribute("k");
            this.Value = reader.GetAttribute("v");
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttribute("k", this.Key);
            writer.WriteAttribute("v", this.Value);
        }
    }
}