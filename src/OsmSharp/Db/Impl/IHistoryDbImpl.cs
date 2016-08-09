using OsmSharp.Changesets;
using System.Collections.Generic;

namespace OsmSharp.Db.Impl
{
    /// <summary>
    /// Abstract representation of a history db implementation.
    /// </summary>
    public interface IHistoryDbImpl
    {
        /// <summary>
        /// Clears all data.
        /// </summary>
        void Clear();

        /// <summary>
        /// Adds an object.
        /// </summary>
        /// <remarks>The object gets inserted into the archive it is not visible.</remarks>
        void Add(IEnumerable<OsmGeo> osmGeo);

        /// <summary>
        /// Gets all non-archived objects.
        /// </summary>
        /// <returns>
        /// Objects sorted by type (node, way, relation) and then id ascending.
        /// </returns>
        IEnumerable<OsmGeo> Get();

        /// <summary>
        /// Gets all objects.
        /// </summary>
        /// <returns>
        /// Objects sorted by type (node, way, relation) and then id and then version ascending.
        /// </returns>
        IEnumerable<OsmGeo> GetWithArchived();

        /// <summary>
        /// Gets all non-archived objects with the given keys.
        /// </summary>
        /// <returns>
        /// Objects sorted by type (node, way, relation) and then id ascending.
        /// </returns>
        IEnumerable<OsmGeo> Get(IEnumerable<OsmGeoKey> keys);

        /// <summary>
        /// Gets all objects with the given keys.
        /// </summary>
        /// <returns>
        /// Objects sorted by type (node, way, relation) and then id and then version ascending.
        /// </returns>
        IEnumerable<OsmGeo> Get(IEnumerable<OsmGeoVersionKey> keys);

        /// <summary>
        /// Gets allobjects with the given keys.
        /// </summary>
        /// <returns>
        /// Objects sorted by type (node, way, relation) and then id and then version ascending.
        /// </returns>
        IEnumerable<OsmGeo> GetWithArchived(IEnumerable<OsmGeoKey> keys);

        /// <summary>
        /// Gets all non-archived objects within the given bounding box.
        /// </summary>
        /// <returns>
        /// - All non-archived nodes within bounding box.
        /// - All non-archived ways with at least one node.
        /// - All non-archived nodes outside of the bounding box but member of a way with at least one node.
        /// - All non-archived relations with at least one member that is a node within the bounding box or a way with at least one node in the bounding box.
        /// - Sorted by type (node, way, relation) and then id ascending.
        /// </returns>
        IEnumerable<OsmGeo> Get(float minLatitude, float minLongitude, float maxLatitude, float maxLongitude);

        /// <summary>
        /// Archives all the objects with the given keys.
        /// </summary>
        void Archive(IEnumerable<OsmGeoKey> keys);

        /// <summary>
        /// Gets the last id for the given type.
        /// </summary>
        long GetLastId(OsmGeoType type);

        /// <summary>
        /// Gets the last changeset id.
        /// </summary>
        /// <returns></returns>
        long GetLastChangesetId();

        /// <summary>
        /// Adds or updates a changeset.
        /// </summary>
        void AddOrUpdate(Changeset changeset);

        /// <summary>
        /// Adds changes for the given changeset.
        /// </summary>
        void AddChanges(long id, OsmChange changes);

        /// <summary>
        /// Gets the changeset with the given id.
        /// </summary>
        Changeset GetChangeset(long id);

        /// <summary>
        /// Gets all then changes for the given changeset.
        /// </summary>
        OsmChange GetChanges(long id);
    }
}