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
using OsmSharp.Math.Geo;
using OsmSharp.Math.Primitives;
using OsmSharp.Math;

namespace OsmSharp.Geo.Geometries
{
    /// <summary>
    /// Represents a lineair ring, a polygon without holes.
    /// </summary>
    public class LineairRing : LineString
    {
        /// <summary>
        /// Creates a new lineair ring.
        /// </summary>
        public LineairRing()
        {

        }

        /// <summary>
        /// Creates a new lineair ring.
        /// </summary>
        /// <param name="coordinates">The coordinates.</param>
        public LineairRing(IEnumerable<GeoCoordinate> coordinates)
            : base(coordinates)
        {

        }

        /// <summary>
        /// Creates a new lineair ring.
        /// </summary>
        /// <param name="coordinates">The coordinates.</param>
        public LineairRing(params GeoCoordinate[] coordinates)
            : base(coordinates)
        {

        }

        /// <summary>
        /// Returns true if the given vertex is convex.
        /// </summary>
        /// <param name="vertexIdx"></param>
        /// <returns></returns>
        public bool IsEar(int vertexIdx)
        {
            int previousIdx = vertexIdx == 0 ? this.Coordinates.Count - 1 : vertexIdx - 1;
            int nextIdx = vertexIdx == this.Coordinates.Count - 1 ? 0 : vertexIdx + 1;

            GeoCoordinate vertex = this.Coordinates[vertexIdx];
            GeoCoordinate previous = this.Coordinates[previousIdx];
            GeoCoordinate next = this.Coordinates[nextIdx];

            GeoCoordinate between = (next + previous) / 2;

            return (this.Contains(between));
        }

        /// <summary>
        /// Returns the neighbours of the given vertex.
        /// </summary>
        /// <returns></returns>
        public GeoCoordinate[] GetNeigbours(int vertexIdx)
        {
            int previousIdx = vertexIdx == 0 ? this.Coordinates.Count - 1 : vertexIdx - 1;
            int nextIdx = vertexIdx == this.Coordinates.Count - 1 ? 0 : vertexIdx + 1;

            GeoCoordinate previous = this.Coordinates[previousIdx];
            GeoCoordinate next = this.Coordinates[nextIdx];
            return new GeoCoordinate[] { previous, next };
        }

        /// <summary>
        /// Returns true if the given coordinate is contained in the inner area of the ring or lying on the border of the ring.
        /// Fast way based on the winding number aproach.        
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public bool Contains(GeoCoordinate coordinate)
        {
            bool flipflop = false;
            const bool includeBorder = true; // Algoritm could be parameterized to optionally include the border.

            for (int i = 0, j = Coordinates.Count - 1; i < Coordinates.Count; j = i++)
            {
                if (Coordinates[j].Equals(coordinate))
                    return includeBorder;

                bool b = Coordinates[i].Latitude <= coordinate.Latitude;
                if (b != (Coordinates[j].Latitude <= coordinate.Latitude))
                {
                    var triangularOrientation = (Coordinates[j].Longitude - Coordinates[i].Longitude) * (coordinate.Latitude - Coordinates[i].Latitude) - (Coordinates[j].Latitude - Coordinates[i].Latitude) * (coordinate.Longitude - Coordinates[i].Longitude);
                    if (triangularOrientation > 0 && b || triangularOrientation < 0 && !b)
                        flipflop = !flipflop;
                    else if (triangularOrientation == 0)
                        return includeBorder;
                }
            }
            return flipflop;
        }

        /// <summary>
        /// Returns true if the given ring is contained in this ring.
        /// </summary>
        /// <param name="lineairRing"></param>
        /// <returns></returns>
        public bool Contains(LineairRing lineairRing)
        {
            // check if all points are inside this ring.
            foreach (var coordinate in lineairRing.Coordinates)
            {
                if (!this.Contains(coordinate))
                { // a coordinate ouside of this ring can never be part of a contained inner ring.
                    return false;
                }
            }
            // check if none of the points of this ring are inside the other ring.
            foreach (var coordinate in this.Coordinates)
            {
                if (lineairRing.Contains(coordinate))
                { // a coordinate ouside of this ring can never be part of a contained inner ring.
                    return false;
                }
            }
            return true;
        }
    }
}
