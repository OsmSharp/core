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
using OsmSharp.Math.Primitives;

namespace OsmSharp.Math.Structures
{
    /// <summary>
    /// Abstracts a data structure indexing objects by their location.
    /// </summary>
    public interface ILocatedObjectIndex<TPointType, TDataType>
        where TPointType : PointF2D
    {
        /// <summary>
        /// Returns all objects inside the given bounding box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
		IEnumerable<TDataType> GetInside(BoxF2D box);

        /// <summary>
        /// Adds new located data.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="data"></param>
        void Add(TPointType location, TDataType data);

        /// <summary>
        /// Clears all data from this index.
        /// </summary>
        void Clear();
    }
}