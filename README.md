# OsmSharp.Core

![Build status](http://build.osmsharp.com/app/rest/builds/buildType:(id:OsmSharp_CoreDevelop)/statusIcon)

OsmSharp's core enables you to work directly with OSM-data in .NET/Mono. Most important features are:

- Read/Write OSM-XML.
- Read/Write OSM-PBF.
- Streamed architecture, minimal memory footprint.
- Convert a stream of native OSM objects to 'complete' OSM objects: Ways with all their actual nodes, Relations with all members instantiated.
- Convert OSM objects to geometries.

### Install

    PM> Install-Package OsmSharp.Core -IncludePrerelease

### Usage

**WARNING: All this documentation applies to v2 while the latest stable release is v1.3.5! Install the prerelease version.**

A common usecase is to stream and filter OSM data. To read from an OSM file and enumerate all objects just open the file as a stream source and use foreach.

Read data from an OSM-PBF file:

```csharp
// using OsmSharp.Osm.PBF.Streams;

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
// using OsmSharp.Collections.Tags;
// using OsmSharp.Osm;
// using OsmSharp.Osm.PBF.Streams;

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
// using OsmSharp.Math.Geo;
// using OsmSharp.Osm.PBF.Streams;

var source = new PBFOsmStreamSource(
	new FileInfo(@"/path/to/file.osm.pbf").OpenRead());

var filtered = source.FilterBox(6.238002777099609f, 49.72076145492323f, 
	6.272850036621093f, 49.69928180928878f); // left, top, right, bottom

var target = new PBFOsmStreamTarget(
	new FileInfo(@"/path/to/filterede.osm.pbf").Open(FileMode.Create, FileAccess.ReadWrite));
target.RegisterSource(filter);
target.Pull();
```


