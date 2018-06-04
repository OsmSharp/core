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
using OsmSharp.Geo;
using OsmSharp.Tags;
using System.Linq;

namespace OsmSharp.Test.Geo.Streams.Features.Interpreted
{
    /// <summary>
    /// Contains tests for the interpreted feature stream.
    /// </summary>
    [TestFixture]
    public class InterpretedFeatureStreamSourceTests
    {
        /// <summary>
        /// Tests a stream with an area.
        /// </summary>
        [Test]
        public void TestArea()
        {
            var source = new OsmGeo[] {
                new Node()
                {
                    Id = 1,
                    Latitude = 0,
                    Longitude = 0
                },
                new Node()
                {
                    Id = 2,
                    Latitude = 1,
                    Longitude = 0
                },
                new Node()
                {
                    Id = 3,
                    Latitude = 0,
                    Longitude = 1
                },
                new Way()
                {
                    Id = 1,
                    Nodes = new long[]
                    {
                        1, 2, 3, 1
                    },
                    Tags = new TagsCollection(
                        new Tag("area", "yes"))
                }
            };

            var features = source.ToFeatureSource();
            Assert.IsNotNull(features);
            var featuresList = features.ToList();
            Assert.IsNotNull(featuresList);
            Assert.AreEqual(1, featuresList.Count);
        }
    }
}