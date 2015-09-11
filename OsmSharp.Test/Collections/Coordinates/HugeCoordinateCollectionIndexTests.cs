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
using OsmSharp.Collections.Coordinates.Collections;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Simple;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace OsmSharp.Test.Collections.Coordinates
{
    /// <summary>
    /// Testclass with facilities to test the huge dictionary.
    /// </summary>
    [TestFixture]
    public class HugeCoordinateCollectionIndexTests
    {
        /// <summary>
        /// Tests adding a collection.
        /// </summary>
        [Test]
        public void TestAdd()
        {
            var coordinates = new HugeCoordinateCollectionIndex(10);
            coordinates.Add(0, new CoordinateArrayCollection<GeoCoordinate>(
                new GeoCoordinate[] { 
                    new GeoCoordinate(0, 1),
                    new GeoCoordinate(2, 3) }));
            Assert.Catch<InvalidOperationException>(() =>
            {
                coordinates.Add(0, null);
            });

            ICoordinateCollection actual;
            Assert.IsTrue(coordinates.TryGet(0, out actual));
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(0, actual.ElementAt(0).Latitude);
            Assert.AreEqual(1, actual.ElementAt(0).Longitude);
            Assert.AreEqual(2, actual.ElementAt(1).Latitude);
            Assert.AreEqual(3, actual.ElementAt(1).Longitude);

            coordinates = new HugeCoordinateCollectionIndex(10);
            coordinates.Add(0, new CoordinateArrayCollection<GeoCoordinate>(
                new GeoCoordinate[] { 
                    new GeoCoordinate(0, 1),
                    new GeoCoordinate(2, 3) }));
            Assert.Catch<InvalidOperationException>(() =>
            {
                coordinates.Add(0, null);
            });
            coordinates.Add(1, new CoordinateArrayCollection<GeoCoordinate>(
                new GeoCoordinate[] { 
                    new GeoCoordinate(4, 5),
                    new GeoCoordinate(6, 7),
                    new GeoCoordinate(8, 9) }));
            Assert.Catch<InvalidOperationException>(() =>
            {
                coordinates.Add(1, null);
            });

            Assert.IsTrue(coordinates.TryGet(0, out actual));
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(0, actual.ElementAt(0).Latitude);
            Assert.AreEqual(1, actual.ElementAt(0).Longitude);
            Assert.AreEqual(2, actual.ElementAt(1).Latitude);
            Assert.AreEqual(3, actual.ElementAt(1).Longitude);

            Assert.IsTrue(coordinates.TryGet(1, out actual));
            Assert.AreEqual(3, actual.Count);
            Assert.AreEqual(4, actual.ElementAt(0).Latitude);
            Assert.AreEqual(5, actual.ElementAt(0).Longitude);
            Assert.AreEqual(6, actual.ElementAt(1).Latitude);
            Assert.AreEqual(7, actual.ElementAt(1).Longitude);
            Assert.AreEqual(8, actual.ElementAt(2).Latitude);
            Assert.AreEqual(9, actual.ElementAt(2).Longitude);
        }

        /// <summary>
        /// Tests a adding a collection of zero-size of a connection that is null.
        /// </summary>
        [Test]
        public void TestNullVersusEmpty()
        {
            var coordinates = new HugeCoordinateCollectionIndex(10);
            coordinates.Add(0, new CoordinateArrayCollection<GeoCoordinate>(
                new GeoCoordinate[] { }));
            coordinates.Add(1, null);

            ICoordinateCollection actual;
            Assert.IsTrue(coordinates.TryGet(0, out actual));
            Assert.AreEqual(0, actual.Count);

            Assert.IsTrue(coordinates.TryGet(1, out actual));
            Assert.AreEqual(null, actual);

            Assert.IsFalse(coordinates.TryGet(2, out actual));
        }

        /// <summary>
        /// Tests removing an element.
        /// </summary>
        [Test]
        public void TestRemove()
        {
            var coordinates = new HugeCoordinateCollectionIndex(10);
            coordinates.Add(0, new CoordinateArrayCollection<GeoCoordinate>(
                new GeoCoordinate[] { }));

            coordinates.Remove(0);

            ICoordinateCollection actual;
            Assert.IsFalse(coordinates.TryGet(0, out actual));
            Assert.Catch<KeyNotFoundException>(() =>
            {
                actual = coordinates[0];
            });

            coordinates = new HugeCoordinateCollectionIndex(10);
            coordinates.Add(0, null);

            coordinates.Remove(0);

            Assert.IsFalse(coordinates.TryGet(0, out actual));
            Assert.Catch<KeyNotFoundException>(() =>
            {
                actual = coordinates[0];
            });
        }

        /// <summary>
        /// Tests the index by filling it along with a dictionary and comparing the results.
        /// </summary>
        [Test]
        public void TestCompareWithDictionary()
        {
            OsmSharp.Math.Random.StaticRandomGenerator.Set(116542346);

            var box = new GeoCoordinateBox(
                new GeoCoordinate(90, 180),
                new GeoCoordinate(-90, -180));
            var size = 100;
            var maxCollectionSize = 4;
            var referenceDictionary = new Dictionary<long, ICoordinateCollection>();
            var coordinates = new HugeCoordinateCollectionIndex(400);
            for(int id = 0; id < size; id++)
            {
                var currentSize = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(maxCollectionSize) + 1;
                var coordinatesArray = new GeoCoordinate[currentSize];
                while(currentSize > 0)
                {
                    coordinatesArray[currentSize - 1] =  box.GenerateRandomIn(OsmSharp.Math.Random.StaticRandomGenerator.Get());
                    currentSize--;
                }
                var coordinatesCollection = new CoordinateArrayCollection<GeoCoordinate>(coordinatesArray);
                referenceDictionary[id] = coordinatesCollection;
                coordinates[id] = coordinatesCollection;
            }

            // check result.
            for (int idx = 0; idx < size; idx++)
            {
                var referenceCollection = referenceDictionary[idx];
                var collection = coordinates[idx];

                referenceCollection.Reset();
                collection.Reset();

                while(referenceCollection.MoveNext())
                {
                    Assert.IsTrue(collection.MoveNext());
                    Assert.AreEqual(referenceCollection.Latitude, collection.Latitude);
                    Assert.AreEqual(referenceCollection.Longitude, collection.Longitude);
                }
                Assert.IsFalse(collection.MoveNext());
            }

            // generate new randoms.
            for (int idx = 0; idx < size; idx++)
            {
                var currentSize = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(maxCollectionSize) + 1;
                var coordinatesArray = new GeoCoordinate[currentSize];
                while (currentSize > 0)
                {
                    coordinatesArray[currentSize - 1] = box.GenerateRandomIn(OsmSharp.Math.Random.StaticRandomGenerator.Get());
                    currentSize--;
                }
                var coordinatesCollection = new CoordinateArrayCollection<GeoCoordinate>(coordinatesArray);
                referenceDictionary[idx] = coordinatesCollection;
                coordinates[idx] = coordinatesCollection;

                var referenceCollection = referenceDictionary[idx];
                var collection = coordinates[idx];

                referenceCollection.Reset();
                collection.Reset();

                while (referenceCollection.MoveNext())
                {
                    Assert.IsTrue(collection.MoveNext());
                    Assert.AreEqual(referenceCollection.Latitude, collection.Latitude);
                    Assert.AreEqual(referenceCollection.Longitude, collection.Longitude);
                }
                Assert.IsFalse(collection.MoveNext());
            }

            // check again.
            for (int idx = 0; idx < size; idx++)
            {
                var referenceCollection = referenceDictionary[idx];
                var collection = coordinates[idx];

                referenceCollection.Reset();
                collection.Reset();

                while (referenceCollection.MoveNext())
                {
                    Assert.IsTrue(collection.MoveNext());
                    Assert.AreEqual(referenceCollection.Latitude, collection.Latitude);
                    Assert.AreEqual(referenceCollection.Longitude, collection.Longitude);
                }
                Assert.IsFalse(collection.MoveNext());
            }

            // randomly remove stuff.
            for (int idx = 0; idx < size; idx++)
            {
                if(OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(2) > 1)
                {
                    referenceDictionary[idx] = null;
                    coordinates[idx] = null;
                }
            }

            // check again.
            for (int idx = 0; idx < size; idx++)
            {
                var referenceCollection = referenceDictionary[idx];
                var collection = coordinates[idx];

                if (referenceCollection == null)
                {
                    Assert.IsNull(collection);
                }
                else
                {
                    referenceCollection.Reset();
                    collection.Reset();

                    while (referenceCollection.MoveNext())
                    {
                        Assert.IsTrue(collection.MoveNext());
                        Assert.AreEqual(referenceCollection.Latitude, collection.Latitude);
                        Assert.AreEqual(referenceCollection.Longitude, collection.Longitude);
                    }
                    Assert.IsFalse(collection.MoveNext());
                }
            }

            // generate new randoms.
            for (int idx = 0; idx < size; idx++)
            {
                var currentSize = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(maxCollectionSize) + 1;
                var coordinatesArray = new GeoCoordinate[currentSize];
                while (currentSize > 0)
                {
                    coordinatesArray[currentSize - 1] = box.GenerateRandomIn(OsmSharp.Math.Random.StaticRandomGenerator.Get());
                    currentSize--;
                }
                var coordinatesCollection = new CoordinateArrayCollection<GeoCoordinate>(coordinatesArray);
                referenceDictionary[idx] = coordinatesCollection;
                coordinates[idx] = coordinatesCollection;

                var referenceCollection = referenceDictionary[idx];
                var collection = coordinates[idx];

                referenceCollection.Reset();
                collection.Reset();

                while (referenceCollection.MoveNext())
                {
                    Assert.IsTrue(collection.MoveNext());
                    Assert.AreEqual(referenceCollection.Latitude, collection.Latitude);
                    Assert.AreEqual(referenceCollection.Longitude, collection.Longitude);
                }
                Assert.IsFalse(collection.MoveNext());
            }

            // check again.
            for (int idx = 0; idx < size; idx++)
            {
                var referenceCollection = referenceDictionary[idx];
                var collection = coordinates[idx];

                referenceCollection.Reset();
                collection.Reset();

                while (referenceCollection.MoveNext())
                {
                    Assert.IsTrue(collection.MoveNext());
                    Assert.AreEqual(referenceCollection.Latitude, collection.Latitude);
                    Assert.AreEqual(referenceCollection.Longitude, collection.Longitude);
                }
                Assert.IsFalse(collection.MoveNext());
            }
        }

        /// <summary>
        /// Tests trim functionality.
        /// </summary>
        [Test]
        public void TestTrim()
        {
            OsmSharp.Math.Random.StaticRandomGenerator.Set(116542346);

            var box = new GeoCoordinateBox(
                new GeoCoordinate(90, 180),
                new GeoCoordinate(-90, -180));
            var size = 100;
            var maxCollectionSize = 4;
            var referenceDictionary = new Dictionary<long, ICoordinateCollection>();
            var coordinates = new HugeCoordinateCollectionIndex(400);
            for (int idx = 0; idx < size; idx++)
            {
                var currentSize = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(maxCollectionSize) + 1;
                var coordinatesArray = new GeoCoordinate[currentSize];
                while (currentSize > 0)
                {
                    coordinatesArray[currentSize - 1] = box.GenerateRandomIn(OsmSharp.Math.Random.StaticRandomGenerator.Get());
                    currentSize--;
                }
                var coordinatesCollection = new CoordinateArrayCollection<GeoCoordinate>(coordinatesArray);
                referenceDictionary[idx] = coordinatesCollection;
                coordinates[idx] = coordinatesCollection;
            }

            // execute the trim.
            coordinates.Trim();

            // check result.
            for (int idx = 0; idx < size; idx++)
            {
                var referenceCollection = referenceDictionary[idx];
                var collection = coordinates[idx];

                referenceCollection.Reset();
                collection.Reset();

                while (referenceCollection.MoveNext())
                {
                    Assert.IsTrue(collection.MoveNext());
                    Assert.AreEqual(referenceCollection.Latitude, collection.Latitude);
                    Assert.AreEqual(referenceCollection.Longitude, collection.Longitude);
                }
                Assert.IsFalse(collection.MoveNext());
            }
        }

        /// <summary>
        /// Tests compress functionality.
        /// </summary>
        [Test]
        public void TestCompress()
        {
            // build a coordinate collection.
            var coordinates = new HugeCoordinateCollectionIndex(100);
            coordinates.Add(0, new CoordinateArrayCollection<GeoCoordinate>(
                new GeoCoordinate[] { 
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(1, 1) }));
            coordinates.Add(1, new CoordinateArrayCollection<GeoCoordinate>(
                new GeoCoordinate[] { 
                    new GeoCoordinate(2, 2),
                    new GeoCoordinate(3, 3) }));

            // compress.
            coordinates.Compress();

            // check if everything is still there.
            Assert.AreEqual(8, coordinates.LengthCoordinates);
            Assert.AreEqual(2, coordinates.LengthIndex);
            Assert.IsTrue(coordinates[0].ElementAt(0).Latitude == 0);
            Assert.IsTrue(coordinates[0].ElementAt(0).Longitude == 0);
            Assert.IsTrue(coordinates[0].ElementAt(1).Latitude == 1);
            Assert.IsTrue(coordinates[0].ElementAt(1).Longitude == 1);
            Assert.IsTrue(coordinates[1].ElementAt(0).Latitude == 2);
            Assert.IsTrue(coordinates[1].ElementAt(0).Longitude == 2);
            Assert.IsTrue(coordinates[1].ElementAt(1).Latitude == 3);
            Assert.IsTrue(coordinates[1].ElementAt(1).Longitude == 3);

            // build a coordinate collection and remove some data.
            coordinates = new HugeCoordinateCollectionIndex(100);
            coordinates.Add(0, new CoordinateArrayCollection<GeoCoordinate>(
                new GeoCoordinate[] { 
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(1, 1) }));
            coordinates.Add(1, new CoordinateArrayCollection<GeoCoordinate>(
                new GeoCoordinate[] { 
                    new GeoCoordinate(2, 2),
                    new GeoCoordinate(3, 3) }));
            coordinates.Remove(0);

            // compress.
            coordinates.Compress();

            // check if everything is still there.
            Assert.AreEqual(4, coordinates.LengthCoordinates);
            Assert.AreEqual(2, coordinates.LengthIndex);
            ICoordinateCollection actual;
            Assert.IsFalse(coordinates.TryGet(0, out actual));
            Assert.IsTrue(coordinates[1].ElementAt(0).Latitude == 2);
            Assert.IsTrue(coordinates[1].ElementAt(0).Longitude == 2);
            Assert.IsTrue(coordinates[1].ElementAt(1).Latitude == 3);
            Assert.IsTrue(coordinates[1].ElementAt(1).Longitude == 3);
        }

        /// <summary>
        /// Tests the index by filling it along with a dictionary and comparing the results.
        /// </summary>
        [Test]
        public void TestCompressAndCompareWithDictionary()
        {
            OsmSharp.Math.Random.StaticRandomGenerator.Set(116542346);

            var box = new GeoCoordinateBox(
                new GeoCoordinate(90, 180),
                new GeoCoordinate(-90, -180));
            var size = 100;
            var maxCollectionSize = 4;
            var referenceDictionary = new Dictionary<long, ICoordinateCollection>();
            var coordinates = new HugeCoordinateCollectionIndex(400);
            for (int id = 0; id < size; id++)
            {
                var currentSize = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(maxCollectionSize) + 1;
                var coordinatesArray = new GeoCoordinate[currentSize];
                while (currentSize > 0)
                {
                    coordinatesArray[currentSize - 1] = box.GenerateRandomIn(OsmSharp.Math.Random.StaticRandomGenerator.Get());
                    currentSize--;
                }
                var coordinatesCollection = new CoordinateArrayCollection<GeoCoordinate>(coordinatesArray);
                referenceDictionary[id] = coordinatesCollection;
                coordinates[id] = coordinatesCollection;
            }

            coordinates.Compress();

            // check result.
            for (int idx = 0; idx < size; idx++)
            {
                var referenceCollection = referenceDictionary[idx];
                var collection = coordinates[idx];

                referenceCollection.Reset();
                collection.Reset();

                while (referenceCollection.MoveNext())
                {
                    Assert.IsTrue(collection.MoveNext());
                    Assert.AreEqual(referenceCollection.Latitude, collection.Latitude);
                    Assert.AreEqual(referenceCollection.Longitude, collection.Longitude);
                }
                Assert.IsFalse(collection.MoveNext());
            }

            // generate new randoms.
            for (int idx = 0; idx < size; idx++)
            {
                var currentSize = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(maxCollectionSize) + 1;
                var coordinatesArray = new GeoCoordinate[currentSize];
                while (currentSize > 0)
                {
                    coordinatesArray[currentSize - 1] = box.GenerateRandomIn(OsmSharp.Math.Random.StaticRandomGenerator.Get());
                    currentSize--;
                }
                var coordinatesCollection = new CoordinateArrayCollection<GeoCoordinate>(coordinatesArray);
                referenceDictionary[idx] = coordinatesCollection;
                coordinates[idx] = coordinatesCollection;

                var referenceCollection = referenceDictionary[idx];
                var collection = coordinates[idx];

                referenceCollection.Reset();
                collection.Reset();

                while (referenceCollection.MoveNext())
                {
                    Assert.IsTrue(collection.MoveNext());
                    Assert.AreEqual(referenceCollection.Latitude, collection.Latitude);
                    Assert.AreEqual(referenceCollection.Longitude, collection.Longitude);
                }
                Assert.IsFalse(collection.MoveNext());
            }

            coordinates.Compress();

            // check again.
            for (int idx = 0; idx < size; idx++)
            {
                var referenceCollection = referenceDictionary[idx];
                var collection = coordinates[idx];

                referenceCollection.Reset();
                collection.Reset();

                while (referenceCollection.MoveNext())
                {
                    Assert.IsTrue(collection.MoveNext());
                    Assert.AreEqual(referenceCollection.Latitude, collection.Latitude);
                    Assert.AreEqual(referenceCollection.Longitude, collection.Longitude);
                }
                Assert.IsFalse(collection.MoveNext());
            }

            // randomly remove stuff.
            for (int idx = 0; idx < size; idx++)
            {
                if (OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(2) > 1)
                {
                    referenceDictionary[idx] = null;
                    coordinates[idx] = null;
                }
            }

            coordinates.Compress();

            // check again.
            for (int idx = 0; idx < size; idx++)
            {
                var referenceCollection = referenceDictionary[idx];
                var collection = coordinates[idx];

                if (referenceCollection == null)
                {
                    Assert.IsNull(collection);
                }
                else
                {
                    referenceCollection.Reset();
                    collection.Reset();

                    while (referenceCollection.MoveNext())
                    {
                        Assert.IsTrue(collection.MoveNext());
                        Assert.AreEqual(referenceCollection.Latitude, collection.Latitude);
                        Assert.AreEqual(referenceCollection.Longitude, collection.Longitude);
                    }
                    Assert.IsFalse(collection.MoveNext());
                }
            }

            // generate new randoms.
            for (int idx = 0; idx < size; idx++)
            {
                var currentSize = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(maxCollectionSize) + 1;
                var coordinatesArray = new GeoCoordinate[currentSize];
                while (currentSize > 0)
                {
                    coordinatesArray[currentSize - 1] = box.GenerateRandomIn(OsmSharp.Math.Random.StaticRandomGenerator.Get());
                    currentSize--;
                }
                var coordinatesCollection = new CoordinateArrayCollection<GeoCoordinate>(coordinatesArray);
                referenceDictionary[idx] = coordinatesCollection;
                coordinates[idx] = coordinatesCollection;

                var referenceCollection = referenceDictionary[idx];
                var collection = coordinates[idx];

                referenceCollection.Reset();
                collection.Reset();

                while (referenceCollection.MoveNext())
                {
                    Assert.IsTrue(collection.MoveNext());
                    Assert.AreEqual(referenceCollection.Latitude, collection.Latitude);
                    Assert.AreEqual(referenceCollection.Longitude, collection.Longitude);
                }
                Assert.IsFalse(collection.MoveNext());
            }

            coordinates.Compress();

            // check again.
            for (int idx = 0; idx < size; idx++)
            {
                var referenceCollection = referenceDictionary[idx];
                var collection = coordinates[idx];

                referenceCollection.Reset();
                collection.Reset();

                while (referenceCollection.MoveNext())
                {
                    Assert.IsTrue(collection.MoveNext());
                    Assert.AreEqual(referenceCollection.Latitude, collection.Latitude);
                    Assert.AreEqual(referenceCollection.Longitude, collection.Longitude);
                }
                Assert.IsFalse(collection.MoveNext());
            }
        }

        /// <summary>
        /// Tests serialization.
        /// </summary>
        [Test]
        public void TestSerialize()
        {
            // build a coordinate collection.
            var coordinates = new HugeCoordinateCollectionIndex(100);
            coordinates.Add(0, new CoordinateArrayCollection<GeoCoordinate>(
                new GeoCoordinate[] { 
                    new GeoCoordinate(0, 0),
                    new GeoCoordinate(1, 1) }));
            coordinates.Add(1, new CoordinateArrayCollection<GeoCoordinate>(
                new GeoCoordinate[] { 
                    new GeoCoordinate(2, 2),
                    new GeoCoordinate(3, 3) }));

            // serialize and test size.
            byte[] data = null;
            var expected = 2 * 8 + // header, indexsize and coordinates size.
                2 * 8 + // index.
                4 * 8; // 8 coordinates.
            using(var stream = new MemoryStream())
            {
                Assert.AreEqual(expected, coordinates.Serialize(stream));
                data = stream.ToArray();

                Assert.AreEqual(expected, data.Length);
            }

            // build a coordinate collection.
            coordinates = new HugeCoordinateCollectionIndex(100);

            // serialize and test size.
            expected = 2 * 8 + // header, indexsize and coordinates size.
                0 * 8 + // index.
                0 * 8; // 8 coordinates.
            using (var stream = new MemoryStream())
            {
                Assert.AreEqual(expected, coordinates.Serialize(stream));
                data = stream.ToArray();

                Assert.AreEqual(expected, data.Length);
            }

            // build a coordinate collection.
            coordinates = new HugeCoordinateCollectionIndex(100);
            coordinates.Add(0, null);

            // serialize and test size.
            expected = 2 * 8 + // header, indexsize and coordinates size.
                1 * 8 + // index.
                0 * 8; // 8 coordinates.
            using (var stream = new MemoryStream())
            {
                Assert.AreEqual(expected, coordinates.Serialize(stream));
                data = stream.ToArray();

                Assert.AreEqual(expected, data.Length);
            }
        }

        /// <summary>
        /// Tests deserialization.
        /// </summary>
        [Test]
        public void TestDeserialize()
        {
            // build a coordinate collection.
            var coordinates = new HugeCoordinateCollectionIndex(100);
            coordinates.Add(0, new CoordinateArrayCollection<GeoCoordinate>(
                new GeoCoordinate[] { 
                    new GeoCoordinate(0, 1),
                    new GeoCoordinate(2, 3) }));
            coordinates.Add(1, new CoordinateArrayCollection<GeoCoordinate>(
                new GeoCoordinate[] { 
                    new GeoCoordinate(4, 5),
                    new GeoCoordinate(6, 7) }));

            // serialize/deserialize and test size.
            using (var stream = new MemoryStream())
            {
                coordinates.Serialize(stream);

                stream.Seek(0, SeekOrigin.Begin);

                coordinates = HugeCoordinateCollectionIndex.Deserialize(stream, false);

                ICoordinateCollection actual;
                Assert.IsTrue(coordinates.TryGet(0, out actual));
                Assert.AreEqual(2, actual.Count);
                Assert.AreEqual(0, actual.ElementAt(0).Latitude);
                Assert.AreEqual(1, actual.ElementAt(0).Longitude);
                Assert.AreEqual(2, actual.ElementAt(1).Latitude);
                Assert.AreEqual(3, actual.ElementAt(1).Longitude);

                Assert.IsTrue(coordinates.TryGet(1, out actual));
                Assert.AreEqual(2, actual.Count);
                Assert.AreEqual(4, actual.ElementAt(0).Latitude);
                Assert.AreEqual(5, actual.ElementAt(0).Longitude);
                Assert.AreEqual(6, actual.ElementAt(1).Latitude);
                Assert.AreEqual(7, actual.ElementAt(1).Longitude);
            }

            // build a coordinate collection.
            coordinates = new HugeCoordinateCollectionIndex(100);

            // serialize/deserialize and test size.
            using (var stream = new MemoryStream())
            {
                coordinates.Serialize(stream);

                stream.Seek(0, SeekOrigin.Begin);

                coordinates = HugeCoordinateCollectionIndex.Deserialize(stream, false);
                Assert.AreEqual(0, coordinates.LengthIndex);
                Assert.AreEqual(0, coordinates.LengthCoordinates);
            }

            // build a coordinate collection.
            coordinates = new HugeCoordinateCollectionIndex(100);
            coordinates.Add(0, null);

            // serialize/deserialize and test size.
            using (var stream = new MemoryStream())
            {
                coordinates.Serialize(stream);

                stream.Seek(0, SeekOrigin.Begin);

                coordinates = HugeCoordinateCollectionIndex.Deserialize(stream, false);
                Assert.AreEqual(1, coordinates.LengthIndex);
                Assert.AreEqual(0, coordinates.LengthCoordinates);
            }
        }

        /// <summary>
        /// Tests resize.
        /// </summary>
        [Test]
        public void TestResize()
        {
            OsmSharp.Math.Random.StaticRandomGenerator.Set(116542346);

            var box = new GeoCoordinateBox(
                new GeoCoordinate(90, 180),
                new GeoCoordinate(-90, -180));
            var size = 100;
            var maxCollectionSize = 4;
            var referenceDictionary = new Dictionary<long, ICoordinateCollection>();
            var coordinates = new HugeCoordinateCollectionIndex(400);
            for (int idx = 0; idx < size; idx++)
            {
                var currentSize = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(maxCollectionSize) + 1;
                var coordinatesArray = new GeoCoordinate[currentSize];
                while (currentSize > 0)
                {
                    coordinatesArray[currentSize - 1] = box.GenerateRandomIn(OsmSharp.Math.Random.StaticRandomGenerator.Get());
                    currentSize--;
                }
                var coordinatesCollection = new CoordinateArrayCollection<GeoCoordinate>(coordinatesArray);
                referenceDictionary[idx] = coordinatesCollection;
                coordinates[idx] = coordinatesCollection;
            }

            // check result.
            for (int idx = 0; idx < size; idx++)
            {
                var referenceCollection = referenceDictionary[idx];
                var collection = coordinates[idx];

                referenceCollection.Reset();
                collection.Reset();

                while (referenceCollection.MoveNext())
                {
                    Assert.IsTrue(collection.MoveNext());
                    Assert.AreEqual(referenceCollection.Latitude, collection.Latitude);
                    Assert.AreEqual(referenceCollection.Longitude, collection.Longitude);
                }
                Assert.IsFalse(collection.MoveNext());
            }

            // change size and check result.
            var newSize = 75;
            coordinates.Resize(newSize);

            // check result.
            Assert.AreEqual(75, coordinates.LengthIndex);
            for (var idx = 0; idx < newSize; idx++)
            {
                var referenceCollection = referenceDictionary[idx];
                var collection = coordinates[idx];

                referenceCollection.Reset();
                collection.Reset();

                while (referenceCollection.MoveNext())
                {
                    Assert.IsTrue(collection.MoveNext());
                    Assert.AreEqual(referenceCollection.Latitude, collection.Latitude);
                    Assert.AreEqual(referenceCollection.Longitude, collection.Longitude);
                }
                Assert.IsFalse(collection.MoveNext());
            }
        }

        /// <summary>
        /// Tests switching two elements.
        /// </summary>
        [Test]
        public void TestSwitch()
        {
            OsmSharp.Math.Random.StaticRandomGenerator.Set(116542346);

            var box = new GeoCoordinateBox(
                new GeoCoordinate(90, 180),
                new GeoCoordinate(-90, -180));
            var size = 100;
            var maxCollectionSize = 4;
            var referenceDictionary = new Dictionary<long, ICoordinateCollection>();
            var coordinates = new HugeCoordinateCollectionIndex(400);
            for (int idx = 0; idx < size; idx++)
            {
                var currentSize = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(maxCollectionSize) + 1;
                var coordinatesArray = new GeoCoordinate[currentSize];
                while (currentSize > 0)
                {
                    coordinatesArray[currentSize - 1] = box.GenerateRandomIn(OsmSharp.Math.Random.StaticRandomGenerator.Get());
                    currentSize--;
                }
                var coordinatesCollection = new CoordinateArrayCollection<GeoCoordinate>(coordinatesArray);
                referenceDictionary[idx] = coordinatesCollection;
                coordinates[idx] = coordinatesCollection;
            }
            
            // generate a sequence of two id's and switch.
            for(var i = 0; i < 20; i++)
            {
                var id1 = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(size);
                var id2 = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(size - 1);
                if (id1 <= id2) { id2++; }

                var temp = referenceDictionary[id1];
                referenceDictionary[id1] = referenceDictionary[id2];
                referenceDictionary[id2] = temp;

                coordinates.Switch(id1, id2);
            }

            // check result.
            for (int idx = 0; idx < size; idx++)
            {
                var referenceCollection = referenceDictionary[idx];
                var collection = coordinates[idx];

                referenceCollection.Reset();
                collection.Reset();

                while (referenceCollection.MoveNext())
                {
                    Assert.IsTrue(collection.MoveNext());
                    Assert.AreEqual(referenceCollection.Latitude, collection.Latitude);
                    Assert.AreEqual(referenceCollection.Longitude, collection.Longitude);
                }
                Assert.IsFalse(collection.MoveNext());
            }
        }
    }
}