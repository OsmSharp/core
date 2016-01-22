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

namespace OsmSharp.Osm.Data
{
    /// <summary>
    /// Abstract representation of a source of a snapshot of OSM-objects.
    /// </summary>
    public interface IOsmGeoSource
    {
        /// <summary>
        /// Returns a node with the given id from this source.
        /// </summary>
        Node GetNode(long id);

        /// <summary>
        /// Returns a way with the given id from this source.
        /// </summary>
        Way GetWay(long id);

        /// <summary>
        /// Returns a relation with the given id from this source.
        /// </summary>
        Relation GetRelation(long id);
    }
}
