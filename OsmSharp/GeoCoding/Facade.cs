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
using System.Reflection;
using System.IO;

namespace OsmSharp.GeoCoding
{
    /// <summary>
    /// Facade to this assembly.
    /// </summary>
    public static class Facade
    {
        /// <summary>
        /// Holds a cache of all found geocoders.
        /// </summary>
        private static Dictionary<string, IGeoCoder> _coders;

        /// <summary>
        /// Registers a geocoder under it's given name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="code"></param>
        public static void RegisterGeoCoder(string name, IGeoCoder code)
        {
            // create and cache the coder class.
            if (_coders == null)
            {
                _coders = new Dictionary<string, IGeoCoder>();
            }

            _coders[name] = code;
        }

        /// <summary>
        /// Unregister a geocoder with the given name.
        /// </summary>
        /// <param name="name"></param>
        public static void UnregisterGeoCoder(string name)
        {
            // create and cache the coder class.
            if (_coders != null)
            {
                _coders.Remove(name);
            }
        }

        /// <summary>
        /// Geocodes the given address and returns the result.
        /// </summary>
        /// <param name="coderName"></param>
        /// <param name="country"></param>
        /// <param name="postalCode"></param>
        /// <param name="commune"></param>
        /// <param name="street"></param>
        /// <param name="houseNumber"></param>
        /// <returns></returns>
        public static IGeoCoderResult Code(
            string coderName,
            string country,
            string postalCode,
            string commune,
            string street,
            string houseNumber)
        {
            IGeoCoder coderInstance = null;

            // create and cache the coder class.
            if (_coders == null)
            {
                _coders = new Dictionary<string, IGeoCoder>();
            }
            if (!_coders.TryGetValue(coderName, out coderInstance))
            {
                throw new InvalidOperationException(string.Format(
                    "No geocoder registered with name: {0}", coderName));
            }

            return coderInstance.Code(
                country,
                postalCode,
                commune,
                street,
                houseNumber);
        }
    }
}
