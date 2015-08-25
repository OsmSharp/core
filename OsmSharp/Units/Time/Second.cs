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
using OsmSharp.Units.Distance;
using OsmSharp.Units.Speed;

namespace OsmSharp.Units.Time
{
    /// <summary>
    /// Represents a unit of time in seconds.
    /// </summary>
    public class Second : Unit
    {
        /// <summary>
        /// Creates a new second.
        /// </summary>
        public Second()
            :base(0.0d)
        {

        }

        private Second(double value)
            : base(value)
        {

        }

        #region Conversions

        /// <summary>
        /// Converts the given value to seconds.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Second(double value)
        {
            Second sec = new Second(value);
            return sec;
        }

        /// <summary>
        /// Converts the given value to seconds.
        /// </summary>
        /// <param name="timespan"></param>
        /// <returns></returns>
        public static implicit operator Second(TimeSpan timespan)
        {
            Second sec = new Second();
            sec = timespan.TotalMilliseconds / 1000.0d;
            return sec;
        }

        /// <summary>
        /// Converts the given value to seconds.
        /// </summary>
        /// <param name="hour"></param>
        /// <returns></returns>
        public static implicit operator Second(Hour hour)
        {
            Second sec = new Second();
            sec = hour.Value * 3600.0d;
            return sec;
        }

        #endregion
        
        /// <summary>
        /// Returns a description of this seconds.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Value.ToString() + "s";
        }
    }
}
