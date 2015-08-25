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
using OsmSharp.IO.Xml.Nominatim.Search.v1;

namespace OsmSharp.IO.Xml.Nominatim.Search
{
    /// <summary>
    /// Search document.
    /// </summary>
    public class SearchDocument
    {
        /// <summary>
        /// The actual search object.
        /// </summary>
        private object _search_object;
 
        /// <summary>
        /// The xml source this documents comes from.
        /// </summary>
        private IXmlSource _source;

        /// <summary>
        /// Returns the search version.
        /// </summary>
        private SearchVersion _version;

        /// <summary>
        /// Creates a new search document based on an xml source.
        /// </summary>
        /// <param name="source"></param>
        public SearchDocument(IXmlSource source)
        {
            _source = source;
            _version = SearchVersion.Unknown;
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
        /// Returns the search version.
        /// </summary>
        public SearchVersion Version
        {
            get
            {
                return _version;
            }
        }

        /// <summary>
        /// Gets/Sets the search object.
        /// </summary>
        public object Search
        {
            get
            {
                this.DoReadSearch();

                return _search_object;
            }
            set
            {
                _search_object = value;

                this.FindVersionFromObject();
            }
        }

        /// <summary>
        /// Saves this search back to it's source.
        /// </summary>
        public void Save()
        {
            this.DoWriteSearch();
        }

        #region Private Serialize/Desirialize functions

        private void FindVersionFromObject()
        {
            _version = SearchVersion.Unknown;
            if (_search_object is searchresults)
            {
                _version = SearchVersion.Searchv1;
            }
        }

        private void FindVersionFromSource()
        {
            // try to find the xmlns and the correct version to use.
            XmlReader reader = _source.GetReader();
            while (!reader.EOF)
            {
                if (reader.NodeType == XmlNodeType.Element
                    && reader.Name == "searchresults")
                {
                    _version = SearchVersion.Searchv1;
                }
                else if (reader.NodeType == XmlNodeType.Element)
                {
                    throw new XmlException("First element expected: searchresults!");
                }

                // check end conditions.
                if (_version != SearchVersion.Unknown)
                {
                    reader = null;
                    break;
                }

                reader.Read();
            }
        }

        private void DoReadSearch()
        {
            if (_search_object == null)
            {
                // decide on the version to use here!
                Type version_type = null;

                // determine the version from source.
                this.FindVersionFromSource();
                switch (_version)
                {
                    case SearchVersion.Searchv1:
                        version_type = typeof(searchresults);
                        break;
                    case SearchVersion.Unknown:
                        throw new XmlException("Version could not be determined!");
                }

                // deserialize here!
                XmlReader reader = _source.GetReader();
                XmlSerializer xmlSerializer = null;
                xmlSerializer = new XmlSerializer(version_type);
                _search_object = xmlSerializer.Deserialize(reader);
            }
        }

        private void DoWriteSearch()
        {
            if (_search_object != null)
            {
                // the version is already decided; find out which one.
                Type version_type = null;
                switch (_version)
                {
                    case SearchVersion.Searchv1:
                        version_type = typeof(searchresults);
                        break;
                    case SearchVersion.Unknown:
                        throw new XmlException("Version could not be determined!");
                }

                XmlSerializer xmlSerializer = null;
                xmlSerializer = new XmlSerializer(version_type);

                XmlWriter writer = _source.GetWriter();
                xmlSerializer.Serialize(writer, _search_object);
                writer.Flush();

                xmlSerializer = null;
                writer = null;
            }
        }
        #endregion

        /// <summary>
        /// Closes this search document.
        /// </summary>
        public void Close()
        {
            _search_object = null;
            _source = null;
        }
    }

    /// <summary>
    /// The possible kml document versions.
    /// </summary>
    public enum SearchVersion
    {
        /// <summary>
        /// Version 1
        /// </summary>
        Searchv1,
        /// <summary>
        /// Uknown.
        /// </summary>
        Unknown
    }
}
