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

namespace OsmSharp.Osm.Filters.Tags
{
    /// <summary>
    /// Filters on the existence of a tag.
    /// </summary>
    internal class FilterTagExists : FilterTag
    {
        /// <summary>
        /// The tag that needs to exist.
        /// </summary>
        private string _tag;

        /// <summary>
        /// Creates a new tag existance filter.
        /// </summary>
        /// <param name="tag"></param>
        public FilterTagExists(string tag)
        {
            _tag = tag;
        }

        /// <summary>
        /// Returns true if the object passed through the filter.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Evaluate(OsmGeo obj)
        {
            return obj.Tags.ContainsKey(_tag);
        }

        /// <summary>
        /// Returns a description of this filter.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("hastag:key={0}", _tag);
        }
    }
}
