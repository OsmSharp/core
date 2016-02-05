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
    /// Contains extensions for the history db.
    /// </summary>
    public static class IHistoryDbExtensions
    {
        /// <summary>
        /// Gets a osm geo source for the db.
        /// </summary>
        public static IOsmGeoSource ToOsmGeoSource(this IHistoryDb db)
        {
            return new OsmGeoSourceHistoryDb(db);
        }

        /// <summary>
        /// Gets the node with the given id.
        /// </summary>
        public static Node GetNode(this IHistoryDb db, long id)
        {
            return db.Get(OsmGeoType.Node, id) as Node;
        }

        /// <summary>
        /// Gets the way with the given id.
        /// </summary>
        public static Way GetWay(this IHistoryDb db, long id)
        {
            return db.Get(OsmGeoType.Way, id) as Way;
        }

        /// <summary>
        /// Gets the relation with the given id.
        /// </summary>
        public static Relation GetRelation(this IHistoryDb db, long id)
        {
            return db.Get(OsmGeoType.Relation, id) as Relation;
        }

        /// <summary>
        /// Gets all data in the form of a complete stream.
        /// </summary>
        public static OsmCompleteStreamSource GetComplete(this IHistoryDb db)
        {
            var osmGeoSource = db.ToOsmGeoSource();
            return new Streams.Complete.OsmCompleteEnumerableStreamSource(
                db.Get().Select(x => x.CreateComplete(osmGeoSource)));
        }

        /// <summary>
        /// Gets all osm objects that pass the given filter within the given bounding box.
        /// </summary>
        public static IEnumerable<OsmGeo> Get(this IHistoryDb db, Math.Geo.GeoCoordinateBox box)
        {
            return db.Get((float)box.MinLat, (float)box.MinLon, (float)box.MaxLat, (float)box.MaxLon);
        }
    }
}