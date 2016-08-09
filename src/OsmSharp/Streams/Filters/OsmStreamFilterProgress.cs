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

using OsmSharp.Logging;
using System;

namespace OsmSharp.Streams.Filters
{
    /// <summary>
    /// A stream filter that reports progress.
    /// </summary>
    public class OsmStreamFilterProgress : OsmStreamFilter
    {
        private OsmGeoType? _lastType = OsmGeoType.Node;
        private long _lastTypeStart;
        private long _nodeInterval = 100000;
        private long _wayInterval = 10000;
        private long _relationInterval = 1000;

        private int _pass;
        private long _node;
        private long _nodeTicks;
        private long _way;
        private long _wayTicks;
        private long _relation;
        private long _relationTicks;

        /// <summary>
        /// Creates a new progress reporting source.
        /// </summary>
        public OsmStreamFilterProgress()
        {
            _pass = 1;
        }

        /// <summary>
        /// Creates a new progress reporting source.
        /// </summary>
        public OsmStreamFilterProgress(long nodesInterval, long waysInterval,
            long relationInterval)
        {
            _nodeInterval = nodesInterval;
            _wayInterval = waysInterval;
            _relationInterval = relationInterval;
        }

        private bool _initialized = false;

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

            return this.Source.MoveNext(ignoreNodes, ignoreWays, ignoreRelations);
        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        /// <returns></returns>
        public override OsmGeo Current()
        {
            var current = this.Source.Current();

            // keep the start ticks.
            long ticksStart = DateTime.Now.Ticks;

            if (!_lastType.HasValue)
            { // has a last type.
                _lastTypeStart = DateTime.Now.Ticks;
                _lastType = current.Type;
            }

            if (_lastType != current.Type)
            { // the last type has changed.
                long lastTicks = ticksStart - _lastTypeStart;
                switch (_lastType)
                {
                    case OsmGeoType.Node:
                        _nodeTicks = _nodeTicks + lastTicks;
                        break;
                    case OsmGeoType.Way:
                        _wayTicks = _wayTicks + lastTicks;
                        break;
                    case OsmGeoType.Relation:
                        _relationTicks = _relationTicks + lastTicks;
                        break;
                }
                // start new ticks.
                _lastTypeStart = DateTime.Now.Ticks;
                _lastType = current.Type;
            }

            switch (current.Type)
            {
                case OsmGeoType.Node:
                    _node++;

                    if ((_node % _nodeInterval) == 0)
                    {
                        var nodeSpan = new TimeSpan(_nodeTicks + (ticksStart - _lastTypeStart));
                        var nodePerSecond = System.Math.Round((double)_node / nodeSpan.TotalSeconds, 0);
                        Logging.Logger.Log("StreamProgress", TraceEventType.Information,
                            "Pass {2} - Node[{0}] @ {1}/s", _node, nodePerSecond, _pass);
                    }
                    break;
                case OsmGeoType.Relation:
                    _relation++;

                    if ((_relation % _relationInterval) == 0)
                    {
                        var relationSpan = new TimeSpan(_relationTicks + (ticksStart - _lastTypeStart));
                        var relationPerSecond = System.Math.Round((double)_relation / relationSpan.TotalSeconds, 2);
                        Logging.Logger.Log("StreamProgress", TraceEventType.Information,
                            "Pass {2} - Relation[{0}] @ {1}/s", _relation, relationPerSecond, _pass);
                    }
                    break;
                case OsmGeoType.Way:
                    _way++;

                    if ((_way % _wayInterval) == 0)
                    {
                        var waySpan = new TimeSpan(_wayTicks + (ticksStart - _lastTypeStart));
                        var wayPerSecond = System.Math.Round((double)_way / waySpan.TotalSeconds, 2);
                        Logging.Logger.Log("StreamProgress", TraceEventType.Information,
                            "Pass {2} - Way[{0}] @ {1}/s", _way, wayPerSecond, _pass);
                    }
                    break;
            }

            return current;
        }

        /// <summary>
        /// Resets this source.
        /// </summary>
        public override void Reset()
        {
            _lastTypeStart = 0;
            _lastType = null;

            _pass++;
            _node = 0;
            _nodeTicks = 0;
            _way = 0;
            _wayTicks = 0;
            _relation = 0;
            _relationTicks = 0;

            this.Source.Reset();
        }
        
        /// <summary>
        /// Initializes this source.
        /// </summary>
        private void Initialize()
        {
            _lastTypeStart = 0;
            _lastType = null;

            _pass = 1;
            _node = 0;
            _nodeTicks = 0;
            _way = 0;
            _wayTicks = 0;
            _relation = 0;
            _relationTicks = 0;
        }

        /// <summary>
        /// Returns true if this source can be reset.
        /// </summary>
        public override bool CanReset
        {
            get { return this.Source.CanReset; }
        }
    }
}