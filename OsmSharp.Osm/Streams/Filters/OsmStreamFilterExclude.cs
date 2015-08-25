// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2014 Abelshausen Ben
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

namespace OsmSharp.Osm.Streams.Filters
{
    /// <summary>
    /// Excludes objects from the stream that are already present in the other streams.
    /// </summary>
    public class OsmStreamFilterExclude : OsmStreamFilter
    {
        /// <summary>
        /// Holds all the sources.
        /// </summary>
        private List<OsmStreamSource> _sources;

        /// <summary>
        /// Flag to configure excluding nodes.
        /// </summary>
        private bool _excludeNodes = true;

        /// <summary>
        /// Flag to configure excluding ways.
        /// </summary>
        private bool _excludeWays = true;

        /// <summary>
        /// Flags to configure excluding relations.
        /// </summary>
        private bool _excludeRelations = true;

        /// <summary>
        /// Holds an index of nodes to exclude.
        /// </summary>
        private HashSet<long> _nodesToExclude;

        /// <summary>
        /// Holds an index of ways to exclude.
        /// </summary>
        private HashSet<long> _waysToExclude;
        
        /// <summary>
        /// Holds an index of relations to exclude.
        /// </summary>
        private HashSet<long> _relationsToExclude;

        /// <summary>
        /// Creates a new exclude OsmStreamSource.
        /// </summary>
        public OsmStreamFilterExclude()
            : this(true, true, true)
        {

        }

        /// <summary>
        /// Creates a new exclude OsmStreamSource.
        /// </summary>
        /// <param name="excludeNodes"></param>
        /// <param name="excludeWays"></param>
        /// <param name="excludeRelations"></param>
        public OsmStreamFilterExclude(bool excludeNodes, bool excludeWays, bool excludeRelations)
        {
            _sources = new List<OsmStreamSource>();

            _excludeNodes = excludeNodes;
            _excludeWays = excludeWays;
            _excludeRelations = excludeRelations;
        }

        /// <summary>
        /// Registers a reader as the source to filter.
        /// </summary>
        /// <param name="source"></param>
        public override void RegisterSource(IEnumerable<OsmGeo> source)
        {
            this.RegisterSource(source.ToOsmStreamSource());
        }

        /// <summary>
        /// Registers another source.
        /// </summary>
        /// <param name="source"></param>
        public override void RegisterSource(OsmStreamSource source)
        {
            _sources.Add(source);
        }

        /// <summary>
        /// Returns true if this source can be reset.
        /// </summary>
        public override bool CanReset
        {
            get
            {
                foreach (var source in _sources)
                {
                    if(!source.CanReset)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        /// <returns></returns>
        public override OsmGeo Current()
        {
            return _sources[0].Current();
        }

        /// <summary>
        /// Initializes this source.
        /// </summary>
        public override void Initialize()
        {
            foreach (var source in _sources)
            {
                source.Initialize();
            }
        }

        /// <summary>
        /// Returns true if this source is sorted.
        /// </summary>
        public override bool IsSorted
        {
            get
            {
                if(_sources.Count == 0)
                {
                    return false;
                }
                return _sources[0].IsSorted;
            }
        }      
        
        /// <summary>
        /// Move to the next item in the stream.
        /// </summary>
        /// <param name="ignoreNodes">Makes this source skip all nodes.</param>
        /// <param name="ignoreWays">Makes this source skip all ways.</param>
        /// <param name="ignoreRelations">Makes this source skip all relations.</param>
        /// <returns></returns>
        public override bool MoveNext(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
        {
            // build excluding index.
            _nodesToExclude = new HashSet<long>();
            _waysToExclude = new HashSet<long>();
            _relationsToExclude = new HashSet<long>();
            for (int idx = 1; idx < _sources.Count; idx++)
            {
                while (_sources[idx].MoveNext())
                {
                    var current = _sources[idx].Current();
                    switch (current.Type)
                    {
                        case OsmGeoType.Node:
                            _nodesToExclude.Add(current.Id.Value);
                            break;
                        case OsmGeoType.Way:
                            _waysToExclude.Add(current.Id.Value);
                            break;
                        case OsmGeoType.Relation:
                            _relationsToExclude.Add(current.Id.Value);
                            break;
                    }
                }
            }

            // move to the next object.
            while (_sources[0].MoveNext(ignoreNodes, ignoreWays, ignoreRelations))
            { // break when not exluded.
                var current = _sources[0].Current();
                var moveNext = false;
                switch (current.Type)
                {
                    case OsmGeoType.Node:
                        moveNext = _excludeNodes &&
                            _nodesToExclude.Contains(current.Id.Value);
                        break;
                    case OsmGeoType.Way:
                        moveNext = _excludeWays &&
                            _waysToExclude.Contains(current.Id.Value);
                        break;
                    case OsmGeoType.Relation:
                        moveNext = _excludeRelations &&
                            _relationsToExclude.Contains(current.Id.Value);
                        break;
                }
                if(!moveNext)
                { // ok, this object should not be excluded.
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Resets this source.
        /// </summary>
        public override void Reset()
        {
            _nodesToExclude = null;
            _waysToExclude = null;
            _relationsToExclude = null;
            foreach (var source in _sources)
            {
                source.Reset();
            }
        }
    }
}