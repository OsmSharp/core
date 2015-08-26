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
using OsmSharp.Math;
using OsmSharp.Math.Primitives;

namespace OsmSharp.Test.Math.Primitives
{
    /// <summary>
    /// Tests for the Line2D class.
    /// </summary>
    [TestFixture]
    public class Line2DTests
    {
        /// <summary>
        /// Tests if the line position test is correct.
        /// </summary>
        [Test]
        public void LinePosition2DTest()
        {
            PointF2D a = new PointF2D(0, 0);
            PointF2D b = new PointF2D(1, 1);

            LineF2D line = new LineF2D(a, b);

            // test where the position lie.
            Assert.AreEqual(line.PositionOfPoint(new PointF2D(0, 0.5f)), LinePointPosition.Left, "Point position should be right!");
            Assert.AreEqual(line.PositionOfPoint(new PointF2D(0.5f, 0.5f)), LinePointPosition.On, "Point position should be on!");
            Assert.AreEqual(line.PositionOfPoint(new PointF2D(0.5f, 0)), LinePointPosition.Right, "Point position should be left!");
        }

        /// <summary>
        /// Tests if the line-point distance is correct.
        /// </summary>
        [Test]
        public void LineDistance2DTest()
        {
            double delta = 0.000000000000001;

            // create the line to test.
            PointF2D a = new PointF2D(0, 0);
            PointF2D b = new PointF2D(1, 1);
            LineF2D line = new LineF2D(a, b);

            // calculate the results
            double sqrt_2 = (double)System.Math.Sqrt(2);
            double sqrt_2_div_2 = (double)System.Math.Sqrt(2) / 2.0f;

            // the point to test to.
            PointF2D c = new PointF2D(1, 0);
            Assert.AreEqual(line.Distance(c), sqrt_2_div_2, delta, string.Format("Point distance should be {0}f!", sqrt_2_div_2));

            // the point to test to.
            c = new PointF2D(0, 1);
            Assert.AreEqual(line.Distance(c), sqrt_2_div_2, delta, string.Format("Point distance should be {0}f!", sqrt_2_div_2));

            // the point to test to.
            c = new PointF2D(2, 2);
            Assert.AreEqual(line.Distance(c), 0.0f, delta, "Point distance should be 0.0f!");

            // the point to test to.
            c = new PointF2D(2, 3);
            Assert.AreEqual(line.Distance(c), sqrt_2_div_2, delta, string.Format("Point distance should be {0}f!", sqrt_2_div_2));

            // the point to test to.
            c = new PointF2D(3, 2);
            Assert.AreEqual(line.Distance(c), sqrt_2_div_2, delta, string.Format("Point distance should be {0}f!", sqrt_2_div_2));

            // Segments tests.
            line = new LineF2D(a, b, true, true);

            // the point to test to.
            c = new PointF2D(1, 0);
            Assert.AreEqual(line.Distance(c), sqrt_2_div_2, delta, string.Format("Point distance should be {0}f!", sqrt_2_div_2));

            // the point to test to.
            c = new PointF2D(0, 1);
            Assert.AreEqual(line.Distance(c), sqrt_2_div_2, delta, string.Format("Point distance should be {0}f!", sqrt_2_div_2));

            // the point to test to.
            c = new PointF2D(2, 2);
            Assert.AreEqual(line.Distance(c), sqrt_2, delta, string.Format("Point distance should be {0}!", sqrt_2));

            // the point to test to.
            c = new PointF2D(2, 1);
            Assert.AreEqual(line.Distance(c), 1, delta, string.Format("Point distance should be {0}f!", 1));

            // the point to test to.
            c = new PointF2D(1, 2);
            Assert.AreEqual(line.Distance(c), 1, delta, string.Format("Point distance should be {0}f!", 1));
        }

        /// <summary>
        /// Tests if the line-point distance is correct.
        /// </summary>
        [Test]
        public void LineDistance2DSegmentTest()
        {
            double delta = 0.000000000000001;

            // create the line to test.
            PointF2D a = new PointF2D(0, 0);
            PointF2D b = new PointF2D(1, 1);
            LineF2D line = new LineF2D(a, b, true, true);

            // calculate the results
            double sqrt_2 = (double)System.Math.Sqrt(2);
            double sqrt_2_div_2 = (double)System.Math.Sqrt(2) / 2.0f;

            // the point to test to.
            PointF2D c = new PointF2D(1, 0);
            Assert.AreEqual(line.Distance(c), sqrt_2_div_2, delta, string.Format("Point distance should be {0}f!", sqrt_2_div_2));

            // the point to test to.
            c = new PointF2D(0, 1);
            Assert.AreEqual(line.Distance(c), sqrt_2_div_2, delta, string.Format("Point distance should be {0}f!", sqrt_2_div_2));

            // the point to test to.
            c = new PointF2D(2, 2);
            Assert.AreEqual(line.Distance(c), sqrt_2, delta);

            // the point to test to.
            c = new PointF2D(2, 3);
            Assert.AreEqual(line.Distance(c), 2.23606797749979, delta, string.Format("Point distance should be {0}f!", sqrt_2_div_2));

            // the point to test to.
            c = new PointF2D(3, 2);
            Assert.AreEqual(line.Distance(c), 2.23606797749979, delta, string.Format("Point distance should be {0}f!", sqrt_2_div_2));

            // the point to test to.
            c = new PointF2D(1, 0);
            Assert.AreEqual(line.Distance(c), sqrt_2_div_2, delta, string.Format("Point distance should be {0}f!", sqrt_2_div_2));

            // the point to test to.
            c = new PointF2D(0, 1);
            Assert.AreEqual(line.Distance(c), sqrt_2_div_2, delta, string.Format("Point distance should be {0}f!", sqrt_2_div_2));

            // the point to test to.
            c = new PointF2D(2, 2);
            Assert.AreEqual(line.Distance(c), sqrt_2, delta, string.Format("Point distance should be {0}!", sqrt_2));

            // the point to test to.
            c = new PointF2D(2, 1);
            Assert.AreEqual(line.Distance(c), 1, delta, string.Format("Point distance should be {0}f!", 1));

            // the point to test to.
            c = new PointF2D(1, 2);
            Assert.AreEqual(line.Distance(c), 1, delta, string.Format("Point distance should be {0}f!", 1));

            // the point to test to.
            c = new PointF2D(-1, -1);
            Assert.AreEqual(line.Distance(c), sqrt_2, delta, string.Format("Point distance should be {0}f!", 1));

            // the point to test to.
            c = new PointF2D(1, -1);
            Assert.AreEqual(line.Distance(c), sqrt_2, delta, string.Format("Point distance should be {0}f!", 1));
        }

        /// <summary>
        /// Tests the intersections of the line.
        /// </summary>
        [Test]
        public void LineSegmentIntersectionTests()
        {
            // double segments.
            LineF2D segment1 = new LineF2D(0, 0, 5, 0, true);
            LineF2D segment2 = new LineF2D(0, 0, 0, 5, true);
            LineF2D segment3 = new LineF2D(0, 3, 3, 0, true);
            LineF2D segment4 = new LineF2D(1, 1, 2, 2, true);
            LineF2D segment5 = new LineF2D(3, 3, 4, 4, true);
            LineF2D segment6 = new LineF2D(3, 1, 3, -1, true);

            PrimitiveF2D primitive = segment1.Intersection(segment2);
            Assert.IsNotNull(primitive);
            Assert.IsInstanceOf<PointF2D>(primitive);
            Assert.AreEqual(new PointF2D(0, 0), primitive as PointF2D);

            primitive = segment1.Intersection(segment3);
            Assert.IsNotNull(primitive);
            Assert.IsInstanceOf<PointF2D>(primitive);
            Assert.AreEqual(new PointF2D(3, 0), primitive as PointF2D);

            primitive = segment2.Intersection(segment3);
            Assert.IsNotNull(primitive);
            Assert.IsInstanceOf<PointF2D>(primitive);
            Assert.AreEqual(new PointF2D(0, 3), primitive as PointF2D);

            primitive = segment3.Intersection(segment4);
            Assert.IsNotNull(primitive);
            Assert.IsInstanceOf<PointF2D>(primitive);
            Assert.AreEqual(new PointF2D(1.5, 1.5), primitive as PointF2D);

            primitive = segment5.Intersection(segment1);
            Assert.IsNull(primitive);
            primitive = segment5.Intersection(segment2);
            Assert.IsNull(primitive);
            primitive = segment5.Intersection(segment3);
            Assert.IsNull(primitive);
            primitive = segment5.Intersection(segment4);
            Assert.IsNull(primitive);
            primitive = segment5.Intersection(segment6);
            Assert.IsNull(primitive);

            primitive = segment6.Intersection(segment3);
            Assert.IsNotNull(primitive);
            Assert.IsInstanceOf<PointF2D>(primitive);
            Assert.AreEqual(new PointF2D(3, 0), primitive as PointF2D);

            primitive = segment6.Intersection(segment1);
            Assert.IsNotNull(primitive);
            Assert.IsInstanceOf<PointF2D>(primitive);
            Assert.AreEqual(new PointF2D(3, 0), primitive as PointF2D);
            
            primitive = segment6.Intersection(segment3);
            Assert.IsNotNull(primitive);
            Assert.IsInstanceOf<PointF2D>(primitive);
            Assert.AreEqual(new PointF2D(3, 0), primitive as PointF2D);

            // half segments.
            LineF2D segment7 = new LineF2D(1.5, 2, 1.5, 0, true, false); // only closed upwards.
            LineF2D segment9 = new LineF2D(1.5, 2, 1.5, 4, true, false); // only closed downwards.

            LineF2D segment8 = new LineF2D(1.5, 1, 1.5, 0, true, false); // only closed upwards.
            LineF2D segment10 = new LineF2D(1.5, 1, 1.5, 4, true, false); // only closed downwards.

            primitive = segment7.Intersection(segment3);
            Assert.IsNotNull(primitive);
            Assert.IsInstanceOf<PointF2D>(primitive);
            Assert.AreEqual(new PointF2D(1.5, 1.5), primitive as PointF2D);
            primitive = segment3.Intersection(segment7);
            Assert.IsNotNull(primitive);
            Assert.IsInstanceOf<PointF2D>(primitive);
            Assert.AreEqual(new PointF2D(1.5, 1.5), primitive as PointF2D);
            primitive = segment9.Intersection(segment3);
            Assert.IsNull(primitive);
            primitive = segment3.Intersection(segment9);
            Assert.IsNull(primitive);

            primitive = segment10.Intersection(segment3);
            Assert.IsNotNull(primitive);
            Assert.IsInstanceOf<PointF2D>(primitive);
            Assert.AreEqual(new PointF2D(1.5, 1.5), primitive as PointF2D);
            primitive = segment3.Intersection(segment10);
            Assert.IsNotNull(primitive);
            Assert.IsInstanceOf<PointF2D>(primitive);
            Assert.AreEqual(new PointF2D(1.5, 1.5), primitive as PointF2D);
            primitive = segment8.Intersection(segment3);
            Assert.IsNull(primitive);
            primitive = segment3.Intersection(segment8);
            Assert.IsNull(primitive);

            LineF2D segment11 = new LineF2D(-1, 1, 0, 1, true, false);
            LineF2D segment12 = new LineF2D(0, 3, 3, 0, true, true);
            primitive = segment11.Intersection(segment12);
            Assert.IsNotNull(primitive);
            Assert.IsInstanceOf<PointF2D>(primitive);
            Assert.AreEqual(new PointF2D(2, 1), primitive as PointF2D);
            primitive = segment12.Intersection(segment11);
            Assert.IsNotNull(primitive);
            Assert.IsInstanceOf<PointF2D>(primitive);
            Assert.AreEqual(new PointF2D(2, 1), primitive as PointF2D);
        }
    }
}
