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

using OsmSharp.Osm;

namespace OsmSharp.Osm.Streams
{
    /// <summary>
    /// An empty OSM stream writer doing nothing with objects being streamed to it.
    /// </summary>
    public class OsmStreamTargetEmpty : OsmStreamTarget
    {
        /// <summary>
        /// Initializes this target.
        /// </summary>
        public override void Initialize()
        {

        }

        /// <summary>
        /// Adds a node.
        /// </summary>
        /// <param name="simpleNode"></param>
        public override void AddNode(Node simpleNode)
        {

        }

        /// <summary>
        /// Adds a way.
        /// </summary>
        /// <param name="simpleWay"></param>
        public override void AddWay(Way simpleWay)
        {

        }

        /// <summary>
        /// Adds a relation.
        /// </summary>
        /// <param name="simpleRelation"></param>
        public override void AddRelation(Relation simpleRelation)
        {

        }
    }
}