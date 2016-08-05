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

using OsmSharp.Db.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using OsmSharp.Changesets;

namespace OsmSharp.Db
{
    /// <summary>
    /// An internal class implementing a snapshot db.
    /// </summary>
    public class SnapshotDb : ISnapshotDb
    {
        private readonly ISnapshotDbImpl _db;

        /// <summary>
        /// Creates a new snapshot db.
        /// </summary>
        public SnapshotDb(ISnapshotDbImpl db)
        {
            _db = db;
        }

        /// <summary>
        /// Adds or updates.
        /// </summary>
        public void AddOrUpdate(IEnumerable<OsmGeo> osmGeos)
        {
            _db.AddOrUpdate(osmGeos);
        }

        /// <summary>
        /// Clears all data.
        /// </summary>
        public void Clear()
        {
            _db.Clear();
        }

        /// <summary>
        /// Deletes all given objects.
        /// </summary>
        public void Delete(IEnumerable<OsmGeoKey> keys)
        {
            _db.Delete(keys);
        }

        /// <summary>
        /// Gets all objects.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OsmGeo> Get()
        {
            return _db.Get();
        }

        /// <summary>
        /// Gets the object with the given type and given id.
        /// </summary>
        public OsmGeo Get(OsmGeoType type, long id)
        {
            var res = this.Get(new OsmGeoKey[] { new OsmGeoKey()
            {
                Id = id,
                Type = type
            }});

            return res.FirstOrDefault();
        }

        /// <summary>
        /// Returns all given objects.
        /// </summary>
        public IEnumerable<OsmGeo> Get(IEnumerable<OsmGeoKey> keys)
        {
            return _db.Get(keys);
        }

        /// <summary>
        /// Gets all objects within the given bounding box.
        /// </summary>
        public IEnumerable<OsmGeo> Get(float minLatitude, float minLongitude, float maxLatitude, float maxLongitude)
        {
            return _db.Get(minLatitude, minLongitude, maxLatitude, maxLongitude);
        }
        
        /// <summary>
        /// Applies the given changes.
        /// </summary>
        public void ApplyChangeset(OsmChange changeset)
        {
            if (changeset == null) { throw new ArgumentNullException("changeset"); }

            if (changeset.Delete != null)
            {
                this.Delete(changeset.Delete.Select(x => new OsmGeoKey()
                {
                    Id = x.Id.Value,
                    Type = x.Type
                }));
            }
            
            if (changeset.Modify != null)
            {
                this.AddOrUpdate(changeset.Modify);
            }

            if (changeset.Create != null)
            {
                this.AddOrUpdate(changeset.Create);
            }
        }
    }
}