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

using OsmSharp.Collections.Arrays;
using OsmSharp.Collections.Arrays.MemoryMapped;
using OsmSharp.Collections.Sorting;
using OsmSharp.IO;
using OsmSharp.IO.MemoryMappedFiles;
using OsmSharp.Math.Geo.Simple;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Collections.Coordinates.Collections
{
    /// <summary>
    /// Represents a write-only large coordinate index.
    /// </summary>
    public class HugeCoordinateCollectionIndex : IDisposable
    {
        private readonly int MAX_COLLECTION_SIZE = ushort.MaxValue;
        private readonly int ESTIMATED_SIZE = 5;
        private readonly ulong NULL = ulong.MaxValue;
        private readonly ulong NOT_SET = ulong.MaxValue - 1;
        private readonly HugeArrayBase<ulong> _index;
        private readonly HugeArrayBase<float> _coordinates;

        /// <summary>
        /// Creates a new huge coordinate index.
        /// </summary>
        /// <param name="size">The original size.</param>
        public HugeCoordinateCollectionIndex(long size)
        {
            _index = new HugeArray<ulong>(size);
            _coordinates = new HugeArray<float>(size * 2 * ESTIMATED_SIZE);

            for (long idx = 0; idx < _index.Length; idx++)
            {
                _index[idx] = NOT_SET;
            }

            for (long idx = 0; idx < _coordinates.Length; idx++)
            {
                _coordinates[idx] = float.MinValue;
            }
        }

        /// <summary>
        /// Creates a huge coordinate index based on exisiting data.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="index">The index array.</param>
        /// <param name="coordinates">The coordinates array.</param>
        public HugeCoordinateCollectionIndex(long size, 
            HugeArrayBase<ulong> index, HugeArrayBase<float> coordinates)
        {
            _nextIdx = size;
            _index = index;
            _coordinates = coordinates;
        }

        /// <summary>
        /// Creates a new, memory mapped, huge coordinate index.
        /// </summary>
        /// <param name="file">The memory mapped file.</param>
        /// <param name="size">The original size.</param>
        public HugeCoordinateCollectionIndex(MemoryMappedFile file, long size)
        {
            _index = new MemoryMappedHugeArrayUInt64(file, size);
            _coordinates = new MemoryMappedHugeArraySingle(file, size * 2 * ESTIMATED_SIZE);

            for (long idx = 0; idx < _index.Length; idx++)
            {
                _index[idx] = 0;
            }
        }

        /// <summary>
        /// Returns the collection with the given id.
        /// </summary>
        /// <returns>True if the collection was found.</returns>
        public bool Remove(long id)
        {
            long index, size;
            if(this.TryGetIndexAndSize(id, out index, out size))
            {
                _index[id] = NOT_SET;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds the coordinate collection at the given id.
        /// </summary>
        public void Add(long id, ICoordinateCollection coordinates)
        {
            long index, size;
            if (this.TryGetIndexAndSize(id, out index, out size))
            {
                throw new InvalidOperationException("Item with same key already exists.");
            }
            _index[id] = this.DoAdd(coordinates);

            if (_nextId <= id)
            { // update max id.
                _nextId = id + 1;
            }
        }

        /// <summary>
        /// Returns the coordinate collection at the given id.
        /// </summary>
        /// <returns></returns>
        public bool TryGet(long id, out ICoordinateCollection coordinates)
        {
            long index, size;
            if (this.TryGetIndexAndSize(id, out index, out size))
            {
                if(size >= 0)
                {
                    coordinates = new HugeCoordinateCollection(_coordinates, index, size);
                    return true;
                }
                coordinates = null;
                return true;
            }
            coordinates = null;
            return false;
        }

        /// <summary>
        /// Quickly switches two elements.
        /// </summary>
        public void Switch(long id1, long id2)
        {
            var data = _index[id1];
            _index[id1] = _index[id2];
            _index[id2] = data;
        }

        /// <summary>
        /// Gets or sets the coordinate collection at the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ICoordinateCollection this[long id]
        {
            get
            {
                ICoordinateCollection coordinates;
                if(this.TryGet(id, out coordinates))
                {
                    return coordinates;
                }
                throw new KeyNotFoundException();
            }
            set
            {
                long index, size;
                if (this.TryGetIndexAndSize(id, out index, out size))
                {
                    if (value == null ||
                        value.Count <= size)
                    { // overwite coordinates.
                        _index[id] = this.DoSet(index, value);
                    }
                    else
                    { // add new and waste space.
                        _index[id] = this.DoAdd(value);
                    }
                }
                else
                { // add new coordinates.
                    _index[id] = this.DoAdd(value);
                }

                if (_nextId <= id)
                { // update max id.
                    _nextId = id + 1;
                }
            }
        }

        /// <summary>
        /// Resizes this array.
        /// </summary>
        public void Resize(long size)
        {
            _index.Resize(size);

            if(_nextId > size)
            { 
                _nextId = size;
            }
        }

        /// <summary>
        /// Returns the length of the coordinates index.
        /// </summary>
        public long LengthIndex
        {
            get
            {
                return _index.Length;
            }
        }

        /// <summary>
        /// Returns the length of the coordinates.
        /// </summary>
        public long LengthCoordinates
        {
            get
            {
                return _coordinates.Length;
            }
        }

        #region Helper Methods

        private long _nextIdx = 0;
        private long _nextId = 0;

        /// <summary>
        /// Increases the size of the coordinates array.
        /// To be used when the ESTIMATED_SIZE has underestimated to average coordinate collection size.
        /// </summary>
        private void IncreaseCoordinates()
        {
            _coordinates.Resize(_coordinates.Length + (1 << 20));
        }

        /// <summary>
        /// Tries to get the index and the size (if any).
        /// </summary>
        /// <returns></returns>
        private bool TryGetIndexAndSize(long id, out long index, out long size)
        {
            if (id >= _index.Length)
            {
                index = 0;
                size = 0;
                return false;
            }
            var data = _index[id];
            if(data == NOT_SET)
            { // no data.
                index = -1;
                size = -1;
                return false;
            }
            if(data == NULL)
            { // in this case data is null.
                index = 0;
                size = -1;
                return true;
            }
            if(data == 0)
            { // in this case collection is empty.
                index = 0;
                size = 0;
                return true;
            }
            index = ((long)(data / (ulong)MAX_COLLECTION_SIZE));
            size = (long)(data % (ulong)MAX_COLLECTION_SIZE);
            return true;
        }

        /// <summary>
        /// Sets the coordinates starting at the given index.
        /// </summary>
        private ulong DoSet(long index, ICoordinateCollection coordinates)
        {
            if (coordinates == null)
            { // just return the null-pointer if null.
                return NULL;
            }

            var id = (ulong)(index * MAX_COLLECTION_SIZE) + (ulong)coordinates.Count;
            coordinates.Reset();
            while(coordinates.MoveNext())
            {
                _coordinates[index * 2] = coordinates.Latitude;
                _coordinates[index * 2 + 1] = coordinates.Longitude;
                index++;
            }
            return id;
        }

        /// <summary>
        /// Adds the new coordinates at the end of the current coordinates.
        /// </summary>
        /// <returns></returns>
        private ulong DoAdd(ICoordinateCollection coordinates)
        {
            if(coordinates == null)
            { // just return the null-pointer if null.
                return NULL;
            }

            var id = (ulong)(_nextIdx * MAX_COLLECTION_SIZE) + (ulong)coordinates.Count;
            coordinates.Reset();
            while(coordinates.MoveNext())
            {
                if (_coordinates.Length <= (_nextIdx * 2) + 1)
                { // make sure they fit!
                    this.IncreaseCoordinates();
                }
                _coordinates[(_nextIdx * 2)] = coordinates.Latitude;
                _coordinates[(_nextIdx * 2) + 1] = coordinates.Longitude;
                _nextIdx = _nextIdx + 1;
            }
            return id;
        }

        #endregion

        /// <summary>
        /// Represents the huge coordinate collection.
        /// </summary>
        private class HugeCoordinateCollection : ICoordinateCollection
        {
            private readonly long _pointer;
            private readonly long _count;
            private readonly HugeArrayBase<float> _coordinates;
            private readonly bool _reverse;

            /// <summary>
            /// Creates a new huge coordinate collection.
            /// </summary>
            public HugeCoordinateCollection(HugeArrayBase<float> coordinates, long startIdx, long size)
                : this(coordinates, startIdx, size, false)
            {

            }

            /// <summary>
            /// Creates a new coordinate collection.
            /// </summary>
            public HugeCoordinateCollection(HugeArrayBase<float> coordinates, long index, long size, bool reverse)
            {
                _pointer = index * 2;
                _count = size * 2;
                _coordinates = coordinates;
                _reverse = reverse;
            }

            private long _currentPointer = -2;

            /// <summary>
            /// Returns the count.
            /// </summary>
            public int Count
            {
                get { return (int)(_count / 2); }
            }

            /// <summary>
            /// Returns the enumerator.
            /// </summary>
            /// <returns></returns>
            public IEnumerator<ICoordinate> GetEnumerator()
            {
                this.Reset();
                return this;
            }

            /// <summary>
            /// Returns the enumerator.
            /// </summary>
            /// <returns></returns>
            IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                this.Reset();
                return this;
            }

            /// <summary>
            /// Returns the current.
            /// </summary>
            public ICoordinate Current
            {
                get
                {
                    return new GeoCoordinateSimple()
                    {
                        Latitude = this.Latitude,
                        Longitude = this.Longitude
                    };
                }
            }

            /// <summary>
            /// Returns the current.
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    return new GeoCoordinateSimple()
                    {
                        Latitude = this.Latitude,
                        Longitude = this.Longitude
                    };
                }
            }

            /// <summary>
            /// Moves to the next coordinate.
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                if(_reverse)
                {
                    _currentPointer = _currentPointer - 2;
                    return (_currentPointer - _pointer) >= 0;
                }
                else
                {
                    _currentPointer = _currentPointer + 2;
                    return (_currentPointer - _pointer) < _count;
                }
            }

            /// <summary>
            /// Resets the current enumerator.
            /// </summary>
            public void Reset()
            {
                if(_reverse)
                {
                    _currentPointer = _pointer + _count;
                }
                else
                {
                    _currentPointer = _pointer - 2;
                }
            }

            /// <summary>
            /// Disposes all resources associated with this enumerable.
            /// </summary>
            public void Dispose()
            {

            }

            /// <summary>
            /// Returns the latitude.
            /// </summary>
            public float Latitude
            {
                get { return _coordinates[_currentPointer]; }
            }

            /// <summary>
            /// Returns the longitude.
            /// </summary>
            public float Longitude
            {
                get { return _coordinates[_currentPointer + 1]; }
            }

            /// <summary>
            /// Returns the reverse collection.
            /// </summary>
            /// <returns></returns>
            public ICoordinateCollection Reverse()
            {
                return new HugeCoordinateCollection(_coordinates, _pointer, _count, !_reverse);
            }
        }

        /// <summary>
        /// Trims the size of the index and of the coordinate array to the exact size needed.
        /// </summary>
        /// <returns></returns>
        public void Trim()
        {
            _index.Resize(_nextId);
            _coordinates.Resize(_nextIdx * 2);
        }

        /// <summary>
        /// Moves data around to obtain the smallest possible datastructure still containing all data.
        /// </summary>
        public void Compress()
        {
            // trim first.
            this.Trim();

            // build a list of all vertices sorted by their first position.
            var sortedVertices = new HugeArray<uint>(_index.Length);
            for (uint i = 0; i < sortedVertices.Length; i++)
            {
                sortedVertices[i] = i;
            }

            // sort vertices and coordinates.
            QuickSort.Sort((i) => ((long)(_index[sortedVertices[i]] / (ulong)MAX_COLLECTION_SIZE)) * 2, (i, j) =>
            {
                var tempRef = sortedVertices[i];
                sortedVertices[i] = sortedVertices[j];
                sortedVertices[j] = tempRef;
            }, 0, _index.Length - 1);

            // move data down.
            uint pointer = 0;
            for (uint i = 0; i < sortedVertices.Length; i++)
            {
                long index, size;
                if(this.TryGetIndexAndSize(sortedVertices[i], out index, out size))
                {
                    if(size > 0)
                    {
                        var newIndex = pointer / 2;
                        for(var c = 0; c < size; c++)
                        {
                            _coordinates[pointer] = _coordinates[(index + c) * 2];
                            pointer++;
                            _coordinates[pointer] = _coordinates[(index + c) * 2 + 1];
                            pointer++;
                        }

                        _index[sortedVertices[i]] = (ulong)(newIndex * MAX_COLLECTION_SIZE) + (ulong)size;
                    }
                }
            }
            _nextIdx = pointer / 2;

            // trim again, something may have changed.
            this.Trim();
        }

        /// <summary>
        /// Serializes this huge collection index to the given stream.
        /// </summary>
        /// <param name="stream"></param>
        public long Serialize(Stream stream)
        {
            // first trim to minimize the amount of useless data store.
            this.Compress();

            // start writing.
            long indexSize = _index.Length;
            long coordinatesCount = _coordinates.Length;
            
            long position = 0;
            stream.Write(BitConverter.GetBytes(indexSize), 0, 8); // write the actual index size.
            position = position + 8;
            stream.Write(BitConverter.GetBytes(coordinatesCount), 0, 8); // write the actual number of coordinates.
            position = position + 8;

            // write in this order: index, shapes.
            using (var file = new MemoryMappedStream(new LimitedStream(stream)))
            {
                if (indexSize > 0)
                { // write index.
                    var indexArray = new MemoryMappedHugeArrayUInt64(file, indexSize, indexSize, 1024);
                    indexArray.CopyFrom(_index, indexSize);
                    indexArray.Dispose();
                    position = position + (indexSize * 8);
                }

                if(coordinatesCount > 0)
                { // write coordinates.
                    var coordinatesArray = new MemoryMappedHugeArraySingle(file, coordinatesCount, coordinatesCount, 1024);
                    coordinatesArray.CopyFrom(_coordinates);
                    coordinatesArray.Dispose();
                    position = position + (coordinatesCount * 4);
                }
            }

            return position;
        }

        /// <summary>
        /// Deserializes a huge collection index from the given stream.
        /// </summary>
        /// <param name="stream">The source stream.</param>
        /// <param name="copy">The copy flag. When true all data is copied into memory, otherwise the source-stream is used as a memorymapped file.</param>
        /// <returns></returns>
        public static HugeCoordinateCollectionIndex Deserialize(Stream stream, bool copy = false)
        {
            // read sizes.
            long position = 0;
            var longBytes = new byte[8];
            stream.Read(longBytes, 0, 8);
            position = position + 8;
            var indexLength = BitConverter.ToInt64(longBytes, 0);
            stream.Read(longBytes, 0, 8);
            position = position + 8;
            var coordinateLength = BitConverter.ToInt64(longBytes, 0);

            var file = new MemoryMappedStream(new LimitedStream(stream));

            HugeArrayBase<ulong> indexArray;
            if(indexLength == 0)
            { // create an empty array.
                indexArray = new HugeArray<ulong>(0);
            }
            else
            { // use the stream.
                indexArray = new MemoryMappedHugeArrayUInt64(file, indexLength, indexLength, 1024);
            }
            HugeArrayBase<float> coordinateArray;
            if(coordinateLength == 0)
            { // create an empty array.
                coordinateArray = new HugeArray<float>(0);
            }
            else
            { // use the stream.
                coordinateArray = new MemoryMappedHugeArraySingle(file, coordinateLength, coordinateLength, 1024);
            }
            
            if (copy)
            { // copy the data.
                var indexArrayCopy = new HugeArray<ulong>(indexLength);
                indexArrayCopy.CopyFrom(indexArray);

                var coordinateArrayCopy = new HugeArray<float>(coordinateLength);
                coordinateArrayCopy.CopyFrom(coordinateArray);

                file.Dispose();

                return new HugeCoordinateCollectionIndex(indexLength, indexArrayCopy, coordinateArrayCopy);
            }

            return new HugeCoordinateCollectionIndex(indexLength, indexArray, coordinateArray);
        }

        /// <summary>
        /// Disposes of all resources associated with this coordinate collection index.
        /// </summary>
        public void Dispose()
        {
            _index.Dispose();
            _coordinates.Dispose();
        }
    }
}