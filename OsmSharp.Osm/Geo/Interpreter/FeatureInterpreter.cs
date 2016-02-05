// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2016 Abelshausen Ben
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

using System;
using OsmSharp.Osm.Data;
using OsmSharp.Collections.Tags;
using OsmSharp.Geo.Features;
using OsmSharp.Osm.Complete;

namespace OsmSharp.Osm.Geo.Interpreter
{
    /// <summary>
    /// Represents a geometry interpreter to convert OSM-objects to corresponding geometries.
    /// </summary>
    public abstract class FeatureInterpreter
    {
        /// <summary>
        /// Holds the default geometry interpreter.
        /// </summary>
        private static FeatureInterpreter _defaultInterpreter;

        /// <summary>
        /// Gets/sets the default interpreter.
        /// </summary>
        public static FeatureInterpreter DefaultInterpreter
        {
            get
            {
                if (_defaultInterpreter == null)
                { 
                    _defaultInterpreter = new SimpleFeatureInterpreter();
                }
                return _defaultInterpreter;
            }
            set
            {
                _defaultInterpreter = value;
            }
        }
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
        public virtual FeatureCollection Interpret(OsmGeo simpleOsmGeo, IOsmGeoSource data)
        {
            switch (simpleOsmGeo.Type)
            {
                case OsmGeoType.Node:
                    return this.Interpret(simpleOsmGeo as Node);
                case OsmGeoType.Way:
                    return this.Interpret((simpleOsmGeo as Way).CreateComplete(data));
                case OsmGeoType.Relation:
                    return this.Interpret((simpleOsmGeo as Relation).CreateComplete(data));
            }
            throw new ArgumentOutOfRangeException();
        }
    }
}