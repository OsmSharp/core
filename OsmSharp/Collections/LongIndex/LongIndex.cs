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

namespace OsmSharp.Collections.LongIndex.LongIndex
{
    /// <summary>
    /// An efficient index for OSM object ids.
    /// </summary>
    public class LongIndex : ILongIndex
    {
        /// <summary>
        /// Holds the total size.
        /// </summary>
        private readonly long _size = (long)(1024 * 1024) * (long)(1024 * 32);

        /// <summary>
        /// Holds the block size.
        /// </summary>
        private readonly int _blockSize = 1024 * 1024;

        /// <summary>
        /// Holds the number of flags.
        /// </summary>
        private long _count = 0;

        /// <summary>
        /// Holds the positive flags array.
        /// </summary>
        private SparseLargeBitArray32 _positiveFlags = null;

        /// <summary>
        /// Holds the negative flags array.
        /// </summary>
        private SparseLargeBitArray32 _negativeFlags = null;

        /// <summary>
        /// Creates a new longindex.
        /// </summary>
        public LongIndex()
        {

        }

        /// <summary>
        /// Adds an id.
        /// </summary>
        /// <param name="number"></param>
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
        /// <param name="number"></param>
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
        /// <param name="number"></param>
        /// <returns></returns>
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
        /// <param name="number"></param>
        private void PositiveAdd(long number)
        {
            if(_positiveFlags == null)
            {
                _positiveFlags = new SparseLargeBitArray32(_size, _blockSize);
            }

            if(!_positiveFlags[number])
            { // there is a new positive flag.
                _count++;
            }
            _positiveFlags[number] = true;
        }

        /// <summary>
        /// Removes an id.
        /// </summary>
        /// <param name="number"></param>
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
        /// <param name="number"></param>
        /// <returns></returns>
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
        /// <param name="number"></param>
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
        /// <param name="number"></param>
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
        /// <param name="number"></param>
        /// <returns></returns>
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