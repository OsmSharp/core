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
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Filter
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Download.Download.ToFile("http://planet.anyways.eu/planet/europe/luxembourg/luxembourg-latest.osm.pbf", "luxembourg-latest.osm.pbf");

            await using var fileStream = File.OpenRead("luxembourg-latest.osm.pbf");
            
            var source = new PBFOsmStreamSource(fileStream); // create source stream.
            var filtered = from osmGeo in source 
                where osmGeo.Id % 100000 == 0 // let's use linq to leave only objects with and id dividable by 100000.
                select osmGeo;  
            foreach(var osmGeo in filtered)
            {
                Console.WriteLine(osmGeo.ToString());
            }
        }
    }
}