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

using OsmSharp.Collections;
using OsmSharp.Geo.Features;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Geo.Interpreter;

namespace OsmSharp.Osm
{
    /// <summary>
    /// Base class for all the osm data that represents an object on the map.
    /// 
    /// Nodes, Ways and Relations
    /// </summary>
    public abstract class CompleteOsmGeo : CompleteOsmBase, ICompleteOsmGeo
    {
        /// <summary>
        /// Creates a new OsmGeo object.
        /// </summary>
        /// <param name="id"></param>
        internal CompleteOsmGeo(long id)
            :base(id)
        {
            this.Visible = true;
            this.UserId = null;
            this.User = null;
        }

        /// <summary>
        /// Creates a new OsmGeo object with a string table.
        /// </summary>
        /// <param name="string_table"></param>
        /// <param name="id"></param>
        internal CompleteOsmGeo(ObjectTable<string> string_table, long id)
            : base(string_table, id)
        {
            this.Visible = true;
            this.UserId = null;
            this.User = null;
        }

        /// <summary>
        /// Converts this OsmGeo object to an OsmGeoSimple object.
        /// </summary>
        /// <returns></returns>
        public abstract OsmSharp.Osm.OsmGeo ToSimple();

        /// <summary>
        /// The bounding box of object.
        /// </summary>
        public override GeoCoordinateBox BoundingBox
        {
            get
            {
                return this.Features.Box;
            }
        }

        /// <summary>
        /// Gets/Sets the changeset id.
        /// </summary>
        public long? ChangeSetId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/Sets the visible flag.
        /// </summary>
        public bool Visible { get; set; }

        #region Features - Interpreter

        /// <summary>
        /// The interpreter for these objects.
        /// </summary>
        public static FeatureInterpreter FeatureInterperter = 
            new SimpleFeatureInterpreter();

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
        
    }
}