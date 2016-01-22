// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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

using OsmSharp.Collections;
using OsmSharp.Geo.Features;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Geo.Interpreter;

namespace OsmSharp.Osm
{
    /// <summary>
    /// Base class for all the osm data that represents an object on the map.
    /// 
    /// Nodes, Ways and Relations
    /// </summary>
    public abstract class CompleteOsmGeo : CompleteOsmBase, ICompleteOsmGeo
    {
        /// <summary>
        /// Creates a new OsmGeo object.
        /// </summary>
        /// <param name="id"></param>
        internal CompleteOsmGeo(long id)
            :base(id)
        {
            this.Visible = true;
            this.UserId = null;
            this.User = null;
        }

        /// <summary>
        /// Gets/Sets the changeset id.
        /// </summary>
        public long? ChangeSetId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/Sets the visible flag.
        /// </summary>
        public bool Visible { get; set; }        
    }
}