// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using System;

namespace OsmSharp.Collections
{
    /// <summary>
    /// Represents a sparce large bit array.
    /// </summary>
    public class SparseLargeBitArray32
    {
        /// <summary>
        /// Holds the blocksize, or the size of the 'sub arrays'.
        /// </summary>
        private int _blockSize;

        /// <summary>
        /// Holds the length of this array.
        /// </summary>
        private long _length;

        /// <summary>
        /// Holds the actual data blocks.
        /// </summary>
        private LargeBitArray32[] _data;

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