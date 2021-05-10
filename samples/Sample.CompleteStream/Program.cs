using OsmSharp.Streams;
using Sample.CompleteStream.Staging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.CompleteStream
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Download.ToFile("http://planet.anyways.eu/planet/europe/luxembourg/luxembourg-latest.osm.pbf", "luxembourg-latest.osm.pbf");
            await using var fileStream = File.OpenRead("luxembourg-latest.osm.pbf");
            
            // create source stream.
            var source = new PBFOsmStreamSource(fileStream);
                
            // filter all power lines and keep all nodes.
            var filtered = from osmGeo in source
                where osmGeo.Type == OsmSharp.OsmGeoType.Node || 
                      (osmGeo.Type == OsmSharp.OsmGeoType.Way && osmGeo.Tags != null && osmGeo.Tags.Contains("power", "line"))
                select osmGeo;

            // convert to complete stream.
            // WARNING: nodes that are part of power lines will be kept in-memory.
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