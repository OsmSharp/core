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

namespace OsmSharp.Geo.Features
{
    /// <summary>
    /// Represents a collection of features.
    /// </summary>
    public class FeatureCollection : IEnumerable<Feature>
    {
        private readonly List<Feature> _features;

        /// <summary>
        /// Creates a new feature collection.
        /// </summary>
        public FeatureCollection()
        {
            _features = new List<Feature>();
        }

        /// <summary>
        /// Creates a new Feature collection.
        /// </summary>
        public FeatureCollection(IEnumerable<Feature> features)
        {
            _features = new List<Feature>(features);
        }

        /// <summary>
        /// Returns the object count.
        /// </summary>
        public int Count
        {
            get
            {
                return _features.Count;
            }
        }

        /// <summary>
        /// Adds a new feature.
        /// </summary>
        public void Add(Feature feature)
        {
            _features.Add(feature);
        }

        /// <summary>
        /// Adds all features in the given enumerable.
        /// </summary>
        public void AddRange(IEnumerable<Feature> features)
        {
            foreach (var feature in features)
            {
                this.Add(feature);
            }
        }

        /// <summary>
        /// Returns the feature at the given idx.
        /// </summary>
        /// <returns></returns>
        public Feature this[int idx]
        {
            get
            {
                return _features[idx];
            }
        }

        /// <summary>
        /// Returns the smallest bounding box containing all features in this collection.
        /// </summary>
        public GeoCoordinateBox Box
        {
            get
            {
                GeoCoordinateBox box = null;
                foreach (var feature in _features)
                {
                    if (box == null)
                    {
                        box = feature.Geometry.Box;
                    }
                    else
                    {
                        box = box + feature.Geometry.Box;
                    }
                }
                return box;
            }
        }

        /// <summary>
        /// Returns true if at least one of the features in this collection exists inside the given boundingbox.
        /// </summary>
        /// 
        /// <returns></returns>
        public bool IsInside(GeoCoordinateBox box)
        {
            foreach (Feature feature in _features)
            {
                if (feature.Geometry.IsInside(box))
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
            _features.Clear();
        }

        #region IEnumerable<Feature> Implementation

        /// <summary>
        /// Returns an enumerator that iterates through the Feature collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Feature> GetEnumerator()
        {
            return _features.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the Feature collection.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _features.GetEnumerator();
        }

        #endregion
    }
}