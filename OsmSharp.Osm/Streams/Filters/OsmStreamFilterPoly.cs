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

using OsmSharp.Collections.LongIndex.LongIndex;
using OsmSharp.Math.Geo;
using System;
using System.Collections.Generic;

namespace OsmSharp.Osm.Streams.Filters
{
    /// <summary>
    /// A filter to filter OSM-data by polygon.
    /// </summary>
    public class OsmStreamFilterPoly : OsmStreamFilter
    {
        private readonly OsmSharp.Geo.Geometries.LineairRing _poly;
        private readonly GeoCoordinateBox _box;
        private readonly LongIndex _nodesIn = new LongIndex();
        private readonly LongIndex _waysIn = new LongIndex();

        /// <summary>
        /// Creates a new polygon filter.
        /// </summary>
        public OsmStreamFilterPoly(OsmSharp.Geo.Geometries.LineairRing poly)
            : base()
        {
            if (poly == null) { throw new ArgumentNullException("poly"); }

            _poly = poly;
            _box = new GeoCoordinateBox(poly.Coordinates);

            this.Meta.Add("poly", OsmSharp.Geo.Streams.GeoJson.GeoJsonConverter.ToGeoJson(_poly));
        }

        private OsmGeoType _currentType = OsmGeoType.Node;

        /// <summary>
        /// Initializes this filter.
        /// </summary>
        public override void Initialize()
        {
            this.Source.Initialize();
        }

        /// <summary>
        /// Move to the next item in the stream.
        /// </summary>
        /// <param name="ignoreNodes">Makes this source skip all nodes.</param>
        /// <param name="ignoreWays">Makes this source skip all ways.</param>
        /// <param name="ignoreRelations">Makes this source skip all relations.</param>
        /// <returns></returns>
        public override bool MoveNext(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
        {
            while (this.DoMoveNext())
            {
                if (this.Current().Type == OsmGeoType.Node &&
                    !ignoreNodes)
                { // there is a node and it is not to be ignored.
                    return true;
                }
                else if (this.Current().Type == OsmGeoType.Way &&
                        !ignoreWays)
                { // there is a way and it is not to be ignored.
                    return true;
                }
                else if (this.Current().Type == OsmGeoType.Relation &&
                        !ignoreRelations)
                { // there is a relation and it is not to be ignored.
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Moves to the next object.
        /// </summary>
        /// <returns></returns>
        private bool DoMoveNext()
        {
            while (this.Source.MoveNext())
            {
                // check the type and check if the source is sorted.
                var current = this.Source.Current();
                switch (current.Type)
                {
                    case OsmGeoType.Node:
                        if(_currentType != OsmGeoType.Node)
                        { // source is not sorted, a node appeared after a way/relation.
                            throw new OsmStreamNotSortedException("OsmStreamFilterPoly - Source stream is not sorted.");
                        }
                        var node = current as Node;
                        if(this.IsInsidePoly(node.Latitude.Value, node.Longitude.Value))
                        { // keep this node.
                            _nodesIn.Add(node.Id.Value);
                            return true;
                        }
                        break;
                    case OsmGeoType.Way:
                        if(_currentType == OsmGeoType.Relation)
                        { // source is not sorted, a way appeared after a relation.
                            throw new OsmStreamNotSortedException("OsmStreamFilterPoly - Source stream is not sorted.");
                        }
                        if(_currentType == OsmGeoType.Node)
                        { // switch type to way.
                            _currentType = OsmGeoType.Way;
                        }
                        var way = current as Way;
                        if(way.Nodes != null)
                        {
                            for(var i = 0; i < way.Nodes.Count; i++)
                            {
                                if(_nodesIn.Contains(way.Nodes[i]))
                                {
                                    _waysIn.Add(way.Id.Value);
                                    return true;
                                }
                            }
                        }
                        break;
                    case OsmGeoType.Relation:
                        if (_currentType == OsmGeoType.Way || _currentType == OsmGeoType.Node)
                        { // switch type to way.
                            _currentType = OsmGeoType.Relation;
                        }
                        var relation = current as Relation;
                        if(relation.Members != null)
                        {
                            for(var i = 0; i < relation.Members.Count; i++)
                            {
                                var member = relation.Members[i];
                                switch(member.MemberType.Value)
                                {
                                    case OsmGeoType.Node:
                                        if(_nodesIn.Contains(member.MemberId.Value))
                                        {
                                            return true;
                                        }
                                        break;
                                    case OsmGeoType.Way:
                                        if(_waysIn.Contains(member.MemberId.Value))
                                        {
                                            return true;
                                        }
                                        break;
                                }
                            }
                        }
                        break;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true if the coordinate is inside the polygon being filtered against.
        /// </summary>
        /// <returns></returns>
        private bool IsInsidePoly(double latitude, double longitude)
        {
            if(!_box.Contains(longitude, latitude))
            { // use the bounding box checks as a negative-first.
                return false;
            }
            return _poly.Contains(new GeoCoordinate(latitude, longitude));
        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        /// <returns></returns>
        public override OsmGeo Current()
        {
            return this.Source.Current();
        }

        /// <summary>
        /// Resets this filter.
        /// </summary>
        public override void Reset()
        {
            _nodesIn.Clear();
            _currentType = OsmGeoType.Node;
            this.Source.Reset();
        }

        /// <summary>
        /// Returns true if this filter can reset.
        /// </summary>
        public override bool CanReset
        {
            get { return this.Source.CanReset; }
        }
    }
}