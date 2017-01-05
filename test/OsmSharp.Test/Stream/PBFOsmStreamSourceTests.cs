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
using OsmSharp.Streams;
using System.Reflection;

namespace OsmSharp.Test.Stream
{
    /// <summary>
    /// Contains tests for the PBF osm stream source.
    /// </summary>
    [TestFixture]
    class PBFOsmStreamSourceTests
    {
        /// <summary>
        /// A regression test on resetting a PBF osm stream.
        /// </summary>
        [Test]
        public void PBFOsmStreamReaderReset()
        {
            // generate the source.
            var source = new PBFOsmStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "OsmSharp.Test.data.pbf.api.osm.pbf"));

            // pull the data out.
            var target = new OsmStreamTargetEmpty();
            target.RegisterSource(source);
            target.Pull();

            // reset the source.
            if (source.CanReset)
            {
                source.Reset();

                // pull the data again.
                target.Pull();
            }
        }

        /// <summary>
        /// A regression test on initializing a stream.
        /// </summary>
        [Test]
        public void MoveNextWayRegression1()
        {
            using (var fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "OsmSharp.Test.data.pbf.api.osm.pbf"))
            {
                using (var reader = new PBFOsmStreamSource(fileStream))
                {
                    var counter = 0;
                    while (reader.MoveNextWay())
                    {
                        if (counter++ % 10000 == 0)
                        {

                        }
                    }
                }
            }
        }
    }
}