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

using System.IO;

namespace OsmSharp.IO.MemoryMappedFiles
{
    /// <summary>
    /// A memory mapped file that is using a single stream.
    /// </summary>
    public class MemoryMappedStream : MemoryMappedFile
    {
        /// <summary>
        /// Holds the stream.
        /// </summary>
        private Stream _stream;

        /// <summary>
        /// Creates a new memory mapped stream using a memory stream.
        /// </summary>
        public MemoryMappedStream()
            :this(new MemoryStream())
        {

        }

        /// <summary>
        /// Creates a new memory mapped stream.
        /// </summary>
        /// <param name="stream">The stream to read/write.</param>
        public MemoryMappedStream(Stream stream)
        {
            _stream = stream;
        }

        /// <summary>
        /// Creates a new memory mapped file based on the given stream and the given size in bytes.
        /// </summary>
        /// <param name="position">The position to start at.</param>
        /// <param name="sizeInBytes">The size.</param>
        /// <returns></returns>
        protected override MemoryMappedAccessor<uint> DoCreateNewUInt32(long position, long sizeInBytes)
        {
            return new Accessors.MemoryMappedAccessorUInt32(this, new CappedStream(_stream, position, sizeInBytes));
        }

        /// <summary>
        /// Creates a new memory mapped file based on the given stream and the given size in bytes.
        /// </summary>
        /// <param name="position">The position to start at.</param>
        /// <param name="sizeInBytes">The size.</param>
        /// <returns></returns>
        protected override MemoryMappedAccessor<int> DoCreateNewInt32(long position, long sizeInBytes)
        {
            return new Accessors.MemoryMappedAccessorInt32(this, new CappedStream(_stream, position, sizeInBytes));
        }

        /// <summary>
        /// Creates a new memory mapped file based on the given stream and the given size in bytes.
        /// </summary>
        /// <param name="position">The position to start at.</param>
        /// <param name="sizeInBytes">The size.</param>
        /// <returns></returns>
        protected override MemoryMappedAccessor<float> DoCreateNewSingle(long position, long sizeInBytes)
        {
            return new Accessors.MemoryMappedAccessorSingle(this, new CappedStream(_stream, position, sizeInBytes));
        }

        /// <summary>
        /// Creates a new memory mapped file based on the given stream and the given size in bytes.
        /// </summary>
        /// <param name="position">The position to start at.</param>
        /// <param name="sizeInBytes">The size.</param>
        /// <returns></returns>
        protected override MemoryMappedAccessor<ulong> DoCreateNewUInt64(long position, long sizeInBytes)
        {
            return new Accessors.MemoryMappedAccessorUInt64(this, new CappedStream(_stream, position, sizeInBytes));
        }

        /// <summary>
        /// Creates a new memory mapped file based on the given stream and the given size in bytes.
        /// </summary>
        /// <param name="position">The position to start at.</param>
        /// <param name="sizeInBytes">The size.</param>
        /// <returns></returns>
        protected override MemoryMappedAccessor<long> DoCreateNewInt64(long position, long sizeInBytes)
        {
            return new Accessors.MemoryMappedAccessorInt64(this, new CappedStream(_stream, position, sizeInBytes));
        }

        /// <summary>
        /// Creates a new memory mapped file based on the given stream and the given size in bytes.
        /// </summary>
        /// <param name="position">The position to start at.</param>
        /// <param name="sizeInBytes">The size.</param>
        /// <param name="readFrom"></param>
        /// <param name="writeTo"></param>
        /// <returns></returns>
        protected override MemoryMappedAccessor<T> DoCreateVariable<T>(long position, long sizeInBytes, MemoryMappedFile.ReadFromDelegate<T> readFrom, MemoryMappedFile.WriteToDelegate<T> writeTo)
        {
            return new Accessors.MemoryMappedAccessorVariable<T>(this, new CappedStream(_stream, position, sizeInBytes), readFrom, writeTo);
        }
    }
}