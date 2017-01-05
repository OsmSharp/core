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

using OsmSharp.Tags;

namespace OsmSharp.Streams.Filters
{
    /// <summary>
    /// An OSM stream filter.
    /// </summary>
    public abstract class OsmStreamFilter : OsmStreamSource, IOsmStreamTarget
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
        public virtual void RegisterSource(OsmStreamSource source)
        {
            _source = source;
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
        /// Returns the current object.
        /// </summary>
        /// <returns></returns>
        public abstract override OsmGeo Current();
    }
}