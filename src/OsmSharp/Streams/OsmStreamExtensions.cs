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

using OsmSharp.Changesets;
using OsmSharp.Db;
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

        /// <summary>
        /// Merges the given sources into this source.
        /// </summary>
        public static OsmStreamSource Progress(this OsmStreamSource source)
        {
            var progress = new Streams.Filters.OsmStreamFilterProgress();
            progress.RegisterSource(source);
            return progress;
        }

        /// <summary>
        /// Merges the given sources into this source.
        /// </summary>
        public static OsmStreamSource Merge(this IEnumerable<OsmGeo> source, params OsmStreamSource[] sources)
        {
            return source.Merge(ConflictResolutionType.FirstStream, new OsmEnumerableStreamSource(source));
        }

        /// <summary>
        /// Merges the given sources into this source.
        /// </summary>
        public static OsmStreamSource Merge(this IEnumerable<OsmGeo> source, ConflictResolutionType resolutionType, params IEnumerable<OsmGeo>[] sources)
        {
            var merge = new OsmStreamFilterMerge(resolutionType);
            for(var i = 0; i < sources.Length; i++)
            {
                merge.RegisterSource(sources[i]);
            }
            return merge;
        }

        /// <summary>
        /// Filters nodes and keeps ways/relations that are relevant.
        /// </summary>
        public static OsmStreamSource FilterNodes(this IEnumerable<OsmGeo> source, Func<Node, bool> filter, bool completeWays = false)
        {
            var nodeFilter = new Filters.OsmStreamFilterNode(filter, completeWays);
            nodeFilter.RegisterSource(source);
            return nodeFilter;
        }

        /// <summary>
        /// Filters nodes using a bounding box and keeps ways/relations that are relevant.
        /// </summary>
        public static OsmStreamSource FilterBox(this IEnumerable<OsmGeo> source, float left, float top, float right, float bottom,
            bool completeWays = false)
        {
            return source.FilterNodes(x =>
            { // TODO: take into account the 180/-180 thing.
                return x.Longitude.Value >= left && x.Longitude < right &&
                    x.Latitude.Value >= bottom && x.Latitude < top;
            }, completeWays);
        }

        /// <summary>
        /// Converts the given source to a complete stream.
        /// </summary>
        public static Complete.OsmCompleteStreamSource ToComplete(this IEnumerable<OsmGeo> source)
        {
            return new OsmSharp.Streams.Complete.OsmSimpleCompleteStreamSource(
                    new OsmSharp.Streams.OsmEnumerableStreamSource(source));
        }

        /// <summary>
        /// Converts the given source to a complete stream.
        /// </summary>
        public static Complete.OsmCompleteStreamSource ToComplete(this IEnumerable<OsmGeo> source, ISnapshotDb cache)
        {
            return new OsmSharp.Streams.Complete.OsmSimpleCompleteStreamSource(
                    new OsmSharp.Streams.OsmEnumerableStreamSource(source), cache);
        }

        /// <summary>
        /// Shows progress when consuming the returned stream.
        /// </summary>
        public static OsmStreamSource ShowProgress(this IEnumerable<OsmGeo> source)
        {
            var progress = new Filters.OsmStreamFilterProgress();
            progress.RegisterSource(source);
            return progress;
        }

        /// <summary>
        /// Applies the given changes to the given stream.
        /// </summary>
        public static OsmStreamSource ApplyChanges(this IEnumerable<OsmGeo> source, params OsmChange[] osmChange)
        {
            var filter = new OsmStreamFilterApplyChangeset(osmChange);
            filter.RegisterSource(source);
            return filter;
        }
    }
}