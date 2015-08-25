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

namespace OsmSharp.Collections.Arrays
{
    /// <summary>
    /// A mapped huge array wrapping another huge array. Used to map a more generic class or type to a more simple type like ints or floats.
    /// </summary>
    /// <typeparam name="TMapped">The more 'advanced' stucture.</typeparam>
    /// <typeparam name="T">The 'simple' structure.</typeparam>
    public class MappedHugeArray<TMapped, T> : HugeArrayBase<TMapped>
        where TMapped : struct
        where T : struct
    {
        /// <summary>
        /// Holds the base array.
        /// </summary>
        private HugeArrayBase<T> _baseArray;

        /// <summary>
        /// Holds the size of one element.
        /// </summary>
        private int _elementSize;

        /// <summary>
        /// Holds the map to implemenation.
        /// </summary>
        private MapTo _mapTo;

        /// <summary>
        /// Delegate to abstract mapping implementation for structure mapping.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="idx"></param>
        /// <param name="toMap"></param>
        public delegate void MapTo(HugeArrayBase<T> array, long idx, TMapped toMap);

        /// <summary>
        /// Holds the map from implemenation.
        /// </summary>
        private MapFrom _mapFrom;

        /// <summary>
        /// Delegate to abstract mapping implementation for structure mapping.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="idx"></param>
        public delegate TMapped MapFrom(HugeArrayBase<T> array, long idx);

        /// <summary>
        /// Creates a new mapped huge array.
        /// </summary>
        /// <param name="baseArray">The base array.</param>
        /// <param name="elementSize">The size of one mapped structure when translate to the simpler structure.</param>
        /// <param name="mapTo">The map to implementation.</param>
        /// <param name="mapFrom">The map from implementation.</param>
        public MappedHugeArray(HugeArrayBase<T> baseArray, int elementSize, MapTo mapTo, MapFrom mapFrom)
        {
            _baseArray = baseArray;
            _elementSize = elementSize;
            _mapTo = mapTo;
            _mapFrom = mapFrom;
        }

        /// <summary>
        /// Returns the length of this array.
        /// </summary>
        public override long Length
        {
            get { return _baseArray.Length / _elementSize; }
        }

        /// <summary>
        /// Resizes this array.
        /// </summary>
        /// <param name="size"></param>
        public override void Resize(long size)
        {
            _baseArray.Resize(size * _elementSize);
        }

        /// <summary>
        /// Returns the element at the given index.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public override TMapped this[long idx]
        {
            get
            {
                return _mapFrom.Invoke(_baseArray, idx * _elementSize);
            }
            set
            {
                _mapTo.Invoke(_baseArray, idx * _elementSize, value);
            }
        }

        /// <summary>
        /// Disposes of all native resources associated with this object.
        /// </summary>
        public override void Dispose()
        {
            _baseArray.Dispose();
            _baseArray = null;
        }
    }
}