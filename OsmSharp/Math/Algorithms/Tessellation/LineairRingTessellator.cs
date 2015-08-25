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
using OsmSharp.Geo.Geometries;
using OsmSharp.Math.Geo;

namespace OsmSharp.Math.Algorithms.Tessellation
{
    /// <summary>
    /// A tessellator for LineairRings.
    /// </summary>
    public class LineairRingTessellator
    {
        /// <summary>
        /// Tessellates the given LineairRings.
        /// </summary>
        /// <param name="ring"></param>
        /// <returns>A list of coordinates grouped per three.</returns>
        public GeoCoordinate[] Tessellate(LineairRing ring)
        {
            // TODO: yes i know this can be more efficient; proof of concept!
            // TODO: yes i know we know the number of triangles beforehand.
            // TODO: yes i know we can create a strip instead of duplicating coordinates!!

            List<GeoCoordinate> triangles = new List<GeoCoordinate>();
            if (ring.Coordinates.Count < 3)
            {
                throw new ArgumentOutOfRangeException("Invalid ring detected, only 1 or 2 vertices.");
            }
            LineairRing workRing = new LineairRing(ring.Coordinates);
            while (workRing.Coordinates.Count > 3)
            { // cut an ear.
                int earIdx = 0;
                while(!workRing.IsEar(earIdx))
                {
                    earIdx++;
                }

                // ear should be found, cut it!
                GeoCoordinate[] neighbours = workRing.GetNeigbours(earIdx);
                triangles.Add(neighbours[0]);
                triangles.Add(neighbours[1]);
                triangles.Add(workRing.Coordinates[earIdx]);

                // remove ear and update workring.
                List<GeoCoordinate> ringCoordinates = workRing.Coordinates;
                ringCoordinates.RemoveAt(earIdx);
                workRing = new LineairRing(ringCoordinates);
            }
            if (ring.Coordinates.Count == 3)
            { // this ring is already a triangle.
                triangles.Add(ring.Coordinates[0]);
                triangles.Add(ring.Coordinates[1]);
                triangles.Add(ring.Coordinates[2]);
            }
            return triangles.ToArray();
        }
    }
}
