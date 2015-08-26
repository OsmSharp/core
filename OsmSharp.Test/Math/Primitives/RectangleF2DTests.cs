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
using OsmSharp.Math.Primitives;
using OsmSharp.Math;

namespace OsmSharp.Test.Math.Primitives
{
	/// <summary>
	/// Contains test for the RectangleF2D primitive.
	/// </summary>
	[TestFixture]
	public class RectangleF2DTests
	{
		/// <summary>
		/// Tests a RectangleF2D without a direction.
		/// </summary>
		[Test]
		public void TestRectangleF2DNoDirection()
		{
			RectangleF2D rectangle = new RectangleF2D (0, 0, 1, 1);
            Assert.AreEqual(0, rectangle.BottomLeft[0]);
            Assert.AreEqual(0, rectangle.BottomLeft[1]);
            Assert.AreEqual(1, rectangle.BottomRight[0]);
            Assert.AreEqual(0, rectangle.BottomRight[1]);
            Assert.AreEqual(0, rectangle.TopLeft[0]);
            Assert.AreEqual(1, rectangle.TopLeft[1]);
            Assert.AreEqual(1, rectangle.TopRight[0]);
            Assert.AreEqual(1, rectangle.TopRight[1]);
			Assert.AreEqual (0, rectangle.Angle.Value);

            BoxF2D box = rectangle.BoundingBox;
            Assert.AreEqual(0, box.Min[0]);
            Assert.AreEqual(0, box.Min[1]);
            Assert.AreEqual(1, box.Max[0]);
            Assert.AreEqual(1, box.Max[1]);

            Assert.IsTrue(rectangle.Contains(0.25, 0.75));
            Assert.IsFalse(rectangle.Contains(1.2, 0.25));
            Assert.IsFalse(rectangle.Contains(0.25, 1.2));

            Assert.IsTrue(rectangle.Contains(new PointF2D(0.25, 0.75)));
            Assert.IsFalse(rectangle.Contains(new PointF2D(1.2, 0.25)));
            Assert.IsFalse(rectangle.Contains(new PointF2D(0.25, 1.2)));

            Assert.AreEqual(1, rectangle.Distance(new PointF2D(2, 0)));
            Assert.AreEqual(0, rectangle.Distance(new PointF2D(1, 0)));
            Assert.AreEqual(1, rectangle.Distance(new PointF2D(0, 2)));
            Assert.AreEqual(0, rectangle.Distance(new PointF2D(0, 1)));

            Assert.AreEqual(1, rectangle.Distance(new PointF2D(-1, 0.5)));
            Assert.AreEqual(0, rectangle.Distance(new PointF2D(0, 0.5)));

            Assert.AreEqual(1, rectangle.Height);
            Assert.AreEqual(1, rectangle.Width);

			double[] converted = rectangle.TransformFrom (100, 100, false, false,
			                                             new double[] { 25, 75 });
			Assert.AreEqual (2, converted.Length);
			Assert.AreEqual (0.25, converted [0]);
			Assert.AreEqual (0.75, converted [1]);
			double[] convertedBack = rectangle.TransformTo (100, 100, false, false,
			                                               converted);
			Assert.AreEqual (2, convertedBack.Length);
			Assert.AreEqual (25, convertedBack [0]);
			Assert.AreEqual (75, convertedBack [1]);

			converted = rectangle.TransformFrom (100, 100, false, true,
			                                     new double[] { 25, 75 });
			Assert.AreEqual (2, converted.Length);
			Assert.AreEqual (0.25, converted [0]);
			Assert.AreEqual (0.25, converted [1]);
			convertedBack = rectangle.TransformTo (100, 100, false, false,
			                                                converted);
			Assert.AreEqual (2, convertedBack.Length);
			Assert.AreEqual (25, convertedBack [0]);
			Assert.AreEqual (25, convertedBack [1]);

			converted = rectangle.TransformFrom (100, 100, true, false,
			                                     new double[] { 25, 75 });
			Assert.AreEqual (2, converted.Length);
			Assert.AreEqual (0.75, converted [0]);
			Assert.AreEqual (0.75, converted [1]);
			convertedBack = rectangle.TransformTo (100, 100, false, false,
			                                                converted);
			Assert.AreEqual (2, convertedBack.Length);
			Assert.AreEqual (75, convertedBack [0]);
			Assert.AreEqual (75, convertedBack [1]);

			converted = rectangle.TransformFrom (100, 100, true, true,
			                                     new double[] { 25, 75 });
			Assert.AreEqual (2, converted.Length);
			Assert.AreEqual (0.75, converted [0]);
			Assert.AreEqual (0.25, converted [1]);
			convertedBack = rectangle.TransformTo (100, 100, false, false,
			                                                converted);
			Assert.AreEqual (2, convertedBack.Length);
			Assert.AreEqual (75, convertedBack [0]);
			Assert.AreEqual (25, convertedBack [1]);
		}

		/// <summary>
		/// Tests a RectangleF2D without a direction.
		/// </summary>
		[Test]
		public void TestRectangleF2DDirection()
		{
			double delta = 0.00001;
			RectangleF2D rectangle = new RectangleF2D (1, 1, System.Math.Sqrt (2) * 2,
			                                           System.Math.Sqrt (2) * 2, 45);
			Assert.AreEqual (45, rectangle.Angle.Value, delta);

			double[] converted = rectangle.TransformFrom (100, 100, false, false,
			                                              new double[] { 25, 75 });
			Assert.AreEqual (2, converted.Length);
			Assert.AreEqual (3, converted [0], delta);
			Assert.AreEqual (2, converted [1], delta);
			double[] convertedBack = rectangle.TransformTo (100, 100, false, false,
			                                                converted);
			Assert.AreEqual (2, convertedBack.Length);
			Assert.AreEqual (25, convertedBack [0], delta);
			Assert.AreEqual (75, convertedBack [1], delta);

			converted = rectangle.TransformFrom (100, 100, true, false,
			                                              new double[] { 25, 75 });
			Assert.AreEqual (2, converted.Length);
			Assert.AreEqual (4, converted [0], delta);
			Assert.AreEqual (1, converted [1], delta);
			convertedBack = rectangle.TransformTo (100, 100, true, false,
			                                                converted);
			Assert.AreEqual (2, convertedBack.Length);
			Assert.AreEqual (25, convertedBack [0], delta);
			Assert.AreEqual (75, convertedBack [1], delta);

			converted = rectangle.TransformFrom (100, 100, false, true,
			                                     new double[] { 25, 75 });
			Assert.AreEqual (2, converted.Length);
			Assert.AreEqual (2, converted [0], delta);
			Assert.AreEqual (1, converted [1], delta);
			convertedBack = rectangle.TransformTo (100, 100, false, true,
			                                       converted);
			Assert.AreEqual (2, convertedBack.Length);
			Assert.AreEqual (25, convertedBack [0], delta);
			Assert.AreEqual (75, convertedBack [1], delta);

			converted = rectangle.TransformFrom (100, 100, true, true,
			                                     new double[] { 25, 75 });
			Assert.AreEqual (2, converted.Length);
			Assert.AreEqual (3, converted [0], delta);
			Assert.AreEqual (0, converted [1], delta);
			convertedBack = rectangle.TransformTo (100, 100, true, true,
			                                       converted);
			Assert.AreEqual (2, convertedBack.Length);
			Assert.AreEqual (25, convertedBack [0], delta);
			Assert.AreEqual (75, convertedBack [1], delta);

            rectangle = new RectangleF2D(new PointF2D(1, 1), System.Math.Sqrt(2) * 2,
                                                       System.Math.Sqrt(2) * 2, new VectorF2D(1,1));

            converted = rectangle.TransformFrom(100, 100, false, false,
                                                          new double[] { 25, 75 });
            Assert.AreEqual(2, converted.Length);
            Assert.AreEqual(3, converted[0], delta);
            Assert.AreEqual(2, converted[1], delta);
            convertedBack = rectangle.TransformTo(100, 100, false, false,
                                                            converted);
            Assert.AreEqual(2, convertedBack.Length);
            Assert.AreEqual(25, convertedBack[0], delta);
            Assert.AreEqual(75, convertedBack[1], delta);

            converted = rectangle.TransformFrom(100, 100, true, false,
                                                          new double[] { 25, 75 });
            Assert.AreEqual(2, converted.Length);
            Assert.AreEqual(4, converted[0], delta);
            Assert.AreEqual(1, converted[1], delta);
            convertedBack = rectangle.TransformTo(100, 100, true, false,
                                                            converted);
            Assert.AreEqual(2, convertedBack.Length);
            Assert.AreEqual(25, convertedBack[0], delta);
            Assert.AreEqual(75, convertedBack[1], delta);

            converted = rectangle.TransformFrom(100, 100, false, true,
                                                 new double[] { 25, 75 });
            Assert.AreEqual(2, converted.Length);
            Assert.AreEqual(2, converted[0], delta);
            Assert.AreEqual(1, converted[1], delta);
            convertedBack = rectangle.TransformTo(100, 100, false, true,
                                                   converted);
            Assert.AreEqual(2, convertedBack.Length);
            Assert.AreEqual(25, convertedBack[0], delta);
            Assert.AreEqual(75, convertedBack[1], delta);

            converted = rectangle.TransformFrom(100, 100, true, true,
                                                 new double[] { 25, 75 });
            Assert.AreEqual(2, converted.Length);
            Assert.AreEqual(3, converted[0], delta);
            Assert.AreEqual(0, converted[1], delta);
            convertedBack = rectangle.TransformTo(100, 100, true, true,
                                                   converted);
            Assert.AreEqual(2, convertedBack.Length);
            Assert.AreEqual(25, convertedBack[0], delta);
            Assert.AreEqual(75, convertedBack[1], delta);
		}

		/// <summary>
		/// Tests transformations with coordinates outside of the given coordinate box.
		/// </summary>
		[Test]
		public void TestRectangleF2DOutsideTransforms(){
			RectangleF2D rectangle = new RectangleF2D (0, 0, 1, 1);

			double[] converted = rectangle.TransformFrom (100, 100, false, false,
			                                              new double[] { -100, -100 });
			Assert.AreEqual (2, converted.Length);
			Assert.AreEqual (-1, converted [0]);
			Assert.AreEqual (-1, converted [1]);
			double[] convertedBack = rectangle.TransformTo (100, 100, false, false,
			                                                converted);
			Assert.AreEqual (2, convertedBack.Length);
			Assert.AreEqual (-100, convertedBack [0]);
			Assert.AreEqual (-100, convertedBack [1]);
		}

        /// <summary>
        /// Tests the rectangle overlap function.
        /// </summary>
        [Test]
        public void TestRectangleF2DOverlaps()
        {
            double delta = 0.00001;
            RectangleF2D rectangle = new RectangleF2D(1, 1, System.Math.Sqrt(2) * 2,
                                                       System.Math.Sqrt(2) * 2, 45);

            double[] converted = rectangle.TransformFrom(100, 100, false, false,
                                                          new double[] { 25, 75 });
            Assert.AreEqual(2, converted.Length);
            Assert.AreEqual(3, converted[0], delta);
            Assert.AreEqual(2, converted[1], delta);
            double[] convertedBack = rectangle.TransformTo(100, 100, false, false,
                                                            converted);
            Assert.AreEqual(2, convertedBack.Length);
            Assert.AreEqual(25, convertedBack[0], delta);
            Assert.AreEqual(75, convertedBack[1], delta);

            Assert.IsFalse(rectangle.Overlaps(new BoxF2D(5, 3, 6, 4)));
            Assert.IsTrue(rectangle.Overlaps(new BoxF2D(3.5, 1.5, 4.5, 2.5)));
            Assert.IsTrue(rectangle.Overlaps(new BoxF2D(2, 0.5, 3, 1.5)));
            Assert.IsTrue(rectangle.Overlaps(new BoxF2D(0, -2, 4, 6)));
            Assert.IsTrue(rectangle.Overlaps(new BoxF2D(4, -2, 6, 4)));
            Assert.IsTrue(rectangle.Overlaps(new BoxF2D(1.5, -2, 2.5, 4)));
        }

        /// <summary>
        /// Tests the create from bounds and center.
        /// </summary>
        [Test]
        public void TestRectangleF2DCreateFromBoundsAndCenter()
        {
            double delta = 0.00001;
            // this should create the exact same rectangle as in the other tests.
            RectangleF2D rectangle = RectangleF2D.FromBoundsAndCenter(System.Math.Sqrt(2) * 2,
                                                       System.Math.Sqrt(2) * 2, 3, 1, 45);

            RectangleF2D rectangleReference = new RectangleF2D(1, 1, System.Math.Sqrt(2) * 2,
                                           System.Math.Sqrt(2) * 2, 45);

            Assert.AreEqual(rectangleReference.Height, rectangle.Height);
            Assert.AreEqual(rectangleReference.Width, rectangle.Width);
            Assert.AreEqual(rectangleReference.BottomLeft[0], rectangle.BottomLeft[0], delta);
            Assert.AreEqual(rectangleReference.BottomLeft[1], rectangle.BottomLeft[1], delta);
            Assert.AreEqual(rectangleReference.TopLeft[0], rectangle.TopLeft[0], delta);
            Assert.AreEqual(rectangleReference.TopLeft[1], rectangle.TopLeft[1], delta);
            Assert.AreEqual(rectangleReference.TopRight[0], rectangle.TopRight[0], delta);
            Assert.AreEqual(rectangleReference.TopRight[1], rectangle.TopRight[1], delta);
            Assert.AreEqual(rectangleReference.BottomRight[0], rectangle.BottomRight[0], delta);
            Assert.AreEqual(rectangleReference.BottomRight[1], rectangle.BottomRight[1], delta);
        }

        /// <summary>
        /// Tests the outerbox.
        /// </summary>
        [Test]
        public void TestRectangleF2DOuterBox()
        {
            double delta = 0.00001;
            // this should create the exact same rectangle as in the other tests.
            RectangleF2D rectangle = RectangleF2D.FromBoundsAndCenter(System.Math.Sqrt(2) * 2,
                                                       System.Math.Sqrt(2) * 2, 3, 1, 45);

            // get the box and tests it's bounds.
            BoxF2D box = rectangle.BoundingBox;
            Assert.AreEqual(1, box.Min[0], delta);
            Assert.AreEqual(-1, box.Min[1], delta);
            Assert.AreEqual(5, box.Max[0], delta);
            Assert.AreEqual(3, box.Max[1], delta);
        }

		/// <summary>
		/// Tests a rotation around a given point of a rectangle.
		/// </summary>
		[Test]
		public void TestRectangleF2DRotationAroundPoint() {
			double delta = 0.00001;

			RectangleF2D rectangle = new RectangleF2D (2, 2, 1, 1);
			RectangleF2D rotatedRectangle = rectangle.RotateAround (90, new PointF2D (2, 2));

			Assert.AreEqual (2, rotatedRectangle.BottomLeft [0], delta);
			Assert.AreEqual (2, rotatedRectangle.BottomLeft [1], delta);
			Assert.AreEqual (2, rotatedRectangle.BottomRight [0], delta);
			Assert.AreEqual (1, rotatedRectangle.BottomRight [1], delta);
			Assert.AreEqual (3, rotatedRectangle.TopLeft [0], delta);
			Assert.AreEqual (2, rotatedRectangle.TopLeft [1], delta);
			Assert.AreEqual (3, rotatedRectangle.TopRight [0], delta);
			Assert.AreEqual (1, rotatedRectangle.TopRight [1], delta);
		}

		/// <summary>
		/// Tests a the fit around a given set of points.
		/// </summary>
        [Test]
        public void TestRectangleF2DFit()
        {
            double delta = 0.00001;
            RectangleF2D rectangle = new RectangleF2D(0, 0, 1, 1);

            PointF2D[] points = new PointF2D[] {
                new PointF2D(2, 2),
                new PointF2D(1, 1)
            };

            RectangleF2D fitted = rectangle.Fit(points, 0);
            Assert.AreEqual(1, fitted.Width, delta);
            Assert.AreEqual(1, fitted.Height, delta);
            Assert.AreEqual(1, fitted.BottomLeft[0], delta);
            Assert.AreEqual(1, fitted.BottomLeft[1], delta);
            Assert.AreEqual(2, fitted.TopRight[0], delta);
            Assert.AreEqual(2, fitted.TopRight[1], delta);

            // this should create the exact same rectangle as in the other tests.
            rectangle = RectangleF2D.FromBoundsAndCenter(System.Math.Sqrt(2) * 2,
                                                         System.Math.Sqrt(2) * 2, 0, 0, 45);
            fitted = rectangle.Fit(points, 0);
            Assert.AreEqual(0, fitted.Width, delta);
            Assert.AreEqual(System.Math.Sqrt(2), fitted.Height, delta);
            Assert.AreEqual(1, fitted.BottomLeft[0], delta);
            Assert.AreEqual(1, fitted.BottomLeft[1], delta);
            Assert.AreEqual(2, fitted.TopRight[0], delta);
            Assert.AreEqual(2, fitted.TopRight[1], delta);
        }

        /// <summary>
        /// Tests a the fit around a given set of points.
        /// </summary>
        [Test]
        public void TestRectangleF2DFitAndKeepAspectRatio()
        {
            double delta = 0.00001;
            RectangleF2D rectangle = new RectangleF2D(0, 0, 1, 1);

            PointF2D[] points = new PointF2D[] {
                new PointF2D(2, 2),
                new PointF2D(1, 1)
            };

            RectangleF2D fitted = rectangle.FitAndKeepAspectRatio(points, 0);
            Assert.AreEqual(1, fitted.Width, delta);
            Assert.AreEqual(1, fitted.Height, delta);
            Assert.AreEqual(1, fitted.BottomLeft[0], delta);
            Assert.AreEqual(1, fitted.BottomLeft[1], delta);
            Assert.AreEqual(2, fitted.TopRight[0], delta);
            Assert.AreEqual(2, fitted.TopRight[1], delta);

            // this should create the exact same rectangle as in the other tests.
            rectangle = RectangleF2D.FromBoundsAndCenter(System.Math.Sqrt(2) * 2,
                                                         System.Math.Sqrt(2) * 2, 0, 0, 45);
            fitted = rectangle.FitAndKeepAspectRatio(points, 0);
            Assert.AreEqual(System.Math.Sqrt(2), fitted.Width, delta);
            Assert.AreEqual(System.Math.Sqrt(2), fitted.Height, delta);
            Assert.AreEqual(0.5, fitted.BottomLeft[0], delta);
            Assert.AreEqual(1.5, fitted.BottomLeft[1], delta);
            Assert.AreEqual(2.5, fitted.TopRight[0], delta);
            Assert.AreEqual(1.5, fitted.TopRight[1], delta);
        }
	}
}

