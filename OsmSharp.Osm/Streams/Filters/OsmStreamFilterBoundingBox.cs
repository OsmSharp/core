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

using System;
using System.Collections.Generic;
using OsmSharp.Math.Geo;
using OsmSharp.Osm;
using OsmSharp.Collections.LongIndex.LongIndex;

namespace OsmSharp.Osm.Streams.Filters
{
    /// <summary>
    /// An OSM filter bounding box.
    /// </summary>
    public class OsmStreamFilterBoundingBox : OsmStreamFilter
    {
        /// <summary>
        /// Holds the current type.
        /// </summary>
        private OsmGeoType _currentType = OsmGeoType.Node;

        /// <summary>
        /// Flag for extra mode.
        /// </summary>
        private bool _includeExtraMode = false;

        /// <summary>
        /// The box to filter against.
        /// </summary>
        private readonly GeoCoordinateBox _box;

        /// <summary>
        /// An index of the actual nodes inside the bounding box.
        /// </summary>
        private readonly LongIndex _nodesIn = 
            new  LongIndex();

        /// <summary>
        /// An index of the actual ways inside the bounding box.
        /// </summary>
        private readonly HashSet<long> _waysIn = new HashSet<long>();

        /// <summary>
        /// An index of the actual relations inside the bounding box.
        /// </summary>
        private readonly HashSet<long> _relationIn = new HashSet<long>();

        /// <summary>
        /// An index of extra nodes to include.
        /// </summary>
        private readonly LongIndex _nodesToInclude = 
            new LongIndex();

        /// <summary>
        /// An index of extra ways to include.
        /// </summary>
        private readonly HashSet<long> _relationsToInclude = 
            new HashSet<long>();

        /// <summary>
        /// An index of extra relations to include.
        /// </summary>
        private readonly HashSet<long> _waysToInclude = 
            new HashSet<long>();

        /// <summary>
        /// Creates a new bounding box filter.
        /// </summary>
        public OsmStreamFilterBoundingBox(GeoCoordinateBox box)
            : base()
        {
            if (box == null) { throw new ArgumentNullException("box"); }

            _box = box;

            this.Meta.Add("bbox", _box.ToInvariantString());
        }

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
            if (!_includeExtraMode)
            { // just go over all nodes and ways.
                if (this.Source.MoveNext())
                {
                    bool finished = false;
                    bool isIn = false;

                    // move to the next object of the current type.
                    while (this.Current().Type != _currentType)
                    {
                        if (!this.Source.MoveNext())
                        {
                            finished = true;
                            break;
                        }
                    }

                    if (!finished)
                    {
                        while (this.Current().Type == _currentType && !isIn)
                        {
                            OsmGeo current = this.Source.Current();
                            isIn = this.IsInBB(current); // check and keep the extras.
                            if (isIn)
                            {
                                // add to the actual in-boundingbox indexes.
                                switch (current.Type)
                                {
                                    case OsmGeoType.Node:
                                        _nodesIn.Add(current.Id.Value);
                                        break;
                                    case OsmGeoType.Way:
                                        _waysIn.Add(current.Id.Value);
                                        break;
                                    case OsmGeoType.Relation:
                                        _relationIn.Add(current.Id.Value);
                                        break;
                                }
                                break;
                            }

                            // move to the next object of the current type.
                            if (!this.Source.MoveNext())
                            {
                                finished = true;
                                break;
                            }
                            while (this.Current().Type != _currentType)
                            {
                                if (!this.Source.MoveNext())
                                {
                                    finished = true;
                                    break;
                                }
                            }

                            // stop when finished.
                            if (finished)
                            {
                                break;
                            }
                        }
                    }

                    if (!finished && this.Current().Type == _currentType)
                    { // nothing was finished and the types match.
                        return true;
                    }
                }

                // switch to the next mode.
                switch (_currentType)
                {
                    case OsmGeoType.Node:
                        this.Source.Reset();
                        _currentType = OsmGeoType.Way;
                        return this.MoveNext();
                    case OsmGeoType.Way:
                        this.Source.Reset();
                        _currentType = OsmGeoType.Relation;
                        return this.MoveNext();
                    case OsmGeoType.Relation:
                        this.Source.Reset();
                        _includeExtraMode = true;
                        return this.MoveNext();
                }
                throw new InvalidOperationException("Unkown SimpleOsmGeoType");
            }
            else
            {
                while (this.Source.MoveNext())
                {
                    switch (this.Source.Current().Type)
                    {
                        case OsmGeoType.Node:
                            if (_nodesToInclude.Contains(this.Source.Current().Id.Value))
                            {
                                if (!_nodesIn.Contains(this.Source.Current().Id.Value))
                                {
                                    return true;
                                }
                            }
                            break;
                        case OsmGeoType.Way:
                            if (_waysToInclude.Contains(this.Source.Current().Id.Value))
                            {
                                if (!_waysIn.Contains(this.Source.Current().Id.Value))
                                {
                                    return true;
                                }
                            }
                            break;
                        case OsmGeoType.Relation:
                            if (_relationsToInclude.Contains(this.Source.Current().Id.Value))
                            {
                                if (!_relationIn.Contains(this.Source.Current().Id.Value))
                                {
                                    return true;
                                }
                            }
                            break;
                    }
                }
                return false;
            }
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
            _waysIn.Clear();
            _nodesIn.Clear();
            _currentType = OsmGeoType.Node;
            _includeExtraMode = false;
            this.Source.Reset();
        }

        /// <summary>
        /// Returns true if this filter can reset.
        /// </summary>
        public override bool CanReset
        {
            get { return this.Source.CanReset; }
        }

        /// <summary>
        /// Holds the relations considered already.
        /// </summary>
        readonly HashSet<long> _relationsConsidered = new HashSet<long>();

        /// <summary>
        /// Returns true if the given object is relevant in the bounding box.
        /// </summary>
        /// <param name="osmGeo"></param>
        /// <returns></returns>
        private bool IsInBB(OsmGeo osmGeo)
        {
            bool isIn = false;
            switch (osmGeo.Type)
            {
                case OsmGeoType.Node:
                    isIn = _box.Contains(new GeoCoordinate(
                        (osmGeo as Node).Latitude.Value,
                        (osmGeo as Node).Longitude.Value));
                    break;
                case OsmGeoType.Way:
                    foreach (long nodeId in (osmGeo as Way).Nodes)
                    {
                        if (_nodesIn.Contains(nodeId))
                        {
                            isIn = true;
                            break;
                        }
                    }
                    if (isIn)
                    {                   
                        foreach (long nodeId in (osmGeo as Way).Nodes)
                        {
                            _nodesToInclude.Add(nodeId);
                        }
                    }
                    break;
                case OsmGeoType.Relation:
                    if (!_relationsConsidered.Contains(osmGeo.Id.Value))
                    {
                        foreach (RelationMember member in (osmGeo as Relation).Members)
                        {
                            switch (member.MemberType.Value)
                            {
                                case OsmGeoType.Node:
                                    if (_nodesIn.Contains(member.MemberId.Value))
                                    {
                                        isIn = true;
                                        break;
                                    }
                                    break;
                                case OsmGeoType.Way:
                                    if (_waysIn.Contains(member.MemberId.Value))
                                    {
                                        isIn = true;
                                        break;
                                    }
                                    break;
                                case OsmGeoType.Relation:
                                    if (_relationIn.Contains(member.MemberId.Value))
                                    {
                                        isIn = true;
                                        break;
                                    }
                                    break;
                            }
                        }

                        if (isIn)
                        {
                            foreach (RelationMember member in (osmGeo as Relation).Members)
                            {
                                switch (member.MemberType.Value)
                                {
                                    case OsmGeoType.Node:
                                        _nodesToInclude.Add(member.MemberId.Value);
                                        break;
                                    case OsmGeoType.Way:
                                        _waysToInclude.Add(member.MemberId.Value);
                                        break;
                                    case OsmGeoType.Relation:
                                        _relationsToInclude.Add(member.MemberId.Value);
                                        break;
                                }
                            }
                        }
                    }
                    break;
            }
            return isIn;
        }
    }
}