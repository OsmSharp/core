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

using NUnit.Framework;
using OsmSharp.Osm;
using OsmSharp.Osm.Data.Memory;
using OsmSharp.Osm.Streams;
using OsmSharp.Osm.Streams.Complete;
using OsmSharp.Osm.Xml.Streams;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OsmSharp.Test.Osm.Streams.Complete
{
    /// <summary>
    /// Contains test for the simple to complete stream conversion class.
    /// </summary>
    [TestFixture]
    public class OsmSimpleCompleteStreamSourceTests
    {
        /// <summary>
        /// Tests simple to complete node conversion.
        /// </summary>
        [Test]
        public void TestSimpleToCompleteNode()
        {
            // execute
            var completeList = this.PullToCompleteList(new OsmGeo[] {
                Node.Create(1, 0, 0),
                Node.Create(2, 1, 0),
                Node.Create(3, 0, 1) });

            // verify.
            Assert.IsNotNull(completeList);
            Assert.AreEqual(3, completeList.Count);
            Assert.IsTrue(completeList.Any(x => (x.Id == 1 && 
                (x as Node).Coordinate.Latitude == 0 &&
                (x as Node).Coordinate.Longitude == 0)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 2 &&
                (x as Node).Coordinate.Latitude == 1 &&
                (x as Node).Coordinate.Longitude == 0)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 3 &&
                (x as Node).Coordinate.Latitude == 0 &&
                (x as Node).Coordinate.Longitude == 1)));
        }

        /// <summary>
        /// Tests simple to complete way conversion.
        /// </summary>
        [Test]
        public void TestSimpleToCompleteWay()
        {
            // execute
            var completeList = this.PullToCompleteList(new OsmGeo[] {
                Node.Create(1, 0, 0),
                Node.Create(2, 1, 0),
                Node.Create(3, 0, 1),
                Way.Create(1 ,1,2,3)});

            // verify.
            Assert.IsNotNull(completeList);
            Assert.AreEqual(4, completeList.Count);
            Assert.IsTrue(completeList.Any(x => (x.Id == 1 && (x is Node) &&
                (x as Node).Coordinate.Latitude == 0 &&
                (x as Node).Coordinate.Longitude == 0)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 2 && (x is Node) &&
                (x as Node).Coordinate.Latitude == 1 &&
                (x as Node).Coordinate.Longitude == 0)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 3 && (x is Node) &&
                (x as Node).Coordinate.Latitude == 0 &&
                (x as Node).Coordinate.Longitude == 1)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 1 && (x is CompleteWay) &&
                (x as CompleteWay).Nodes.Count == 3 &&
                (x as CompleteWay).Nodes[0].Id == 1 &&
                (x as CompleteWay).Nodes[0].Coordinate.Latitude == 0 &&
                (x as CompleteWay).Nodes[0].Coordinate.Longitude == 0 &&
                (x as CompleteWay).Nodes[1].Id == 2 &&
                (x as CompleteWay).Nodes[1].Coordinate.Latitude == 1 &&
                (x as CompleteWay).Nodes[1].Coordinate.Longitude == 0 &&
                (x as CompleteWay).Nodes[2].Id == 3 &&
                (x as CompleteWay).Nodes[2].Coordinate.Latitude == 0 &&
                (x as CompleteWay).Nodes[2].Coordinate.Longitude == 1)));
        }

        /// <summary>
        /// Tests simple to complete relation conversion.
        /// </summary>
        [Test]
        public void TestSimpleToCompleteRelation()
        {
            // execute
            var completeList = this.PullToCompleteList(new OsmGeo[] {
                Node.Create(1, 0, 0),
                Node.Create(2, 1, 0),
                Node.Create(3, 0, 1),
                Way.Create(1 ,1,2,3),
                Relation.Create(1, 
                    RelationMember.Create(1, "way", OsmGeoType.Way))});

            // verify.
            Assert.IsNotNull(completeList);
            Assert.AreEqual(5, completeList.Count);
            Assert.IsTrue(completeList.Any(x => (x.Id == 1 && (x is Node) &&
                (x as Node).Coordinate.Latitude == 0 &&
                (x as Node).Coordinate.Longitude == 0)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 2 && (x is Node) &&
                (x as Node).Coordinate.Latitude == 1 &&
                (x as Node).Coordinate.Longitude == 0)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 3 && (x is Node) &&
                (x as Node).Coordinate.Latitude == 0 &&
                (x as Node).Coordinate.Longitude == 1)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 1 && (x is CompleteWay) &&
                (x as CompleteWay).Nodes.Count == 3 &&
                (x as CompleteWay).Nodes[0].Id == 1 &&
                (x as CompleteWay).Nodes[0].Coordinate.Latitude == 0 &&
                (x as CompleteWay).Nodes[0].Coordinate.Longitude == 0 &&
                (x as CompleteWay).Nodes[1].Id == 2 &&
                (x as CompleteWay).Nodes[1].Coordinate.Latitude == 1 &&
                (x as CompleteWay).Nodes[1].Coordinate.Longitude == 0 &&
                (x as CompleteWay).Nodes[2].Id == 3 &&
                (x as CompleteWay).Nodes[2].Coordinate.Latitude == 0 &&
                (x as CompleteWay).Nodes[2].Coordinate.Longitude == 1)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 1 && (x is CompleteRelation) &&
                (x as CompleteRelation).Members.Count == 1)));
        }

        /// <summary>
        /// Tests simple to complete with nested relations.
        /// </summary>
        [Test]
        public void TestSimpleToCompleteNestedRelations()
        {
            // execute
            var completeList = this.PullToCompleteList(new OsmGeo[] {
                Node.Create(1, 0, 0),
                Node.Create(2, 1, 0),
                Node.Create(3, 0, 1),
                Way.Create(1 ,1,2,3),
                Relation.Create(1, 
                    RelationMember.Create(1, "way", OsmGeoType.Way)),
                Relation.Create(2, 
                    RelationMember.Create(1, "way", OsmGeoType.Way),
                    RelationMember.Create(1, "relation", OsmGeoType.Relation))});

            // verify.
            Assert.IsNotNull(completeList);
            Assert.AreEqual(6, completeList.Count);
            Assert.IsTrue(completeList.Any(x => (x.Id == 1 && (x is Node) &&
                (x as Node).Coordinate.Latitude == 0 &&
                (x as Node).Coordinate.Longitude == 0)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 2 && (x is Node) &&
                (x as Node).Coordinate.Latitude == 1 &&
                (x as Node).Coordinate.Longitude == 0)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 3 && (x is Node) &&
                (x as Node).Coordinate.Latitude == 0 &&
                (x as Node).Coordinate.Longitude == 1)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 1 && (x is CompleteWay) &&
                (x as CompleteWay).Nodes.Count == 3 &&
                (x as CompleteWay).Nodes[0].Id == 1 &&
                (x as CompleteWay).Nodes[0].Coordinate.Latitude == 0 &&
                (x as CompleteWay).Nodes[0].Coordinate.Longitude == 0 &&
                (x as CompleteWay).Nodes[1].Id == 2 &&
                (x as CompleteWay).Nodes[1].Coordinate.Latitude == 1 &&
                (x as CompleteWay).Nodes[1].Coordinate.Longitude == 0 &&
                (x as CompleteWay).Nodes[2].Id == 3 &&
                (x as CompleteWay).Nodes[2].Coordinate.Latitude == 0 &&
                (x as CompleteWay).Nodes[2].Coordinate.Longitude == 1)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 1 && (x is CompleteRelation) &&
                (x as CompleteRelation).Members.Count == 1)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 2 && (x is CompleteRelation) &&
                (x as CompleteRelation).Members.Count == 2)));
        }

        /// <summary>
        /// Tests simple to complete with nested relations but in reverse order.
        /// </summary>
        [Test]
        public void TestSimpleToCompleteNestedRelationsReverseOrder()
        {
            // execute
            var completeList = this.PullToCompleteList(new OsmGeo[] {
                Node.Create(1, 0, 0),
                Node.Create(2, 1, 0),
                Node.Create(3, 0, 1),
                Way.Create(1 ,1,2,3),
                Relation.Create(2, 
                    RelationMember.Create(1, "way", OsmGeoType.Way),
                    RelationMember.Create(1, "relation", OsmGeoType.Relation)),
                Relation.Create(1, 
                    RelationMember.Create(1, "way", OsmGeoType.Way))});

            // verify.
            Assert.IsNotNull(completeList);
            Assert.AreEqual(6, completeList.Count);
            Assert.IsTrue(completeList.Any(x => (x.Id == 1 && (x is Node) &&
                (x as Node).Coordinate.Latitude == 0 &&
                (x as Node).Coordinate.Longitude == 0)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 2 && (x is Node) &&
                (x as Node).Coordinate.Latitude == 1 &&
                (x as Node).Coordinate.Longitude == 0)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 3 && (x is Node) &&
                (x as Node).Coordinate.Latitude == 0 &&
                (x as Node).Coordinate.Longitude == 1)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 1 && (x is CompleteWay) &&
                (x as CompleteWay).Nodes.Count == 3 &&
                (x as CompleteWay).Nodes[0].Id == 1 &&
                (x as CompleteWay).Nodes[0].Coordinate.Latitude == 0 &&
                (x as CompleteWay).Nodes[0].Coordinate.Longitude == 0 &&
                (x as CompleteWay).Nodes[1].Id == 2 &&
                (x as CompleteWay).Nodes[1].Coordinate.Latitude == 1 &&
                (x as CompleteWay).Nodes[1].Coordinate.Longitude == 0 &&
                (x as CompleteWay).Nodes[2].Id == 3 &&
                (x as CompleteWay).Nodes[2].Coordinate.Latitude == 0 &&
                (x as CompleteWay).Nodes[2].Coordinate.Longitude == 1)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 1 && (x is CompleteRelation) &&
                (x as CompleteRelation).Members.Count == 1)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 2 && (x is CompleteRelation) &&
                (x as CompleteRelation).Members.Count == 2)));
        }

        /// <summary>
        /// Tests simple to complete with multiple node usages in multiple ways.
        /// </summary>
        [Test]
        public void TestSimpleToCompleteMultipleUsagesNodesInWays()
        {
            // execute
            var completeList = this.PullToCompleteList(new OsmGeo[] {
                Node.Create(1, 0, 0),
                Node.Create(2, 1, 0),
                Node.Create(3, 0, 1),
                Node.Create(4, 1, 1),
                Node.Create(5, 2, 2),
                Way.Create(1, 1, 2, 3),
                Way.Create(2, 3, 4)});

            // verify.
            Assert.IsNotNull(completeList);
            Assert.AreEqual(7, completeList.Count);
            Assert.IsTrue(completeList.Any(x => (x.Id == 1 && (x is Node) &&
                (x as Node).Coordinate.Latitude == 0 &&
                (x as Node).Coordinate.Longitude == 0)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 2 && (x is Node) &&
                (x as Node).Coordinate.Latitude == 1 &&
                (x as Node).Coordinate.Longitude == 0)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 3 && (x is Node) &&
                (x as Node).Coordinate.Latitude == 0 &&
                (x as Node).Coordinate.Longitude == 1)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 4 && (x is Node) &&
                (x as Node).Coordinate.Latitude == 1 &&
                (x as Node).Coordinate.Longitude == 1)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 5 && (x is Node) &&
                (x as Node).Coordinate.Latitude == 2 &&
                (x as Node).Coordinate.Longitude == 2)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 1 && (x is CompleteWay) &&
                (x as CompleteWay).Nodes.Count == 3 &&
                (x as CompleteWay).Nodes[0].Id == 1 &&
                (x as CompleteWay).Nodes[0].Coordinate.Latitude == 0 &&
                (x as CompleteWay).Nodes[0].Coordinate.Longitude == 0 &&
                (x as CompleteWay).Nodes[1].Id == 2 &&
                (x as CompleteWay).Nodes[1].Coordinate.Latitude == 1 &&
                (x as CompleteWay).Nodes[1].Coordinate.Longitude == 0 &&
                (x as CompleteWay).Nodes[2].Id == 3 &&
                (x as CompleteWay).Nodes[2].Coordinate.Latitude == 0 &&
                (x as CompleteWay).Nodes[2].Coordinate.Longitude == 1)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 2 && (x is CompleteWay) &&
                (x as CompleteWay).Nodes.Count == 2 &&
                (x as CompleteWay).Nodes[0].Id == 3 &&
                (x as CompleteWay).Nodes[0].Coordinate.Latitude == 0 &&
                (x as CompleteWay).Nodes[0].Coordinate.Longitude == 1 &&
                (x as CompleteWay).Nodes[1].Id == 4 &&
                (x as CompleteWay).Nodes[1].Coordinate.Latitude == 1 &&
                (x as CompleteWay).Nodes[1].Coordinate.Longitude == 1)));
        }

        /// <summary>
        /// Tests simple to complete with nested relations.
        /// </summary>
        [Test]
        public void TestSimpleToCompleteMultipleUsages()
        {
            // execute
            var completeList = this.PullToCompleteList(new OsmGeo[] {
                Node.Create(1, 0, 0),
                Node.Create(2, 1, 0),
                Node.Create(3, 0, 1),
                Node.Create(4, 2, 0),
                Node.Create(5, 0, 2),
                Way.Create(1, 1, 2, 3),
                Way.Create(2, 1, 4, 5),
                Way.Create(3, 3, 2, 5),
                Way.Create(5, 10, 11, 12),
                Relation.Create(1,
                    RelationMember.Create(1, "way", OsmGeoType.Way)),
                Relation.Create(2,
                    RelationMember.Create(1, "way", OsmGeoType.Way),
                    RelationMember.Create(1, "relation", OsmGeoType.Relation)),
                Relation.Create(3,
                    RelationMember.Create(1, "node", OsmGeoType.Node),
                    RelationMember.Create(2, "node", OsmGeoType.Node),
                    RelationMember.Create(3, "node", OsmGeoType.Node)),
                Relation.Create(4,
                    RelationMember.Create(10, "node", OsmGeoType.Node),
                    RelationMember.Create(11, "node", OsmGeoType.Node),
                    RelationMember.Create(12, "node", OsmGeoType.Node))});

            // verify.
            Assert.IsNotNull(completeList);
            Assert.AreEqual(11, completeList.Count);
            Assert.IsTrue(completeList.Any(x => (x.Id == 1 && (x is Node) &&
                (x as Node).Coordinate.Latitude == 0 &&
                (x as Node).Coordinate.Longitude == 0)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 2 && (x is Node) &&
                (x as Node).Coordinate.Latitude == 1 &&
                (x as Node).Coordinate.Longitude == 0)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 3 && (x is Node) &&
                (x as Node).Coordinate.Latitude == 0 &&
                (x as Node).Coordinate.Longitude == 1)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 4 && (x is Node) &&
                (x as Node).Coordinate.Latitude == 2 &&
                (x as Node).Coordinate.Longitude == 0)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 5 && (x is Node) &&
                (x as Node).Coordinate.Latitude == 0 &&
                (x as Node).Coordinate.Longitude == 2)));

            Assert.IsTrue(completeList.Any(x => (x.Id == 1 && (x is CompleteWay) &&
                (x as CompleteWay).Nodes.Count == 3 &&
                (x as CompleteWay).Nodes[0].Id == 1 &&
                (x as CompleteWay).Nodes[0].Coordinate.Latitude == 0 &&
                (x as CompleteWay).Nodes[0].Coordinate.Longitude == 0 &&
                (x as CompleteWay).Nodes[1].Id == 2 &&
                (x as CompleteWay).Nodes[1].Coordinate.Latitude == 1 &&
                (x as CompleteWay).Nodes[1].Coordinate.Longitude == 0 &&
                (x as CompleteWay).Nodes[2].Id == 3 &&
                (x as CompleteWay).Nodes[2].Coordinate.Latitude == 0 &&
                (x as CompleteWay).Nodes[2].Coordinate.Longitude == 1)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 2 && (x is CompleteWay) &&
                (x as CompleteWay).Nodes.Count == 3 &&
                (x as CompleteWay).Nodes[0].Id == 1 &&
                (x as CompleteWay).Nodes[0].Coordinate.Latitude == 0 &&
                (x as CompleteWay).Nodes[0].Coordinate.Longitude == 0 &&
                (x as CompleteWay).Nodes[1].Id == 4 &&
                (x as CompleteWay).Nodes[1].Coordinate.Latitude == 2 &&
                (x as CompleteWay).Nodes[1].Coordinate.Longitude == 0 &&
                (x as CompleteWay).Nodes[2].Id == 5 &&
                (x as CompleteWay).Nodes[2].Coordinate.Latitude == 0 &&
                (x as CompleteWay).Nodes[2].Coordinate.Longitude == 2)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 3 && (x is CompleteWay) &&
                (x as CompleteWay).Nodes.Count == 3 &&
                (x as CompleteWay).Nodes[0].Id == 3 &&
                (x as CompleteWay).Nodes[0].Coordinate.Latitude == 0 &&
                (x as CompleteWay).Nodes[0].Coordinate.Longitude == 1 &&
                (x as CompleteWay).Nodes[1].Id == 2 &&
                (x as CompleteWay).Nodes[1].Coordinate.Latitude == 1 &&
                (x as CompleteWay).Nodes[1].Coordinate.Longitude == 0 &&
                (x as CompleteWay).Nodes[2].Id == 5 &&
                (x as CompleteWay).Nodes[2].Coordinate.Latitude == 0 &&
                (x as CompleteWay).Nodes[2].Coordinate.Longitude == 2)));
            Assert.IsFalse(completeList.Any(x => (x.Id == 5 && (x is CompleteWay))));

            Assert.IsTrue(completeList.Any(x => (x.Id == 1 && (x is CompleteRelation) &&
                (x as CompleteRelation).Members.Count == 1)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 2 && (x is CompleteRelation) &&
                (x as CompleteRelation).Members.Count == 2)));
            Assert.IsTrue(completeList.Any(x => (x.Id == 3 && (x is CompleteRelation) &&
                (x as CompleteRelation).Members.Count == 3)));
        }

        /// <summary>
        /// Tests simple to complete of osm_network.
        /// </summary>
        [Test]
        public void TestSimpleToCompleteOsmNetwork()
        {
            this.TestSimpleToCompleteOn("OsmSharp.Test.data.test_network.osm");
        }

        /// <summary>
        /// Execute the simple-to-complete code.
        /// </summary>
        /// <param name="osmGeoList"></param>
        /// <returns></returns>
        private List<ICompleteOsmGeo> PullToCompleteList(IEnumerable<OsmGeo> osmGeoList)
        {
            return new List<ICompleteOsmGeo>( // pull into collection.
                new OsmSimpleCompleteStreamSource( // create complete source.
                    osmGeoList.ToOsmStreamSource())); // create the basic stream.
        }

        /// <summary>
        /// Test simple to complete conversion on the given resource.
        /// </summary>
        /// <param name="embeddedResource"></param>
        private void TestSimpleToCompleteOn(string embeddedResource)
        {
            var dataProcessorSource = new XmlOsmStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedResource));

            // fill the memory data source with source-data.
            var referenceSource = MemoryDataSource.CreateFrom(dataProcessorSource);

            // pull the complete objects.
            dataProcessorSource.Reset();
            var list = this.PullToCompleteList(dataProcessorSource);

            // compare the two.
            this.Compare(referenceSource, list);
        }

        /// <summary>
        /// Compares what is in the complete list against the objects in the reference source.
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        private void Compare(MemoryDataSource expected, List<ICompleteOsmGeo> actual)
        {
            var exectedList = new List<ICompleteOsmGeo>();
            foreach (var node in expected.GetNodes())
            {
                var completeNode = node;
                if (completeNode != null)
                {
                    exectedList.Add(completeNode);
                }
            }
            foreach (var way in expected.GetWays())
            {
                var completeWay = CompleteWay.CreateFrom(way, expected);
                if (completeWay != null)
                {
                    exectedList.Add(completeWay);
                }
            }
            foreach (var relation in expected.GetRelations())
            {
                var completeRelation = CompleteRelation.CreateFrom(relation, expected);
                if (completeRelation != null)
                {
                    exectedList.Add(completeRelation);
                }
            }

            ComparisonHelpers.CompareComplete(exectedList, actual);
        }
    }
}
