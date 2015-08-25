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

namespace OsmSharp.Math.Random
{
    /// <summary>
    /// A representation of generic random generator functions
    /// </summary>
    public interface IRandomGenerator
    {
        /// <summary>
        /// Generates a random integer
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        int Generate(int max);

        /// <summary>
        /// Generates a random double
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        double Generate(double max);

        /// <summary>
        /// Sets the seed of the static generator.
        /// </summary>
        /// <param name="buffer"></param>
        void Generate(byte[] buffer);

        /// <summary>
        /// Generates a random unicode string.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        string GenerateString(int length);
    }
}
