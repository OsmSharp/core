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

using System;
using System.Collections.Generic;

namespace OsmSharp.Collections.Indexes
{
    /// <summary>
    /// An in-memory implementation of an index of objects linked to a unique id.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Index<T> : IndexBase<T>
    {
        /// <summary>
        /// Holds all the objects in this index.
        /// </summary>
        private List<T> _objects;

        /// <summary>
        /// Creates a new empty index.
        /// </summary>
        public Index()
        {
            _objects = new List<T>();
        }

        /// <summary>
        /// Returns true if this index is readonly.
        /// </summary>
        public override bool IsReadonly
        {
            get { return false; }
        }

        /// <summary>
        /// Returns true if this index is serializable.
        /// </summary>
        public override bool IsSerializable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Tries to get and object from this index.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public override bool TryGet(long id, out T element)
        {
            if(id < _objects.Count)
            {
                element = _objects[(int)id];
                return true;
            }
            element = default(T);
            return false;
        }

        /// <summary>
        /// Returns the object with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override T Get(long id)
        {
            return _objects[(int)id];
        }

        /// <summary>
        /// Adds a new object to this index.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public override long Add(T element)
        {
            int id = _objects.Count;
            _objects.Add(element);
            return id;
        }

        /// <summary>
        /// Serializes this index to the given stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public override long Serialize(System.IO.Stream stream)
        {
            throw new InvalidOperationException("This index is not serializable, check IsSerializable.");
        }
    }
}