// The MIT License (MIT)

// Copyright (c) 2016 Ben Abelshausen

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Collections.Generic;

namespace OsmSharp.Db.Impl
{
    /// <summary>
    /// Abstract representation of basic operations a snapshot db should support.
    /// </summary>
    public interface ISnapshotDbImpl
    {
        /// <summary>
        /// Clears all data.
        /// </summary>
        void Clear();

        /// <summary>
        /// Adds or updates the given objects.
        /// </summary>
        /// <remarks>
        /// - Adds objects that don't exist yet.
        /// - Updates objects that already exist.
        /// </remarks>
        void AddOrUpdate(IEnumerable<OsmGeo> osmGeos);

        /// <summary>
        /// Gets all objects.
        /// </summary>
        /// <returns>
        /// All objects sorted by type (node, way, relation) and then id ascending.
        /// </returns>
        IEnumerable<OsmGeo> Get();

        /// <summary>
        /// Gets all objects for the given keys.
        /// </summary>
        /// <returns>
        /// All objects for the given keys sorted by type (node, way, relation) and then id ascending.
        /// </returns>
        IEnumerable<OsmGeo> Get(IEnumerable<OsmGeoKey> keys);

        /// <summary>
        /// Gets all objects in the given bounding box.
        /// </summary>
        /// <returns>
        /// - All nodes within bounding box.
        /// - All ways with at least one node.
        /// - All nodes outside of the bounding box but member of a way with at least one node.
        /// - All relations with at least one member that is a node within the bounding box or a way with at least one node in the bounding box.
        /// - Sorted by type (node, way, relation) and then id ascending.
        /// </returns>
        IEnumerable<OsmGeo> Get(float minLatitude, float minLongitude, float maxLatitude, float maxLongitude);

        /// <summary>
        /// Deletes all objects for the given keys.
        /// </summary>
        void Delete(IEnumerable<OsmGeoKey> keys);

        /// <summary>
        /// Gets all ways with at least one node in the given ids.
        /// </summary>
        /// <returns>
        /// All ways with at least one node in the given id set sorted by id ascending.
        /// </returns>
        IEnumerable<Way> GetWays(IEnumerable<long> ids);

        /// <summary>
        /// Gets all relations with at least one member in the given keys.
        /// </summary>
        /// <returns>
        /// All relations with at least one member in the given key set sorted by id ascending.
        /// </returns>
        IEnumerable<Relation> GetRelations(IEnumerable<OsmGeoKey> keys);
    }
}