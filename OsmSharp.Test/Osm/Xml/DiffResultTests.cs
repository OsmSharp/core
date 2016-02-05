// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2016 Abelshausen Ben
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
using System.IO;
using System.Xml.Serialization;
using OsmSharp.Osm.Changesets;

namespace OsmSharp.Test.Osm.Xml
{
    /// <summary>
    /// Contains tests for the diff result class.
    /// </summary>
    [TestFixture]
    public class DiffResultTests
    {
        /// <summary>
        /// Tests serialization.
        /// </summary>
        [Test]
        public void TestSerialize()
        {
            var diffResult = new DiffResult()
            {
                Version = 0.6,
                Generator = "OsmSharp",
                Results = new OsmGeoResult[]
                {
                    new NodeResult()
                    {
                        OldId = 1,
                        NewId = 2,
                        NewVersion = 2,
                    }
                }
            };

            var result = diffResult.SerializeToXml();
            Assert.AreEqual("<diffResult generator=\"OsmSharp\" version=\"0.6\"><node old_id=\"1\" new_id=\"2\" new_version=\"2\" /></diffResult>",
                result);
        }

        /// <summary>
        /// Tests deserialization.
        /// </summary>
        [Test]
        public void TestDeserialize()
        {
            var serializer = new XmlSerializer(typeof(DiffResult));

            var diffResult = serializer.Deserialize(
                new StringReader("<diffResult version=\"0.6\"></diffResult>")) as DiffResult;
            Assert.IsNotNull(diffResult);
            Assert.IsNull(diffResult.Results);
            Assert.AreEqual(0.6, diffResult.Version);
            Assert.IsNull(diffResult.Generator);

            diffResult = serializer.Deserialize(
                new StringReader("<diffResult generator=\"OsmSharp\" version=\"0.6\"></diffResult>")) as DiffResult;
            Assert.IsNotNull(diffResult);
            Assert.IsNull(diffResult.Results);
            Assert.AreEqual(0.6, diffResult.Version);
            Assert.AreEqual("OsmSharp", diffResult.Generator);

            diffResult = serializer.Deserialize(
                new StringReader("<diffResult generator=\"OsmSharp\" version=\"0.6\"><node old_id=\"1\" new_id=\"2\" new_version=\"2\" /></diffResult>")) as DiffResult;
            Assert.IsNotNull(diffResult);
            Assert.IsNull(diffResult.Results);
            Assert.AreEqual(0.6, diffResult.Version);
            Assert.AreEqual("OsmSharp", diffResult.Generator);
        }
    }
}