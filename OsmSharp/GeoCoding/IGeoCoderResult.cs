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
    /// Describes what a geo coder result should be.
    /// </summary>
    public interface IGeoCoderResult
    {
        /// <summary>
        /// The latitude result.
        /// </summary>
        double Latitude
        {
            get;
        }

        /// <summary>
        /// The longitude result.
        /// </summary>
        double Longitude
        {
            get;
        }

        /// <summary>
        /// Text value of geocoder
        /// </summary>
        string Text { get; }

        /// <summary>
        /// The accuracy of the result.
        /// </summary>
        AccuracyEnum Accuracy
        {
            get;
        }
    }
}
