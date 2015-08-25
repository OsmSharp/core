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
        /// <summary>
        /// Holds the arrays.
        /// </summary>
        private T[][] _arrays;

        /// <summary>
        /// Holds the maximum array size.
        /// </summary>
        private int _arraySize = (int)System.Math.Pow(2, 20);

        /// <summary>
        /// Holds the array size power of 2.
        /// </summary>
        private int _arrayPow = 20;

        /// <summary>
        /// Holds the size of this array.
        /// </summary>
        private long _size;

        /// <summary>
        /// Creates a new huge array.
        /// </summary>
        /// <param name="size"></param>
        public HugeArray(long size)
        {
            _arraySize = (int)System.Math.Pow(2, 20);
            _size = size;

            long arrayCount = (long)System.Math.Ceiling((double)size / _arraySize);
            _arrays = new T[arrayCount][];
            for (int arrayIdx = 0; arrayIdx < arrayCount - 1; arrayIdx++)
            {
                _arrays[arrayIdx] = new T[_arraySize];
            }
            _arrays[arrayCount - 1] = new T[size - ((arrayCount - 1) * _arraySize)];
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
                long arrayIdx = idx >> _arrayPow;
                long localIdx = idx - (arrayIdx << _arrayPow);
                return _arrays[arrayIdx][localIdx];
            }
            set
            {
                long arrayIdx = (long)System.Math.Floor(idx / _arraySize);
                long localIdx = idx % _arraySize;
                _arrays[arrayIdx][localIdx] = value;
            }
        }

        /// <summary>
        /// Resizes this array.
        /// </summary>
        /// <param name="size"></param>
        public override void Resize(long size)
        {
            if (size <= 0) { throw new ArgumentOutOfRangeException("Cannot resize a huge array to a size of zero or smaller."); }

            _size = size;

            long arrayCount = (long)System.Math.Ceiling((double)size / _arraySize);
            if (arrayCount != _arrays.Length)
            {
                Array.Resize<T[]>(ref _arrays, (int)arrayCount);
            }
            for (int arrayIdx = 0; arrayIdx < arrayCount - 1; arrayIdx++)
            {
                if (_arrays[arrayIdx] == null)
                { // there is no array, create it.
                    _arrays[arrayIdx] = new T[_arraySize];
                }
                if (_arrays[arrayIdx].Length != _arraySize)
                { // the size is the same, keep it as it.
                    var localArray = _arrays[arrayIdx];
                    Array.Resize<T>(ref localArray, (int)_arraySize);
                    _arrays[arrayIdx] = localArray;
                }
            }
            var lastArraySize = size - ((arrayCount - 1) * _arraySize);
            if (_arrays[arrayCount - 1] == null)
            { // there is no array, create it.
                _arrays[arrayCount - 1] = new T[lastArraySize];
            }
            if (_arrays[arrayCount - 1].Length != lastArraySize)
            { // the size is the same, keep it as it.
                var localArray = _arrays[arrayCount - 1];
                Array.Resize<T>(ref localArray, (int)lastArraySize);
                _arrays[arrayCount - 1] = localArray;
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