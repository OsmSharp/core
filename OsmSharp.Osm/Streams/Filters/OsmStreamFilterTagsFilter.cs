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
using OsmSharp.Osm;
using OsmSharp.Collections.Tags;

namespace OsmSharp.Osm.Streams.Filters
{
    /// <summary>
    /// A data processor filter that filters objects by their tags.
    /// </summary>
    public class OsmStreamFilterTagsFilter : OsmStreamFilter
    {
        /// <summary>
        /// A delegate that represents a tags filter from one collection into a new collection.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public delegate void TagsFilterDelegate(TagsCollectionBase collection);

        /// <summary>
        /// The tags filter.
        /// </summary>
        private TagsFilterDelegate _filter;

        /// <summary>
        /// Filters data according to the given filters.
        /// </summary>
        public OsmStreamFilterTagsFilter(TagsFilterDelegate tagsFilter)
        {
            _filter = tagsFilter;
        }

        /// <summary>
        /// Initializes this filter.
        /// </summary>
        public override void Initialize()
        {
            this.Source.Initialize();
        }

        /// <summary>
        /// Holds the current object.
        /// </summary>
        private OsmGeo _current;

        /// <summary>
        /// Move to the next item in the stream.
        /// </summary>
        /// <param name="ignoreNodes">Makes this source skip all nodes.</param>
        /// <param name="ignoreWays">Makes this source skip all ways.</param>
        /// <param name="ignoreRelations">Makes this source skip all relations.</param>
        /// <returns></returns>
        public override bool MoveNext(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
        {
            while (this.DoMoveNext(ignoreNodes, ignoreWays, ignoreRelations))
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
        private bool DoMoveNext(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
        {
            if (this.Source.MoveNext(ignoreNodes, ignoreWays, ignoreRelations))
            {
                _current = this.Source.Current();

                // give the opportunity to filter tags.
                if (_current.Tags != null &&
                    _current.Tags.Count > 0)
                { // only filter tags when there are tags to be filtered.
                    _filter.Invoke(_current.Tags);
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        /// <returns></returns>
        public override OsmGeo Current()
        {
            return _current;
        }

        /// <summary>
        /// Resets this filter.
        /// </summary>
        public override void Reset()
        {
            _current = null;

            this.Source.Reset();
        }

        /// <summary>
        /// Returns true if this filter can be reset.
        /// </summary>
        public override bool CanReset
        {
            get { return this.Source.CanReset; }
        }
    }
}