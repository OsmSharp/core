# OsmSharp

[![Prerelease](https://github.com/OsmSharp/core/actions/workflows/develop.yml/badge.svg)](https://github.com/OsmSharp/core/actions/workflows/develop.yml)[![Visit our website](https://img.shields.io/badge/website-osmsharp.com-020031.svg) ](http://www.osmsharp.com/)[![MIT licensed](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/OsmSharp/core/blob/develop/LICENSE.md)  

- OsmSharp: [![NuGet Badge](https://buildstats.info/nuget/OsmSharp)](https://www.nuget.org/packages/OsmSharp/) [![NuGet Badge](https://buildstats.info/nuget/OsmSharp?includePreReleases=true)](https://www.nuget.org/packages/OsmSharp)  
- OsmSharp.Geo: [![NuGet Badge](https://buildstats.info/nuget/OsmSharp.Geo)](https://www.nuget.org/packages/OsmSharp.Geo) [![NuGet Badge](https://buildstats.info/nuget/OsmSharp.Geo?includePreReleases=true)](https://www.nuget.org/packages/OsmSharp.Geo)  

OsmSharp enables you to work directly with OpenStreetMap (OSM) data in .NET. Most important features are:

- Read/Write OSM-XML.
- Read/Write OSM-PBF.
- Streamed architecture, minimal memory footprint.
- Convert a stream of native OSM objects to 'complete' OSM objects: Ways with all their actual nodes, Relations with all members instantiated.
- Convert OSM objects to geometries.

### Documentation & Samples

Check the [documentation website](http://docs.itinero.tech/docs/osmsharp/index.html) for documentation and sample code. Don't hesitate to ask questions using an [issue](https://github.com/osmsharp/core/issues) or request more documentation on any topic you need.

There are 5 sample projects in the `samples` folder:

- Sample.CompleteStream: Convert Ways and Relation into objects with nodes and members instantiated.
- Sample.Filter: Filter data from an OSM file using LINQ.
- Sample.GeoFilter: Filter data using a bounding box or polygon.
- Sample.GeometryStream: Extract all power lines from an OSM file, convert them to linestring features and write to GeoJSON.
- Sample.GeometryStream.Shape: Extract all power lines from an OSM file, convert them to linestring features and write to a shapefile.

### Install

    PM> Install-Package OsmSharp
    
There's also a package to use [NTS](https://github.com/NetTopologySuite/) together with OsmSharp to convert OSM-data to features/geometries.

    PM> Install-Package OsmSharp.Geo

### Usage

A really good way to get started is to have a look at the [samples](https://github.com/OsmSharp/core/tree/master/samples) but we collected a few code-snippets for you here anyway:

A common usecase is to stream and filter OSM data. To read from an OSM file and enumerate all objects just open the file as a stream source and use foreach.

Read data from an OSM-PBF file:

```csharp
using(var fileStream = new FileInfo(@"/path/to/some/osmfile.osm.pbf").OpenRead())
{
  var source = new PBFOsmStreamSource(fileStream);
  foreach (var element in source)
  {
    Console.WriteLine(element.ToString());
  }
}
```

Write data to an OSM-PBF file:

```csharp
using(var fileStream = new FileInfo(@"/path/to/my/osmfile.osm.pbf").OpenRead())
{
	var target = new PBFOsmStreamTarget(fileStream);
	target.Initialize();
	target.AddNode(new Node()
		{
			Id = 1,
			ChangeSetId = 1,
			Latitude = 0,
			Longitude = 0,
			Tags = new TagsCollection(
				Tag.Create("key", "value")),
			TimeStamp = DateTime.Now,
			UserId = 1424,
			UserName = "you",
			Version = 1,
			Visible = true
		});
	target.Flush();
	target.Close();
}
```

Filter an area and extract a smaller region:

```csharp
var source = new PBFOsmStreamSource(
	new FileInfo(@"/path/to/file.osm.pbf").OpenRead());

var filtered = source.FilterBox(6.238002777099609f, 49.72076145492323f, 
	6.272850036621093f, 49.69928180928878f); // left, top, right, bottom

using (var stream = new FileInfo(@"/path/to/filterede.osm.pbf").Open(FileMode.Create, FileAccess.ReadWrite))
{
   var target = new PBFOsmStreamTarget(stream);
   target.RegisterSource(filtered);
   target.Pull();
}
```

### Licensing

The OsmSharp project is licensed under the [MIT license](https://github.com/OsmSharp/core/blob/master/LICENSE.md).

This project includes some code from [SharpZipLib](https://github.com/icsharpcode/SharpZipLib), also MIT licensed.
