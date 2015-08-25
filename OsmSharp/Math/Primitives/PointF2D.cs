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
using System.Globalization;

namespace OsmSharp.Math.Primitives
{
    /// <summary>
    /// Represents a point in 2 dimensions.
    /// </summary>
    public class PointF2D : PrimitiveF2D
    {
        /// <summary>
        /// The values that represents the point.
        /// </summary>
        private double[] _values;

        /// <summary>
        /// Creates a new point.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public PointF2D(double x, double y)
        {
            _values = new double[] { x, y };
        }

        /// <summary>
        /// Creates a new point.
        /// </summary>
        /// <param name="values"></param>
        public PointF2D(double[] values)
        {
            _values = values;

            if (_values.Length != 2)
            {
                throw new ArgumentException("Invalid # dimensions!");
            }
        }

        #region Properties

        /// <summary>
        /// Gets/Sets the value at index idx.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public double this[int idx]
        {
            get
            {
                return _values[idx];
            }
        }

        /// <summary>
        /// Converts to point to an array.
        /// </summary>
        /// <returns></returns>
        internal double[] ToArray()
        {
            return _values;
        }

        #endregion

        #region Calculations

        /// <summary>
        /// Calculates the distance between this point and the given point.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public override double Distance(PointF2D p)
        {
            return PointF2D.Distance(this, p);
        }

        /// <summary>
        /// Calculates the distance between two points.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected static double Distance(PointF2D a, PointF2D b)
        {
            double distance = 0.0f;

            for (int idx = 0; idx < 2; idx++)
            {
                double idx_diff = a[idx] - b[idx];
                distance = distance + (idx_diff * idx_diff);
            }
            return (double)System.Math.Sqrt(distance);
        }

        /// <summary>
        /// Creates a 
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public BoxF2D CreateBox(double offset)
        {
            return new BoxF2D(this[0] - offset, this[1] - offset, 
                this[0] + offset, this[1] + offset);
        }

        #endregion
        
        #region Operators

        /// <summary>
        /// Substracts two points and returns the resulting vector.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static VectorF2D operator -(PointF2D a, PointF2D b)
        {
            double[] c = new double[2];

            for (int idx = 0; idx < 2; idx++)
            {
                c[idx] = a[idx] - b[idx];
            }

            return new VectorF2D(c);
        }

        /// <summary>
        /// Adds a point and a vector and returns the resulting point.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static PointF2D operator +(PointF2D a, VectorF2D b)
        {
            double[] c = new double[2];

            for (int idx = 0; idx < 2; idx++)
            {
                c[idx] = a[idx] + b[idx];
            }

            return new PointF2D(c);
        }

		/// <summary>
		/// Adds a point and a vector and returns the resulting point.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static PointF2D operator -(PointF2D a, VectorF2D b)
		{
			double[] c = new double[2];

			for (int idx = 0; idx < 2; idx++)
			{
				c[idx] = a[idx] - b[idx];
			}

			return new PointF2D(c);
		}

        /// <summary>
        /// Returns true if both points represent the same point.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(PointF2D a, PointF2D b)
        {
            if ((object)a != null &&
                (object)b == null)
            {
                return false;
            }
            if ((object)b != null &&
                (object)a == null)
            {
                return false;
            }
            if ((object)a == null &&
                (object)b == null)
            {
                return true;
            }
            return a._values[0] == b._values[0] &&
                a._values[1] == b._values[1];
        }

        /// <summary>
        /// Returns false if both points represent the same point.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(PointF2D a, PointF2D b)
        {
            if ((object)a != null &&
                (object)b == null)
            {
                return true;
            }
            if ((object)b != null &&
                (object)a == null)
            {
                return true;
            }
            if ((object)a == null &&
                (object)b == null)
            {
                return false;
            }
            return a._values[0] != b._values[0] ||
                a._values[1] != b._values[1];
        }

        #endregion

        /// <summary>
        /// Returns a description of this point.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Point({0},{1})",
                _values[0].ToInvariantString(),
                _values[1].ToInvariantString());
        }

        #region Equals/GetHashCode

        /// <summary>
        /// Returns true if both objects are equal in value.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var other = obj as PointF2D;
            if (obj != null)
            {
                return this._values[0] == other[0] &&
                    this._values[1] == other[1];
            }
            return false;
        }

        /// <summary>
        /// Returns a unique hashcode for this point.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return "point".GetHashCode() ^ this[0].GetHashCode() ^ this[1].GetHashCode();
        }

        #endregion
    }
}
