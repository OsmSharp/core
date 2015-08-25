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
    /// An enumerable of all possible permutations of a given sequence of objects.
    /// </summary>
    /// <remarks>
    /// Implements the Shimon Even variant of the Steinhaus–Johnson–Trotter algorithm.
    /// 
    /// https://en.wikipedia.org/wiki/Steinhaus%E2%80%93Johnson%E2%80%93Trotter_algorithm#Even.27s_speedup
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class PermutationEnumerable<T> : IEnumerable<T[]>
    {
        /// <summary>
        /// Holds the sequence to be enumerated.
        /// </summary>
        private T[] _sequence;

        /// <summary>
        /// Creates a new permutation enumerator over a given sequence.
        /// </summary>
        /// <param name="sequence"></param>
        public PermutationEnumerable(T[] sequence)
        {
            _sequence = sequence;
        }

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T[]> GetEnumerator()
        {
            return new PermutationEnumerator<T>(_sequence);
        }

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new PermutationEnumerator<T>(_sequence);
        }
    }

    /// <summary>
    /// Implements the Shimon Even variant of the Steinhaus–Johnson–Trotter algorithm.
    /// 
    /// https://en.wikipedia.org/wiki/Steinhaus%E2%80%93Johnson%E2%80%93Trotter_algorithm#Even.27s_speedup
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PermutationEnumerator<T> : IEnumerator<T[]>
    {
        /// <summary>
        /// The current sequence to permute.
        /// </summary>
        private T[] _sequence;

        /// <summary>
        /// The current status.
        /// </summary>
        private ElementStatus[] _status;

        /// <summary>
        /// Creates a new permutation operation.
        /// </summary>
        /// <param name="sequence"></param>
        internal PermutationEnumerator(T[] sequence)
        {
            _sequence = new T[sequence.Length];
            for (uint idx = 0; idx < _sequence.Length; idx++)
            {
                _sequence[idx] = sequence[idx];
            }
            _status = null;
        }

        /// <summary>
        /// Returns the current permutation.
        /// </summary>
        public T[] Current
        {
            get { return _sequence.Clone() as T[]; }
        }

        /// <summary>
        /// Disposes a enumerator.
        /// </summary>
        public void Dispose()
        {

        }

        /// <summary>
        /// Returns the current permutation.
        /// </summary>
        object System.Collections.IEnumerator.Current
        {
            get { return this.Current; }
        }

        /// <summary>
        /// Moves to the next permutation.
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            // intialize if needed.
            if (_status == null)
            {
                // reset the status.
                _status = new ElementStatus[_sequence.Length];
                _status[0] = new ElementStatus(1, null);
                for (uint idx = 1; idx < _sequence.Length; idx++)
                {
                    _status[idx] = new ElementStatus(idx + 1, false);
                }
                return true;
            }

            int max_idx = 0;
            ElementStatus max = new ElementStatus(uint.MinValue, null);
            for (int idx = 0; idx < _status.Length; idx++)
            {
                if (max.Value < _status[idx].Value && _status[idx].Direction.HasValue)
                { // the new value is bigger!
                    max_idx = idx;
                    max = _status[idx];
                }
            }

            // the algorithm is terminated if nothing is found.
            if (!max.Direction.HasValue)
            { // all directions are unmarked.
                return false;
            }

            // execute the swap.
            int idx1 = max_idx;
            int idx2 = max.IsForward ? max_idx + 1 : max_idx - 1;
            this.Swap(idx1, idx2);

            // update status.
            if (idx2 > idx1 && idx2 < _status.Length - 1)
            { // there is a next element, test if it is larger.
                if (_status[idx2 + 1].Value > max.Value)
                { // reset the direction of max.
                    max.Direction = null;
                }
            }
            else if (idx2 < idx1 && idx2 > 0)
            { // there is a next element, test if it is larger.
                if (_status[idx2 - 1].Value > max.Value)
                { // reset the direction of max.
                    max.Direction = null;
                }
            }
            if (idx2 == 0 || idx2 == _status.Length - 1)
            { // reset the direction of that element
                _status[idx2].Direction = null;
            }
            for (int idx = 0; idx < _status.Length; idx++)
            { // redirect all of the element higher than the selected element.
                if (_status[idx].Value > max.Value)
                { // reset the direction.
                    _status[idx].Direction = (idx < idx2);
                }
            }
            return true;
        }

        /// <summary>
        /// Actually executes a swap.
        /// </summary>
        /// <param name="idx1"></param>
        /// <param name="idx2"></param>
        public void Swap(int idx1, int idx2)
        {
            ElementStatus temp_status = _status[idx1];
            _status[idx1] = _status[idx2];
            _status[idx2] = temp_status;

            T temp = _sequence[idx1];
            _sequence[idx1] = _sequence[idx2];
            _sequence[idx2] = temp;
        }

        /// <summary>
        /// Resets the current enumeration.
        /// </summary>
        public void Reset()
        {
            // restore the initial sequence.
            T[] original = new T[_sequence.Length];
            for(int idx = 0; idx < _sequence.Length; idx++)
            { // loop over all positions and restore them from the sequence.
                original[idx] = _sequence[_status[idx].Value];
            }
            _sequence = original;
            _status = null;
        }

        /// <summary>
        /// Represents the status of one element in the permutation.
        /// </summary>
        private class ElementStatus
        {
            /// <summary>
            /// Creates a new element status.
            /// </summary>
            /// <param name="value"></param>
            /// <param name="direction"></param>
            public ElementStatus(uint value, bool? direction)
            {
                this.Value = value;
                this.Direction = direction;
            }

            /// <summary>
            /// The current value.
            /// </summary>
            public uint Value { get; private set; }

            /// <summary>
            /// The direction, true is forward, false is backward.
            /// </summary>
            public bool? Direction { get; set; }

            /// <summary>
            /// The direction is forward.
            /// </summary>
            public bool IsForward
            {
                get
                {
                    return this.Direction.HasValue && this.Direction.Value;
                }
            }

            /// <summary>
            /// The direction is backward.
            /// </summary>
            public bool IsBackward
            {
                get
                {
                    return this.Direction.HasValue && !this.Direction.Value;
                }
            }

            /// <summary>
            /// Returns a description of this element status.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                if (!this.Direction.HasValue)
                { // undirected.
                    return string.Format("{0}:Undirected",
                        this.Value);
                }
                else if (this.IsForward)
                { // forward.
                    return string.Format("{0}:Forward",
                        this.Value);
                }
                else
                { // backward.
                    return string.Format("{0}:Backward",
                        this.Value);
                }
            }
        }
    }
}
