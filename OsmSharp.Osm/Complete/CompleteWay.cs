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
using OsmSharp.Collections;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Osm;
using OsmSharp.Osm.Cache;
using OsmSharp.Osm.Data;

namespace OsmSharp.Osm
{
    /// <summary>
    /// Way class.
    /// </summary>
    public class CompleteWay : CompleteOsmGeo
    {
        /// <summary>
        /// Holds the nodes of this way.
        /// </summary>
        private readonly List<Node> _nodes;

        /// <summary>
        /// Creates a new way.
        /// </summary>
        /// <param name="id"></param>
        internal protected CompleteWay(long id)
            : base(id)
        {
            _nodes = new List<Node>();
        }

        /// <summary>
        /// Creates a new way using a string table.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="stringTable"></param>
        internal protected CompleteWay(ObjectTable<string> stringTable, long id)
            : base(stringTable, id)
        {
            _nodes = new List<Node>();
        }

        /// <summary>
        /// Returns the way type.
        /// </summary>
        public override CompleteOsmType Type
        {
            get { return CompleteOsmType.Way; }
        }

        /// <summary>
        /// Gets the ordered list of nodes.
        /// </summary>
        public List<Node> Nodes
        {
            get
            {
                return _nodes;
            }
        }

        /// <summary>
        /// Returns all the coordinates in this way in the same order as the nodes.
        /// </summary>
        /// <returns></returns>
        public List<GeoCoordinate> GetCoordinates()
        {
            var coordinates = new List<GeoCoordinate>();

            for (int idx = 0; idx < this.Nodes.Count; idx++)
            {
                coordinates.Add(this.Nodes[idx].Coordinate);
            }

            return coordinates;
        }

        /// <summary>
        /// Copies all info in this way to the given way without changing the id.
        /// </summary>
        /// <param name="w"></param>
        public void CopyTo(CompleteWay w)
        {
            foreach (Tag tag in this.Tags)
            {
                w.Tags.Add(tag.Key, tag.Value);
            }
            w.Nodes.AddRange(this.Nodes);
            w.TimeStamp = this.TimeStamp;
            w.User = this.User;
            w.UserId = this.UserId;
            w.Version = this.Version;
            w.Visible = this.Visible;
        }

        /// <summary>
        /// Returns an exact copy of this way.
        /// 
        /// WARNING: even the id is copied!
        /// </summary>
        /// <returns></returns>
        public CompleteWay Copy()
        {
            var w = new CompleteWay(this.Id);
            this.CopyTo(w);
            return w;
        }

        /// <summary>
        /// Returns true if this way contains the given node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool HasNode(Node node)
        {
            return this.Nodes.Contains(node);
        }
        
        /// <summary>
        /// Returns true if this way is closed (firstnode == lastnode).
        /// </summary>
        /// <returns></returns>
        public bool IsClosed()
        {
            return this.Nodes != null &&
                this.Nodes.Count > 1 &&
                this.Nodes[0].Id == this.Nodes[this.Nodes.Count - 1].Id;
        }

        /// <summary>
        /// Converts this relation into it's simple counterpart.
        /// </summary>
        /// <returns></returns>
        public override OsmGeo ToSimple()
        {
            var way = new Way();
            way.Id = this.Id;
            way.ChangeSetId = this.ChangeSetId;
            way.Tags = this.Tags;
            way.TimeStamp = this.TimeStamp;
            way.UserId = this.UserId;
            way.UserName = this.User;
            way.Version = (ulong?)this.Version;
            way.Visible = this.Visible;

            way.Nodes = new List<long>();
            foreach (Node node in this.Nodes)
            {
                way.Nodes.Add(node.Id.Value);
            }
            return way;
        }

        /// <summary>
        /// Returns a description of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("http://www.openstreetmap.org/?way={0}",
                this.Id);
        }

        #region Way factory functions

        /// <summary>
        /// Creates a new way with a given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static CompleteWay Create(long id)
        {
            return new CompleteWay(id);
        }

        /// <summary>
        /// Creates a new way from a SimpleWay given a dictionary with nodes.
        /// </summary>
        /// <param name="simpleWay"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static CompleteWay CreateFrom(Way simpleWay, IDictionary<long, Node> nodes)
        {
            if (simpleWay == null) throw new ArgumentNullException("simpleWay");
            if (nodes == null) throw new ArgumentNullException("nodes");
            if (simpleWay.Id == null) throw new Exception("simpleWay.id is null");

            CompleteWay way = Create(simpleWay.Id.Value);

            way.ChangeSetId = simpleWay.ChangeSetId;
            foreach (Tag pair in simpleWay.Tags)
            {
                way.Tags.Add(pair);
            }
            for (int idx = 0; idx < simpleWay.Nodes.Count; idx++)
            {
                long nodeId = simpleWay.Nodes[idx];
                Node node = null;
                if (nodes.TryGetValue(nodeId, out node))
                {
                    way.Nodes.Add(node);
                }
                else
                {
                    return null;
                }
            }
            way.TimeStamp = simpleWay.TimeStamp;
            way.User = simpleWay.UserName;
            way.UserId = simpleWay.UserId;
            way.Version = simpleWay.Version.HasValue ? (long)simpleWay.Version.Value : (long?)null;
            way.Visible = simpleWay.Visible.HasValue && simpleWay.Visible.Value;

            return way;
        }

        /// <summary>
        /// Creates a new way from a SimpleWay.
        /// </summary>
        /// <param name="simpleWay"></param>
        /// <param name="nodeSource"></param>
        /// <returns></returns>
        public static CompleteWay CreateFrom(Way simpleWay, INodeSource nodeSource)
        {
            if (simpleWay == null) throw new ArgumentNullException("simpleWay");
            if (nodeSource == null) throw new ArgumentNullException("nodeSource");
            if (simpleWay.Id == null) throw new Exception("simpleWay.id is null");

            CompleteWay way = Create(simpleWay.Id.Value);

            way.ChangeSetId = simpleWay.ChangeSetId;
            if (simpleWay.Tags != null)
            {
                foreach (Tag pair in simpleWay.Tags)
                {
                    way.Tags.Add(pair);
                }
            }
            if (simpleWay.Nodes != null)
            {
                for (int idx = 0; idx < simpleWay.Nodes.Count; idx++)
                {
                    long nodeId = simpleWay.Nodes[idx];
                    Node node = nodeSource.GetNode(nodeId);
                    if (node == null)
                    {
                        return null;
                    }
                    Node completeNode = node;
                    if (completeNode != null)
                    {
                        way.Nodes.Add(completeNode);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            way.TimeStamp = simpleWay.TimeStamp;
            way.User = simpleWay.UserName;
            way.UserId = simpleWay.UserId;
            way.Version = simpleWay.Version.HasValue ? (long)simpleWay.Version.Value : (long?)null;
            way.Visible = simpleWay.Visible.HasValue && simpleWay.Visible.Value;

            return way;
        }

        /// <summary>
        /// Creates a new way with a given id given a stringtable.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static CompleteWay Create(ObjectTable<string> table, long id)
        {
            return new CompleteWay(table, id);
        }

        /// <summary>
        /// Creates a new way from a SimpleWay given a stringtable.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="simpleWay"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static CompleteWay CreateFrom(ObjectTable<string> table, Way simpleWay,
                                        IDictionary<long, Node> nodes)
        {
            if (table == null) throw new ArgumentNullException("table");
            if (simpleWay == null) throw new ArgumentNullException("simpleWay");
            if (nodes == null) throw new ArgumentNullException("nodes");
            if (simpleWay.Id == null) throw new Exception("simpleWay.id is null");

            CompleteWay way = Create(table, simpleWay.Id.Value);

            way.ChangeSetId = simpleWay.ChangeSetId;
            foreach (Tag pair in simpleWay.Tags)
            {
                way.Tags.Add(pair);
            }
            for (int idx = 0; idx < simpleWay.Nodes.Count; idx++)
            {
                long nodeId = simpleWay.Nodes[idx];
                Node node = null;
                if (nodes.TryGetValue(nodeId, out node))
                {
                    way.Nodes.Add(node);
                }
                else
                {
                    return null;
                }
            }
            way.TimeStamp = simpleWay.TimeStamp;
            way.User = simpleWay.UserName;
            way.UserId = simpleWay.UserId;
            way.Version = simpleWay.Version.HasValue ? (long)simpleWay.Version.Value : (long?)null;
            way.Visible = simpleWay.Visible.HasValue && simpleWay.Visible.Value;

            return way;
        }

        #endregion
    }
}
