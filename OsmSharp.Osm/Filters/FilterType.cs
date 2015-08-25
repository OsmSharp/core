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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Osm;

namespace OsmSharp.Osm.Filters
{
    /// <summary>
    /// A filter that filters on types of OSM objects.
    /// </summary>
    internal class FilterType : Filter
    {
        /// <summary>
        /// Holds the type to filter on.
        /// </summary>
        private OsmGeoType _type;

        /// <summary>
        /// Creates a new filter type.
        /// </summary>
        /// <param name="type"></param>
        internal FilterType(OsmGeoType type)
        {
            _type = type;
        }

        /// <summary>
        /// Returns true if this object pass through this filter.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Evaluate(OsmGeo obj)
        {
            return obj.Type == _type;
        }

        /// <summary>
        /// Returns a description of this filter.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("istype:{0}",
                                 _type.ToString());
        }
    }
}
