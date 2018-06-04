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
using System.Collections;
using System.Collections.Generic;

namespace OsmSharp.Streams
{
    /// <summary>
    /// An enumerable that can ignore nodes, ways or relations.
    /// </summary>
    public class OsmStreamSourceEnumerable : IEnumerable<OsmGeo>, IEnumerator<OsmGeo>
    {
        private readonly OsmStreamSource _source;
        private readonly bool _ignoreNodes;
        private readonly bool _ignoreWays;
        private readonly bool _ignoreRelations;

        /// <summary>
        /// Creates a new enumerable.
        /// </summary>
        public OsmStreamSourceEnumerable(OsmStreamSource source, bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
        {
            _source = source;
            _ignoreNodes = ignoreNodes;
            _ignoreWays = ignoreWays;
            _ignoreRelations = ignoreRelations;
        }

        /// <summary>
        /// Gets the current object.
        /// </summary>
        public OsmGeo Current
        {
            get
            {
                return _source.Current();
            }
        }

        /// <summary>
        /// Gets the current object.
        /// </summary>
        object IEnumerator.Current
        {
            get
            {
                return _source.Current();
            }
        }

        /// <summary>
        /// Moves to the next object.
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            return _source.MoveNext(_ignoreNodes, _ignoreWays, _ignoreRelations);
        }

        /// <summary>
        /// Resets this enumerator.
        /// </summary>
        public void Reset()
        {
            if (!_source.CanReset)
            {
                throw new Exception("The source for this enumerator cannot be reset. You can only loop over these objects once or recreate the source.");
            }
            _source.Reset();
        }

        /// <summary>
        /// Disposes this enumerator.
        /// </summary>
        public void Dispose()
        {

        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<OsmGeo> GetEnumerator()
        {
            return this;
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }
    }
}