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

using OsmSharp.Collections.Arrays;
using OsmSharp.Collections.Arrays.MemoryMapped;
using OsmSharp.Collections.Coordinates.Collections;
using OsmSharp.IO.MemoryMappedFiles;
using OsmSharp.Math.Geo.Simple;
using System;

namespace OsmSharp.Collections.Coordinates
{
    /// <summary>
    /// Represents a coordinate index based on a huge array.
    /// </summary>
    public class HugeCoordinateIndex : ICoordinateIndex, IDisposable
    {
        /// <summary>
        /// Holds all coordinates.
        /// </summary>
        private HugeArrayBase<float> _coordinates;

        /// <summary>
        /// Holds the count.
        /// </summary>
        private long _count = 0;

        /// <summary>
        /// Creates a new huge coordinate index.
        /// </summary>
        /// <param name="size">The initial size.</param>
        public HugeCoordinateIndex(long size)
            : this(new HugeArray<float>(size * 2))
        {

        }

        /// <summary>
        /// Creates a new, memory mapped, huge coordinate index.
        /// </summary>
        /// <param name="size">The initial size.</param>
        /// <param name="file">The file.</param>
        public HugeCoordinateIndex(MemoryMappedFile file, long size)
            : this(new MemoryMappedHugeArraySingle(file, size * 2))
        {

        }

        /// <summary>
        /// Creates a new huge coordinate index.
        /// </summary>
        /// <param name="coordinates"></param>
        private HugeCoordinateIndex(HugeArrayBase<float> coordinates)
        {
            _coordinates = coordinates;

            for(long idx = 0; idx < _coordinates.Length; idx++)
            {
                _coordinates[idx] = float.MaxValue;
            }
        }

        /// <summary>
        /// Adds the coordinate at the given idx.
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="coordinate"></param>
        public void Add(long idx, Collections.ICoordinate coordinate)
        {
            if(this.Contains(idx))
            {
                throw new ArgumentException("An element with the same key already exists.");
            }
            _coordinates[idx * 2] = coordinate.Latitude;
            _coordinates[(idx * 2) + 1] = coordinate.Longitude;
            _count++;
        }


        /// <summary>
        /// Tries to the the coordinate at the given index.
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public bool TryGet(long idx, out ICoordinate coordinate)
        {
            bool value = this.Contains(idx);
            if(value)
            {
                coordinate = new GeoCoordinateSimple()
                {
                    Latitude = _coordinates[idx * 2],
                    Longitude = _coordinates[(idx * 2) + 1]
                };
                return true;
            }
            coordinate = null;
            return false;
        }

        /// <summary>
        /// Gets or sets the coordinate at the given idx.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public ICoordinate this[long idx]
        {
            get
            {
                if (this.Contains(idx))
                {
                    throw new ArgumentException("Key not found.");
                }
                return new GeoCoordinateSimple()
                {
                    Latitude = _coordinates[idx * 2],
                    Longitude = _coordinates[(idx * 2) + 1]
                };
            }
            set
            {
                if(_coordinates[idx * 2] == float.MinValue)
                {
                    _count++;
                }
                _coordinates[idx * 2] = value.Latitude;
                _coordinates[(idx * 2) + 1] = value.Longitude;
            }
        }

        /// <summary>
        /// Returns true if the given idx is set.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public bool Contains(long idx)
        {
            return _coordinates[idx * 2] != float.MinValue;
        }

        /// <summary>
        /// Removes the coordinate at the given index.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public bool Remove(long idx)
        {
            bool removed = false;
            if (_coordinates[idx * 2] != float.MinValue)
            {
                _count--;
                removed = true;
            }
            _coordinates[idx * 2] = float.MinValue;
            _coordinates[(idx * 2) + 1] = float.MinValue;
            return removed;
        }

        /// <summary>
        /// Returns the count.
        /// </summary>
        public long Count
        {
            get { return _count; }
        }

        /// <summary>
        /// Clears all coordinates from this index.
        /// </summary>
        public void Clear()
        {
            for (long idx = 0; idx < _coordinates.Length; idx++)
            {
                _coordinates[idx] = float.MaxValue;
            }
        }

        /// <summary>
        /// Increases the size of this index.
        /// </summary>
        /// <param name="idx"></param>
        private void IncreaseSize(long idx)
        {
            _coordinates.Resize((idx * 2) + 100000);
        }

        /// <summary>
        /// Disposes of all resources associated with this index.
        /// </summary>
        public void Dispose()
        {
            if(_coordinates != null)
            {
                _coordinates.Dispose();
                _coordinates = null;
            }
        }
    }
}