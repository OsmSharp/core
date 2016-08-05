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

namespace OsmSharp.Changesets
{
    /// <summary>
    /// Represents a diff result after applying a changeset.
    /// </summary>
    public partial class DiffResult
    {
        /// <summary>
        /// Gets or sets the generator.
        /// </summary>
        public string Generator { get; set; }

        /// <summary>
        /// Gets or sets the version #.
        /// </summary>
        public double? Version { get; set; }

        /// <summary>
        /// Gets or sets the results array.
        /// </summary>
        public OsmGeoResult[] Results { get; set; }
    }

    /// <summary>
    /// An osm-geo result.
    /// </summary>
    public abstract partial class OsmGeoResult
    {
        /// <summary>
        /// Gets or sets the old id.
        /// </summary>
        public long? OldId { get; set; }

        /// <summary>
        /// Gets or sets the new id.
        /// </summary>
        public long? NewId { get; set; }

        /// <summary>
        /// Gets or sets the new version #.
        /// </summary>
        public int? NewVersion { get; set; }
    }

    /// <summary>
    /// A node result.
    /// </summary>
    public class NodeResult : OsmGeoResult
    {

    }

    /// <summary>
    /// A way result.
    /// </summary>
    public class WayResult : OsmGeoResult
    {

    }

    /// <summary>
    /// A relation result.
    /// </summary>
    public class RelationResult : OsmGeoResult
    {

    }
}