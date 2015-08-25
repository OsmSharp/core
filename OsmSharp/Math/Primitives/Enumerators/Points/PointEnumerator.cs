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
using System.Collections;

namespace OsmSharp.Math.Primitives.Enumerators.Points
{
    /// <summary>
    /// Enumerator class for any collection if points implementing IPointEnumerable.
    /// </summary>
    internal class PointEnumerator :
        IEnumerator<PointF2D>,
        IEnumerator
    {
        /// <summary>
        /// Holds the enumerable being enumerated.
        /// </summary>
        private IPointList _enumerable;

        /// <summary>
        /// Holds the current point.
        /// </summary>
        private PointF2D _current_point;

        /// <summary>
        /// Holds the current index.
        /// </summary>
        private int _current_idx;

        /// <summary>
        /// Creates a new enumerator.
        /// </summary>
        /// <param name="enumerable"></param>
        public PointEnumerator(IPointList enumerable)
        {
            _enumerable = enumerable;
        }

        #region IEnumerator<PointF2D> Members

        /// <summary>
        /// Returns the current points.
        /// </summary>
        public PointF2D Current
        {
            get { return _current_point; }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes of this object.
        /// </summary>
        public void Dispose()
        {
            _current_point = null;
        }

        #endregion

        #region IEnumerator Members

        /// <summary>
        /// Returns the current point.
        /// </summary>
        object System.Collections.IEnumerator.Current
        {
            get { return _current_point; }
        }

        /// <summary>
        /// Move to the next point.
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            _current_idx++;
            if (_enumerable.Count > _current_idx)
            {
                _current_point = _enumerable[_current_idx];
                return true;
            }
            return false;
        }

        /// <summary>
        /// Resets the enumerator.
        /// </summary>
        public void Reset()
        {
            _current_idx--;
            _current_point = null;
        }

        #endregion

        #region IEnumerator Members
        
        /// <summary>
        /// Move to the next point.
        /// </summary>
        /// <returns></returns>
        bool IEnumerator.MoveNext()
        {
            return this.MoveNext();
        }

        /// <summary>
        /// Resets the enumerator.
        /// </summary>
        void IEnumerator.Reset()
        {
            this.Reset();
        }

        #endregion
    }
}
