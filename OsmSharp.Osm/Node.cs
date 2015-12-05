// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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

using OsmSharp.Collections.Tags;
using OsmSharp.Geo.Features;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Geo.Interpreter;

namespace OsmSharp.Osm
{
    /// <summary>
    /// Represents a simple node.
    /// </summary>
    public class Node : OsmGeo, ICompleteOsmGeo
    {
        /// <summary>
        /// Creates a new simple node.
        /// </summary>
        public Node()
        {
            this.Type = OsmGeoType.Node;
        }

        /// <summary>
        /// The latitude.
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// The longitude.
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// Returns a description of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string tags = "{no tags}";
            if (this.Tags != null && this.Tags.Count > 0)
            {
                tags = this.Tags.ToString();
            }
            if (!this.Id.HasValue)
            {
                return string.Format("Node[null]{0}", tags);
            }
            return string.Format("Node[{0}]{1}", this.Id.Value, tags);
        }

        #region Construction Methods

        /// <summary>
        /// Creates a new node.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public static Node Create(long id, double latitude, double longitude)
        {
            Node node = new Node();
            node.Id = id;
            node.Latitude = latitude;
            node.Longitude = longitude;
            return node;
        }

        /// <summary>
        /// Creates a new node.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tags"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public static Node Create(long id, TagsCollectionBase tags, double latitude, double longitude)
        {
            Node node = new Node();
            node.Id = id;
            node.Latitude = latitude;
            node.Longitude = longitude;
            node.Tags = tags;
            return node;
        }

        #endregion

        /// <summary>
        /// The bounding box of object.
        /// </summary>
        public GeoCoordinateBox BoundingBox
        {
            get
            {
                return this.Features.Box;
            }
        }

        /// <summary>
        /// The coordinates of this node.
        /// </summary>
        public GeoCoordinate Coordinate
        {
            get
            {
                return new GeoCoordinate(this.Latitude.Value, this.Longitude.Value);
            }
        }

        #region Geometry - Interpreter

        /// <summary>
        /// The interpreter for these objects.
        /// </summary>
        public static FeatureInterpreter FeatureInterperter = new SimpleFeatureInterpreter(); // set a default feature interpreter.

        /// <summary>
        /// The feature(s) this OSM-object represents.
        /// </summary>
        private FeatureCollection _features;

        /// <summary>
        /// Returns the feature(s) this OSM-object represents.
        /// </summary>
        public FeatureCollection Features
        {
            get
            {
                if (_features == null)
                {
                    _features = CompleteOsmGeo.FeatureInterperter.Interpret(this);
                }
                return _features;
            }
        }

        /// <summary>
        /// Make sure the geometries of this objects will be recalculated.
        /// </summary>
        public void ResetFeatures()
        {
            _features = null;
        }

        #endregion

        /// <summary>
        /// Returns a simple version of this object.
        /// </summary>
        /// <returns></returns>
        public OsmGeo ToSimple()
        {
            return this;
        }

        /// <summary>
        /// Gets or sets the visible flag.
        /// </summary>
        bool ICompleteOsmGeo.Visible
        {
            get
            {
                return this.Visible.HasValue && this.Visible.Value;
            }
            set
            {
                this.Visible = value;
            }
        }

        /// <summary>
        /// Gets the id.
        /// </summary>
        long ICompleteOsmGeo.Id
        {
            get
            {
                return this.Id.Value;
            }
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        CompleteOsmType ICompleteOsmGeo.Type
        {
            get
            {
                return CompleteOsmType.Node;
            }
        }
    }
}
