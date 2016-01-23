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

using OsmSharp.Math.Geo;
using OsmSharp.Osm.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using OsmSharp.Osm.Streams;

namespace OsmSharp.Osm.Data
{
    /// <summary>
    /// An in-memory data repository of OSM data primitives.
    /// </summary>
    /// <remarks>This is only a not-very-efficient reference implementation.</remarks>
    public class MemoryDataSource : ISnapshotDb
    {
        private readonly Dictionary<long, Node> _nodes;
        private readonly Dictionary<long, Way> _ways;
        private readonly Dictionary<long, Relation> _relations;

        /// <summary>
        /// Creates a new memory data structure using the default geometry interpreter.
        /// </summary>
        public MemoryDataSource()
        {
            _nodes = new Dictionary<long, Node>();
            _ways = new Dictionary<long, Way>();
            _relations = new Dictionary<long, Relation>();
        }

        /// <summary>
        /// Creates a new memory data structure using the default geometry interpreter.
        /// </summary>
        public MemoryDataSource(params OsmGeo[] initial)
            : this(initial as IEnumerable<OsmGeo>)
        {

        }

        /// <summary>
        /// Creates a new memory data structure using the default geometry interpreter.
        /// </summary>
        public MemoryDataSource(IEnumerable<OsmGeo> initial)
        {
            _nodes = new Dictionary<long, Node>();
            _ways = new Dictionary<long, Way>();
            _relations = new Dictionary<long, Relation>();

            this.AddOrUpdate(initial);
        }

        /// <summary>
        /// Gets all the objects in the form of an osm stream source.
        /// </summary>
        /// <returns></returns>
        public OsmStreamSource Get()
        {
            return new OsmSharp.Osm.Streams.Collections.OsmEnumerableStreamSource(
                _nodes.Values.Cast<OsmGeo>().Concat(
                    _ways.Values.Cast<OsmGeo>().Concat(
                     _relations.Values.Cast<OsmGeo>())));
        }

        /// <summary>
        /// Returns all the objects within a given bounding box and filtered by a given filter.
        /// </summary>
        public IList<OsmGeo> Get(float minLatitude, float minLongitude, float maxLatitude, float maxLongitude, Filter filter)
        {
            var box = new GeoCoordinateBox(
                new GeoCoordinate(minLatitude, minLongitude),
                new GeoCoordinate(maxLatitude, maxLongitude));

            var nodesInBox = new HashSet<long>();
            var nodesToInclude = new HashSet<long>();
            var waysToInclude = new HashSet<long>();
            var relationsToInclude = new HashSet<long>();

            foreach(var node in _nodes.Values)
            {
                if (box.Contains(node.Longitude.Value, node.Latitude.Value) &&
                    (filter == null || filter.Evaluate(node)))
                {
                    nodesInBox.Add(node.Id.Value);
                }
            }

            foreach(var way in _ways.Values)
            {
                if (way.Nodes != null)
                {
                    if (filter == null || filter.Evaluate(way))
                    {
                        for (var n = 0; n < way.Nodes.Count; n++)
                        {
                            if (nodesInBox.Contains(way.Nodes[n]))
                            {
                                waysToInclude.Add(way.Id.Value);
                                for (var n1 = 0; n1 < way.Nodes.Count; n1++)
                                {
                                    nodesToInclude.Add(way.Nodes[n1]);
                                }
                                break;
                            }
                        }
                    }
                }
            }

            foreach(var relation in _relations.Values)
            {
                if (filter == null || filter.Evaluate(relation))
                {
                    if (relation.Members != null)
                    {
                        for (var m = 0; m < relation.Members.Count; m++)
                        {
                            var relationFound = false;
                            var member = relation.Members[m];
                            switch (member.MemberType.Value)
                            {
                                case OsmGeoType.Node:
                                    if (nodesInBox.Contains(member.MemberId.Value))
                                    {
                                        relationFound = true;
                                    }
                                    break;
                                case OsmGeoType.Way:
                                    if (waysToInclude.Contains(member.MemberId.Value))
                                    {
                                        relationFound = true;
                                    }
                                    break;
                            }
                            if (relationFound)
                            {
                                relationsToInclude.Add(relation.Id.Value);
                                break;
                            }
                        }
                    }
                }
            }

            var found = new List<OsmGeo>();
            found.AddRange(_nodes.Values.Where(x => nodesInBox.Contains(x.Id.Value) || nodesToInclude.Contains(x.Id.Value)));
            found.AddRange(_ways.Values.Where(x => waysToInclude.Contains(x.Id.Value)));
            found.AddRange(_relations.Values.Where(x => relationsToInclude.Contains(x.Id.Value)));
            return found;
        }

        /// <summary>
        /// Clears all data.
        /// </summary>
        public void Clear()
        {
            _nodes.Clear();
            _ways.Clear();
            _relations.Clear();
        }

        /// <summary>
        /// Adds or updates the given osm object in the db exactly as given.
        /// </summary>
        /// <remarks>
        /// - Replaces objects that already exist with the given id.
        /// </remarks>
        public void AddOrUpdate(OsmGeo osmGeo)
        {
            if (osmGeo == null) throw new ArgumentNullException();
            if (!osmGeo.Id.HasValue) throw new ArgumentException("Objects without an id cannot be added!");
            
            switch(osmGeo.Type)
            {
                case OsmGeoType.Node:
                    _nodes[osmGeo.Id.Value] = osmGeo as Node;
                    break;
                case OsmGeoType.Way:
                    this.AddWay(osmGeo as Way);
                    break;
                case OsmGeoType.Relation:
                    this.AddRelation(osmGeo as Relation);
                    break;
            }
        }

        /// <summary>
        /// Adds or updates osm objects in the db exactly as they are given.
        /// </summary>
        /// <remarks>
        /// - Replaces objects that already exist with the given id.
        /// </remarks>
        public void AddOrUpdate(IEnumerable<OsmGeo> osmGeos)
        {
            foreach(var osmGeo in osmGeos)
            {
                this.AddOrUpdate(osmGeos);
            }
        }
        
        /// <summary>
        /// Gets an osm object of the given type and the given id.
        /// </summary>
        public OsmGeo Get(OsmGeoType type, long id)
        {
            switch(type)
            {
                case OsmGeoType.Node:
                    Node node;
                    if(!_nodes.TryGetValue(id, out node))
                    {
                        return null;
                    }
                    return node;
                case OsmGeoType.Way:
                    Way way;
                    if (!_ways.TryGetValue(id, out way))
                    {
                        return null;
                    }
                    return way;
                case OsmGeoType.Relation:
                    Relation relation;
                    if (!_relations.TryGetValue(id, out relation))
                    {
                        return null;
                    }
                    return relation;
            }
            return null;
        }

        /// <summary>
        /// Gets all osm objects with the given types and the given id's.
        /// </summary>
        public IList<OsmGeo> Get(IList<OsmGeoType> type, IList<long> id)
        {
            if (type == null) { throw new ArgumentNullException("type"); }
            if (id == null) { throw new ArgumentNullException("id"); }
            if (id.Count != type.Count) { throw new ArgumentException("Type and id lists need to have the same size."); }

            var result = new List<OsmGeo>();
            for (int i = 0; i < id.Count; i++)
            {
                result.Add(this.Get(type[i], id[i]));
            }
            return result;
        }

        /// <summary>
        /// Deletes the osm object with the given type, the given id without applying a changeset.
        /// </summary>
        public bool Delete(OsmGeoType type, long id)
        {
            switch(type)
            {
                case OsmGeoType.Node:
                    return _nodes.Remove(id);
                case OsmGeoType.Way:
                    return _ways.Remove(id);
                case OsmGeoType.Relation:
                    return _relations.Remove(id);
            }
            return false;
        }
        
        /// <summary>
        /// Deletes all osm objects with the given types and the given id's.
        /// </summary>
        public IList<bool> Delete(IList<OsmGeoType> type, IList<long> id)
        {
            if (type == null) { throw new ArgumentNullException("type"); }
            if (id == null) { throw new ArgumentNullException("id"); }
            if (id.Count != type.Count) { throw new ArgumentException("Type and id lists need to have the same size."); }

            var result = new List<bool>();
            for (int i = 0; i < id.Count; i++)
            {
                result.Add(this.Delete(type[i], id[i]));
            }
            return result;
        }

        /// <summary>
        /// Applies the given changeset.
        /// </summary>
        /// <param name="changeset">The changeset to apply.</param>
        /// <param name="atomic">Then true, it's the entire changeset or nothing. When false the changeset is applied using best-effort.</param>
        /// <returns>True when the entire changeset was applied without issues, false otherwise.</returns>
        public bool ApplyChangeset(ChangeSet changeset, bool atomic = false)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Adds a way.
        /// </summary>
        private void AddWay(Way way)
        {
            _ways[way.Id.Value] = way;
        }

        /// <summary>
        /// Adds a relation.
        /// </summary>
        private void AddRelation(Relation relation)
        {
            _relations[relation.Id.Value] = relation;
        }
    }
}