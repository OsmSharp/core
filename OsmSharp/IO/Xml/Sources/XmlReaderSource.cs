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

using System.Xml;

namespace OsmSharp.IO.Xml.Sources
{
    /// <summary>
    /// Class implementing an xml reader source.
    /// </summary>
    public class XmlReaderSource : IXmlSource
    {
        /// <summary>
        /// The string source.
        /// </summary>
        private XmlReader _reader;

        /// <summary>
        /// Creates a new xml reader source.
        /// </summary>
        /// <param name="reader"></param>
        public XmlReaderSource(XmlReader reader)
        {
            _reader = reader;
        }

        #region IXmlSource Members

        /// <summary>
        /// Returns an xml reader.
        /// </summary>
        public XmlReader GetReader()
        {
            return _reader;
        }

        /// <summary>
        /// Returns an xml writer.
        /// </summary>
        public XmlWriter GetWriter()
        {
            return null;
        }

        /// <summary>
        /// Returns true if readonly.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Returns true if contains data.
        /// </summary>
        public bool HasData
        {
            get
            {
                return _reader != null;
            }
        }

        /// <summary>
        /// Returns the name of this source.
        /// </summary>
        public string Name
        {
            get
            {
                return "Generic Reader Source";
            }
        }

        /// <summary>
        /// Closes this source.
        /// </summary>
        public void Close()
        {
            _reader = null;
        }

        #endregion
    }
}