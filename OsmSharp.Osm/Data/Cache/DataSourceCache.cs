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
using OsmSharp.Osm;
using OsmSharp.Units.Angle;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Filters;
using OsmSharp.Osm.Tiles;
using OsmSharp.Collections.Cache;

namespace OsmSharp.Osm.Data.Cache
{
    /// <summary>
    /// Class used for caching data using bounding boxes.
    /// </summary>
    public class DataSourceCache : DataSourceReadOnlyBase
    {
        /// <summary>
        /// Holds the id of this datasource.
        /// </summary>
        private Guid _id = Guid.NewGuid();

        /// <summary>
        /// Holds the datasource being cached.
        /// </summary>
        private IDataSourceReadOnly _source;

        /// <summary>
        /// Creates a new datasource cache.
        /// </summary>
        /// <param name="source"></param>
        public DataSourceCache(IDataSourceReadOnly source)
        {
            _source = source;
        }

        /// <summary>
        /// Holds the lru cache for the nodes.
        /// </summary>
        private LRUCache<long, Node> _nodesCache = new LRUCache<long,Node>(10000);

        /// <summary>
        /// Holds the lru cache for the ways.
        /// </summary>
        private LRUCache<long, Way> _waysCache = new LRUCache<long, Way>(5000);

        /// <summary>
        /// Holds the lru cache for the relations.
        /// </summary>
        private LRUCache<long, Relation> _relationsCache = new LRUCache<long, Relation>(1000);

        /// <summary>
        /// Returns the boundingbox.
        /// </summary>
        public override GeoCoordinateBox BoundingBox
        {
            get { return _source.BoundingBox; }
        }

        /// <summary>
        /// Returns the id of this datasource.
        /// </summary>
        public override Guid Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Returns true if this datasource is bounded.
        /// </summary>
        public override bool HasBoundinBox
        {
            get { return _source.HasBoundinBox; }
        }

        /// <summary>
        /// Returns the node with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override Node GetNode(long id)
        {
            Node node;
            if(!_nodesCache.TryGet(id, out node))
            { // cache miss.
                node = _source.GetNode(id);
                _nodesCache.Add(id, node);
            }
            return node;
        }

        /// <summary>
        /// Returns all the nodes in the same order than the given list.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public override IList<Node> GetNodes(IList<long> ids)
        {
            List<Node> nodes = new List<Node>(ids.Count);
            for (int idx = 0; idx < nodes.Count; idx++)
            {
                nodes.Add(this.GetNode(ids[idx]));
            }
            return nodes;
        }

        /// <summary>
        /// Returns the relation with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override Relation GetRelation(long id)
        {
            Relation relation;
            if (!_relationsCache.TryGet(id, out relation))
            { // cache miss.
                relation = _source.GetRelation(id);
                _relationsCache.Add(id, relation);
            }
            return relation;
        }

        /// <summary>
        /// Returns all the relations in the same order than the given list.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public override IList<Relation> GetRelations(IList<long> ids)
        {
            List<Relation> relations = new List<Relation>(ids.Count);
            for (int idx = 0; idx < relations.Count; idx++)
            {
                relations.Add(this.GetRelation(ids[idx]));
            }
            return relations;
        }

        /// <summary>
        /// Returns all relations for the given object.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public override IList<Relation> GetRelationsFor(OsmGeoType type, long id)
        {
            IList<Relation> relations = _source.GetRelationsFor(type, id);
            foreach (Relation relation in relations)
            {
                _relationsCache.Add(relation.Id.Value, relation);
            }
            return relations;
        }

        /// <summary>
        /// Returns the way for the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override Way GetWay(long id)
        {
            Way way;
            if (!_waysCache.TryGet(id, out way))
            { // cache miss.
                way = _source.GetWay(id);
                _waysCache.Add(id, way);
            }
            return way;
        }

        /// <summary>
        /// Returns all the ways in the same order than the given list.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public override IList<Way> GetWays(IList<long> ids)
        {
            List<Way> ways = new List<Way>(ids.Count);
            for (int idx = 0; idx < ways.Count; idx++)
            {
                ways.Add(this.GetWay(ids[idx]));
            }
            return ways;
        }

        /// <summary>
        /// Returns all the ways for a given node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override IList<Way> GetWaysFor(Node node)
        {
            return this.GetWaysFor(node.Id.Value);
        }

        /// <summary>
        /// Returns all ways containing the given node.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override IList<Way> GetWaysFor(long id)
        {
            IList<Way> ways = _source.GetWaysFor(id);
            foreach (Way way in ways)
            {
                _waysCache.Add(way.Id.Value, way);
            }
            return ways;
        }

        /// <summary>
        /// Returns all data in the given bounding box.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public override IList<OsmGeo> Get(GeoCoordinateBox box, Filter filter)
        {
            IList<OsmGeo> objects = _source.Get(box, filter);
            foreach (OsmGeo osmGeo in objects)
            {
                switch(osmGeo.Type)
                {
                    case OsmGeoType.Node:
                        _nodesCache.Add(osmGeo.Id.Value, osmGeo as Node);
                        break;
                    case OsmGeoType.Way:
                        _waysCache.Add(osmGeo.Id.Value, osmGeo as Way);
                        break;
                    case OsmGeoType.Relation:
                        _relationsCache.Add(osmGeo.Id.Value, osmGeo as Relation);
                        break;
                }
            }
            return objects;
        }
    }
}
