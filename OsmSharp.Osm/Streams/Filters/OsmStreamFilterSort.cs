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
using OsmSharp.Osm;

namespace OsmSharp.Osm.Streams.Filters
{
    /// <summary>
    /// An OSM stream filter sort.
    /// </summary>
    public class OsmStreamFilterSort : OsmStreamFilter
    {
        /// <summary>
        /// The current type.
        /// </summary>
        private OsmGeoType _currentType = OsmGeoType.Node;

        /// <summary>
        /// Holds a flag indicating that the source is already sorted.
        /// </summary>
        private bool? _isSourceSorted = null;

        /// <summary>
        /// Holds the first way.
        /// </summary>
        private bool _firstWay = true;

        /// <summary>
        /// Holds the first relation.
        /// </summary>
        private bool _firstRelation = true;

        /// <summary>
        /// Initializes this filter.
        /// </summary>
        public override void Initialize()
        {
            if (this.Source == null)
            {
                throw new Exception("No target registered!");
            }
            // no intialisation this filter does the same thing every time.
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
            if(ignoreNodes && ignoreWays && !ignoreRelations)
            { // only one needs to be ignored.
                return this.Source.MoveNext(ignoreNodes, ignoreWays, ignoreRelations);
            }
            else if (ignoreNodes && !ignoreWays && ignoreRelations)
            { // only one needs to be ignored.
                return this.Source.MoveNext(ignoreNodes, ignoreWays, ignoreRelations);
            }
            else if (!ignoreNodes && ignoreWays && ignoreRelations)
            { // only one needs to be ignored.
                return this.Source.MoveNext(ignoreNodes, ignoreWays, ignoreRelations);
            }
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
        /// Move to the next object.
        /// </summary>
        /// <returns></returns>
        private bool DoMoveNext()
        {
            if (this.Source.IsSorted || (_isSourceSorted.HasValue && _isSourceSorted.Value))
            { // the source is already sorted.
                return this.Source.MoveNext();
            }
            else
            { // leave it to this filter to sort this.
                if (this.Source.MoveNext())
                { // make sure this object is of the correct type.
                    bool finished = false;
                    bool invalid = this.Current().Type != _currentType;
                    while (this.Current().Type != _currentType)
                    { // check if source is at the end.
                        if (!this.Source.MoveNext())
                        {
                            finished = true;
                            break;
                        }
                    }

                    if (invalid && !finished)
                    { // an object was found but first another one was found.
                        if (_currentType == OsmGeoType.Node)
                        { // ok, this source is definetly not sorted.
                            _isSourceSorted = false;
                        }
                        else if (_currentType == OsmGeoType.Way)
                        { // the current type is way.
                            if (!_firstWay)
                            { // a way after a relation.
                                _isSourceSorted = false;
                            }
                            _firstWay = false;
                        }
                        else if (_currentType == OsmGeoType.Relation)
                        { // the current type is relation.
                            if (!_firstRelation)
                            { // a way after a relation.
                                _isSourceSorted = false;
                            }
                            _firstRelation = false;
                        }
                    }

                    if (!finished && this.Current().Type == _currentType)
                    {
                        return true;
                    }
                }

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
                        if (!_isSourceSorted.HasValue)
                        { // no invalid order was found.
                            _isSourceSorted = true;
                        }
                        return false;
                }
                throw new InvalidOperationException("Unkown SimpleOsmGeoType");
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
        /// Returns true if this source is sorted.
        /// </summary>
        public override bool IsSorted
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Resets this filter.
        /// </summary>
        public override void Reset()
        {
            _currentType = OsmGeoType.Node;
            this.Source.Reset();
        }

        /// <summary>
        /// Returns true if this source can be reset.
        /// </summary>
        public override bool CanReset
        {
            get
            {
                return this.Source.CanReset;
            }
        }
    }
}