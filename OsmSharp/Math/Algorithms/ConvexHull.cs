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
using OsmSharp.Math.Primitives;

namespace OsmSharp.Math.Algorithms
{
    /// <summary>
    /// A convex hull algorithm implementation.
    /// </summary>
    public static class ConvexHull
    {
        /// <summary>
        /// Calculates a polygon out of a list of points. The resulting polygon the convex hull of all points.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static IList<PointF2D> Calculate(IList<PointF2D> points)
        {
            if (points.Count < 3)
            {
                throw new ArgumentOutOfRangeException(string.Format("Cannot calculate the convex hull of {0} points!",
                    points.Count));
            }
            
            // find the 'left-most' and 'top-most' point.
            PointF2D start = points[0];
            foreach (PointF2D point in points)
            {
                if (start[0] > point[0])
                {
                    start = point;
                }
                else if (start[0] == point[0])
                {
                    if (start[1] < point[01])
                    {
                        start = point;
                    }
                }
            }

            // produce the first reference vector.
            double[] before_start_coords = new double[]{ start[0], start[1] - 10};
            PointF2D before_start = new PointF2D(before_start_coords);
            VectorF2D reference = start - before_start;

            // start the gift-wrapping!
            List<PointF2D> result = new List<PointF2D>();
            PointF2D current = start;
            result.Add(current);

            do
            {
                // find the point with the smallest angle.
                double angle = double.MaxValue;
                PointF2D next = null;
                foreach (PointF2D point in points)
                {
                    if (point != current)
                    {
                        VectorF2D next_vector = point - current;

                        double next_angle = reference.Angle(next_vector).Value;
                        if (next_angle < angle)
                        {
                            angle = next_angle;
                            next = point;
                        }
                    }
                }

                // the found point is the next one.
                reference = next - current;
                current = next;
                result.Add(current);
            }
            while(current != start);

            // return the result.
            return result;
        }
    }
}
