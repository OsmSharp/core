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
    /// A multi line string, a collection of one or more linestring instances.
    /// </summary>
    public class MultiLineString : GeometryCollectionBase<LineString>
    {
        /// <summary>
        /// Creates a new multilinestring.
        /// </summary>
        public MultiLineString()
        {

        }

        /// <summary>
        /// Creates a new multilinestring.
        /// </summary>
        public MultiLineString(params LineString[] lineStrings)
            :base(lineStrings)
        {

        }

        /// <summary>
        /// Creates a new multilinestring.
        /// </summary>
        public MultiLineString(IEnumerable<LineString> lineStrings)
            : base(lineStrings)
        {

        }
    }
}