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

using System.Collections.Generic;

namespace OsmSharp.Streams
{
    /// <summary>
    /// An OSM Stream Reader that wraps around a collection of OSM objects.
    /// </summary>
    public class OsmEnumerableStreamSource : OsmStreamSource
    {
        private readonly IEnumerable<OsmGeo> _baseObjects; // Holds the list of SimpleOsmGeo objects.

        /// <summary>
        /// Creates a new OsmBase source.
        /// </summary>
        public OsmEnumerableStreamSource(IEnumerable<OsmGeo> baseObjects)
        {
            _baseObjects = baseObjects;
        }

        private IEnumerator<OsmGeo> _baseObjectEnumerator; // Holds the current enumerator.
        
        /// <summary>
        /// Move to the next item in the stream.
        /// </summary>
        /// 
        public override bool MoveNext(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
        {
            if (_baseObjectEnumerator == null)
            { // create the enumerator.
                _baseObjectEnumerator = _baseObjects.GetEnumerator();
            }

            // move next.
            do
            {
                if (!_baseObjectEnumerator.MoveNext())
                { // the move failed!
                    _baseObjectEnumerator = null;
                    return false;
                }
            } while ((ignoreNodes && _baseObjectEnumerator.Current.Type == OsmGeoType.Node) ||
                (ignoreWays && _baseObjectEnumerator.Current.Type == OsmGeoType.Way) ||
                (ignoreRelations && _baseObjectEnumerator.Current.Type == OsmGeoType.Relation));
            return true;
        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        public override OsmGeo Current()
        {
            return _baseObjectEnumerator.Current;
        }

        /// <summary>
        /// Resets this data source.
        /// </summary>
        public override void Reset()
        {
            _baseObjectEnumerator = null;
        }

        /// <summary>
        /// Returns true, this source can be reset.
        /// </summary>
        public override bool CanReset
        {
            get { return true; }
        }
    }
}