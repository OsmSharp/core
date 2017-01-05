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

using Ionic.Zlib;
using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.IO;

namespace OsmSharp.IO.PBF
{
    /// <summary>
    /// Reads PBF files.
    /// </summary>
    public class PBFReader
    {
        private readonly Stream _stream;
        private readonly RuntimeTypeModel _runtimeTypeModel;
        private readonly Type _blockHeaderType = typeof(BlobHeader);
        private readonly Type _blobType = typeof(Blob);
        private readonly Type _primitiveBlockType = typeof(PrimitiveBlock);
        private readonly Type _headerBlockType = typeof(HeaderBlock);

        /// <summary>
        /// Creates a new PBF reader.
        /// </summary>
        public PBFReader(Stream stream)
        {
            _stream = stream;

            _runtimeTypeModel = RuntimeTypeModel.Create();
            _runtimeTypeModel.Add(_blockHeaderType, true);
            _runtimeTypeModel.Add(_blobType, true);
            _runtimeTypeModel.Add(_primitiveBlockType, true);
            _runtimeTypeModel.Add(_headerBlockType, true);
        }

        /// <summary>
        /// Closes this reader.
        /// </summary>
        public void Dispose()
        {
            _stream.Dispose();
        }

        /// <summary>
        /// Holds a block object that can be reused.
        /// </summary>
        private PrimitiveBlock _block = new PrimitiveBlock();

        /// <summary>
        /// Moves to the next primitive block, returns null at the end.
        /// </summary>
        /// <returns></returns>
        public PrimitiveBlock MoveNext()
        {
            // make sure previous block data is removed.
            if (_block.primitivegroup != null)
            {
                _block.primitivegroup.Clear();
            }
            if (_block.stringtable != null)
            {
                _block.stringtable.s.Clear();
            }

            // read next block.
            PrimitiveBlock block = null;
            int length;
            bool notFoundBut = true;
            while (notFoundBut)
            { // continue if there is still data but not a primitiveblock.
                notFoundBut = false; // not found.
                if (Serializer.TryReadLengthPrefix(_stream, PrefixStyle.Fixed32BigEndian, out length))
                {
                    // TODO: remove some of the v1 specific code.
                    // TODO: this means also to use the built-in capped streams.

                    // code borrowed from: http://stackoverflow.com/questions/4663298/protobuf-net-deserialize-open-street-maps

                    // I'm just being lazy and re-using something "close enough" here
                    // note that v2 has a big-endian option, but Fixed32 assumes little-endian - we
                    // actually need the other way around (network byte order):
                    // length = IntLittleEndianToBigEndian((uint)length);

                    BlobHeader header;
                    // again, v2 has capped-streams built in, but I'm deliberately
                    // limiting myself to v1 features
                    using (var tmp = new LimitedStream(_stream, length))
                    {
                        header = _runtimeTypeModel.Deserialize(tmp, null, _blockHeaderType) as BlobHeader;
                    }
                    Blob blob;
                    using (var tmp = new LimitedStream(_stream, header.datasize))
                    {
                        blob = _runtimeTypeModel.Deserialize(tmp, null, _blobType) as Blob;
                    }

                    // construct the source stream, compressed or not.
                    Stream sourceStream = null;
                    if (blob.zlib_data == null)
                    { // use a regular uncompressed stream.
                        sourceStream = new MemoryStream(blob.raw);
                    }
                    else
                    { // construct a compressed stream.
                        var ms = new MemoryStream(blob.zlib_data);
                        sourceStream = new ZLibStreamWrapper(ms);
                    }

                    // use the stream to read the block.
                    using (sourceStream)
                    {
                        if (header.type == Encoder.OSMHeader)
                        {
                            _runtimeTypeModel.Deserialize(sourceStream, null, _headerBlockType);
                            notFoundBut = true;
                        }

                        if (header.type == Encoder.OSMData)
                        {
                            block = _runtimeTypeModel.Deserialize(sourceStream, _block, _primitiveBlockType) as PrimitiveBlock;
                        }
                    }
                }
            }
            return block;
        }

        // 4-byte number
        private static int IntLittleEndianToBigEndian(uint i)
        {
            return (int)(((i & 0xff) << 24) + ((i & 0xff00) << 8) + ((i & 0xff0000) >> 8) + ((i >> 24) & 0xff));
        }
    }

    abstract class InputStream : Stream
    {
        protected abstract int ReadNextBlock(byte[] buffer, int offset, int count);
        public sealed override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead, totalRead = 0;
            while (count > 0 && (bytesRead = ReadNextBlock(buffer, offset, count)) > 0)
            {
                count -= bytesRead;
                offset += bytesRead;
                totalRead += bytesRead;
                pos += bytesRead;
            }
            return totalRead;
        }
        long pos;
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }
        public override long Position
        {
            get
            {
                return pos;
            }
            set
            {
                if (pos != value) throw new NotImplementedException();
            }
        }
        public override long Length
        {
            get { throw new NotImplementedException(); }
        }
        public override void Flush()
        {
            throw new NotImplementedException();
        }
        public override bool CanWrite
        {
            get { return false; }
        }
        public override bool CanRead
        {
            get { return true; }
        }
        public override bool CanSeek
        {
            get { return false; }
        }
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }
    }
    class ZLibStreamWrapper : InputStream
    {
        private ZlibStream reader; 
        public ZLibStreamWrapper(Stream stream)
        {
            reader = new ZlibStream(stream, CompressionMode.Decompress);
        }
        protected override int ReadNextBlock(byte[] buffer, int offset, int count)
        {
            return reader.Read(buffer, offset, count);
        }

    }
    // deliberately doesn't dispose the base-stream    
    class LimitedStream : InputStream
    {
        private Stream stream;
        private long remaining;
        public LimitedStream(Stream stream, long length)
        {
            if (length < 0) throw new ArgumentOutOfRangeException("length");
            if (stream == null) throw new ArgumentNullException("stream");
            if (!stream.CanRead) throw new ArgumentException("stream");
            this.stream = stream;
            this.remaining = length;
        }
        protected override int ReadNextBlock(byte[] buffer, int offset, int count)
        {
            if (count > remaining) count = (int)remaining;
            int bytesRead = stream.Read(buffer, offset, count);
            if (bytesRead > 0) remaining -= bytesRead;
            return bytesRead;
        }
    }
}