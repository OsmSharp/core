// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Collections
{
    /// <summary>
    /// A sparse array; an array that still handles 'holes' between elements.
    /// </summary>
    /// <remarks>Stores it's data in blocks of smaller arrays. Assumes some localitiy in the data, otherwise, use a regular array.</remarks>
    /// <typeparam name="T"></typeparam>
    public class SparseArray<T> : IEnumerable<T>
    {
        /// <summary>
        /// Holds the block size.
        /// </summary>
        private readonly int _blockSize;

        /// <summary>
        /// Holds the virtual size.
        /// </summary>
        private long _virtualSize;

        /// <summary>
        /// Holds the array blocks.
        /// </summary>
        private readonly Dictionary<long, ArrayBlock> _arrayBlocks;

        /// <summary>
        /// Creates a new sparse array.
        /// </summary>
        /// <param name="size">The initial size.</param>
        public SparseArray(long size)
        {
            _virtualSize = size;
            _blockSize = 256;

            _arrayBlocks = new Dictionary<long, ArrayBlock>();
            _lastAccessedBlock = null;
        }

        /// <summary>
        /// Holds the last accessed block to exploit locality of access.
        /// </summary>
        private ArrayBlock _lastAccessedBlock; 

        /// <summary>
        /// Gets/sets a value.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[long index]
        {
            get
            {
                if (index >= _virtualSize)
                { // index out of range!
                    throw new IndexOutOfRangeException();
                }
                if (_lastAccessedBlock != null &&
                    _lastAccessedBlock.Index <= index && (_lastAccessedBlock.Index + _blockSize) > index)
                { // the last accessed block is contains the requested value.
                    return _lastAccessedBlock.Data[index - _lastAccessedBlock.Index];
                }
                // calculate block index.
                long blockIndex = index / _blockSize;
                
                // get block.
                ArrayBlock block;
                if (_arrayBlocks.TryGetValue(blockIndex, out block))
                { // return the value from this block.
                    _lastAccessedBlock = block; // set last accessed block.
                    return _lastAccessedBlock.Data[index - _lastAccessedBlock.Index]; 
                }
                return default(T); // no block was found!
            }
            set
            {
                if (index >= _virtualSize)
                { // index out of range!
                    throw new IndexOutOfRangeException();
                }
                if (_lastAccessedBlock != null &&
                    _lastAccessedBlock.Index <= index && (_lastAccessedBlock.Index + _blockSize) > index)
                { // the last accessed block is contains the requested value.
                    _lastAccessedBlock.Data[index - _lastAccessedBlock.Index] = value;
                    return;
                }
                // calculate block index.
                long blockIndex = index / _blockSize;

                // get block.
                ArrayBlock block;
                if (!_arrayBlocks.TryGetValue(blockIndex, out block) &&
                    value != null)
                { // return the value from this block.
                    block = new ArrayBlock(blockIndex*_blockSize, _blockSize);
                    _arrayBlocks.Add(blockIndex, block);
                }
                if (block != null)
                { // the block exists.
                    _lastAccessedBlock = block; // set last accessed block.
                    _lastAccessedBlock.Data[index - _lastAccessedBlock.Index] = value;
                }
            }
        }

        /// <summary>
        /// Resizes this array.
        /// </summary>
        /// <param name="size">The new size.</param>
        public void Resize(long size)
        {
            if (size >= _virtualSize)
            { // increasing the size is easy!
                _virtualSize = size;
            }
            else
            { // decreasing the size harder.
                _virtualSize = size;

                // remove unneeded blocks.
                var unneededBlocks =
                    new List<KeyValuePair<long, ArrayBlock>>();
                foreach (var block in _arrayBlocks)
                {
                    if (block.Value.Index > _virtualSize)
                    {
                        unneededBlocks.Add(block);
                    }
                }
                foreach (var unneededBlock in unneededBlocks)
                {
                    _arrayBlocks.Remove(unneededBlock.Key);
                }
            }
        }

        /// <summary>
        /// Gets the length of this array.
        /// </summary>
        public long Length { get { return _virtualSize; }}

        /// <summary>
        /// Represents an array block.
        /// </summary>
        private class ArrayBlock
        {
            /// <summary>
            /// Creates a new array block.
            /// </summary>
            /// <param name="index"></param>
            /// <param name="size"></param>
            public ArrayBlock(long index, int size)
            {
                this.Index = index;
                this.Data = new T[size];
            }

            /// <summary>
            /// The starting index of this block.
            /// </summary>
            public long Index { get; private set; }

            /// <summary>
            /// The actual data.
            /// </summary>
            public T[] Data { get; private set; }
        }

        #region IEnumerable Implementation

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new SparseArrayEnumerator(this);
        }

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new SparseArrayEnumerator(this);
        }

        /// <summary>
        /// Enumerator class for this sparse array.
        /// </summary>
        private class SparseArrayEnumerator : IEnumerator, IEnumerator<T>
        {
            /// <summary>
            /// Holds the current index.
            /// </summary>
            private int _current = -1;

            /// <summary>
            /// Holds the array.
            /// </summary>
            private SparseArray<T> _array;

            /// <summary>
            /// Creates a new enumerator.
            /// </summary>
            /// <param name="array"></param>
            public SparseArrayEnumerator(SparseArray<T> array)
            {
                _array = array;
            }

            /// <summary>
            /// Returns the current object.
            /// </summary>
            public object Current
            {
                get { return _array[_current]; }
            }

            /// <summary>
            /// Move to the next object.
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                _current++;
                return _current < _array.Length;
            }

            public void Reset()
            {
                _current = 0;
            }

            T IEnumerator<T>.Current
            {
                get { return _array[_current]; }
            }

            public void Dispose()
            {

            }
        }

        #endregion
    }
}
