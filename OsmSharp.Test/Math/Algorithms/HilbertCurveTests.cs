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
using OsmSharp.Math.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Test.Math.Algorithms
{
    /// <summary>
    /// Tests hilbert curve calculations.
    /// </summary>
    [TestFixture]
    public class HilbertCurveTests
    {
        /// <summary>
        /// Tests simple distance calculations.
        /// </summary>
        [Test]
        public void TestHilbertDistance2()
        {
            Assert.AreEqual(0, HilbertCurve.HilbertDistance(-45, -90, 2));
            Assert.AreEqual(1, HilbertCurve.HilbertDistance(+45, -90, 2));
            Assert.AreEqual(2, HilbertCurve.HilbertDistance(+45, +90, 2));
            Assert.AreEqual(3, HilbertCurve.HilbertDistance(-45, +90, 2));
        }

        /// <summary>
        /// Tests simple distance calculations.
        /// </summary>
        [Test]
        public void TestHilbertDistance4()
        {
            Assert.AreEqual(0, HilbertCurve.HilbertDistance(-90 + (45 * 0) + 25.5f, -180 + (90 * 0) + 45f, 4));
            Assert.AreEqual(1, HilbertCurve.HilbertDistance(-90 + (45 * 0) + 25.5f, -180 + (90 * 1) + 45f, 4));
            Assert.AreEqual(2, HilbertCurve.HilbertDistance(-90 + (45 * 1) + 25.5f, -180 + (90 * 1) + 45f, 4));
            Assert.AreEqual(3, HilbertCurve.HilbertDistance(-90 + (45 * 1) + 25.5f, -180 + (90 * 0) + 45f, 4));

            Assert.AreEqual(4, HilbertCurve.HilbertDistance(-90 + (45 * 2) + 25.5f, -180 + (90 * 0) + 45f, 4));
            Assert.AreEqual(5, HilbertCurve.HilbertDistance(-90 + (45 * 3) + 25.5f, -180 + (90 * 0) + 45f, 4));
            Assert.AreEqual(6, HilbertCurve.HilbertDistance(-90 + (45 * 3) + 25.5f, -180 + (90 * 1) + 45f, 4));
            Assert.AreEqual(7, HilbertCurve.HilbertDistance(-90 + (45 * 2) + 25.5f, -180 + (90 * 1) + 45f, 4));

            Assert.AreEqual(8, HilbertCurve.HilbertDistance(-90 + (45 * 2) + 25.5f, -180 + (90 * 2) + 45f, 4));
            Assert.AreEqual(9, HilbertCurve.HilbertDistance(-90 + (45 * 3) + 25.5f, -180 + (90 * 2) + 45f, 4));
            Assert.AreEqual(10, HilbertCurve.HilbertDistance(-90 + (45 * 3) + 25.5f, -180 + (90 * 3) + 45f, 4));
            Assert.AreEqual(11, HilbertCurve.HilbertDistance(-90 + (45 * 2) + 25.5f, -180 + (90 * 3) + 45f, 4));

            Assert.AreEqual(12, HilbertCurve.HilbertDistance(-90 + (45 * 1) + 25.5f, -180 + (90 * 3) + 45f, 4));
            Assert.AreEqual(13, HilbertCurve.HilbertDistance(-90 + (45 * 1) + 25.5f, -180 + (90 * 2) + 45f, 4));
            Assert.AreEqual(14, HilbertCurve.HilbertDistance(-90 + (45 * 0) + 25.5f, -180 + (90 * 2) + 45f, 4));
            Assert.AreEqual(15, HilbertCurve.HilbertDistance(-90 + (45 * 0) + 25.5f, -180 + (90 * 3) + 45f, 4));
        }

        /// <summary>
        /// Tests simple curve calculations.
        /// </summary>
        [Test]
        public void TestHilbertDistances1()
        {
            var distances = HilbertCurve.HilbertDistances(-90, -180, 90, 180, 2);
            var expectedDistances = new long[] { 0, 1, 2, 3 };

            Assert.AreEqual(expectedDistances.Length, distances.Count);
            foreach(var expectedDistance in expectedDistances)
            {
                Assert.IsTrue(distances.Remove(expectedDistance));
            }
            Assert.AreEqual(0, distances.Count);

            distances = HilbertCurve.HilbertDistances(-90, -180, 90, 180, 4);
            expectedDistances = new long[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            Assert.AreEqual(expectedDistances.Length, distances.Count);

            foreach (var expectedDistance in expectedDistances)
            {
                Assert.IsTrue(distances.Remove(expectedDistance));
            }
            Assert.AreEqual(0, distances.Count);
        }
    }
}