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

using OsmSharp.Collections.Tags;
using OsmSharp.Osm;
using System.Collections.Generic;

namespace OsmSharp.Osm.Streams
{
    /// <summary>
    /// An OSM stream filter.
    /// </summary>
    public abstract class OsmStreamFilter : OsmStreamSource
    {
        /// <summary>
        /// Holds the reader.
        /// </summary>
        private OsmStreamSource _source;

        /// <summary>
        /// Creates a new OSM filter.
        /// </summary>
        public OsmStreamFilter()
        {

        }

        /// <summary>
        /// Registers a reader as the source to filter.
        /// </summary>
        /// <param name="source"></param>
        public virtual void RegisterSource(OsmStreamSource source)
        {
            _source = source;
        }

        /// <summary>
        /// Registers a reader as the source to filter.
        /// </summary>
        /// <param name="source"></param>
        public virtual void RegisterSource(IEnumerable<OsmGeo> source)
        {
            _source = source.ToOsmStreamSource();
        }

        /// <summary>
        /// Returns the reader being filtered.
        /// </summary>
        protected OsmStreamSource Source
        {
            get
            {
                return _source;
            }
        }

        /// <summary>
        /// Gets all meta-data from all sources and filters that provide this filter of data.
        /// </summary>
        /// <returns></returns>
        public override TagsCollection GetAllMeta()
        {
            var tags = this.Source.GetAllMeta();
            tags.AddOrReplace(new TagsCollection(this.Meta));
            return tags;
        }

        /// <summary>
        /// Initializes this filter.
        /// </summary>
        public abstract override void Initialize();

        /// <summary>
        /// Returns the current object.
        /// </summary>
        /// <returns></returns>
        public abstract override OsmGeo Current();
    }
}