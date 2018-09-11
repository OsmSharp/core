// The MIT License (MIT)

// Copyright (c) 2016 Ben Abelshausen

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using GeoAPI.Geometries;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using OsmSharp.Complete;
using OsmSharp.Geo.Streams;
using OsmSharp.Geo.Streams.Features.Interpreted;
using OsmSharp.Streams;
using OsmSharp.Tags;
using System;
using System.Collections.Generic;

namespace OsmSharp.Geo
{
    /// <summary>
    /// Contains extensions methods related to OSM-GeoAPI-NTS objects.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Gets the coordinate from the given node.
        /// </summary>
        public static Coordinate GetCoordinate(this Node node)
        {
            if (!node.Latitude.HasValue || !node.Longitude.HasValue) { throw new ArgumentException("Cannot get a coordinate from a node without a valid lat/lon."); }

            return new Coordinate(node.Longitude.Value, node.Latitude.Value);
        }

        /// <summary>
        /// Gets the coordinates from the given way.
        /// </summary>
        public static List<Coordinate> GetCoordinates(this CompleteWay way)
        {
            if (way.Nodes == null) return null;
            
            var coordinates = new List<Coordinate>();
            for (var i = 0; i < way.Nodes.Length; i++)
            {
                coordinates.Add(way.Nodes[i].GetCoordinate());
            }
            return coordinates;
        }

        /// <summary>
        /// Converts this tags collection to an attribute table.
        /// </summary>
        public static AttributesTable ToAttributeTable(this TagsCollectionBase tags)
        {
            var table = new AttributesTable();
            if (tags == null) return table;
            foreach(var tag in tags)
            {
                table.Add(tag.Key, tag.Value);
            }
            return table;
        }

        /// <summary>
        /// Returns true if this table contains the given attribute and value.
        /// </summary>
        public static bool Contains(this IAttributesTable table, string attribute, object value)
        {
            if (!table.Exists(attribute))
            {
                return false;
            }

            var existing = table[attribute];
            if (existing == null && value == null)
            {
                return true;
            }
            return existing.Equals(value);
        }

        /// <summary>
        /// Gets a range of elements.
        /// </summary>
        public static List<T> GetRange<T>(this T[] array, int index, int count)
        {
            var list = new List<T>(count);
            for(var i = index; i < index + count; i++)
            {
                list.Add(array[i]);
            }
            return list;
        }

        /// <summary>
        /// Filters a stream of objects spatially.
        /// </summary>
        public static OsmStreamSource FilterSpatial(this IEnumerable<OsmGeo> source, IPolygon polygon, bool completeWays = false)
        {
            var nodeFilter = new OsmSharp.Streams.Filters.OsmStreamFilterNode(n =>
            {
                if (n.Latitude == null || n.Longitude == null)
                {
                    return false;
                }
                return polygon.Contains(new Point(n.Longitude.Value, n.Latitude.Value));
            }, completeWays);
            nodeFilter.RegisterSource(source);
            return nodeFilter;
        }

        /// <summary>
        /// Converts the given source into a feature source.
        /// </summary>
        public static IFeatureStreamSource ToFeatureSource(this IEnumerable<OsmGeo> source)
        {
            return new InterpretedFeatureStreamSource(source.ToComplete(), new DefaultFeatureInterpreter());
        }
    }
}