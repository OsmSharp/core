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

using OsmSharp.Changesets;

namespace OsmSharp.API
{
    /// <summary>
    /// Represents the root-object for all API-related communication.
    /// </summary>
    public partial class Osm
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
        /// Gets or sets the capabilities.
        /// </summary>
        public Capabilities Api { get; set; }

        /// <summary>
        /// Gets or sets the nodes array.
        /// </summary>
        public Node[] Nodes { get; set; }

        /// <summary>
        /// Gets or sets the ways array.
        /// </summary>
        public Way[] Ways { get; set; }

        /// <summary>
        /// Gets or sets the relations array.
        /// </summary>
        public Relation[] Relations { get; set; }

        /// <summary>
        /// Gets or sets the changeset.
        /// </summary>
        public Changeset[] Changesets { get; set; }

        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        public User[] Users { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Gets or sets the bounds.
        /// </summary>
        public Bounds Bounds { get; set; }

        /// <summary>
        /// Gets or sets the GPX files.
        /// </summary>
        public GpxFile[] GpxFiles { get; set; }

        /// <summary>
        /// Gets or sets the Policy.
        /// </summary>
        public Policy Policy { get; set; }

        /// <summary>
        /// Gets or sets the Permissions.
        /// </summary>
        public Permissions Permissions { get; set; }

        /// <summary>
        /// Gets or sets the Preferences.
        /// </summary>
        public Preferences Preferences { get; set; }

        /// <summary>
        /// Gets or sets the Notes.
        /// </summary>
        public Note[] Notes { get; set; }
    }
}