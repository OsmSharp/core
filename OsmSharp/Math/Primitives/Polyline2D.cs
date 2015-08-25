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

namespace OsmSharp.Math.Primitives
{
    /// <summary>
    /// A polyline.
    /// </summary>
    public class Polyline2D
    {
        /// <summary>
        /// Calculates the length of the given polyline.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double Length(double[] x, double[] y)
        {
            double length = 0;
            if (x.Length > 1)
            {
                for (int idx = 1; idx < x.Length; idx++)
				{
					double xDiff = (x [idx - 1] - x [idx]);
					double yDiff = (y [idx - 1] - y [idx]);
					length = length + System.Math.Sqrt ((xDiff * xDiff) + (yDiff * yDiff));
                }
            }
            return length;
        }

        /// <summary>
        /// Returns a position on the given polyline corresponding with the given position. (0 smaller than position smaller than polyline.lenght).
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static PointF2D PositionAtPosition(double[] x, double[] y, double position)
        {
            if (x.Length < 2) throw new ArgumentOutOfRangeException("Given coordinates do not represent a polyline.");

            double lenght = 0;
			LineF2D localLine;
            for (int idx = 1; idx < x.Length; idx++)
            {
				double xDiff = (x [idx - 1] - x [idx]);
				double yDiff = (y [idx - 1] - y [idx]);
				double localLength = System.Math.Sqrt ((xDiff * xDiff) + (yDiff * yDiff));
                if(lenght + localLength > position)
				{ // position is between point at idx and idx + 1.
					localLine = new LineF2D(new PointF2D(x[idx - 1], y[idx - 1]),
						new PointF2D(x[idx], y[idx]));
                    double localPosition = position - lenght;
                    VectorF2D vector = localLine.Direction.Normalize() * localPosition;
                    return localLine.Point1 + vector;
                }
                lenght = lenght + localLength;
            }
			localLine = new LineF2D(new PointF2D(x[x.Length - 2], y[x.Length - 2]),
				new PointF2D(x[x.Length - 1], y[x.Length - 1]));
            return localLine.Point1 + (localLine.Direction.Normalize() * (position - lenght));
        }
    }
}
