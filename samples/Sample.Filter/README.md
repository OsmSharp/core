### Filter sample

When streaming from an OSM-PBF or OSM-XML file the source being used is a regular IEnumerable<T>. This means you can use Linq to select only this objects you want to keep.

This sample shows how to keep objects last touched by one OSM contributor. The main part of code is as follows:


```csharp
using (var fileStream = File.OpenRead("path/to/file.osm.pbf"))
{
  // create source stream.
  var source = new PBFOsmStreamSource(fileStream); 
  
  // let's use linq to leave only objects last touched by the given mapper.
  var filtered = from osmGeo in source where 
    osmGeo.UserName == "Javier Gutiérrez" 
    select osmGeo;  
    
  ...
}
```
