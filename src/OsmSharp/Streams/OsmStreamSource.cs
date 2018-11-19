// The MIT License (MIT)

// Copyright (c) 2017 Ben Abelshausen

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
using OsmSharp.Tags;
using System.Collections;
using System.Collections.Generic;
using OsmSharp.Streams.Seek;

namespace OsmSharp.Streams
{
    /// <summary>
    /// An abstract representation of an OSM source stream.
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
        public abstract bool MoveNext(bool ignoreNodes, bool ignoreWays, bool ignoreRelations);

        /// <summary>
        /// Returns the current item in the stream.
        /// </summary>
        public abstract OsmGeo Current();
        
        /// <summary>
        /// Gets the current position.
        /// </summary>
        public abstract SeekPosition Position { get; }

        /// <summary>
        /// Positions the stream to the given seek position.
        /// </summary>
        /// <param name="position">The seek position.</param>
        /// <remarks>Throws an exception if the given seek position is not supported.</remarks>
        public abstract void Seek(SeekPosition position);

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
        public virtual bool IsSorted => false;

        /// <summary>
        /// Gets the meta-data.
        /// </summary>
        public TagsCollectionBase Meta => _meta;

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
            if (this.CanReset)
            {
                this.Reset();
            }
            return this;
        }

        /// <summary>
        /// Returns the enumerator for this enumerable.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
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

        /// <summary>
        /// Returns an enumerable that ignores some types of objects.
        /// </summary>
        [Obsolete]
        public IEnumerable<OsmGeo> EnumerateAndIgore(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
        {
            return new OsmStreamSourceEnumerable(this, ignoreNodes, ignoreWays, ignoreRelations);
        }

        /// <summary>
        /// Returns an enumerable that ignores some types of objects.
        /// </summary>
        public IEnumerable<OsmGeo> EnumerateAndIgnore(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
        {
            return new OsmStreamSourceEnumerable(this, ignoreNodes, ignoreWays, ignoreRelations);
        }
    }
}