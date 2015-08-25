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

namespace OsmSharp.Units.Angle
{
    /// <summary>
    /// Represents an angle in degress.
    /// </summary>
    public class Degree : Unit
    {
        private Degree()
            : base(0.0d)
        {

        }

        /// <summary>
        /// Creates a new angle in degrees.
        /// </summary>
        /// <param name="value"></param>
        public Degree(double value)
            :base(Degree.Normalize(value))
        {

        }

		/// <summary>
		/// Normalize the specified value.
		/// </summary>
		/// <param name="value">Value.</param>
        private static double Normalize(double value)
        {
            int count360 = (int)System.Math.Floor(value / 360.0);
            return value - (count360 * 360.0);
        }

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="OsmSharp.Units.Angle.Degree"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="OsmSharp.Units.Angle.Degree"/>.</returns>
		public override string ToString ()
		{
			return string.Format ("{0}°", this.Value);
		}

        /// <summary>
        /// Converts the given angle to the range -180, +180.
        /// </summary>
        /// <returns></returns>
        public double Range180()
        {
            if (this.Value > 180)
            {
                return this.Value - 360;
            }
            return this.Value;
        }

        /// <summary>
        /// Substracts the two angles returning an angle in the range -180, +180
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public double SmallestDifference(Degree angle)
        {
            return (this - angle).Range180();
        }

        #region Conversion

        /// <summary>
        /// Converts the given value to degrees.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Degree(double value)
        {
            return new Degree(value);
        }

        /// <summary>
        /// Converts the given value to degrees.
        /// </summary>
        /// <param name="rad"></param>
        /// <returns></returns>
        public static implicit operator Degree(Radian rad)
        {
            double value = (rad.Value / System.Math.PI) * 180d;
            return new Degree(value);
        }

        #endregion

        #region Operators

        /// <summary>
        /// Subtracts two angles.
        /// </summary>
        /// <param name="deg1"></param>
        /// <param name="deg2"></param>
        /// <returns></returns>
        public static Degree operator -(Degree deg1, Degree deg2)
        {
            return deg1.Value - deg2.Value;
        }

        /// <summary>
        /// Adds two angles.
        /// </summary>
        /// <param name="deg1"></param>
        /// <param name="deg2"></param>
        /// <returns></returns>
        public static Degree operator +(Degree deg1, Degree deg2)
        {
            return deg1.Value + deg2.Value;
        }

        /// <summary>
        /// Returns the absolute value of the angle.
        /// </summary>
        /// <returns></returns>
        public Degree Abs()
        {
            return System.Math.Abs(this.Value);
        }

        /// <summary>
        /// Returns true if one angle is greater than the other.
        /// </summary>
        /// <param name="deg1"></param>
        /// <param name="deg2"></param>
        /// <returns></returns>
        public static bool operator >(Degree deg1,Degree deg2)
        {
            return deg1.Value > deg2.Value;
        }

        /// <summary>
        /// Returns true if one angle is smaller than the other.
        /// </summary>
        /// <param name="deg1"></param>
        /// <param name="deg2"></param>
        /// <returns></returns>
        public static bool operator <(Degree deg1, Degree deg2)
        {
            return deg1.Value < deg2.Value;
        }

        /// <summary>
        /// Returns true if one angle is greater or equal than the other.
        /// </summary>
        /// <param name="deg1"></param>
        /// <param name="deg2"></param>
        /// <returns></returns>
        public static bool operator >=(Degree deg1, Degree deg2)
        {
            return deg1.Value >= deg2.Value;
        }

        /// <summary>
        /// Returns true if one angle is smaller or equal than the other.
        /// </summary>
        /// <param name="deg1"></param>
        /// <param name="deg2"></param>
        /// <returns></returns>
        public static bool operator <=(Degree deg1, Degree deg2)
        {
            return deg1.Value <= deg2.Value;
        }

        #endregion
    }
}
