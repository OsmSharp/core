// The MIT License (MIT)

// Copyright (c) 2016 Ben Abelshausen

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using OsmSharp.Streams.Complete;

namespace OsmSharp.Geo.Streams.Features.Interpreted
{
    /// <summary>
    /// A feature stream based on an osm complete stream and a feature interpreter.
    /// </summary>
    public class InterpretedFeatureStreamSource : IFeatureStreamSource
    {
        private readonly FeatureInterpreter _interpreter;
        private readonly OsmCompleteStreamSource _source;

        /// <summary>
        /// Creates a new feature stream source.
        /// </summary>
        public InterpretedFeatureStreamSource(OsmCompleteStreamSource source, FeatureInterpreter interpreter)
        {
            _source = source;
            _interpreter = interpreter;
        }

        private List<IFeature> _currentFeatures;
        private int _currentFeatureIndex = -1;

        /// <summary>
        /// Gets the current feature.
        /// </summary>
        public IFeature Current
        {
            get
            {
                if (_currentFeatures == null ||
                    _currentFeatureIndex >= _currentFeatures.Count ||
                    _currentFeatureIndex < 0)
                {
                    throw new InvalidOperationException("No current feature available, use MoveNext(), returns true when there is an object available.");
                }
                return _currentFeatures[_currentFeatureIndex];
            }
        }

        /// <summary>
        /// Returns true if this source has bounds.
        /// </summary>
        public bool HasBounds
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        object IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }

        /// <summary>
        /// Returns true if this stream can be reset.
        /// </summary>
        /// <returns></returns>
        public bool CanReset()
        {
            return _source.CanReset;
        }

        /// <summary>
        /// Closes this stream.
        /// </summary>
        public void Close()
        {
            _currentFeatures = new List<IFeature>();
            _currentFeatureIndex = -1;
        }

        /// <summary>
        /// Disposes of all native resource associated with this stream.
        /// </summary>
        public void Dispose()
        {

        }

        /// <summary>
        /// Returns a bounding box if available.
        /// </summary>
        /// <returns></returns>
        public Envelope GetBounds()
        {
            throw new InvalidOperationException("No bounds available, check HasBounds.");
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IFeature> GetEnumerator()
        {
            return this;
        }

        /// <summary>
        /// Initializes this stream.
        /// </summary>
        public void Initialize()
        {
            this.Reset();
        }

        /// <summary>
        /// Moves to the next object.
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            _currentFeatureIndex++;
            if (_currentFeatures != null &&
                _currentFeatureIndex < _currentFeatures.Count)
            { // moved to the next object in the current features list.
                return true;
            }
            
            while(true)
            {
                if (!_source.MoveNext())
                {
                    return false;
                }
                var next = _source.Current();
                var nextFeatures = _interpreter.Interpret(next);
                _currentFeatures = new List<IFeature>(nextFeatures);
                _currentFeatureIndex = 0;

                if(_currentFeatures.Count > 0)
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Resets this stream.
        /// </summary>
        public void Reset()
        {
            _source.Reset();
            
            _currentFeatures = new List<IFeature>();
            _currentFeatureIndex = -1;
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }
    }
}