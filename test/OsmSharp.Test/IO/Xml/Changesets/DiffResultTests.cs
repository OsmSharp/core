// The MIT License (MIT)

// Copyright (c) 2016 Ben Abelshausen

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using NUnit.Framework;
using OsmSharp.IO.Xml;
using OsmSharp.Changesets;
using System.Xml.Serialization;
using System.IO;

namespace OsmSharp.Test.IO.Xml.Changesets
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