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

using OsmSharp.IO;
using OsmSharp.IO.MemoryMappedFiles;
using System;
using System.Collections.Generic;

namespace OsmSharp.Collections.Indexes.MemoryMapped
{
    /// <summary>
    /// A memory-mapped implementation of an index.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class MemoryMappedIndex<T> : IndexBase<T>, IDisposable
    {
        /// <summary>
        /// Holds the file to create the memory mapped accessors.
        /// </summary>
        private MemoryMappedFile _file;

        /// <summary>
        /// Holds the list of accessors, one for each range.
        /// </summary>
        private List<MemoryMappedAccessor<T>> _accessors;

        /// <summary>
        /// Holds the size of the data used per accessor.
        /// </summary>
        private List<long> _accessorSize;

        /// <summary>
        /// Holds the default file element size.
        /// </summary>
        public static long DefaultFileSize = (long)1024 * (long)1024 * (long)64;

        /// <summary>
        /// Holds the accessor size.
        /// </summary>
        private long _accessorSizeInBytes;

        /// <summary>
        /// Holds the readonly flag.
        /// </summary>
        private bool _readOnly;

        /// <summary>
        /// Holds the read-from delegate.
        /// </summary>
        private MemoryMappedFile.ReadFromDelegate<T> _readFrom;

        /// <summary>
        /// Holds the read-to delegate.
        /// </summary>
        private MemoryMappedFile.WriteToDelegate<T> _writeTo;

        /// <summary>
        /// Creates a new memory-mapped index.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="readFrom"></param>
        /// <param name="writeTo"></param>
        public MemoryMappedIndex(MemoryMappedFile file, MemoryMappedFile.ReadFromDelegate<T> readFrom,
            MemoryMappedFile.WriteToDelegate<T> writeTo)
            : this(file, readFrom, writeTo, DefaultFileSize, false)
        {

        }

        /// <summary>
        /// Creates a new readonly fixed-size memory-mapped index.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="readFrom"></param>
        /// <param name="writeTo"></param>
        /// <param name="sizeInBytes"></param>
        public MemoryMappedIndex(MemoryMappedFile file, MemoryMappedFile.ReadFromDelegate<T> readFrom,
            MemoryMappedFile.WriteToDelegate<T> writeTo, long sizeInBytes)
            : this(file, readFrom, writeTo, sizeInBytes, true)
        {

        }

        /// <summary>
        /// Creates a new memory-mapped index.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="readFrom"></param>
        /// <param name="writeTo"></param>
        /// <param name="accessorSizeInBytes"></param>
        /// <param name="readOnly"></param>
        public MemoryMappedIndex(MemoryMappedFile file, MemoryMappedFile.ReadFromDelegate<T> readFrom,
            MemoryMappedFile.WriteToDelegate<T> writeTo, long accessorSizeInBytes, bool readOnly)
        {
            if (file == null) { throw new ArgumentNullException("file"); }
            if (readFrom == null) { throw new ArgumentNullException("readFrom"); }
            if (writeTo == null) { throw new ArgumentNullException("writeTo"); }

            _file = file;
            _readFrom = readFrom;
            _writeTo = writeTo;
            _position = 0;
            _accessorSizeInBytes = accessorSizeInBytes;

            _accessors = new List<MemoryMappedAccessor<T>>();
            _accessorSize = new List<long>();
            _accessors.Add(this.CreateAccessor(_file, _accessorSizeInBytes));
            if (readOnly)
            { // index is readonly, meaning it already contains data on construction, meaning accessor size is non-zero.
                _accessorSize.Add(_accessorSizeInBytes);
            }
            else
            { // is writable.
                _accessorSize.Add(0);
            }

            _readOnly = readOnly;
        }

        /// <summary>
        /// Holds the current position.
        /// </summary>
        private long _position;

        /// <summary>
        /// Creates a variable-sized accessor.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="sizeInBytes"></param>
        /// <returns></returns>
        private MemoryMappedAccessor<T> CreateAccessor(MemoryMappedFile file, long sizeInBytes)
        {
            return file.CreateVariable<T>(sizeInBytes, _readFrom, _writeTo);
        }

        /// <summary>
        /// Returns true if this index is readonly.
        /// </summary>
        public override bool IsReadonly
        {
            get { return _readOnly; }
        }

        /// <summary>
        /// Returns true if this index is serializable.
        /// </summary>
        public override bool IsSerializable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Adds a new element to this index.
        /// </summary>
        /// <param name="element"></param>
        /// <returns>The id of the object.</returns>
        public override long Add(T element)
        {
            if (_readOnly) { throw new InvalidOperationException("This index is readonly."); }

            var id = _position;
            var accessorId = (int)System.Math.Floor(_position / _accessorSizeInBytes);
            if (accessorId == _accessors.Count)
            { // add new accessor.
                _accessors.Add(this.CreateAccessor(_file, _accessorSizeInBytes));
                _accessorSize.Add(0);
            }
            var accessor = _accessors[accessorId];
            var localPosition = _position - (accessorId * _accessorSizeInBytes);
            var size = accessor.Write(localPosition, ref element);
            if(size < 0)
            { // writing failed.
                _position = (_accessors.Count * _accessorSizeInBytes);
                id = _position;
                localPosition = 0;

                // add new accessor.
                _accessors.Add(this.CreateAccessor(_file, _accessorSizeInBytes));
                accessorId++;
                accessor = _accessors[accessorId];
                size = accessor.Write(localPosition, ref element);
                _accessorSize.Add((int)size);
            }
            else
            { // add the size to the current accessor size.
                _accessorSize[accessorId] = _accessorSize[accessorId] + (int)size;
            }
            _position = _position + size;
            return id;
        }

        /// <summary>
        /// Tries to get an element with the given id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="element">The element.</param>
        /// <returns>True if an element with the given id was found.</returns>
        public override bool TryGet(long id, out T element)
        {
            var accessorId = (int)System.Math.Floor(id / _accessorSizeInBytes);
            if (accessorId >= _accessors.Count)
            {
                element = default(T);
                return false;
            }
            var localPosition = (id - (_accessorSizeInBytes * accessorId));
            _accessors[accessorId].Read(localPosition, out element);
            return true;
        }

        /// <summary>
        /// Gets the element with the given id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The element.</returns>
        public override T Get(long id)
        {
            T element;
            if(!this.TryGet(id, out element))
            {
                throw new KeyNotFoundException();
            }
            return element;
        }

        /// <summary>
        /// Disposes of all native resources associated with this index.
        /// </summary>
        public void Dispose()
        {
            // dispose only the accessors, the file may still be in use.
            foreach (var accessor in _accessors)
            {
                accessor.Dispose();
            }
        }

        /// <summary>
        /// Serializes this index to the given stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public override long Serialize(System.IO.Stream stream)
        {
            // make room for size.
            stream.Seek(8, System.IO.SeekOrigin.Begin);

            // write data one after the other.
            long size = 0;
            for(int idx = 0; idx < _accessors.Count; idx++)
            {
                var accessorSize = _accessorSize[idx];
                _accessors[idx].CopyTo(stream, 0, (int)accessorSize);
                size = size + accessorSize;
            }

            stream.Seek(0, System.IO.SeekOrigin.Begin);
            stream.Write(BitConverter.GetBytes(size), 0, 8);

            stream.Seek(8 + size, System.IO.SeekOrigin.Begin);
            return 8 + size;
        }

        /// <summary>
        /// Deserializes an index from the given stream.
        /// </summary>
        /// <param name="stream">The stream to read from. Reading will start at position 0.</param>
        /// <param name="readFrom"></param>
        /// <param name="writeTo"></param>
        /// <param name="copy">Flag to make an in-memory copy.</param>
        /// <returns></returns>
        public static MemoryMappedIndex<T> Deserialize(System.IO.Stream stream, MemoryMappedFile.ReadFromDelegate<T> readFrom,
            MemoryMappedFile.WriteToDelegate<T> writeTo, bool copy)
        {
            long size;
            return MemoryMappedIndex<T>.Deserialize(stream, readFrom, writeTo, copy, out size);
        }

        /// <summary>
        /// Deserializes an index from the given stream.
        /// </summary>
        /// <param name="stream">The stream to read from. Reading will start at position 0.</param>
        /// <param name="readFrom"></param>
        /// <param name="writeTo"></param>
        /// <param name="copy">Flag to make an in-memory copy.</param>
        /// <param name="size">The total size in bytes of the deserialized data.</param>
        /// <returns></returns>
        public static MemoryMappedIndex<T> Deserialize(System.IO.Stream stream, MemoryMappedFile.ReadFromDelegate<T> readFrom,
            MemoryMappedFile.WriteToDelegate<T> writeTo, bool copy, out long size)
        {
            if (copy) { throw new NotSupportedException("Deserializing a memory mapped index with copy option is not supported."); }
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            var longBytes = new byte[8];
            stream.Read(longBytes, 0, 8);
            size = BitConverter.ToInt64(longBytes, 0) + 8;

            var file = new MemoryMappedStream(new LimitedStream(stream));
            return new MemoryMappedIndex<T>(file, readFrom, writeTo, size - 8);
        }
    }
}