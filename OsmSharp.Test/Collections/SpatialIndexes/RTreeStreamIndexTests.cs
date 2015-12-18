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
using System.IO;
using NUnit.Framework;
using OsmSharp.Collections.SpatialIndexes;
using OsmSharp.Collections.SpatialIndexes.Serialization.v2;
using OsmSharp.Math;
using OsmSharp.Math.Primitives;
using OsmSharp.Math.Random;
using ProtoBuf.Meta;
using System.Linq;

namespace OsmSharp.Test.Collections.SpatialIndexes
{
    /// <summary>
    /// Contains tests for the R-tree index.
    /// </summary>
    [TestFixture]
    public class RTreeStreamIndexTests
    {
        /// <summary>
        /// Tests a simple empty R-tree.
        /// </summary>
        [Test]
        public void RTreeStreamSerializeSmallTest()
        {
			var rect1 = new BoxF2D(0, 0, 2, 2);
			var rect2 = new BoxF2D(4, 0, 6, 2);
			var rect3 = new BoxF2D(0, 4, 2, 6);
			var rect4 = new BoxF2D(4, 4, 6, 6);

            // create the index and reference index.
            var index = new RTreeMemoryIndex<string>();

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
            
            var stream = new MemoryStream();
            var serializer = new DataTestClassSerializer();
            serializer.Serialize(stream, index);

            ISpatialIndexReadonly<string> deserialized =
                serializer.Deserialize(stream, true);

            // some simple queries.
            var result = new HashSet<string>(
                deserialized.Get(rect4));
            Assert.AreEqual(4, result.Count);
            Assert.IsTrue(result.Contains(rect4.ToString() + "1"));
            Assert.IsTrue(result.Contains(rect4.ToString() + "2"));
            Assert.IsTrue(result.Contains(rect4.ToString() + "3"));
            Assert.IsTrue(result.Contains(rect4.ToString() + "4"));

            result = new HashSet<string>(
                deserialized.Get(rect3));
            Assert.AreEqual(4, result.Count);
            Assert.IsTrue(result.Contains(rect3.ToString() + "1"));
            Assert.IsTrue(result.Contains(rect3.ToString() + "2"));
            Assert.IsTrue(result.Contains(rect3.ToString() + "3"));
            Assert.IsTrue(result.Contains(rect3.ToString() + "4"));

            result = new HashSet<string>(
                deserialized.Get(rect2));
            Assert.AreEqual(4, result.Count);
            Assert.IsTrue(result.Contains(rect2.ToString() + "1"));
            Assert.IsTrue(result.Contains(rect2.ToString() + "2"));
            Assert.IsTrue(result.Contains(rect2.ToString() + "3"));
            Assert.IsTrue(result.Contains(rect2.ToString() + "4"));

            result = new HashSet<string>(
                deserialized.Get(rect1));
            Assert.AreEqual(4, result.Count);
            Assert.IsTrue(result.Contains(rect1.ToString() + "1"));
            Assert.IsTrue(result.Contains(rect1.ToString() + "2"));
            Assert.IsTrue(result.Contains(rect1.ToString() + "3"));
            Assert.IsTrue(result.Contains(rect1.ToString() + "4"));

        }

        /// <summary>
        /// Tests an empty R-tree.
        /// </summary>
        [Test]
        public void RTreeStreamSerializeIndexAddTests()
        {
            // build test-data.
			var testDataList = new List<KeyValuePair<BoxF2D, string>>();
            const int count = 1000;
            var randomGenerator = new RandomGenerator(66707770); // make this deterministic 
            for (int idx = 0; idx < count; idx++)
            {
                double x1 = randomGenerator.Generate(1.0);
                double x2 = randomGenerator.Generate(1.0);
                double y1 = randomGenerator.Generate(1.0);
                double y2 = randomGenerator.Generate(1.0);

				var box = new BoxF2D(new PointF2D(x1, y1), new PointF2D(x2, y2));

				testDataList.Add(new KeyValuePair<BoxF2D, string>(
                    box, idx.ToString(System.Globalization.CultureInfo.InvariantCulture)));
            }

            // create the index and reference index.
            var index = new RTreeMemoryIndex<string>();
            var reference = new ReferenceImplementation<string>();

            // add all the data.
            for (int idx = 0; idx < count; idx++)
            {
                var keyValuePair = testDataList[idx];
                index.Add(keyValuePair.Key, keyValuePair.Value);
                reference.Add(keyValuePair.Key, keyValuePair.Value);

                //Assert.AreEqual(reference.Count(), index.Count());
            }

            //Assert.AreEqual(count, index.Count());

            var stream = new MemoryStream();
            var serializer = new DataTestClassSerializer();
            serializer.Serialize(stream, index);

            ISpatialIndexReadonly<string> deserialized =
                serializer.Deserialize(stream, true);

            // query all.
			var totalBox = new BoxF2D(0, 0, 1, 1);
            var resultIndex = new HashSet<string>(deserialized.Get(totalBox));
            var resultReference = new HashSet<string>(reference.Get(totalBox));
            foreach (var data in resultIndex)
            {
                Assert.IsTrue(resultReference.Contains(data));
            }
            foreach (var data in resultReference)
            {
                Assert.IsTrue(resultIndex.Contains(data));
            }

            // generate random boxes and compare results.
            for (int idx = 0; idx < 20; idx++)
            {
                double x1 = randomGenerator.Generate(1.0);
                double x2 = randomGenerator.Generate(1.0);
                double y1 = randomGenerator.Generate(1.0);
                double y2 = randomGenerator.Generate(1.0);

				var box = new BoxF2D(new PointF2D(x1, y1), new PointF2D(x2, y2));

                resultIndex = new HashSet<string>(deserialized.Get(box));
                resultReference = new HashSet<string>(reference.Get(box));

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
        /// A serializer for scene primitive spatial index.
        /// </summary>
        private class DataTestClassSerializer : RTreeStreamSerializer<string>
        {
            /// <summary>
            /// Builds the runtime type model.
            /// </summary>
            /// <param name="typeModel"></param>
            protected override void BuildRuntimeTypeModel(RuntimeTypeModel typeModel)
            {
                MetaType collectionMetaType = 
                    typeModel.Add(typeof(PrimitivesCollection), false);
                collectionMetaType.Add(1, "Data");
                collectionMetaType.Add(2, "MinX");
                collectionMetaType.Add(3, "MaxX");
                collectionMetaType.Add(4, "MinY");
                collectionMetaType.Add(5, "MaxY");
            }

            /// <summary>
            /// Serializes the actual data.
            /// </summary>
            /// <param name="typeModel"></param>
            /// <param name="data"></param>
            /// <param name="boxes"></param>
            /// <returns></returns>
            protected override byte[] Serialize(RuntimeTypeModel typeModel, List<string> data, 
			                                    List<BoxF2D> boxes)
            {
                var collection = new PrimitivesCollection();
                collection.Data = data.ToArray();
                collection.MinX = new double[boxes.Count];
                collection.MaxX = new double[boxes.Count];
                collection.MinY = new double[boxes.Count];
                collection.MaxY = new double[boxes.Count];
                for (int idx = 0; idx < boxes.Count; idx++)
                {
                    collection.MinX[idx] = boxes[idx].Min[0];
                    collection.MaxX[idx] = boxes[idx].Max[0];
                    collection.MinY[idx] = boxes[idx].Min[1];
                    collection.MaxY[idx] = boxes[idx].Max[1];
                }

                // create the memory stream.
                var stream = new MemoryStream();
                typeModel.Serialize(stream, collection);
                return stream.ToArray();
            }

            /// <summary>
            /// Deserializes the actual data.
            /// </summary>
            /// <param name="typeModel"></param>
            /// <param name="data"></param>
            /// <param name="boxes"></param>
            /// <returns></returns>
			protected override List<string> DeSerialize(RuntimeTypeModel typeModel, byte[] data, out List<BoxF2D> boxes)
            {
                // deserialize data.
                var collection =
                    (typeModel.Deserialize(new MemoryStream(data), null, typeof (PrimitivesCollection)) as PrimitivesCollection);

                if (collection == null) throw new Exception("Cannot deserialize data!");

                // fill boxes.
				boxes = new List<BoxF2D>(collection.Data.Length);
                for (int idx = 0; idx < collection.Data.Length; idx++)
                {
					boxes.Add(new BoxF2D(collection.MinX[idx], collection.MinY[idx], 
                        collection.MaxX[idx], collection.MaxY[idx]));
                }
                return new List<string>(collection.Data);
            }

            /// <summary>
            /// Returns the version.
            /// </summary>
            public override string VersionString
            {
                get { return "TestString.v1"; }
            }

            /// <summary>
            /// Holds primitives.
            /// </summary>
            private class PrimitivesCollection
            {
                /// <summary>
                /// Holds the icons.
                /// </summary>
                public string[] Data { get; set; }

                /// <summary>
                /// Gets or sets the minX arrays.
                /// </summary>
                public double[] MinX { get; set; }

                /// <summary>
                /// Gets or sets the maxX arrays.
                /// </summary>
                public double[] MaxX { get; set; }

                /// <summary>
                /// Gets or sets the minY arrays.
                /// </summary>
                public double[] MinY { get; set; }

                /// <summary>
                /// Gets or sets the maxY arrays.
                /// </summary>
                public double[] MaxY { get; set; }
            }
        }
    }
}