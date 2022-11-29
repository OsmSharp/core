// The MIT License (MIT)

// Copyright (c) 2016 Ben Abelshausen

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using OsmSharp.Db;
using OsmSharp.Tags;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Complete
{
    /// <summary>
    /// Contains extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Creates a complete object.
        /// </summary>
        public static ICompleteOsmGeo CreateComplete(this OsmGeo simpleOsmGeo, IOsmGeoSource osmGeoSource)
        {
            switch (simpleOsmGeo.Type)
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
        /// Creates a complete way.
        /// </summary>
        public static CompleteWay CreateComplete(this Way way, IOsmGeoSource osmGeoSource)
        {
            if (way == null) throw new ArgumentNullException("simpleWay");
            if (way.Id == null) throw new Exception("simpleWay.id is null");
            if (osmGeoSource == null) throw new ArgumentNullException("osmGeoSource");

            var completeWay = new CompleteWay();
            completeWay.Id = way.Id.Value;
            completeWay.ChangeSetId = way.ChangeSetId;
            if (way.Tags != null)
            {
                completeWay.Tags = new TagsCollection(way.Tags);
            }
            if (way.Nodes != null)
            {
                var nodes = new List<Node>();
                for (int i = 0; i < way.Nodes.Length; i++)
                {
                    var node = osmGeoSource.GetNode(way.Nodes[i]);
                    if (node == null)
                    {
                        return null;
                    }
                    nodes.Add(node);
                }
                completeWay.Nodes = nodes.ToArray();
            }
            completeWay.TimeStamp = way.TimeStamp;
            completeWay.UserName = way.UserName;
            completeWay.UserId = way.UserId;
            completeWay.Version = way.Version;
            completeWay.Visible = way.Visible;
            return completeWay;
        }

        /// <summary>
        /// Creates a complete relation.
        /// </summary>
        public static CompleteRelation CreateComplete(this Relation relation, IOsmGeoSource osmGeoSource)
        {
            if (relation == null) throw new ArgumentNullException("relation");
            if (relation.Id == null) throw new Exception("relation.Id is null");
            if (osmGeoSource == null) throw new ArgumentNullException("osmGeoSource");

            var completeRelation = new CompleteRelation();
            completeRelation.Id = relation.Id.Value;

            completeRelation.ChangeSetId = relation.ChangeSetId;
            if (relation.Tags != null)
            {
                completeRelation.Tags = new TagsCollection(relation.Tags);
            }
            if (relation.Members != null)
            {
                var relationMembers = new List<CompleteRelationMember>();
                for (var i = 0; i < relation.Members.Length; i++)
                {
                    var memberId = relation.Members[i].Id;
                    var role = relation.Members[i].Role;
                    var member = new CompleteRelationMember();
                    member.Role = role;
                    switch (relation.Members[i].Type)
                    {
                        case OsmGeoType.Node:
                            var memberNode = osmGeoSource.GetNode(memberId);
                            if (memberNode == null)
                            {
                                continue;
                            }
                            var completeMemberNode = memberNode;
                            if (completeMemberNode != null)
                            {
                                member.Member = completeMemberNode;
                            }
                            else
                            {
                                continue;
                            }
                            break;
                        case OsmGeoType.Way:
                            var memberWay = osmGeoSource.GetWay(memberId);
                            if (memberWay == null)
                            {
                                continue;
                            }
                            var completeMemberWay = memberWay.CreateComplete(osmGeoSource);
                            if (completeMemberWay != null)
                            {
                                member.Member = completeMemberWay;
                            }
                            else
                            {
                                continue;
                            }
                            break;
                        case OsmGeoType.Relation:
                            if (relation.Id == memberId)                                
                                continue;
                            var relationMember = osmGeoSource.GetRelation(memberId);
                            if (relationMember == null)
                            {
                                continue;
                            }
                            var completeMemberRelation = relationMember.CreateComplete(osmGeoSource);
                            if (completeMemberRelation != null)
                            {
                                member.Member = completeMemberRelation;
                            }
                            else
                            {
                                continue;
                            }
                            break;
                    }
                    relationMembers.Add(member);
                }
                completeRelation.Members = relationMembers.ToArray();
            }
            completeRelation.TimeStamp = relation.TimeStamp;
            completeRelation.UserName = relation.UserName;
            completeRelation.UserId = relation.UserId;
            completeRelation.Version = relation.Version;
            completeRelation.Visible = relation.Visible;
            return completeRelation;
        }

        /// <summary>
        /// Returns true if this way is closed.
        /// </summary>
        public static bool IsClosed(this CompleteWay way)
        {
            return way.Nodes != null &&
                way.Nodes.Length > 1 &&
                way.Nodes[0].Id == way.Nodes[way.Nodes.Length - 1].Id;
        }

        /// <summary>
        /// Converts these complete elements into their simple counterparts,
        /// including the simple versions of their component elements.
        /// Resulting elements will be distinct.
        /// </summary>
        public static OsmGeo[] ToSimpleWithChildren(this IEnumerable<ICompleteOsmGeo> completes)
        {
            return completes.SelectMany(e => e.ToSimpleWithChildren()).DistinctByGeoKey().ToArray();
        }

        /// <summary>
        /// Converts a complete element into its simple counterpart,
        /// including the simple versions of its component elements.
        /// </summary>
        public static OsmGeo[] ToSimpleWithChildren(this ICompleteOsmGeo complete)
        {
            switch (complete)
            {
                case Node node:
                    return new [] { node };
                case CompleteWay way:
                    return way.ToSimpleWithChildren();
                case CompleteRelation relation:
                    return relation.ToSimpleWithChildren();
                default:
                    throw new Exception("Unknown Complete Element Type.");
            }
        }

        /// <summary>
        /// Converts a complete way into its simple counterpart,
        /// including the simple versions of nodes.
        /// </summary>
        public static OsmGeo[] ToSimpleWithChildren(this CompleteWay way)
        {
            return way.Nodes.DistinctByGeoKey().Append(way.ToSimple()).ToArray();
        }

        /// <summary>
        /// Converts a complete relation into its simple counterpart,
        /// including the simple versions of all members, and their components, recursively.
        /// </summary>
        public static OsmGeo[] ToSimpleWithChildren(this CompleteRelation relation)
        {
            var withChildren = relation.ToSimpleWithChildrenCircularSafe(new HashSet<OsmGeoKey>());
            return withChildren.DistinctByGeoKey().ToArray();
        }

        // Recursive, safe against circular references, may return duplicate nodes.
        private static IEnumerable<OsmGeo> ToSimpleWithChildrenCircularSafe(
            this CompleteRelation complete, HashSet<OsmGeoKey> seenGeoKeys)
        {
            if (complete.Members == null)
            {
                return new[] { complete.ToSimple() };
            }

            var children = new List<OsmGeo>();

            foreach (var completeMember in complete.Members.Where(m => m != null))
            {
                var key = new OsmGeoKey(completeMember.Member.Type, completeMember.Member.Id);

                if (seenGeoKeys.Add(key)) // Returns true if element was not already present
                {
                    switch (completeMember.Member)
                    {
                        case Node node:
                            children.Add(node);
                            break;
                        case CompleteWay way:
                            children.AddRange(way.ToSimpleWithChildren());
                            break;
                        case CompleteRelation relation:
                            children.AddRange(relation.ToSimpleWithChildrenCircularSafe(seenGeoKeys));
                            break;
                        default:
                            throw new Exception("Unknown Complete Element Type.");
                    }
                }
            }

            return children.Append(complete.ToSimple());
        }

        private static IEnumerable<OsmGeo> DistinctByGeoKey(this IEnumerable<OsmGeo> elements)
        {
            return elements.GroupBy(e => new OsmGeoKey(e)).Select(g => g.First());
        }
    }
}
