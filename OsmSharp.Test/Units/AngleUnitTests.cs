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
using OsmSharp.Units.Angle;

namespace OsmSharp.Test
{
    /// <summary>
    /// Tests for the angle unit classes.
    /// </summary>
    [TestFixture]
    public class AngleUnitTests
    {
        /// <summary>
        /// Tests the angle normalization (in degrees).
        /// </summary>
        [Test]
        public void TestDegreeNormalization()
        {
            const double angle = 30;

            Assert.AreEqual(angle, ((Degree)(angle + 360)).Value);
            Assert.AreEqual(angle, ((Degree)(angle - 360)).Value);
            Assert.AreEqual(angle, ((Degree)(angle + 360 + 360)).Value);
            Assert.AreEqual(angle, ((Degree)(angle - 360 - 360)).Value);
        }

        /// <summary>
        /// Tests the angle normalization (in radians).
        /// </summary>
        [Test]
        public void TestRadianNormalization()
        {
            const double angle = 1.5;

            Assert.AreEqual(angle, ((Radian)(angle + Constants.TwoPi)).Value);
            Assert.AreEqual(angle, ((Radian)(angle - Constants.TwoPi)).Value);
            Assert.AreEqual(angle, ((Radian)(angle + Constants.TwoPi + Constants.TwoPi)).Value);
            Assert.AreEqual(angle, ((Radian)(angle - Constants.TwoPi - Constants.TwoPi)).Value);
        }

        /// <summary>
        /// Tests angle range.
        /// </summary>
        [Test]
        public void TestDegreeRang180()
        {
            Assert.AreEqual(30, ((Degree)390).Range180());
            Assert.AreEqual(30, ((Degree)30).Range180());
            Assert.AreEqual(-90, ((Degree)270).Range180());
        }

        /// <summary>
        /// Tests angle subtraction.
        /// </summary>
        [Test]
        public void TestDegreeSubstract180()
        {
            Degree angle = 30;
            Assert.AreEqual(60, angle.SmallestDifference(330));
            Assert.AreEqual(-60, angle.SmallestDifference(90));

            Assert.AreEqual(-20, ((Degree)350).SmallestDifference(10));
            Assert.AreEqual(20, ((Degree)10).SmallestDifference(350));
            Assert.AreEqual(20, ((Degree)350).SmallestDifference(330));
            Assert.AreEqual(20, ((Degree)40).SmallestDifference(20));

            Assert.AreEqual(-2, ((Degree)179).SmallestDifference((Degree)181));
            Assert.AreEqual(2, ((Degree)181).SmallestDifference((Degree)179));
        }
    }
}

