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

        /// <summary>
        /// Tests the key collection.
        /// </summary>
        [Test]
        public void TestKeyAndValueCollections()
        {
            var dic = new HugeDictionary<int, string>(5);
            dic.Add(1, "1");
            dic.Add(2, "2");
            dic.Add(3, "3");
            dic.Add(4, "4");
            dic.Add(5, "5");
            dic.Add(6, "6");
            dic.Add(7, "7");

            var keys = dic.Keys;
            Assert.IsNotNull(keys);
            Assert.IsTrue(keys.IsReadOnly);
            Assert.AreEqual(7, keys.Count);
            Assert.IsTrue(keys.Contains(1));
            Assert.IsTrue(keys.Contains(2));
            Assert.IsTrue(keys.Contains(3));
            Assert.IsTrue(keys.Contains(4));
            Assert.IsTrue(keys.Contains(5));
            Assert.IsTrue(keys.Contains(6));
            Assert.IsTrue(keys.Contains(7));

            var values = dic.Values;
            Assert.IsNotNull(values);
            Assert.IsTrue(values.IsReadOnly);
            Assert.AreEqual(7, values.Count);
            Assert.IsTrue(values.Contains("1"));
            Assert.IsTrue(values.Contains("2"));
            Assert.IsTrue(values.Contains("3"));
            Assert.IsTrue(values.Contains("4"));
            Assert.IsTrue(values.Contains("5"));
            Assert.IsTrue(values.Contains("6"));
            Assert.IsTrue(values.Contains("7"));
        }
    }
}