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
using OsmSharp.Collections.Cache;

namespace OsmSharp.Test.Collections.Cache
{
    /// <summary>
    /// Contains tests for the LRU in-memory cache implementation.
    /// </summary>
    [TestFixture]
    public class LRUCacheTests
    {
        /// <summary>
        /// Simple LRU cache tests.
        /// </summary>
        [Test]
        public void LRUCacheTest()
        {
            // create the LRU cache.
            LRUCache<int, int> cache = new LRUCache<int, int>(5);

            // add some stuff.
            cache.Add(0, 1);
            cache.Add(1, 1);
            cache.Add(2, 1);
            cache.Add(3, 1);
            cache.Add(4, 1);
            cache.Add(5, 1);
            cache.Add(6, 1);

            int value;
            Assert.IsTrue(cache.TryPeek(6, out value));
            Assert.AreEqual(1, value);
            Assert.IsTrue(cache.TryPeek(5, out value));
            Assert.AreEqual(1, value);
            Assert.IsTrue(cache.TryPeek(4, out value));
            Assert.AreEqual(1, value);
            Assert.IsTrue(cache.TryPeek(3, out value));
            Assert.AreEqual(1, value);
            Assert.IsTrue(cache.TryPeek(2, out value));
            Assert.AreEqual(1, value);

            Assert.IsFalse(cache.TryPeek(1, out value)); // not in cache anymore.
            Assert.IsFalse(cache.TryPeek(0, out value)); // not in cache anymore.

            // 'use' 2 and 3.
            Assert.IsTrue(cache.TryGet(3, out value));
            Assert.AreEqual(1, value);
            Assert.IsTrue(cache.TryGet(2, out value));
            Assert.AreEqual(1, value);

            // add 7 and 8.
            cache.Add(7, 1);
            cache.Add(8, 1);

            // cache should now contain 2, 3, 7, 8, 6
            Assert.IsTrue(cache.TryPeek(2, out value));
            Assert.AreEqual(1, value);
            Assert.IsTrue(cache.TryPeek(3, out value));
            Assert.AreEqual(1, value);
            Assert.IsTrue(cache.TryPeek(7, out value));
            Assert.AreEqual(1, value);
            Assert.IsTrue(cache.TryPeek(8, out value));
            Assert.AreEqual(1, value);
            Assert.IsTrue(cache.TryPeek(6, out value));
            Assert.AreEqual(1, value);

            Assert.IsFalse(cache.TryPeek(4, out value)); // not in cache anymore.
            Assert.IsFalse(cache.TryPeek(5, out value)); // not in cache anymore.
        }
    }
}
