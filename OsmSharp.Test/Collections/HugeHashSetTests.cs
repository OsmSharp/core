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

using OsmSharp.Collections;
using NUnit.Framework;
using System.Collections.Generic;

namespace OsmSharp.Test.Collections
{
    /// <summary>
    /// Testclass with facilities to test the huge dictionary.
    /// </summary>
    [TestFixture]
    public class HugeHashSetTest
    {
        /// <summary>
        /// Tests a huge dictionary.
        /// </summary>
        [Test]
        public void TestHugeHashSet()
        {
            var referenceSet = new HashSet<string>();
            var set = new HugeHashSet<string>();

            for (long idx = 0; idx < 10000; idx++)
            {
                referenceSet.Add(idx.ToString());
                set.Add(idx.ToString());
            }

            for (long idx = 0; idx < 10000; idx++)
            {
                Assert.IsTrue(set.Contains(idx.ToString()));
            }

            foreach(string refValue in referenceSet)
            {
                Assert.IsTrue(set.Contains(refValue));
            }

            foreach(string value in set)
            {
                Assert.IsTrue(referenceSet.Contains(value));
            }
        }
    }
}