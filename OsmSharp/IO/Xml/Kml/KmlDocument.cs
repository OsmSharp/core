// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Xml;
using System.Xml.Serialization;
using OsmSharp.IO.Xml;

namespace OsmSharp.IO.Xml.Kml
{
    /// <summary>
    /// Represents a kml document.
    /// </summary>
    public class KmlDocument
    {
        /// <summary>
        /// The actual kml object.
        /// </summary>
        private object _kml_object;
 
        /// <summary>
        /// The xml source this documents comes from.
        /// </summary>
        private IXmlSource _source;

        /// <summary>
        /// Returns the kml version.
        /// </summary>
        private KmlVersion _version;

        /// <summary>
        /// Creates a new kml document based on an xml source.
        /// </summary>
        /// <param name="source"></param>
        public KmlDocument(IXmlSource source)
        {
            _source = source;
            _version = KmlVersion.Unknown;
        }


        ///// <summary>
        ///// Returns the name of this document.
        ///// </summary>
        //public string Name
        //{
        //    get
        //    {
        //        return _source.Name;
        //    }
        //}

        /// <summary>
        /// Returns the readonly flag.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return _source.IsReadOnly;
            }
        }

        /// <summary>
        /// Returns the kml version.
        /// </summary>
        public KmlVersion Version
        {
            get
            {
                return _version;
            }
        }

        /// <summary>
        /// Gets/Sets the kml object.
        /// </summary>
        public object Kml
        {
            get
            {
                this.DoReadKml();

                return _kml_object;
            }
            set
            {
                _kml_object = value;

                this.FindVersionFromObject();
            }
        }

        /// <summary>
        /// Saves this kml back to it's source.
        /// </summary>
        public void Save()
        {
            this.DoWriteKml();
        }

        #region Private Serialize/Desirialize functions

        private void FindVersionFromObject()
        {
            _version = KmlVersion.Unknown;
            if (_kml_object is OsmSharp.IO.Xml.Kml.v2_0.kml)
            {
                _version = KmlVersion.Kmlv2_0;
            }
            else if (_kml_object is OsmSharp.IO.Xml.Kml.v2_0_response.kml)
            {
                _version = KmlVersion.Kmlv2_0_response;
            }
            else if (_kml_object is OsmSharp.IO.Xml.Kml.v2_1.KmlType)
            {
                _version = KmlVersion.Kmlv2_1;
            }
        }

        private void FindVersionFromSource()
        {
            // try to find the xmlns and the correct version to use.
            XmlReader reader = _source.GetReader();
            while (!reader.EOF)
            {
                if (reader.NodeType == XmlNodeType.Element
                    && reader.Name == "kml")
                {
                    string ns = reader.GetAttribute("xmlns");
                    switch (ns)
                    {
                        case "http://earth.google.com/kml/2.0":
                            // trick here for the google maps geocoding resultes.
                            reader.Read();
                            while (!reader.EOF)
                            {
                                if (reader.NodeType == XmlNodeType.Element
                                    && reader.Name.ToLower() == "response")
                                {
                                    _version = KmlVersion.Kmlv2_0_response;
                                    break;
                                }
                                else if (reader.NodeType == XmlNodeType.Element)
                                {
                                    _version = KmlVersion.Kmlv2_0;
                                    break;
                                }
                                reader.Read();
                            }
                            break;
                        case "http://earth.google.com/kml/2.1":
                            _version = KmlVersion.Kmlv2_1;
                            break;
                    }
                }
                else if (reader.NodeType == XmlNodeType.Element)
                {
                    throw new XmlException("First element expected: kml!");
                }

                // check end conditions.
                if (_version != KmlVersion.Unknown)
                {
                    reader = null;
                    break;
                }

                reader.Read();
            }
        }

        private void DoReadKml()
        {
            if (_kml_object == null)
            {
                if (_source != null)
                {
                    // decide on the version to use here!
                    Type version_type = null;

                    // determine the version from source.
                    this.FindVersionFromSource();
                    switch (_version)
                    {
                        case KmlVersion.Kmlv2_0:
                            version_type = typeof(OsmSharp.IO.Xml.Kml.v2_0.kml);
                            break;
                        case KmlVersion.Kmlv2_0_response:
                            version_type = typeof(OsmSharp.IO.Xml.Kml.v2_0_response.kml);
                            break;
                        case KmlVersion.Kmlv2_1:
                            version_type = typeof(OsmSharp.IO.Xml.Kml.v2_1.KmlType);
                            break;
                        case KmlVersion.Unknown:
                            throw new XmlException("Version could not be determined!");
                    }

                    // deserialize here!
                    XmlReader reader = _source.GetReader();
                    XmlSerializer xmlSerializer = null;
                    xmlSerializer = new XmlSerializer(version_type);
                    _kml_object = xmlSerializer.Deserialize(reader);
                }
            }
        }

        private void DoWriteKml()
        {
            if (_kml_object != null)
            {
                // the version is already decided; find out which one.
                Type version_type = null;
                switch (_version)
                {
                    case KmlVersion.Kmlv2_0:
                        version_type = typeof(OsmSharp.IO.Xml.Kml.v2_0.kml);
                        break;
                    case KmlVersion.Kmlv2_0_response:
                        version_type = typeof(OsmSharp.IO.Xml.Kml.v2_0_response.kml);
                        break;
                    case KmlVersion.Kmlv2_1:
                        version_type = typeof(OsmSharp.IO.Xml.Kml.v2_1.KmlType);
                        break;
                    case KmlVersion.Unknown:
                        throw new XmlException("Version could not be determined!");
                }

                XmlSerializer xmlSerializer = null;
                xmlSerializer = new XmlSerializer(version_type);

                XmlWriter writer = _source.GetWriter();
                xmlSerializer.Serialize(writer, _kml_object);
                writer.Flush();

                xmlSerializer = null;
                writer = null;
            }
        }
        #endregion

        /// <summary>
        /// Closes this data source.
        /// </summary>
        public void Close()
        {
            _kml_object = null;
            _source = null;
        }
    }

    /// <summary>
    /// The possible kml document versions.
    /// </summary>
    public enum KmlVersion
    {
        /// <summary>
        /// Kml v2.1
        /// </summary>
        Kmlv2_1,
        /// <summary>
        /// Kml v2.0
        /// </summary>
        Kmlv2_0,
        /// <summary>
        /// Kml v2.0 response.
        /// </summary>
        Kmlv2_0_response,
        /// <summary>
        /// Unknown.
        /// </summary>
        Unknown
    }
}
