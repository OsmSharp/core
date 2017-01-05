### CompleteStream sample

When streaming from an OSM-PBF or OSM-XML file ways and relations only references to nodes/members by their id's. OsmSharp has the option to pick nodes that are part of a way or members of a relation from a stream and output objects that contain the actual nodes/members.

This sample shows how to convert objects in a stream to _complete_ objects. The main part of code is as follows:


```csharp
using (var fileStream = File.OpenRead("path/to/file.osm.pbf"))
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
}
```
