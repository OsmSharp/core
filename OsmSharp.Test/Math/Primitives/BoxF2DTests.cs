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
using NUnit.Framework;
using OsmSharp.Math.Primitives;

namespace OsmSharp.Test.Math.Primitives
{
    /// <summary>
    /// Holds common tests for the BoxF2D class.
    /// </summary>
    [TestFixture]
    public class BoxF2DTests
    {
        /// <summary>
        /// Tests the union operation.
        /// </summary>
        [Test]
        public void BoxF2DUnionTest()
        {
			var testDataList = new List<BoxF2D>();
            for (int idx = 0; idx < 10000; idx++)
            {
                double x1 = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(1.0);
                double x2 = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(1.0);
                double y1 = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(1.0);
                double y2 = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(1.0);

				testDataList.Add(new BoxF2D(x1, y1, x2, y2));
            }

			BoxF2D box = testDataList[0];
			foreach (BoxF2D rectangleF2D in testDataList)
            {
                box = box.Union(rectangleF2D);
            }
            
			foreach (BoxF2D rectangleF2D in testDataList)
            {
                box.Contains(rectangleF2D);
            }
        }

        /// <summary>
        /// Tests the overlaps function.
        /// </summary>
        [Test]
        public void BoxF2DOverlapsTest()
        {
			var rect1 = new BoxF2D(0, 0, 2, 2);
			var rect2 = new BoxF2D(3, 2, 5, 4);

            Assert.IsFalse(rect1.Overlaps(rect2));
            Assert.AreEqual(rect1.Overlaps(rect2), rect2.Overlaps(rect1));

			rect1 = new BoxF2D(0, 0, 2, 2);
			rect2 = new BoxF2D(2, 0, 4, 2);
            
            Assert.IsTrue(rect1.Overlaps(rect2));
            Assert.AreEqual(rect1.Overlaps(rect2), rect2.Overlaps(rect1));

			rect1 = new BoxF2D(0, 0, 2, 2);
			rect2 = new BoxF2D(1, 1, 3, 3);

            Assert.IsTrue(rect1.Overlaps(rect2));
            Assert.AreEqual(rect1.Overlaps(rect2), rect2.Overlaps(rect1));

			rect1 = new BoxF2D(1, 0, 2, 3);
			rect2 = new BoxF2D(0, 1, 3, 2);

            Assert.IsTrue(rect1.Overlaps(rect2));
            Assert.AreEqual(rect1.Overlaps(rect2), rect2.Overlaps(rect1));

			rect1 = new BoxF2D(0, 0, 3, 3);
			rect2 = new BoxF2D(1, 1, 2, 2);

            Assert.IsTrue(rect1.Overlaps(rect2));
            Assert.AreEqual(rect1.Overlaps(rect2), rect2.Overlaps(rect1));
        }

        /// <summary>
        /// Line enumeration (regression) test.
        /// </summary>
        [Test]
        public void TestBoxF2DLineEnumeration()
        {
            var rect1 = new BoxF2D(0, 0, 2, 2);

            List<LineF2D> lines = new List<LineF2D>(rect1 as IEnumerable<LineF2D>);
            Assert.AreEqual(4, lines.Count);
            Assert.IsTrue(lines[0].IsSegment);
            Assert.IsTrue(lines[1].IsSegment);
            Assert.IsTrue(lines[2].IsSegment);
            Assert.IsTrue(lines[3].IsSegment);
            Assert.IsTrue(lines.Exists(x => (x.Point1 == rect1.Corners[0] && x.Point2 == rect1.Corners[1]) ||
                (x.Point2 == rect1.Corners[0] && x.Point1 == rect1.Corners[1])));
            Assert.IsTrue(lines.Exists(x => (x.Point1 == rect1.Corners[1] && x.Point2 == rect1.Corners[2]) ||
                (x.Point2 == rect1.Corners[2] && x.Point1 == rect1.Corners[1])));
            Assert.IsTrue(lines.Exists(x => (x.Point1 == rect1.Corners[2] && x.Point2 == rect1.Corners[3]) ||
                (x.Point2 == rect1.Corners[3] && x.Point1 == rect1.Corners[2])));
            Assert.IsTrue(lines.Exists(x => (x.Point1 == rect1.Corners[3] && x.Point2 == rect1.Corners[0]) ||
                (x.Point2 == rect1.Corners[0] && x.Point1 == rect1.Corners[3])));
        }
    }
}
