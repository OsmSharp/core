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

using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using OsmSharp.Osm.Streams;
using Ionic.Zlib;

namespace OsmSharp.Osm.Xml.Streams
{
    /// <summary>
    /// A stream reader that reads from OSM Xml.
    /// </summary>
    public class XmlOsmStreamSource : OsmStreamSource
    {
        private XmlReader _reader;

        private XmlSerializer _serNode;

        private XmlSerializer _serWay;

        private XmlSerializer _serRelation;

        private OsmGeo _next;

        private Stream _stream;

        private readonly bool _gzip;

        private readonly bool _disposeStream = false;

        /// <summary>
        /// Creates a new OSM Xml processor source.
        /// </summary>
        /// <param name="stream"></param>
        public XmlOsmStreamSource(Stream stream) :
            this(stream, false)
        {

        }

        /// <summary>
        /// Creates a new OSM XML processor source.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="gzip"></param>
        public XmlOsmStreamSource(Stream stream, bool gzip)
        {
            _stream = stream;
            _gzip = gzip;
        }

        /// <summary>
        /// Initializes this source.
        /// </summary>
        public override void Initialize()
        {
            _next = null;
            _serNode = new XmlSerializer(typeof(Osm.Xml.v0_6.node));
            _serWay = new XmlSerializer(typeof(Osm.Xml.v0_6.way));
            _serRelation = new XmlSerializer(typeof(Osm.Xml.v0_6.relation));

            this.Reset();
        }
        
        /// <summary>
        /// Resets this source.
        /// </summary>
        public override void Reset()
        {
            // create the xml reader settings.
            var settings = new XmlReaderSettings();
            settings.CloseInput = true;
            settings.CheckCharacters = false;
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            //settings.IgnoreWhitespace = true;

            // seek to the beginning of the stream.
            if (_stream.CanSeek)
            { // if a non-seekable stream is given resetting is disabled.
                _stream.Seek(0, SeekOrigin.Begin);
            }

            // decompress if needed.
            if (_gzip)
            {
                _stream = new GZipStream(_stream, CompressionMode.Decompress);
            }

            TextReader textReader = new StreamReader(_stream, Encoding.UTF8);
            _reader = XmlReader.Create(textReader, settings);
        }

        /// <summary>
        /// Returns true if this source can be reset.
        /// </summary>
        public override bool CanReset
        {
            get
            {
                return _stream.CanSeek;
            }
        }

        /// <summary>
        /// Move to the next item in the stream.
        /// </summary>
        /// <param name="ignoreNodes">Makes this source skip all nodes.</param>
        /// <param name="ignoreWays">Makes this source skip all ways.</param>
        /// <param name="ignoreRelations">Makes this source skip all relations.</param>
        /// <returns></returns>
        public override bool MoveNext(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
        {
            while (_reader.Read())
            {
                if (_reader.NodeType == XmlNodeType.Element && 
                    (_reader.Name == "node" && !ignoreNodes) || 
                    (_reader.Name == "way" && !ignoreWays) || 
                    (_reader.Name == "relation" && !ignoreRelations))
                {
                    // create a stream for only this element.
                    string name = _reader.Name;
                    string nextElement = _reader.ReadOuterXml();
                    XmlReader reader = XmlReader.Create(new MemoryStream(Encoding.UTF8.GetBytes(nextElement)));
                    object osmObj = null;

                    // select type of element.
                    switch (name)
                    {
                         case "node":
                             osmObj = _serNode.Deserialize(reader);
                             if (osmObj is OsmSharp.Osm.Xml.v0_6.node)
                             {
                                 _next = XmlSimpleConverter.ConvertToSimple(osmObj as OsmSharp.Osm.Xml.v0_6.node);
                                 return true;
                             }
                             break;
                         case "way":
                             osmObj = _serWay.Deserialize(reader);
                             if (osmObj is OsmSharp.Osm.Xml.v0_6.way)
                             {
                                 _next = XmlSimpleConverter.ConvertToSimple(osmObj as OsmSharp.Osm.Xml.v0_6.way);
                                 return true;
                             }
                             break;
                         case "relation":
                             osmObj = _serRelation.Deserialize(reader);
                             if (osmObj is OsmSharp.Osm.Xml.v0_6.relation)
                             {
                                 _next = XmlSimpleConverter.ConvertToSimple(osmObj as OsmSharp.Osm.Xml.v0_6.relation);
                                 return true;
                             }
                             break;
                    }
                }
            }
            _next = null;
            return false;
        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        /// <returns></returns>
        public override OsmGeo Current()
        {
            return _next;
        }

        /// <summary>
        /// Disposes all resources associated with this stream.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            if (_disposeStream)
            {
                _stream.Dispose();
            }
        }
    }
}