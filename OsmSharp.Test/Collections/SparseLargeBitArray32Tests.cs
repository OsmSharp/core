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
using OsmSharp.Collections;

namespace OsmSharp.Test.Collections
{
    /// <summary>
    /// Contains tests for a sparse large bit array.
    /// </summary>
    [TestFixture]
    public class SparseLargeBitArray32Tests
    {
        /// <summary>
        /// Test for large bit array.
        /// </summary>
        [Test]
        public void TestSparseLargeBitArray32()
        {
            var size = 32 * 1000;
            var referenceArray = new bool[size];
            var array = new SparseLargeBitArray32(size, 256);

            // test size.
            Assert.AreEqual(referenceArray.Length, array.Length);

            // add random data.
            for (int idx = 0; idx < 1000; idx++)
            {
                referenceArray[idx] = true;
                array[idx] = true;
                Assert.IsTrue(array[idx]);
            }

            // compare both.
            for (int idx = 0; idx < referenceArray.Length; idx++)
            {
                Assert.AreEqual(referenceArray[idx], array[idx]);
            }

            // remove random data.
            for (int idx = 0; idx < 1000; idx++)
            {
                referenceArray[idx] = false;
                array[idx] = false;
            }

            // compare both.
            for (int idx = 0; idx < referenceArray.Length; idx++)
            {
                Assert.AreEqual(referenceArray[idx], array[idx]);
            }
        }
    }
}
