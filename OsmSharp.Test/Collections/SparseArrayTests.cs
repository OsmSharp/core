using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OsmSharp.Collections;
using OsmSharp.Math.Random;

namespace OsmSharp.Test.Collections
{
    /// <summary>
    /// Contains tests for the serializable sparse array.
    /// </summary>
    [TestFixture]
    public class SparseArrayTests
    {
        /// <summary>
        /// A simple test for the serializable sparse array.
        /// </summary>
        [Test]
        public void SparseArraySimpleTest()
        {
            var stringArrayRef = new string[1000];
            var stringArray = new SparseArray<string>(1000);

            var randomGenerator = new RandomGenerator(66707770); // make this deterministic 
            for (int idx = 0; idx < 1000; idx++)
            {
                if (randomGenerator.Generate(2.0) > 1)
                { // add data.
                    stringArrayRef[idx] = idx.ToString();
                    stringArray[idx] = idx.ToString();
                }
                else
                {
                    stringArrayRef[idx] = null;
                    stringArray[idx] = null;
                }
            }

            for (int idx = 0; idx < 1000; idx++)
            {
                Assert.AreEqual(stringArrayRef[idx], stringArray[idx]);
            }
        }

        /// <summary>
        /// Does a simple sparse array test.
        /// </summary>
        [Test]
        public void TestSparseArraySimple()
        {
            // intialize.
            var array = new SparseArray<int>(10);

            // fill and resize in the process.
            for (int idx = 0; idx < 1000; idx++)
            {
                if (idx >= array.Length)
                {
                    array.Resize(idx + 100);
                }
                array[idx] = idx;
            }
            for (int idx = 5000; idx < 10000; idx++)
            {
                if (idx >= array.Length)
                {
                    array.Resize(idx + 100);
                }
                array[idx] = idx;
            }

            // test content.
            for (int idx = 0; idx < 1000; idx++)
            {
                Assert.AreEqual(idx, array[idx]);
            }
            for (int idx = 1001; idx < 5000; idx++)
            {
                Assert.AreEqual(0, array[idx]);
            }
            for (int idx = 5000; idx < 10000; idx++)
            {
                Assert.AreEqual(idx, array[idx]);
            }

            // test enumerator.
            var list = new List<int>(array);
            for (int idx = 0; idx < 1000; idx++)
            {
                Assert.AreEqual(idx, list[idx]);
            }
            for (int idx = 1001; idx < 5000; idx++)
            {
                Assert.AreEqual(0, list[idx]);
            }
            for (int idx = 5000; idx < 10000; idx++)
            {
                Assert.AreEqual(idx, list[idx]);
            }
        }
    }
}
