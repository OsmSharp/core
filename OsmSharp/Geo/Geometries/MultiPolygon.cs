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

using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Geo.Geometries
{
    /// <summary>
    /// A multi polygon, a collection of zero or more polygons.
    /// </summary>
    public class MultiPolygon : GeometryCollectionBase<Polygon>
    {
        /// <summary>
        /// Creates a new multipolygon string.
        /// </summary>
        public MultiPolygon()
        {

        }

        /// <summary>
        /// Creates a new multipolygon string.
        /// </summary>
        public MultiPolygon(params Polygon[] polygons)
            : base(polygons)
        {

        }

        /// <summary>
        /// Creates a new multipolygon string.
        /// </summary>
        public MultiPolygon(IEnumerable<Polygon> polygons)
            : base(polygons)
        {

        }
    }
}