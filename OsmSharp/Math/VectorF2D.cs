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
using OsmSharp.Units.Angle;
using System.Globalization;

namespace OsmSharp.Math
{
    /// <summary>
    /// Represents a vector in n dimensions.
    /// 
    /// A vector is immutable.
    /// </summary>
    /// <remarks>
    /// A vector is not a primitive, it can only be represented relative to real primitives.
    /// </remarks>
    public class VectorF2D
    {
        /// <summary>
        /// The values that represent the vector.
        /// </summary>
        private double[] _values;

        #region Constructors

        /// <summary>
        /// Creates a new vector.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public VectorF2D(double a, double b)
        {
            _values = new double[2];
            _values[0] = a;
            _values[1] = b;
        }

        /// <summary>
        /// Creates a new vector.
        /// </summary>
        /// <param name="values"></param>
        public VectorF2D(double[] values)
        {
            _values = values;

            if (_values.Length != 2)
            {
                throw new ArgumentException("Invalid # dimensions!");
            }
        }

        #endregion

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
        /// Returns the size of the vector.
        /// </summary>
        public double Size
        {
            get
            {
                double size = 0.0f;

                for (int idx = 0; idx < 2; idx++)
                {
                    size = size + (_values[idx] * _values[idx]);
                }

                return (double)System.Math.Sqrt(size);
            }
        }

        /// <summary>
        /// Returns the exact inverse of this vector.
        /// </summary>
        public VectorF2D Inverse
        {
            get
			{
				return new VectorF2D (new double[] { -_values[0], -_values[1] });
            }
        }

		/// <summary>
		/// Gets the inverse x.
		/// </summary>
		/// <value>The inverse x.</value>
		public VectorF2D InverseX
		{
			get{
				return new VectorF2D (new double[] { -_values[0], _values[1] });
			}
		}

		/// <summary>
		/// Gets the inverse y.
		/// </summary>
		/// <value>The inverse y.</value>
		public VectorF2D InverseY
		{
			get{
				return new VectorF2D (new double[] { _values[0], -_values[1] });
			}
		}

		/// <summary>
		/// Returns a description of this vector.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("Vector({0},{1})",
			                     _values[0].ToString(CultureInfo.InvariantCulture),
			                     _values[1].ToString(CultureInfo.InvariantCulture));
		}

        #endregion

        #region Calculations

        /// <summary>
        /// Calcuates the dot-product of the two given vectors.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double Dot(VectorF2D a, VectorF2D b)
        {
            double dot = 0.0f;

            for (int idx = 0; idx < 2; idx++)
            {
                dot = dot + a[idx] * b[idx];
            }

            return dot;
        }

        /// <summary>
        /// Calculates the cross product.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double Cross(VectorF2D a, VectorF2D b)
        {
            return a[0] * b[1] - a[1] * b[0];
        }

        /// <summary>
        /// Returns the angle between 0 and 2*pi between the two vectors given.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Radian Angle(VectorF2D v1, VectorF2D v2)
        {
            double size_v1 = v1.Size;
            double size_v2 = v2.Size;

            double dot = VectorF2D.Dot(v1, v2);
            double cross = VectorF2D.Cross(v1, v2);

            // filter out the vectors that are parallel.
            if (v1[0] == v2[0]
                && v1[1] == v2[1])
            {
                return 0;
            }
            else if (v1[0] == v2[0]
                && v1[1] == -v2[1])
            {
                return System.Math.PI / 2.0f;
            }
            else if (v1[0] == -v2[0]
                && v1[1] == v2[1])
            {
                return -System.Math.PI / 2.0f;
            }
            else if (v1[0] == -v2[0]
                && v1[1] == -v2[1])
            {
                return System.Math.PI;
            }

            // split per quadrant.
            double angle;
            if (dot > 0)
            { // dot > 0
                if (cross > 0)
                { // dot > 0 and cross > 0
                    // Quadrant 1
                    angle = (double)System.Math.Asin(cross / (size_v1 * size_v2));
                    if (angle < System.Math.PI / 4f)
                    { // use cosine.
                        angle = (double)System.Math.Acos(dot / (size_v1 * size_v2));
                    }
                    // angle is ok here for quadrant 1.
                }
                else
                { // dot > 0 and cross <= 0
                    // Quadrant 4
                    angle = (double)(System.Math.PI * 2.0f) + (double)System.Math.Asin(cross / (size_v1 * size_v2));
                    if (angle > (double)(System.Math.PI * 2.0f) - System.Math.PI / 4f)
                    { // use cosine.
                        angle = (double)(System.Math.PI * 2.0f) - (double)System.Math.Acos(dot / (size_v1 * size_v2));
                    }
                    // angle is ok here for quadrant 1.
                }
            }
            else
            { // dot <= 0
                if (cross > 0)
                { // dot > 0 and cross > 0
                    // Quadrant 2
                    angle = (double)System.Math.PI - (double)System.Math.Asin(cross / (size_v1 * size_v2));
                    if (angle > System.Math.PI / 2f + System.Math.PI / 4f)
                    { // use cosine.
                        angle = (double)System.Math.Acos(dot / (size_v1 * size_v2));
                    }
                    // angle is ok here for quadrant 2.
                }
                else
                { // dot > 0 and cross <= 0
                    // Quadrant 3
                    angle = -(-(double)System.Math.PI + (double)System.Math.Asin(cross / (size_v1 * size_v2)));
                    if (angle < System.Math.PI + System.Math.PI / 4f)
                    { // use cosine.
                        angle = (double)(System.Math.PI * 2.0f) - (double)System.Math.Acos(dot / (size_v1 * size_v2));
                    }
                    // angle is ok here for quadrant 3.
                }
            }
            return angle;
        }

        /// <summary>
        /// Returns the angle between this vector and the given vector in the range 0-2pi.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Radian Angle(VectorF2D v)
        {
            return VectorF2D.Angle(this, v);
        }

		/// <summary>
		/// Returns a normalized vector with a direction equal to the given angle relative to the y-axis.
		/// </summary>
		/// <returns>The angle y.</returns>
		/// <param name="angle">Direction.</param>
		public static VectorF2D FromAngleY (Degree angle)
		{
			double sinX = System.Math.Sin (((Radian)angle).Value);
			double cosX = System.Math.Cos (((Radian)angle).Value);

			return new VectorF2D (sinX, cosX);
		}

        /// <summary>
        /// Rotates a vector 90 degrees.
        /// </summary>
        /// <param name="clockwise">True when rotation has to be clockwize.</param>
        /// <returns></returns>
        public VectorF2D Rotate90(bool clockwise)
        {
            if (clockwise)
            {
                double y = -this[0];
                double x = this[1];
                return new VectorF2D(x, y);
            }
            else
            {
                double y = this[0];
                double x = -this[1];
                return new VectorF2D(x, y);
            }
        }

		/// <summary>
		/// Returns a normalized version of this Vector.
		/// </summary>
		public VectorF2D Normalize()
		{
			double s = this.Size;
			double x = this [0] / s;
			double y = this [1] / s;
			return new VectorF2D (x, y);
		}

        #endregion

        #region Operators

        /// <summary>
        /// Substracts two vectors and returns the resulting vector.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static VectorF2D operator -(VectorF2D a, VectorF2D b)
        {
            double[] c = new double[2];

            for (int idx = 0; idx < 2; idx++)
            {
                c[idx] = a[idx] - b[idx];
            }
            return new VectorF2D(c);
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static VectorF2D operator +(VectorF2D a, VectorF2D b)
        {
            double[] c = new double[2];

            for (int idx = 0; idx < 2; idx++)
            {
                c[idx] = a[idx] + b[idx];
            }

            return new VectorF2D(c);
        }

        /// <summary>
        /// Multiplies the given vector with the given value.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static VectorF2D operator *(VectorF2D a, double value)
        {
            double[] c = new double[2];

            for (int idx = 0; idx < 2; idx++)
            {
                c[idx] = a[idx] * value;
            }

            return new VectorF2D(c);
        }

        /// <summary>
        /// Divides the given vector with the given value.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static VectorF2D operator /(VectorF2D a, double value)
        {
            double[] c = new double[2];

            for (int idx = 0; idx < 2; idx++)
            {
                c[idx] = a[idx] / value;
            }

            return new VectorF2D(c);
        }

        /// <summary>
        /// Returns true if the two vectors represent the same.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(VectorF2D a, VectorF2D b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match.
            for (int idx = 0; idx < 2; idx++)
            {
                if(a[idx] != b[idx])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns true if the two vectors don't represent the same.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(VectorF2D a, VectorF2D b)
        {
            return !(a == b);
        }

        #endregion

        #region Comparison Methods

        /// <summary>
        /// Compares the two vectors just based on their direction.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool CompareNormalized(VectorF2D other)
        { // be a sensitive as possible.
            return this.CompareNormalized(other, 0);
        }

        /// <summary>
        /// Compares the two vectors just based on their direction.
        /// </summary>
        /// <param name="other">The other vector to compare to.</param>
        /// <param name="epsilon">The tolerance on the total difference between the normalized vectors.</param>
        /// <returns></returns>
        public bool CompareNormalized(VectorF2D other, double epsilon)
        {
            VectorF2D normalizedThis = this.Normalize();
            VectorF2D normalizedOther = other.Normalize();

            double difference = System.Math.Abs(normalizedThis[0] - normalizedOther[0]) +
                System.Math.Abs(normalizedThis[1] - normalizedOther[1]);
            return difference < epsilon;
        }

        #endregion

        #region Equals/GetHashCode

        /// <summary>
        /// Returns true if both objects are equal in value.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is VectorF2D)
            {
                return this == (obj as VectorF2D);
            }
            return false;
        }

        /// <summary>
        /// Returns a unique hascode for this vector.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            // this is possible because a vector is immutable.
            return "vector".GetHashCode() ^ this[0].GetHashCode() ^ this[1].GetHashCode();
        }

        #endregion

    }
}
