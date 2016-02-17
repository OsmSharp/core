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

using System.Linq;
using OsmSharp.Streams.Complete;
using OsmSharp.Complete;
using System.Collections.Generic;
using System;

namespace OsmSharp.Db
{
    /// <summary>
    /// Contains extensions for the history db.
    /// </summary>
    public static class IHistoryDbExtensions
    {
        /// <summary>
        /// Gets all osm objects with the given types and the given id's.
        /// </summary>
        public static IList<OsmGeo> Get(this ISnapshotDb db, IList<OsmGeoType> type, IList<long> id)
        {
            if (type == null) { throw new ArgumentNullException("type"); }
            if (id == null) { throw new ArgumentNullException("id"); }
            if (id.Count != type.Count) { throw new ArgumentException("Type and id lists need to have the same size."); }

            var result = new List<OsmGeo>();
            for (int i = 0; i < id.Count; i++)
            {
                result.Add(db.Get(type[i], id[i]));
            }
            return result;
        }

        /// <summary>
        /// Gets all osm objects with the given types and the given id's and versions.
        /// </summary>
        public static IList<OsmGeo> Get(this IHistoryDb db, IList<OsmGeoType> type, IList<long> ids, IList<int> versions)
        {
            if (type == null) { throw new ArgumentNullException("type"); }
            if (ids == null) { throw new ArgumentNullException("id"); }
            if (ids.Count != type.Count) { throw new ArgumentException("Type and id lists need to have the same size."); }
            if (versions.Count != type.Count) { throw new ArgumentException("Type and version lists need to have the same size."); }

            var result = new List<OsmGeo>();
            for (int i = 0; i < ids.Count; i++)
            {
                result.Add(db.Get(type[i], ids[i], versions[i]));
            }
            return result;
        }

        /// <summary>
        /// Gets all data in the form of a complete stream.
        /// </summary>
        public static OsmCompleteStreamSource GetComplete(this IHistoryDb db)
        {
            return new Streams.Complete.OsmCompleteEnumerableStreamSource(
                db.Get().Select(x => x.CreateComplete(db)));
        }
    }
}