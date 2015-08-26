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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OsmSharp.Collections;

namespace OsmSharp.Test
{
    /// <summary>
    /// Summary description for StringTableTests
    /// </summary>
    [TestFixture]
    public class StringTableTests
    {
        /// <summary>
        /// Tests adding strings to a string table.
        /// </summary>
        [Test]
        public void TestStringTable_AddStrings()
        {
            ObjectTable<string> table = new ObjectTable<string>(false);
            table.Add("zero");
            table.Add("one");
            table.Add("two");
            table.Add("three");
            table.Add("four");
            table.Add("five");
            table.Add("six");

            Assert.AreEqual("zero", table.Get(0));
            Assert.AreEqual("one", table.Get(1));
            Assert.AreEqual("two", table.Get(2));
            Assert.AreEqual("three", table.Get(3));
            Assert.AreEqual("four", table.Get(4));
            Assert.AreEqual("five", table.Get(5));
            Assert.AreEqual("six", table.Get(6));


            table = new ObjectTable<string>(true);
            table.Add("zero");
            table.Add("one");
            table.Add("two");
            table.Add("three");
            table.Add("four");
            table.Add("five");
            table.Add("six");

            Assert.AreEqual("zero", table.Get(0));
            Assert.AreEqual("one", table.Get(1));
            Assert.AreEqual("two", table.Get(2));
            Assert.AreEqual("three", table.Get(3));
            Assert.AreEqual("four", table.Get(4));
            Assert.AreEqual("five", table.Get(5));
            Assert.AreEqual("six", table.Get(6));
        }

        /// <summary>
        /// Tests adding strings twice to a string table.
        /// </summary>
        [Test]
        public void TestStringTable_AddStringsTwice()
        {
            ObjectTable<string> table = new ObjectTable<string>(false);
            table.Add("zero");
            table.Add("one");
            table.Add("two");
            table.Add("three");
            table.Add("four");
            table.Add("five");
            table.Add("six");

            Assert.AreEqual("zero", table.Get(0));
            Assert.AreEqual("one", table.Get(1));
            Assert.AreEqual("two", table.Get(2));
            Assert.AreEqual("three", table.Get(3));
            Assert.AreEqual("four", table.Get(4));
            Assert.AreEqual("five", table.Get(5));
            Assert.AreEqual("six", table.Get(6));

            Assert.AreEqual((uint)0, table.Add("zero"));
            Assert.AreEqual((uint)1, table.Add("one"));
            Assert.AreEqual((uint)2, table.Add("two"));
            Assert.AreEqual((uint)3, table.Add("three"));
            Assert.AreEqual((uint)4, table.Add("four"));
            Assert.AreEqual((uint)5, table.Add("five"));
            Assert.AreEqual((uint)6, table.Add("six"));

            Assert.AreEqual("zero", table.Get(0));
            Assert.AreEqual("one", table.Get(1));
            Assert.AreEqual("two", table.Get(2));
            Assert.AreEqual("three", table.Get(3));
            Assert.AreEqual("four", table.Get(4));
            Assert.AreEqual("five", table.Get(5));
            Assert.AreEqual("six", table.Get(6));

            table = new ObjectTable<string>(true);
            table.Add("zero");
            table.Add("one");
            table.Add("two");
            table.Add("three");
            table.Add("four");
            table.Add("five");
            table.Add("six");

            Assert.AreEqual("zero", table.Get(0));
            Assert.AreEqual("one", table.Get(1));
            Assert.AreEqual("two", table.Get(2));
            Assert.AreEqual("three", table.Get(3));
            Assert.AreEqual("four", table.Get(4));
            Assert.AreEqual("five", table.Get(5));
            Assert.AreEqual("six", table.Get(6));

            Assert.AreEqual((uint)0, table.Add("zero"));
            Assert.AreEqual((uint)1, table.Add("one"));
            Assert.AreEqual((uint)2, table.Add("two"));
            Assert.AreEqual((uint)3, table.Add("three"));
            Assert.AreEqual((uint)4, table.Add("four"));
            Assert.AreEqual((uint)5, table.Add("five"));
            Assert.AreEqual((uint)6, table.Add("six"));

            Assert.AreEqual("zero", table.Get(0));
            Assert.AreEqual("one", table.Get(1));
            Assert.AreEqual("two", table.Get(2));
            Assert.AreEqual("three", table.Get(3));
            Assert.AreEqual("four", table.Get(4));
            Assert.AreEqual("five", table.Get(5));
            Assert.AreEqual("six", table.Get(6));
        }
    }
}