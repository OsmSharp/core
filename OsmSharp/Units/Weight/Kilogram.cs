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

namespace OsmSharp.Units.Weight
{
    /// <summary>
    /// Represents a weight in kilograms.
    /// </summary>
    public class Kilogram : Unit
    {
        /// <summary>
        /// Creates a new kilogram.
        /// </summary>
        public Kilogram()
            : base(0.0d)
        {

        }

        private Kilogram(double value)
            : base(value)
        {

        }

        #region Conversions

        /// <summary>
        /// Converts a value to kilograms.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Kilogram(double value)
        {
            return new Kilogram(value);
        }

        /// <summary>
        /// Converts a value to kilograms.
        /// </summary>
        /// <param name="gram"></param>
        /// <returns></returns>
        public static implicit operator Kilogram(Gram gram)
        {
            return gram.Value / 1000d;
        }

        #endregion

        /// <summary>
        /// Returns a description of this kilogram.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Value.ToString() + "Kg";
        }

    }
}
