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

namespace OsmSharp.GeoCoding
{
    /// <summary>
    /// Describing the accuracy of a geocoding result.
    /// 
    /// This is the same as the Google "enum GGeoAddressAccuracy".
    /// </summary>
    public enum AccuracyEnum : int
    {
        /// <summary>
        /// Unknown location. 
        /// </summary>
        UnkownLocationLevel = 0,
        /// <summary>
        /// Country level accuracy. 
        /// </summary>
        CountryLevel = 1,
        /// <summary>
        /// Region (state, province, prefecture, etc.) level accuracy. 
        /// </summary>
        RegionLevel = 2,
        /// <summary>
        /// Sub-region (county, municipality, etc.) level accuracy.
        /// </summary>
        SubRegionLevel = 3,
        /// <summary>
        /// Town (city, village) level accuracy. 
        /// </summary>
        TownLevel = 4,
        /// <summary>
        /// 	 Post code (zip code) level accuracy.
        /// </summary>
        PostalCodeLevel = 5,
        /// <summary>
        /// Street level accuracy. 
        /// </summary>
        StreetLevel = 6,
        /// <summary>
        /// Intersection level accuracy. 
        /// </summary>
        IntersectionLevel = 7,
        /// <summary>
        /// Address level accuracy. 
        /// </summary>
        AddressLevel = 8,
        /// <summary>
        /// Premise (building name, property name, shopping center, etc.) level accuracy
        /// </summary>
        PremiseLevel = 9
    }
}