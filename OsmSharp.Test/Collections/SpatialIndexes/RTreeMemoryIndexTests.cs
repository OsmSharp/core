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

using System;
using System.Collections.Generic;
using NUnit.Framework;
using OsmSharp.Collections.SpatialIndexes;
using OsmSharp.Math;
using OsmSharp.Math.Primitives;
using OsmSharp.Math.Random;
using System.Linq;

namespace OsmSharp.Test.Collections.SpatialIndexes
{
    /// <summary>
    /// Contains tests for the R-tree index.
    /// </summary>
    [TestFixture]
    public class RTreeMemoryIndexTests
    {
        /// <summary>
        /// Some small human-readable tests for the R-tree structure.
        /// </summary>
        [Test]
        public void RTreeMemoryIndexSmall1Tests()
        {
			var rect1 = new BoxF2D(0, 0, 2, 2);
			var rect2 = new BoxF2D(4, 0, 6, 2);
			var rect3 = new BoxF2D(0, 4, 2, 6);
			var rect4 = new BoxF2D(4, 4, 6, 6);

			var rect5 = new BoxF2D(1, 1, 3, 3);

            // create the index and reference index.
            var index = new RTreeMemoryIndex<string>(4, 1);

            // add data.
            index.Add(rect1, rect1.ToString() + "1");
            index.Add(rect1, rect1.ToString() + "2");
            index.Add(rect1, rect1.ToString() + "3");
            index.Add(rect1, rect1.ToString() + "4");

            index.Add(rect2, rect2.ToString() + "1");
            index.Add(rect2, rect2.ToString() + "2");
            index.Add(rect2, rect2.ToString() + "3");
            index.Add(rect2, rect2.ToString() + "4");

            index.Add(rect3, rect3.ToString() + "1");
            index.Add(rect3, rect3.ToString() + "2");
            index.Add(rect3, rect3.ToString() + "3");
            index.Add(rect3, rect3.ToString() + "4");

            index.Add(rect4, rect4.ToString() + "1");
            index.Add(rect4, rect4.ToString() + "2");
            index.Add(rect4, rect4.ToString() + "3");
            index.Add(rect4, rect4.ToString() + "4");

            index.Add(rect5, rect5.ToString());

            // some simple queries.
            var result = new HashSet<string>(
                index.Get(rect4));
            Assert.AreEqual(4, result.Count);
            Assert.IsTrue(result.Contains(rect4.ToString() + "1"));
            Assert.IsTrue(result.Contains(rect4.ToString() + "2"));
            Assert.IsTrue(result.Contains(rect4.ToString() + "3"));
            Assert.IsTrue(result.Contains(rect4.ToString() + "4"));

            result = new HashSet<string>(
                index.Get(rect3));
            Assert.AreEqual(4, result.Count);
            Assert.IsTrue(result.Contains(rect3.ToString() + "1"));
            Assert.IsTrue(result.Contains(rect3.ToString() + "2"));
            Assert.IsTrue(result.Contains(rect3.ToString() + "3"));
            Assert.IsTrue(result.Contains(rect3.ToString() + "4"));

            result = new HashSet<string>(
                index.Get(rect2));
            Assert.AreEqual(4, result.Count);
            Assert.IsTrue(result.Contains(rect2.ToString() + "1"));
            Assert.IsTrue(result.Contains(rect2.ToString() + "2"));
            Assert.IsTrue(result.Contains(rect2.ToString() + "3"));
            Assert.IsTrue(result.Contains(rect2.ToString() + "4"));

            result = new HashSet<string>(
                index.Get(rect1));
            Assert.AreEqual(5, result.Count);
            Assert.IsTrue(result.Contains(rect1.ToString() + "1"));
            Assert.IsTrue(result.Contains(rect1.ToString() + "2"));
            Assert.IsTrue(result.Contains(rect1.ToString() + "3"));
            Assert.IsTrue(result.Contains(rect1.ToString() + "4"));
            Assert.IsTrue(result.Contains(rect5.ToString()));
        }

        /// <summary>
        /// Tests adding stuff to the R-tree.
        /// </summary>
        [Test]
        public void RTreeMemoryIndexAddTests()
        {
            // build test-data.
			var testDataList = new List<KeyValuePair<BoxF2D, DataTestClass>>();
            const int count = 1000;
            var randomGenerator = new RandomGenerator(66707770); // make this deterministic 
            for (int idx = 0; idx < count; idx++)
            {
                double x1 = randomGenerator.Generate(1.0);
                double x2 = randomGenerator.Generate(1.0);
                double y1 = randomGenerator.Generate(1.0);
                double y2 = randomGenerator.Generate(1.0);

				var box = new BoxF2D(new PointF2D(x1, y1), new PointF2D(x2, y2));
                var testData = new DataTestClass();
                testData.Data = idx.ToString(System.Globalization.CultureInfo.InvariantCulture);

				testDataList.Add(new KeyValuePair<BoxF2D, DataTestClass>(
                    box, testData));
            }

            // create the index and reference index.
            var index = new RTreeMemoryIndex<DataTestClass>();
            var reference = new ReferenceImplementation<DataTestClass>();

            // add all the data.
            for (int idx = 0; idx < count; idx++)
            {
                var keyValuePair = testDataList[idx];
                index.Add(keyValuePair.Key, keyValuePair.Value);
                reference.Add(keyValuePair.Key, keyValuePair.Value);

                //Assert.AreEqual(reference.Count(), index.Count());
            }

            //Assert.AreEqual(count, index.Count());

            // generate random boxes and compare results.
            for (int idx = 0; idx < 20; idx++)
            {
                double x1 = randomGenerator.Generate(1.0);
                double x2 = randomGenerator.Generate(1.0);
                double y1 = randomGenerator.Generate(1.0);
                double y2 = randomGenerator.Generate(1.0);

				var box = new BoxF2D(new PointF2D(x1, y1), new PointF2D(x2, y2));

                var resultIndex = new HashSet<DataTestClass>(index.Get(box));
                var resultReference = new HashSet<DataTestClass>(reference.Get(box));

                foreach (var data in resultIndex)
                {
                    Assert.IsTrue(resultReference.Contains(data));
                }
                foreach (var data in resultReference)
                {
                    Assert.IsTrue(resultIndex.Contains(data));
                }
            }
        }

        /// <summary>
        /// Tests adding stuff to the R-tree and then enumerating.
        /// </summary>
        [Test]
        public void RTreeMemoryIndexEnumerationTests()
        {
            // create the index.
            var index = new RTreeMemoryIndex<DataTestClass>();

            // build test-data.
            var testDataList = new HashSet<DataTestClass>();
            const int count = 1000;
            var randomGenerator = new RandomGenerator(66707770); // make this deterministic 
            for (int idx = 0; idx < count; idx++)
            {
                double x1 = randomGenerator.Generate(1.0);
                double x2 = randomGenerator.Generate(1.0);
                double y1 = randomGenerator.Generate(1.0);
                double y2 = randomGenerator.Generate(1.0);

				var box = new BoxF2D(new PointF2D(x1, y1), new PointF2D(x2, y2));
                var testData = new DataTestClass();
                testData.Data = idx.ToString(System.Globalization.CultureInfo.InvariantCulture);
                testDataList.Add(testData);

                index.Add(box, testData);
            }

            // compare and check if all data gets enumerated.
            HashSet<DataTestClass> reference = new HashSet<DataTestClass>();
            foreach (DataTestClass dataTestClass in index)
            {
                reference.Add(dataTestClass);
                Assert.IsTrue(testDataList.Contains(dataTestClass));
            }
            Assert.AreEqual(testDataList.Count, reference.Count);
            Assert.AreEqual(testDataList.Count, index.Count);
        }

        /// <summary>
        /// Data test class.
        /// </summary>
        private class DataTestClass
        {
            /// <summary>
            /// Gets/sets data.
            /// </summary>
            public string Data { get; set; }

            /// <summary>
            /// Returns true if both contain the same data.
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                if (obj is DataTestClass)
                {
                    DataTestClass testClass = obj as DataTestClass;
                    if (string.IsNullOrEmpty(testClass.Data))
                    {
                        return string.IsNullOrEmpty(this.Data);
                    }
                    else if (string.IsNullOrEmpty(this.Data))
                    {
                        return string.IsNullOrEmpty(testClass.Data);
                    }
                    else
                    {
                        return this.Data.Equals(testClass.Data);
                    }
                }
                return false;
            }

            /// <summary>
            /// Returns the hashcode.
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                if (this.Data != null)
                {
                    return this.Data.GetHashCode();
                }
                return string.Empty.GetHashCode();
            }
        }
    }
}