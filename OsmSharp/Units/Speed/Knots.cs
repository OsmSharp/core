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
using System.Text.RegularExpressions;
using System.Globalization;
using OsmSharp.Units.Distance;
using OsmSharp.Units.Time;

namespace OsmSharp.Units.Speed
{
    /// <summary>
    /// Represents a speed in knots.
    /// </summary>
    public class Knots : Speed
    {
        private const string RegexUnitKnots = @"\s*(knots)\s*";

        /// <summary>
        /// Creates a new knots.
        /// </summary>
        /// <param name="value"></param>
        public Knots(double value)
            : base(value)
        {

        }

        #region Conversions

        /// <summary>
        /// Converts a given value to knots.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Knots(double value)
        {
            return new Knots(value);
        }

        /// <summary>
        /// Converts a given value to knots.
        /// </summary>
        /// <param name="meterPerSec"></param>
        /// <returns></returns>
        public static implicit operator Knots(MeterPerSecond meterPerSec)
        {
            return meterPerSec.Value / 0.5144444444444444;
        }

        /// <summary>
        /// Converts a given value to knots.
        /// </summary>
        /// <param name="knot"></param>
        /// <returns></returns>
        public static implicit operator Knots(KilometerPerHour knot)
        {
            return knot.Value / 1.85200;
        }

        /// <summary>
        /// Converts a given value to knots.
        /// </summary>
        /// <param name="mph"></param>
        /// <returns></returns>
        public static implicit operator Knots(MilesPerHour mph)
        {
            return mph.Value / 1.150779;
        }

        /// <summary>
        /// Divides a distance to a time resulting in a speed.
        /// </summary>
        /// <param name="knot"></param>
        /// <param name="hour"></param>
        /// <returns></returns>
        public static Kilometer operator *(Knots knot, Hour hour)
        {
            return ((KilometerPerHour)knot) * hour;
        }

        #endregion

        #region Parsing

        /// <summary>
        /// Tries to parse a string containing a meters per second value.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(string s, out Knots result)
        {
            result = null;
            double value;
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            { // the value is just a numeric value.
                result = new Knots(value);
                return true;
            }

            // do some more parsing work.
            Regex regex = new Regex("^" + Constants.RegexDecimalWhiteSpace + Knots.RegexUnitKnots + "$", RegexOptions.IgnoreCase);
            Match match = regex.Match(s);
            if (match.Success)
            {
                result = new Knots(double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture));
                return true;
            }
            return false;
        }

        #endregion

        /// <summary>
        /// Returns a description of this speed.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Value.ToString(CultureInfo.InvariantCulture) + "knots";
        }
    }
}
