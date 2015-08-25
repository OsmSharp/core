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

using System;
using System.IO;

namespace OsmSharp.IO.MemoryMappedFiles.Accessors
{
    /// <summary>
    /// A memory mapped accessor that stores uints.
    /// </summary>
    internal sealed class MemoryMappedAccessorInt32 : MemoryMappedAccessor<int>
    {
        /// <summary>
        /// Creates a new memory mapped file.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="stream"></param>
        internal MemoryMappedAccessorInt32(MemoryMappedFile file, Stream stream)
            : base(file, stream, 4)
        {

        }

        /// <summary>
        /// Reads from the buffer at the given position.
        /// </summary>
        /// <param name="position">The position to read from.</param>
        /// <returns></returns>
        protected sealed override int ReadFrom(int position)
        {
            return BitConverter.ToInt32(_buffer, position);
        }

        /// <summary>
        /// Writes to the stream.
        /// </summary>
        /// <param name="structure"></param>
        protected sealed override long WriteTo(int structure)
        {
            _stream.Write(BitConverter.GetBytes(structure), 0, _elementSize);
            return _elementSize;
        }
    }
}