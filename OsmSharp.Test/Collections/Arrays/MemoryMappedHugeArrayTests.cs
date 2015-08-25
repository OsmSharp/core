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
using OsmSharp.Collections.Arrays;
using OsmSharp.Collections.Arrays.MemoryMapped;
using OsmSharp.IO.MemoryMappedFiles;
using OsmSharp.Math.Random;
using System;
using System.IO;

namespace OsmSharp.Test.Unittests.Collections.Arrays
{  
    /// <summary>
    /// Contains tests for the huge array.
    /// </summary>
    [TestFixture]
    public class MemoryMappedMemoryMappedHugeArrayTests
    {
        /// <summary>
        /// A simple test for the huge array.
        /// </summary>
        [Test]
        public void MemoryMappedHugeArrayArgumentTest()
        {
            using (var intArray = new MemoryMappedHugeArrayUInt32(new MemoryMappedStream(new MemoryStream()), 1000, 1024))
            {
                Assert.AreEqual(1000, intArray.Length);
                Assert.Catch<ArgumentOutOfRangeException>(() =>
                {
                    intArray[1001] = 10;
                });
                Assert.Catch<ArgumentOutOfRangeException>(() =>
                {
                    intArray[-1] = 10;
                });

                uint value;
                Assert.Catch<ArgumentOutOfRangeException>(() =>
                {
                    value = intArray[1001];
                });
                Assert.Catch<ArgumentOutOfRangeException>(() =>
                {
                    value = intArray[-1];
                });
            }
        }

        /// <summary>
        /// A simple test for the huge array.
        /// </summary>
        [Test]
        public void MemoryMappedHugeArrayTest()
        {
            using (var intArray = new MemoryMappedHugeArrayUInt32(new MemoryMappedStream(new MemoryStream()), 1000, 1024))
            {
                var intArrayRef = new uint[1000];

                var randomGenerator = new RandomGenerator(66707770); // make this deterministic 
                for (uint idx = 0; idx < 1000; idx++)
                {
                    if (randomGenerator.Generate(2.0) > 1)
                    { // add data.
                        intArrayRef[idx] = idx;
                        intArray[idx] = idx;
                    }
                    else
                    {
                        intArrayRef[idx] = int.MaxValue;
                        intArray[idx] = int.MaxValue;
                    }
                    Assert.AreEqual(intArrayRef[idx], intArray[idx]);
                }

                for (int idx = 0; idx < 1000; idx++)
                {
                    Assert.AreEqual(intArrayRef[idx], intArray[idx]);
                }
            }
        }

        /// <summary>
        /// A simple test resizing the huge array 
        /// </summary>
        [Test]
        public void MemoryMappedHugeArrayResizeTests()
        {
            var randomGenerator = new RandomGenerator(66707770); // make this deterministic 

            using (var intArray = new MemoryMappedHugeArrayUInt32(new MemoryMappedStream(new MemoryStream()), 1000, 256))
            {
                var intArrayRef = new uint[1000];

                for (uint idx = 0; idx < 1000; idx++)
                {
                    if (randomGenerator.Generate(2.0) > 1)
                    { // add data.
                        intArrayRef[idx] = idx;
                        intArray[idx] = idx;
                    }
                    else
                    {
                        intArrayRef[idx] = int.MaxValue;
                        intArray[idx] = int.MaxValue;
                    }

                    Assert.AreEqual(intArrayRef[idx], intArray[idx]);
                }

                Array.Resize<uint>(ref intArrayRef, 335);
                intArray.Resize(335);

                Assert.AreEqual(intArrayRef.Length, intArray.Length);
                for (int idx = 0; idx < intArrayRef.Length; idx++)
                {
                    Assert.AreEqual(intArrayRef[idx], intArray[idx]);
                }
            }

            using (var intArray = new MemoryMappedHugeArrayUInt32(new MemoryMappedStream(new MemoryStream()), 1000, 256))
            {
                var intArrayRef = new uint[1000];

                for (uint idx = 0; idx < 1000; idx++)
                {
                    if (randomGenerator.Generate(2.0) > 1)
                    { // add data.
                        intArrayRef[idx] = idx;
                        intArray[idx] = idx;
                    }
                    else
                    {
                        intArrayRef[idx] = int.MaxValue;
                        intArray[idx] = int.MaxValue;
                    }

                    Assert.AreEqual(intArrayRef[idx], intArray[idx]);
                }

                Array.Resize<uint>(ref intArrayRef, 1235);
                var oldSize = intArray.Length;
                intArray.Resize(1235);
                // huge array is not initialized.
                for (long idx = oldSize; idx < intArray.Length;idx++)
                {
                    intArray[idx] = 0;
                }

                Assert.AreEqual(intArrayRef.Length, intArray.Length);
                for (int idx = 0; idx < intArrayRef.Length; idx++)
                {
                    Assert.AreEqual(intArrayRef[idx], intArray[idx], string.Format("Array element not equal at index: {0}. Expected {1}, found {2}",
                        idx, intArray[idx], intArrayRef[idx]));
                }
            }
        }
    }
}