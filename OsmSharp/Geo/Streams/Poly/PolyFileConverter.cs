// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using OsmSharp.Geo.Attributes;
using OsmSharp.Geo.Features;
using OsmSharp.Geo.Geometries;
using OsmSharp.Math.Geo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
namespace OsmSharp.Geo.Streams.Poly
{
    /// <summary>
    /// Contains read/writing methods for poly files.
    /// </summary>
    public static class PolyFileConverter
    {
        private static string END_TOKEN = "END";

        /// <summary>
        /// Reads a polygon from a string.
        /// </summary>
        /// <returns></returns>
        public static Feature ReadPolygon(string poly)
        {
            return PolyFileConverter.ReadPolygon(
                new StringReader(poly));
        }

        /// <summary>
        /// Reads a polygon from a stream.
        /// </summary>
        /// <returns></returns>
        public static Feature ReadPolygon(Stream stream)
        {
            return PolyFileConverter.ReadPolygon(
                new StreamReader(stream));
        }

        /// <summary>
        /// Reads a polygon from a text stream.
        /// </summary>
        /// <returns></returns>
        public static Feature ReadPolygon(TextReader reader)
        {
            var name = reader.ReadLine();
            var outer = PolyFileConverter.ReadRing(reader);
            var inner = PolyFileConverter.ReadRing(reader);
            var inners = new List<LineairRing>();
            while (inner != null)
            {
                inners.Add(inner);
                inner = PolyFileConverter.ReadRing(reader);
            }
            return new Feature(new Polygon(outer, inners), new SimpleGeometryAttributeCollection(
                new GeometryAttribute[] {
                    new GeometryAttribute()
                    {
                        Key = "name",
                        Value = name
                    }
                }));
        }

        /// <summary>
        /// Reads a lineair ring.
        /// </summary>
        /// <returns></returns>
        private static LineairRing ReadRing(TextReader reader)
        {
            var first = reader.ReadLine();
            if (first == null || END_TOKEN.Equals(first))
            { // there is no ring here.
                return null;
            }
            var ringCoordinates = new List<GeoCoordinate>();
            var line = reader.ReadLine();
            while (line != null && !END_TOKEN.Equals(line))
            {
                var coordinates = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                double x, y;
                if (!double.TryParse(coordinates[0], NumberStyles.Any, CultureInfo.InvariantCulture, out x) ||
                   !double.TryParse(coordinates[1], NumberStyles.Any, CultureInfo.InvariantCulture, out y))
                {
                    throw new Exception("Could not parse coordinates in poly.");
                }
                ringCoordinates.Add(new GeoCoordinate(y, x));
                line = reader.ReadLine();
            }
            if (ringCoordinates.Count < 3)
            {
                throw new Exception("Could not parse poly, a minimum of three coordinates are required.");
            }
            if (ringCoordinates[1] == ringCoordinates[ringCoordinates.Count - 1])
            { // detect centroid and discard it.
                ringCoordinates.RemoveAt(0);
            }
            else if (ringCoordinates[0] != ringCoordinates[ringCoordinates.Count - 1])
            { // make sure the ring is closed.
                ringCoordinates.Add(ringCoordinates[0]);
            }
            return new LineairRing(ringCoordinates);
        }
    }
}