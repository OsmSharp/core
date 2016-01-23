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

using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Data;
using System;
using System.Collections.Generic;

namespace OsmSharp.Osm.Complete
{
    /// <summary>
    /// Contains extensions for complete osm objects.
    /// </summary>
    public static class CompleteExtensions
    {
        
        /// <summary>
        /// Creates a complete object.
        /// </summary>
        public static ICompleteOsmGeo CreateComplete(this OsmGeo simpleOsmGeo, IOsmGeoSource osmGeoSource)
        {
            switch(simpleOsmGeo.Type)
            {
                case OsmGeoType.Node:
                    return (simpleOsmGeo as Node);
                case OsmGeoType.Way:
                    return (simpleOsmGeo as Way).CreateComplete(osmGeoSource);
                case OsmGeoType.Relation:
                    return (simpleOsmGeo as Relation).CreateComplete(osmGeoSource);
            }
            throw new Exception("Unknown OsmGeoType.");
        }

        /// <summary>
        /// Creates a complete relation.
        /// </summary>
        public static CompleteRelation CreateComplete(this Relation simpleRelation,
            Func<long, OsmGeoType, ICompleteOsmGeo> getMember)
        {
            if (simpleRelation == null) throw new ArgumentNullException("simpleRelation");
            if (getMember == null) throw new ArgumentNullException("getMember");
            if (simpleRelation.Id == null) throw new Exception("simpleRelation.Id is null");

            var relation = new CompleteRelation(simpleRelation.Id.Value);

            relation.ChangeSetId = simpleRelation.ChangeSetId;
            relation.Tags = new TagsCollection(simpleRelation.Tags);
            for (var i = 0; i < simpleRelation.Members.Count; i++)
            {
                var simpleMember = simpleRelation.Members[i];

                var member = new CompleteRelationMember();
                member.Role = simpleMember.MemberRole;
                member.Member = getMember(simpleMember.MemberId.Value,
                    simpleMember.MemberType.Value);
                if (member.Member == null)
                {
                    return null;
                }
                relation.Members.Add(member);
            }
            relation.TimeStamp = simpleRelation.TimeStamp;
            relation.User = simpleRelation.UserName;
            relation.UserId = simpleRelation.UserId;
            relation.Version = simpleRelation.Version;
            relation.Visible = simpleRelation.Visible.HasValue && simpleRelation.Visible.Value;
            return relation;
        }

        /// <summary>
        /// Creates a complete relation.
        /// </summary>
        public static CompleteRelation CreateComplete(this Relation simpleRelation, IOsmGeoSource osmGeoSource)
        {
            if (simpleRelation == null) throw new ArgumentNullException("simpleRelation");
            if (osmGeoSource == null) throw new ArgumentNullException("osmGeoSource");
            if (simpleRelation.Id == null) throw new Exception("simpleRelation.Id is null");

            return simpleRelation.CreateComplete((id, type) =>
            {
                switch (type)
                {
                    case OsmGeoType.Node:
                        return osmGeoSource.GetNode(id);
                    case OsmGeoType.Way:
                        var way = osmGeoSource.GetWay(id);
                        if(way != null)
                        {
                            return way.CreateComplete(osmGeoSource);
                        }
                        return null;
                    case OsmGeoType.Relation:
                        var relation = osmGeoSource.GetRelation(id);
                        if(relation != null)
                        {
                            return relation.CreateComplete(osmGeoSource);
                        }
                        return null;
                }
                throw new Exception("Unknown OsmGeoType.");
            });
        }

        /// <summary>
        /// Creates a complete way.
        /// </summary>
        /// <returns></returns>
        public static CompleteWay CreateComplete(this Way simpleWay, Func<long, Node> getNode)
        {
            if (simpleWay == null) throw new ArgumentNullException("simpleWay");
            if (getNode == null) throw new ArgumentNullException("getNode");
            if (simpleWay.Id == null) throw new Exception("simpleWay.id is null");

            var way = new CompleteWay(simpleWay.Id.Value);
            way.ChangeSetId = simpleWay.ChangeSetId;
            way.Tags = new TagsCollection(simpleWay.Tags);
            for (var i  = 0; i < simpleWay.Nodes.Count; i++)
            {
                var node = getNode(simpleWay.Nodes[i]);
                if(node == null)
                {
                    return null;
                }
                way.Nodes.Add(node);
            }
            way.TimeStamp = simpleWay.TimeStamp;
            way.User = simpleWay.UserName;
            way.UserId = simpleWay.UserId;
            way.Version = simpleWay.Version.HasValue ? (int)simpleWay.Version.Value : (int?)null;
            way.Visible = simpleWay.Visible.HasValue && simpleWay.Visible.Value;
            return way;
        }

        /// <summary>
        /// Creates a complete way.
        /// </summary>
        /// <returns></returns>
        public static CompleteWay CreateComplete(this Way simpleWay, IOsmGeoSource nodeSource)
        {
            if (simpleWay == null) throw new ArgumentNullException("simpleWay");
            if (nodeSource == null) throw new ArgumentNullException("nodeSource");
            if (simpleWay.Id == null) throw new Exception("simpleWay.id is null");

            return simpleWay.CreateComplete((id) => nodeSource.GetNode(id));
        }

        /// <summary>
        /// Creates a simple version of the given complete object.
        /// </summary>
        /// <param name="completeOsmGeo"></param>
        /// <returns></returns>
        public static OsmGeo CreateSimple(this ICompleteOsmGeo completeOsmGeo)
        {
            if (completeOsmGeo == null) throw new ArgumentNullException("completeOsmGeo");

            switch (completeOsmGeo.Type)
            {
                case CompleteOsmType.Node:
                    return (completeOsmGeo as Node);
                case CompleteOsmType.Way:
                    return (completeOsmGeo as CompleteWay).CreateSimple();
                case CompleteOsmType.Relation:
                    return (completeOsmGeo as CompleteRelation).CreateSimple();
            }
            throw new Exception("Cannot convert a changeset to a simple object.");
        }

        /// <summary>
        /// Creates a simple way.
        /// </summary>
        public static Way CreateSimple(this CompleteWay way)
        {
            if (way == null) throw new ArgumentNullException("way");

            var simpleWay = new Way();
            simpleWay.Id = way.Id;
            simpleWay.ChangeSetId = way.ChangeSetId;
            simpleWay.Tags = new TagsCollection(way.Tags);
            if(way.Nodes != null)
            {
                simpleWay.Nodes = new System.Collections.Generic.List<long>(
                    way.Nodes.Count);
                for (var i = 0; i < way.Nodes.Count - 1; i++)
                {
                    simpleWay.Nodes.Add(way.Nodes[i].Id.Value);
                }
            }
            simpleWay.TimeStamp = way.TimeStamp;
            simpleWay.UserId = way.UserId;
            simpleWay.UserName = way.User;
            simpleWay.Version = way.Version;
            simpleWay.Visible = way.Visible;
            return simpleWay;
        }

        /// <summary>
        /// Gets a list of coordinates for the given way, one coordinate for each node.
        /// </summary>
        public static List<GeoCoordinate> GetCoordinates(this CompleteWay way)
        {
            if (way == null) throw new ArgumentNullException("way");

            if (way.Nodes != null)
            {
                var result = new List<GeoCoordinate>(way.Nodes.Count);
                for (var i = 0; i < way.Nodes.Count - 1; i++)
                {
                    result.Add(way.Nodes[i].Coordinate);
                }
                return result;
            }
            return new List<GeoCoordinate>();
        }

        /// <summary>
        /// Returns true if this way is closed (firstnode == lastnode).
        /// </summary>
        /// <returns></returns>
        public static bool IsClosed(this CompleteWay way)
        {
            return way.Nodes != null &&
                way.Nodes.Count > 1 &&
                way.Nodes[0].Id == way.Nodes[way.Nodes.Count - 1].Id;
        }
    }
}
