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
using OsmSharp.IO.MemoryMappedFiles;

namespace OsmSharp.IO.MemoryMappedFiles.Accessors
{
    /// <summary>
    /// A memory mapped accessor that stores objects of a variable size in bytes.
    /// </summary>
    internal sealed class MemoryMappedAccessorVariable<T> : MemoryMappedAccessor<T>
    {
        /// <summary>
        /// Holds the read-from delegate.
        /// </summary>
        private MemoryMappedFile.ReadFromDelegate<T> _readFrom;

        /// <summary>
        /// Holds the write-to delegate.
        /// </summary>
        private MemoryMappedFile.WriteToDelegate<T> _writeTo;

        /// <summary>
        /// Creates a new memory mapped file.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="stream"></param>
        /// <param name="readFrom"></param>
        /// <param name="writeTo"></param>
        internal MemoryMappedAccessorVariable(MemoryMappedFile file, Stream stream, 
            MemoryMappedFile.ReadFromDelegate<T> readFrom, MemoryMappedFile.WriteToDelegate<T> writeTo)
            : base(file, stream, -1)
        {
            _readFrom = readFrom;
            _writeTo = writeTo;
        }
        
        /// <summary>
        /// Reads the structure at the given position.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="structure"></param>
        public sealed override void Read(long position, out T structure)
        {
            structure = _readFrom.Invoke(_stream, position);
        }

        /// <summary>
        /// Reads a number of structures starting at the given position adding them to the array the the given offset.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public sealed override int ReadArray(long position, T[] array, int offset, int count)
        {
            throw new NotSupportedException("Reading arrays of variable sized-structures is not suppored in a memory-mapped accessor.");
        }

        /// <summary>
        /// Writes the structure at the given position.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="structure"></param>
        /// <returns></returns>
        public sealed override long Write(long position, ref T structure)
        {
            return _writeTo.Invoke(_stream, position, structure);
        }

        /// <summary>
        /// Writes the structures from the array as the given position.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public sealed override long WriteArray(long position, T[] array, int offset, int count)
        {
            throw new NotSupportedException("Writing arrays of variable sized-structures is not suppored in a memory-mapped accessor.");
        }
    }
}