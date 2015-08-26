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
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Data;
using System;
using System.Collections.Generic;

namespace OsmSharp.Osm
{
    /// <summary>
    /// Relation class.
    /// </summary>
    public class CompleteRelation : CompleteOsmGeo
    {
        private readonly IList<CompleteRelationMember> _members;

        /// <summary>
        /// Creates a new relation.
        /// </summary>
        /// <param name="id"></param>
        internal protected CompleteRelation(long id)
            : base(id)
        {
            _members = new List<CompleteRelationMember>();
        }

        /// <summary>
        /// Creates a new relation using a string table.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="stringTable"></param>
        internal protected CompleteRelation(ObjectTable<string> stringTable, long id)
            : base(stringTable, id)
        {
            _members = new List<CompleteRelationMember>();
        }

        /// <summary>
        /// Returns the relation type.
        /// </summary>
        public override CompleteOsmType Type
        {
            get { return CompleteOsmType.Relation; }
        }

        /// <summary>
        /// Gets the relation members.
        /// </summary>
        public IList<CompleteRelationMember> Members
        {
            get
            {
                return _members;
            }
        }

        /// <summary>
        /// Find a member in this relation with the given role.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public ICompleteOsmGeo FindMember(string role)
        {
            if (this.Members != null)
            {
                foreach (CompleteRelationMember member in this.Members)
                {
                    if (member.Role == role)
                    {
                        return member.Member;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Converts this relation into it's simple counterpart.
        /// </summary>
        /// <returns></returns>
        public override OsmGeo ToSimple()
        {
            var relation = new Relation();
            relation.Id = this.Id;
            relation.ChangeSetId = this.ChangeSetId;
            relation.Tags = this.Tags;
            relation.TimeStamp = this.TimeStamp;
            relation.UserId = this.UserId;
            relation.UserName = this.User;
            relation.Version = (ulong?)this.Version;
            relation.Visible = this.Visible;
            relation.Members = new List<RelationMember>();
            foreach (var member in this.Members)
            {
                var simpleMember = new RelationMember();
                simpleMember.MemberId = member.Member.Id;
                simpleMember.MemberRole = member.Role;
                switch (member.Member.Type)
                {
                    case CompleteOsmType.Node:
                        simpleMember.MemberType = OsmGeoType.Node;
                        break;
                    case CompleteOsmType.Relation:
                        simpleMember.MemberType = OsmGeoType.Relation;
                        break;
                    case CompleteOsmType.Way:
                        simpleMember.MemberType = OsmGeoType.Way;
                        break;
                }
                relation.Members.Add(simpleMember);
            }
            return relation;
        }

        /// <summary>
        /// Returns all the coordinates in this way in the same order as the nodes.
        /// </summary>
        /// <returns></returns>
        public IList<GeoCoordinate> GetCoordinates()
        {
            var coordinates = new List<GeoCoordinate>();
            for (int idx = 0; idx < this.Members.Count; idx++)
            {
                if (this.Members[idx].Member is Node)
                {
                    var node = this.Members[idx].Member as Node;
                    coordinates.Add(node.Coordinate);
                }
                else if (this.Members[idx].Member is CompleteWay)
                {
                    var way = this.Members[idx].Member as CompleteWay;
                    coordinates.AddRange(way.GetCoordinates());
                }
                else if (this.Members[idx].Member is CompleteRelation)
                {
                    var relation = this.Members[idx].Member as CompleteRelation;
                    coordinates.AddRange(relation.GetCoordinates());
                }
            }
            return coordinates;
        }

        #region Relation factory functions

        /// <summary>
        /// Creates a relation with a given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static CompleteRelation Create(long id)
        {
            return new CompleteRelation(id);
        }

        /// <summary>
        /// Creates a relation from a SimpleRelation.
        /// </summary>
        /// <returns></returns>
        public static CompleteRelation CreateFrom(Relation simpleRelation,
            IDictionary<long, Node> nodes,
            IDictionary<long, CompleteWay> ways,
            IDictionary<long, CompleteRelation> relations)
        {
            if (simpleRelation == null) throw new ArgumentNullException("simpleRelation");
            if (nodes == null) throw new ArgumentNullException("nodes");
            if (ways == null) throw new ArgumentNullException("ways");
            if (relations == null) throw new ArgumentNullException("relations");
            if (simpleRelation.Id == null) throw new Exception("simpleRelation.Id is null");

            var relation = Create(simpleRelation.Id.Value);

            relation.ChangeSetId = simpleRelation.ChangeSetId;
            foreach (var pair in simpleRelation.Tags)
            {
                relation.Tags.Add(pair);
            }
            for (int idx = 0; idx < simpleRelation.Members.Count; idx++)
            {
                long memberId = simpleRelation.Members[idx].MemberId.Value;
                string role = simpleRelation.Members[idx].MemberRole;

                var member = new CompleteRelationMember();
                member.Role = role;
                switch (simpleRelation.Members[idx].MemberType.Value)
                {
                    case OsmGeoType.Node:
                        Node node = null;
                        if (nodes.TryGetValue(memberId, out node))
                        {
                            member.Member = node;
                        }
                        else
                        {
                            return null;
                        }
                        break;
                    case OsmGeoType.Way:
                        CompleteWay way = null;
                        if (ways.TryGetValue(memberId, out way))
                        {
                            member.Member = way;
                        }
                        else
                        {
                            return null;
                        }
                        break;
                    case OsmGeoType.Relation:
                        CompleteRelation relationMember = null;
                        if (relations.TryGetValue(memberId, out relationMember))
                        {
                            member.Member = relationMember;
                        }
                        else
                        {
                            return null;
                        }
                        break;
                }
                relation.Members.Add(member);
            }
            relation.TimeStamp = simpleRelation.TimeStamp;
            relation.User = simpleRelation.UserName;
            relation.UserId = simpleRelation.UserId;
            relation.Version = simpleRelation.Version.HasValue ? (long)simpleRelation.Version.Value : (long?)null;
            relation.Visible = simpleRelation.Visible.HasValue && simpleRelation.Visible.Value;
            return relation;
        }

        /// <summary>
        /// Creates a relation from a SimpleRelation.
        /// </summary>
        /// <returns></returns>
        public static CompleteRelation CreateFrom(Relation simpleRelation, IOsmGeoSource osmGeoSource)
        {
            if (simpleRelation == null) throw new ArgumentNullException("simpleRelation");
            if (osmGeoSource == null) throw new ArgumentNullException("osmGeoSource");
            if (simpleRelation.Id == null) throw new Exception("simpleRelation.Id is null");

            var relation = Create(simpleRelation.Id.Value);

            relation.ChangeSetId = simpleRelation.ChangeSetId;
            if (simpleRelation.Tags != null)
            {
                foreach (var pair in simpleRelation.Tags)
                {
                    relation.Tags.Add(pair);
                }
            }
            if (simpleRelation.Members != null)
            {
                for (var idx = 0; idx < simpleRelation.Members.Count; idx++)
                {
                    var memberId = simpleRelation.Members[idx].MemberId.Value;
                    var role = simpleRelation.Members[idx].MemberRole;
                    var member = new CompleteRelationMember();
                    member.Role = role;
                    switch (simpleRelation.Members[idx].MemberType.Value)
                    {
                        case OsmGeoType.Node:
                            var simpleNode = osmGeoSource.GetNode(memberId);
                            if (simpleNode == null)
                            {
                                return null;
                            }
                            var completeNode = simpleNode;
                            if (completeNode != null)
                            {
                                member.Member = completeNode;
                            }
                            else
                            {
                                return null;
                            }
                            break;
                        case OsmGeoType.Way:
                            var simpleWay = osmGeoSource.GetWay(memberId);
                            if (simpleWay == null)
                            {
                                return null;
                            }
                            var completeWay = CompleteWay.CreateFrom(simpleWay, osmGeoSource);
                            if (completeWay != null)
                            {
                                member.Member = completeWay;
                            }
                            else
                            {
                                return null;
                            }
                            break;
                        case OsmGeoType.Relation:
                            var simpleRelationMember = osmGeoSource.GetRelation(memberId);
                            if (simpleRelationMember == null)
                            {
                                return null;
                            }
                            var completeRelation = CompleteRelation.CreateFrom(simpleRelationMember, osmGeoSource);
                            if (completeRelation != null)
                            {
                                member.Member = completeRelation;
                            }
                            else
                            {
                                return null;
                            }
                            break;
                    }
                    relation.Members.Add(member);
                }
            }
            relation.TimeStamp = simpleRelation.TimeStamp;
            relation.User = simpleRelation.UserName;
            relation.UserId = simpleRelation.UserId;
            relation.Version = simpleRelation.Version.HasValue ? (long)simpleRelation.Version.Value : (long?)null;
            relation.Visible = simpleRelation.Visible.HasValue && simpleRelation.Visible.Value;
            return relation;
        }

        /// <summary>
        /// Creates a relation from a SimpleRelation.
        /// </summary>
        /// <returns></returns>
        public static CompleteRelation CreateFrom(Relation simpleRelation, IOsmGeoSource osmGeoSource,
            IDictionary<long, CompleteWay> ways,
            IDictionary<long, CompleteRelation> relations)
        {
            if (simpleRelation == null) throw new ArgumentNullException("simpleRelation");
            if (osmGeoSource == null) throw new ArgumentNullException("osmGeoSource");
            if (simpleRelation.Id == null) throw new Exception("simpleRelation.Id is null");

            var relation = Create(simpleRelation.Id.Value);
            relation.ChangeSetId = simpleRelation.ChangeSetId;
            foreach (var pair in simpleRelation.Tags)
            {
                relation.Tags.Add(pair);
            }
            for (int idx = 0; idx < simpleRelation.Members.Count; idx++)
            {
                long memberId = simpleRelation.Members[idx].MemberId.Value;
                string role = simpleRelation.Members[idx].MemberRole;
                var member = new CompleteRelationMember();
                member.Role = role;
                switch (simpleRelation.Members[idx].MemberType.Value)
                {
                    case OsmGeoType.Node:
                        var simpleNode = osmGeoSource.GetNode(memberId);
                        if (simpleNode != null)
                        {
                            member.Member = simpleNode;
                        }
                        else
                        {
                            return null;
                        }
                        break;
                    case OsmGeoType.Way:
                        CompleteWay completeWay;
                        if (!ways.TryGetValue(memberId, out completeWay))
                        {
                            var simpleWay = osmGeoSource.GetWay(memberId);
                            if (simpleWay != null)
                            {
                                completeWay = CompleteWay.CreateFrom(simpleWay, osmGeoSource);
                            }
                        }
                        if (completeWay != null)
                        {
                            member.Member = completeWay;
                        }
                        else
                        {
                            return null;
                        }
                        break;
                    case OsmGeoType.Relation:
                        CompleteRelation completeRelation;
                        if (!relations.TryGetValue(memberId, out completeRelation))
                        {
                            Relation simpleRelationMember = osmGeoSource.GetRelation(memberId);
                            if (simpleRelationMember != null)
                            {
                                completeRelation = CompleteRelation.CreateFrom(simpleRelationMember, osmGeoSource);
                            }
                        }
                        if (completeRelation != null)
                        {
                            member.Member = completeRelation;
                        }
                        else
                        {
                            return null;
                        }
                        break;
                }
                relation.Members.Add(member);
            }
            relation.TimeStamp = simpleRelation.TimeStamp;
            relation.User = simpleRelation.UserName;
            relation.UserId = simpleRelation.UserId;
            relation.Version = simpleRelation.Version.HasValue ? (long)simpleRelation.Version.Value : (long?)null;
            relation.Visible = simpleRelation.Visible.HasValue && simpleRelation.Visible.Value;
            return relation;
        }

        /// <summary>
        /// Creates a new relation.
        /// </summary>
        /// <returns></returns>
        public static CompleteRelation Create(ObjectTable<string> table, long id)
        {
            return new CompleteRelation(table, id);
        }

        /// <summary>
        /// Creates a new relation from a SimpleRelation.
        /// </summary>
        /// <returns></returns>
        public static CompleteRelation CreateFrom(ObjectTable<string> table, Relation simpleRelation,
            IDictionary<long, Node> nodes,
            IDictionary<long, CompleteWay> ways,
            IDictionary<long, CompleteRelation> relations)
        {
            if (simpleRelation == null) throw new ArgumentNullException("simpleRelation");
            if (nodes == null) throw new ArgumentNullException("nodes");
            if (ways == null) throw new ArgumentNullException("ways");
            if (relations == null) throw new ArgumentNullException("relations");
            if (simpleRelation.Id == null) throw new Exception("simpleRelation.Id is null");

            var relation = Create(table, simpleRelation.Id.Value);
            relation.ChangeSetId = simpleRelation.ChangeSetId;
            foreach (var pair in simpleRelation.Tags)
            {
                relation.Tags.Add(pair);
            }
            for (var idx = 0; idx < simpleRelation.Members.Count; idx++)
            {
                var memberId = simpleRelation.Members[idx].MemberId.Value;
                var role = simpleRelation.Members[idx].MemberRole;
                var member = new CompleteRelationMember();
                member.Role = role;
                switch (simpleRelation.Members[idx].MemberType.Value)
                {
                    case OsmGeoType.Node:
                        Node node = null;
                        if (nodes.TryGetValue(memberId, out node))
                        {
                            member.Member = node;
                        }
                        else
                        {
                            return null;
                        }
                        break;
                    case OsmGeoType.Way:
                        CompleteWay way = null;
                        if (ways.TryGetValue(memberId, out way))
                        {
                            member.Member = way;
                        }
                        else
                        {
                            return null;
                        }
                        break;
                    case OsmGeoType.Relation:
                        CompleteRelation relationMember = null;
                        if (relations.TryGetValue(memberId, out relationMember))
                        {
                            member.Member = relationMember;
                        }
                        else
                        {
                            return null;
                        }
                        break;
                }
                relation.Members.Add(member);
            }
            relation.TimeStamp = simpleRelation.TimeStamp;
            relation.User = simpleRelation.UserName;
            relation.UserId = simpleRelation.UserId;
            relation.Version = simpleRelation.Version.HasValue ? (long)simpleRelation.Version.Value : (long?)null;
            relation.Visible = simpleRelation.Visible.HasValue && simpleRelation.Visible.Value;
            return relation;
        }

        /// <summary>
        /// Creates a new changeset.
        /// </summary>
        /// <returns></returns>
        public static CompleteChangeSet CreateChangeSet(long id)
        {
            return new CompleteChangeSet(id);
        }

        #endregion

        /// <summary>
        /// Returns a description of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("http://www.openstreetmap.org/?relation={0}",
                this.Id);
        }
    }
}