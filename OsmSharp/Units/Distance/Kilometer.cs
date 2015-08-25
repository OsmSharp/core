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
using OsmSharp.Units.Time;
using OsmSharp.Units.Speed;

namespace OsmSharp.Units.Distance
{
    /// <summary>
    /// Represents a distance in kilometers.
    /// </summary>
    public class Kilometer : Unit
    {
        /// <summary>
        /// Creates a new kilometer.
        /// </summary>
        public Kilometer()
            : base(0.0d)
        {

        }

        private Kilometer(double value)
            : base(value)
        {

        }

        #region Conversions

        /// <summary>
        /// Converts the given value to kilometers.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Kilometer(double value)
        {
            return new Kilometer(value);
        }

        /// <summary>
        /// Converts the given value to kilometers.
        /// </summary>
        /// <param name="meter"></param>
        /// <returns></returns>
        public static implicit operator Kilometer(Meter meter)
        {
            return meter.Value / 1000d;
        }

        #endregion
        
        #region Division

        /// <summary>
        /// Divides a distance to a speed resulting in a time.
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public static Hour operator /(Kilometer distance, KilometerPerHour speed)
        {
            return distance.Value / speed.Value;
        }

        /// <summary>
        /// Divides a distance to a time resulting in a speed.
        /// </summary>
        /// <param name="kilometer"></param>
        /// <param name="hour"></param>
        /// <returns></returns>
        public static KilometerPerHour operator /(Kilometer kilometer, Hour hour)
        {
            return kilometer.Value / hour.Value;
        }

        #endregion

        /// <summary>
        /// Returns a description of this kilometer.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Value.ToString() + "Km";
        }
    }
}