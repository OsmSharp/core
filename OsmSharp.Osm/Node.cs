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

using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;

namespace OsmSharp.Osm
{
    /// <summary>
    /// Represents a simple node.
    /// </summary>
    public class Node : OsmGeo, ICompleteOsmGeo
    {
        /// <summary>
        /// Creates a new node.
        /// </summary>
        public Node()
        {
            this.Type = OsmGeoType.Node;
        }

        /// <summary>
        /// Creates a new node.
        /// </summary>
        public Node(long id, double latitude, double longitude)
            : this()
        {
            this.Id = id;
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        /// <summary>
        /// Creates a new node.
        /// </summary>
        public Node(long id, double latitude, double longitude, TagsCollectionBase tags)
            : this()
        {
            this.Id = id;
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Tags = tags;
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
