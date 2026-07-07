// The MIT License (MIT)

// Copyright (c) 2017 Ben Abelshausen

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
using OsmSharp.IO.PBF;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Streams
{
    /// <summary>
    /// A source of PBF formatted OSM data.
    /// </summary>
    public class PBFOsmStreamSource : OsmStreamSource, IPBFOsmPrimitiveConsumer
    {
        private readonly Stream _stream;
        private readonly long? _initialPosition;

        /// <summary>
        /// Creates a new source of PBF formatted OSM data.
        /// </summary>
        public PBFOsmStreamSource(Stream stream)
        {
            _stream = stream;
            _initialPosition = null;
            if (_stream.CanSeek)
            {
                _initialPosition = _stream.Position;
            }
        }

        private bool _initialized = false;

        /// <summary>
        /// Initializes the current source.
        /// </summary>
        private void Initialize()
        {
            this.InitializePBFReader();
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
            if (!_initialized)
            {
                this.Initialize();
                _initialized = true;
            }

            var nextPBFPrimitive = this.MoveToNextPrimitive(ignoreNodes, ignoreWays, ignoreRelations, null, null);
            while(nextPBFPrimitive.Value != null)
            {
                switch (nextPBFPrimitive.Value)
                {
                    case IO.PBF.Node node when !ignoreNodes: // next primitive is a node.
                        _current = Encoder.DecodeNode(nextPBFPrimitive.Key, node);
                        return true;
                    case IO.PBF.Way way when !ignoreWays: // next primitive is a way.
                        _current = Encoder.DecodeWay(nextPBFPrimitive.Key, way);
                        return true;
                    case IO.PBF.Relation relation when !ignoreRelations: // next primitive is a relation.
                        _current = Encoder.DecodeRelation(nextPBFPrimitive.Key, relation);
                        return true;
                    default:
                        nextPBFPrimitive = this.MoveToNextPrimitive(ignoreNodes, ignoreWays, ignoreRelations, null, null);
                        break;
                }
            }
            return false;
        }

        /// <summary>
        /// Advances to the next element of the given <paramref name="type"/> whose id is
        /// greater than or equal to <paramref name="minId"/>. Elements of other types are
        /// skipped. Returns <c>true</c> and updates <see cref="Current"/> when one is found,
        /// or <c>false</c> at end of stream.
        ///
        /// Forward-only. Throws <see cref="InvalidOperationException"/> when the current
        /// element sits after the requested position — either because it's the same type
        /// with an id past <paramref name="minId"/>, or because its type comes later in
        /// the PBF section order (nodes → ways → relations). Use <see cref="Reset"/> to
        /// restart, or a future <c>MoveTo</c> overload once one exists.
        ///
        /// Uses the internal block index (accreted from prior walks) to seek past whole blobs
        /// that don't contain the requested type, and past blobs whose id range for the type
        /// is entirely below <paramref name="minId"/>. Cold blobs still decode as today; the
        /// speedup grows as the stream is walked.
        /// </summary>
        public bool MoveNext(OsmGeoType type, long minId)
        {
            if (!_initialized)
            {
                this.Initialize();
                _initialized = true;
            }

            if (_current != null)
            {
                // PBF ordering is nodes → ways → relations. A caller that already saw a way
                // and asks for a node (or already saw a relation and asks for anything earlier)
                // cannot get there by moving forward. Refuse rather than silently return false
                // or return the wrong element from the queue.
                var currentOrder = TypeOrder(_current.Type);
                var requestedOrder = TypeOrder(type);
                if (currentOrder > requestedOrder)
                {
                    throw new InvalidOperationException(
                        $"Cannot advance to {type} id={minId}: current is a {_current.Type} " +
                        $"(id={_current.Id?.ToString() ?? "?"}), which comes after all {type}s in PBF ordering. " +
                        "Use Reset() to restart. A rewinding MoveTo(type, id) API will be added separately.");
                }
                if (_current.Type == type && _current.Id.HasValue && _current.Id.Value > minId)
                {
                    throw new InvalidOperationException(
                        $"Cannot advance to {type} id={minId}: current {type} id={_current.Id.Value} is already past it. " +
                        "Use Reset() to restart. A rewinding MoveTo(type, id) API will be added separately.");
                }
            }

            var ignoreNodes = type != OsmGeoType.Node;
            var ignoreWays = type != OsmGeoType.Way;
            var ignoreRelations = type != OsmGeoType.Relation;

            var next = this.MoveToNextPrimitive(ignoreNodes, ignoreWays, ignoreRelations, type, minId);
            while (next.Value != null)
            {
                switch (next.Value)
                {
                    case IO.PBF.Node node when type == OsmGeoType.Node:
                        if (node.id < minId) break;
                        _current = Encoder.DecodeNode(next.Key, node);
                        return true;
                    case IO.PBF.Way way when type == OsmGeoType.Way:
                        if (way.id < minId) break;
                        _current = Encoder.DecodeWay(next.Key, way);
                        return true;
                    case IO.PBF.Relation relation when type == OsmGeoType.Relation:
                        if (relation.id < minId) break;
                        _current = Encoder.DecodeRelation(next.Key, relation);
                        return true;
                }
                next = this.MoveToNextPrimitive(ignoreNodes, ignoreWays, ignoreRelations, type, minId);
            }
            return false;
        }

        /// <summary>
        /// Holds the current object.
        /// </summary>
        private OsmSharp.OsmGeo _current;

        /// <summary>
        /// Returns the current geometry.
        /// </summary>
        public override OsmSharp.OsmGeo Current()
        {
            return _current;
        }

        /// <summary>
        /// Resetting this data source.
        /// Preserves the accreted block index — subsequent walks reuse it to fast-skip blobs.
        /// </summary>
        public override void Reset()
        {
            if (_initialPosition == null) throw new NotSupportedException(
                $"Cannot reset this stream, source stream is not seekable, check {nameof(this.CanReset)} before calling {nameof(this.Reset)}");

            _current = null;
            _cachedPrimitives?.Clear();
            _stream.Seek(_initialPosition.Value, SeekOrigin.Begin);
        }

        /// <summary>
        /// Returns true if this source can be reset.
        /// </summary>
        public override bool CanReset => _initialPosition.HasValue && _stream.CanSeek;

        #region PBF Blocks Reader

        private PBFReader _reader;
        private long _firstWayPosition = -1;
        private long _firstRelationPosition = -1;

        // Internal block index accreted as blobs are decoded. Preserved across Reset() so a
        // second walk can fast-skip blobs whose contents are known to be unwanted. Never
        // exposed publicly — misconfiguring blob offsets would silently misread the file.
        private readonly PBFBlockIndex _blockIndex = new PBFBlockIndex();
        private BlockRecorder _activeRecorder;

        /// <summary>
        /// Initializes the PBF reader.
        /// </summary>
        private void InitializePBFReader()
        {
            _reader = new PBFReader(_stream);

            this.InitializeBlockCache();
        }

        /// <summary>
        /// Moves the PBF reader to the next primitive or returns one of the cached ones.
        /// </summary>
        /// <returns></returns>
        private KeyValuePair<PrimitiveBlock, object> MoveToNextPrimitive(
            bool ignoreNodes, bool ignoreWays, bool ignoreRelations,
            OsmGeoType? onlyType, long? minId)
        {
            // Drain any leftover primitives first, honoring the extra (onlyType, minId) filter.
            var next = this.DeQueueMatchingPrimitive(ignoreNodes, ignoreWays, ignoreRelations, onlyType, minId);
            if (next.Value != null) return next;

            while (true)
            {
                // Coarse pre-seek from the two learned positions. Kept as-is: composes fine with
                // the block index — the index gives finer-grained skipping on top.
                if (_firstWayPosition > 0 && ignoreNodes && !ignoreWays)
                { // if nodes have to be ignored, there was already a first pass and ways are not to be ignored jump to the first way.
                    if (_stream.Position <= _firstWayPosition)
                    { // only just to the first way if that hasn't happened yet.
                        _stream.Seek(_firstWayPosition, SeekOrigin.Begin);
                    }
                }

                if (_firstRelationPosition > 0 && ignoreNodes && ignoreWays && !ignoreRelations)
                {
                    // if nodes and ways have to be ignored, there was already a first pass and ways are not be ignored jump to the first relation.
                    if (_stream.Position < _firstRelationPosition)
                    {
                        // only just to the first relation if that hasn't happened yet.
                        _stream.Seek(_firstRelationPosition, SeekOrigin.Begin);
                    }
                }

                // just to the next block.
                long beforeBlockPosition = -1;
                if (_stream.CanSeek) beforeBlockPosition = _stream.Position;

                // Warm-path skip: if we've seen this blob before AND every "wanted" primitive
                // type in it is either not present, or (Tier-2 known) has maxId < minId, seek
                // past without decompressing.
                if (beforeBlockPosition >= 0 &&
                    _blockIndex.TryGetAt(beforeBlockPosition, out var known) &&
                    CanSkipKnownBlob(known, ignoreNodes, ignoreWays, ignoreRelations, onlyType, minId))
                {
                    _stream.Seek(known.EndOffset, SeekOrigin.Begin);
                    continue;
                }

                // Cold path: decode the block. Records Tier-1 (presence) unconditionally and
                // Tier-2 (id ranges) only for types that were actually decoded — types the caller
                // ignored aren't tracked, and will fall back to full decode next time we care.
                var recorder = new BlockRecorder();
                _activeRecorder = recorder;

                var block = _reader.MoveNext();
                if (block == null)
                {
                    _activeRecorder = null;
                    return default;
                }

                bool hasNodes = false, hasWays = false, hasRelations = false;
                while (block != null && !block.Decode(this, ignoreNodes, ignoreWays, ignoreRelations,
                    out hasNodes, out hasWays, out hasRelations))
                {
                    if (hasWays && _firstWayPosition == -1)
                    {
                        _firstWayPosition = beforeBlockPosition;
                    }
                    if (hasRelations && _firstRelationPosition == -1)
                    {
                        _firstRelationPosition = beforeBlockPosition;
                    }

                    // Blob was present but yielded nothing (everything ignored). Record what
                    // we now know before advancing to the next blob.
                    UpsertIndex(recorder, beforeBlockPosition, hasNodes, hasWays, hasRelations,
                        ignoreNodes, ignoreWays, ignoreRelations);
                    recorder = new BlockRecorder();
                    _activeRecorder = recorder;

                    if (_stream.CanSeek) beforeBlockPosition = _stream.Position;
                    block = _reader.MoveNext();
                    if (block == null)
                    {
                        _activeRecorder = null;
                        return default;
                    }
                }
                if (hasWays && _firstWayPosition == -1)
                {
                    _firstWayPosition = beforeBlockPosition;
                }
                if (hasRelations && _firstRelationPosition == -1)
                {
                    _firstRelationPosition = beforeBlockPosition;
                }
                UpsertIndex(recorder, beforeBlockPosition, hasNodes, hasWays, hasRelations,
                    ignoreNodes, ignoreWays, ignoreRelations);
                _activeRecorder = null;

                next = this.DeQueueMatchingPrimitive(ignoreNodes, ignoreWays, ignoreRelations, onlyType, minId);
                if (next.Value != null) return next;

                // Nothing in this blob matched the (onlyType, minId) constraint; loop to the next blob.
            }
        }

        private void UpsertIndex(BlockRecorder recorder, long beforeBlockPosition,
            bool hasNodes, bool hasWays, bool hasRelations,
            bool ignoredNodes, bool ignoredWays, bool ignoredRelations)
        {
            if (beforeBlockPosition < 0) return;
            var endOffset = _stream.CanSeek ? _stream.Position : -1;
            if (endOffset < 0) return;

            var entry = new PBFBlockEntry
            {
                FileOffset = beforeBlockPosition,
                EndOffset = endOffset,
                HasNodes = hasNodes,
                HasWays = hasWays,
                HasRelations = hasRelations,
            };

            // Only record id ranges for types that were actually decoded on this pass. Types
            // the caller ignored didn't fire our Process* callbacks, so the recorder has no
            // data for them. Leave those Tier-2 fields at their "unknown" sentinel.
            if (!ignoredNodes && recorder.HasNodeIds)
            {
                entry.NodeMinId = recorder.NodeMinId;
                entry.NodeMaxId = recorder.NodeMaxId;
                entry.NodeIdsKnown = true;
            }
            if (!ignoredWays && recorder.HasWayIds)
            {
                entry.WayMinId = recorder.WayMinId;
                entry.WayMaxId = recorder.WayMaxId;
                entry.WayIdsKnown = true;
            }
            if (!ignoredRelations && recorder.HasRelationIds)
            {
                entry.RelationMinId = recorder.RelationMinId;
                entry.RelationMaxId = recorder.RelationMaxId;
                entry.RelationIdsKnown = true;
            }

            _blockIndex.Upsert(entry);
        }

        // Section order in a well-formed PBF: nodes first, then ways, then relations. Kept as
        // an explicit helper so the forward-only guard on MoveNext(type, minId) doesn't depend
        // on the enum's numeric values, which are declaration-order artefacts.
        private static int TypeOrder(OsmGeoType type)
        {
            switch (type)
            {
                case OsmGeoType.Node: return 0;
                case OsmGeoType.Way: return 1;
                case OsmGeoType.Relation: return 2;
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private static bool CanSkipKnownBlob(PBFBlockEntry entry,
            bool ignoreNodes, bool ignoreWays, bool ignoreRelations,
            OsmGeoType? onlyType, long? minId)
        {
            // If any present type is wanted (not ignored), we generally can't skip. But if
            // (onlyType, minId) is set, we can still skip when Tier-2 tells us the id range
            // for the wanted type is entirely below minId.

            if (entry.HasNodes && !ignoreNodes)
            {
                if (!TypeExhaustedByMinId(entry, OsmGeoType.Node, onlyType, minId)) return false;
            }
            if (entry.HasWays && !ignoreWays)
            {
                if (!TypeExhaustedByMinId(entry, OsmGeoType.Way, onlyType, minId)) return false;
            }
            if (entry.HasRelations && !ignoreRelations)
            {
                if (!TypeExhaustedByMinId(entry, OsmGeoType.Relation, onlyType, minId)) return false;
            }
            return true;
        }

        private static bool TypeExhaustedByMinId(PBFBlockEntry entry, OsmGeoType typePresent,
            OsmGeoType? onlyType, long? minId)
        {
            // Only meaningful when the caller asked for a single type + minId, and that type
            // matches the one present here. Otherwise we can't say the blob is exhaustable.
            if (!minId.HasValue || onlyType != typePresent) return false;

            switch (typePresent)
            {
                case OsmGeoType.Node:     return entry.NodeIdsKnown     && entry.NodeMaxId     < minId.Value;
                case OsmGeoType.Way:      return entry.WayIdsKnown      && entry.WayMaxId      < minId.Value;
                case OsmGeoType.Relation: return entry.RelationIdsKnown && entry.RelationMaxId < minId.Value;
                default: return false;
            }
        }

        #region Block Cache

        /// <summary>
        /// Holds the cached primitives.
        /// </summary>
        private Queue<KeyValuePair<PrimitiveBlock, object>> _cachedPrimitives;

        /// <summary>
        /// Initializes the block cache.
        /// </summary>
        private void InitializeBlockCache()
        {
            _cachedPrimitives = new Queue<KeyValuePair<PrimitiveBlock, object>>();
        }

        /// <summary>
        /// Queues the primitives.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="primitive"></param>
        private void QueuePrimitive(PrimitiveBlock block, object primitive)
        {
            _cachedPrimitives.Enqueue(new KeyValuePair<PrimitiveBlock, object>(block, primitive));
        }

        /// <summary>
        /// DeQueues the next primitive matching the caller's filter, discarding non-matching
        /// entries. Used by both the standard 3-flag MoveNext and the typed MoveNext overload.
        /// </summary>
        /// <returns></returns>
        private KeyValuePair<PrimitiveBlock, object> DeQueueMatchingPrimitive(
            bool ignoreNodes, bool ignoreWays, bool ignoreRelations,
            OsmGeoType? onlyType, long? minId)
        {
            while (_cachedPrimitives.Count > 0)
            {
                var head = _cachedPrimitives.Dequeue();
                if (head.Value is IO.PBF.Node n)
                {
                    if (ignoreNodes) continue;
                    if (onlyType.HasValue && onlyType.Value != OsmGeoType.Node) continue;
                    if (minId.HasValue && n.id < minId.Value) continue;
                    return head;
                }
                if (head.Value is IO.PBF.Way w)
                {
                    if (ignoreWays) continue;
                    if (onlyType.HasValue && onlyType.Value != OsmGeoType.Way) continue;
                    if (minId.HasValue && w.id < minId.Value) continue;
                    return head;
                }
                if (head.Value is IO.PBF.Relation r)
                {
                    if (ignoreRelations) continue;
                    if (onlyType.HasValue && onlyType.Value != OsmGeoType.Relation) continue;
                    if (minId.HasValue && r.id < minId.Value) continue;
                    return head;
                }
            }
            return new KeyValuePair<PrimitiveBlock, object>();
        }

        #endregion

        #endregion

        /// <summary>
        /// Processes a node.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="node"></param>
        void IPBFOsmPrimitiveConsumer.ProcessNode(PrimitiveBlock block, OsmSharp.IO.PBF.Node node)
        {
            _activeRecorder?.TrackNode(node.id);
            this.QueuePrimitive(block, node);
        }

        /// <summary>
        /// Processes a way.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="way"></param>
        void IPBFOsmPrimitiveConsumer.ProcessWay(PrimitiveBlock block, OsmSharp.IO.PBF.Way way)
        {
            _activeRecorder?.TrackWay(way.id);
            this.QueuePrimitive(block, way);
        }

        /// <summary>
        /// Processes a relation.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="relation"></param>
        void IPBFOsmPrimitiveConsumer.ProcessRelation(PrimitiveBlock block, OsmSharp.IO.PBF.Relation relation)
        {
            _activeRecorder?.TrackRelation(relation.id);
            this.QueuePrimitive(block, relation);
        }

        // Internal block index: sorted list of entries keyed by FileOffset. Warm-path skip
        // consults this to avoid decompressing blobs whose contents we know we don't want.
        // Deliberately internal: exposing raw offsets to callers would let a wrong value
        // silently misread the file, and the win from cross-process caching isn't worth that
        // risk. Cross-process caching, if ever needed, should ship as a verified sidecar file.
        private sealed class PBFBlockIndex
        {
            private readonly List<PBFBlockEntry> _entries = new List<PBFBlockEntry>();

            public bool TryGetAt(long offset, out PBFBlockEntry entry)
            {
                var i = BinarySearchByOffset(offset);
                if (i < 0) { entry = default; return false; }
                entry = _entries[i];
                return true;
            }

            public void Upsert(PBFBlockEntry entry)
            {
                var i = BinarySearchByOffset(entry.FileOffset);
                if (i >= 0)
                {
                    // Merge: preserve any Tier-2 info that a previous decode learned but this one didn't.
                    var existing = _entries[i];
                    if (!entry.NodeIdsKnown && existing.NodeIdsKnown)
                    {
                        entry.NodeIdsKnown = true;
                        entry.NodeMinId = existing.NodeMinId;
                        entry.NodeMaxId = existing.NodeMaxId;
                    }
                    if (!entry.WayIdsKnown && existing.WayIdsKnown)
                    {
                        entry.WayIdsKnown = true;
                        entry.WayMinId = existing.WayMinId;
                        entry.WayMaxId = existing.WayMaxId;
                    }
                    if (!entry.RelationIdsKnown && existing.RelationIdsKnown)
                    {
                        entry.RelationIdsKnown = true;
                        entry.RelationMinId = existing.RelationMinId;
                        entry.RelationMaxId = existing.RelationMaxId;
                    }
                    _entries[i] = entry;
                }
                else
                {
                    _entries.Insert(~i, entry);
                }
            }

            private int BinarySearchByOffset(long offset)
            {
                var lo = 0;
                var hi = _entries.Count - 1;
                while (lo <= hi)
                {
                    var mid = lo + ((hi - lo) >> 1);
                    var midOffset = _entries[mid].FileOffset;
                    if (midOffset == offset) return mid;
                    if (midOffset < offset) lo = mid + 1;
                    else hi = mid - 1;
                }
                return ~lo;
            }
        }

        private struct PBFBlockEntry
        {
            public long FileOffset;
            public long EndOffset;

            public bool HasNodes;
            public bool HasWays;
            public bool HasRelations;

            public bool NodeIdsKnown;
            public long NodeMinId;
            public long NodeMaxId;

            public bool WayIdsKnown;
            public long WayMinId;
            public long WayMaxId;

            public bool RelationIdsKnown;
            public long RelationMinId;
            public long RelationMaxId;
        }

        private sealed class BlockRecorder
        {
            public bool HasNodeIds;
            public long NodeMinId;
            public long NodeMaxId;

            public bool HasWayIds;
            public long WayMinId;
            public long WayMaxId;

            public bool HasRelationIds;
            public long RelationMinId;
            public long RelationMaxId;

            public void TrackNode(long id)
            {
                if (!HasNodeIds) { NodeMinId = id; NodeMaxId = id; HasNodeIds = true; return; }
                if (id < NodeMinId) NodeMinId = id;
                if (id > NodeMaxId) NodeMaxId = id;
            }

            public void TrackWay(long id)
            {
                if (!HasWayIds) { WayMinId = id; WayMaxId = id; HasWayIds = true; return; }
                if (id < WayMinId) WayMinId = id;
                if (id > WayMaxId) WayMaxId = id;
            }

            public void TrackRelation(long id)
            {
                if (!HasRelationIds) { RelationMinId = id; RelationMaxId = id; HasRelationIds = true; return; }
                if (id < RelationMinId) RelationMinId = id;
                if (id > RelationMaxId) RelationMaxId = id;
            }
        }
    }
}
