// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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

using OsmSharp.Collections.SpatialIndexes;
using OsmSharp.Math.Primitives;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Test.Collections.SpatialIndexes
{
    /// <summary>
    /// Reference implementation of a spatial index.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class ReferenceImplementation<T> : ISpatialIndex<T>
    {
        /// <summary>
        /// Holds the list of objects.
        /// </summary>
		private readonly List<KeyValuePair<BoxF2D, T>> _list;

        /// <summary>
        /// Creates a new reference spatial implementation.
        /// </summary>
        public ReferenceImplementation()
        {
			_list = new List<KeyValuePair<BoxF2D, T>>();
        }

        /// <summary>
        /// Queries this index and returns all objects with overlapping bounding boxes.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public IEnumerable<T> Get(BoxF2D box)
        {
            var result = new HashSet<T>();
            foreach (var entry in _list)
            {
                if (entry.Key.Overlaps(box))
                {
                    result.Add(entry.Value);
                }
            }
            return result;
        }

        /// <summary>
        /// Cancels the current get-request.
        /// </summary>
        public void GetCancel()
        {

        }

        /// <summary>
        /// Adds a new item with the corresponding box.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="item"></param>
		public void Add(BoxF2D box, T item)
        {
			_list.Add(new KeyValuePair<BoxF2D, T>(box, item));
        }

        /// <summary>
        /// Returns the count.
        /// </summary>
        public int Count()
        {
            return _list.Count;
        }

        /// <summary>
        /// Removes the given item.
        /// </summary>
        /// <param name="item"></param>
        public void Remove(T item)
        {
            for (int idx = 0; idx < _list.Count; idx++)
            {
                if (_list[idx].Value.Equals(item))
                {
                    _list.RemoveAt(idx);
                    return;
                }
            }
        }

        /// <summary>
        /// Removes the given item.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="item"></param>
        public void Remove(BoxF2D box, T item)
        {
            this.Remove(item);
        }


        public IEnumerator<T> GetEnumerator()
        {
            return _list.Select(x => x.Value).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.Select(x => x.Value).GetEnumerator();
        }
    }
}