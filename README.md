# OsmSharp.Core

OsmSharp's core enables you to work directly with OSM-data in .NET/Mono. Most important features are:

- Read/Write OSM-XML.
- Read/Write OSM-PBF.
- Streamed architecture, minimal memory footprint.
- Convert a stream of native OSM objects to 'complete' OSM objects: Ways with all their actual nodes, Relations with all members instantiated.
- Convert OSM objects to geometries.

### Install

    PM> Install-Package OsmSharp.Core -IncludePrerelease

### Usage

A common usecase is to stream and filter OSM data. To read from an OSM file and enumerate all objects just open the file as a stream source and use foreach.


```csharp
// using OsmSharp.Osm.PBF.Streams;

using(var fileStream = new FileInfo(@"D:\work\data\OSM\albania.osm.pbf").OpenRead())
{
  var source = new PBFOsmStreamSource(fileStream);
  foreach (var element in source)
  {
    Console.WriteLine(element.ToString());
  }
}
```

