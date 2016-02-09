// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2016 Abelshausen Ben
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

namespace OsmSharp.IO.PBF
{
    /// <summary>
    /// Consumers Osm PBF primitives.
    /// </summary>
    public interface IPBFOsmPrimitiveConsumer
    {
        /// <summary>
        /// Processes the given node using the properties in the given block.
        /// </summary>
        void ProcessNode(PrimitiveBlock block, Node node);

        /// <summary>
        /// Processes the given way using the properties in the given block.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="way"></param>
        void ProcessWay(PrimitiveBlock block, Way way);

        /// <summary>
        /// Processing the given relation using the properties in the given block.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="relation"></param>
        void ProcessRelation(PrimitiveBlock block, Relation relation);
    }
}