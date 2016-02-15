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
using OsmSharp.Complete;
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
            if (way.Nodes != null)
            {
                var coordinates = new List<Coordinate>();
                for (int i = 0; i < way.Nodes.Length; i++)
                {
                    coordinates.Add(way.Nodes[i].GetCoordinate());
                }
                return coordinates;
            }
            return null;
        }

        /// <summary>
        /// Converts this tags collection to an attribute table.
        /// </summary>
        public static AttributesTable ToAttributeTable(this TagsCollectionBase tags)
        {
            var table = new AttributesTable();
            if (tags != null)
            {
                foreach(var tag in tags)
                {
                    table.AddAttribute(tag.Key, tag.Value);
                }
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
    }
}