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
using OsmSharp.Streams;
using OsmSharp.Test.Functional.Staging;
using System;
using System.IO;
using System.Linq;
using OsmSharp.Streams.Complete;

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
            OsmSharp.Logging.Logger.Log("Program", TraceEventType.Information, "Downloading PBF...");
            Download.DownloadAll();

            // create a source.
            var source = new OsmSharp.Streams.PBFOsmStreamSource(File.OpenRead(Download.Local));

            // loop over all objects and count them.
            int nodes = 0, ways = 0, relations = 0;
            var testAction = new Action(() =>
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
            testAction.TestPerf("Test counting objects.");
            OsmSharp.Logging.Logger.Log("Program", TraceEventType.Information, "Counted {0} nodes, {1} ways and {2} relations.",
                nodes, ways, relations);

            // loop over all objects and count them ignoring nodes.
            nodes = 0;
            ways = 0;
            relations = 0;
            source.Reset();
            testAction = new Action(() =>
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
            testAction.TestPerf("Test counting objects without nodes.");
            OsmSharp.Logging.Logger.Log("Program", TraceEventType.Information, "Counted {0} nodes, {1} ways and {2} relations.",
                nodes, ways, relations);

            // loop over all objects and count them ignoring nodes.
            nodes = 0;
            ways = 0;
            relations = 0;
            source.Reset();
            testAction = new Action(() =>
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
            testAction.TestPerf("Test counting objects without nodes and ways.");
            OsmSharp.Logging.Logger.Log("Program", TraceEventType.Information, "Counted {0} nodes, {1} ways and {2} relations.",
                nodes, ways, relations);

            // write a compressed PBF.
            source.Reset();
            testAction = new Action(() =>
            {
                using (var output = File.Open("output.osm.pbf", FileMode.Create))
                {
                    var target = new OsmSharp.Streams.PBFOsmStreamTarget(output, true);
                    target.RegisterSource(source);
                    target.Pull();
                }
            });
            testAction.TestPerf("Test writing a PBF.");
            OsmSharp.Logging.Logger.Log("Program", TraceEventType.Information, "Written PBF.");
            
            // test converting to complete.
            source.Reset();
            testAction = new Action(() =>
            {
                var filteredQuery = from osmGeo in source where
                        osmGeo.Type == OsmGeoType.Way &&
                        osmGeo.Tags.ContainsKey("highway") &&
                        !osmGeo.Tags.Contains("highway", "cycleway") &&
                        !osmGeo.Tags.Contains("highway", "bus_stop") &&
                        !osmGeo.Tags.Contains("highway", "street_lamp") &&
                        !osmGeo.Tags.Contains("highway", "path") &&
                        !osmGeo.Tags.Contains("highway", "turning_cycle") &&
                        !osmGeo.Tags.Contains("highway", "traffic_signals") &&
                        !osmGeo.Tags.Contains("highway", "stops") &&
                        !osmGeo.Tags.Contains("highway", "pedestrian") &&
                        !osmGeo.Tags.Contains("highway", "footway") || 
                        osmGeo.Type == OsmGeoType.Node
                    select osmGeo;

                var collectionComplete = filteredQuery.ToComplete();
                var completeWays = collectionComplete.Where(x => x.Type == OsmGeoType.Way).ToList();
                OsmSharp.Logging.Logger.Log("Program", TraceEventType.Information, "Found {0} complete ways.", 
                    completeWays.Count);
            });
            testAction.TestPerf("Test converting to complete ways.");
            OsmSharp.Logging.Logger.Log("Program", TraceEventType.Information, "Converted completed ways.");
            
            // regression test for indexing names.
            source.Reset();
            testAction = new Action(() =>
            {
                var completeSource = new OsmSimpleCompleteStreamSource(source);
                var namesDictionary = completeSource
                    .Where(o => string.IsNullOrWhiteSpace(o.Tags.GetValue("name")) == false)
                    .GroupBy(o => o.Tags.GetValue("name"))
                    .ToDictionary(g => g.Key, g => g.ToList());
                OsmSharp.Logging.Logger.Log("Program", TraceEventType.Information, "Indexed {0} names.", 
                    namesDictionary.Count);
            });
            testAction.TestPerf("Test indexing names.");
            OsmSharp.Logging.Logger.Log("Program", TraceEventType.Information, "Indexed names.");

            OsmSharp.Logging.Logger.Log("Program", TraceEventType.Information, "Testing finished.");
#if DEBUG
            Console.ReadLine();
#endif
        }
    }
}