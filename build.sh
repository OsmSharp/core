dotnet build ./src/OsmSharp -f netstandard1.3
# Waiting to port to .NET core, the problem is the NTS dependency.
# dotnet build ./src/OsmSharp.IO.Osm -f netstandard1.6

# Waiting to port to .NET core, the problem is the NTS dependency.
# dotnet build ./test/OsmSharp.Test
# dotnet build ./test/OsmSharp.Test.Functional

# Build samples.
dotnet build ./samples/Sample.CompleteStream
dotnet build ./samples/Sample.Filter
# Waiting to port to .NET core app, the problem is the NTS dependency..
# dotnet build ./samples/Sample.GeoFilter
# Waiting to port to .NET core app, the problem is the NTS dependency..
# dotnet build ./samples/Sample.GeometryStream
# Waiting to port to .NET core app, the problem is the NTS dependency..
# dotnet build ./samples/Sample.GeometryStream.Shape