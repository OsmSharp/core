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

using System;
using System.Globalization;

namespace OsmSharp
{
    /// <summary>
    /// Contains extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Returns a string representing the object in a culture invariant way.
        /// </summary>
        public static string ToInvariantString(this object obj)
        {
            return obj is IConvertible ? ((IConvertible)obj).ToString(CultureInfo.InvariantCulture)
                : obj is IFormattable ? ((IFormattable)obj).ToString(null, CultureInfo.InvariantCulture)
                : obj.ToString();
        }

        /// <summary>
        /// Compares this OsmGeo object to another using type, id and version number. Nodes before ways, ways before relations.
        /// </summary>
        public static int CompareByIdVersionAndType(this OsmGeo osmGeo, OsmGeo other)
        {
            if (osmGeo == null) { throw new ArgumentNullException("osmGeo"); }
            if (other == null) { throw new ArgumentNullException("other"); }
            if (osmGeo.Id == null || osmGeo.Version == null) { throw new ArgumentException("To compare objects must have id and version set."); }
            if (other.Id == null || other.Version == null) { throw new ArgumentException("To compare objects must have id and version set."); }

            if (osmGeo.Type == other.Type)
            {
                if (osmGeo.Id == other.Id)
                {
                    return osmGeo.Version.Value.CompareTo(other.Version.Value);
                }
                return osmGeo.Id.Value.CompareTo(other.Id.Value);
            }
            switch (osmGeo.Type)
            {
                case OsmGeoType.Node:
                    return -1;
                case OsmGeoType.Way:
                    switch (other.Type)
                    {
                        case OsmGeoType.Node:
                            return 1;
                        case OsmGeoType.Relation:
                            return -1;
                    }
                    throw new Exception("Invalid OsmGeoType.");
                case OsmGeoType.Relation:
                    return 1;
            }
            throw new Exception("Invalid OsmGeoType.");
        }

        /// <summary>
        /// Compares this OsmGeo object to another using type, and id. Nodes before ways, ways before relations.
        /// </summary>
        public static int CompareByIdAndType(this OsmGeo osmGeo, OsmGeo other)
        {
            if (osmGeo == null) { throw new ArgumentNullException("osmGeo"); }
            if (other == null) { throw new ArgumentNullException("other"); }
            if (osmGeo.Id == null) { throw new ArgumentException("To compare objects must have id set."); }
            if (other.Id == null) { throw new ArgumentException("To compare objects must have id set."); }

            if (osmGeo.Type == other.Type)
            {
                return osmGeo.Id.Value.CompareTo(other.Id.Value);
            }
            switch (osmGeo.Type)
            {
                case OsmGeoType.Node:
                    return -1;
                case OsmGeoType.Way:
                    switch (other.Type)
                    {
                        case OsmGeoType.Node:
                            return 1;
                        case OsmGeoType.Relation:
                            return -1;
                    }
                    throw new Exception("Invalid OsmGeoType.");
                case OsmGeoType.Relation:
                    return 1;
            }
            throw new Exception("Invalid OsmGeoType.");
        }
    }
}