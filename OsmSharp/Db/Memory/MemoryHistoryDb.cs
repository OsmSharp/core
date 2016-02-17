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
using OsmSharp.Streams;
using System.Linq;

namespace OsmSharp.Db.Memory
{
    /// <summary>
    /// An in-memory data repository of a full-history db.
    /// </summary>
    /// <remarks>This is only a not-very-efficient reference implementation.</remarks>
    public class MemoryHistoryDb : IHistoryDb
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

            _nodeIdManager = new IdManager(() =>
            {
                var id = 0L;
                foreach(var node in _nodes)
                {
                    if (id < node.Key.Id)
                    {
                        id = node.Key.Id;
                    }
                }
                return id;
            });
            _wayIdManager = new IdManager(() =>
            {
                var id = 0L;
                foreach (var way in _ways)
                {
                    if (id < way.Key.Id)
                    {
                        id = way.Key.Id;
                    }
                }
                return id;
            });
            _relationIdManager = new IdManager(() =>
            {
                var id = 0L;
                foreach (var relation in _nodes)
                {
                    if (id < relation.Key.Id)
                    {
                        id = relation.Key.Id;
                    }
                }
                return id;
            });
            _changesetIdManager = new IdManager(() =>
            {
                var id = 0L;
                foreach (var changeset in _changesets)
                {
                    if (id < changeset.Key)
                    {
                        id = changeset.Key;
                    }
                }
                return id;
            });
        }

        private IdManager _nodeIdManager;
        private IdManager _wayIdManager;
        private IdManager _relationIdManager;
        private IdManager _changesetIdManager;

        /// <summary>
        /// Adds osm objects in the db exactly as they are given.
        /// </summary>
        public void Add(OsmGeo osmGeo)
        {
            if (!osmGeo.Id.HasValue) { throw new ArgumentException("OsmGeo object cannot be added without a valid id."); }
            if (!osmGeo.Version.HasValue) { throw new ArgumentException("OsmGeo object cannot be added without a valid version #."); }

            var key = new Key()
            {
                Id = osmGeo.Id.Value,
                Version = osmGeo.Version.Value
            };

            switch (osmGeo.Type)
            {
                case OsmGeoType.Node:
                    _nodeIdManager.Update(osmGeo.Id.Value);
                    _nodes[key] = (osmGeo as Node);
                    return;
                case OsmGeoType.Way:
                    _wayIdManager.Update(osmGeo.Id.Value);
                    _ways[key] = (osmGeo as Way);
                    return;
                case OsmGeoType.Relation:
                    _relationIdManager.Update(osmGeo.Id.Value);
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
        /// Adds the given changeset in the db exactly as given.
        /// </summary>
        public void Add(Changeset meta, OsmChange changes)
        {
            if (!meta.Id.HasValue) { throw new ArgumentException("OsmGeo object cannot be added without a valid id."); }

            _changesetIdManager.Update(meta.Id.Value);

            Tuple<Changeset, List<OsmChange>> existing = null;
            if (!_changesets.TryGetValue(meta.Id.Value, out existing))
            {
                existing = new Tuple<Changeset, List<OsmChange>>(meta, new List<OsmChange>());
                _changesets.Add(meta.Id.Value, existing);
            }
            existing.Item2.Add(changes);
        }

        /// <summary>
        /// Applies the given changeset.
        /// </summary>
        public DiffResultResult ApplyChangeset(long id, OsmChange changeset, bool bestEffort = false)
        {
            Tuple<Changeset, List<OsmChange>> value;
            if (!_changesets.TryGetValue(id, out value))
            {
                throw new DbException("Cannot apply changes for a changeset that doesn't exist.");
            }
            if (!value.Item1.Open.HasValue ||
                !value.Item1.Open.Value)
            {
                throw new DbException("Cannot apply changes for a changeset that isn't open.");
            }

            if (!bestEffort)
            { // validate the changeset first, it's all or nothing so we need to make sure things are correct before applying.
                // throw new NotImplementedException("Changeset validation is not implemented.");
            }

            var results = new List<OsmGeoResult>();
            var status = DiffResultStatus.OK;
            if (changeset.Create != null)
            {
                foreach(var create in changeset.Create)
                {
                    var result = this.ApplyCreate(create);
                    if (result == null)
                    {
                        if (!bestEffort)
                        {
                            return new DiffResultResult("Applying one of the changes failed.");
                        }
                        status = DiffResultStatus.BestEffortOK;
                    }
                    results.Add(result);
                }
            }
            if (changeset.Modify != null)
            {
                foreach (var modify in changeset.Modify)
                {
                    var result = this.ApplyModify(modify);
                    if (result == null)
                    {
                        if (!bestEffort)
                        {
                            return new DiffResultResult("Applying one of the changes failed.");
                        }
                        status = DiffResultStatus.BestEffortOK;
                    }
                    results.Add(result);
                }
            }
            if (changeset.Delete != null)
            {
                foreach (var delete in changeset.Delete)
                {
                    var result = this.ApplyDelete(delete);
                    if (result == null)
                    {
                        if (!bestEffort)
                        {
                            return new DiffResultResult("Applying one of the changes failed.");
                        }
                        status = DiffResultStatus.BestEffortOK;
                    }
                    results.Add(result);
                }
            }
            return new DiffResultResult(new DiffResult()
            {
                Generator = "OsmSharp",
                Version = 0.6,
                Results = results.ToArray()
            }, status);
        }

        /// <summary>
        /// Applies a modification.
        /// </summary>
        private OsmGeoResult ApplyModify(OsmGeo osmGeo)
        {
            if (osmGeo == null) { throw new ArgumentNullException("osmGeo"); }
            if (osmGeo.Id == null) { throw new ArgumentException("Object has no id."); }
            if (osmGeo.Version == null) { throw new ArgumentException("Object has no version."); }

            switch (osmGeo.Type)
            {
                case OsmGeoType.Node:
                    if (!_nodes.ContainsKey(new Key(osmGeo)))
                    {
                        return null;
                    }
                    osmGeo.Version++;
                    _nodes.Add(new Key(osmGeo), osmGeo as Node);
                    return new NodeResult()
                    {
                        NewId = osmGeo.Id,
                        OldId = osmGeo.Id,
                        NewVersion = osmGeo.Version
                    };
                case OsmGeoType.Way:
                    if (!_ways.ContainsKey(new Key(osmGeo)))
                    {
                        return null;
                    }
                    osmGeo.Version++;
                    _ways.Add(new Key(osmGeo), osmGeo as Way);
                    return new WayResult()
                    {
                        NewId = osmGeo.Id,
                        OldId = osmGeo.Id,
                        NewVersion = osmGeo.Version
                    };
                case OsmGeoType.Relation:
                    if (!_relations.ContainsKey(new Key(osmGeo)))
                    {
                        return null;
                    }
                    osmGeo.Version++;
                    _relations.Add(new Key(osmGeo), osmGeo as Relation);
                    return new RelationResult()
                    {
                        NewId = osmGeo.Id,
                        OldId = osmGeo.Id,
                        NewVersion = osmGeo.Version
                    };
            }
            return null;
        }

        /// <summary>
        /// Applies a creation.
        /// </summary>
        private OsmGeoResult ApplyCreate(OsmGeo osmGeo)
        {
            if (osmGeo == null) { throw new ArgumentNullException("osmGeo"); }
            if (osmGeo.Id != null) { throw new ArgumentException("Object already has an id."); }
            if (osmGeo.Version != null) { throw new ArgumentException("Object already has a version."); }

            osmGeo.Version = 1; // first version of this object.
            switch (osmGeo.Type)
            {
                case OsmGeoType.Node:
                    osmGeo.Id = _nodeIdManager.GetNew();
                    _nodes.Add(new Key(osmGeo), osmGeo as Node);
                    return new NodeResult()
                    {
                        OldId = null,
                        NewId = osmGeo.Id,
                        NewVersion = osmGeo.Version
                    };
                case OsmGeoType.Way:
                    osmGeo.Id = _wayIdManager.GetNew();
                    _ways.Add(new Key(osmGeo), osmGeo as Way);
                    return new WayResult()
                    {
                        OldId = null,
                        NewId = osmGeo.Id,
                        NewVersion = osmGeo.Version
                    };
                case OsmGeoType.Relation:
                    osmGeo.Id = _relationIdManager.GetNew();
                    _relations.Add(new Key(osmGeo), osmGeo as Relation);

                    return new RelationResult()
                    {
                        OldId = null,
                        NewId = osmGeo.Id,
                        NewVersion = osmGeo.Version
                    };
            }
            return null;
        }

        /// <summary>
        /// Applies a deletion.
        /// </summary>
        private OsmGeoResult ApplyDelete(OsmGeo osmGeo)
        {
            if (osmGeo == null) { throw new ArgumentNullException("osmGeo"); }
            if (osmGeo.Id == null) { throw new ArgumentException("Object has no id."); }
            if (osmGeo.Version == null) { throw new ArgumentException("Object has no version."); }

            var key = new Key(osmGeo);
            switch (osmGeo.Type)
            {
                case OsmGeoType.Node:
                    Node node;
                    if (!_nodes.TryGetValue(new Key(osmGeo), out node))
                    {
                        return null;
                    }
                    node.Visible = false;
                    return new NodeResult()
                    {
                        OldId = osmGeo.Id.Value,
                        NewId = null,
                        NewVersion = null
                    };
                case OsmGeoType.Way:
                    Way way;
                    if (!_ways.TryGetValue(new Key(osmGeo), out way))
                    {
                        return null;
                    }
                    way.Visible = false;
                    return new WayResult()
                    {
                        OldId = osmGeo.Id.Value,
                        NewId = null,
                        NewVersion = null
                    };
                case OsmGeoType.Relation:
                    Relation relation;
                    if (!_relations.TryGetValue(new Key(osmGeo), out relation))
                    {
                        return null;
                    }
                    return new RelationResult()
                    {
                        OldId = osmGeo.Id.Value,
                        NewId = null,
                        NewVersion = null
                    };
            }
            return null;
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
        /// Closes the changeset with the given id.
        /// </summary>
        public bool CloseChangeset(long id)
        {
            Tuple<Changeset, List<OsmChange>> existing;
            if (!_changesets.TryGetValue(id, out existing))
            {
                return false;
            }
            if (existing.Item1.Open.HasValue &&
                existing.Item1.Open.Value)
            {
                existing.Item1.Open = false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets all the objects in the form of an osm stream source.
        /// </summary>
        public OsmStreamSource Get()
        {
            return new OsmSharp.Streams.OsmEnumerableStreamSource(
               _nodes.Values.Cast<OsmGeo>().Concat(
                   _ways.Values.Cast<OsmGeo>().Concat(
                    _relations.Values.Cast<OsmGeo>())));
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
        /// Gets all osm objects with the given types and the given id's.
        /// </summary>
        public IList<OsmGeo> Get(OsmGeoType type, IList<long> id)
        {
            var osmGeos = new List<OsmGeo>(id.Count);
            for (var i = 0; i < osmGeos.Count; i++)
            {
                osmGeos.Add(this.Get(type, id[i]));
            }
            return osmGeos;
        }

        /// <summary>
        /// Gets an osm object of the given type, the given id and the given version #.
        /// </summary>
        public OsmGeo Get(OsmGeoType type, long id, int version)
        {
            var key = new Key()
            {
                Id = id,
                Version = version
            };

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

        /// <summary>
        /// Gets all osm objects with the given types, the given id's and the given version #'s.
        /// </summary>
        public IList<OsmGeo> Get(OsmGeoType type, IList<long> id, IList<int> version)
        {
            var osmGeos = new List<OsmGeo>(id.Count);
            for (var i = 0; i < osmGeos.Count; i++)
            {
                osmGeos.Add(this.Get(type, id[i], version[i]));
            }
            return osmGeos;
        }

        /// <summary>
        /// Gets all latest versions of osm objects within the given bounding box.
        /// </summary>
        public IList<OsmGeo> Get(float minLatitude, float minLongitude, float maxLatitude, float maxLongitude)
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
            return found;
        }

        /// <summary>
        /// Opens a new changeset.
        /// </summary>
        public long OpenChangeset(Changeset info)
        {
            if (info == null) { throw new ArgumentNullException("info"); }
            if (info.Id.HasValue && info.Id != 0) { throw new ArgumentException("Changeset already has an id."); }
            
            info.Id = _changesetIdManager.GetNew();
            info.Open = true;
            _changesets.Add(info.Id.Value, new Tuple<Changeset, List<OsmChange>>(
                info, new List<OsmChange>()));
            return info.Id.Value;
        }

        /// <summary>
        /// Updates the changeset info.
        /// </summary>
        public bool UpdateChangesetInfo(Changeset info)
        {
            Tuple<Changeset, List<OsmChange>> existing;
            if (!_changesets.TryGetValue(info.Id.Value, out existing))
            {
                return false;
            }
            if (existing.Item1.Open.HasValue &&
                existing.Item1.Open.Value)
            {
                _changesets.Remove(info.Id.Value);
                _changesets[info.Id.Value] = new Tuple<Changeset, List<OsmChange>>(
                    info, existing.Item2);
            }
            return false;
        }

        struct Key
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

        struct IdManager
        {
            private long? _nextId;
            private Func<long> _getLatestId;

            /// <summary>
            /// Creates a new id manager.
            /// </summary>
            public IdManager(Func<long> getLatestId)
            {
                if (getLatestId == null) { throw new ArgumentNullException("getLatestId"); }

                _getLatestId = getLatestId;
                _nextId = null;
            }

            /// <summary>
            /// Gets an id for a new node.
            /// </summary>
            public long GetNew()
            {
                if (!_nextId.HasValue)
                {
                    _nextId = _getLatestId() + 1;
                }
                _nextId++;
                return _nextId.Value - 1;
            }

            /// <summary>
            /// Updates the new node id.
            /// </summary>
            public void Update(long id)
            {
                if (_nextId.HasValue &&
                    id >= _nextId.Value)
                {
                    _nextId = id + 1;
                }
            }
        }
    }
}