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
using OsmSharp.Osm;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Filters;

namespace OsmSharp.Osm.Data
{
    /// <summary>
    /// Represents a generic readonly data source. 
    /// </summary>
    public interface IDataSourceReadOnly : IOsmGeoSource
    {
        /// <summary>
        /// Returns the bounding box of the data in this source if possible.
        /// </summary>
        GeoCoordinateBox BoundingBox { get; }

        /// <summary>
        /// The unique id for this datasource.
        /// </summary>
        Guid Id { get; }
        
        #region Features

        /// <summary>
        /// Returns true if this datasource is bounded.
        /// </summary>
        bool HasBoundinBox { get; }

        /// <summary>
        /// Returns true if this datasource is readonly.
        /// </summary>
        bool IsReadOnly { get; }

        #endregion

        #region Nodes

        /// <summary>
        /// Returns the node(s) with the given id(s).
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        IList<Node> GetNodes(IList<long> ids);

        #endregion

        #region Relation

        /// <summary>
        /// Returns the relation(s) with the given id(s).
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        IList<Relation> GetRelations(IList<long> ids);

        /// <summary>
        /// Returns all the relations for the given object.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        IList<Relation> GetRelationsFor(OsmGeoType type, long id);

        /// <summary>
        /// Returns all the relations for the given object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        IList<Relation> GetRelationsFor(OsmGeo obj);

        #endregion

        #region Way

        /// <summary>
        /// Returns the way(s) with given id.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        IList<Way> GetWays(IList<long> ids);

        /// <summary>
        /// Returns the way(s) for a given node.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IList<Way> GetWaysFor(long id);

        /// <summary>
        /// Returns the way(s) for a given node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        IList<Way> GetWaysFor(Node node);

        #endregion

        #region Queries

        /// <summary>
        /// Returns all the objects in this dataset that evaluate the filter to true.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        IList<OsmGeo> Get(GeoCoordinateBox box, Filter filter);

        #endregion

    }
}
