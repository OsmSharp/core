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
using OsmSharp.Math.Algorithms;
using OsmSharp.Units.Angle;

namespace OsmSharp.Test.Math.Algorithms
{
	/// <summary>
	/// Rotation tests.
	/// </summary>
	[TestFixture]
	public class RotationTests
	{
		/// <summary>
		/// Tests simple rotations.
		/// </summary>
		[Test]
		public void RotationSimpleTest(){
			double delta = 0.00001;

			PointF2D center = new PointF2D (1, 1);
			PointF2D point = new PointF2D (1, 2);

			PointF2D rotated = Rotation.RotateAroundPoint ((Degree)90, center, point);

			Assert.AreEqual (2, rotated [0], delta);
			Assert.AreEqual (1, rotated [1], delta);

			rotated = Rotation.RotateAroundPoint ((Degree)180, center, point);

			Assert.AreEqual (1, rotated [0], delta);
			Assert.AreEqual (0, rotated [1], delta);
		}
	}
}

