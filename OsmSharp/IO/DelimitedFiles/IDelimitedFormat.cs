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

namespace OsmSharp.IO.DelimitedFiles
{
    /// <summary>
    /// Provides an interface to enable a custom format for the delimited files.
    /// </summary>
    public interface IDelimitedFormat
    {
        /// <summary>
        /// Converts a value in a given field to another value.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        string ConvertValue(int index, object value);

        /// <summary>
        /// Returns true if the column has to be exported.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool DoExport(int index);
    }
}
