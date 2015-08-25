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

using System.Collections.Generic;
using OsmSharp.Osm.Data;

namespace OsmSharp.Osm.Cache
{
    /// <summary>
    /// An osm data cache for simple OSM objects.
    /// </summary>
    public abstract class OsmDataCache : IOsmGeoSource
    {
        /// <summary>
        /// Adds a new node.
        /// </summary>
        /// <param name="node"></param>
        public abstract void AddNode(Node node);

        /// <summary>
        /// Returns the node with the given id if present.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Node GetNode(long id)
        {
            Node node;
            if (this.TryGetNode(id, out node))
            {
                return node;
            }
            return null;
        }

        /// <summary>
        /// Removes the node with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract bool RemoveNode(long id);

        /// <summary>
        /// Retruns true if the node exists.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool ContainsNode(long id)
        {
            Node node;
            return (this.TryGetNode(id, out node));
        }

        /// <summary>
        /// Tries to get the node with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public abstract bool TryGetNode(long id, out Node node);

        ///// <summary>
        ///// Returns a enumerable of all nodes in this cache.
        ///// </summary>
        ///// <returns></returns>
        //public abstract IEnumerable<Node> GetNodes();

        /// <summary>
        /// Adds a new way.
        /// </summary>
        /// <param name="way"></param>
        public abstract void AddWay(Way way);

        /// <summary>
        /// Removes the way with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract bool RemoveWay(long id);

        /// <summary>
        /// Returns the way with the given id if present.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Way GetWay(long id)
        {
            Way way;
            if (this.TryGetWay(id, out way))
            {
                return way;
            }
            return null;
        }

        /// <summary>
        /// Retruns true if the way exists.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool ContainsWay(long id)
        {
            Way way;
            return (this.TryGetWay(id, out way));
        }

        /// <summary>
        /// Tries to get the way with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="way"></param>
        /// <returns></returns>
        public abstract bool TryGetWay(long id, out Way way);

        ///// <summary>
        ///// Returns a enumerable of all ways in this cache.
        ///// </summary>
        ///// <returns></returns>
        //public abstract IEnumerable<Way> GetWays();

        /// <summary>
        /// Adds a new relation.
        /// </summary>
        /// <param name="relation"></param>
        public abstract void AddRelation(Relation relation);

        /// <summary>
        /// Returns the relation with the given id if present.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Relation GetRelation(long id)
        {
            Relation relation;
            if (this.TryGetRelation(id, out relation))
            {
                return relation;
            }
            return null;
        }

        /// <summary>
        /// Removes the relation with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract bool RemoveRelation(long id);

        /// <summary>
        /// Retruns true if the relation exists.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool ContainsRelation(long id)
        {
            Relation relation;
            return (this.TryGetRelation(id, out relation));
        }

        /// <summary>
        /// Tries to get the relation with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="relation"></param>
        /// <returns></returns>
        public abstract bool TryGetRelation(long id, out Relation relation);

        ///// <summary>
        ///// Returns an enumerable of all relations in this cache.
        ///// </summary>
        ///// <returns></returns>
        //public abstract IEnumerable<Relation> GetRelations();

        /// <summary>
        /// Clears all data from this cache.
        /// </summary>
        public abstract void Clear();
    }
}