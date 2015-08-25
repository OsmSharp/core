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

using OsmSharp.Collections.PriorityQueues;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Collections.Cache
{
    /// <summary>
    /// Generic LRU cache implementation.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class LRUCache<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        /// <summary>
        /// Holds the cached data.
        /// </summary>
        private Dictionary<TKey, CacheEntry> _data;

        /// <summary>
        /// Holds the next id.
        /// </summary>
        private ulong _id;

        /// <summary>
        /// Holds the last id.
        /// </summary>
        private ulong _lastId;

        /// <summary>
        /// A delegate to use for when an item is pushed out of the cache.
        /// </summary>
        /// <param name="item"></param>
        public delegate void OnRemoveDelegate(TValue item);

        /// <summary>
        /// Called when an item is pushed out of the cache.
        /// </summary>
        public OnRemoveDelegate OnRemove;

        /// <summary>
        /// Initializes this cache.
        /// </summary>
        /// <param name="capacity"></param>
        public LRUCache(int capacity)
        {
            _id = ulong.MinValue;
            _lastId = _id;
            _data = new Dictionary<TKey, CacheEntry>();

            this.MaxCapacity = ((capacity / 100) * 10) + capacity + 1;
            this.MinCapacity = capacity;
        }

        /// <summary>
        /// Gets the maximum number of items to keep until the cache is full.
        /// </summary>
        public int MaxCapacity { get; private set; }

        /// <summary>
        /// Gets the number of items keep when cache overflows.
        /// </summary>
        public int MinCapacity { get; private set; }

        /// <summary>
        /// Adds a new value for the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value)
        {
            CacheEntry entry = new CacheEntry
            {
                Id = _id,
                Value = value
            };
            lock (_data)
            {
                _id++;
                _data[key] = entry;
            }

            this.ResizeCache();
        }

        /// <summary>
        /// Returns the amount of entries in this cache.
        /// </summary>
        public int Count
        {
            get
            {
                return _data.Count;
            }
        }

        /// <summary>
        /// Returns the value for this given key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGet(TKey key, out TValue value)
        {
            lock (_data)
            {
                CacheEntry entry;
                _id++;
                if (_data.TryGetValue(key, out entry))
                {
                    entry.Id = _id;
                    value = entry.Value;
                    return true;
                }
            }
            value = default(TValue);
            return false;
        }

        /// <summary>
        /// Returns the value for this given key but does not effect the cache.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryPeek(TKey key, out TValue value)
        {
            lock (_data)
            {
                CacheEntry entry;
                if (_data.TryGetValue(key, out entry))
                {
                    value = entry.Value;
                    return true;
                }
            }
            value = default(TValue);
            return false;
        }

        /// <summary>
        /// Clears this cache.
        /// </summary>
        public void Clear()
        {
            lock(_data)
            {
                if (this.OnRemove != null)
                { // call the OnRemove delegate.
                    foreach (var entry in _data)
                    {
                        this.OnRemove(entry.Value.Value);
                    }
                }
                _data.Clear();
            }
            _id = ulong.MinValue;
            _lastId = _id;
        }

        /// <summary>
        /// Removes the value for the given key.
        /// </summary>
        /// <param name="id"></param>
        public void Remove(TKey id)
        {
            lock(_data)
            {
                _data.Remove(id);
            }
        }

        /// <summary>
        /// Resizes the cache.
        /// </summary>
        private void ResizeCache()
        {
            lock (_data)
            {
                if (_data.Count > this.MaxCapacity)
                {
                    var n = this.MaxCapacity - this.MinCapacity + 1;
                    var pairEnumerator = _data.GetEnumerator();
                    var queue = new BinaryHeapULong<KeyValuePair<TKey, CacheEntry>>((uint)n + 1);
                    while (queue.Count < n &&
                        pairEnumerator.MoveNext())
                    {
                        var current = pairEnumerator.Current;
                        queue.Push(current, ulong.MaxValue - current.Value.Id);
                    }
                    ulong min = queue.PeekWeight();
                    while (pairEnumerator.MoveNext())
                    {
                        var current = pairEnumerator.Current;
                        if (min < ulong.MaxValue - current.Value.Id)
                        {
                            queue.Push(current, ulong.MaxValue - current.Value.Id);
                            queue.Pop();
                            min = queue.PeekWeight();
                        }
                    }
                    while(queue.Count > 0)
                    {
                        var toRemove = queue.Pop();
                        if (this.OnRemove != null)
                        { // call the OnRemove delegate.
                            this.OnRemove(toRemove.Value.Value);
                        }
                        _data.Remove(toRemove.Key);
                        // update the 'last_id'
                        _lastId++;
                    }
                }
            }
        }

        /// <summary>
        /// An entry in this cache.
        /// </summary>
        private class CacheEntry
        {
            /// <summary>
            /// The id of the object.
            /// </summary>
            public ulong Id { get; set; }

            /// <summary>
            /// The object being cached.
            /// </summary>
            public TValue Value { get; set; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _data.Select<KeyValuePair<TKey, CacheEntry>, KeyValuePair<TKey, TValue>>(
                (source) =>
                {
                    return new KeyValuePair<TKey, TValue>(source.Key, source.Value.Value);
                }).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _data.Select<KeyValuePair<TKey, CacheEntry>, KeyValuePair<TKey, TValue>>(
                (source) =>
                {
                    return new KeyValuePair<TKey, TValue>(source.Key, source.Value.Value);
                }).GetEnumerator();
        }
    }
}