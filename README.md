# OsmSharp.Core

OsmSharp's core enables you to work directly with OSM-data in .NET/Mono. Most important features are:

- Read/Write OSM-XML.
- Read/Write OSM-PBF.
- Streamed architecture, minimal memory footprint.
- Convert a stream of native OSM objects to 'complete' OSM objects: Ways with all their actual nodes, Relations with all members instantiated.
- Convert OSM objects to geometries.

### Install

    PM> Install-Package OsmSharp.Core

### Usage

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
	new FileInfo(@"/path/to/belgium-latest.osm.pbf").OpenRead());

var filter = new OsmSharp.Osm.Streams.Filters.OsmStreamFilterPoly(
	new OsmSharp.Geo.Geometries.LineairRing(
		new GeoCoordinate(51.084978552372114, 3.655529022216797),
		new GeoCoordinate(51.081851317961930, 3.812427520751953),
		new GeoCoordinate(51.994851160022010, 3.760070800781250),
		new GeoCoordinate(51.084978552372114, 3.655529022216797)));
filter.RegisterSource(source);

var target = new PBFOsmStreamTarget(
	new FileInfo(@"/path/to/gent-triangle.osm.pbf").Open(FileMode.Create, FileAccess.ReadWrite));
target.RegisterSource(filter);
target.Pull();
```


