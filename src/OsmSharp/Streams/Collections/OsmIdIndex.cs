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

namespace OsmSharp.Streams.Collections
{
    /// <summary>
    /// An efficient index for OSM object ids.
    /// </summary>
    public class OsmIdIndex
    {
        private readonly long _size = (long)(1024 * 1024) * (long)(1024 * 32);
        private readonly int _blockSize = 1024 * 1024;

        /// <summary>
        /// Creates a new longindex.
        /// </summary>
        public OsmIdIndex()
        {

        }
        
        private long _count = 0;
        private SparseLargeBitArray32 _positiveFlags = null;
        private SparseLargeBitArray32 _negativeFlags = null;

        /// <summary>
        /// Adds an id.
        /// </summary>
        public void Add(long number)
        {
            if (number >= 0)
            {
                this.PositiveAdd(number);
            }
            else
            {
                this.NegativeAdd(-number);
            }
        }

        /// <summary>
        /// Removes an id.
        /// </summary>
        public void Remove(long number)
        {
            if (number >= 0)
            {
                this.PositiveRemove(number);
            }
            else
            {
                this.NegativeAdd(-number);
            }
        }

        /// <summary>
        /// Returns true if the id is there.
        /// </summary>
        public bool Contains(long number)
        {
            if (number >= 0)
            {
                return this.PositiveContains(number);
            }
            else
            {
                return this.NegativeContains(-number);
            }
        }

        #region Positive

        /// <summary>
        /// Adds an id.
        /// </summary>
        private void PositiveAdd(long number)
        {
            if (_positiveFlags == null)
            {
                _positiveFlags = new SparseLargeBitArray32(_size, _blockSize);
            }

            if (!_positiveFlags[number])
            { // there is a new positive flag.
                _count++;
            }
            _positiveFlags[number] = true;
        }

        /// <summary>
        /// Removes an id.
        /// </summary>
        private void PositiveRemove(long number)
        {
            if (_positiveFlags == null)
            {
                _positiveFlags = new SparseLargeBitArray32(_size, _blockSize);
            }

            if (_positiveFlags[number])
            { // there is one less positive flag.
                _count--;
            }
            _positiveFlags[number] = false;
        }

        /// <summary>
        /// Returns true if the id is there.
        /// </summary>
        private bool PositiveContains(long number)
        {
            if (_positiveFlags == null)
            {
                return false;
            }

            return _positiveFlags[number];
        }

        #endregion

        #region Negative

        /// <summary>
        /// Adds an id.
        /// </summary>
        private void NegativeAdd(long number)
        {
            if (_negativeFlags == null)
            {
                _negativeFlags = new SparseLargeBitArray32(_size, _blockSize);
            }

            if (!_negativeFlags[number])
            { // there is one more negative flag.
                _count++;
            }
            _negativeFlags[number] = true;
        }

        /// <summary>
        /// Removes an id.
        /// </summary>
        private void NegativeRemove(long number)
        {
            if (_negativeFlags == null)
            {
                _negativeFlags = new SparseLargeBitArray32(_size, _blockSize);
            }

            if (_negativeFlags[number])
            { // there is one less negative flag.
                _count--;
            }
            _negativeFlags[number] = false;
        }

        /// <summary>
        /// Returns true if the id is there.
        /// </summary>
        private bool NegativeContains(long number)
        {
            if (_negativeFlags == null)
            {
                return false;
            }

            return _negativeFlags[number];
        }

        #endregion

        /// <summary>
        /// Returns the number of positive flags.
        /// </summary>
        public long Count
        {
            get
            {
                return _count;
            }
        }

        /// <summary>
        /// Clears this index.
        /// </summary>
        public void Clear()
        {
            _negativeFlags = null;
            _positiveFlags = null;
        }
    }
}