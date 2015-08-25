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

namespace OsmSharp.Collections.Sorting
{
    /// <summary>
    /// An implementation of the quicksort algorithm.
    /// </summary>
    public static class QuickSort
    {
        /// <summary>
        /// Executes a quicksort algorithm given the value and swap methods.
        /// </summary>
        public static void Sort(Func<long, long> value, Action<long, long> swap, long left, long right)
        {
            if (left < right)
            {
                var stack = new System.Collections.Generic.Stack<Pair>();
                stack.Push(new Pair(left, right));
                while (stack.Count > 0)
                {
                    var pair = stack.Pop();
                    var pivot = QuickSort.Partition(value, swap, pair.Left, pair.Right);
                    if (pair.Left < pivot)
                    {
                        stack.Push(new Pair(pair.Left, pivot - 1));
                    }
                    if (pivot < pair.Right)
                    {
                        stack.Push(new Pair(pivot + 1, pair.Right));
                    }
                }
            }
        }

        private struct Pair
        {
            public Pair(long left, long right)
                : this()
            {
                this.Left = left;
                this.Right = right;
            }

            public long Left { get; set; }
            public long Right { get; set; }
        }

        /// <summary>
        /// Partion the based on the pivot value.
        /// </summary>
        /// <return>The new left.</return>
        private static long Partition(Func<long, long> value, Action<long, long> swap, long left, long right)
        { // get the pivot value.
            if (left > right) { throw new ArgumentException("left should be smaller than or equal to right."); }
            if (left == right)
            { // sorting just one item results in that item being sorted already and a pivot equal to that item itself.
                return right;
            }

            // select the middle one as the pivot value to counteract sorting.
            var pivot = (left + right) / (long)2;
            if(pivot != left)
            { // switch.
                swap(pivot, left);
            }

            // start with the left as pivot value.
            pivot = left;
            var pivotValue = value(pivot);

            while (true)
            {
                // move the left to the right until the first value bigger than pivot.
                var leftValue = value(left + 1);
                while (leftValue <= pivotValue)
                {
                    left++;
                    if(left == right)
                    {
                        break;
                    }
                    leftValue = value(left + 1);
                }

                // move the right to left until the first value smaller than pivot.
                if (left != right)
                {
                    var rightValue = value(right);
                    while (rightValue > pivotValue)
                    {
                        right--;
                        if (left == right)
                        {
                            break;
                        }
                        rightValue = value(right);
                    }
                }

                if (left == right)
                { // we are done searching, left == right.
                    if (pivot != left)
                    { // make sure the pivot value is where it is supposed to be.
                        swap(pivot, left);
                    }
                    return left;
                }

                // swith left<->right.
                swap(left + 1, right);
            }
        }
    }
}
