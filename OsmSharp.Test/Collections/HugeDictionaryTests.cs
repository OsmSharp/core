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
    public class HugeDictionaryTest
    {
        /// <summary>
        /// Tests a huge dictionary.
        /// </summary>
        [Test]
        public void TestHugeDictionary()
        {
            // create the huge dictionary.
            var hugeDictionary = new HugeDictionary<long, long>();

            for (long idx = 0; idx < 10000; idx++)
            {
                hugeDictionary.Add(idx, idx);
            }

            Assert.AreEqual(10000, hugeDictionary.Count);
            Assert.AreEqual(1, hugeDictionary.CountDictionaries);

            for (long idx = 0; idx < 10000; idx++)
            {
                hugeDictionary.Remove(idx);
            }

            Assert.AreEqual(0, hugeDictionary.Count);
            Assert.AreEqual(1, hugeDictionary.CountDictionaries);

            hugeDictionary = new HugeDictionary<long, long>(1000);

            for (long idx = 0; idx < 10000; idx++)
            {
                hugeDictionary.Add(idx, idx);
            }

            Assert.AreEqual(10000, hugeDictionary.Count);
            Assert.AreEqual(10, hugeDictionary.CountDictionaries);

            for (long idx = 0; idx < 10000; idx++)
            {
                hugeDictionary.Remove(idx);
            }

            Assert.AreEqual(0, hugeDictionary.Count);
            Assert.AreEqual(1, hugeDictionary.CountDictionaries);
        }
        /// <summary>
        /// Tests a huge dictionary.
        /// </summary>
        [Test]
        public void TestHugeDictionaryEnumeration()
        {
            // create the huge dictionary.
            var hugeDictionary = new HugeDictionary<long, long>(100);

            for (long idx = 0; idx < 1000; idx++)
            {
                hugeDictionary.Add(idx, idx);
            }

            var items = new List<KeyValuePair<long, long>>(
                hugeDictionary);
            Assert.AreEqual(hugeDictionary.Count, items.Count);
        }
    }
}