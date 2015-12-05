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

namespace OsmSharp.Osm.Streams.Filters
{
    /// <summary>
    /// A filter that raizes events for each object.
    /// </summary>
    public class OsmStreamFilterWithEvents : OsmStreamFilter
    {
        /// <summary>
        /// Holds the parameters object sent with the events.
        /// </summary>
        private readonly object _param;

        /// <summary>
        /// Creates a new filter with events.
        /// </summary>
        public OsmStreamFilterWithEvents()
        {
            _param = null;
        }

        /// <summary>
        /// Creates a new filter with events.
        /// </summary>
        /// <param name="param"></param>
        public OsmStreamFilterWithEvents(object param)
        {
            _param = param;
        }

        private OsmGeo _current = null;

        /// <summary>
        /// An empty delegate.
        /// </summary>
        public delegate void EmptyDelegate();

        /// <summary>
        /// Event raised when filter is initialized.
        /// </summary>
        public event EmptyDelegate InitializeEvent;

        /// <summary>
        /// Initializes this filter.
        /// </summary>
        public override void Initialize()
        {
            if (this.InitializeEvent != null)
            {
                this.InitializeEvent();
            }

            this.Source.Initialize();
        }

        /// <summary>
        /// A delegate with a simple osm geo object as parameter.
        /// </summary>
        public delegate OsmGeo SimpleOsmGeoDelegate(OsmGeo osmGeo, object param);

        /// <summary>
        /// Event raised when the move is made to the next object.
        /// </summary>
        public event SimpleOsmGeoDelegate MovedToNextEvent;

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
        /// Moves this filter to the next object.
        /// </summary>
        /// <returns></returns>
        private bool DoMoveNext()
        {
            _current = null;
            while (_current == null)
            {
                if(!this.Source.MoveNext())
                { // source is finished.
                    return false;
                }
                _current = this.Source.Current();
                if (this.MovedToNextEvent != null)
                {
                    _current = this.MovedToNextEvent(_current, _param);
                }
            }
            return true;
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
    }
}