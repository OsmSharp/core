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
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Test.Unittests.Collections.Coordinates
{
    /// <summary>
    /// Testclass with facilities to test the huge dictionary.
    /// </summary>
    [TestFixture]
    public class HugeCoordinateCollectionIndexTests
    {
        /// <summary>
        /// Tests the index with a small amount of coordinates.
        /// </summary>
        [Test]
        public void TestSmall()
        {
            OsmSharp.Math.Random.StaticRandomGenerator.Set(116542346);

            var box = new GeoCoordinateBox(
                new GeoCoordinate(90, 180),
                new GeoCoordinate(-90, -180));
            var size = 100;
            var maxCollectionSize = 4;
            var referenceDictionary = new Dictionary<long, ICoordinateCollection>();
            var coordinates = new HugeCoordinateCollectionIndex(400);
            for(int idx = 0; idx < size; idx++)
            {
                var currentSize = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(maxCollectionSize) + 1;
                var coordinatesArray = new GeoCoordinate[currentSize];
                while(currentSize > 0)
                {
                    coordinatesArray[currentSize - 1] =  box.GenerateRandomIn(OsmSharp.Math.Random.StaticRandomGenerator.Get());
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
        /// Tests serializing a huge coordinate collection index.
        /// </summary>
        [Test]
        public void TestSerialize()
        {
            OsmSharp.Math.Random.StaticRandomGenerator.Set(116542346);

            var box = new GeoCoordinateBox(
                new GeoCoordinate(90, 180),
                new GeoCoordinate(-90, -180));
            var size = 5;
            var maxCollectionSize = 4;
            var referenceDictionary = new Dictionary<long, ICoordinateCollection>();
            var coordinates = new HugeCoordinateCollectionIndex(100);
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

            coordinates.Trim();
            coordinates.Compress();

            byte[] data = null;
            using(var stream = new MemoryStream())
            {
                long length = coordinates.Serialize(stream);
                data = stream.ToArray();

                Assert.AreEqual(168, length);
                Assert.AreEqual(data.Length, length);
            }

            var result = HugeCoordinateCollectionIndex.Deserialize(new MemoryStream(data));

            // check result.
            for (int idx = 0; idx < size; idx++)
            {
                var referenceCollection = referenceDictionary[idx];
                var collection = result[idx];

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

            result = HugeCoordinateCollectionIndex.Deserialize(new MemoryStream(data), true);

            // check result.
            for (int idx = 0; idx < size; idx++)
            {
                var referenceCollection = referenceDictionary[idx];
                var collection = result[idx];

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