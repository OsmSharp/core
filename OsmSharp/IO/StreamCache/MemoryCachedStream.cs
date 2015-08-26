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

using System.Collections.Generic;
using System.IO;

namespace OsmSharp.IO.StreamCache
{
    /// <summary>
    /// An in-memory stream cache.
    /// </summary>
    public class MemoryCachedStream : IStreamCache
    {
        private readonly HashSet<Stream> _streams = new HashSet<Stream>(); 

        /// <summary>
        /// Creates a new stream.
        /// </summary>
        /// <returns></returns>
        public Stream CreateNew()
        {
            var stream = new MemoryStream();
            _streams.Add(stream);
            return stream;
        }

        /// <summary>
        /// Disposes all resource associated with this object.
        /// </summary>
        public void Dispose(Stream stream)
        {
            _streams.Remove(stream);
            stream.Dispose();
        }

        /// <summary>
        /// Disposes all resource associated with this object.
        /// </summary>
        public void Dispose()
        {
            foreach (var stream in _streams)
            {
                stream.Dispose();
            }
            _streams.Clear();
        }
    }
}
