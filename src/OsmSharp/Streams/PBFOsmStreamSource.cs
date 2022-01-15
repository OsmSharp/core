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

            var nextPBFPrimitive = this.MoveToNextPrimitive(ignoreNodes, ignoreWays, ignoreRelations);
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
                        nextPBFPrimitive = this.MoveToNextPrimitive(ignoreNodes, ignoreWays, ignoreRelations);
                        break;
                }
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
        /// Resetting this data source 
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
        private KeyValuePair<PrimitiveBlock, object> MoveToNextPrimitive(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
        {
            var next = this.DeQueuePrimitive();
            if (next.Value == null)
            { // decode another block.
                // move to first way/relation position if they are known and nodes and or ways are to be skipped.
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
                var block = _reader.MoveNext();
                bool hasWays = false, hasRelations = false;
                while (block != null && !block.Decode(this, ignoreNodes, ignoreWays, ignoreRelations,
                    out _, out hasWays, out hasRelations))
                {
                    if (hasWays && _firstWayPosition == -1)
                    {
                        _firstWayPosition = beforeBlockPosition;
                    }
                    if (hasRelations && _firstRelationPosition == -1)
                    {
                        _firstRelationPosition = beforeBlockPosition;
                    }
                    if (_stream.CanSeek) beforeBlockPosition = _stream.Position;
                    block = _reader.MoveNext();
                }
                if (hasWays && _firstWayPosition == -1)
                {
                    _firstWayPosition = beforeBlockPosition;
                }
                if (hasRelations && _firstRelationPosition == -1)
                {
                    _firstRelationPosition = beforeBlockPosition;
                }
                next = this.DeQueuePrimitive();
            }
            return next;
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
        /// DeQueues a primitive.
        /// </summary>
        /// <returns></returns>
        private KeyValuePair<PrimitiveBlock, object> DeQueuePrimitive()
        {
            if (_cachedPrimitives.Count > 0)
            {
                return _cachedPrimitives.Dequeue();
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
            this.QueuePrimitive(block, node);
        }

        /// <summary>
        /// Processes a way.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="way"></param>
        void IPBFOsmPrimitiveConsumer.ProcessWay(PrimitiveBlock block, OsmSharp.IO.PBF.Way way)
        {
            this.QueuePrimitive(block, way);
        }

        /// <summary>
        /// Processes a relation.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="relation"></param>
        void IPBFOsmPrimitiveConsumer.ProcessRelation(PrimitiveBlock block, OsmSharp.IO.PBF.Relation relation)
        {
            this.QueuePrimitive(block, relation);
        }
    }
}