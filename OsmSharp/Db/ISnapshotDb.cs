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

using OsmSharp.Changesets;
using OsmSharp.Streams;
using System.Collections.Generic;

namespace OsmSharp.Db
{
    /// <summary>
    /// Abstract representation of a database to store OSM nodes, ways and relations without version and using only one object per id.
    /// 
    /// - Only one node per id.
    /// - Only one way per id.
    /// - Only one relation per id.
    /// - Does not generate id's, objects are stored as-is.
    /// 
    /// </summary>
    public interface ISnapshotDb : IOsmGeoSource
    {
        /// <summary>
        /// Clears all data.
        /// </summary>
        void Clear();

        /// <summary>
        /// Adds or updates the given osm object in the db exactly as given.
        /// </summary>
        void AddOrUpdate(OsmGeo osmGeo);

        /// <summary>
        /// Adds or updates osm objects in the db exactly as they are given.
        /// </summary>
        /// <remarks>
        /// - Replaces objects that already exist with the given id.
        /// </remarks>
        void AddOrUpdate(IEnumerable<OsmGeo> osmGeos);

        /// <summary>
        /// Gets all the objects in the form of an osm stream source.
        /// </summary>
        /// <returns></returns>
        OsmStreamSource Get();

        /// <summary>
        /// Gets all osm objects with the given type and the given id's.
        /// </summary>
        IList<OsmGeo> Get(OsmGeoType type, IList<long> id);

        /// <summary>
        /// Gets all osm objects that pass the given filter within the given bounding box.
        /// </summary>
        IList<OsmGeo> Get(float minLatitude, float minLongitude, float maxLatitude, float maxLongitude);

        /// <summary>
        /// Deletes the osm object with the given type, the given id without applying a changeset.
        /// </summary>
        bool Delete(OsmGeoType type, long id);

        /// <summary>
        /// Deletes all osm objects with the given types and the given id's.
        /// </summary>
        IList<bool> Delete(IList<OsmGeoType> type, IList<long> id);

        /// <summary>
        /// Applies the given changeset.
        /// </summary>
        /// <param name="changeset">The changeset to apply.</param>
        /// <param name="bestEffort">When false, it's the entire changeset or nothing. When true the changeset is applied using best-effort.</param>
        /// <returns>The diff result result object containing the diff result and status information.</returns>
        DiffResultResult ApplyChangeset(OsmChange changeset, bool bestEffort = false);
    }
}