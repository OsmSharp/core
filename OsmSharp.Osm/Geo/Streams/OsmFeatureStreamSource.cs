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

using OsmSharp.Geo.Features;
using OsmSharp.Geo.Geometries.Streams;
using OsmSharp.Osm.Geo.Interpreter;
using OsmSharp.Osm.Streams.Complete;
using System.Collections.Generic;

namespace OsmSharp.Osm.Geo.Streams
{
    /// <summary>
    /// Represents a stream translating an OSM stream into a feature stream translating OSM-data into features.
    /// </summary>
    public class OsmFeatureStreamSource : IFeatureStreamSource
    {
        /// <summary>
        /// Holds the feature interpreter.
        /// </summary>
        private FeatureInterpreter _interpreter;

        /// <summary>
        /// Holds the OSM sourc stream.
        /// </summary>
        private OsmCompleteStreamSource _source;

        /// <summary>
        /// Creates a new OSM feature stream source.
        /// </summary>
        /// <param name="source">The OSM source stream.</param>
        public OsmFeatureStreamSource(OsmCompleteStreamSource source)
        {
            _source = source;
            _interpreter = new SimpleFeatureInterpreter();
        }

        /// <summary>
        /// Creates a new OSM feature stream source.
        /// </summary>
        /// <param name="source">The OSM source stream.</param>
        /// <param name="interpreter">The interpreter used to translate OSM-data into features.</param>
        public OsmFeatureStreamSource(OsmCompleteStreamSource source, FeatureInterpreter interpreter)
        {
            _source = source;
            _interpreter = interpreter;
        }

        /// <summary>
        /// Initializes this source.
        /// </summary>
        public void Initialize()
        {
            _source.Reset();
            _source.Initialize();
            _current = null;
        }

        /// <summary>
        /// Returns true if this source can be reset.
        /// </summary>
        /// <returns></returns>
        public bool CanReset()
        {
            return _source.CanReset;
        }

        /// <summary>
        /// Closes this source.
        /// </summary>
        public void Close()
        {
            _current = null;
        }

        /// <summary>
        /// Returns true if this source has bounds.
        /// </summary>
        public bool HasBounds
        {
            get { return false; }
        }

        /// <summary>
        /// Returns the bounding box of all data in this source.
        /// </summary>
        /// <returns></returns>
        public Math.Geo.GeoCoordinateBox GetBounds()
        {
            throw new System.InvalidOperationException("This source has no bounds, check HasBounds.");
        }

        /// <summary>
        /// Holds the current feature.
        /// </summary>
        private Feature _current;

        /// <summary>
        /// Holds the current enumerator.
        /// </summary>
        private IEnumerator<Feature> _currentEnumerator;

        /// <summary>
        /// Gets the current feature.
        /// </summary>
        public Feature Current
        {
            get { return _current; }
        }

        /// <summary>
        /// Gets the current feature.
        /// </summary>
        object System.Collections.IEnumerator.Current
        {
            get { return _current; }
        }

        /// <summary>
        /// Moves to the next feature.
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            if(_currentEnumerator != null &&
                _currentEnumerator.MoveNext())
            { // there is still a current enumerator.
                _current = _currentEnumerator.Current;
                return true;
            }
            _currentEnumerator = null;
            while(_source.MoveNext())
            {
                var features = _interpreter.Interpret(_source.Current());
                if (features != null)
                { // an object was succesfully converted.
                    _currentEnumerator = features.GetEnumerator();
                    if(_currentEnumerator.MoveNext())
                    { // move to first object in feature collection.
                        _current = _currentEnumerator.Current;
                        return true;
                    }
                    else
                    { // an empty feature collection, try next one.
                        _currentEnumerator.Dispose();
                        _currentEnumerator = null;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Resets this source.
        /// </summary>
        public void Reset()
        {
            _current = null;
            _source.Reset();
        }

        /// <summary>
        /// Disposes of all resources associated with this source.
        /// </summary>
        public void Dispose()
        {

        }

        /// <summary>
        /// Returns the enumerator for this source.
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.IEnumerator<OsmSharp.Geo.Features.Feature> GetEnumerator()
        {
            this.Reset();
            return this;
        }

        /// <summary>
        /// Returns the enumerator for this source.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            this.Reset();
            return this;
        }
    }
}
