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
        /// <summary>
        /// The maximum size of one collection is.
        /// </summary>
        private int MAX_COLLECTION_SIZE = ushort.MaxValue;

        /// <summary>
        /// The average estimated size.
        /// </summary>
        private int ESTIMATED_SIZE = 5;

        /// <summary>
        /// Holds the coordinates index position and count.
        /// </summary>
        private HugeArrayBase<ulong> _index;

        /// <summary>
        /// Holds the coordinates in linked-list form.
        /// </summary>
        private HugeArrayBase<float> _coordinates;

        /// <summary>
        /// Holds the next idx.
        /// </summary>
        private long _nextIdx = 0;

        /// <summary>
        /// Holds the max id.
        /// </summary>
        private long _maxId = 0;

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
                _index[idx] = 0;
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

            for (long idx = 0; idx < _coordinates.Length; idx++)
            {
                _coordinates[idx] = float.MinValue;
            }
        }

        /// <summary>
        /// Returns the collection with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if the collection was found.</returns>
        public bool Remove(long id)
        {
            long index, size;
            if(this.TryGetIndexAndSize(id, out index, out size))
            {
                this.DoReset(index, size);
                _index[id] = 0;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds/updates the coordinate collection at the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="coordinates"></param>
        public void Add(long id, ICoordinateCollection coordinates)
        {
            long index, size;
            if (this.TryGetIndexAndSize(id, out index, out size))
            {
                if(size == coordinates.Count * 2)
                { // just update in-place.
                    this.DoSet(index, coordinates);
                }
                else
                { // remove and add new.
                    _index[id] = this.DoAdd(coordinates);
                }
            }
            else
            { // add new coordinates.
                _index[id] = this.DoAdd(coordinates);
            }
        }

        /// <summary>
        /// Returns the coordinate collection at the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public bool Get(long id, out ICoordinateCollection coordinates)
        {
            long index, size;
            if (this.TryGetIndexAndSize(id, out index, out size))
            {
                coordinates = new HugeCoordinateCollection(_coordinates, index, size);
                return true;
            }
            coordinates = null;
            return false;
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
                if(this.Get(id, out coordinates))
                {
                    return coordinates;
                }
                return null;
            }
            set
            {
                if (value == null || value.Count == 0)
                {
                    this.Remove(id);
                }
                else
                {
                    this.Add(id, value);
                }

                if(_maxId < id)
                { // update max id.
                    _maxId = id;
                }
            }
        }

        /// <summary>
        /// Resizes this array.
        /// </summary>
        /// <param name="size"></param>
        public void Resize(long size)
        {
            _index.Resize(size);
            if(_maxId >= size)
            { 
                _maxId = size - 1;
            }

            //this.Trim();
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
        /// <param name="id"></param>
        /// <param name="index"></param>
        /// <param name="size"></param>
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
            if(data == 0)
            {
                index = 0;
                size = 0;
                return false;
            }
            index = ((long)(data / (ulong)MAX_COLLECTION_SIZE)) * 2;
            size = (long)(data % (ulong)MAX_COLLECTION_SIZE) * 2;
            return true;
        }

        /// <summary>
        /// Resets all coordinates to the default.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="size"></param>
        private void DoReset(long index, long size)
        {
            for(long idx = index; idx < index + (size); idx++)
            {
                _coordinates[idx] = float.MinValue; 
            }
        }

        /// <summary>
        /// Sets the coordinates starting at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="coordinates"></param>
        private void DoSet(long index, ICoordinateCollection coordinates)
        {
            long idx = index;
            coordinates.Reset();
            while(coordinates.MoveNext())
            {
                _coordinates[idx] = coordinates.Latitude;
                _coordinates[idx + 1] = coordinates.Longitude;
                idx = idx + 2;
            }
        }

        /// <summary>
        /// Adds the new coordinates at the end of the current coordinates.
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        private ulong DoAdd(ICoordinateCollection coordinates)
        {
            var newId = (ulong)(_nextIdx * MAX_COLLECTION_SIZE) + (ulong)coordinates.Count;
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
            return newId;
        }

        #endregion

        /// <summary>
        /// Represents the huge coordinate collection.
        /// </summary>
        private class HugeCoordinateCollection : ICoordinateCollection
        {
            /// <summary>
            /// Holds the start idx.
            /// </summary>
            private long _startIdx;

            /// <summary>
            /// Holds the current idx.
            /// </summary>
            private long _currentIdx = -2;

            /// <summary>
            /// Holds the size.
            /// </summary>
            private long _size;

            /// <summary>
            /// Holds the coordinates.
            /// </summary>
            private HugeArrayBase<float> _coordinates;

            /// <summary>
            /// Holds the reverse flag.
            /// </summary>
            private bool _reverse;

            /// <summary>
            /// Creates a new huge coordinate collection.
            /// </summary>
            /// <param name="coordinates"></param>
            /// <param name="startIdx"></param>
            /// <param name="size"></param>
            public HugeCoordinateCollection(HugeArrayBase<float> coordinates, long startIdx, long size)
                : this(coordinates, startIdx, size, false)
            {

            }

            /// <summary>
            /// Creates a new huge coordinate collection.
            /// </summary>
            /// <param name="coordinates"></param>
            /// <param name="startIdx"></param>
            /// <param name="size"></param>
            /// <param name="reverse"></param>
            public HugeCoordinateCollection(HugeArrayBase<float> coordinates, long startIdx, long size, bool reverse)
            {
                _startIdx = startIdx;
                _size = size;
                _coordinates = coordinates;
                _reverse = reverse;
            }

            /// <summary>
            /// Returns the count.
            /// </summary>
            public int Count
            {
                get { return (int)(_size / 2); }
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
                    _currentIdx = _currentIdx - 2;
                    return (_currentIdx - _startIdx) >= 0;
                }
                else
                {
                    _currentIdx = _currentIdx + 2;
                    return (_currentIdx - _startIdx) < _size;
                }
            }

            /// <summary>
            /// Resets the current enumerator.
            /// </summary>
            public void Reset()
            {
                if(_reverse)
                {
                    _currentIdx = _startIdx + _size;
                }
                else
                {
                    _currentIdx = _startIdx - 2;
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
                get { return _coordinates[_currentIdx]; }
            }

            /// <summary>
            /// Returns the longitude.
            /// </summary>
            public float Longitude
            {
                get { return _coordinates[_currentIdx + 1]; }
            }

            /// <summary>
            /// Returns the reverse collection.
            /// </summary>
            /// <returns></returns>
            public ICoordinateCollection Reverse()
            {
                return new HugeCoordinateCollection(_coordinates, _startIdx, _size, !_reverse);
            }
        }

        /// <summary>
        /// Compresses the data in this index.
        /// </summary>
        public void Compress()
        {
            // reorganizes the data in the coordinate array and resizes it.
            var offset = _nextIdx;
            var nextIdx = offset * 2;
            for(int idx = 0; idx < _maxId + 1; idx++)
            {
                var data = _index[idx];
                var index = ((long)(data / (ulong)MAX_COLLECTION_SIZE)) * 2;
                var size = (long)(data % (ulong)MAX_COLLECTION_SIZE) * 2;
                var newId = (ulong)(((nextIdx / 2) - offset) * MAX_COLLECTION_SIZE) + (ulong)(size / 2);
                for(var localIdx = index; localIdx < index + size; localIdx = localIdx + 2)
                {
                    if(nextIdx >= _coordinates.Length)
                    { // increase the index.
                        this.IncreaseCoordinates();
                    }
                    _coordinates[nextIdx] = _coordinates[localIdx];
                    _coordinates[nextIdx + 1] = _coordinates[localIdx + 1];
                    nextIdx = nextIdx + 2;
                }

                _index[idx] = newId;
            }

            // copy to the beginning.
            for(int idx = 0; idx < nextIdx - (offset * 2); idx++)
            {
                _coordinates[idx] = _coordinates[idx + (offset * 2)];
            }
            var newSize = nextIdx - (offset * 2);
            if (newSize <= 0)
            { // don't resize to zero, leave things a mess.
                newSize = 1;
            }
            _coordinates.Resize(newSize);
        }

        /// <summary>
        /// Trims the size of the index and of the coordinate array to the exact size needed.
        /// </summary>
        /// <returns></returns>
        public void Trim()
        {
            // find the highest index where the index-entry is non-null.
            var maxIndex = _maxId;
            _index.Resize(maxIndex + 1);

            //// resize accordingly.
            //if (_maxId > 0)
            //{ // only compress.
            //    this.Compress();
            //}
        }

        /// <summary>
        /// Serializes this huge collection index to the given stream.
        /// </summary>
        /// <param name="stream"></param>
        public long Serialize(Stream stream)
        {
            // first trim to minimize the amount of useless data store.
            this.Trim();

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
                // write index.
                var indexArray = new MemoryMappedHugeArrayUInt64(file, indexSize, indexSize, 1024);
                indexArray.CopyFrom(_index, indexSize);
                indexArray.Dispose();
                position = position + (indexSize * 8);

                // write coordinates.
                var coordinatesArray = new MemoryMappedHugeArraySingle(file, coordinatesCount * 2, coordinatesCount * 2, 1024);
                coordinatesArray.CopyFrom(_coordinates);
                coordinatesArray.Dispose();
                position = position + (coordinatesCount * 2 * 4);
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
            var indexArray = new MemoryMappedHugeArrayUInt64(file, indexLength, indexLength, 1024);
            var coordinateArray = new MemoryMappedHugeArraySingle(file, coordinateLength * 2, coordinateLength * 2, 1024);

            if (copy)
            { // copy the data.
                var indexArrayCopy = new HugeArray<ulong>(indexLength);
                indexArrayCopy.CopyFrom(indexArray);

                var coordinateArrayCopy = new HugeArray<float>(coordinateLength * 2);
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
            _index = null;
            _coordinates.Dispose();
            _coordinates = null;
        }
    }
}