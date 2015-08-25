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
    /// A data processor filter that filters objects by their tags.
    /// </summary>
    public class OsmStreamFilterTags : OsmStreamFilter
    {
        /// <summary>
        /// Holds the nodes filter.
        /// </summary>
        private readonly Osm.Filters.Filter _nodesFilter;

        /// <summary>
        /// Holds the ways filter.
        /// </summary>
        private readonly Osm.Filters.Filter _waysFilter;

        /// <summary>
        /// Keeps the nodes in the ways also.
        /// </summary>
        private readonly bool _wayKeepNodes;

        /// <summary>
        /// Holds the relations filter.
        /// </summary>
        private readonly Osm.Filters.Filter _relationsFilter;

        /// <summary>
        /// Keeps the objects in the relations also.
        /// </summary>
        private readonly bool _relationKeepObjects;

        /// <summary>
        /// Filters data according to the given filters.
        /// </summary>
        /// <param name="nodesFilter"></param>
        /// <param name="waysFilter"></param>
        /// <param name="relationsFilter"></param>
        public OsmStreamFilterTags(Osm.Filters.Filter nodesFilter, Osm.Filters.Filter waysFilter,
                                       Osm.Filters.Filter relationsFilter)
        {
            _nodesFilter = nodesFilter;
            _waysFilter = waysFilter;
            _relationsFilter = relationsFilter;

            _wayKeepNodes = false;
            _relationKeepObjects = false;
        }

        /// <summary>
        /// Initializes this filter.
        /// </summary>
        public override void Initialize()
        {

        }

        /// <summary>
        /// Holds the current object.
        /// </summary>
        private OsmGeo _current;

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
            if (!_relationKeepObjects && !_wayKeepNodes)
            {
                // simple here just filter!
                const bool filterOk = false;
                while (!filterOk)
                {
                    if (this.Source.MoveNext())
                    {
                        OsmGeo current = this.Source.Current();

                        switch (current.Type)
                        {
                            case OsmGeoType.Node:
                                if (_nodesFilter == null ||
                                    _nodesFilter.Evaluate(current))
                                {
                                    _current = current;
                                    return true;
                                }
                                break;
                            case OsmGeoType.Way:
                                if (_waysFilter == null ||
                                    _waysFilter.Evaluate(current))
                                {
                                    _current = current;
                                    return true;
                                }
                                break;
                            case OsmGeoType.Relation:
                                if (_relationsFilter == null ||
                                    _relationsFilter.Evaluate(current))
                                {
                                    _current = current;
                                    return true;
                                }
                                break;
                        }
                    }
                    else
                    { // there is no more data in the source!
                        return false;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        /// <returns></returns>
        public override OsmGeo Current()
        {
            return _current;
        }

        /// <summary>
        /// Resets this filter.
        /// </summary>
        public override void Reset()
        {
            _current = null;

            this.Source.Reset();
        }

        /// <summary>
        /// Returns true if this filter can be reset.
        /// </summary>
        public override bool CanReset
        {
            get { return this.Source.CanReset; }
        }

        /// <summary>
        /// Registers the reader of this filter.
        /// </summary>
        /// <param name="reader"></param>
        public override void RegisterSource(OsmStreamSource reader)
        {
            if (_wayKeepNodes || _relationKeepObjects)
            {
                if (!reader.CanReset)
                {
                    throw new ArgumentException("The tags source cannot be reset!",
                        "reader");
                }
            }

            base.RegisterSource(reader);
        }
    }
}