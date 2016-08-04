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

using OsmSharp.Geo;
using OsmSharp.Streams;
using System.IO;

namespace Sample.GeoFilter
{
    class Program
    {
        static void Main(string[] args)
        {
            Staging.ToFile("ftp://ftp.osmsharp.com/data/OSM/planet/europe/luxembourg-latest.osm.pbf", "luxembourg-latest.osm.pbf");
            var polygon = Staging.LoadPolygon();

            using (var fileStreamSource = File.OpenRead("luxembourg-latest.osm.pbf"))
            using (var fileStreamTarget = File.OpenWrite("polygon.osm"))
            {
                // create source stream.
                var source = new PBFOsmStreamSource(fileStreamSource); 

                // filter by keeping everything inside the given polygon.
                var filtered = source.FilterSpatial(polygon);

                // write to output xml
                var target = new XmlOsmStreamTarget(fileStreamTarget);
                target.RegisterSource(filtered);
                target.Pull();
            }
        }
    }
}