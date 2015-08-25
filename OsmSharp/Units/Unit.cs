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

namespace OsmSharp.Units
{
    /// <summary>
    /// Represents a value representing a unit value.
    /// </summary>
    public abstract class Unit
    {
        /// <summary>
        /// The value of the unit.
        /// </summary>
        private double _value;

        /// <summary>
        /// Creates a new valued unit.
        /// </summary>
        /// <param name="value"></param>
        protected Unit(double value)
        {
            _value = value;
        }

        /// <summary>
        /// Returns the value.
        /// </summary>
        public double Value
        {
            get
            {
                return _value;
            }
        }
    }
}
