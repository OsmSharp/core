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

namespace OsmSharp.API
{
    /// <summary>
    /// Represents the API capabilities.
    /// </summary>
    public partial class Capabilities
    {
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        public Version Version { get; set; }

        /// <summary>
        /// Gets or sets the area.
        /// </summary>
        public Area Area { get; set; }

        /// <summary>
        /// Gets or sets the tracepoints.
        /// </summary>
        public Tracepoints Tracepoints { get; set; }

        /// <summary>
        /// Gets or sets the way nodes.
        /// </summary>
        public WayNodes WayNodes { get; set; }

        /// <summary>
        /// Gets or sets the changesets.
        /// </summary>
        public OsmSharp.API.Changesets Changesets { get; set; }

        /// <summary>
        /// Gets or sets the timeout.
        /// </summary>
        public Timeout Timeout { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public Status Status { get; set; }
    }

    /// <summary>
    /// Represents the API-version capabilities.
    /// </summary>
    public partial class Version
    {
        /// <summary>
        /// Gets or sets the minimum version.
        /// </summary>
        public double? Minimum { get; set; }

        /// <summary>
        /// Gets or sets the maximum version.
        /// </summary>
        public double? Maximum { get; set; }
    }

    /// <summary>
    /// Represents the API-area capability.
    /// </summary>
    public partial class Area
    {
        /// <summary>
        /// Gets or sets the maxium.
        /// </summary>
        public double? Maximum { get; set; }
    }

    /// <summary>
    /// Represents the API-tracepoints capability.
    /// </summary>
    public partial class Tracepoints
    {
        /// <summary>
        /// Gets or sets the per page setting.
        /// </summary>
        public int? PerPage { get; set; }
    }

    /// <summary>
    /// Represents the API-waynodes capability.
    /// </summary>
    public partial class WayNodes
    {
        /// <summary>
        /// Gets or sets the maximum.
        /// </summary>
        public int? Maximum { get; set; }
    }

    /// <summary>
    /// Represents the API-changesets capabilitiy.
    /// </summary>
    public partial class Changesets
    {
        /// <summary>
        /// Gets or sets the maximum element count.
        /// </summary>
        public int? MaximumElements { get; set; }
    }

    /// <summary>
    /// Represents the API-timeout capability.
    /// </summary>
    public partial class Timeout
    {
        /// <summary>
        /// Gets or sets the # of seconds.
        /// </summary>
        public int? Seconds { get; set; }
    }

    /// <summary>
    /// Represents the API-status.
    /// </summary>
    public partial class Status
    {
        /// <summary>
        /// Gets or sets the database status.
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// Gets or sets the api status.
        /// </summary>
        public string Api { get; set; }

        /// <summary>
        /// Gets or sets the gpx status.
        /// </summary>
        public string Gpx { get; set; }
    }
}