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

using OsmSharp.Math.Geo.Simple;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.IO.MemoryMappedFiles
{
    /// <summary>
    /// Represents a memory mapped file.
    /// </summary>
    public abstract class MemoryMappedFile : IDisposable
    {
        /// <summary>
        /// Holds all acessors generated for this file.
        /// </summary>
        private List<IDisposable> _accessors;

        /// <summary>
        /// Holds the next position of a new empty accessor.
        /// </summary>
        private long _nextPosition;

        /// <summary>
        /// Creates a new memory mapped file.
        /// </summary>
        public MemoryMappedFile()
        {
            _accessors = new List<IDisposable>();
            _nextPosition = 0;
        }

        /// <summary>
        /// Creates a new memory mapped accessor for a given part of this file with given size in bytes and the start position.
        /// </summary>
        /// <param name="position">The position to start this accessor at.</param>
        /// <param name="sizeInBytes">The size of this accessor.</param>
        /// <returns></returns>
        public MemoryMappedAccessor<uint> CreateUInt32(long position, long sizeInBytes)
        {
            var accessor = this.DoCreateNewUInt32(position, sizeInBytes);
            _accessors.Add(accessor);

            var nextPosition = position + sizeInBytes;
            if(nextPosition > _nextPosition)
            {
                _nextPosition = nextPosition;
            }

            return accessor;
        }

        /// <summary>
        /// Creates a new empty memory mapped accessor with given size in bytes.
        /// </summary>
        /// <param name="sizeInBytes">The size of this accessor.</param>
        /// <returns></returns>
        public MemoryMappedAccessor<uint> CreateUInt32(long sizeInBytes)
        {
            var accessor = this.DoCreateNewUInt32(_nextPosition, sizeInBytes);
            _accessors.Add(accessor);

            _nextPosition = _nextPosition + sizeInBytes;

            return accessor;
        }

        /// <summary>
        /// Creates a new memory mapped file based on the given stream and the given size in bytes.
        /// </summary>
        /// <param name="position">The position to start at.</param>
        /// <param name="sizeInByte">The size.</param>
        /// <returns></returns>
        protected abstract MemoryMappedAccessor<int> DoCreateNewInt32(long position, long sizeInByte);

        /// <summary>
        /// Creates a new empty memory mapped accessor with given size in bytes.
        /// </summary>
        /// <param name="sizeInBytes">The size of this accessor.</param>
        /// <returns></returns>
        public MemoryMappedAccessor<int> CreateInt32(long sizeInBytes)
        {
            var accessor = this.DoCreateNewInt32(_nextPosition, sizeInBytes);
            _accessors.Add(accessor);

            _nextPosition = _nextPosition + sizeInBytes;

            return accessor;
        }

        /// <summary>
        /// Creates a new memory mapped file based on the given stream and the given size in bytes.
        /// </summary>
        /// <param name="position">The position to start at.</param>
        /// <param name="sizeInByte">The size.</param>
        /// <returns></returns>
        protected abstract MemoryMappedAccessor<uint> DoCreateNewUInt32(long position, long sizeInByte);

        /// <summary>
        /// Creates a new memory mapped accessor for a given part of this file with given size in bytes and the start position.
        /// </summary>
        /// <param name="position">The position to start this accessor at.</param>
        /// <param name="sizeInBytes">The size of this accessor.</param>
        /// <returns></returns>
        public MemoryMappedAccessor<float> CreateSingle(long position, long sizeInBytes)
        {
            var accessor = this.DoCreateNewSingle(position, sizeInBytes);
            _accessors.Add(accessor);

            var nextPosition = position + sizeInBytes;
            if (nextPosition > _nextPosition)
            {
                _nextPosition = nextPosition;
            }

            return accessor;
        }

        /// <summary>
        /// Creates a new empty memory mapped accessor with given size in bytes.
        /// </summary>
        /// <param name="sizeInBytes">The size of this accessor.</param>
        /// <returns></returns>
        public MemoryMappedAccessor<float> CreateSingle(long sizeInBytes)
        {
            var accessor = this.DoCreateNewSingle(_nextPosition, sizeInBytes);
            _accessors.Add(accessor);

            _nextPosition = _nextPosition + sizeInBytes;

            return accessor;
        }

        /// <summary>
        /// Creates a new memory mapped file based on the given stream and the given size in bytes.
        /// </summary>
        /// <param name="position">The position to start at.</param>
        /// <param name="sizeInByte">The size.</param>
        /// <returns></returns>
        protected abstract MemoryMappedAccessor<float> DoCreateNewSingle(long position, long sizeInByte);

        /// <summary>
        /// Creates a new memory mapped accessor for a given part of this file with given size in bytes and the start position.
        /// </summary>
        /// <param name="position">The position to start this accessor at.</param>
        /// <param name="sizeInBytes">The size of this accessor.</param>
        /// <returns></returns>
        public MemoryMappedAccessor<ulong> CreateUInt64(long position, long sizeInBytes)
        {
            var accessor = this.DoCreateNewUInt64(position, sizeInBytes);
            _accessors.Add(accessor);

            var nextPosition = position + sizeInBytes;
            if (nextPosition > _nextPosition)
            {
                _nextPosition = nextPosition;
            }

            return accessor;
        }

        /// <summary>
        /// Creates a new empty memory mapped accessor with given size in bytes.
        /// </summary>
        /// <param name="sizeInBytes">The size of this accessor.</param>
        /// <returns></returns>
        public MemoryMappedAccessor<ulong> CreateUInt64(long sizeInBytes)
        {
            var accessor = this.DoCreateNewUInt64(_nextPosition, sizeInBytes);
            _accessors.Add(accessor);

            _nextPosition = _nextPosition + sizeInBytes;

            return accessor;
        }

        /// <summary>
        /// Creates a new memory mapped file based on the given stream and the given size in bytes.
        /// </summary>
        /// <param name="position">The position to start at.</param>
        /// <param name="sizeInByte">The size.</param>
        /// <returns></returns>
        protected abstract MemoryMappedAccessor<ulong> DoCreateNewUInt64(long position, long sizeInByte);

        /// <summary>
        /// Creates a new empty memory mapped accessor with given size in bytes.
        /// </summary>
        /// <param name="sizeInBytes">The size of this accessor.</param>
        /// <returns></returns>
        public MemoryMappedAccessor<long> CreateInt64(long sizeInBytes)
        {
            var accessor = this.DoCreateNewInt64(_nextPosition, sizeInBytes);
            _accessors.Add(accessor);

            _nextPosition = _nextPosition + sizeInBytes;

            return accessor;
        }

        /// <summary>
        /// Creates a new memory mapped file based on the given stream and the given size in bytes.
        /// </summary>
        /// <param name="position">The position to start at.</param>
        /// <param name="sizeInByte">The size.</param>
        /// <returns></returns>
        protected abstract MemoryMappedAccessor<long> DoCreateNewInt64(long position, long sizeInByte);

        /// <summary>
        /// A delegate to facilitate reading a variable-sized object.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="position">The position to start reading at.</param>
        /// <returns></returns>
        public delegate T ReadFromDelegate<T>(Stream stream, long position);

        /// <summary>
        /// A delegate to facilitate writing a variable-sized object.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="position">The position to start reading at.</param>
        /// <param name="structure">The structure to write.</param>
        /// <returns></returns>
        public delegate long WriteToDelegate<T>(Stream stream, long position, T structure);

        /// <summary>
        /// Creates a new empty memory mapped accessor with given size in bytes.
        /// </summary>
        /// <param name="sizeInBytes">The size of this accessor.</param>
        /// <param name="readFrom">The delegate to read a structure.</param>
        /// <param name="writeTo">The delegate to write a structure.</param>
        /// <returns></returns>
        public MemoryMappedAccessor<T> CreateVariable<T>(long sizeInBytes, 
            ReadFromDelegate<T> readFrom, WriteToDelegate<T> writeTo)
        {
            var accessor = this.DoCreateVariable<T>(_nextPosition, sizeInBytes, readFrom, writeTo);
            _accessors.Add(accessor);

            _nextPosition = _nextPosition + sizeInBytes;

            return accessor;
        }

        /// <summary>
        /// Creates a new memory mapped file based on the given stream and the given size in bytes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_nextPosition"></param>
        /// <param name="sizeInBytes"></param>
        /// <param name="readFrom"></param>
        /// <param name="writeTo"></param>
        /// <returns></returns>
        protected abstract MemoryMappedAccessor<T> DoCreateVariable<T>(long _nextPosition, long sizeInBytes, ReadFromDelegate<T> readFrom, WriteToDelegate<T> writeTo);

        /// <summary>
        /// Notifies this factory that the given file was already disposed. This given the opportunity to dispose of files without disposing the entire factory.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileToDispose"></param>
        internal void Disposed<T>(MemoryMappedAccessor<T> fileToDispose)
        {
            _accessors.Remove(fileToDispose);
        }

        /// <summary>
        /// Disposes of all resources associated with this files.
        /// </summary>
        public virtual void Dispose()
        {
            while (_accessors.Count > 0)
            {
                _accessors[0].Dispose();
            }
            _accessors.Clear();
        }
    }
}