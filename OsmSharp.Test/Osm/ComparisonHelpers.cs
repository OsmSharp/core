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

using NUnit.Framework;
using OsmSharp.Collections.Tags;
using OsmSharp.Osm;
using System.Linq;

namespace OsmSharp.Test.Osm
{
    /// <summary>
    /// Contains various comparison helpers.
    /// </summary>
    public static class ComparisonHelpers
    {
        /// <summary>
        /// Compares two tags collections.
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        public static void CompareTags(TagsCollectionBase expected, TagsCollectionBase actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
            }
            else
            {
                if (expected.Count == 0)
                {
                    Assert.IsTrue(actual == null || actual.Count == 0);
                }
                else
                {
                    Assert.IsNotNull(actual);
                    Assert.AreEqual(expected.Count, actual.Count);
                    foreach (Tag tag in expected)
                    {
                        Assert.IsTrue(actual.ContainsKeyValue(tag.Key, tag.Value));
                    }
                    foreach (Tag tag in actual)
                    {
                        Assert.IsTrue(expected.ContainsKeyValue(tag.Key, tag.Value));
                    }
                }
            }
        }

        /// <summary>
        /// Compares the two complete objects.
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        public static void CompareComplete(Node expected, Node actual)
        {
            if (expected == null)
            { // ok, if the value is also null.
                Assert.IsNull(actual);
            }
            else
            { // check and compare the value.
                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.Id, actual.Id);
                Assert.AreEqual(expected.ChangeSetId, actual.ChangeSetId);
                Assert.AreEqual(expected.Coordinate, actual.Coordinate);
                Assert.AreEqual(expected.TimeStamp, actual.TimeStamp);
                Assert.AreEqual(expected.UserName, actual.UserName);
                Assert.AreEqual(expected.UserId, actual.UserId);
                Assert.AreEqual(expected.Version, actual.Version);
                Assert.AreEqual(expected.Visible, actual.Visible);
            }
        }

        /// <summary>
        /// Compares the two complete objects.
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        public static void CompareComplete(CompleteWay expected, CompleteWay actual)
        {
            if (expected == null)
            { // ok, if the value is also null.
                Assert.IsNull(actual);
            }
            else
            { // check and compare the value.
                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.Id, actual.Id);
                Assert.AreEqual(expected.ChangeSetId, actual.ChangeSetId);
                Assert.AreEqual(expected.TimeStamp, actual.TimeStamp);
                Assert.AreEqual(expected.User, actual.User);
                Assert.AreEqual(expected.UserId, actual.UserId);
                Assert.AreEqual(expected.Version, actual.Version);
                Assert.AreEqual(expected.Visible, actual.Visible);

                if (expected.Nodes == null)
                { // ok, if the value is also null.
                    Assert.IsNotNull(actual.Nodes);
                }
                else
                { // check and compare the nodes.
                    Assert.AreEqual(expected.Nodes.Count, actual.Nodes.Count);
                    for (int idx = 0; idx < expected.Nodes.Count; idx++)
                    {
                        ComparisonHelpers.CompareComplete(
                            expected.Nodes[idx], actual.Nodes[idx]);
                    }
                }
            }
        }

        /// <summary>
        /// Compares the two complete objects.
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        public static void CompareComplete(CompleteRelation expected, CompleteRelation actual)
        {
            if (expected == null)
            { // ok, if the value is also null.
                Assert.IsNull(actual);
            }
            else
            { // check and compare the value.
                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.Id, actual.Id);
                Assert.AreEqual(expected.ChangeSetId, actual.ChangeSetId);
                Assert.AreEqual(expected.TimeStamp, actual.TimeStamp);
                Assert.AreEqual(expected.User, actual.User);
                Assert.AreEqual(expected.UserId, actual.UserId);
                Assert.AreEqual(expected.Version, actual.Version);
                Assert.AreEqual(expected.Visible, actual.Visible);

                if (expected.Members == null)
                { // ok, if the value is also null.
                    Assert.IsNotNull(actual.Members);
                }
                else
                { // check and compare the nodes.
                    Assert.AreEqual(expected.Members.Count, actual.Members.Count);
                    for (int idx = 0; idx < expected.Members.Count; idx++)
                    {
                        CompleteRelationMember expectedMember = expected.Members[idx];
                        CompleteRelationMember actualMember = actual.Members[idx];

                        Assert.AreEqual(expectedMember.Role, actualMember.Role);
                        Assert.IsNotNull(expectedMember.Member);
                        Assert.IsNotNull(actualMember.Member);
                        Assert.AreEqual(expectedMember.Member.Type, actualMember.Member.Type);

                        switch (expectedMember.Member.Type)
                        {
                            case CompleteOsmType.Node:
                                ComparisonHelpers.CompareComplete(
                                    expectedMember.Member as Node, 
                                    actualMember.Member as Node);
                                break;
                            case CompleteOsmType.Way:
                                ComparisonHelpers.CompareComplete(
                                    expectedMember.Member as CompleteWay,
                                    actualMember.Member as CompleteWay);
                                break;
                            case CompleteOsmType.Relation:
                                ComparisonHelpers.CompareComplete(
                                    expectedMember.Member as CompleteRelation,
                                    actualMember.Member as CompleteRelation);
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Compares a found node to an expected node.
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        public static void CompareSimple(Node expected, Node actual)
        {
            Assert.IsNotNull(expected);
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.ChangeSetId, actual.ChangeSetId);
            Assert.AreEqual((float)expected.Latitude, (float)actual.Latitude);
            Assert.AreEqual((float)expected.Longitude, (float)actual.Longitude);
            Assert.AreEqual(expected.TimeStamp, actual.TimeStamp);
            Assert.AreEqual(expected.Type, actual.Type);
            Assert.AreEqual(expected.UserId, actual.UserId);
            Assert.AreEqual(expected.UserName, actual.UserName);
            Assert.AreEqual(expected.Version, actual.Version);
            Assert.AreEqual(expected.Visible, actual.Visible);

            ComparisonHelpers.CompareTags(expected.Tags, actual.Tags);
        }

        /// <summary>
        /// Compares a found way to an expected way.
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        public static void CompareSimple(Way expected, Way actual)
        {
            Assert.IsNotNull(expected);
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.ChangeSetId, actual.ChangeSetId);
            Assert.AreEqual(expected.TimeStamp, actual.TimeStamp);
            Assert.AreEqual(expected.Type, actual.Type);
            Assert.AreEqual(expected.UserId, actual.UserId);
            Assert.AreEqual(expected.UserName, actual.UserName);
            Assert.AreEqual(expected.Version, actual.Version);
            Assert.AreEqual(expected.Visible, actual.Visible);

            if (expected.Nodes == null)
            {
                Assert.IsNull(actual.Nodes);
            }
            else
            {
                Assert.IsNotNull(actual.Nodes);
                Assert.AreEqual(expected.Nodes.Count, actual.Nodes.Count);
                for (int idx = 0; idx < expected.Nodes.Count; idx++)
                {
                    Assert.AreEqual(expected.Nodes[idx], actual.Nodes[idx]);
                }
            }

            ComparisonHelpers.CompareTags(expected.Tags, actual.Tags);
        }

        /// <summary>
        /// Compares a found relation to an expected relation.
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        public static void CompareSimple(Relation expected, Relation actual)
        {
            Assert.IsNotNull(expected);
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.ChangeSetId, actual.ChangeSetId);
            Assert.AreEqual(expected.TimeStamp, actual.TimeStamp);
            Assert.AreEqual(expected.Type, actual.Type);
            Assert.AreEqual(expected.UserId, actual.UserId);
            Assert.AreEqual(expected.UserName, actual.UserName);
            Assert.AreEqual(expected.Version, actual.Version);
            Assert.AreEqual(expected.Visible, actual.Visible);

            if (expected.Members == null)
            {
                Assert.IsNull(actual.Members);
            }
            else
            {
                Assert.IsNotNull(actual.Members);
                Assert.AreEqual(expected.Members.Count, actual.Members.Count);
                for (int idx = 0; idx < expected.Members.Count; idx++)
                {
                    Assert.AreEqual(expected.Members[idx].MemberId, actual.Members[idx].MemberId);
                    // the oracle database converts empty strings to null and does not follow standards.
                    // this is why there is this ugly code here matching empty strings to null.
                    if (expected.Members[idx].MemberRole == string.Empty &&
                        actual.Members[idx].MemberRole == null)
                    { // only for oracle!
                        Assert.AreEqual(null, actual.Members[idx].MemberRole);
                    }
                    else
                    {
                        Assert.AreEqual(expected.Members[idx].MemberRole, actual.Members[idx].MemberRole);
                    }
                    Assert.AreEqual(expected.Members[idx].MemberType, actual.Members[idx].MemberType);
                }
            }

            ComparisonHelpers.CompareTags(expected.Tags, actual.Tags);
        }

        /// <summary>
        /// Compares two lists of complete osm objects.
        /// </summary>
        /// <param name="expectedList"></param>
        /// <param name="actualList"></param>
        public static void CompareComplete(System.Collections.Generic.List<ICompleteOsmGeo> expectedList, 
			System.Collections.Generic.List<ICompleteOsmGeo> actualList)
        {
            foreach (ICompleteOsmGeo actual in actualList)
            {
                foreach (ICompleteOsmGeo expected in 
                    expectedList.Where((expected) => { return expected.Id == actual.Id && expected.Type == actual.Type; }))
                {
                    switch (expected.Type)
                    {
                        case CompleteOsmType.Node:
                            ComparisonHelpers.CompareComplete(
                                expected as Node, actual as Node);
                            break;
                        case CompleteOsmType.Way:
                            ComparisonHelpers.CompareComplete(
                                expected as CompleteWay, actual as CompleteWay);
                            break;
                        case CompleteOsmType.Relation:
                            ComparisonHelpers.CompareComplete(
                                expected as CompleteRelation, actual as CompleteRelation);
                            break;
                    }
                }
            }
            foreach (ICompleteOsmGeo expected in expectedList)
            {
                foreach (ICompleteOsmGeo actual in
                    actualList.Where((actual) => { return expected.Id == actual.Id && expected.Type == actual.Type; }))
                {
                    switch (expected.Type)
                    {
                        case CompleteOsmType.Node:
                            ComparisonHelpers.CompareComplete(
                                expected as Node, actual as Node);
                            break;
                        case CompleteOsmType.Way:
                            ComparisonHelpers.CompareComplete(
                                expected as CompleteWay, actual as CompleteWay);
                            break;
                        case CompleteOsmType.Relation:
                            ComparisonHelpers.CompareComplete(
                                expected as CompleteRelation, actual as CompleteRelation);
                            break;
                    }
                }
            }
        }
    }
}