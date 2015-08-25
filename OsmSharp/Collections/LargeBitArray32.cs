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

namespace OsmSharp.Collections
{
    /// <summary>
    /// Represents a large bit array.
    /// </summary>
    public class LargeBitArray32
    {
        /// <summary>
        /// Holds the bit vector array.
        /// </summary>
        private uint[] _array;

        /// <summary>
        /// Holds the length of this array.
        /// </summary>
        private long _length;

        /// <summary>
        /// Creates a new bitvector array.
        /// </summary>
        /// <param name="size"></param>
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
