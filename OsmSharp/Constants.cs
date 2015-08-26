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

namespace OsmSharp
{
    /// <summary>
    /// Contains generic constants.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The radius of earth in meters.
        /// </summary>
        public const double RadiusOfEarth = 6371000;

        /// <summary>
        /// 2.0 * Math.PI
        /// </summary>
        public const double TwoPi = 6.283185307179586476925286766559;  // 2.0 * Math.PI;

        /// <summary>
        /// The number of seconds per hour.
        /// </summary>
        public const double SecondsPerHour = 3600.0;

        /// <summary>
        /// Regex to parse decimals.
        /// </summary>
        public const string RegexDecimal = @"(\d+(?:\.\d*)?)";

        /// <summary>
        /// Regex for whitespaces.
        /// </summary>
        public const string RegexDecimalWhiteSpace = @"\s*" + RegexDecimal + @"\s*";
    }
}