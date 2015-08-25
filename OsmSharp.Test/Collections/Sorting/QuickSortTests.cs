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

namespace OsmSharp.Test.Unittests.Collections.Sorting
{
    /// <summary>
    /// Tests the quicksort implementation.
    /// </summary>
    [TestFixture]
    public class QuickSortTests
    {
        /// <summary>
        /// Tests sorting a simple array.
        /// </summary>
        [Test]
        public void TestSimpleList()
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
    }
}