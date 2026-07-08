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

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using OsmSharp.IO.PBF;
using ProtoBuf.Meta;

namespace OsmSharp.Streams;

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
    private readonly bool _compress;
    private readonly CompressionLevel _level;

    // Set when the target was constructed from a PBF file path. Enables the block-index
    // persistence path: an in-memory index is populated for every primitive blob written,
    // periodically flushed to `{pbfPath}.blockindex` and once more on Close(). A reader
    // constructed against this file inherits the index directly — no cold walk needed.
    private readonly string _pbfPath;
    private readonly string _blockIndexPath;
    private readonly bool _persistBlockIndex;
    private readonly bool _ownsStream;

    private readonly PBFBlockIndex _blockIndex = new PBFBlockIndex();
    private int _pendingMutations;
    private const int FlushMutationThreshold = 256;

    // Guards Close() against double-invocation. OsmStreamTarget.Pull() already ends with a
    // Close() call, so a caller that wraps the target in a try/finally would otherwise
    // double-dispose the owned FileStream and throw ObjectDisposedException on the second
    // Flush().
    private bool _closed;

    /// <summary>
    /// Creates a new PBF stream target that writes to the given stream.
    /// </summary>
    /// <param name="stream">The output stream.</param>
    /// <param name="compress">if set to <c>true</c> use compression.</param>
    /// <param name="compressionLevel">Compression level applied when <paramref name="compress"/> is true.</param>
    public PBFOsmStreamTarget(
        Stream stream,
        bool compress = true,
        CompressionLevel compressionLevel = CompressionLevel.Optimal)
        : this(stream, pbfPath: null, persistBlockIndex: false, ownsStream: false, compress, compressionLevel)
    {
    }

    /// <summary>
    /// Creates a new PBF stream target that writes to <paramref name="pbfPath"/>. The
    /// target owns the file handle and closes it on <see cref="Close"/>. When
    /// <paramref name="persistBlockIndex"/> is <c>true</c>, a block-index sidecar is
    /// written to <c>{pbfPath}.blockindex</c> as the file is produced — periodically
    /// during the run and once more on <see cref="Close"/>. A reader constructed against
    /// the produced file will auto-load the sidecar and skip the cold-walk phase entirely.
    /// </summary>
    public PBFOsmStreamTarget(
        string pbfPath,
        bool persistBlockIndex = false,
        bool compress = true,
        CompressionLevel compressionLevel = CompressionLevel.Optimal)
        : this(File.Create(pbfPath), pbfPath, persistBlockIndex, ownsStream: true, compress, compressionLevel)
    {
    }

    private PBFOsmStreamTarget(
        Stream stream, string pbfPath, bool persistBlockIndex, bool ownsStream,
        bool compress, CompressionLevel compressionLevel)
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

        _compress = compress;
        _level = compressionLevel;

        _pbfPath = pbfPath;
        _blockIndexPath = pbfPath != null ? pbfPath + ".blockindex" : null;
        _persistBlockIndex = persistBlockIndex;
        _ownsStream = ownsStream;

        // A fresh write invalidates any prior sidecar. The reader would discard a stale
        // one anyway via the length+mtime check, but removing it up-front avoids leaving
        // a misleading file in the directory while the write is in progress.
        if (_blockIndexPath != null && File.Exists(_blockIndexPath))
        {
            try { File.Delete(_blockIndexPath); }
            catch (IOException) { /* not fatal — will be overwritten on first flush. */ }
        }
    }

    private readonly List<OsmGeo> _currentEntities;
    private readonly Dictionary<string, int> _reverseStringTable;
    private readonly MemoryStream _buffer;

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
        blob.raw_size = blockHeaderData.Length;
        if (_compress)
        {
            using (var target = new MemoryStream())
            {
                using (var deflate = new ZLibStream(target, _level, leaveOpen: true))
                {
                    deflate.Write(blockHeaderData, 0, blockHeaderData.Length);
                }
                blob.zlib_data = target.ToArray();
            }
        }
        else
        {
            blob.raw = blockHeaderData;
        }

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

        // Capture the file offset of the blob about to be written, and derive Tier-1/Tier-2
        // info from the entities before Encoder.Encode clears them. Only meaningful when the
        // stream is seekable — a non-seekable stream can't be recorded (offsets aren't
        // knowable), but the pass through _persistBlockIndex should never be true in that
        // case since the path ctor uses File.Create which is seekable.
        var canRecord = _persistBlockIndex && _stream.CanSeek;
        var beforeOffset = canRecord ? _stream.Position : -1L;
        PBFBlockEntry pendingEntry = default;
        if (canRecord)
        {
            pendingEntry = this.BuildEntryFromEntities(beforeOffset);
        }

        // encode into block.
        var block = new PrimitiveBlock();
        Encoder.Encode(block, _reverseStringTable, _currentEntities, _compress);
        _currentEntities.Clear();
        _reverseStringTable.Clear();

        // serialize.
        _buffer.SetLength(0);
        _runtimeTypeModel.Serialize(_buffer, block);
        var blockBytes = _buffer.ToArray();
        _buffer.SetLength(0);

        // create blob.
        var blob = new Blob();
        blob.raw_size = blockBytes.Length;
        if (_compress)
        {
            using (var target = new MemoryStream())
            {
                using (var deflate = new ZLibStream(target, _level, leaveOpen: true))
                {
                    deflate.Write(blockBytes, 0, blockBytes.Length);
                }
                blob.zlib_data = target.ToArray();
            }
        }
        else
        {
            blob.raw = blockBytes;
        }

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

        if (canRecord)
        {
            pendingEntry.EndOffset = _stream.Position;
            _blockIndex.Upsert(pendingEntry);
            _pendingMutations++;
            if (_pendingMutations >= FlushMutationThreshold) this.WriteBlockIndex();
        }
    }

    /// <summary>
    /// Derives a <see cref="PBFBlockEntry"/> from the currently buffered entities. Called
    /// from <see cref="FlushBlock"/> right before <c>Encoder.Encode</c> clears the buffer.
    /// Cheaper than the source's <c>BlockRecorder</c> path: the writer already has every
    /// element in hand, so a single pass over <see cref="_currentEntities"/> gives both
    /// Tier-1 presence and Tier-2 id ranges.
    /// </summary>
    private PBFBlockEntry BuildEntryFromEntities(long fileOffset)
    {
        var entry = new PBFBlockEntry { FileOffset = fileOffset };
        foreach (var geo in _currentEntities)
        {
            switch (geo.Type)
            {
                case OsmGeoType.Node:
                    entry.HasNodes = true;
                    if (geo.Id.HasValue) TrackId(ref entry.NodeIdsKnown, ref entry.NodeMinId, ref entry.NodeMaxId, geo.Id.Value);
                    break;
                case OsmGeoType.Way:
                    entry.HasWays = true;
                    if (geo.Id.HasValue) TrackId(ref entry.WayIdsKnown, ref entry.WayMinId, ref entry.WayMaxId, geo.Id.Value);
                    break;
                case OsmGeoType.Relation:
                    entry.HasRelations = true;
                    if (geo.Id.HasValue) TrackId(ref entry.RelationIdsKnown, ref entry.RelationMinId, ref entry.RelationMaxId, geo.Id.Value);
                    break;
            }
        }
        return entry;
    }

    private static void TrackId(ref bool known, ref long min, ref long max, long id)
    {
        if (!known) { known = true; min = id; max = id; return; }
        if (id < min) min = id;
        if (id > max) max = id;
    }

    private void WriteBlockIndex()
    {
        if (_blockIndexPath == null) return;
        // Reset the counter regardless of outcome — matches the source-side behavior:
        // on I/O failure we don't want to spin retrying every flush.
        BlockIndexSidecar.Save(_blockIndexPath, _pbfPath, _blockIndex.Snapshot());
        _pendingMutations = 0;
    }

    /// <summary>
    /// Adds a node.
    /// </summary>
    public override void AddNode(Node node)
    {
        _currentEntities.Add(node);
        if (_currentEntities.Count >= 8000)
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
        if (_currentEntities.Count >= 8000)
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
        if (_currentEntities.Count >= 8000)
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
    /// Closes this target. Flushes any pending data, writes the final block-index sidecar
    /// when persistence is enabled, and disposes the underlying stream if this target
    /// opened it (path constructor).
    /// </summary>
    public override void Close()
    {
        if (_closed) return;
        _closed = true;

        this.Flush();

        if (_persistBlockIndex && _pendingMutations > 0)
        {
            this.WriteBlockIndex();
        }

        if (_ownsStream)
        {
            _stream.Dispose();
        }
    }
}
