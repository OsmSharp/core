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
using System.Collections.Generic;

namespace OsmSharp.Geo.Geometries
{
    /// <summary>
    /// Represents a polygon.
    /// </summary>
    public class Polygon : Geometry
    {
        /// <summary>
        /// Creates a new polygon.
        /// </summary>
        public Polygon()
        {
            this.Holes = new List<LineairRing>();
            this.Ring = new LineairRing();
        }

        /// <summary>
        /// Creates a new polygon.
        /// </summary>
        public Polygon(LineairRing outline)
        {
            this.Holes = new List<LineairRing>();
            this.Ring = outline;
        }

        /// <summary>
        /// Creates a new polygon.
        /// </summary>
        public Polygon(LineairRing outline, IEnumerable<LineairRing> holes)
        {
            this.Holes = holes;
            this.Ring = outline;
        }

        /// <summary>
        /// Gets the holes in this polygon.
        /// </summary>
        public IEnumerable<LineairRing> Holes { get; private set; }

        /// <summary>
        /// Gets the outer outline lineair ring of this polygon.
        /// </summary>
        public LineairRing Ring { get; set; }

        /// <summary>
        /// Returns the smallest possible bounding box containing this geometry.
        /// </summary>
        public override GeoCoordinateBox Box
        {
            get
            {
                GeoCoordinateBox box = this.Ring.Box;
                foreach (Geometry geometry in this.Holes)
                {
                    if (box == null)
                    {
                        box = geometry.Box;
                    }
                    else
                    {
                        box = box + geometry.Box;
                    }
                }
                return box;
            }
        }

        /// <summary>
        /// Returns true if the point is inside this polygon.
        /// </summary>
        /// <returns></returns>
        public bool Contains(GeoCoordinate point)
        {
            if (this.Ring.Contains(point))
            {
                foreach (var hole in this.Holes)
                {
                    if (hole.Contains(point))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if the given ring is contained in this polygon.
        /// </summary>
        /// <returns></returns>
        public bool Contains(LineairRing lineairRing)
        {
            // check if all points are inside this polygon.
            foreach (var coordinate in lineairRing.Coordinates)
            {
                if (!this.Contains(coordinate))
                { // a coordinate ouside of this ring can never be part of a contained inner ring.
                    return false;
                }
            }
            // check if none of the points of this ring are inside the other ring.
            foreach (var coordinate in this.Ring.Coordinates)
            {
                if (lineairRing.Contains(coordinate))
                { // a coordinate ouside of this ring can never be part of a contained inner ring.
                    return false;
                }
            }
            foreach (var hole in this.Holes)
            {
                foreach (var coordinate in hole.Coordinates)
                {
                    if (lineairRing.Contains(coordinate))
                    { // a coordinate ouside of this ring can never be part of a contained inner ring.
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Returns true if the given polygon is contained in this polygon.
        /// </summary>
        /// <returns></returns>
        public bool Contains(Polygon polygon)
        {
            return this.Contains(polygon);
        }

        /// <summary>
        /// Returns true if this polygon is inside the given bounding box.
        /// </summary>
        /// <returns></returns>
        public override bool IsInside(GeoCoordinateBox box)
        {
            throw new NotImplementedException();
        }
    }
}