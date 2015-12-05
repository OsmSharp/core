// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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

using OsmSharp.Osm.Streams;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Osm.PBF.Streams
{
    /// <summary>
    /// A PBF stream target.
    /// </summary>
    public class PBFOsmStreamTarget : OsmStreamTarget
    {
        private readonly Stream _stream;
        private readonly RuntimeTypeModel _runtimeTypeModel;
        private readonly Type _blobHeaderType = typeof(BlobHeader);
        private readonly Type _blobType = typeof(Blob);
        private readonly Type _primitiveBlockType = typeof(PrimitiveBlock);
        private readonly Type _headerBlockType = typeof(HeaderBlock);
        private readonly bool _compress = false;

        /// <summary>
        /// Creates a new PBF stream target.
        /// </summary>
        public PBFOsmStreamTarget(Stream stream)
        {
            _stream = stream;

            _currentEntities = new List<Osm.OsmGeo>();
            _reverseStringTable = new Dictionary<string, int>();
            _buffer = new MemoryStream();

            _runtimeTypeModel = RuntimeTypeModel.Create();
            _runtimeTypeModel.Add(_blobHeaderType, true);
            _runtimeTypeModel.Add(_blobType, true);
            _runtimeTypeModel.Add(_primitiveBlockType, true);
            _runtimeTypeModel.Add(_headerBlockType, true);
        }

        private List<Osm.OsmGeo> _currentEntities;
        private Dictionary<string, int> _reverseStringTable;
        private MemoryStream _buffer;

        /// <summary>
        /// Initializes this target.
        /// </summary>
        public override void Initialize()
        {
            _currentEntities.Clear();

            // write the mandatory header.
            _buffer.Seek(0, SeekOrigin.Begin);

            // create header block.
            var blockHeader = new HeaderBlock();
            blockHeader.required_features.Add("OsmSchema-V0.6");
            blockHeader.required_features.Add("DenseNodes");
            _runtimeTypeModel.Serialize(_buffer, blockHeader);
            var blockHeaderData = _buffer.ToArray();
            _buffer.SetLength(0);

            // create blob.
            var blob = new Blob();
            blob.raw = blockHeaderData;
            _runtimeTypeModel.Serialize(_buffer, blob);

            // create blobheader.
            var blobHeader = new BlobHeader();
            blobHeader.datasize = (int)_buffer.Length;
            blobHeader.indexdata = null;
            blobHeader.type = Encoder.OSMHeader;
            _runtimeTypeModel.SerializeWithLengthPrefix(_stream, blobHeader, _blobHeaderType, ProtoBuf.PrefixStyle.Fixed32BigEndian, 0);

            // flush to stream.
            _buffer.Seek(0, SeekOrigin.Begin);
            _buffer.CopyTo(_stream);
        }

        /// <summary>
        /// Flushes the current block of data.
        /// </summary>
        private void FlushBlock()
        {
            if (_currentEntities.Count == 0) { return; }

            // encode into block.
            var block = new PrimitiveBlock();
            Encoder.Encode(block, _reverseStringTable, _currentEntities);
            _currentEntities.Clear();
            _reverseStringTable.Clear();

            // serialize.
            _buffer.SetLength(0);
            _runtimeTypeModel.Serialize(_buffer, block);
            var blockBytes = _buffer.ToArray();
            _buffer.SetLength(0);

            if (_compress)
            { // compress buffer.
                throw new NotSupportedException();
            }

            // create blob.
            var blob = new Blob();
            blob.raw = blockBytes;
            _runtimeTypeModel.Serialize(_buffer, blob);

            // create blobheader.
            var blobHeader = new BlobHeader();
            blobHeader.datasize = (int)_buffer.Length;
            blobHeader.indexdata = null;
            blobHeader.type = Encoder.OSMData;
            _runtimeTypeModel.SerializeWithLengthPrefix(_stream, blobHeader, _blobHeaderType, ProtoBuf.PrefixStyle.Fixed32BigEndian, 0);

            // serialize to stream.
            _buffer.Seek(0, SeekOrigin.Begin);
            _buffer.CopyTo(_stream);
        }

        /// <summary>
        /// Adds a node.
        /// </summary>
        public override void AddNode(Osm.Node node)
        {
            _currentEntities.Add(node);
            if (_currentEntities.Count > 8000)
            {
                this.FlushBlock();
            }
        }

        /// <summary>
        /// Adds a way.
        /// </summary>
        public override void AddWay(Osm.Way way)
        {
            _currentEntities.Add(way);
            if (_currentEntities.Count > 8000)
            {
                this.FlushBlock();
            }
        }

        /// <summary>
        /// Adds a relation.
        /// </summary>
        public override void AddRelation(Osm.Relation relation)
        {
            _currentEntities.Add(relation);
            if (_currentEntities.Count > 8000)
            {
                this.FlushBlock();
            }
        }

        /// <summary>
        /// Flushes data in this stream.
        /// </summary>
        public override void Flush()
        {
            this.FlushBlock();
            _stream.Flush();
        }

        /// <summary>
        /// Closes this target.
        /// </summary>
        public override void Close()
        {
            this.Flush();
        }
    }
}