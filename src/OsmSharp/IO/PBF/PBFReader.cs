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

using System;
using System.IO;
using System.IO.Compression;
using ProtoBuf;
using ProtoBuf.Meta;

namespace OsmSharp.IO.PBF;

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

    private readonly PrimitiveBlock _block = new PrimitiveBlock();
    private readonly BlobHeader _header = new BlobHeader();

    /// <summary>
    /// Skips over the next blob without decompressing its payload.
    /// Reads the length-prefixed BlobHeader (to learn <c>datasize</c>) and then advances the
    /// underlying stream past the blob body. Returns <c>true</c> when a blob was skipped,
    /// <c>false</c> on end-of-stream. The blob's contents are NOT decoded — callers relying
    /// on this must have out-of-band knowledge (e.g. a prior block index) about what's inside.
    /// </summary>
    public bool SkipNext()
    {
        if (!Serializer.TryReadLengthPrefix(_stream, PrefixStyle.Fixed32BigEndian, out var length)) return false;

        var headerBytes = ReadExact(_stream, length);
        BlobHeader header;
        using (var ms = new MemoryStream(headerBytes))
        {
            header = _runtimeTypeModel.Deserialize<BlobHeader>(ms, _header, _blockHeaderType);
        }

        // Advance past the payload. Prefer Seek when possible; fall back to a read-and-discard
        // loop for non-seekable streams so the API stays valid even without CanSeek.
        var datasize = header.datasize;
        if (_stream.CanSeek)
        {
            _stream.Seek(datasize, SeekOrigin.Current);
        }
        else
        {
            var buffer = new byte[Math.Min(datasize, 64 * 1024)];
            var remaining = datasize;
            while (remaining > 0)
            {
                var read = _stream.Read(buffer, 0, Math.Min(remaining, buffer.Length));
                if (read <= 0) return false;
                remaining -= read;
            }
        }
        return true;
    }

    /// <summary>
    /// Moves to the next primitive block, returns null at the end.
    /// </summary>
    /// <returns></returns>
    public PrimitiveBlock MoveNext()
    {
        // make sure previous block data is removed.
        _block.primitivegroup?.Clear();
        _block.stringtable?.s.Clear();

        // read next block.
        PrimitiveBlock block = null;
        var notFoundBut = true;
        while (notFoundBut)
        { // continue if there is still data but not a primitiveblock.
            notFoundBut = false; // not found.
            if (!Serializer.TryReadLengthPrefix(_stream, PrefixStyle.Fixed32BigEndian, out var length)) continue;

            var headerBytes = ReadExact(_stream, length);
            BlobHeader header;
            using (var ms = new MemoryStream(headerBytes))
            {
                header = _runtimeTypeModel.Deserialize<BlobHeader>(ms, _header, _blockHeaderType);
            }
            var blobBytes = ReadExact(_stream, header.datasize);
            Blob blob;
            using (var ms = new MemoryStream(blobBytes))
            {
                blob = _runtimeTypeModel.Deserialize(ms, null, _blobType) as Blob;
            }

            // construct the source stream, compressed or not.
            Stream sourceStream;
            if (blob.zlib_data == null)
            { // use a regular uncompressed stream.
                sourceStream = new MemoryStream(blob.raw);
            }
            else
            { // wrap the zlib blob in ZLibStream — handles the 2-byte header and Adler32 trailer.
                sourceStream = new ZLibStream(new MemoryStream(blob.zlib_data), CompressionMode.Decompress);
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
                    block = _runtimeTypeModel.Deserialize<PrimitiveBlock>(sourceStream, _block, _primitiveBlockType);
                }
            }
        }
        return block;
    }

    /// <summary>
    /// Reads exactly <paramref name="count"/> bytes from <paramref name="stream"/> into a
    /// fresh buffer. Throws <see cref="EndOfStreamException"/> if the stream ends first.
    /// Replaces the previous <c>LimitedStream</c> wrapper used to cap protobuf deserialization
    /// reads — with net6+ we just materialize the exact byte slice and wrap it in a
    /// <see cref="MemoryStream"/>.
    /// </summary>
    private static byte[] ReadExact(Stream stream, int count)
    {
        var buffer = new byte[count];
        var total = 0;
        while (total < count)
        {
            var read = stream.Read(buffer, total, count - total);
            if (read == 0) throw new EndOfStreamException($"Expected {count} bytes, got {total}.");
            total += read;
        }
        return buffer;
    }
}
