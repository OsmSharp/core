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

using OsmSharp.Collections.Tags;
using System;

namespace OsmSharp.Osm
{
    /// <summary>
    /// Primive used as a base class for any osm object that has a meaning on the map (Nodes, Ways and Relations).
    /// </summary>
    public abstract class OsmGeo
    {
        /// <summary>
        /// The id.
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// The type.
        /// </summary>
        public OsmGeoType Type { get; protected set; }

        /// <summary>
        /// The tags.
        /// </summary>
        public TagsCollectionBase Tags { get; set; }

        /// <summary>
        /// The changeset id.
        /// </summary>
        public long? ChangeSetId { get; set; }

        /// <summary>
        /// The visible flag.
        /// </summary>
        public bool? Visible { get; set; }

        /// <summary>
        /// The timestamp.
        /// </summary>
        public DateTime? TimeStamp { get; set; }

        /// <summary>
        /// The version.
        /// </summary>
        public ulong? Version { get; set; }

        /// <summary>
        /// The userid.
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// The username.
        /// </summary>
        public string UserName { get; set; }
    }
}
