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

using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace OsmSharp.Streams
{
    /// <summary>
    /// A stream source that reads OSM-XML.
    /// </summary>
    public class XmlOsmStreamSource : OsmStreamSource
    {
        private readonly bool _disposeStream = false;
        private readonly Stream _stream;

        /// <summary>
        /// Creates a new OSM XML processor source.
        /// </summary>
        public XmlOsmStreamSource(Stream stream)
        {
            _stream = stream;
        }

        private XmlReader _reader;
        private XmlSerializer _serNode;
        private XmlSerializer _serWay;
        private XmlSerializer _serRelation;
        private OsmGeo _next;
        private bool _initialized;

        /// <summary>
        /// Initializes this source.
        /// </summary>
        private void Initialize()
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
            if (!_initialized)
            {
                this.Initialize();
                _initialized = true;
            }

            while (!_reader.EOF &&
                _reader.MoveToContent() != XmlNodeType.Whitespace)
            {
                if (_reader.NodeType == XmlNodeType.Element &&
                    (_reader.Name == "node" && !ignoreNodes) ||
                    (_reader.Name == "way" && !ignoreWays) ||
                    (_reader.Name == "relation" && !ignoreRelations))
                {
                    var name = _reader.Name;
                    
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
                            if (_reader.NodeType == XmlNodeType.EndElement &&
                                _reader.Name == "way")
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