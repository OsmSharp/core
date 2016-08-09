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

using OsmSharp.Streams;
using Sample.CompleteStream.Staging;
using System;
using System.IO;
using System.Linq;

namespace Sample.CompleteStream
{
    class Program
    {
        static void Main(string[] args)
        {
            Download.ToFile("http://files.itinero.tech/data/OSM/planet/europe/luxembourg-latest.osm.pbf", "luxembourg-latest.osm.pbf").Wait();

            using (var fileStream = File.OpenRead("luxembourg-latest.osm.pbf"))
            {
                // create source stream.
                var source = new PBFOsmStreamSource(fileStream);
                
                // filter all powerlines and keep all nodes.
                var filtered = from osmGeo in source
                               where osmGeo.Type == OsmSharp.OsmGeoType.Node || 
                                     (osmGeo.Type == OsmSharp.OsmGeoType.Way && osmGeo.Tags != null && osmGeo.Tags.Contains("power", "line"))
                               select osmGeo;

                // convert to complete stream.
                // WARNING: nodes that are partof powerlines will be kept in-memory.
                //          it's important to filter only the objects you need **before** 
                //          you convert to a complete stream otherwise all objects will 
                //          be kept in-memory.
                var complete = filtered.ToComplete();

                // enumerate result.
                foreach (var osmGeo in complete)
                {
                    if (osmGeo.Type == OsmSharp.OsmGeoType.Way)
                    { // the nodes are still in the stream, we need them to construct the 'complete' objects.
                        Console.WriteLine(osmGeo.ToString());
                    }
                }
            }
        }
    }
}