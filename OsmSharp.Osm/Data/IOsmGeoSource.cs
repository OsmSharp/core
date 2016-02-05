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

using System;

namespace OsmSharp.Osm.Data
{
    /// <summary>
    /// Abstract representation of a source of a snapshot of OSM-objects.
    /// </summary>
    public interface IOsmGeoSource
    {
        /// <summary>
        /// Returns a node with the given id from this source.
        /// </summary>
        Node GetNode(long id);

        /// <summary>
        /// Returns a way with the given id from this source.
        /// </summary>
        Way GetWay(long id);

        /// <summary>
        /// Returns a relation with the given id from this source.
        /// </summary>
        Relation GetRelation(long id);
    }

    /// <summary>
    /// A osm geo source implementation for a snapshot db.
    /// </summary>
    internal class OsmGeoSourceSnapshotDb : IOsmGeoSource
    {
        private readonly ISnapshotDb _db;

        /// <summary>
        /// Creates a osm geo source.
        /// </summary>
        public OsmGeoSourceSnapshotDb(ISnapshotDb db)
        {
            _db = db;
        }

        /// <summary>
        /// Returns the node for the given id.
        /// </summary>
        public Node GetNode(long id)
        {
            return _db.Get(OsmGeoType.Node, id) as Node;
        }

        /// <summary>
        /// Returns the way for the given id.
        /// </summary>
        public Way GetWay(long id)
        {
            return _db.Get(OsmGeoType.Way, id) as Way;
        }

        /// <summary>
        /// Returns the relation for the given id.
        /// </summary>
        public Relation GetRelation(long id)
        {
            return _db.Get(OsmGeoType.Relation, id) as Relation;
        }
    }

    /// <summary>
    /// A osm geo source implementation for a history db.
    /// </summary>
    internal class OsmGeoSourceHistoryDb : IOsmGeoSource
    {
        private readonly IHistoryDb _db;

        /// <summary>
        /// Creates a osm geo source.
        /// </summary>
        public OsmGeoSourceHistoryDb(IHistoryDb db)
        {
            _db = db;
        }

        /// <summary>
        /// Returns the node for the given id.
        /// </summary>
        public Node GetNode(long id)
        {
            return _db.Get(OsmGeoType.Node, id) as Node;
        }

        /// <summary>
        /// Returns the way for the given id.
        /// </summary>
        public Way GetWay(long id)
        {
            return _db.Get(OsmGeoType.Way, id) as Way;
        }

        /// <summary>
        /// Returns the relation for the given id.
        /// </summary>
        public Relation GetRelation(long id)
        {
            return _db.Get(OsmGeoType.Relation, id) as Relation;
        }
    }
}
