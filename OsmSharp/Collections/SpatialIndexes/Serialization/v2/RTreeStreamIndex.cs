// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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
using System.Collections.Generic;
using System.IO;
using OsmSharp.Math.Primitives;
using OsmSharp.Logging;

namespace OsmSharp.Collections.SpatialIndexes.Serialization.v2
{
    /// <summary>
    /// R-tree implementation of a spatial index.
    /// http://en.wikipedia.org/wiki/R-tree
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class RTreeStreamIndex<T> : ISpatialIndexReadonly<T>
    {
        /// <summary>
        /// Holds the serializer.
        /// </summary>
        private readonly RTreeStreamSerializer<T> _serializer;

        /// <summary>
        /// Holds the stream.
        /// </summary>
        private readonly SpatialIndexSerializerStream _stream;

        /// <summary>
        /// Creates a new index.
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="stream"></param>
        public RTreeStreamIndex(RTreeStreamSerializer<T> serializer,
            SpatialIndexSerializerStream stream)
        {
            _serializer = serializer;
            _stream = stream;
        }

        /// <summary>
        /// Returns the data that has overlapping bounding boxes with the given box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public IEnumerable<T> Get(BoxF2D box)
        {
            var results = new HashSet<T>();

            // move to the start.
            long ticksBefore = DateTime.Now.Ticks;
            _stream.Seek(0, SeekOrigin.Begin);

            _serializer.Search(_stream, box, results);

            long ticksAfter = DateTime.Now.Ticks;
            OsmSharp.Logging.Log.TraceEvent("RTreeStreamIndex", TraceEventType.Verbose,
                string.Format("Deserialized {0} objects in {1}ms.", results.Count,
                    (new TimeSpan(ticksAfter - ticksBefore).TotalMilliseconds)));
            return results;
        }

        /// <summary>
        /// Cancels the current request.
        /// </summary>
        public void GetCancel()
        {
            _serializer.SearchCancel();
        }
    }
}