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
using System.Text;

namespace OsmSharp.Collections.PriorityQueues
{
    /// <summary>
    /// Implements a priority queue in the form of a binairy heap.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BinaryHeap<T> : IPriorityQueue<T>
    {
        /// <summary>
        /// The objects per priority.
        /// </summary>
        private T[] _heap;

        /// <summary>
        /// Holds the priorities of this heap.
        /// </summary>
        private float[] _priorities;

        /// <summary>
        /// The current count of elements.
        /// </summary>
        private int _count;

        /// <summary>
        /// The latest unused index
        /// </summary>
        private uint _latest_index;

        /// <summary>
        /// Creates a new binairy heap.
        /// </summary>
        public BinaryHeap()
            : this(2)
        {

        }

        /// <summary>
        /// Creates a new binairy heap.
        /// </summary>
        public BinaryHeap(uint initialSize)
        {
            _heap = new T[initialSize];
            _priorities = new float[initialSize];

            _count = 0;
            _latest_index = 1;
        }

        /// <summary>
        /// Returns the number of items in this queue.
        /// </summary>
        public int Count
        {
            get { return _count; }
        }

        /// <summary>
        /// Enqueues a given item.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="priority"></param>
        public void Push(T item, float priority)
        {
            _count++; // another item was added!

            // increase size if needed.
            if (_latest_index == _priorities.Length - 1)
            { // time to increase size!
                Array.Resize<T>(ref _heap, _heap.Length + 100);
                Array.Resize<float>(ref _priorities, _priorities.Length + 100);
            }

            // add the item at the first free point 
            _priorities[_latest_index] = priority;
            _heap[_latest_index] = item;

            // ... and let it 'bubble' up.
            uint bubble_index = _latest_index;
            _latest_index++;
            while (bubble_index != 1)
            { // bubble until the indx is one.
                uint parent_idx = bubble_index / 2;
                if (_priorities[bubble_index] < _priorities[parent_idx])
                { // the parent priority is higher; do the swap.
                    float temp_priority = _priorities[parent_idx];
                    T temp_item = _heap[parent_idx];
                    _priorities[parent_idx] = _priorities[bubble_index];
                    _heap[parent_idx] = _heap[bubble_index];
                    _priorities[bubble_index] = temp_priority;
                    _heap[bubble_index] = temp_item;

                    bubble_index = parent_idx;
                }
                else
                { // the parent priority is lower or equal; the item will not bubble up more.
                    break;
                }
            }
        }

        /// <summary>
        /// Returns the smallest weight in the queue.
        /// </summary>
        /// <returns></returns>
        public float PeekWeight()
        {
            return _priorities[1];
        }

        /// <summary>
        /// Returns the object with the smallest weight.
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            return _heap[1];
        }

        /// <summary>
        /// Returns the object with the smallest weight and removes it.
        /// </summary>
        /// <returns></returns>
        public T Pop()
        {
            if (_count > 0)
            {
                T item = _heap[1]; // get the first item.

                _count--; // reduce the element count.
                _latest_index--; // reduce the latest index.

                int swapitem = 1, parent = 1;
                float swapItemPriority = 0;
                float parentPriority = _priorities[_latest_index];
                T parentItem = _heap[_latest_index];
                _heap[1] = parentItem;// _heap[_latest_index]; // place the last element on top.
                _priorities[1] = parentPriority; // _priorities[_latest_index]; // place the last element on top.
                do
                {
                    parent = swapitem;
                    if ((2 * parent + 1) <= _latest_index)
                    {
                        swapItemPriority = _priorities[2 * parent];
                        float potentialSwapItem = _priorities[2 * parent + 1];
                        if (parentPriority >= swapItemPriority)
                        {
                            swapitem = 2 * parent;
                            if (_priorities[swapitem] >= potentialSwapItem)
                            {
                                swapItemPriority = potentialSwapItem;
                                swapitem = 2 * parent + 1;
                            }
                        }
                        else if (parentPriority >= potentialSwapItem)
                        {
                            swapItemPriority = potentialSwapItem;
                            swapitem = 2 * parent + 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else if ((2 * parent) <= _latest_index)
                    {
                        // Only one child exists
                        swapItemPriority = _priorities[2 * parent];
                        if (parentPriority >= swapItemPriority)
                        {
                            swapitem = 2 * parent;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }

                    // One if the parent's children are smaller or equal, swap them
                    //if (parent == swapitem)
                    //{
                    //    OsmSharp.Logging.Log.TraceEvent("hug", Logging.TraceEventType.Information, "");
                    //}

                    //float temp_priority = _priorities[parent];
                    //T temp_item = _heap[parent];
                    _priorities[parent] = swapItemPriority;
                    _priorities[swapitem] = parentPriority;
                    _heap[parent] = _heap[swapitem];
                    _heap[swapitem] = parentItem;

                    //parentPriority = _priorities[swapitem];
                    //parentItem = _heap[swapitem];
                } while (true);

                return item;
            }
            return default(T);
        }

        /// <summary>
        /// Clears this priority queue.
        /// </summary>
        public void Clear()
        {
            //for (int idx = 0; idx < _heap.Length; idx++)
            //{
            //    _heap[idx] = default(T);
            //    _priorities[idx] = 0;
            //}

            _count = 0;
            _latest_index = 1;
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}