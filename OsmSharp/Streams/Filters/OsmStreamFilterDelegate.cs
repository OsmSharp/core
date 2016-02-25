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

namespace OsmSharp.Streams.Filters
{

    /// <summary>
    /// A filter that use a function to filter objects.
    /// </summary>
    public class OsmStreamFilterDelegate : OsmStreamFilter
    {
        private readonly object _param; // Holds the parameters object sent with the events.

        /// <summary>
        /// Creates a new filter with events.
        /// </summary>
        public OsmStreamFilterDelegate()
        {
            _param = null;
        }

        /// <summary>
        /// Creates a new filter with events.
        /// </summary>
        public OsmStreamFilterDelegate(object param)
        {
            _param = param;
        }

        private OsmGeo _current = null;
        
        /// <summary>
        /// Called when the move is made to the next object.
        /// </summary>
        public Func<OsmGeo, object, OsmGeo> MoveToNextEvent;

        /// <summary>
        /// Move to the next item in the stream.
        /// </summary>
        public override bool MoveNext(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
        {
            while (this.DoMoveNext())
            {
                if (this.Current().Type == OsmGeoType.Node &&
                    !ignoreNodes)
                { // there is a node and it is not to be ignored.
                    return true;
                }
                else if (this.Current().Type == OsmGeoType.Way &&
                        !ignoreWays)
                { // there is a way and it is not to be ignored.
                    return true;
                }
                else if (this.Current().Type == OsmGeoType.Relation &&
                        !ignoreRelations)
                { // there is a relation and it is not to be ignored.
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Moves this filter to the next object.
        /// </summary>
        private bool DoMoveNext()
        {
            while (this.Source.MoveNext())
            {
                _current = this.Source.Current();
                if (this.MoveToNextEvent != null)
                {
                    _current = this.MoveToNextEvent(_current, _param);
                    if (_current != null)
                    { // when null is return the object is to be ignored.
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        /// <returns></returns>
        public override OsmGeo Current()
        {
            return _current;
        }

        /// <summary>
        /// Resets this filter.
        /// </summary>
        public override void Reset()
        {
            _current = null;
            this.Source.Reset();
        }

        /// <summary>
        /// Returns true if this filter can be reset.
        /// </summary>
        public override bool CanReset
        {
            get { return this.Source.CanReset; }
        }
    }
}
