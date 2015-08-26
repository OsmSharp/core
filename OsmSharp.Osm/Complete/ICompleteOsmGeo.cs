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

using OsmSharp.Collections.Tags;
using OsmSharp.Geo.Features;
using OsmSharp.Math.Geo;

namespace OsmSharp.Osm
{
    /// <summary>
    /// An abstract representation of a complete OsmGeo object.
    /// </summary>
    public interface ICompleteOsmGeo
    {
        /// <summary>
        /// Gets the bounding box.
        /// </summary>
        GeoCoordinateBox BoundingBox { get; }

        /// <summary>
        /// Gets or sets the changeset id.
        /// </summary>
        long? ChangeSetId { get; set; }

        /// <summary>
        /// Gets the interpreted feature collection.
        /// </summary>
        FeatureCollection Features { get; }

        /// <summary>
        /// Resets the current features for this object.
        /// </summary>
        void ResetFeatures();

        /// <summary>
        /// Converts this object to it's corresponding simple version.
        /// </summary>
        /// <returns></returns>
        OsmSharp.Osm.OsmGeo ToSimple();

        /// <summary>
        /// Gets or sets the visible flag.
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// The id of this object.
        /// </summary>
        long Id { get; }

        /// <summary>
        /// Returns the type of osm data.
        /// </summary>
        CompleteOsmType Type { get;  }

        /// <summary>
        /// Returns the tags dictionary.
        /// </summary>
        TagsCollectionBase Tags { get; set; }
    }
}