OsmSharp 6.1.0 Release Notes
----------------------------

This is a minor release.

- Completed the ICompleteOsmGeo interface by adding missing properties.
- Added support for netstandard1.3 and netstandard2.0 to OsmSharp.Geo after the NTS stable release (Thanks NTS team!).

### 6.0.1

This was a mayor release upgrading 5.x to 6.x because of some important breaking changes. The most important being the upgrade of lat/lons on nodes from floats to doubles. We also setup some documentation [here](http://docs.itinero.tech/docs/osmsharp/index.html).

New features:

- Changed lat/lon to doubles instead of floats.
- Writing compressed PBF.
- Added other concepts apart from Nodes, Ways and Relations like Users to be better able to handle interactions with the OSM-API.
- OsmSharp now includes .NET standard support, we now support `netstandard1.3` and `netstandard2.0`. We removed the PCL profiles because it was only there for Xamarin but that is now covered by .NET standard.
- OsmSharp.Geo for now only supports .NET Framework 4.5 and up. Support for .NET standard is coming as soon as [NTS](https://github.com/NetTopologySuite/NetTopologySuite) releases a stable with support.
- Support for extra root attributes in OSM-XML.
- Better support for changesets:
   - Applying changesets to OSM streams.
   - Compressing multiple changesets to their minimized equivalent.
- Better support for non-seekable streams.
- MIT license.

### 5.0.8

We didn't write release notes before. The 5.x release are the first stables after refactoring OsmSharp by removing UI & routing components.