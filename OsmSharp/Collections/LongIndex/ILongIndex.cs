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

namespace OsmSharp.Collections.LongIndex
{
    /// <summary>
    /// Abstract representation of an index containing flags indexed by longs.
    /// </summary>
    public interface ILongIndex
    {
        /// <summary>
        /// Sets the flag at the given index.
        /// </summary>
        /// <param name="number"></param>
        void Add(long number);

        /// <summary>
        /// Sets all flags to false.
        /// </summary>
        void Clear();

        /// <summary>
        /// Returns the value of the flag at the given number.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        bool Contains(long number);

        /// <summary>
        /// Sets the flag at the given number to false.
        /// </summary>
        /// <param name="number"></param>
        void Remove(long number);

        /// <summary>
        /// Returns the number of flags that are set.
        /// </summary>
        long Count
        {
            get;
        }
    }
}