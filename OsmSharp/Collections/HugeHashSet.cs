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

using System;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Collections
{
    /// <summary>
    /// A hashset working around the pre .NET 4.5 memory limitations for one object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HugeHashSet<T> : ISet<T>
    {
        /// <summary>
        /// Holds the list of internal set.
        /// </summary>
        private List<HashSet<T>> _set;

        /// <summary>
        /// Holds the maximum size of one individual set.
        /// </summary>
        private const int _MAX_SET_SIZE = 1000000;

        /// <summary>
        /// Creates a new huge dictionary.
        /// </summary>
        public HugeHashSet()
        {
            _set = new List<HashSet<T>>();
            _set.Add(new HashSet<T>());
        }

        /// <summary>
        /// Returns the count of the internal dictionaries.
        /// </summary>
        public int CountSets
        {
            get
            {
                return _set.Count;
            }
        }

        /// <summary>
        /// Returns false.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Adds a new item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Add(T item)
        {
            HashSet<T> setWithRoom = null;
            for (int idx = 0; idx < _set.Count; idx++)
            {
                if (_set[idx].Contains(item))
                {
                    return true;
                }
                if (_set[idx].Count < _MAX_SET_SIZE)
                {
                    setWithRoom = _set[idx];
                }
            }
            if (setWithRoom == null)
            { // add another set.
                setWithRoom = new HashSet<T>();
                _set.Add(setWithRoom);
            }
            return setWithRoom.Add(item);
        }

        #region ISet<T> Members

        /// <summary>
        /// Removes all elements in the specified collection.
        /// </summary>
        /// <param name="other"></param>
        public void ExceptWith(IEnumerable<T> other)
        {
            foreach (T item in other)
            {
                this.Remove(item);
            }
        }

        /// <summary>
        /// Keeps only the elements that are in the given collection and in this set.
        /// </summary>
        /// <param name="other"></param>
        public void IntersectWith(IEnumerable<T> other)
        {
            // calculate intersection.
            var intersection = new HashSet<T>();
            foreach (T item in other)
            {
                if (this.Contains(item))
                {
                    intersection.Add(item);
                }
            }

            // clear this set.
            this.Clear();
            this.UnionWith(intersection);
        }

        /// <summary>
        /// Returns true if all elements in the given collection are contained in this set.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            // TODO: very naive implementation for now!
            var other_set = new HashSet<T>(other);
            foreach (T item in this)
            {
                if (!other_set.Contains(item))
                {
                    return false;
                }
            }
            // make sure this is proper.
            foreach (T item in other)
            {
                if (!this.Contains(item))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true if all elements in this set are also in the given collection. 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            foreach (T item in other)
            {
                if (!this.Contains(item))
                {
                    return false;
                }
            }
            // make sure this is proper.
            // TODO: very naive implementation for now!
            var other_set = new HashSet<T>(other);
            foreach (T item in this)
            {
                if (!other_set.Contains(item))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsSubsetOf(IEnumerable<T> other)
        {
            // TODO: very naive implementation for now!
            var other_set = new HashSet<T>(other);
            foreach (T item in this)
            {
                if (!other_set.Contains(item))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsSupersetOf(IEnumerable<T> other)
        {
            foreach (T item in other)
            {
                if (!this.Contains(item))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns true if this collection and the other share at least one element.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Overlaps(IEnumerable<T> other)
        {
            foreach (T item in other)
            {
                if (this.Contains(item))
                {
                    return true;
                }
            }
            // TODO: very naive implementation for now!
            var other_set = new HashSet<T>(other);
            foreach (T item in this)
            {
                if (other_set.Contains(item))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true if the two sets contain the same elements.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool SetEquals(IEnumerable<T> other)
        {
            var other_set = new HashSet<T>(other);
            foreach (T item in this)
            {
                other_set.Remove(item);
            }
            return other_set.Count == 0;
        }

        /// <summary>
        /// Keeps only items that are in one set or the other but not in both.
        /// </summary>
        /// <param name="other"></param>
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            foreach (T item in this.Intersect(other))
            {
                this.Remove(item);
            }
        }

        /// <summary>
        /// Adds all items in other too.
        /// </summary>
        /// <param name="other"></param>
        public void UnionWith(IEnumerable<T> other)
        {
            foreach (T item in other)
            {
                if (!this.Contains(item))
                {
                    this.Add(item);
                }
            }
        }

        #endregion

        /// <summary>
        /// Adds the item to this collection.
        /// </summary>
        /// <param name="item"></param>
        void ICollection<T>.Add(T item)
        {
            this.Add(item);
        }

        /// <summary>
        /// Clears all elements from this set.
        /// </summary>
        public void Clear()
        {
            _set.Clear();
            _set.Add(new HashSet<T>());
        }

        /// <summary>
        /// Returns true if this item is in this set.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            for (int idx = 0; idx < _set.Count; idx++)
            {
                if (_set[idx].Contains(item))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Copies the elements of this collection to the given array.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (T element in this)
            {
                array[arrayIndex] = element;
                arrayIndex++;
            }
        }

        /// <summary>
        /// Returns the number of elements.
        /// </summary>
        public int Count
        {
            get
            {
                int count = 0;
                for (int idx = 0; idx < _set.Count; idx++)
                {
                    count = count + _set[idx].Count;
                }
                return count;
            }
        }

        /// <summary>
        /// Removes the given item from this set.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            for (int idx = 0; idx < _set.Count; idx++)
            {
                if (_set[idx].Remove(item))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            IEnumerable<T> enumerable = _set[0];
            for (int idx = 1; idx < _set.Count; idx++)
            {
                enumerable = enumerable.Concat(_set[idx]);
            }
            return enumerable.GetEnumerator();
        }

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            IEnumerable<T> enumerable = _set[0];
            for (int idx = 1; idx < _set.Count; idx++)
            {
                enumerable = enumerable.Concat(_set[idx]);
            }
            return enumerable.GetEnumerator();
        }
    }
}