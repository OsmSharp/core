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
using OsmSharp.Osm;
using OsmSharp.Osm.Streams.Collections;

namespace OsmSharp.Osm.Streams
{
    /// <summary>
    /// Contains extensions for common OsmStreamReader/Writer/Filter implementations.
    /// </summary>
    public static class OsmStreamExtensions
    {
        /// <summary>
        /// Returns an OSM stream reader for the objects in this enumerable.
        /// </summary>
        /// <param name="enumerable"></param>
        public static OsmStreamSource ToOsmStreamSource(this IEnumerable<OsmGeo> enumerable)
        {
            return new OsmEnumerableStreamSource(enumerable);
        }
    }
}