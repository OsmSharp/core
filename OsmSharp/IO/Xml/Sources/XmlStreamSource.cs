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
using System.IO;
using System.Text;
using System.Xml;

namespace OsmSharp.IO.Xml.Sources
{
    /// <summary>
    /// XML stream source.
    /// </summary>
    public class XmlStreamSource : IXmlSource
    {
        /// <summary>
        /// The reference to the file.
        /// </summary>
        private Stream _stream;

        /// <summary>
        /// Creates a new xml file source.
        /// </summary>
        /// <param name="stream"></param>
        public XmlStreamSource(Stream stream)
        {
            _stream = stream;
        }

        #region IXmlSource Members

        /// <summary>
        /// Returns an xml reader.
        /// </summary>
        public XmlReader GetReader()
        {
            _stream.Seek(0, SeekOrigin.Begin);
            return XmlReader.Create(_stream);
        }

        /// <summary>
        /// Returns an xml writer.
        /// </summary>
        /// <returns></returns>
        public XmlWriter GetWriter()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.CheckCharacters = true;
            settings.CloseOutput = true;
            settings.ConformanceLevel = ConformanceLevel.Document;
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;
            settings.NewLineChars  = Environment.NewLine;
            settings.NewLineHandling = NewLineHandling.Entitize;
            settings.OmitXmlDeclaration = true;

            _stream.SetLength(0);
            return XmlWriter.Create(_stream);
        }

        /// <summary>
        /// Returns true if this file source is readonly.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                //if (!_stream.CanWrite)
                //{
                //    return true;
                //}
                return false;
            }
        }

        /// <summary>
        /// Returns true if the file source has data.
        /// </summary>
        public bool HasData
        {
            get
            {
                return _stream.Length > 1;
            }
        }

        /// <summary>
        /// Closes this file source.
        /// </summary>
        public void Close()
        {
            _stream = null;
        }

        #endregion
    }
}