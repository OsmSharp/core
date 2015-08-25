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

using OsmSharp.Collections.Cache;
using OsmSharp.IO.MemoryMappedFiles;
using System;
using System.Collections.Generic;

namespace OsmSharp.Collections.Arrays.MemoryMapped
{
    /// <summary>
    /// Represents a memory mapped huge array.
    /// </summary>
    public abstract class MemoryMappedHugeArray<T> : HugeArrayBase<T>
        where T : struct
    {
        /// <summary>
        /// Holds the length of this array.
        /// </summary>
        private long _length;

        /// <summary>
        /// Holds the file to create the memory mapped accessors.
        /// </summary>
        private MemoryMappedFile _file;

        /// <summary>
        /// Holds the list of accessors, one for each range.
        /// </summary>
        private List<MemoryMappedAccessor<T>> _accessors;

        /// <summary>
        /// Holds the default file element size.
        /// </summary>
        public static long DefaultFileElementSize = (long)1024;

        /// <summary>
        /// Holds the default buffer size.
        /// </summary>
        public static int DefaultBufferSize = 128;

        /// <summary>
        /// Holds the default cache size.
        /// </summary>
        public static int DefaultCacheSize = 64 * 8;

        /// <summary>
        /// Holds the file element size.
        /// </summary>
        private long _fileElementSize = DefaultFileElementSize;

        /// <summary>
        /// Holds the element size.
        /// </summary>
        private int _elementSize;

        /// <summary>
        /// Holds the maximum array size in bytes.
        /// </summary>
        private long _fileSizeBytes;

        /// <summary>
        /// Creates a memory mapped huge array.
        /// </summary>
        /// <param name="file">The the memory mapped file.</param>
        /// <param name="elementSize">The element size.</param>
        /// <param name="size">The initial size of the array.</param>
        /// <param name="arraySize">The size of an indivdual array block.</param>
        /// <param name="bufferSize">The size of an idividual buffer.</param>
        /// <param name="cacheSize">The size of the LRU cache to keep buffers.</param>
        public MemoryMappedHugeArray(MemoryMappedFile file, int elementSize, long size, long arraySize, int bufferSize, int cacheSize)
        {
            if (file == null) { throw new ArgumentNullException(); }
            if (elementSize < 0) { throw new ArgumentOutOfRangeException("elementSize"); }
            if (arraySize < 0) { throw new ArgumentOutOfRangeException("arraySize"); }
            if (size < 0) { throw new ArgumentOutOfRangeException("size"); }

            _file = file;
            _length = size;
            _fileElementSize = arraySize;
            _elementSize = elementSize;
            _fileSizeBytes = arraySize * _elementSize;

            _bufferSize = bufferSize;
            _cachedBuffer = null;
            _cachedBuffers = new LRUCache<long, CachedBuffer>(cacheSize);
            _cachedBuffers.OnRemove += new LRUCache<long, CachedBuffer>.OnRemoveDelegate(buffer_OnRemove);

            var arrayCount = (int)System.Math.Ceiling((double)size / _fileElementSize);
            _accessors = new List<MemoryMappedAccessor<T>>(arrayCount);
            for (int arrayIdx = 0; arrayIdx < arrayCount; arrayIdx++)
            {
                _accessors.Add(this.CreateAccessor(_file, _fileSizeBytes));
            }
        }

        /// <summary>
        /// Creates a new memory mapped accessor.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="sizeInBytes"></param>
        /// <returns></returns>
        protected abstract MemoryMappedAccessor<T> CreateAccessor(MemoryMappedFile file, long sizeInBytes);

        /// <summary>
        /// Returns the length of this array.
        /// </summary>
        public override long Length
        {
            get { return _length; }
        }

        /// <summary>
        /// Resizes this array.
        /// </summary>
        /// <param name="size"></param>
        public override void Resize(long size)
        {
            if (size < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            // clear cache (and save dirty blocks).
            _cachedBuffers.Clear();
            _cachedBuffer = null;

            // store old size.
            var oldSize = _length;
            _length = size;

            var arrayCount = (int)System.Math.Ceiling((double)size / _fileElementSize);
            // _accessors = new List<MemoryMappedAccessor<T>>(arrayCount);
            if (arrayCount < _accessors.Count)
            { // decrease files/accessors.
                for (int arrayIdx = (int)arrayCount; arrayIdx < _accessors.Count; arrayIdx++)
                {
                    _accessors[arrayIdx].Dispose();
                    _accessors[arrayIdx] = null;
                }
                _accessors.RemoveRange((int)arrayCount, (int)(_accessors.Count - arrayCount));
            }
            else
            { // increase files/accessors.
                for (int arrayIdx = _accessors.Count; arrayIdx < arrayCount; arrayIdx++)
                {
                    _accessors.Add(this.CreateAccessor(_file, _fileSizeBytes));
                }
            }
        }

        /// <summary>
        /// Returns the element at the given index.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public override T this[long idx]
        {
            get
            {
                if (idx < 0 || idx >= _length)
                {
                    throw new ArgumentOutOfRangeException();
                }

                // sync buffer.
                var relativePosition = this.SyncBuffer(idx);

                return _cachedBuffer.Buffer[relativePosition];
            }
            set
            {
                if (idx < 0 || idx >= _length)
                {
                    throw new ArgumentOutOfRangeException();
                }

                // sync buffer.
                var relativePosition = this.SyncBuffer(idx);

                _cachedBuffer.Buffer[relativePosition] = value;
                _cachedBuffer.IsDirty = true;
            }
        }

        #region Buffering

        /// <summary>
        /// Holds all the cached buffers.
        /// </summary>
        private LRUCache<long, CachedBuffer> _cachedBuffers;

        /// <summary>
        /// Holds the buffer size.
        /// </summary>
        private int _bufferSize;

        /// <summary>
        /// Holds the last used cached buffer.
        /// </summary>
        private CachedBuffer _cachedBuffer = null;

        /// <summary>
        /// Called when an item is removed from the cache.
        /// </summary>
        /// <param name="item"></param>
        void buffer_OnRemove(MemoryMappedHugeArray<T>.CachedBuffer item)
        {
            if (item.IsDirty)
            {
                long arrayIdx = (long)System.Math.Floor(item.Position / _fileElementSize);
                long localIdx = item.Position % _fileElementSize;
                long localPosition = localIdx * _elementSize;

                if(item.Position + _bufferSize > _length)
                { // only partially write buffer, do not write past the end.
                    _accessors[(int)arrayIdx].WriteArray(localPosition, item.Buffer, 0, (int)(_length - item.Position));
                    return;
                }
                _accessors[(int)arrayIdx].WriteArray(localPosition, item.Buffer, 0, _bufferSize);
            }
        }

        /// <summary>
        /// Syncs buffer.
        /// </summary>
        private int SyncBuffer(long idx)
        {
            // calculate the buffer position.
            var bufferPosition = idx - (idx % _bufferSize);

            // check buffer.
            if (_cachedBuffer == null ||
                _cachedBuffer.Position != bufferPosition)
            { // not in buffer.
                if (!_cachedBuffers.TryGet(bufferPosition, out _cachedBuffer))
                {
                    var newBuffer = new T[_bufferSize];

                    var arrayIdx = (long)System.Math.Floor(bufferPosition / _fileElementSize);
                    var localIdx = bufferPosition % _fileElementSize;
                    var localPosition = localIdx * _elementSize;

                    _accessors[(int)arrayIdx].ReadArray(localPosition, newBuffer, 0, _bufferSize);
                    _cachedBuffer = new CachedBuffer()
                    {
                        Buffer = newBuffer,
                        IsDirty = false,
                        Position = bufferPosition
                    };
                    _cachedBuffers.Add(_cachedBuffer.Position, _cachedBuffer);
                }
            }
            return (int)(idx - bufferPosition);
        }

        /// <summary>
        /// A cached buffer.
        /// </summary>
        private class CachedBuffer
        {
            /// <summary>
            /// Holds the position.
            /// </summary>
            public long Position;

            /// <summary>
            /// Holds the dirty flag.
            /// </summary>
            public bool IsDirty;

            /// <summary>
            /// Holds the buffer.
            /// </summary>
            public T[] Buffer;
        }

        #endregion

        /// <summary>
        /// Diposes of all native resource associated with this array.
        /// </summary>
        public override void Dispose()
        {
            // clear cache.
            _cachedBuffers.Clear();

            // dispose only the accessors, the file may still be in use.
            foreach(var accessor in _accessors)
            {
                accessor.Dispose();
            }
        }
    }
}
