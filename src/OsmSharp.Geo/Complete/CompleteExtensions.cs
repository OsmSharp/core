using System.Linq;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using OsmSharp.Complete;

namespace OsmSharp.Geo.Complete;

/// <summary>
/// Extensions of the complete objects.
/// </summary>
public static class CompleteExtensions
{
    public static Feature ToFeature(this ICompleteOsmGeo completeOsmGeo)
    {
        return completeOsmGeo switch
        {
            Node n => new Feature(new Point(n.GetCoordinate()), n.Tags.ToAttributeTable()),
            CompleteWay w => new Feature(w.ToLineString(), w.Tags.ToAttributeTable()),
            _ => null
        };
    }

    /// <summary>
    /// Returns a line string representing the given way.
    /// </summary>
    /// <param name="way">The way.</param>
    /// <returns>The line string.</returns>
    internal static LineString ToLineString(this CompleteWay way)
    {
        return new LineString(way.Nodes
            .Select(x => new Coordinate(x.Longitude.Value, x.Latitude.Value)).ToArray());
    }
}