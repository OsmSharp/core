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

namespace OsmSharp.Collections.Arrays
{
    /// <summary>
    /// An array working around the pre .NET 4.5 memory limitations for one object.
    /// </summary>
    public class HugeArray<T> : HugeArrayBase<T>
    {
        private T[][] blocks;
        private readonly int _blockSize = (int)System.Math.Pow(2, 20); // Holds the maximum array size, always needs to be a power of 2.
        private int _arrayPow = 20;
        private long _size; // the total size of this array.

        /// <summary>
        /// Creates a new huge array.
        /// </summary>
        public HugeArray(long size)
            : this(size, (int)System.Math.Pow(2, 20))
        {

        }

        /// <summary>
        /// Creates a new huge array.
        /// </summary>
        public HugeArray(long size, int blockSize)
        {
            if (size < 0) { throw new ArgumentOutOfRangeException("Size needs to be bigger than or equal to zero."); }
            if (blockSize < 0) { throw new ArgumentOutOfRangeException("Blocksize needs to be bigger than or equal to zero."); }
            if ((blockSize & (blockSize - 1)) != 0) { throw new ArgumentOutOfRangeException("Blocksize needs to be a power of 2."); }

            _blockSize = blockSize;
            _size = size;

            var blockCount = (long)System.Math.Ceiling((double)size / _blockSize);
            blocks = new T[blockCount][];
            for (var i = 0; i < blockCount - 1; i++)
            {
                blocks[i] = new T[_blockSize];
            }
            if (blockCount > 0)
            {
                blocks[blockCount - 1] = new T[size - ((blockCount - 1) * _blockSize)];
            }
        }

        /// <summary>
        /// Gets or sets the element at the given idx.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public override T this[long idx]
        {
            get
            {
                var block = idx >> _arrayPow;
                var localIdx = idx - (block << _arrayPow);
                return blocks[block][localIdx];
            }
            set
            {
                long block = (long)System.Math.Floor(idx / _blockSize);
                long localIdx = idx % _blockSize;
                blocks[block][localIdx] = value;
            }
        }

        /// <summary>
        /// Resizes this array.
        /// </summary>
        /// <param name="size"></param>
        public override void Resize(long size)
        {
            if (size < 0) { throw new ArgumentOutOfRangeException("Cannot resize a huge array to a size of zero or smaller."); }

            _size = size;

            var blockCount = (long)System.Math.Ceiling((double)size / _blockSize);
            if (blockCount != blocks.Length)
            {
                Array.Resize<T[]>(ref blocks, (int)blockCount);
            }
            for (int i = 0; i < blockCount - 1; i++)
            {
                if (blocks[i] == null)
                { // there is no array, create it.
                    blocks[i] = new T[_blockSize];
                }
                if (blocks[i].Length != _blockSize)
                { // the size is the same, keep it as it.
                    var localArray = blocks[i];
                    Array.Resize<T>(ref localArray, (int)_blockSize);
                    blocks[i] = localArray;
                }
            }
            if (blockCount > 0)
            {
                var lastBlockSize = size - ((blockCount - 1) * _blockSize);
                if (blocks[blockCount - 1] == null)
                { // there is no array, create it.
                    blocks[blockCount - 1] = new T[lastBlockSize];
                }
                if (blocks[blockCount - 1].Length != lastBlockSize)
                { // the size is the same, keep it as it.
                    var localArray = blocks[blockCount - 1];
                    Array.Resize<T>(ref localArray, (int)lastBlockSize);
                    blocks[blockCount - 1] = localArray;
                }
            }
        }

        /// <summary>
        /// Returns the length of this array.
        /// </summary>
        public override long Length
        {
            get
            {
                return _size;
            }
        }

        /// <summary>
        /// Diposes of all associated native resources held by this object.
        /// </summary>
        public override void Dispose()
        {

        }
    }
}