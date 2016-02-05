// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2016 Abelshausen Ben
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
using Ionic.Zlib;

namespace OsmSharp.Osm.Streams
{
    /// <summary>
    /// A stream reader that reads from OSM Xml.
    /// </summary>
    public class XmlOsmStreamSource : OsmStreamSource
    {
        private readonly bool _gzip;
        private readonly bool _disposeStream = false;

        /// <summary>
        /// Creates a new OSM Xml processor source.
        /// </summary>
        public XmlOsmStreamSource(Stream stream) :
            this(stream, false)
        {

        }

        /// <summary>
        /// Creates a new OSM XML processor source.
        /// </summary>
        public XmlOsmStreamSource(Stream stream, bool gzip)
        {
            _stream = stream;
            _gzip = gzip;
        }

        private Stream _stream;
        private XmlReader _reader;
        private XmlSerializer _serNode;
        private XmlSerializer _serWay;
        private XmlSerializer _serRelation;
        private OsmGeo _next;

        /// <summary>
        /// Initializes this source.
        /// </summary>
        public override void Initialize()
        {
            _next = null;
            _serNode = new XmlSerializer(typeof(Node));
            _serWay = new XmlSerializer(typeof(Way));
            _serRelation = new XmlSerializer(typeof(Relation));

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

            var textReader = new StreamReader(_stream, Encoding.UTF8);
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
        public override bool MoveNext(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
        {
            while (!_reader.EOF &&
                _reader.MoveToContent() != XmlNodeType.Whitespace)
            {
                if (_reader.NodeType == XmlNodeType.Element && 
                    (_reader.Name == "node" && !ignoreNodes) || 
                    (_reader.Name == "way" && !ignoreWays) || 
                    (_reader.Name == "relation" && !ignoreRelations))
                {
                    // create a stream for only this element.
                    var name = _reader.Name;
                    //var nextElement = _reader.ReadOuterXml();
                    //var reader = XmlReader.Create(new MemoryStream(Encoding.UTF8.GetBytes(nextElement)));

                    // select type of element.
                    switch (name)
                    {
                        case "node":
                            _next = _serNode.Deserialize(_reader) as Node;
                            if (_reader.NodeType == XmlNodeType.EndElement &&
                                _reader.Name == "node")
                            {
                                _reader.Read();
                            }
                            return true;
                        case "way":
                            _next = _serWay.Deserialize(_reader) as Way;
                            if(_reader.NodeType == XmlNodeType.EndElement &&
                                _reader.Name =="way")
                            {
                                _reader.Read();
                            }
                            return true;
                        case "relation":
                            _next = _serRelation.Deserialize(_reader) as Relation;
                            if (_reader.NodeType == XmlNodeType.EndElement &&
                                _reader.Name == "relation")
                            {
                                _reader.Read();
                            }
                            return true;
                    }
                }
                else
                { // unknown element or to be ignored, skip it.
                    _reader.Read();
                }
            }
            _next = null;
            return false;
        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
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