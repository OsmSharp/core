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

using OsmSharp.Collections.Tags;
using System;
using System.Collections.Generic;

namespace OsmSharp.Osm.Streams.Filters
{
    /// <summary>
    /// A stream source that merges multiple source streams together.
    /// </summary>
    public class OsmStreamFilterMerge : OsmStreamFilter
    {
        /// <summary>
        /// Holds all the sources to merge.
        /// </summary>
        private List<OsmStreamSource> _sources;

        /// <summary>
        /// Holds the current source.
        /// </summary>
        private int _current = -1;

        /// <summary>
        /// Creates a new merged OsmStreamSource.
        /// </summary>
        public OsmStreamFilterMerge()
        {
            _sources = new List<OsmStreamSource>();
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
            if (_current < 0 || _current > _sources.Count)
            {
                throw new InvalidOperationException("Cannot return a current object before moving to the first object.");
            }
            return _sources[_current].Current();
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
                return false;
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
            while (this.DoMoveNext())
            {
                if (this.Current().Type == OsmGeoType.Node &&
                    !ignoreNodes)
                { // there is a node and it is not to be ignored.
                    return true;
                }
                else if (this.Current().Type == OsmGeoType.Way &&
                        !ignoreWays)
                { // there is a way and it is not to be ignored.
                    return true;
                }
                else if (this.Current().Type == OsmGeoType.Relation &&
                        !ignoreRelations)
                { // there is a relation and it is not to be ignored.
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Moves to the next object.
        /// </summary>
        /// <returns></returns>
        private bool DoMoveNext()
        {
            if (_current == -1)
            { // move to the first source.
                _current = 0;
            }

            // move to the next object.
            bool moved = _sources[_current].MoveNext();
            while (!moved)
            {
                _current++;
                if (_current < _sources.Count)
                { // ok, there is next a source.
                    moved = _sources[_current].MoveNext();
                }
                else
                { // there are no more sources.
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Gets all meta-data from all sources and filters that provide this filter of data.
        /// </summary>
        /// <returns></returns>
        public override TagsCollection GetAllMeta()
        {
            var tags = new TagsCollection();
            foreach(var source in _sources)
            {
                tags.AddOrReplace(source.GetAllMeta());
            }
            tags.AddOrReplace(new TagsCollection(this.Meta));
            return tags;
        }

        /// <summary>
        /// Resets this source.
        /// </summary>
        public override void Reset()
        {
            _current = -1;
            foreach (var source in _sources)
            {
                source.Reset();
            }
        }
    }
}