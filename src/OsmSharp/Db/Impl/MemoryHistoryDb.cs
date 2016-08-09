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

using System;
using System.Collections.Generic;
using OsmSharp.Changesets;
using System.Linq;

namespace OsmSharp.Db.Impl
{
    /// <summary>
    /// An in-memory data repository of a full-history db.
    /// </summary>
    /// <remarks>This is only a not-very-efficient reference implementation.</remarks>
    public class MemoryHistoryDb : IHistoryDbImpl
    {
        private readonly Dictionary<Key, Node> _nodes;
        private readonly Dictionary<Key, Way> _ways;
        private readonly Dictionary<Key, Relation> _relations;
        private readonly Dictionary<long, Tuple<Changeset, List<OsmChange>>> _changesets;

        /// <summary>
        /// Creates a new history db.
        /// </summary>
        public MemoryHistoryDb()
        {
            _nodes = new Dictionary<Key, Node>();
            _ways = new Dictionary<Key, Way>();
            _relations = new Dictionary<Key, Relation>();
            _changesets = new Dictionary<long, Tuple<Changeset, List<OsmChange>>>();
        }

        /// <summary>
        /// Clears all data from this db.
        /// </summary>
        public void Clear()
        {
            _nodes.Clear();
            _ways.Clear();
            _relations.Clear();

            _changesets.Clear();
        }

        /// <summary>
        /// Adds osm objects in the db exactly as they are given.
        /// </summary>
        public void Add(OsmGeo osmGeo)
        {
            if (!osmGeo.Id.HasValue) { throw new ArgumentException("OsmGeo object cannot be added without a valid id."); }
            if (!osmGeo.Version.HasValue) { throw new ArgumentException("OsmGeo object cannot be added without a valid version #."); }

            var key = new Key(osmGeo);

            switch (osmGeo.Type)
            {
                case OsmGeoType.Node:
                    _nodes[key] = (osmGeo as Node);
                    return;
                case OsmGeoType.Way:
                    _ways[key] = (osmGeo as Way);
                    return;
                case OsmGeoType.Relation:
                    _relations[key] = (osmGeo as Relation);
                    return;
            }
        }

        /// <summary>
        /// Adds osm objects in the db exactly as they are given.
        /// </summary>
        public void Add(IEnumerable<OsmGeo> osmGeos)
        {
            foreach (var osmGeo in osmGeos)
            {
                this.Add(osmGeo);
            }
        }

        /// <summary>
        /// Gets all the visible objects in the form of an osm stream source.
        /// </summary>
        public IEnumerable<OsmGeo> Get()
        {
            var nodes = new List<OsmGeo>(_nodes.Values.Where(x => x.Visible.HasValue && x.Visible.Value));
            nodes.Sort((x, y) => x.Id.Value.CompareTo(y.Id.Value));
            var ways = new List<OsmGeo>(_ways.Values.Where(x => x.Visible.HasValue && x.Visible.Value));
            ways.Sort((x, y) => x.Id.Value.CompareTo(y.Id.Value));
            var relations = new List<OsmGeo>(_ways.Values.Where(x => x.Visible.HasValue && x.Visible.Value));
            relations.Sort((x, y) => x.Id.Value.CompareTo(y.Id.Value));

            return nodes.Concat(ways).Concat(relations);
        }

        /// <summary>
        /// Gets all the objects in the form of an osm stream source.
        /// </summary>
        public IEnumerable<OsmGeo> GetWithArchived()
        {
            var nodes = new List<OsmGeo>(_nodes.Values);
            nodes.Sort((x, y) =>
            {
                if (x.Id == y.Id)
                {
                    return x.Version.Value.CompareTo(y.Version.Value);
                }
                return x.Id.Value.CompareTo(y.Id.Value);
            });
            var ways = new List<OsmGeo>(_ways.Values);
            ways.Sort((x, y) =>
            {
                if (x.Id == y.Id)
                {
                    return x.Version.Value.CompareTo(y.Version.Value);
                }
                return x.Id.Value.CompareTo(y.Id.Value);
            });
            var relations = new List<OsmGeo>(_ways.Values);
            relations.Sort((x, y) =>
            {
                if (x.Id == y.Id)
                {
                    return x.Version.Value.CompareTo(y.Version.Value);
                }
                return x.Id.Value.CompareTo(y.Id.Value);
            });

            return nodes.Concat(ways).Concat(relations);
        }

        /// <summary>
        /// Gets all visible objects for the given keys.
        /// </summary>
        public IEnumerable<OsmGeo> Get(IEnumerable<OsmGeoKey> keys)
        {
            var osmGeos = new List<OsmGeo>();
            foreach (var key in keys)
            {
                osmGeos.Add(this.Get(key.Type, key.Id));
            }
            osmGeos.Sort(Sort);
            return osmGeos;
        }

        /// <summary>
        /// Gets all objects for the given keys.
        /// </summary>
        public IEnumerable<OsmGeo> Get(IEnumerable<OsmGeoVersionKey> keys)
        {
            var osmGeos = new List<OsmGeo>();
            foreach (var key in keys)
            {
                osmGeos.Add(this.Get(key.Type, key.Id, key.Version));
            }
            osmGeos.Sort(Sort);
            return osmGeos;
        }

        /// <summary>
        /// Gets all visible objects for the given keys.
        /// </summary>
        public IEnumerable<OsmGeo> GetWithArchived(IEnumerable<OsmGeoKey> keys)
        {
            var osmGeos = new List<OsmGeo>();
            foreach (var key in keys)
            {
                osmGeos.Add(this.Get(key.Type, key.Id));
            }
            osmGeos.Sort(Sort);
            return osmGeos;
        }

        /// <summary>
        /// Gets all visbible objects within the given bounding box.
        /// </summary>
        public IEnumerable<OsmGeo> Get(float minLatitude, float minLongitude, float maxLatitude, float maxLongitude)
        {
            var nodesInBox = new HashSet<long>();
            var nodesToInclude = new HashSet<long>();
            var waysToInclude = new HashSet<long>();
            var relationsToInclude = new HashSet<long>();

            foreach (var node in _nodes.Values)
            {
                if (node.Longitude.HasValue && node.Latitude.HasValue &&
                    node.Visible.HasValue && node.Visible.Value)
                {
                    if (Utilities.IsInside(minLatitude, minLongitude, maxLatitude, maxLongitude, node.Latitude.Value, node.Longitude.Value))
                    {
                        nodesInBox.Add(node.Id.Value);
                    }
                }
            }

            foreach (var way in _ways.Values)
            {
                if (way.Nodes != null &&
                    way.Visible.HasValue && way.Visible.Value)
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
                if (relation.Members != null &&
                    relation.Visible.HasValue && relation.Visible.Value)
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
            found.Sort(Sort);
            return found;
        }

        /// <summary>
        /// Archives the objects for the given key.
        /// </summary>
        public void Archive(IEnumerable<OsmGeoKey> keys)
        {
            var osmGeos = this.Get(keys);
            foreach (var osmGeo in osmGeos)
            {
                osmGeo.Visible = false;
            }
        }

        /// <summary>
        /// Gets the last id.
        /// </summary>
        public long GetLastId(OsmGeoType type)
        {
            switch (type)
            {
                case OsmGeoType.Node:
                    if (_nodes.Count == 0)
                    {
                        return -1;
                    }
                    return _nodes.Keys.Select(x => x.Id).Max();
                case OsmGeoType.Way:
                    if (_ways.Count == 0)
                    {
                        return -1;
                    }
                    return _ways.Keys.Select(x => x.Id).Max();
                case OsmGeoType.Relation:
                    if (_relations.Count == 0)
                    {
                        return -1;
                    }
                    return _relations.Keys.Select(x => x.Id).Max();
            }
            throw new Exception("Invalid OsmGeo type.");
        }

        /// <summary>
        /// Gets the last changeset id.
        /// </summary>
        /// <returns></returns>
        public long GetLastChangesetId()
        {
            if (_changesets.Count == 0)
            {
                return -1;
            }
            return _changesets.Keys.Max();
        }

        /// <summary>
        /// Adds or updates the given changeset.
        /// </summary>
        public void AddOrUpdate(Changeset info)
        {
            Tuple<Changeset, List<OsmChange>> existing;
            if (!_changesets.TryGetValue(info.Id.Value, out existing))
            {
                _changesets[info.Id.Value] = new Tuple<Changeset, List<OsmChange>>(
                    info, new List<OsmChange>());
                return;
            }
            _changesets.Remove(info.Id.Value);
            _changesets[info.Id.Value] = new Tuple<Changeset, List<OsmChange>>(
                info, existing.Item2);
        }

        /// <summary>
        /// Adds changes.
        /// </summary>
        public void AddChanges(long id, OsmChange changes)
        {
            Tuple<Changeset, List<OsmChange>> existing;
            if (!_changesets.TryGetValue(id, out existing) ||
                existing.Item1.ClosedAt.HasValue)
            {
                throw new Exception("Cannot add changes to a non-existing or closed changetset.");
            }
            existing.Item2.Add(changes);
        }

        /// <summary>
        /// Gets the changes.
        /// </summary>
        public Changeset GetChangeset(long id)
        {
            Tuple<Changeset, List<OsmChange>> existing;
            if (!_changesets.TryGetValue(id, out existing))
            {
                return null;
            }
            return existing.Item1;
        }

        /// <summary>
        /// Gets the changes for the given id.
        /// </summary>
        public OsmChange GetChanges(long id)
        {
            Tuple<Changeset, List<OsmChange>> existing;
            if (!_changesets.TryGetValue(id, out existing))
            {
                return null;
            }
            return existing.Item2.FirstOrDefault();
        }
        
        /// <summary>
        /// Gets an osm object of the given type, the given id and the given version #.
        /// </summary>
        public OsmGeo Get(OsmGeoType type, long id)
        {
            var version = -1;
            switch (type)
            {
                case OsmGeoType.Node:
                    return _nodes.Values.FirstOrDefault(x =>
                    {
                        if (x.Id == id &&
                            x.Version.Value > version)
                        {
                            version = x.Version.Value;
                            return true;
                        }
                        return false;
                    });
                case OsmGeoType.Way:
                    return _ways.Values.FirstOrDefault(x =>
                    {
                        if (x.Id == id &&
                            x.Version.Value > version)
                        {
                            version = x.Version.Value;
                            return true;
                        }
                        return false;
                    });
                case OsmGeoType.Relation:
                    return _relations.Values.FirstOrDefault(x =>
                    {
                        if (x.Id == id &&
                            x.Version.Value > version)
                        {
                            version = x.Version.Value;
                            return true;
                        }
                        return false;
                    });
            }
            throw new Exception(string.Format("Uknown OsmGeoType: {0}.",
                type.ToInvariantString()));
        }

        /// <summary>
        /// Gets an osm object of the given type, the given id and the given version #.
        /// </summary>
        public OsmGeo Get(OsmGeoType type, long id, int version)
        {
            var key = new Key(id, version);

            switch (type)
            {
                case OsmGeoType.Node:
                    Node node;
                    if (!_nodes.TryGetValue(key, out node))
                    {
                        return null;
                    }
                    return node;
                case OsmGeoType.Way:
                    Way way;
                    if (!_ways.TryGetValue(key, out way))
                    {
                        return null;
                    }
                    return way;
                case OsmGeoType.Relation:
                    Relation relation;
                    if (!_relations.TryGetValue(key, out relation))
                    {
                        return null;
                    }
                    return relation;
            }
            throw new Exception(string.Format("Uknown OsmGeoType: {0}.",
                type.ToInvariantString()));
        }
        
        private static System.Comparison<OsmGeo> Sort = (x, y) =>
        {
            return x.CompareByIdVersionAndType(y);
        };

        class Key
        {
            public Key(long id, int version)
            {
                this.Id = id;
                this.Version = version;
            }

            public Key(OsmGeo osmGeo)
                : this(osmGeo.Id.Value, osmGeo.Version.Value)
            {

            }

            public long Id { get; set; }

            public int Version { get; set; }

            public override bool Equals(object obj)
            {
                if (obj is Key)
                {
                    var other = (Key)obj;

                    return other.Id == this.Id &&
                        other.Version == this.Version;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode() ^
                    this.Version.GetHashCode();
            }
        }
    }
}