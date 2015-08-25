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
    /// Represents general functionality of a priority queue.
    /// </summary>
    public interface IPriorityQueue<T> : IEnumerable<T>
    {
        /// <summary>
        /// Returns the number of items in this queue.
        /// </summary>
        int Count
        {
            get;
        }

        /// <summary>
        /// Enqueues a given item.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="priority"></param>
        void Push(T item, float priority);

        /// <summary>
        /// Returns the smallest weight in the queue.
        /// </summary>
        /// <returns></returns>
        float PeekWeight();

        /// <summary>
        /// Returns the object with the smallest weight.
        /// </summary>
        /// <returns></returns>
        T Peek();

        /// <summary>
        /// Returns the object with the smallest weight and removes it.
        /// </summary>
        /// <returns></returns>
        T Pop();
    }
}
