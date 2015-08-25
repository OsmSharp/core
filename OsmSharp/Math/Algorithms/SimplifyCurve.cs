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
using OsmSharp.Math.Primitives;
using OsmSharp.Math;

namespace OsmSharp.Math.Algorithms
{
	/// <summary>
	/// Contains an algorithm to simplify a curve based on the Ramer–Douglas–Peucker algorithm.
	/// 
	/// http://en.wikipedia.org/wiki/Ramer-Douglas-Peucker_algorithm
	/// </summary>
	public static class SimplifyCurve
	{
		/// <summary>
		/// Simplify the specified points using epsilon.
		/// </summary>
		/// <param name="points">Points.</param>
		/// <param name="epsilon">Epsilon.</param>
		public static PointF2D[] Simplify(PointF2D[] points, double epsilon)
		{
			return SimplifyCurve.SimplifyBetween (points, epsilon, 0, points.Length - 1);
		}
		
		/// <summary>
		/// Simplify the specified points using epsilon.
		/// </summary>
		/// <param name="points">Points.</param>
		/// <param name="epsilon">Epsilon.</param>
		/// <param name="first">First.</param>
		/// <param name="last">Last.</param>
		public static PointF2D[] SimplifyBetween(PointF2D[] points, double epsilon, int first, int last)
		{
			if (points == null)
				throw new ArgumentNullException ("points");
			if (epsilon <= 0)
                throw new ArgumentOutOfRangeException("epsilon");
            if (first > last)
                throw new ArgumentException(string.Format("first[{0}] must be smaller or equal than last[{1}]!",
                                                          first, last));

			if (first + 1 != last) {
				// find point with the maximum distance.
				double maxDistance = 0;
				int foundIndex = -1;

				// create the line between first-last.
				LineF2D line = new LineF2D (points[first], points [last]);
				for (int idx = first + 1; idx < last; idx++) {
					double distance = line.Distance (points[idx]);
					if (distance > maxDistance) {
						// larger distance found.
						maxDistance = distance;
						foundIndex = idx;
					}
				}

				if (foundIndex > 0 && maxDistance > epsilon) { // a point was found and it is far enough.
					PointF2D[] before = SimplifyCurve.SimplifyBetween (points, epsilon, first, foundIndex);
					PointF2D[] after = SimplifyCurve.SimplifyBetween (points, epsilon, foundIndex, last);

					// build result.
					PointF2D[] result = new PointF2D[before.Length + after.Length - 1];
					for (int idx = 0; idx < before.Length - 1; idx++) {
						result [idx] = before [idx];
					}
					for (int idx = 0; idx < after.Length; idx++) {
						result [idx + before.Length - 1] = after [idx];
					}
					return result;
				}
			}
			return new PointF2D[] { points[first], points[last] };
		}

        /// <summary>
        /// Simplify the specified points using epsilon.
        /// </summary>
        /// <param name="points">Points.</param>
        /// <param name="epsilon">Epsilon.</param>
        public static double[][] Simplify(double[][] points, double epsilon)
        {
            return SimplifyCurve.SimplifyBetween(points, epsilon, 0, points[0].Length - 1);
        }

        /// <summary>
        /// Simplify the specified points using epsilon.
        /// </summary>
        /// <param name="points">Points.</param>
        /// <param name="epsilon">Epsilon.</param>
        /// <param name="first">First.</param>
        /// <param name="last">Last.</param>
        public static double[][] SimplifyBetween(double[][] points, double epsilon, int first, int last)
        {
            if (points == null)
                throw new ArgumentNullException("points");
            if(points.Length != 2)
                throw new ArgumentException();
            if (epsilon < 0)
                throw new ArgumentOutOfRangeException("epsilon");
            if (first > last)
                throw new ArgumentException(string.Format("first[{0}] must be smaller or equal than last[{1}]!",
                                                          first, last));

            if (epsilon == 0)
            { // no simplification is possible.
                return points;
            }
            if (first == last)
            { // first and last are equal, no simplification possible.
                return new double[][] 
                    { new double[] { points[0][first] }, new double[] { points[1][first] } };
            }

            double[][] result;
            // check for identical first and last points.
            if (points[0][first] == points[0][last] &&
                points[1][first] == points[1][last])
            { // first and last point are indentical.
                double[][] before = SimplifyCurve.SimplifyBetween(points, epsilon, first, last - 1);

                // build result.
                result = new double[2][];
                result[0] = new double[before[0].Length + 1];
                result[1] = new double[before[0].Length + 1];
                for (int idx = 0; idx < before[0].Length; idx++)
                {
                    result[0][idx] = before[0][idx];
                    result[1][idx] = before[1][idx];
                }
                result[0][before[0].Length] = points[0][last];
                result[1][before[0].Length] = points[1][last];
                return result;
            }

            if (first + 1 != last)
            {
                // find point with the maximum distance.
                double maxDistance = 0;
                int foundIndex = -1;

                // create the line between first-last.
                LineF2D line = new LineF2D(new PointF2D(
                                               points[0][first], points[1][first]),
                                           new PointF2D(
                                               points[0][last], points[1][last]));
                for (int idx = first + 1; idx < last; idx++)
                {
                    double distance = line.Distance(new PointF2D(
                                                        points[0][idx], points[1][idx]));
                    if (distance > maxDistance)
                    {
                        // larger distance found.
                        maxDistance = distance;
                        foundIndex = idx;
                    }
                }

                if (foundIndex > 0 && maxDistance > epsilon)
                { // a point was found and it is far enough.
                    double[][] before = SimplifyCurve.SimplifyBetween(points, epsilon, first, foundIndex);
                    double[][] after = SimplifyCurve.SimplifyBetween(points, epsilon, foundIndex, last);

                    // build result.
                    result = new double[2][];
                    result[0] = new double[before[0].Length + after[0].Length - 1];
                    result[1] = new double[before[0].Length + after[0].Length - 1];
                    for (int idx = 0; idx < before[0].Length - 1; idx++)
                    {
                        result[0][idx] = before[0][idx];
                        result[1][idx] = before[1][idx];
                    }
                    for (int idx = 0; idx < after[0].Length; idx++)
                    {
                        result[0][idx + before[0].Length - 1] = after[0][idx];
                        result[1][idx + before[0].Length - 1] = after[1][idx];
                    }
                    return result;
                }
            }
            result = new double[2][];
            result[0] = new double[] { points[0][first], points[0][last] };
            result[1] = new double[] { points[1][first], points[1][last] };
            return result;
        }

        /// <summary>
        /// Simplify the specified points using epsilon value.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static double[][] SimplifyPolygon(double[][] points, double epsilon)
        {
            if(points[0].Length <= 2)
            {
                return points;
            }
            return SimplifyCurve.SimplifyBetween(points, epsilon, 0, points[0].Length - 1);
        }
	}
}