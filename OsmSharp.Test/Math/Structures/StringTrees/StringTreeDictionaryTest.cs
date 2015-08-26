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

using System.Collections.Generic;
using NUnit.Framework;
using OsmSharp.Math.Structures.StringTrees;

namespace OsmSharp.Test
{
    /// <summary>
    /// Does some string dictionary tests.
    /// </summary>
    [TestFixture]
    public class StringTreeDictionaryTest
    {
        /// <summary>
        /// Tests adding a single string to a string dictionary.
        /// </summary>
        [Test]
        public void TestStringTreeDictionaryAddSingle()
        {
            StringTreeDictionary<string> dictionary = new StringTreeDictionary<string>();

            dictionary.Add("test1", "test1");

            Assert.AreEqual("test1", dictionary.SearchExact("test1"));
        }

        /// <summary>
        /// Tests adding two strings to the string dictionary.
        /// </summary>
        [Test]
        public void TestStringTreeDictionaryAddMultiple()
        {
            StringTreeDictionary<string> dictionary = new StringTreeDictionary<string>();

            dictionary.Add("test1", "test1");
            dictionary.Add("test2", "test2");

            Assert.AreEqual("test1", dictionary.SearchExact("test1"));
            Assert.AreEqual("test2", dictionary.SearchExact("test2"));
        }

        /// <summary>
        /// Tests adding random strings to the string dictionary.
        /// </summary>
        [Test]
        public void TestStringTreeDictionaryRandom()
        {
            StringTreeDictionary<string> dictionary = new StringTreeDictionary<string>();

            HashSet<string> test_strings = new HashSet<string>();
            for (int i = 0; i < 1000; i++)
            {
                test_strings.Add(Utilities.RandomString(10));
            }

            foreach (string test_string in test_strings)
            {
                dictionary.Add(test_string, test_string);
            }

            foreach (string test_string in test_strings)
            {
                Assert.AreEqual(test_string, dictionary.SearchExact(test_string));
            }
        }

        /// <summary>
        /// Tests adding alot for random strings to the string dictionary.
        /// </summary>
        [Test]
        public void TestStringTreeDictionaryRandomBig()
        {
            StringTreeDictionary<string> dictionary = new StringTreeDictionary<string>();

            HashSet<string> test_strings = new HashSet<string>();
            for (int i = 0; i < 100000; i++)
            {
                test_strings.Add(Utilities.RandomString(100));
            }

            foreach (string test_string in test_strings)
            {
                dictionary.Add(test_string, test_string);
            }

            foreach (string test_string in test_strings)
            {
                Assert.AreEqual(test_string, dictionary.SearchExact(test_string));
            }
        }
    }
}
