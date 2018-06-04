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

namespace OsmSharp.Db
{
    /// <summary>
    /// A unique identifier including types.
    /// </summary>
    public struct OsmGeoKey : IEquatable<OsmGeoKey>
    {
        /// <summary>
        /// Creates a version key.
        /// </summary>
        public OsmGeoKey(OsmGeoType type, long id)
        {
            this.Type = type;
            this.Id = id;
        }

        /// <summary>
        /// Creates a version key for the given object.
        /// </summary>
        public OsmGeoKey(OsmGeo osmGeo)
        {
            this.Type = osmGeo.Type;
            this.Id = osmGeo.Id.Value;
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public OsmGeoType Type { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Serves as a hashfunction.
        /// </summary>
        public override int GetHashCode()
        {
            return this.Id.GetHashCode() ^
                this.Type.GetHashCode();
        }

        /// <summary>
        /// Returns true if the given object represents the same key.
        /// </summary>
        public bool Equals(OsmGeoKey other)
        {
            return this.Type == other.Type && this.Id == other.Id;
        }

        /// <summary>
        /// Returns true if the given object represents the same key.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is OsmGeo && Equals((OsmGeo)obj);
        }
    }
}