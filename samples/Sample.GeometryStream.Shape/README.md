### GeometryStream Shapefile sample

A common problem is extracting actual geographical information from OSM-data. This sample shows how to extract powerlines from an OSM file and convert them to a shapefile. The main part of code is as follows:


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

  // convert to a feature stream.
  // WARNING: nodes that are partof powerlines will be kept in-memory.
  //          it's important to filter only the objects you need **before** 
  //          you convert to a feature stream otherwise all objects will 
  //          be kept in-memory.
  var features = filtered.ToFeatureSource();

  // filter out only linestrings.
  var lineStrings = from feature in features
    where feature.Geometry is LineString
    select feature;

 // build feature collection.
 var featureCollection = new FeatureCollection();
 var attributesTable = new AttributesTable();
 attributesTable.AddAttribute("type", "powerline");
 foreach (var feature in lineStrings)
 { // make sure there is a constant # of attributes with the same names before writing the shapefile.
	featureCollection.Add(new Feature(feature.Geometry, attributesTable));
 }

 // convert to shape.
 var header = ShapefileDataWriter.GetHeader(featureCollection.Features.First(), featureCollection.Features.Count);
 var shapeWriter = new ShapefileDataWriter("luxembourg.shp", new GeometryFactory())
 {
	Header = header
 };
 shapeWriter.Write(featureCollection.Features);
}
```
