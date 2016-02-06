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

using OsmSharp.Complete;
using OsmSharp.Streams.Complete;
using System.Linq;

namespace OsmSharp.Db
{
    /// <summary>
    /// Contains extensions for the snapshot db.
    /// </summary>
    public static class ISnapshotDbExtensions
    {

        /// <summary>
        /// Returns true if the node with the given id exists.
        /// </summary>
        public static bool ExistsNode(this ISnapshotDb db, long id)
        {
            return db.Exists(OsmGeoType.Node, id);
        }

        /// <summary>
        /// Deletes the node with the given id and returns true if it existed.
        /// </summary>
        public static bool DeleteNode(this ISnapshotDb db, long id)
        {
            return db.Delete(OsmGeoType.Node, id);
        }

        /// <summary>
        /// Returns true if the way with the given id exists.
        /// </summary>
        public static bool ExistsWay(this ISnapshotDb db, long id)
        {
            return db.Exists(OsmGeoType.Way, id);
        }

        /// <summary>
        /// Deletes the way with the given id and returns true if it existed.
        /// </summary>
        public static bool DeleteWay(this ISnapshotDb db, long id)
        {
            return db.Delete(OsmGeoType.Way, id);
        }

        /// <summary>
        /// Returns true if the relation with the given id exists.
        /// </summary>
        public static bool ExistsRelation(this ISnapshotDb db, long id)
        {
            return db.Exists(OsmGeoType.Relation, id);
        }

        /// <summary>
        /// Deletes the relation with the given id and returns true if it existed.
        /// </summary>
        public static bool DeleteRelation(this ISnapshotDb db, long id)
        {
            return db.Delete(OsmGeoType.Relation, id);
        }

        /// <summary>
        /// Gets all data in the form of a complete stream.
        /// </summary>
        public static OsmCompleteStreamSource GetComplete(this ISnapshotDb db)
        {
            return new Streams.Complete.OsmCompleteEnumerableStreamSource(
                db.Get().Select(x => x.CreateComplete(db)));
        }
    }
}