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
using OsmSharp.Math.Structures.StringTrees;
using OsmSharp.Math.Geo;

namespace OsmSharp.GeoCoding.Memory
{
    /// <summary>
    /// An in-memory geocoder.
    /// </summary>
    public class InMemoryGeoCoder : IGeoCoder
    {
        private IndexPostalCodes _index;

        /// <summary>
        /// Creates this geocoder.
        /// </summary>
        public InMemoryGeoCoder()
        {
            _index = new IndexPostalCodes();
        }

        /// <summary>
        /// Adds a new entry.
        /// </summary>
        /// <param name="country"></param>
        /// <param name="postalCode"></param>
        /// <param name="commune"></param>
        /// <param name="street"></param>
        /// <param name="houseNumber"></param>
        /// <param name="value"></param>
        public void Add(string country, string postalCode, string commune, string street, 
            string houseNumber, GeoCoordinate value)
        {
            IndexCommunes communes = _index.SearchExact(postalCode);
            if (communes == null)
            {
                communes = new IndexCommunes();
                _index.Add(postalCode, communes);
            }
            IndexStreets streets = communes.SearchExact(commune);
            if (streets == null)
            {
                streets = new IndexStreets();
                communes.Add(commune, streets);
            }
            IndexHouseNumbers numbers = streets.SearchExact(street);
            if (numbers == null)
            {
                numbers = new IndexHouseNumbers();
                streets.Add(street, numbers);
            }
            numbers.Add(houseNumber, value);
        }

        /// <summary>
        /// Does the actual geocoding.
        /// </summary>
        /// <param name="country"></param>
        /// <param name="postalCode"></param>
        /// <param name="commune"></param>
        /// <param name="street"></param>
        /// <param name="houseNumber"></param>
        /// <returns></returns>
        public IGeoCoderResult Code(string country, string postalCode, 
            string commune, string street, string houseNumber)
        {
            throw new NotImplementedException();
        }
    }
}
