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

namespace OsmSharp.Osm
{
    /// <summary>
    /// Represents a simple change.
    /// </summary>
    public class Change
    {
        /// <summary>
        /// The list of objects changing.
        /// </summary>
        public List<OsmGeo> OsmGeo { get; set; }

        /// <summary>
        /// The type of change.
        /// </summary>
        public ChangeType Type { get; set; }
    }
}
