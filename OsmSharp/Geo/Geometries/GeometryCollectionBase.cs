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

using OsmSharp.Math.Geo;
using System.Collections.Generic;

namespace OsmSharp.Geo.Geometries
{
    /// <summary>
    /// Represents a collection of geometry-objects.
    /// </summary>
    public abstract class GeometryCollectionBase<GeometryType> : Geometry, IEnumerable<GeometryType>
        where GeometryType : Geometry
    {
        private readonly List<GeometryType> _geometries;

        /// <summary>
        /// Creates a new geometry collection.
        /// </summary>
        public GeometryCollectionBase()
        {
            _geometries = new List<GeometryType>();
        }

        /// <summary>
        /// Creates a new geometry collection.
        /// </summary>
        public GeometryCollectionBase(IEnumerable<GeometryType> geometries)
        {
            _geometries = new List<GeometryType>(geometries);
        }

        /// <summary>
        /// Returns the object count.
        /// </summary>
        public int Count
        {
            get
            {
                return _geometries.Count;
            }
        }

        /// <summary>
        /// Adds a new geometry.
        /// </summary>
        public void Add(GeometryType geometry)
        {
            _geometries.Add(geometry);
        }

        /// <summary>
        /// Adds all geometries in the given enumerable.
        /// </summary>
        public void AddRange(IEnumerable<GeometryType> geometries)
        {
            foreach (var geometry in geometries)
            {
                this.Add(geometry);
            }
        }

        /// <summary>
        /// Returns the geometry at the given idx.
        /// </summary>
        /// <returns></returns>
        public GeometryType this[int idx]
        {
            get
            {
                return _geometries[idx];
            }
        }

        /// <summary>
        /// Returns the smallest bounding box containing all geometries in this collection.
        /// </summary>
        public override GeoCoordinateBox Box
        {
            get
            {
                GeoCoordinateBox box = null;
                foreach (var geometry in _geometries)
                {
                    if (box == null)
                    {
                        box = geometry.Box;
                    }
                    else
                    {
                        box = box + geometry.Box;
                    }
                }
                return box;
            }
        }

        /// <summary>
        /// Returns true if at least one of the geometries in this collection exists inside the given boundingbox.
        /// </summary>
        /// <returns></returns>
        public override bool IsInside(GeoCoordinateBox box)
        {
            foreach (var geometry in _geometries)
            {
                if (geometry.IsInside(box))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes all items from this collection.
        /// </summary>
        public void Clear()
        {
            _geometries.Clear();
        }

        #region IEnumerable<Geometry> Implementation

        /// <summary>
        /// Returns an enumerator that iterates through the geometry collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<GeometryType> GetEnumerator()
        {
            return _geometries.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the geometry collection.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _geometries.GetEnumerator();
        }

        #endregion
    }
}