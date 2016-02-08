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

namespace OsmSharp.Db
{
    /// <summary>
    /// An in-memory data repository of a full-history db.
    /// </summary>
    /// <remarks>This is only a not-very-efficient reference implementation.</remarks>
    public class MemoryHistoryDb : IHistoryDb
    {
        private readonly List<Node> _nodes;
        private readonly List<Way> _ways;
        private readonly List<Relation> _relations;
        private readonly List<Tuple<Changeset, List<OsmChange>>> _changesets;

        /// <summary>
        /// Creates a new history db.
        /// </summary>
        public MemoryHistoryDb()
        {
            _nodes = new List<Node>();
            _ways = new List<Way>();
            _relations = new List<Relation>();

            _changesets = new List<Tuple<Changeset, List<OsmChange>>>();
        }

        /// <summary>
        /// Adds osm objects in the db exactly as they are given.
        /// </summary>
        public void Add(OsmGeo osmGeo)
        {
            switch(osmGeo.Type)
            {
                case OsmGeoType.Node:
                    if(_nodes.Any(x => x.Id == osmGeo.Id && 
                        x.Type == osmGeo.Type))
                    {
                        throw new ArgumentException("A node with the id/type already exists.");
                    }
                    _nodes.Add(osmGeo as Node);
                    return;
                case OsmGeoType.Way:
                    if (_ways.Any(x => x.Id == osmGeo.Id &&
                         x.Type == osmGeo.Type))
                    {
                        throw new ArgumentException("A way with the id/type already exists.");
                    }
                    _ways.Add(osmGeo as Way);
                    return;
                case OsmGeoType.Relation:
                    if (_relations.Any(x => x.Id == osmGeo.Id &&
                         x.Type == osmGeo.Type))
                    {
                        throw new ArgumentException("A relation with the id/type already exists.");
                    }
                    _relations.Add(osmGeo as Relation);
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
                this.Add(osmGeos);
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
            var existing = _changesets.FirstOrDefault(x => x.Item1.Id == id);
            if (existing == null)
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
                _nodes.Cast<OsmGeo>().Concat(
                    _ways.Cast<OsmGeo>().Concat(
                     _relations.Cast<OsmGeo>())));
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
                    return _nodes.FirstOrDefault(x =>
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
                    return _ways.FirstOrDefault(x =>
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
                    return _relations.FirstOrDefault(x =>
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
            switch (type)
            {
                case OsmGeoType.Node:
                    return _nodes.FirstOrDefault(x =>
                    {
                        return x.Id == id && x.Version == version;
                    });
                case OsmGeoType.Way:
                    return _ways.FirstOrDefault(x =>
                    {
                        return x.Id == id && x.Version == version;
                    });
                case OsmGeoType.Relation:
                    return _relations.FirstOrDefault(x =>
                    {
                        return x.Id == id && x.Version == version;
                    });
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
            var existing = _changesets.FirstOrDefault(x => x.Item1.Id == info.Id);
            if (existing == null)
            {
                return false;
            }
            if (existing.Item1.Open.HasValue &&
                existing.Item1.Open.Value)
            {
                _changesets.Remove(existing);
                _changesets.Add(new Tuple<Changeset, List<OsmChange>>(
                    info, existing.Item2));
            }
            return false;
        }
    }
}