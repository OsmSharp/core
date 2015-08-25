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
using OsmSharp.Osm.Streams.ChangeSets;

namespace OsmSharp.Osm.Xml.Streams.ChangeSets
{
    /// <summary>
    /// A changeset source.
    /// </summary>
    public class XmlDataProcessorChangeSetSource : DataProcessorChangeSetSource
    {
        private ChangeSet _next;

        private XmlSerializer _ser_create;

        private XmlSerializer _ser_modify;

        private XmlSerializer _ser_delete;

        private XmlReader _reader;

        private Stream _stream;

        /// <summary>
        /// Creates a new changeset source.
        /// </summary>
        /// <param name="stream"></param>
        public XmlDataProcessorChangeSetSource(Stream stream)
        {
            _stream = stream;
        }

        /// <summary>
        /// Initializes this changeset source.
        /// </summary>
        public override void Initialize()
        {
            _next = null;
            _ser_create = new XmlSerializer(typeof(Osm.Xml.v0_6.create));
            _ser_modify = new XmlSerializer(typeof(Osm.Xml.v0_6.modify));
            _ser_delete = new XmlSerializer(typeof(Osm.Xml.v0_6.delete));

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.CloseInput = true;
            settings.CheckCharacters = false;
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            _reader = XmlReader.Create(_stream, settings);
        }

        /// <summary>
        /// Moves to the next changeset.
        /// </summary>
        /// <returns></returns>
        public override bool MoveNext()
        {
            while (_reader.Read())
            {
                if (_reader.NodeType == XmlNodeType.Element && (_reader.Name == "modify" || _reader.Name == "create"||_reader.Name == "delete"))
                {
                    // create a stream for only this element.
                    string name = _reader.Name;
                    string next_element = _reader.ReadOuterXml();
                    XmlReader reader = XmlReader.Create(new MemoryStream(Encoding.UTF8.GetBytes(next_element)));
                    object osm_obj = null;

                    // select type of element.
                    switch (name)
                    {
                        case "delete":
                            osm_obj = _ser_delete.Deserialize(reader);
                            if (osm_obj is OsmSharp.Osm.Xml.v0_6.delete)
                            {
                                _next = XmlSimpleConverter.ConvertToSimple(osm_obj as OsmSharp.Osm.Xml.v0_6.delete);
                                return true;
                            }
                            break;
                        case "modify":
                            osm_obj = _ser_modify.Deserialize(reader);
                            if (osm_obj is OsmSharp.Osm.Xml.v0_6.modify)
                            {
                                _next = XmlSimpleConverter.ConvertToSimple(osm_obj as OsmSharp.Osm.Xml.v0_6.modify);
                                return true;
                            }
                            break;
                        case "create":
                            osm_obj = _ser_create.Deserialize(reader);
                            if (osm_obj is OsmSharp.Osm.Xml.v0_6.create)
                            {
                                _next = XmlSimpleConverter.ConvertToSimple(osm_obj as OsmSharp.Osm.Xml.v0_6.create);
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
        /// Returns the current changeset.
        /// </summary>
        /// <returns></returns>
        public override ChangeSet Current()
        {
            return _next;
        }

        /// <summary>
        /// Resets this changeset source.
        /// </summary>
        public override void Reset()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.CloseInput = true;
            settings.CheckCharacters = false;
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;

            _stream.Seek(0, SeekOrigin.Begin);
            _reader = XmlReader.Create(_stream, settings);
        }

        /// <summary>
        /// Closes this changeset source.
        /// </summary>
        public override void Close()
        {

        }
    }
}