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

namespace OsmSharp.Geo.Features
{
    /// <summary>
    /// Representation of a feature, a geometry with attributes.
    /// </summary>
    public class Feature
    {
        /// <summary>
        /// Creates a new feature.
        /// </summary>
        public Feature(Geometries.Geometry geometry)
        {
            this.Geometry = geometry;
            this.Attributes = new SimpleGeometryAttributeCollection();
        }

        /// <summary>
        /// Creates a new feature.
        /// </summary>
        public Feature(Geometries.Geometry geometry, Attributes.GeometryAttributeCollection attributes)
        {
            this.Geometry = geometry;
            this.Attributes = attributes;
        }

        /// <summary>
        /// Gets the geometry.
        /// </summary>
        public Geometries.Geometry Geometry { get; private set; }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        public Attributes.GeometryAttributeCollection Attributes { get; private set; }
    }
}
