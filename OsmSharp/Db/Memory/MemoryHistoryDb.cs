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
        }

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
            switch(osmGeo.Type)
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
        /// Adds the given changeset in the db exactly as given.
        /// </summary>
        public void Add(Changeset meta, OsmChange changes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Applies the given changeset.
        /// </summary>
        public DiffResultResult ApplyChangeset(long id, OsmChange changeset, bool bestEffort = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Clears all data from this db.
        /// </summary>
        public void Clear()
        {
            _nodes.Clear();
            _ways.Clear();
            _relations.Clear();
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
        public IList<OsmGeo> Get(IList<OsmGeoType> type, IList<long> id)
        {
            var osmGeos = new List<OsmGeo>(type.Count);
            for (var i = 0; i < osmGeos.Count; i++)
            {
                osmGeos.Add(this.Get(type[i], id[i]));
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
        public IList<OsmGeo> Get(IList<OsmGeoType> type, IList<long> id, IList<int> version)
        {
            var osmGeos = new List<OsmGeo>(type.Count);
            for (var i = 0; i < osmGeos.Count; i++)
            {
                osmGeos.Add(this.Get(type[i], id[i], version[i]));
            }
            return osmGeos;
        }

        /// <summary>
        /// Returns all the objects within a given bounding box and filtered by a given filter.
        /// </summary>
        public IList<OsmGeo> Get(float minLatitude, float minLongitude, float maxLatitude, float maxLongitude)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Opens a new changeset.
        /// </summary>
        public long OpenChangeset(Changeset info)
        {
            throw new NotImplementedException();
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