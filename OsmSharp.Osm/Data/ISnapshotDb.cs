// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2016 Abelshausen Ben
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

using OsmSharp.Osm.Changesets;
using OsmSharp.Osm.Filters;
using OsmSharp.Osm.Streams;
using System.Collections.Generic;

namespace OsmSharp.Osm.Data
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
    public interface ISnapshotDb
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
        /// Gets an osm object of the given type and the given id.
        /// </summary>
        OsmGeo Get(OsmGeoType type, long id);

        /// <summary>
        /// Gets all osm objects with the given types and the given id's.
        /// </summary>
        IList<OsmGeo> Get(IList<OsmGeoType> type, IList<long> id);

        /// <summary>
        /// Gets all osm objects that pass the given filter within the given bounding box.
        /// </summary>
        IList<OsmGeo> Get(float minLatitude, float minLongitude, float maxLatitude, float maxLongitude,
            Filter filter);

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
        DiffResultResult ApplyChangeset(ChangeSet changeset, bool bestEffort = false);
    }
}