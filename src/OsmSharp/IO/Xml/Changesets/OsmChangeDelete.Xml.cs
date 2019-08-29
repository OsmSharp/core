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

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Linq;
using OsmSharp.IO.Xml;

namespace OsmSharp.Changesets
{
    /// <summary>
    /// Represents a changeset delete.
    /// </summary>
    [XmlRoot("delete")]
    public partial class OsmChangeDelete : IXmlSerializable
    {
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.IfUnused = reader.GetAttribute("if-unused") != null;

            List<OsmGeo> deletes = new List<OsmGeo>();

            reader.GetElements(
                new Tuple<string, Action>(
                    "node", () =>
                    {
                        var osmGeo = new Node();
                        (osmGeo as IXmlSerializable).ReadXml(reader);
                        deletes.Add(osmGeo);
                    }),
                new Tuple<string, Action>(
                    "way", () =>
                    {
                        var osmGeo = new Way();
                        (osmGeo as IXmlSerializable).ReadXml(reader);
                        deletes.Add(osmGeo);
                    }),
                new Tuple<string, Action>(
                    "relation", () =>
                    {
                        var osmGeo = new Relation();
                        (osmGeo as IXmlSerializable).ReadXml(reader);
                        deletes.Add(osmGeo);
                    }));

            if (deletes.Any())
            {
                this.Delete = deletes.ToArray();
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            // When the 'if-unused' attribute exists, it is interpreted as 'true'
            // so don't put 'false' as a value
            if (this.IfUnused)
            {
                writer.WriteAttribute("if-unused", this.IfUnused);
            }

            if (this.Delete != null)
            {
                foreach (var element in this.Delete)
                {
                    writer.WriteElement(element.Type.ToString().ToLower(), (IXmlSerializable)element);
                }
            }
        }
    }
}