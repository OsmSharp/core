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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Osm
{
    /// <summary>
    /// Represents info on a changeset.
    /// </summary>
    public class ChangeSetInfo
    {
        /// <summary>
        /// The id.
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// The date the changeset was closed.
        /// </summary>
        public DateTime? ClosedAt { get; set; }

        /// <summary>
        /// The date the changeset was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The tags for this changeset.
        /// </summary>
        public Dictionary<string, string> Tags { get; set; }

        /// <summary>
        /// Flag indicating open status of this changeset.
        /// </summary>
        public bool Open { get; set; }

        /// <summary>
        /// The minimum longitude.
        /// </summary>
        public double MinLon { get; set; }

        /// <summary>
        /// The minimum latitude.
        /// </summary>
        public double MinLat { get; set; }

        /// <summary>
        /// The maximum longitude.
        /// </summary>
        public double MaxLon { get; set; }

        /// <summary>
        /// The maximum latititude.
        /// </summary>
        public double MaxLat { get; set; }

        /// <summary>
        /// The user id.
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// The username.
        /// </summary>
        public string User { get; set; }
    }
}
