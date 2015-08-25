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
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Filters;

namespace OsmSharp.Osm.Data
{
    /// <summary>
    /// Base class for IDataSourceReadOnly-implementations.
    /// </summary>
    public abstract class DataSourceReadOnlyBase : IDataSourceReadOnly
    {
        /// <summary>
        /// Returns true when this data-source is readonly.
        /// </summary>
        public virtual bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Returns the node with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual Node GetNode(long id)
        {
            IList<Node> nodes = this.GetNodes(new List<long>(new long[] { id }));
            if (nodes.Count > 0)
            {
                return nodes[0];
            }
            return null;
        }

        /// <summary>
        /// Returns all nodes with the given ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public abstract IList<Node> GetNodes(IList<long> ids);

        /// <summary>
        /// Returns the relation with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual Relation GetRelation(long id)
        {
            IList<Relation> relations = this.GetRelations(new List<long>(new long[] { id }));
            if (relations.Count > 0)
            {
                return relations[0];
            }
            return null;
        }

        /// <summary>
        /// Returns the relation with the given ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public abstract IList<Relation> GetRelations(IList<long> ids);

        /// <summary>
        /// Returns all relations containing the object with the given type and id.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract IList<Relation> GetRelationsFor(OsmGeoType type, long id);

        /// <summary>
        /// Returns all relations containg the given object as a member.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public IList<Relation> GetRelationsFor(OsmGeo obj)
        {
            return this.GetRelationsFor(obj.Type, obj.Id.Value);
        }

        /// <summary>
        /// Returns the way with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual Way GetWay(long id)
        {
            IList<Way> ways = this.GetWays(new List<long>(new long[] { id }));
            if (ways.Count > 0)
            {
                return ways[0];
            }
            return null;
        }

        /// <summary>
        /// Returns the ways with the given ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public abstract IList<Way> GetWays(IList<long> ids);

        /// <summary>
        /// Returns all the ways containing the node with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract IList<Way> GetWaysFor(long id);

        /// <summary>
        /// Returns all the ways containing the given node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public virtual IList<Way> GetWaysFor(Node node)
        {
            return this.GetWaysFor(node.Id.Value);
        }

        /// <summary>
        /// Returns all objects inside the given boundingbox and according to the given filter.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public abstract IList<OsmGeo> Get(GeoCoordinateBox box, Filter filter);

        /// <summary>
        /// Returns the boundingbox of the data in this datasource.
        /// </summary>
        public abstract GeoCoordinateBox BoundingBox
        {
            get;
        }

        /// <summary>
        /// Returns the id of this datasource.
        /// </summary>
        public abstract Guid Id
        {
            get;
        }

        /// <summary>
        /// Returns true if this datasource has a boundingbox.
        /// </summary>
        public abstract bool HasBoundinBox
        {
            get;
        }
    }
}
