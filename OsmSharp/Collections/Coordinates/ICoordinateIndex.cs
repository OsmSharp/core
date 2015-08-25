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

namespace OsmSharp.Collections.Coordinates
{
    /// <summary>
    /// Abstract representation of a coordinate index.
    /// </summary>
    public interface ICoordinateIndex
    {
        /// <summary>
        /// Adds a new coordinate to this index.
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="coordinate"></param>
        void Add(long idx, ICoordinate coordinate);

        /// <summary>
        /// Tries to the coordinate.
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        bool TryGet(long idx, out ICoordinate coordinate);

        /// <summary>
        /// Gets or sets the coordinate at the given idx.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        ICoordinate this[long idx]
        {
            get;
            set;
        }

        /// <summary>
        /// Contains the coordinate at the given idx.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        bool Contains(long idx);

        /// <summary>
        /// Removes the coordinate at the given idx.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        bool Remove(long idx);

        /// <summary>
        /// Clears all coordinates from this index.
        /// </summary>
        void Clear();

        /// <summary>
        /// Returns the count.
        /// </summary>
        long Count
        {
            get;
        }
    }
}