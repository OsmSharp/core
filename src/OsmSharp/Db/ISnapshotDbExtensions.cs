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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Db
{
    /// <summary>
    /// Contains extensions for the snapshot db.
    /// </summary>
    public static class ISnapshotDbExtensions
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
        /// Deletes the object for the given key.
        /// </summary>
        public static void Delete(this ISnapshotDb db, OsmGeoKey key)
        {
            db.Delete(new OsmGeoKey[] { key });
        }

        /// <summary>
        /// Deletes the node with the given id.
        /// </summary>
        public static void DeleteNode(this ISnapshotDb db, long id)
        {
            db.Delete(new OsmGeoKey()
            {
                Id = id,
                Type = OsmGeoType.Node
            });
        }

        /// <summary>
        /// Deletes the way with the given id.
        /// </summary>
        public static void DeleteWay(this ISnapshotDb db, long id)
        {
            db.Delete(new OsmGeoKey()
            {
                Id = id,
                Type = OsmGeoType.Way
            });
        }

        /// <summary>
        /// Deletes the relation with the given id.
        /// </summary>
        public static void DeleteRelation(this ISnapshotDb db, long id)
        {
            db.Delete(new OsmGeoKey()
            {
                Id = id,
                Type = OsmGeoType.Relation
            });
        }

        /// <summary>
        /// Adds or objects the given object.
        /// </summary>
        public static void AddOrUpdate(this ISnapshotDb db, OsmGeo osmGeo)
        {
            db.AddOrUpdate(new OsmGeo[] { osmGeo });
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