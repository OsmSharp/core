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

namespace OsmSharp.Streams.Collections
{
    /// <summary>
    /// Represents a sparce large bit array.
    /// </summary>
    public class SparseLargeBitArray32
    {
        private readonly int _blockSize;
        private readonly long _length;
        private readonly LargeBitArray32[] _data;

        /// <summary>
        /// Creates a new sparse bitvector 32 array.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="blockSize"></param>
        public SparseLargeBitArray32(long size, int blockSize)
        {
            if (size % 32 != 0) { throw new ArgumentOutOfRangeException("Size has to be divisible by 32."); }
            if (size % blockSize != 0) { throw new ArgumentOutOfRangeException("Size has to be divisible by blocksize."); }

            _length = size;
            _blockSize = blockSize;
            _data = new LargeBitArray32[_length / _blockSize];
        }

        /// <summary>
        /// Gets or sets the value at the given index.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public bool this[long idx]
        {
            get
            {
                int blockId = (int)(idx / _blockSize);
                var block = _data[blockId];
                if (block != null)
                { // the block actually exists.
                    int blockIdx = (int)(idx % _blockSize);
                    return _data[blockId][blockIdx];
                }
                return false;
            }
            set
            {
                int blockId = (int)(idx / _blockSize);
                var block = _data[blockId];
                if (block == null)
                { // block is not there.
                    if (value)
                    { // only add new block if true.
                        block = new LargeBitArray32(_blockSize);
                        int blockIdx = (int)(idx % _blockSize);
                        block[blockIdx] = true;
                        _data[blockId] = block;
                    }
                }
                else
                { // set value at block.
                    int blockIdx = (int)(idx % _blockSize);
                    block[blockIdx] = value;
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