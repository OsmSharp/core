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

using GeoAPI.Geometries;
using NetTopologySuite.Features;
using System.Collections.Generic;

namespace OsmSharp.Geo.Streams
{
    /// <summary>
    /// Represents a streamed source of features.
    /// </summary>
    public interface IFeatureStreamSource : IEnumerator<Feature>, IEnumerable<Feature>
    {
        /// <summary>
        /// Intializes this source.
        /// </summary>
        /// <rremarks>Has to be called before starting read objects.</rremarks>
        void Initialize();

        /// <summary>
        /// Returns true if this source can be reset.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Some sources cannot be reset, live feeds of objects for example.</remarks>
        bool CanReset();

        /// <summary>
        /// Closes this target.
        /// </summary>
        /// <remarks>Closes any open connections, file locks or anything related to this source.</remarks>
        void Close();

        /// <summary>
        /// Returns true if this source is bounded.
        /// </summary>
        bool HasBounds
        {
            get;
        }

        /// <summary>
        /// Returns the bounds of this source.
        /// </summary>
        /// <returns></returns>
        Envelope GetBounds();
    }
}
