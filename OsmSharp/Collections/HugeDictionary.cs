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
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace OsmSharp.Collections
{
    /// <summary>
    /// A dictionary working around the pre .NET 4.5 memory limitations for one object.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class HugeDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        /// <summary>
        /// Holds the list of internal dictionaries.
        /// </summary>
        private readonly List<IDictionary<TKey, TValue>> _dictionary;
        private readonly int _maxDictionarySize = 1000000;

        /// <summary>
        /// Creates a new huge dictionary.
        /// </summary>
        public HugeDictionary()
        {
            _dictionary = new List<IDictionary<TKey, TValue>>();
            _dictionary.Add(new Dictionary<TKey, TValue>());
        }

        /// <summary>
        /// Creates a new huge dictionary.
        /// </summary>
        public HugeDictionary(int maxDictionarySize)
        {
            _maxDictionarySize = maxDictionarySize;

            _dictionary = new List<IDictionary<TKey, TValue>>();
            _dictionary.Add(new Dictionary<TKey, TValue>());
        }

        /// <summary>
        /// Adds a new element.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value)
        { // adds a new key-value pair.
            bool added = false;
            for (int idx = 0; idx < _dictionary.Count; idx++)
            {
                if (_dictionary[idx].ContainsKey(key))
                {
                    throw new System.ArgumentException("An element with the same key already exists in the System.Collections.Generic.IDictionary<TKey,TValue>.");
                }
                if (!added && _dictionary[idx].Count < _maxDictionarySize)
                { // add the key-values.
                    _dictionary[idx].Add(key, value);
                    added = true;
                }
            }
            if (!added)
            { // add the key-values.
                _dictionary.Add(new Dictionary<TKey, TValue>());
                _dictionary[_dictionary.Count - 1].Add(key, value);
            }
        }

        /// <summary>
        /// Returns true if contains the given key.
        /// </summary>
        public bool ContainsKey(TKey key)
        {
            for (int idx = 0; idx < _dictionary.Count; idx++)
            {
                if (_dictionary[idx].ContainsKey(key))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the collection of all the keys.
        /// </summary>
        public ICollection<TKey> Keys
        {
            get
            {
                var enumerables = new IEnumerable<TKey>[_dictionary.Count];
                for(var i = 0; i < enumerables.Length; i++)
                {
                    enumerables[i] = _dictionary[i].Keys;
                }
                return new ReadonlyEnumerableCollection<TKey>(enumerables);
            }
        }

        /// <summary>
        /// Removes an item from this dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(TKey key)
        {
            for (int idx = 0; idx < _dictionary.Count; idx++)
            {
                if (_dictionary[idx].Remove(key))
                {
                    if (_dictionary[idx].Count == 0 && _dictionary.Count > 1)
                    {
                        _dictionary.RemoveAt(idx);
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Tries getting a value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default(TValue);
            for (int idx = 0; idx < _dictionary.Count; idx++)
            {
                if (_dictionary[idx].TryGetValue(key, out value))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns a collection of all values.
        /// </summary>
        public ICollection<TValue> Values
        {
            get
            {
                var enumerables = new IEnumerable<TValue>[_dictionary.Count];
                for (var i = 0; i < enumerables.Length; i++)
                {
                    enumerables[i] = _dictionary[i].Values;
                }
                return new ReadonlyEnumerableCollection<TValue>(enumerables);
            }
        }

        /// <summary>
        /// Gets/sets the value corresponding to the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                if (this.TryGetValue(key, out value))
                {
                    return value;
                }
                throw new System.Collections.Generic.KeyNotFoundException();
            }
            set
            {
                for (int idx = 0; idx < _dictionary.Count; idx++)
                {
                    if (_dictionary[idx].ContainsKey(key))
                    { // replace the original value.
                        _dictionary[idx][key] = value;
                        return;
                    }
                }

                // the original does not exist yet.
                this.Add(key, value);
            }
        }

        /// <summary>
        /// Adds the given item.
        /// </summary>
        /// <param name="item"></param>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            this.Add(item.Key, item.Value);
        }

        /// <summary>
        /// Clears the entire dictionary.
        /// </summary>
        public void Clear()
        {
            _dictionary.Clear();
            _dictionary.Add(new Dictionary<TKey, TValue>());
        }

        /// <summary>
        /// Returns true if the given item is contained in this dictionairy.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            for (int idx = 0; idx < _dictionary.Count; idx++)
            {
                if (_dictionary[idx].Contains(item))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Copies the content of an array to this dictionary.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (KeyValuePair<TKey, TValue> element in this)
            {
                array[arrayIndex] = element;
                arrayIndex++;
            }
        }

        /// <summary>
        /// Returns the total element count in this dictionary.
        /// </summary>
        public int Count
        {
            get
            {
                int total = 0;
                for (int idx = 0; idx < _dictionary.Count; idx++)
                {
                    total = total + _dictionary[idx].Count;
                }
                return total;
            }
        }

        /// <summary>
        /// Returns the count of the internal dictionaries.
        /// </summary>
        public int CountDictionaries
        {
            get
            {
                return _dictionary.Count;
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
        /// Removes the given item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return this.Remove(item.Key);
        }

        /// <summary>
        /// Enumerates all key-value pairs.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            if(_dictionary.Count == 0)
            {
                return Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
            }
            IEnumerable<KeyValuePair<TKey, TValue>> enumerable = _dictionary[0];
            for (var i = 1; i < _dictionary.Count; i++)
            {
                enumerable = Enumerable.Concat<KeyValuePair<TKey, TValue>>(enumerable, 
                    _dictionary[i]);
            }
            return enumerable.GetEnumerator();
        }

        /// <summary>
        /// Enumerates all key-value pairs.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private class ReadonlyEnumerableCollection<T> : ICollection<T>
        {
            private IEnumerable<T> _enumerable;

            public ReadonlyEnumerableCollection(params IEnumerable<T>[] enumerables)
            {
                _enumerable = enumerables[0];
                for (var i = 1; i < enumerables.Length; i++)
                {
                    _enumerable = Enumerable.Concat<T>(_enumerable,
                        enumerables[i]);
                }
            }

            int ICollection<T>.Count
            {
                get
                {
                    return _enumerable.Count();
                }
            }

            bool ICollection<T>.IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            void ICollection<T>.Add(T item)
            {
                throw new InvalidOperationException("Collection is readonly.");
            }

            void ICollection<T>.Clear()
            {
                throw new InvalidOperationException("Collection is readonly.");
            }

            bool ICollection<T>.Contains(T item)
            {
                return _enumerable.Contains(item);
            }

            void ICollection<T>.CopyTo(T[] array, int arrayIndex)
            {
                foreach(var item in _enumerable)
                {
                    array[arrayIndex] = item;
                    arrayIndex++;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _enumerable.GetEnumerator();
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                return _enumerable.GetEnumerator();
            }

            bool ICollection<T>.Remove(T item)
            {
                throw new InvalidOperationException("Collection is readonly.");
            }
        }
    }
}