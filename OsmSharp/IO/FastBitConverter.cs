// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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

namespace OsmSharp.IO
{
    /// <summary>
    /// Converts base data types to an array of bytes, and an array of bytes to base data types.
    /// </summary>
    public static class FastBitConverter
    {
        /// <summary>
        /// Returns a 32-bit unsigned integer converted from four bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns></returns>
        public static uint ToUInt32(byte[] value, int startIndex)
        {
            return
                ((uint)value[startIndex]) |
                ((uint)value[startIndex + 1] << 8) |
                ((uint)value[startIndex + 2] << 16) |
                ((uint)value[startIndex + 3] << 24);
        }
    }
}