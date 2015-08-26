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
using OsmSharp.Math.Structures;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Structures.QTree;

namespace OsmSharp.Test.Math.Structures
{
    /// <summary>
    /// Contains tests for a QuadTree index.
    /// </summary>
    [TestFixture]
    public class LocatedObjectQTTests : LocatedObjectIndexTest
    {
        /// <summary>
        /// Tests a quad tree implementation of the located QT index.
        /// </summary>
        [Test]
        public void TestLocatedObjectQTSimple()
        {
            this.DoTestSimple();
        }

        /// <summary>
        /// Tests a quad tree implementation of the located QT index.
        /// </summary>
        [Test]
        public void TestLocatedObjectQTIndex()
        {
            this.DoTestAddingRandom(1000);
        }

        /// <summary>
        /// Creates a located object index to test.
        /// </summary>
        /// <returns></returns>
        public override ILocatedObjectIndex<GeoCoordinate, LocatedObjectData> CreateIndex()
        {
            return new QuadTree<GeoCoordinate, LocatedObjectData>();
            //return new QuadTree<GeoCoordinate, LocatedObjectData>(5,
            //    new GeoCoordinateBox(new GeoCoordinate(50, 3), new GeoCoordinate(40, 2)));
        }
    }
}
