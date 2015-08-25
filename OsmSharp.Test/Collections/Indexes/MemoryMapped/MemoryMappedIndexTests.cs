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
using OsmSharp.Collections.Indexes.MemoryMapped;
using OsmSharp.IO.MemoryMappedFiles;
using OsmSharp.Math.Random;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Test.Unittests.Collections.Indexes.MemoryMapped
{
    /// <summary>
    /// Contains test for a memory-mapped index.
    /// </summary>
    [TestFixture]
    public class MemoryMappedIndexTests
    {
        /// <summary>
        /// Tests using a structure with a variable-sized string.
        /// </summary>
        [Test]
        public void TestString()
        {
            var randomGenerator = new RandomGenerator(66707770); // make this deterministic.

            var index = new MemoryMappedIndex<string>(new MemoryMappedStream(new MemoryStream()), 
                MemoryMappedDelegates.ReadFromString, MemoryMappedDelegates.WriteToString);
            var indexRef = new Dictionary<long, string>();

            // add the data.
            var testCount = 10;
            while(testCount > 0)
            {
                var data = randomGenerator.GenerateString(
                    randomGenerator.Generate(256) + 32);
                indexRef.Add(index.Add(data), data);
                testCount--;
            }

            // get the data and check.
            foreach(var entry in indexRef)
            {
                var data = index.Get(entry.Key);
                Assert.AreEqual(indexRef[entry.Key], data);
            }
        }

        /// <summary>
        /// Tests using a structure with a variable-sized string.
        /// </summary>
        [Test]
        public void TestStringHuge()
        {
            var randomGenerator = new RandomGenerator(66707770); // make this deterministic.

            var index = new MemoryMappedIndex<string>(new MemoryMappedStream(new MemoryStream()),
                MemoryMappedDelegates.ReadFromString, MemoryMappedDelegates.WriteToString, 1024, false);
            var indexRef = new Dictionary<long, string>();

            // add the data.
            var testCount = 1000;
            while (testCount > 0)
            {
                var data = randomGenerator.GenerateString(
                    randomGenerator.Generate(256) + 32);
                indexRef.Add(index.Add(data), data);
                testCount--;
            }

            // get the data and check.
            foreach (var entry in indexRef)
            {
                var data = index.Get(entry.Key);
                Assert.AreEqual(indexRef[entry.Key], data);
            }
        }

        /// <summary>
        /// Tests using an array of int's but also applies to other arrays with fixed-sized elements.
        /// </summary>
        [Test]
        public void TestStructuresOfArrays()
        {
            var randomGenerator = new RandomGenerator(66707770); // make this deterministic.

            var index = new MemoryMappedIndex<int[]>(new MemoryMappedStream(new MemoryStream()),
                MemoryMappedDelegates.ReadFromIntArray, MemoryMappedDelegates.WriteToIntArray);
            var indexRef = new Dictionary<long, int[]>();

            // add the data.
            var testCount = 10;
            while (testCount > 0)
            {
                var data = randomGenerator.GenerateArray(512, 512);
                indexRef.Add(index.Add(data), data);
                testCount--;
            }

            // get the data and check.
            foreach (var entry in indexRef)
            {
                var data = index.Get(entry.Key);
                Assert.AreEqual(indexRef[entry.Key], data);
            }
        }

        /// <summary>
        /// Tests serializing/deserializing an index with strings.
        /// </summary>
        [Test]
        public void TestStringSerialize()
        {
            var randomGenerator = new RandomGenerator(66707770); // make this deterministic.

            var index = new MemoryMappedIndex<string>(new MemoryMappedStream(new MemoryStream()),
                MemoryMappedDelegates.ReadFromString, MemoryMappedDelegates.WriteToString);
            var indexRef = new Dictionary<long, string>();

            // add the data.
            var testCount = 10;
            while (testCount > 0)
            {
                var data = randomGenerator.GenerateString(
                    randomGenerator.Generate(256) + 32);
                indexRef.Add(index.Add(data), data);
                testCount--;
            }

            MemoryMappedIndex<string> deserializedIndex;
            using (var stream = new MemoryStream())
            {
                var size = index.Serialize(stream);
                deserializedIndex = MemoryMappedIndex<string>.Deserialize(stream,
                    MemoryMappedDelegates.ReadFromString, MemoryMappedDelegates.WriteToString, false);

                // get the data and check.
                foreach (var entry in indexRef)
                {
                    var data = index.Get(entry.Key);
                    Assert.AreEqual(deserializedIndex.Get(entry.Key), data);
                }
            }
        }

        /// <summary>
        /// Tests serializing/deserializing an index with arrays of integers.
        /// </summary>
        [Test]
        public void TestStructuresOfArraysSerialize()
        {
            var randomGenerator = new RandomGenerator(66707770); // make this deterministic.

            var index = new MemoryMappedIndex<int[]>(new MemoryMappedStream(new MemoryStream()),
                MemoryMappedDelegates.ReadFromIntArray, MemoryMappedDelegates.WriteToIntArray);
            var indexRef = new Dictionary<long, int[]>();

            // add the data.
            var testCount = 10;
            while (testCount > 0)
            {
                var data = randomGenerator.GenerateArray(512, 512);
                indexRef.Add(index.Add(data), data);
                testCount--;
            }

            MemoryMappedIndex<int[]> deserializedIndex;
            using (var stream = new MemoryStream())
            {
                var size = index.Serialize(stream);
                deserializedIndex = MemoryMappedIndex<int[]>.Deserialize(stream,
                    MemoryMappedDelegates.ReadFromIntArray, MemoryMappedDelegates.WriteToIntArray, false);

                // get the data and check.
                foreach (var entry in indexRef)
                {
                    var data = index.Get(entry.Key);
                    Assert.AreEqual(indexRef[entry.Key], data);
                }
            }
        }
    }
}