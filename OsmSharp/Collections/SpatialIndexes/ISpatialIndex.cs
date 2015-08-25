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

using System.Collections.Generic;
using OsmSharp.Math;
using OsmSharp.Math.Primitives;

namespace OsmSharp.Collections.SpatialIndexes
{
    /// <summary>
    /// Abstract representation of a spatial index.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISpatialIndex<T> : ISpatialIndexReadonly<T>, IEnumerable<T>
    {
        /// <summary>
        /// Adds a new item with the corresponding box.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="item"></param>
		void Add(BoxF2D box, T item);

        /// <summary>
        /// Removes the given item.
        /// </summary>
        /// <param name="item"></param>
        void Remove(T item);

        /// <summary>
        /// Removes the given item.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="item"></param>
        void Remove(BoxF2D box, T item);
    }
}