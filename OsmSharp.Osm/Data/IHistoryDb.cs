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
        /// Opens a new changeset
        /// </summary>
        long OpenChangeset(ChangeSetInfo info);

        /// <summary>
        /// Applies the given changeset.
        /// </summary>
        bool ApplyChangeset(long id, ChangeSet changeset);
    }
}
