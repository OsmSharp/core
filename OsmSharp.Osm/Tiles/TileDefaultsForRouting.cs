// OsmSharp - OpenStreetMap (OSM) SDK
//
// Copyright (C) 2013 Simon Hughes
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

namespace OsmSharp.Osm.Tiles
{
    /// <summary>
    /// Provides default values for routing
    /// </summary>
    public static class TileDefaultsForRouting
    {
        /// <summary>
        /// This is the default zoom level for routing.
        /// Do not alter this value if reading from a database.
        /// If you do, you will have to recalibrate the tiles for your database.
        /// </summary>
        public const int Zoom = 14;
    }
}