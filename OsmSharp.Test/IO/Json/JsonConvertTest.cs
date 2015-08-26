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
using OsmSharp.IO.Json;
using System;

namespace OsmSharp.Test.IO.Json
{
    /// <summary>
    /// Tests json serialization.
    /// </summary>
    [TestFixture]
    public class JsonConvertTest
    {
        /// <summary>
        /// Tests a simple serialization.
        /// </summary>
        [Test]
        public void TestSerialize()
        {
            var product = new Product();
            product.Name = "Apple";
            product.Expiry = new DateTime(2008, 12, 28);
            product.Sizes = new string[] { "Small" };

            var json = JsonConvert.SerializeObject(product);
            json.RemoveWhitespace();

            Assert.AreEqual("{\"Name\":\"Apple\",\"Expiry\":\"2008-12-28T00:00:00\",\"Sizes\":[\"Small\"]}", json);
        }

        /// <summary>
        /// Test a simple deserialization.
        /// </summary>
        [Test]
        public void TestDeserialize()
        {
            var json = "{\"Name\":\"Apple\",\"Expiry\":\"2008-12-28T00:00:00\",\"Sizes\":[\"Small\"]}";
            var product = JsonConvert.DeserializeObject<Product>(json);

            Assert.IsNotNull(product);
            Assert.AreEqual("Apple", product.Name);
            Assert.AreEqual(new DateTime(2008, 12, 28), product.Expiry);
            Assert.AreEqual(1, product.Sizes.Length);
            Assert.AreEqual("Small", product.Sizes[0]);
        }

        private class Product
        {
            public string Name { get; set; }

            public DateTime Expiry { get; set; }

            public string[] Sizes { get; set; }
        }
    }
}