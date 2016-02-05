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

using System.Linq;
using OsmSharp.Osm.Complete;
using OsmSharp.Osm.Streams.Complete;
using System.Collections.Generic;

namespace OsmSharp.Osm.Data
{
    /// <summary>
    /// Contains extensions for the snapshot db.
    /// </summary>
    public static class ISnapshotDbExtensions
    {

        /// <summary>
        /// Returns true if the node with the given id exists.
        /// </summary>
        public static bool ExistsNode(this ISnapshotDb db, long id)
        {
            return db.Exists(OsmGeoType.Node, id);
        }

        /// <summary>
        /// Deletes the node with the given id and returns true if it existed.
        /// </summary>
        public static bool DeleteNode(this ISnapshotDb db, long id)
        {
            return db.Delete(OsmGeoType.Node, id);
        }

        /// <summary>
        /// Returns true if the way with the given id exists.
        /// </summary>
        public static bool ExistsWay(this ISnapshotDb db, long id)
        {
            return db.Exists(OsmGeoType.Way, id);
        }

        /// <summary>
        /// Deletes the way with the given id and returns true if it existed.
        /// </summary>
        public static bool DeleteWay(this ISnapshotDb db, long id)
        {
            return db.Delete(OsmGeoType.Way, id);
        }

        /// <summary>
        /// Returns true if the relation with the given id exists.
        /// </summary>
        public static bool ExistsRelation(this ISnapshotDb db, long id)
        {
            return db.Exists(OsmGeoType.Relation, id);
        }

        /// <summary>
        /// Deletes the relation with the given id and returns true if it existed.
        /// </summary>
        public static bool DeleteRelation(this ISnapshotDb db, long id)
        {
            return db.Delete(OsmGeoType.Relation, id);
        }

        /// <summary>
        /// Gets all data in the form of a complete stream.
        /// </summary>
        public static OsmCompleteStreamSource GetComplete(this ISnapshotDb db)
        {
            return new Streams.Complete.OsmCompleteEnumerableStreamSource(
                db.Get().Select(x => x.CreateComplete(db)));
        }

        /// <summary>
        /// Gets all osm objects that pass the given filter within the given bounding box.
        /// </summary>
        public static IEnumerable<OsmGeo> Get(this ISnapshotDb db, Math.Geo.GeoCoordinateBox box)
        {
            return db.Get((float)box.MinLat, (float)box.MinLon, (float)box.MaxLat, (float)box.MaxLon);
        }
    }
}