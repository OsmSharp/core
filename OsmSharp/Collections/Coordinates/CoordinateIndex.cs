// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2014 Abelshausen Ben
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

using OsmSharp.Collections.Coordinates.Collections;
using System.Collections.Generic;

namespace OsmSharp.Collections.Coordinates
{
    /// <summary>
    /// Represents acoordinate index.
    /// </summary>
    public class CoordinateIndex : ICoordinateIndex
    {
        /// <summary>
        /// Holds the coordinates.
        /// </summary>
        private HugeDictionary<long, ICoordinate> _coordinates;

        /// <summary>
        /// Creates the coordinate index.
        /// </summary>
        public CoordinateIndex()
        {
            _coordinates = new HugeDictionary<long, ICoordinate>();
        }

        /// <summary>
        /// Adds the given coordinate.
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="coordinate"></param>
        public void Add(long idx, ICoordinate coordinate)
        {
            _coordinates.Add(idx, coordinate);
        }

        /// <summary>
        /// Tries to the the coordinate for the given idx.
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public bool TryGet(long idx, out ICoordinate coordinate)
        {
            return _coordinates.TryGetValue(idx, out coordinate);
        }

        /// <summary>
        /// Gets or sets the coordinate for the given idx.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public ICoordinate this[long idx]
        {
            get
            {
                return _coordinates[idx];
            }
            set
            {
                _coordinates[idx] = value;
            }
        }

        /// <summary>
        /// Returns true if the given index is in this index.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public bool Contains(long idx)
        {
            return _coordinates.ContainsKey(idx);
        }

        /// <summary>
        /// Removes the coordinate at the given index.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public bool Remove(long idx)
        {
            return _coordinates.Remove(idx);
        }

        /// <summary>
        /// Returns the count.
        /// </summary>
        public long Count
        {
            get { return _coordinates.Count; }
        }

        /// <summary>
        /// Clears all coordinates from this index.
        /// </summary>
        public void Clear()
        {
            _coordinates.Clear();
        }
    }
}