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
using OsmSharp.Collections.MemoryMapped;
using OsmSharp.IO.MemoryMappedFiles;
using OsmSharp.Math.Random;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Test.Unittests.Collections.MemoryMapped
{
    /// <summary>
    /// Contains tests for the memory-mapped huge dictionary.
    /// </summary>
    [TestFixture]
    public class MemoryMappedHugeDictionaryTests
    {
        /// <summary>
        /// Test small.
        /// </summary>
        [Test]
        public void TestSmall()
        {
            var randomGenerator = new RandomGenerator(66707770); // make this deterministic 

            var buffer = new byte[255];
            var readFrom = new MemoryMappedFile.ReadFromDelegate<string>((stream, position) =>
            {
                stream.Seek(position, System.IO.SeekOrigin.Begin);
                var size = stream.ReadByte();
                int pos = 0;
                stream.Read(buffer, pos, size);
                while (size == 255)
                {
                    pos = pos + size;
                    size = stream.ReadByte();
                    if (buffer.Length < size + pos)
                    {
                        Array.Resize(ref buffer, size + pos);
                    }
                    stream.Read(buffer, pos, size);
                }
                pos = pos + size;
                return System.Text.Encoding.Unicode.GetString(buffer, 0, pos);
            });
            var writeTo = new MemoryMappedFile.WriteToDelegate<string>((stream, position, structure) =>
            {
                stream.Seek(position, System.IO.SeekOrigin.Begin);
                var bytes = System.Text.Encoding.Unicode.GetBytes(structure);
                var length = bytes.Length;
                for (int idx = 0; idx <= bytes.Length; idx = idx + 255)
                {
                    var size = bytes.Length - idx;
                    if (size > 255)
                    {
                        size = 255;
                    }

                    if (stream.Length <= stream.Position + size + 1)
                    { // past end of stream.
                        return -1;
                    }
                    stream.WriteByte((byte)size);
                    stream.Write(bytes, idx, size);
                    length++;
                }
                return length;
            });

            var dictionary = new MemoryMappedHugeDictionary<string, string>(new MemoryMappedStream(new MemoryStream()),
                readFrom, writeTo, readFrom, writeTo);
            var reference = new Dictionary<string, string>();

            var keys = new string[] { "key1", "key2", "key3", "key4", "key5", "key6", "key7", "key8" };
            var testCount = 10;
            while(testCount > 0)
            {
                var keyIdx = randomGenerator.Generate(keys.Length);
                var value = randomGenerator.GenerateString(
                    randomGenerator.Generate(256) + 32);
                dictionary[keys[keyIdx]] = value;
                reference[keys[keyIdx]] = value;
                testCount--;
            }

            foreach(var pair in reference)
            {
                var value = dictionary[pair.Key];
                Assert.AreEqual(pair.Value, value);

                Assert.IsTrue(dictionary.TryGetValue(pair.Key, out value));
                Assert.AreEqual(pair.Value, value);
            }
        }

        /// <summary>
        /// Test huge.
        /// </summary>
        [Test]
        public void TestHuge()
        {
            var randomGenerator = new RandomGenerator(66707770); // make this deterministic 

            var buffer = new byte[255];
            var readFrom = new MemoryMappedFile.ReadFromDelegate<string>((stream, position) =>
            {
                stream.Seek(position, System.IO.SeekOrigin.Begin);
                var size = stream.ReadByte();
                int pos = 0;
                stream.Read(buffer, pos, size);
                while (size == 255)
                {
                    pos = pos + size;
                    size = stream.ReadByte();
                    if (buffer.Length < size + pos)
                    {
                        Array.Resize(ref buffer, size + pos);
                    }
                    stream.Read(buffer, pos, size);
                }
                pos = pos + size;
                return System.Text.Encoding.Unicode.GetString(buffer, 0, pos);
            });
            var writeTo = new MemoryMappedFile.WriteToDelegate<string>((stream, position, structure) =>
            {
                stream.Seek(position, System.IO.SeekOrigin.Begin);
                var bytes = System.Text.Encoding.Unicode.GetBytes(structure);
                var length = bytes.Length;
                for (int idx = 0; idx <= bytes.Length; idx = idx + 255)
                {
                    var size = bytes.Length - idx;
                    if (size > 255)
                    {
                        size = 255;
                    }

                    if (stream.Length <= stream.Position + size + 1)
                    { // past end of stream.
                        return -1;
                    }
                    stream.WriteByte((byte)size);
                    stream.Write(bytes, idx, size);
                    length++;
                }
                return length;
            });

            var dictionary = new MemoryMappedHugeDictionary<string, string>(new MemoryMappedStream(new MemoryStream()),
                readFrom, writeTo, readFrom, writeTo);
            var reference = new Dictionary<string, string>();

            var keys = new List<string>();
            for (int idx = 0; idx < 100; idx++)
            {
                keys.Add(string.Format("key{0}", idx));
            }
            var testCount = 1000;
            while (testCount > 0)
            {
                var keyIdx = randomGenerator.Generate(keys.Count);
                var value = randomGenerator.GenerateString(
                    randomGenerator.Generate(256) + 32);
                dictionary[keys[keyIdx]] = value;
                reference[keys[keyIdx]] = value;
                testCount--;
            }

            foreach (var pair in reference)
            {
                var value = dictionary[pair.Key];
                Assert.AreEqual(pair.Value, value);

                Assert.IsTrue(dictionary.TryGetValue(pair.Key, out value));
                Assert.AreEqual(pair.Value, value);
            }
        }

        /// <summary>
        /// Tests the hash and comparer.
        /// </summary>
        [Test]
        public void TestHashAndComparer()
        {
            var dictionary = new MemoryMappedHugeDictionary<string, string>(new MemoryMappedStream(new MemoryStream()),
                MemoryMappedDelegates.ReadFromString, MemoryMappedDelegates.WriteToString,
                MemoryMappedDelegates.ReadFromString, MemoryMappedDelegates.WriteToString,
                (x) =>
                {
                    return x[0].GetHashCode();
                },
                (x, y) =>
                {
                    return x[0].CompareTo(y[0]);
                });

            // add string but only the first char counts in equals and hash.
            dictionary.Add("ben", "ben");
            Assert.Catch<ArgumentException>(() => {
                dictionary.Add("ban", "ban");
            });
            dictionary.Add("kan", "kan");

            Assert.AreEqual("ben", dictionary["ben"]);
            Assert.AreEqual("kan", dictionary["kan"]);

            dictionary["ban"] = "ban";

            Assert.AreEqual("ban", dictionary["ban"]);
            Assert.AreEqual("ban", dictionary["ben"]);
            Assert.AreEqual("ban", dictionary["b"]);
            Assert.AreEqual("kan", dictionary["kan"]);
        }
    }
}
