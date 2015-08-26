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

namespace OsmSharp.Math.Geo
{
    /// <summary>
    /// Contains generic extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Estimates distance.
        /// </summary>
        /// <returns></returns>
        public static double DistanceEstimate(this GeoCoordinate[] coordinates, int start, int lenght)
        {
            double distance = 0;
            for (int idx = start; idx < lenght + start; idx++)
            {
                if (idx + 1 < lenght + start)
                {
                    distance = distance +
                        coordinates[idx].DistanceEstimate(coordinates[idx + 1]).Value;
                }
            }
            return distance;
        }
    }
}
