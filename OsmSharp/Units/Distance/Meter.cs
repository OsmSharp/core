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
    /// Represents a distance in meters.
    /// </summary>
    public class Meter : Unit
    {
        /// <summary>
        /// Creates a new meter.
        /// </summary>
        public Meter()
            :base(0.0d)
        {

        }

        private Meter(double value)
            : base(value)
        {

        }

        #region Conversions

        /// <summary>
        /// Converts the given value to meters.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Meter(double value)
        {
            return new Meter(value);
        }

        /// <summary>
        /// Converts the given value to meters.
        /// </summary>
        /// <param name="kilometer"></param>
        /// <returns></returns>
        public static implicit operator Meter(Kilometer kilometer)
        {
            return kilometer.Value * 1000d;
        }

        #endregion
        
        #region Division
        
        /// <summary>
        /// Divides the distance to a time into a speed.
        /// </summary>
        /// <param name="meter"></param>
        /// <param name="sec"></param>
        /// <returns></returns>
        public static MeterPerSecond operator /(Meter meter, Second sec)
        {
            return meter.Value / sec.Value;
        }

        /// <summary>
        /// Divides the distance to a speed into a time.
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public static Second operator /(Meter distance, MeterPerSecond speed)
        {
            return distance.Value / speed.Value;
        }

        /// <summary>
        /// Divides the distance to a speed into a time.
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public static Second operator /(Meter distance, KilometerPerHour speed)
        {
            Kilometer distance_km = distance;
            return distance_km / speed;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Adds two distances.
        /// </summary>
        /// <param name="meter1"></param>
        /// <param name="meter2"></param>
        /// <returns></returns>
        public static Meter operator +(Meter meter1, Meter meter2)
        {
            return meter1.Value + meter2.Value;
        }

        /// <summary>
        /// Subtracts two distances.
        /// </summary>
        /// <param name="meter1"></param>
        /// <param name="meter2"></param>
        /// <returns></returns>
        public static Meter operator -(Meter meter1, Meter meter2)
        {
            return meter1.Value - meter2.Value;
        }

        #endregion

        /// <summary>
        /// Returns a description of this meter.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Value.ToString() + "m";
        }
    }
}
