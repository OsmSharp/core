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
using OsmSharp.Osm.Streams.Filters;

namespace OsmSharp.Test.Osm.Streams.Filters
{
    /// <summary>
    /// A simple reference implementation that filters out all objects with even ids.
    /// </summary>
    internal class OsmStreamFilterReference : OsmStreamFilterBase
    {
        /// <summary>
        /// Creates a new stream filter reference.
        /// </summary>
        public OsmStreamFilterReference()
            : base(new OsmSharp.Osm.Cache.OsmDataCacheMemory())
        {

        }

        /// <summary>
        /// Returns true when the given osmGeo object has an uneven id.
        /// </summary>
        /// <param name="osmGeo"></param>
        /// <returns></returns>
        public override bool Include(OsmSharp.Osm.OsmGeo osmGeo)
        {
            return osmGeo.Id.Value % 2 == 1;
        }
    }
}
