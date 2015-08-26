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

using OsmSharp.Math.Geo;
using System;

namespace OsmSharp.Geo.Geometries
{
    /// <summary>
    /// Represents a single point.
    /// </summary>
    public class Point : Geometry
    {
        /// <summary>
        /// Creates a new point geometry.
        /// </summary>
        public Point(GeoCoordinate coordinate)
        {
            this.Coordinate = coordinate;
        }

        /// <summary>
        /// Gets/sets the coordinate.
        /// </summary>
        public GeoCoordinate Coordinate { get; set; }

        /// <summary>
        /// Returns the smallest possible bounding box containing this geometry.
        /// </summary>
        public override GeoCoordinateBox Box
        {
            get { return new GeoCoordinateBox(this.Coordinate, this.Coordinate); }
        }

        /// <summary>
        /// Returns true if this point is visible inside the given bounding box.
        /// </summary>
        /// <returns></returns>
        public override bool IsInside(GeoCoordinateBox box)
        {
            if (box == null) { throw new ArgumentNullException(); }

            return box.Contains(this.Coordinate);
        }
    }
}