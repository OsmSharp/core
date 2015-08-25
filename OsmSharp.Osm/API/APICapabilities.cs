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

namespace OsmSharp.Osm.API
{
    /// <summary>
    /// Holds API capabilities.
    /// 
    /// http://wiki.openstreetmap.org/wiki/API_v0.6#Capabilities:_GET_.2Fapi.2Fcapabilities
    /// </summary>
    public class APICapabilities
    {
        /// <summary>
        /// Version minimum and maximum are the API call versions that the server will accept. 
        /// </summary>
        public double VersionMinimum { get; internal set; }

        /// <summary>
        /// Version minimum and maximum are the API call versions that the server will accept. 
        /// </summary>
        public double VersionMaximum { get; internal set; }

        /// <summary>
        /// Area maximum is the maximum area in square degrees that can be queried by API calls.
        /// </summary>
        public double AreaMaximum { get; internal set; }

        /// <summary>
        /// Tracepoints per_page is the maximum number of points in a single GPS trace.
        /// </summary>
        public long TracePointsPerPage { get; internal set; }

        /// <summary>
        /// Waypoints maximum is the maximum number of nodes that a way may contain. 
        /// </summary>
        public long WayNodesMaximum { get; internal set; }

        /// <summary>
        /// Changesets maximum is the maximum number of combined nodes, ways and relations that can be contained in a changeset. 
        /// </summary>
        public long ChangeSetsMaximumElement { get; internal set; }

        /// <summary>
        /// Timeout in seconds.
        /// </summary>
        public long TimeoutSeconds { get; internal set; }
    }
}
