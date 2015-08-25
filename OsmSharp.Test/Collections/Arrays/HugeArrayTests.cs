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

using NUnit.Framework;
using OsmSharp.Collections.Arrays;
using OsmSharp.Math.Random;
using System;

namespace OsmSharp.Test.Unittests.Collections.Arrays
{
    /// <summary>
    /// Contains tests for the huge array.
    /// </summary>
    [TestFixture]
    public class HugeArrayTests
    {
        ///// <summary>
        ///// A simple test for the huge array.
        ///// </summary>
        //[Test]
        //public void HugeArraySimpleTest()
        //{
        //    var stringArrayRef = new string[1000];
        //    var stringArray = new HugeArray<string>(1000, 300);

        //    var randomGenerator = new RandomGenerator(66707770); // make this deterministic 
        //    for (int idx = 0; idx < 1000; idx++)
        //    {
        //        if (randomGenerator.Generate(2.0) > 1)
        //        { // add data.
        //            stringArrayRef[idx] = idx.ToString();
        //            stringArray[idx] = idx.ToString();
        //        }
        //        else
        //        {
        //            stringArrayRef[idx] = null;
        //            stringArray[idx] = null;
        //        }
        //    }

        //    for (int idx = 0; idx < 1000; idx++)
        //    {
        //        Assert.AreEqual(stringArrayRef[idx], stringArray[idx]);
        //    }
        //}

        ///// <summary>
        ///// A simple test resizing the huge array 
        ///// </summary>
        //[Test]
        //public void HugeArrayResizeTests()
        //{
        //    var stringArrayRef = new string[1000];
        //    var stringArray = new HugeArray<string>(1000, 300);

        //    var randomGenerator = new RandomGenerator(66707770); // make this deterministic 
        //    for (int idx = 0; idx < 1000; idx++)
        //    {
        //        if (randomGenerator.Generate(2.0) > 1)
        //        { // add data.
        //            stringArrayRef[idx] = idx.ToString();
        //            stringArray[idx] = idx.ToString();
        //        }
        //        else
        //        {
        //            stringArrayRef[idx] = null;
        //            stringArray[idx] = null;
        //        }
        //    }

        //    Array.Resize<string>(ref stringArrayRef, 335);
        //    stringArray.Resize(335);

        //    Assert.AreEqual(stringArrayRef.Length, stringArray.Length);
        //    for (int idx = 0; idx < stringArrayRef.Length; idx++)
        //    {
        //        Assert.AreEqual(stringArrayRef[idx], stringArray[idx]);
        //    }

        //    stringArrayRef = new string[1000];
        //    stringArray = new HugeArray<string>(1000, 300);

        //    for (int idx = 0; idx < 1000; idx++)
        //    {
        //        if (randomGenerator.Generate(2.0) > 1)
        //        { // add data.
        //            stringArrayRef[idx] = idx.ToString();
        //            stringArray[idx] = idx.ToString();
        //        }
        //        else
        //        {
        //            stringArrayRef[idx] = null;
        //            stringArray[idx] = null;
        //        }
        //    }

        //    Array.Resize<string>(ref stringArrayRef, 1235);
        //    stringArray.Resize(1235);

        //    Assert.AreEqual(stringArrayRef.Length, stringArray.Length);
        //    for (int idx = 0; idx < stringArrayRef.Length; idx++)
        //    {
        //        Assert.AreEqual(stringArrayRef[idx], stringArray[idx]);
        //    }
        //}
    }
}