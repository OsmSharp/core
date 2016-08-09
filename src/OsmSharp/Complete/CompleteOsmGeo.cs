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
using OsmSharp.Tags;

namespace OsmSharp.Complete
{
    /// <summary>
    /// A complete OSM geo object.
    /// </summary>
    public abstract class CompleteOsmGeo : ICompleteOsmGeo
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the changeset id.
        /// </summary>
        public long? ChangeSetId { get; set; }

        /// <summary>
        /// Gets or sets the visible flag.
        /// </summary>
        public bool? Visible { get; set; }

        /// <summary>
        /// Gets/Sets the timestamp.
        /// </summary>
        public DateTime? TimeStamp { get; set; }

        /// <summary>
        /// Gets/Sets the version.
        /// </summary>
        public int? Version { get; set; }

        /// <summary>
        /// The user that created this object
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// The user id.
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// Gets the osm geo type.
        /// </summary>
        public abstract OsmGeoType Type { get; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        public TagsCollectionBase Tags { get; set; }
    }
}