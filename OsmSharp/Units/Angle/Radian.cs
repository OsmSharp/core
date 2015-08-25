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
    /// Represents an angle in radians.
    /// </summary>
    public class Radian : Unit
    {
        private Radian()
            : base(0.0d)
        {

        }

        /// <summary>
        /// Creates a new angle in radians.
        /// </summary>
        /// <param name="value"></param>
        public Radian(double value)
            : base(Radian.Normalize(value))
        {

        }

        /// <summary>
        /// Normalize the specified value.
        /// </summary>
        /// <param name="value">Value.</param>
        private static double Normalize(double value)
        {
            int count2Pi = (int)System.Math.Floor(value / Constants.TwoPi);
            return value - (count2Pi * Constants.TwoPi);
        }

        #region Conversion

        /// <summary>
        /// Converts the given value to radians.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Radian(double value)
        {
            return new Radian(value);
        }

        /// <summary>
        /// Converts the given value to radians.
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        public static implicit operator Radian(Degree deg)
        {
            double value = (deg.Value / 180d) * System.Math.PI;
            return new Radian(value);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="OsmSharp.Units.Angle.Radian"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="OsmSharp.Units.Angle.Radian"/>.</returns>
        public override string ToString()
        {
            return string.Format("{0} rad", this.Value);
        }

        #endregion

        /// <summary>
        /// Subtracts two radians.
        /// </summary>
        /// <param name="rad1"></param>
        /// <param name="rad2"></param>
        /// <returns></returns>
        public static Radian operator -(Radian rad1, Radian rad2)
        {
            return rad1.Value - rad2.Value;
        }
    }
}
