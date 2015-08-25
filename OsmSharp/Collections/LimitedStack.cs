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

using System.Collections.Generic;

namespace OsmSharp.Collections
{
    /// <summary>
    /// Class implementing a thread-safe limited stack.
    /// 
    /// When the limit is reached the oldest element will be removed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LimitedStack<T>
    {
        /// <summary>
        /// Containts the list of elements in this queue.
        /// </summary>
        private List<T> _elements;

        /// <summary>
        /// The limit to the size of this queue.
        /// </summary>
        private int _limit;

        /// <summary>
        /// Creates a new limited stack with a limit of 10.
        /// </summary>
        public LimitedStack()
        {
            _limit = 10;
            _elements = new List<T>();
        }

        /// <summary>
        /// Creates a new limited stack with a limit of 10.
        /// </summary>
        public LimitedStack(IEnumerable<T> collection)
        {
            _limit = 10;
            _elements = new List<T>(collection);
            if (_elements.Count > _limit)
            {
                _elements.RemoveRange(0, _elements.Count - _limit);
            }
        }

        /// <summary>
        /// Creates a new limited stack with a limit of 10.
        /// </summary>
        public LimitedStack(int capacity)
        {
            _limit = 10;
            // no use initializing greater than limit.
            _elements = new List<T>(capacity>_limit?_limit:capacity);
        }

        /// <summary>
        /// Creates a new limited stack with a limit of 10.
        /// </summary>
        public LimitedStack(int capacity, int limit)
        {
            _limit = limit;
            // no use initializing greater than limit.
            _elements = new List<T>(capacity > _limit ? _limit : capacity);
        }

        /// <summary>
        /// Returns the number of elements in this stack.
        /// </summary>
        public int Count 
        {
            get
            {
                lock (_elements)
                {
                    return _elements.Count;
                }
            }
        }

        /// <summary>
        /// Clears the elements from this stack.
        /// </summary>
        public void Clear()
        {
            lock (_elements)
            {
                _elements.Clear();
            }
        }

        /// <summary>
        /// Returns true if this stack contains the item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            lock (_elements)
            {
                return _elements.Contains(item);
            }
        }

        /// <summary>
        /// Pops the top element from the stack.
        /// </summary>
        /// <returns></returns>
        public T Pop()
        {
            lock (_elements)
            {
                T element = _elements[_elements.Count - 1];
                _elements.RemoveAt(_elements.Count - 1);
                return element;
            }
        }

        /// <summary>
        /// Pushes an item on the stack.
        /// </summary>
        /// <param name="item"></param>
        public void Push(T item)
        {
            lock (_elements)
            {
                if (_elements.Count == _limit)
                {
                    // remove the last fist element to enter the stack
                    _elements.RemoveAt(0);
                }
                _elements.Add(item);
            }
        }

        /// <summary>
        /// Moves an item to the top of the stack if it already exists.
        /// </summary>
        /// <param name="item"></param>
        public void PushToTop(T item)
        {
            lock (_elements)
            {
                if(_elements.Contains(item))
                { // remove the item.
                    _elements.Remove(item);
                }
                // pushes the item to the top of the stack.
                this.Push(item);                
            }
        }

        /// <summary>
        /// The maximum amount of elements in this queue.
        /// </summary>
        public int Limit
        {
            get
            {
                lock (_elements)
                {
                    return _limit;
                }
            }
            set
            {
                lock (_elements)
                {
                    _limit = value;
                    if (_elements.Count > _limit)
                    {
                        _elements.RemoveRange(0, _elements.Count - _limit);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the top element without popping it.
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            lock (_elements)
            {
                return _elements[_elements.Count - 1];
            }
        }
    }
}
