// The MIT License (MIT)

// Copyright (c) 2016 Ben Abelshausen

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using OsmSharp.Changesets;
using OsmSharp.Streams;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Db
{
    /// <summary>
    /// An in-memory data repository of OSM data primitives.
    /// </summary>
    /// <remarks>This is only a not-very-efficient reference implementation.</remarks>
    public class MemorySnapshotDb : ISnapshotDb
    {
        private readonly Dictionary<long, Node> _nodes;
        private readonly Dictionary<long, Way> _ways;
        private readonly Dictionary<long, Relation> _relations;

        /// <summary>
        /// Creates a new memory data structure using the default geometry interpreter.
        /// </summary>
        public MemorySnapshotDb()
        {
            _nodes = new Dictionary<long, Node>();
            _ways = new Dictionary<long, Way>();
            _relations = new Dictionary<long, Relation>();
        }

        /// <summary>
        /// Creates a new memory data structure using the default geometry interpreter.
        /// </summary>
        public MemorySnapshotDb(params OsmGeo[] initial)
            : this(initial as IEnumerable<OsmGeo>)
        {

        }

        /// <summary>
        /// Creates a new memory data structure using the default geometry interpreter.
        /// </summary>
        public MemorySnapshotDb(IEnumerable<OsmGeo> initial)
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
            return new OsmSharp.Streams.OsmEnumerableStreamSource(
                _nodes.Values.Cast<OsmGeo>().Concat(
                    _ways.Values.Cast<OsmGeo>().Concat(
                     _relations.Values.Cast<OsmGeo>())));
        }

        /// <summary>
        /// Returns all the objects within a given bounding box.
        /// </summary>
        public IList<OsmGeo> Get(float minLatitude, float minLongitude, float maxLatitude, float maxLongitude)
        {
            var nodesInBox = new HashSet<long>();
            var nodesToInclude = new HashSet<long>();
            var waysToInclude = new HashSet<long>();
            var relationsToInclude = new HashSet<long>();

            foreach (var node in _nodes.Values)
            {
                if (node.Longitude.HasValue && node.Latitude.HasValue)
                {
                    if (Utilities.IsInside(minLatitude, minLongitude, maxLatitude, maxLongitude, node.Latitude.Value, node.Longitude.Value))
                    {
                        nodesInBox.Add(node.Id.Value);
                    }
                }
            }

            foreach (var way in _ways.Values)
            {
                if (way.Nodes != null)
                {
                    for (var n = 0; n < way.Nodes.Length; n++)
                    {
                        if (nodesInBox.Contains(way.Nodes[n]))
                        {
                            waysToInclude.Add(way.Id.Value);
                            for (var n1 = 0; n1 < way.Nodes.Length; n1++)
                            {
                                nodesToInclude.Add(way.Nodes[n1]);
                            }
                            break;
                        }
                    }
                }
            }

            foreach (var relation in _relations.Values)
            {
                if (relation.Members != null)
                {
                    for (var m = 0; m < relation.Members.Length; m++)
                    {
                        var relationFound = false;
                        var member = relation.Members[m];
                        switch (member.Type)
                        {
                            case OsmGeoType.Node:
                                if (nodesInBox.Contains(member.Id))
                                {
                                    relationFound = true;
                                }
                                break;
                            case OsmGeoType.Way:
                                if (waysToInclude.Contains(member.Id))
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

            switch (osmGeo.Type)
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
            foreach (var osmGeo in osmGeos)
            {
                this.AddOrUpdate(osmGeo);
            }
        }

        /// <summary>
        /// Gets an osm object of the given type and the given id.
        /// </summary>
        public OsmGeo Get(OsmGeoType type, long id)
        {
            switch (type)
            {
                case OsmGeoType.Node:
                    Node node;
                    if (!_nodes.TryGetValue(id, out node))
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
        /// Returns true if an osm object of the given type and the given id exists.
        /// </summary>
        public bool Exists(OsmGeoType type, long id)
        {
            switch (type)
            {
                case OsmGeoType.Node:
                    Node node;
                    if (!_nodes.TryGetValue(id, out node))
                    {
                        return false;
                    }
                    return true;
                case OsmGeoType.Way:
                    Way way;
                    if (!_ways.TryGetValue(id, out way))
                    {
                        return false;
                    }
                    return true;
                case OsmGeoType.Relation:
                    Relation relation;
                    if (!_relations.TryGetValue(id, out relation))
                    {
                        return false;
                    }
                    return true;
            }
            return false;
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
        /// Returns true for osm objects with the given types and the given id's when they exist.
        /// </summary>
        public IList<bool> Exists(IList<OsmGeoType> type, IList<long> id)
        {
            if (type == null) { throw new ArgumentNullException("type"); }
            if (id == null) { throw new ArgumentNullException("id"); }
            if (id.Count != type.Count) { throw new ArgumentException("Type and id lists need to have the same size."); }

            var result = new List<bool>();
            for (int i = 0; i < id.Count; i++)
            {
                result.Add(this.Exists(type[i], id[i]));
            }
            return result;
        }

        /// <summary>
        /// Deletes the osm object with the given type, the given id without applying a changeset.
        /// </summary>
        public bool Delete(OsmGeoType type, long id)
        {
            switch (type)
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
        public DiffResultResult ApplyChangeset(OsmChange changeset, bool atomic = false)
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
