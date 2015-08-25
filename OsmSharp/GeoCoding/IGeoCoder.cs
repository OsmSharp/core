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
using OsmSharp.GeoCoding;

namespace OsmSharp.GeoCoding
{
    /// <summary>
    /// Describes a generic geo coder.
    /// </summary>
    public interface IGeoCoder
    {
        /// <summary>
        /// Does the actual geocoding.
        /// </summary>
        /// <param name="country"></param>
        /// <param name="postalCode"></param>
        /// <param name="commune"></param>
        /// <param name="street"></param>
        /// <param name="houseNumber"></param>
        /// <returns></returns>
        IGeoCoderResult Code(string country,
            string postalCode,
            string commune,
            string street,
            string houseNumber);
    }
}
