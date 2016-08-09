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

namespace OsmSharp.Changesets
{
    /// <summary>
    /// Represents a changeset.
    /// </summary>
    public partial class Changeset
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        public TagsCollectionBase Tags { get; set; }

        /// <summary>
        /// Gets or sets the userid.
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the created at timestamp.
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the closed at timestamp.
        /// </summary>
        public DateTime? ClosedAt { get; set; }

        /// <summary>
        /// Gets or sets the open flag.
        /// </summary>
        public bool? Open { get; set; }

        /// <summary>
        /// Gets or sets the minimum latitude.
        /// </summary>
        public float? MinLatitude { get; set; }

        /// <summary>
        /// Gets or sets the minimum longitude.
        /// </summary>
        public float? MinLongitude { get; set; }

        /// <summary>
        /// Gets or sets the maximum latitude.
        /// </summary>
        public float? MaxLatitude { get; set; }

        /// <summary>
        /// Gets or sets the maximum longitude.
        /// </summary>
        public float? MaxLongitude { get; set; }
    }
}