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
    /// Random number generator.
    /// </summary>
    public class RandomGenerator : IRandomGenerator
    {
        private System.Random _random;

        /// <summary>
        /// Creates a new random generator.
        /// </summary>
        public RandomGenerator()
        {
            _random = new System.Random();
        }

        /// <summary>
        /// Creates a new random generator.
        /// </summary>
        public RandomGenerator(int seed)
        {
            _random = new System.Random(seed);
        }

        #region IRandomGenerator Members

        /// <summary>
        /// Generates a random number.
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public int Generate(int max)
        {
            return _random.Next(max);
        }

        /// <summary>
        /// Generates a random number.
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public double Generate(double max)
        {
            return _random.NextDouble() * max;
        }

        /// <summary>
        /// Fills the elements of a specified array of bytes with random numbers.
        /// </summary>
        /// <param name="buffer"></param>
        public void Generate(byte[] buffer)
        {
            _random.NextBytes(buffer);
        }

        /// <summary>
        /// Generates a random unicode string.
        /// </summary>
        /// <param name="length">The length of the string to generate.</param>
        /// <returns></returns>
        public string GenerateString(int length)
        {
            var str = new byte[length * 2];
            for (int i = 0; i < length * 2; i += 2)
            {
                int chr = this.Generate(0xD7FF);
                str[i + 1] = (byte)((chr & 0xFF00) >> 8);
                str[i] = (byte)(chr & 0xFF);
            }
            return Encoding.Unicode.GetString(str);
        }

        /// <summary>
        /// Generates an array of random size and with random values.
        /// </summary>
        /// <param name="maxSize"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public int[] GenerateArray(int maxSize, int max)
        {
            var array = new int[this.Generate(maxSize)];
            for(int i= 0; i < array.Length; i++)
            {
                array[i] = this.Generate(max);
            }
            return array;
        }

        #endregion
    }
}
