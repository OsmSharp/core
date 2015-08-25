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

using System.Globalization;
using System.Text.RegularExpressions;

namespace OsmSharp.Units.Speed
{
    /// <summary>
    /// Represents a speed in miles per hour.
    /// </summary>
    public class MilesPerHour : Speed
    {
        private const string RegexUnitMilesPerHour = @"\s*(mph)\s*";
        
        /// <summary>
        /// Creates a new miles per hour object initialized to zero.
        /// </summary>
        private MilesPerHour()
            :base(0.0d)
        {

        }

        /// <summary>
        /// Creates a new miles per hour.
        /// </summary>
        /// <param name="value"></param>
        public MilesPerHour(double value)
            : base(value)
        {

        }

        #region Conversions

        /// <summary>
        /// Converts a given value to miles per hour.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator MilesPerHour(double value)
        {
            return new MilesPerHour(value);
        }

        /// <summary>
        /// Converts a given value to miles per hour.
        /// </summary>
        /// <param name="kph"></param>
        /// <returns></returns>
        public static implicit operator MilesPerHour(KilometerPerHour kph)
        {
            return kph.Value * 0.621371192;
        }

        #endregion

        #region Parsing

        /// <summary>
        /// Tries to parse a string containing a miles per hour value.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(string s, out MilesPerHour result)
        {
            result = null;
            double value;
            if (double.TryParse(s, out value))
            { // the value is just a numeric value.
                result = new MilesPerHour(value);
                return true;
            }

            // do some more parsing work.
            Regex regex = new Regex("^" + Constants.RegexDecimalWhiteSpace + MilesPerHour.RegexUnitMilesPerHour + "$", RegexOptions.IgnoreCase);
            Match match = regex.Match(s);
            if (match.Success)
            {
                result = new MilesPerHour(double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture));
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
            return this.Value.ToString() + "mph";
        }
    }
}
