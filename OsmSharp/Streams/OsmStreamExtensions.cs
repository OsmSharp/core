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

using OsmSharp.Streams.Filters;
using System;
using System.Collections.Generic;

namespace OsmSharp.Streams
{
    /// <summary>
    /// Contains extension methods related to the stream implementations.
    /// </summary>
    public static class OsmStreamExtensions
    {
        /// <summary>
        /// Registers a source on this target.
        /// </summary>
        public static void RegisterSource(this IOsmStreamTarget target, IEnumerable<OsmGeo> source)
        {
            target.RegisterSource(new OsmEnumerableStreamSource(source));
        }

        /// <summary>
        /// Registers a source but keeps only the objects that satify a given condition.
        /// </summary>
        public static void RegisterSource(this IOsmStreamTarget target, IEnumerable<OsmGeo> source, Func<OsmGeo, bool> keep)
        {
            var filter = new OsmStreamFilterDelegate();
            filter.RegisterSource(source);
            filter.MoveToNextEvent = (osmGeo, param) =>
            {
                if (keep(osmGeo))
                {
                    return osmGeo;
                }
                return null;
            };
            target.RegisterSource(filter);
        }

        /// <summary>
        /// Registers a source and also allows a given action on all objects coming from this source.
        /// </summary>
        public static void RegisterSource(this IOsmStreamTarget target, IEnumerable<OsmGeo> source, Action<OsmGeo> channel)
        {
            var filter = new OsmStreamFilterDelegate();
            filter.RegisterSource(source);
            filter.MoveToNextEvent = (osmGeo, param) =>
            {
                channel(osmGeo);
                return osmGeo;
            };
            target.RegisterSource(filter);
        }
    }
}