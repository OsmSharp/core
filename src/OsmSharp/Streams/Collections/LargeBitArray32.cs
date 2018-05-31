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
    /// Represents a large bit array.
    /// </summary>
    public class LargeBitArray32
    {
        private readonly uint[] _array;
        private readonly long _length;

        /// <summary>
        /// Creates a new bitvector array.
        /// </summary>
        public LargeBitArray32(long size)
        {
            _length = size;
            _array = new uint[(int)System.Math.Ceiling((double)size / 32)];
        }

        /// <summary>
        /// Returns the element at the given index.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public bool this[long idx]
        {
            get
            {
                int arrayIdx = (int)(idx >> 5);
                int bitIdx = (int)(idx % 32);
                long mask = (long)1 << bitIdx;
                return (_array[arrayIdx] & mask) != 0;
            }
            set
            {
                int arrayIdx = (int)(idx >> 5);
                int bitIdx = (int)(idx % 32);
                long mask = (long)1 << bitIdx;
                if (value)
                { // set value.
                    _array[arrayIdx] = (uint)(mask | _array[arrayIdx]);
                }
                else
                { // unset value.
                    _array[arrayIdx] = (uint)((~mask) & _array[arrayIdx]);
                }
            }
        }

        /// <summary>
        /// Returns the length of this array.
        /// </summary>
        public long Length
        {
            get
            {
                return _length;
            }
        }
    }
}