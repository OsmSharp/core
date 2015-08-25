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
    /// Represents a generic data read/write datasource.
    /// </summary>
    public interface IDataSource : IDataSourceReadOnly
    {
        #region Persist

        /// <summary>
        /// Persists all the data to the underlying source.
        /// </summary>
        void Persist();

        #endregion

        #region Features

        /// <summary>
        /// Returns true if this datasource can generate final id's.
        /// </summary>
        bool IsBaseIdGenerator { get; }

        /// <summary>
        /// Returns true if this datasource can create new objects.
        /// </summary>
        bool IsCreator { get; }

        /// <summary>
        /// Returns true if this data source does not have to be saved.
        /// 
        /// The data is persisted live or not while adding/removing data.
        /// </summary>
        bool IsLive { get; }

        #endregion

        #region Nodes

        /// <summary>
        /// Adds a node.
        /// </summary>
        void AddNode(Node node);

        #endregion

        #region Relation

        /// <summary>
        /// Creates a new relation.
        /// </summary>
        /// <returns></returns>
        void AddRelation (Relation relation);

        #endregion

        #region Way

        /// <summary>
        /// Creates a new way.
        /// </summary>
        /// <returns></returns>
        void AddWay(Way way);

        #endregion

        #region Changes
        
        /// <summary>
        /// Applies the given changeset to the data in this datasource.
        /// </summary>
        /// <param name="changeSet"></param>
        void ApplyChangeSet(CompleteChangeSet changeSet);

        #endregion
    }
}
