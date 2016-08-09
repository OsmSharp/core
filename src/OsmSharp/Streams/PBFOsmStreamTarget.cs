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

using OsmSharp.IO.PBF;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Streams
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

            _currentEntities = new List<OsmGeo>();
            _reverseStringTable = new Dictionary<string, int>();
            _buffer = new MemoryStream();

            _runtimeTypeModel = RuntimeTypeModel.Create();
            _runtimeTypeModel.Add(_blobHeaderType, true);
            _runtimeTypeModel.Add(_blobType, true);
            _runtimeTypeModel.Add(_primitiveBlockType, true);
            _runtimeTypeModel.Add(_headerBlockType, true);
        }

        private List<OsmGeo> _currentEntities;
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
        public override void AddNode(Node node)
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
        public override void AddWay(Way way)
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
        public override void AddRelation(Relation relation)
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