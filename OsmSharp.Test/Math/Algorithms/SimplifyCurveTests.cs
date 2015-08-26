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
using NUnit.Framework;
using OsmSharp.Math;
using OsmSharp.Math.Primitives;
using OsmSharp.Math.Algorithms;

namespace OsmSharp.Test.Math.Algorithms
{
    /// <summary>
    /// Contains simple simplification tests.
    /// </summary>
	[TestFixture]
	public class SimplifyCurveTests
	{
		/// <summary>
		/// Does a few simple simplify curve tests.
		/// </summary>
		[Test]
		public void SimpleSimplifyPointF2DCurveTests ()
		{
			double epsilon = 0.1;

			// simple 2-point line should remain identical.
			PointF2D[] testCurve = new PointF2D[] {
				new PointF2D(0, 0),
				new PointF2D(1, 1)
			};
			PointF2D[] simpleTestCurve = SimplifyCurve.Simplify (testCurve, epsilon);
			Assert.AreEqual (0, simpleTestCurve [0] [0]);
			Assert.AreEqual (0, simpleTestCurve [0] [1]);
			Assert.AreEqual (1, simpleTestCurve [1] [0]);
			Assert.AreEqual (1, simpleTestCurve [1] [1]);

            // simple 1-point line should remain identical.
            testCurve = new PointF2D[] {
				new PointF2D(0, 0),
				new PointF2D(1, 1)
			};
            //PointF2D[] simpleTextCurve = SimplifyCurve.SimplifyBetween(testCurve, epsilon, 0, 0);

			// simple straight line should be simplified to only 2 points.
			testCurve = new PointF2D[] {
				new PointF2D(0, 0),
				new PointF2D(0.5, 0.5),
				new PointF2D(1, 1)
			};
			simpleTestCurve = SimplifyCurve.Simplify (testCurve, epsilon);
			Assert.AreEqual (0, simpleTestCurve [0] [0]);
			Assert.AreEqual (0, simpleTestCurve [0] [1]);
			Assert.AreEqual (1, simpleTestCurve [1] [0]);
			Assert.AreEqual (1, simpleTestCurve [1] [1]);

			// this line should retain all points.
			testCurve = new PointF2D[] {
				new PointF2D(0, 0),
				new PointF2D(1, 1),
				new PointF2D(0, 1)
			};
			simpleTestCurve = SimplifyCurve.Simplify (testCurve, epsilon);
			Assert.AreEqual (0, simpleTestCurve [0] [0]);
			Assert.AreEqual (0, simpleTestCurve [0] [1]);
			Assert.AreEqual (1, simpleTestCurve [1] [0]);
			Assert.AreEqual (1, simpleTestCurve [1] [1]);
			Assert.AreEqual (0, simpleTestCurve [2] [0]);
			Assert.AreEqual (1, simpleTestCurve [2] [1]);

			// this line should be simplified.
			testCurve = new PointF2D[] {
				new PointF2D(0, 0),
				new PointF2D(0.2, 0.21),
				new PointF2D(0.52, 0.5),
				new PointF2D(0.75, 0.76),
				new PointF2D(1, 1)
			};
			simpleTestCurve = SimplifyCurve.Simplify (testCurve, epsilon);
			Assert.AreEqual (0, simpleTestCurve [0] [0]);
			Assert.AreEqual (0, simpleTestCurve [0] [1]);
			Assert.AreEqual (1, simpleTestCurve [1] [0]);
			Assert.AreEqual (1, simpleTestCurve [1] [1]);
		}


        /// <summary>
        /// Does a few simple simplify curve tests.
        /// </summary>
        [Test]
        public void SimpleSimplifyCurveTests()
        {
            double epsilon = 0.1;

            // simple 2-point line should remain identical.
            var testCurve = new double[][] {
				new double[]{ 0, 1 },
				new double[]{ 0, 1 }
			};
            double[][] simpleTestCurve = SimplifyCurve.Simplify(testCurve, epsilon);
            Assert.AreEqual(0, simpleTestCurve[0][0]);
            Assert.AreEqual(0, simpleTestCurve[1][0]);
            Assert.AreEqual(1, simpleTestCurve[0][1]);
            Assert.AreEqual(1, simpleTestCurve[1][1]);

            // between with identical start-end should return one point.
            testCurve = new double[][] {
				new double[]{ 0, 1 },
				new double[]{ 0, 1 }
			};
            simpleTestCurve = SimplifyCurve.SimplifyBetween(testCurve, epsilon, 0,0);
            Assert.AreEqual(0, simpleTestCurve[0][0]);
            Assert.AreEqual(0, simpleTestCurve[1][0]);

            // simple straight line should be simplified to only 2 points.
            testCurve = new double[][] {
				new double[]{ 0, 0.5, 1 },
				new double[]{ 0, 0.5, 1 }
			};
            simpleTestCurve = SimplifyCurve.Simplify(testCurve, epsilon);
            Assert.AreEqual(0, simpleTestCurve[0][0]);
            Assert.AreEqual(0, simpleTestCurve[1][0]);
            Assert.AreEqual(1, simpleTestCurve[0][1]);
            Assert.AreEqual(1, simpleTestCurve[1][1]);

            // this line should retain all points.
            testCurve = new double[][] {
				new double[]{ 0, 1, 0 },
				new double[]{ 0, 1, 1 }
			};
            simpleTestCurve = SimplifyCurve.Simplify(testCurve, epsilon);
            Assert.AreEqual(0, simpleTestCurve[0][0]);
            Assert.AreEqual(0, simpleTestCurve[1][0]);
            Assert.AreEqual(1, simpleTestCurve[0][1]);
            Assert.AreEqual(1, simpleTestCurve[1][1]);
            Assert.AreEqual(0, simpleTestCurve[0][2]);
            Assert.AreEqual(1, simpleTestCurve[1][2]);


            // this line should be simplified.
            testCurve = new double[][] {
				new double[]{ 0, 0.2, 0.52, 0.75, 1 },
				new double[]{ 0, 0.21, 0.5, 0.76, 1 }
			};
            simpleTestCurve = SimplifyCurve.Simplify(testCurve, epsilon);
            Assert.AreEqual(0, simpleTestCurve[0][0]);
            Assert.AreEqual(0, simpleTestCurve[1][0]);
            Assert.AreEqual(1, simpleTestCurve[0][1]);
            Assert.AreEqual(1, simpleTestCurve[1][1]);

            // this 'area' should not be simplified.
            testCurve = new double[][] {
				new double[]{ 0, 1, 1, 0, 0},
				new double[]{ 0, 0, 1, 1, 0}
			};
            simpleTestCurve = SimplifyCurve.Simplify(testCurve, epsilon);
            Assert.AreEqual(0, simpleTestCurve[0][0]);
            Assert.AreEqual(0, simpleTestCurve[1][0]);
            Assert.AreEqual(1, simpleTestCurve[0][1]);
            Assert.AreEqual(0, simpleTestCurve[1][1]);
            Assert.AreEqual(1, simpleTestCurve[0][2]);
            Assert.AreEqual(1, simpleTestCurve[1][2]);
            Assert.AreEqual(0, simpleTestCurve[0][3]);
            Assert.AreEqual(1, simpleTestCurve[1][3]);
            Assert.AreEqual(0, simpleTestCurve[0][4]);
            Assert.AreEqual(0, simpleTestCurve[1][4]);
        }
	}
}