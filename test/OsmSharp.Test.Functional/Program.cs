// The MIT License (MIT)

// Copyright (c) 2017 Ben Abelshausen

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

using OsmSharp.Logging;
using OsmSharp.Test.Functional.Staging;
using System;
using System.IO;

namespace OsmSharp.Test.Functional
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // enable logging.
            OsmSharp.Logging.Logger.LogAction = (o, level, message, parameters) =>
            {
                Console.WriteLine(string.Format("[{0}-{3}] {1} - {2}", o, level, message, DateTime.Now.ToString()));
            };

            // download and extract test-data if not already there.
            OsmSharp.Logging.Logger.Log("Program", TraceEventType.Information, "Downloading Belgium...");
            Download.DownloadBelgiumAll();

            // create a source.
            var source = new OsmSharp.Streams.PBFOsmStreamSource(File.OpenRead(Download.BelgiumLocal));

            // loop over all objects and count them.
            int nodes = 0, ways = 0, relations = 0;
            var count = new Action(() =>
            {
                foreach (var osmGeo in source)
                {
                    if (osmGeo.Type == OsmGeoType.Node)
                    {
                        nodes++;
                    }
                    if (osmGeo.Type == OsmGeoType.Way)
                    {
                        ways++;
                    }
                    if (osmGeo.Type == OsmGeoType.Relation)
                    {
                        relations++;
                    }
                }
            });
            count.TestPerf("Test counting objects.");
            OsmSharp.Logging.Logger.Log("Program", TraceEventType.Information, "Counted {0} nodes, {1} ways and {2} relations.",
                nodes, ways, relations);

            // loop over all objects and count them ignoring nodes.
            nodes = 0;
            ways = 0;
            relations = 0;
            source.Reset();
            count = new Action(() =>
            {
                foreach (var osmGeo in source.EnumerateAndIgore(true, false, false))
                {
                    if (osmGeo.Type == OsmGeoType.Node)
                    {
                        nodes++;
                    }
                    if (osmGeo.Type == OsmGeoType.Way)
                    {
                        ways++;
                    }
                    if (osmGeo.Type == OsmGeoType.Relation)
                    {
                        relations++;
                    }
                }
            });
            count.TestPerf("Test counting objects without nodes.");
            OsmSharp.Logging.Logger.Log("Program", TraceEventType.Information, "Counted {0} nodes, {1} ways and {2} relations.",
                nodes, ways, relations);

            // loop over all objects and count them ignoring nodes.
            nodes = 0;
            ways = 0;
            relations = 0;
            source.Reset();
            count = new Action(() =>
            {
                foreach (var osmGeo in source.EnumerateAndIgore(true, true, false))
                {
                    if (osmGeo.Type == OsmGeoType.Node)
                    {
                        nodes++;
                    }
                    if (osmGeo.Type == OsmGeoType.Way)
                    {
                        ways++;
                    }
                    if (osmGeo.Type == OsmGeoType.Relation)
                    {
                        relations++;
                    }
                }
            });
            count.TestPerf("Test counting objects without nodes and ways.");
            OsmSharp.Logging.Logger.Log("Program", TraceEventType.Information, "Counted {0} nodes, {1} ways and {2} relations.",
                nodes, ways, relations);

            OsmSharp.Logging.Logger.Log("Program", TraceEventType.Information, "Testing finished.");
            Console.ReadLine();
        }
    }
}