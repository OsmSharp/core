### Geofilter sample

Another important operation on OSM-data is geographic filter, cutting out an area. The only object with geographical info in OSM are nodes so we filter the nodes and keep ways/relations that use one or more of the nodes/ways inside the area we want to filter.

This sample shows how filter using a polygon or just a bounding box. The main part of code is as follows:

##### Filter using bounding box

```csharp
using (var fileStream = File.OpenRead("path/to/file.osm.pbf"))
{
  // create source stream.
  var source = new PBFOsmStreamSource(fileStream); 
  
  // filter by keeping everything inside the given polygon.
  var filtered = source.FilterBox(6.238002777099609, 49.72076145492323, 
    6.272850036621093, 49.69928180928878);
  ...
}
```

##### Filter using polygon

```csharp
using (var fileStream = File.OpenRead("path/to/file.osm.pbf"))
{
  // create source stream.
  var source = new PBFOsmStreamSource(fileStream); 
  
  // filter by keeping everything inside the given polygon.
  var filtered = source.FilterSpatial(polygon);
  ...
}
```
