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
using OsmSharp.Collections.Sorting;

namespace OsmSharp.Test.Collections.Sorting
{
    /// <summary>
    /// Tests the quicksort implementation.
    /// </summary>
    [TestFixture]
    public class QuickSortTests
    {
        /// <summary>
        /// Tests sorting an array.
        /// </summary>
        [Test]
        public void TestSort()
        {
            var array = new int[] { 0, 1, 2, 3, 4 };
            QuickSort.Sort((i) => 
            {
                return array[i];
            }, (i, j) =>
            {
                var temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }, 0, array.Length - 1);

            Assert.AreEqual(0, array[0]);
            Assert.AreEqual(1, array[1]);
            Assert.AreEqual(2, array[2]);
            Assert.AreEqual(3, array[3]);
            Assert.AreEqual(4, array[4]);

            array[1] = 2;
            array[2] = 1;
            QuickSort.Sort((i) =>
            {
                return array[i];
            }, (i, j) =>
            {
                var temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }, 0, array.Length - 1);

            Assert.AreEqual(0, array[0]);
            Assert.AreEqual(1, array[1]);
            Assert.AreEqual(2, array[2]);
            Assert.AreEqual(3, array[3]);
            Assert.AreEqual(4, array[4]);

            array[1] = 3;
            array[2] = 1;
            array[4] = 2;
            array[3] = 4;
            QuickSort.Sort((i) =>
            {
                if (i < 0 || i >= array.Length) { return long.MaxValue; }
                return array[i];
            }, (i, j) =>
            {
                var temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }, 0, array.Length - 1);

            Assert.AreEqual(0, array[0]);
            Assert.AreEqual(1, array[1]);
            Assert.AreEqual(2, array[2]);
            Assert.AreEqual(3, array[3]);
            Assert.AreEqual(4, array[4]);

            array = new int[] { 4, 2, 1, 3, 0 };
            QuickSort.Sort((i) =>
            {
                if (i < 0 || i >= array.Length) { return long.MaxValue; }
                return array[i];
            }, (i, j) =>
            {
                var temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }, 0, array.Length - 1);

            Assert.AreEqual(0, array[0]);
            Assert.AreEqual(1, array[1]);
            Assert.AreEqual(2, array[2]);
            Assert.AreEqual(3, array[3]);
            Assert.AreEqual(4, array[4]);
        }

        /// <summary>
        /// Tests three way partioning.
        /// </summary>
        [Test]
        public void TestThreewayPartition()
        {
            var array = new int[] { 1, 0, 2, 1, 4, 1 };

            long lowestHighest, highestLowest;
            QuickSort.ThreewayPartition((i) => array[i],
                (i, j) => 
                    {
                        var temp = array[i];
                        array[i] = array[j];
                        array[j] = temp;
                    }, 0, array.Length - 1, 0, out highestLowest, out lowestHighest);
            Assert.AreEqual(0, highestLowest);
            Assert.AreEqual(4, lowestHighest);

            array = new int[] { 1, 0, 2, 1, 4, 1 };

            QuickSort.ThreewayPartition((i) => array[i],
                (i, j) =>
                {
                    var temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                }, 0, array.Length - 1, 1, out highestLowest, out lowestHighest);
            Assert.AreEqual(-1, highestLowest);
            Assert.AreEqual(1, lowestHighest);

            array = new int[] { 1, 2, 4, 1, 0, 2, 1, 4, 1, 4, 1, 1, 4 };

            QuickSort.ThreewayPartition((i) => array[i],
                (i, j) =>
                {
                    var temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                }, 3, 8, 3, out highestLowest, out lowestHighest);
            Assert.AreEqual(3, highestLowest);
            Assert.AreEqual(7, lowestHighest);
        }

        /// <summary>
        /// Tests the is sorted function.
        /// </summary>
        [Test]
        public void TestIsSorted()
        {
            var array = new int[] { 0, 1, 2, 3, 4 };
            Assert.IsTrue(QuickSort.IsSorted(x => array[x], 0, array.Length - 1)); 
            array = new int[] { 2, 1, 2, 3, 4 };
            Assert.IsFalse(QuickSort.IsSorted(x => array[x], 0, array.Length - 1));
            array = new int[] { 0, 1, 2, 3, 1 };
            Assert.IsFalse(QuickSort.IsSorted(x => array[x], 0, array.Length - 1));
            array = new int[] { 0, 0, 0, 0, 0 };
            Assert.IsTrue(QuickSort.IsSorted(x => array[x], 0, array.Length - 1));
        }
    }
}