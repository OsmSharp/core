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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Collections;

namespace OsmSharp.Osm.Cache
{
    /// <summary>
    /// An osm data cache for simple OSM objects kept in memory.
    /// </summary>
    public class OsmDataCacheMemory : OsmDataCache
    {
        private Dictionary<long, Node> _nodes;
        private Dictionary<long, Way> _ways;
        private Dictionary<long, Relation> _relations;

        /// <summary>
        /// Creates a new osm data cache for simple OSM objects kept in memory.
        /// </summary>
        public OsmDataCacheMemory()
        {
            _nodes = new Dictionary<long, Node>();
            _ways = new Dictionary<long, Way>();
            _relations = new Dictionary<long, Relation>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        public override void AddNode(Node node)
        {
            if (node == null) throw new ArgumentNullException("node");
            if (node.Id == null) throw new Exception("node.Id is null");

            _nodes[node.Id.Value] = node;
        }

        /// <summary>
        /// Removes the node with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool RemoveNode(long id)
        {
            return _nodes.Remove(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public override bool TryGetNode(long id, out Node node)
        {
            return _nodes.TryGetValue(id, out node);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public override IEnumerable<Node> GetNodes()
        //{
        //    return _nodes.Values;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="way"></param>
        public override void AddWay(Way way)
        {
            if (way == null) throw new ArgumentNullException("way");
            if (way.Id == null) throw new Exception("way.Id is null");

            _ways[way.Id.Value] = way;
        }

        /// <summary>
        /// Removes the way with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool RemoveWay(long id)
        {
            return _ways.Remove(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="way"></param>
        /// <returns></returns>
        public override bool TryGetWay(long id, out Way way)
        {
            return _ways.TryGetValue(id, out way);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public override IEnumerable<Way> GetWays()
        //{
        //    return _ways.Values;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relation"></param>
        public override void AddRelation(Relation relation)
        {
            if (relation == null) throw new ArgumentNullException("relation");
            if (relation.Id == null) throw new Exception("relation.Id is null");

            _relations[relation.Id.Value] = relation;
        }

        /// <summary>
        /// Removes the relation with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool RemoveRelation(long id)
        {
            return _relations.Remove(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="relation"></param>
        /// <returns></returns>
        public override bool TryGetRelation(long id, out Relation relation)
        {
            return _relations.TryGetValue(id, out relation);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public override IEnumerable<Relation> GetRelations()
        //{
        //    return _relations.Values;
        //}

        /// <summary>
        /// Clears the data in this cache.
        /// </summary>
        public override void Clear()
        {
            _nodes.Clear();
            _ways.Clear();
            _relations.Clear();
        }
    }
}
