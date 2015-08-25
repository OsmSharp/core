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
    /// Represents a speed.
    /// </summary>
    public abstract class Speed : Unit
    {                
        /// <summary>
        /// Creates a new speed.
        /// </summary>
        internal Speed(double value)
            :base(value)
        {

        }

        #region Parsers

        /// <summary>
        /// Tries to parse a string representing a speed. Assumes kilometers per hour by default, use explicit parsing methods for different behaviour.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParse(string s, out Speed result)
        {
            result = null;

            if (string.IsNullOrWhiteSpace(s))
                return false;

            // try a generic parse first, in this case assume kilometers per hour.
            double value;
            if (double.TryParse(s, out value))
            { // the value is just a numeric value.
                result = new KilometerPerHour(value);
                return true;
            }

            // try kilometers per hour.
            if (KilometerPerHour.TryParse(s, out result))
            { // succes!
                return true;
            }

            // try miles per hour.
            if (MilesPerHour.TryParse(s, out result))
            { // success!
                return true;
            }

            // try knots.
            if (Knots.TryParse(s, out result))
            { // success!
                return true;
            }

            // try meters per second.
            if (MeterPerSecond.TryParse(s, out result))
            { // success!
                return true;
            }
            return false;
        }

        #endregion Parsers
    }
}
