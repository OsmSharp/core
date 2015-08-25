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

using OsmSharp.Collections.Indexes.MemoryMapped;
using OsmSharp.IO.MemoryMappedFiles;
using System;

namespace OsmSharp.Collections.Indexes
{
    /// <summary>
    /// An index of objects linked to a unique id.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class IndexBase<T>
    {
        /// <summary>
        /// Returns true if this index is readonly.
        /// </summary>
        public abstract bool IsReadonly
        {
            get;
        }

        /// <summary>
        /// Returns true if this index is serializable.
        /// </summary>
        public abstract bool IsSerializable
        {
            get;
        }

        /// <summary>
        /// Tries to get an element with the given id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="element">The element.</param>
        /// <returns>True if an element with the given id was found.</returns>
        public abstract bool TryGet(long id, out T element);

        /// <summary>
        /// Gets the element with the given id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The element.</returns>
        public abstract T Get(long id);

        /// <summary>
        /// Adds a new element to this index.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public abstract long Add(T element);

        /// <summary>
        /// Serializes this index to the given stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public abstract long Serialize(System.IO.Stream stream);

        /// <summary>
        /// Deserializes a tags index from the given stream.
        /// </summary>
        /// <param name="stream">The stream to read from. Reading will start at position 0.</param>
        /// <param name="readFrom">The delegate to read data from the stream.</param>
        /// <returns></returns>
        public static IndexBase<T> Deserialize(System.IO.Stream stream, MemoryMappedFile.ReadFromDelegate<T> readFrom)
        {
            var longBytes = new byte[8];
            stream.Read(longBytes, 0, 8);
            var size = BitConverter.ToInt64(longBytes, 0);

            var file = new MemoryMappedStream(new OsmSharp.IO.LimitedStream(stream));
            return new MemoryMappedIndex<T>(file, readFrom,
                (s, position, value) =>
                {
                    throw new InvalidOperationException("This index is readonly.");
                }, size);
        }
    }
}