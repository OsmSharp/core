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
using System;

namespace OsmSharp
{
    /// <summary>
    /// Primive used as a base class for any osm object that has a meaning on the map (Nodes, Ways and Relations).
    /// </summary>
    public abstract class OsmGeo
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// Gets the OsmGeo-type.
        /// </summary>
        public OsmGeoType Type { get; protected set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        public TagsCollectionBase Tags { get; set; }

        /// <summary>
        /// Gets or sets the changeset id.
        /// </summary>
        public long? ChangeSetId { get; set; }

        /// <summary>
        /// Gets or sets the visible flag.
        /// </summary>
        public bool? Visible { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        public DateTime? TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        public int? Version { get; set; }

        /// <summary>
        /// Gets or sets the userid.
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string UserName { get; set; }
    }
}