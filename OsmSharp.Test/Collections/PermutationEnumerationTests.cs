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
using OsmSharp.Collections;

namespace OsmSharp.Test.Collections
{
    /// <summary>
    /// Contains tests for the Permutation Enumeration class.
    /// </summary>
    [TestFixture]
    public class PermutationEnumerationTests
    {
        /// <summary>
        /// Tests the permutation counts.
        /// </summary>
        [Test]
        public void TestPermutationCount()
        {
            int[] test_sequence = new int[] { 1, 2 };
            PermutationEnumerable<int> enumerator =
                new PermutationEnumerable<int>(test_sequence);
            List<int[]> set = new List<int[]>(enumerator);
            Assert.AreEqual(2, set.Count);

            test_sequence = new int[] { 1, 2, 3 };
            enumerator =
                new PermutationEnumerable<int>(test_sequence);
            set = new List<int[]>(enumerator);
            Assert.AreEqual(6, set.Count);

            test_sequence = new int[] { 1, 2, 3, 4 };
            enumerator =
                new PermutationEnumerable<int>(test_sequence);
            set = new List<int[]>(enumerator);
            Assert.AreEqual(24, set.Count);

            test_sequence = new int[] { 1, 2, 3, 4, 5 };
            enumerator =
                new PermutationEnumerable<int>(test_sequence);
            set = new List<int[]>(enumerator);
            Assert.AreEqual(120, set.Count);
        }


        /// <summary>
        /// Tests the permutation contents.
        /// </summary>
        [Test]
        public void TestPermutationContent()
        {
            int[] test_sequence = new int[] { 1, 2 };
            PermutationEnumerable<int> enumerator =
                new PermutationEnumerable<int>(test_sequence);
            List<int[]> set = new List<int[]>(enumerator);
            Assert.AreEqual(2, set.Count);
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 1, 2 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 2, 1 }));

            test_sequence = new int[] { 1, 2, 3 };
            enumerator =
                new PermutationEnumerable<int>(test_sequence);
            set = new List<int[]>(enumerator);
            Assert.AreEqual(6, set.Count);
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 1, 2, 3 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 1, 3, 2 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 3, 1, 2 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 3, 2, 1 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 2, 3, 1 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 2, 1, 3 }));

            // 4 items tests all the crucial elements of the algorithm. (full code coverage)
            test_sequence = new int[] { 1, 2, 3, 4 };
            enumerator =
                new PermutationEnumerable<int>(test_sequence);
            set = new List<int[]>(enumerator);
            Assert.AreEqual(24, set.Count);
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 1, 2, 3, 4 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 1, 3, 2, 4 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 3, 1, 2, 4 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 3, 2, 1, 4 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 2, 3, 1, 4 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 2, 1, 3, 4 }));

            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 1, 2, 4, 3 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 1, 3, 4, 2 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 3, 1, 4, 2 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 3, 2, 4, 1 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 2, 3, 4, 1 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 2, 1, 4, 3 }));

            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 1, 4, 2, 3 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 1, 4, 3, 2 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 3, 4, 1, 2 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 3, 4, 2, 1 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 2, 4, 3, 1 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 2, 4, 1, 3 }));

            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 4, 1, 2, 3 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 4, 1, 3, 2 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 4, 3, 1, 2 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 4, 3, 2, 1 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 4, 2, 3, 1 }));
            Assert.IsTrue(this.TestPermutationContent(set, new int[] { 4, 2, 1, 3 }));
        }

        private bool TestPermutationContent(List<int[]> permuations, int[] permutation)
        {
            foreach (int[] current in permuations)
            {
                bool equal = true;
                for (int idx = 0; idx < current.Length; idx++)
                {
                    if (current[idx] != permutation[idx])
                    {
                        equal = false;
                        break;
                    }
                }
                if (equal)
                {
                    return true;
                }
            }
            return false;
        }
    }
}