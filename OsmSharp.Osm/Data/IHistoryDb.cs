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
using OsmSharp.Osm.Streams;
using System.Collections.Generic;

namespace OsmSharp.Osm.Data
{
    /// <summary>
    /// Abstract representation of a database to store OSM-data including history. This db can store nodes, ways, relations and changesets.
    /// 
    /// - Multiple version of the same node can exist.
    /// - Changes are only possible by:
    ///    - Applying changesets.
    /// - Changeset meta-data can only be updated when changesets are open.
    /// - Changesets have to applied in the following order:
    ///    - 1. Open new changeset, an new id is returned.
    ///    - 2. Apply changes.
    ///    - 3. Close the changeset.
    /// </summary>
    public interface IHistoryDb
    {
        /// <summary>
        /// Clears all data.
        /// </summary>
        void Clear();
        
        /// <summary>
        /// Adds the given osm object in the db exactly as given.
        /// </summary>
        /// <remarks>
        /// - To update use ApplyChanges.
        /// - It's not possible to add multiple objects of a given type with the same id/version# pair.
        /// </remarks>
        void Add(OsmGeo osmGeo);

        /// <summary>
        /// Adds osm objects in the db exactly as they are given.
        /// </summary>
        /// <remarks>
        /// - To update use ApplyChanges.
        /// - It's not possible to add multiple objects of a given type with the same id/version# pair.
        /// </remarks>
        void Add(IEnumerable<OsmGeo> osmGeos);

        /// <summary>
        /// Adds the given changeset in the db exactly as given.
        /// </summary>
        void Add(ChangeSet changeset, ChangeSetInfo info);

        /// <summary>
        /// Gets all the objects in the form of an osm stream source.
        /// </summary>
        /// <returns></returns>
        OsmStreamSource Get();

        /// <summary>
        /// Gets the latest version of an osm object of the given type with the given id.
        /// </summary>
        OsmGeo Get(OsmGeoType type, long id);

        /// <summary>
        /// Gets all latest version of osm objects with the given types and the given id's.
        /// </summary>
        IList<OsmGeo> Get(IList<OsmGeoType> type, IList<long> id);

        /// <summary>
        /// Gets an osm object of the given type, the given id and the given version #.
        /// </summary>
        OsmGeo Get(OsmGeoType type, long id, int version);

        /// <summary>
        /// Gets all osm objects with the given types, the given id's and the given version #'s.
        /// </summary>
        IList<OsmGeo> Get(IList<OsmGeoType> type, IList<long> id, IList<long> version);

        /// <summary>
        /// Gets all latest versions of osm objects within the given bounding box.
        /// </summary>
        IList<OsmGeo> Get(float minLatitude, float minLongitude, float maxLatitude, float maxLongitude);

        /// <summary>
        /// Opens a new changeset
        /// </summary>
        long OpenChangeset(ChangeSetInfo info);

        /// <summary>
        /// Applies the given changeset.
        /// </summary>
        /// <param name="changeset">The changeset to apply.</param>
        /// <param name="bestEffort">When false, it's the entire changeset or nothing. When true the changeset is applied using best-effort.</param>
        /// <returns>The diff result result object containing the diff result and status information.</returns>
        DiffResultResult ApplyChangeset(ChangeSet changeset, bool bestEffort = false);

        /// <summary>
        /// Updates the changeset with the new info.
        /// </summary>
        bool UpdateChangesetInfo(ChangeSetInfo info);

        /// <summary>
        /// Closes the changeset with the given id.
        /// </summary>
        bool CloseChangeset(long id);
    }
}