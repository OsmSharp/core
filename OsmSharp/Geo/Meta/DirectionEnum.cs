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

namespace OsmSharp.Math.Geo.Meta
{
    /// <summary>
    /// Direction types.
    /// </summary>
    public enum DirectionEnum
    {
        /// <summary>
        /// North.
        /// </summary>
        North = 0,
        /// <summary>
        /// Northeast.
        /// </summary>
        NorthEast = 45,
        /// <summary>
        /// East.
        /// </summary>
        East = 90,
        /// <summary>
        /// Southeast.
        /// </summary>
        SouthEast = 135,
        /// <summary>
        /// South.
        /// </summary>
        South = 180,
        /// <summary>
        /// Southwest.
        /// </summary>
        SouthWest = 225,
        /// <summary>
        /// West.
        /// </summary>
        West = 270,
        /// <summary>
        /// Northwest.
        /// </summary>
        NorthWest = 315,
    }
}