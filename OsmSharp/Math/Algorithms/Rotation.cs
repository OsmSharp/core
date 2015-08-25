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
using OsmSharp.Units.Angle;

namespace OsmSharp.Math.Algorithms
{
	/// <summary>
	/// Holds simple rotation algorithms.
	/// </summary>
	public class Rotation
	{
		/// <summary>
		/// Rotates a point around another around point with a given angle clockwise.
		/// </summary>
		/// <returns>The around point.</returns>
		/// <param name="angle">Angle.</param>
		/// <param name="center">Center.</param>
		/// <param name="point">Point.</param>
		public static PointF2D RotateAroundPoint(Radian angle, PointF2D center, PointF2D point){
			double sin = System.Math.Sin (angle.Value);
			double cos = System.Math.Cos (angle.Value);

			double newX = center [0] + (cos * (point [0] - center[0]) + sin * (point [1] - center [1]));
			double newY = center [1] + (-sin * (point [0] - center[0]) + cos * (point [1] - center [1]));

			return new PointF2D (newX, newY);
		}

		/// <summary>
		/// Rotates a set of points around another around point with a given angle clockwise.
		/// </summary>
		/// <returns>The around point.</returns>
		/// <param name="angle">Angle.</param>
		/// <param name="center">Center.</param>
		/// <param name="points">Points.</param>
		public static PointF2D[] RotateAroundPoint(Radian angle, PointF2D center, PointF2D[] points) {
			double sin = System.Math.Sin (angle.Value);
			double cos = System.Math.Cos (angle.Value);

			PointF2D[] rotated = new PointF2D[points.Length];
			for (int idx = 0; idx < points.Length; idx++) {
				double newX = center [0] + (cos * (points[idx] [0] - center[0]) + sin * (points[idx] [1] - center [1]));
				double newY = center [1] + (-sin * (points[idx] [0] - center[0]) + cos * (points[idx] [1] - center [1]));

				rotated[idx] = new PointF2D (newX, newY);
			}
			return rotated;
		}
	}
}

