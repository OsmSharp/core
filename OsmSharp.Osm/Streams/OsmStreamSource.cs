// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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
using System.Collections;
using System.Collections.Generic;

namespace OsmSharp.Osm.Streams
{
    /// <summary>
    /// Base class for any (streamable) source of osm data (Nodes, Ways and Relations).
    /// </summary>
    public abstract class OsmStreamSource : IEnumerable<OsmGeo>, IEnumerator<OsmGeo>
    {
        private readonly TagsCollectionBase _meta;

        /// <summary>
        /// Creates a new source.
        /// </summary>
        protected OsmStreamSource()
        {
            _meta = new TagsCollection();
        }

        /// <summary>
        /// Initializes this source.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Move to the next item in the stream.
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            return this.MoveNext(false, false, false);
        }

        /// <summary>
        /// Move to the next node.
        /// </summary>
        /// <returns></returns>
        public bool MoveNextNode()
        {
            return this.MoveNext(false, true, true);
        }

        /// <summary>
        /// Move to the next way.
        /// </summary>
        /// <returns></returns>
        public bool MoveNextWay()
        {
            return this.MoveNext(true, false, true);
        }

        /// <summary>
        /// Move to the next relation.
        /// </summary>
        /// <returns></returns>
        public bool MoveNextRelation()
        {
            return this.MoveNext(true, true, false);
        }

        /// <summary>
        /// Move to the next item in the stream.
        /// </summary>
        /// <param name="ignoreNodes">Makes this source skip all nodes.</param>
        /// <param name="ignoreWays">Makes this source skip all ways.</param>
        /// <param name="ignoreRelations">Makes this source skip all relations.</param>
        /// <returns></returns>
        public abstract bool MoveNext(bool ignoreNodes, bool ignoreWays, bool ignoreRelations);

        /// <summary>
        /// Returns the current item in the stream.
        /// </summary>
        /// <returns></returns>
        public abstract OsmGeo Current();

        /// <summary>
        /// Resets the source to the beginning.
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Returns true if this source can be reset.
        /// </summary>
        /// <returns></returns>
        public abstract bool CanReset
        {
            get;
        }

        /// <summary>
        /// Returns true if this source never returns a way or relation before an node or a relation before a way.
        /// </summary>
        /// <remarks>Make sure to only return true if this is absolutely certain, some stream depend on this.</remarks>
        public virtual bool IsSorted
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the meta-data.
        /// </summary>
        public TagsCollectionBase Meta
        {
            get
            {
                return _meta;
            }
        }

        /// <summary>
        /// Gets all meta-data from all sources and filters that provide this source of data.
        /// </summary>
        /// <returns></returns>
        public virtual TagsCollection GetAllMeta()
        {
            return new TagsCollection(_meta);
        }

        #region IEnumerator/IEnumerable Implementation

        /// <summary>
        /// Returns the enumerator for this enumerable.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<OsmGeo> GetEnumerator()
        {
            this.Initialize();

            return this;
        }

        /// <summary>
        /// Returns the enumerator for this enumerable.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            this.Initialize();

            return this;
        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        OsmGeo IEnumerator<OsmGeo>.Current
        {
            get { return this.Current(); }
        }

        /// <summary>
        /// Disposes all resources associated with this source.
        /// </summary>
        public virtual void Dispose()
        {

        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        object System.Collections.IEnumerator.Current
        {
            get { return this.Current(); }
        }

        #endregion
    }
}