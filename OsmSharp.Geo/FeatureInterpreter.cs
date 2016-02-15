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

using NetTopologySuite.Features;
using OsmSharp.Complete;
using OsmSharp.Db;
using OsmSharp.Tags;
using System;

namespace OsmSharp.Geo
{
    /// <summary>
    /// Represents a geometry interpreter to convert OSM-objects to corresponding geometries.
    /// </summary>
    public abstract class FeatureInterpreter
    {
        /// <summary>
        /// Interprets an OSM-object and returns the corresponding geometry.
        /// </summary>
        public abstract FeatureCollection Interpret(ICompleteOsmGeo osmObject);

        /// <summary>
        /// Returns true if the given tags collection contains potential area tags.
        /// </summary>
        public abstract bool IsPotentiallyArea(TagsCollectionBase tags);

        /// <summary>
        /// Interprets an OSM-object and returns the correctponding geometry.
        /// </summary>
        public virtual FeatureCollection Interpret(OsmGeo osmGeo, ISnapshotDb data)
        {
            switch (osmGeo.Type)
            {
                case OsmGeoType.Node:
                    return this.Interpret(osmGeo as Node);
                case OsmGeoType.Way:
                    return this.Interpret((osmGeo as Way).CreateComplete(data));
                case OsmGeoType.Relation:
                    return this.Interpret((osmGeo as Relation).CreateComplete(data));
            }
            throw new ArgumentOutOfRangeException();
        }
    }
}