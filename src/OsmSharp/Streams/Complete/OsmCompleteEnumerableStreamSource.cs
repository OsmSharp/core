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

using OsmSharp.Complete;
using System.Collections.Generic;

namespace OsmSharp.Streams.Complete
{
    /// <summary>
    /// An osm complete enumerable stream source.
    /// </summary>
    public class OsmCompleteEnumerableStreamSource : OsmCompleteStreamSource
    {
        private readonly IEnumerable<ICompleteOsmGeo> _enumerable;

        /// <summary>
        /// Creates a new osm complete source based on the given enumerable.
        /// </summary>
        public OsmCompleteEnumerableStreamSource(IEnumerable<ICompleteOsmGeo> enumerable)
        {
            _enumerable = enumerable;
        }

        private IEnumerator<ICompleteOsmGeo> _enumerator;

        /// <summary>
        /// Returns true if this source can be reset.
        /// </summary>
        public override bool CanReset
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        public override ICompleteOsmGeo Current()
        {
            return _enumerator.Current;
        }

        /// <summary>
        /// Initializes this source.
        /// </summary>
        public override void Initialize()
        {
            _enumerator = _enumerable.GetEnumerator();
        }

        /// <summary>
        /// Moves to the next object.
        /// </summary>
        public override bool MoveNext()
        {
            return _enumerator.MoveNext();
        }

        /// <summary>
        /// Resets this source.
        /// </summary>
        public override void Reset()
        {
            _enumerator = _enumerable.GetEnumerator();
        }
    }
}