// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2016 Abelshausen Ben
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

namespace OsmSharp.Osm.Streams.Complete
{
    /// <summary>
    /// An osm complete enumerable stream source.
    /// </summary>
    public class OsmCompleteEnumerableStreamSource : OsmCompleteStreamSource
    {
        private readonly IEnumerable<ICompleteOsmGeo> _enumerable;

        /// <summary>
        /// Creates a new osm complete source based on the given enumerable.
        /// </summary>
        public OsmCompleteEnumerableStreamSource(IEnumerable<ICompleteOsmGeo> enumerable)
        {
            _enumerable = enumerable;
        }

        private IEnumerator<ICompleteOsmGeo> _enumerator;

        /// <summary>
        /// Returns true if this source can be reset.
        /// </summary>
        public override bool CanReset
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        /// <returns></returns>
        public override ICompleteOsmGeo Current()
        {
            return _enumerator.Current;
        }

        /// <summary>
        /// Initializes this source.
        /// </summary>
        public override void Initialize()
        {
            _enumerator = _enumerable.GetEnumerator();
        }

        /// <summary>
        /// Moves to the next object.
        /// </summary>
        /// <returns></returns>
        public override bool MoveNext()
        {
            return _enumerator.MoveNext();
        }

        /// <summary>
        /// Resets this source.
        /// </summary>
        public override void Reset()
        {
            _enumerator = _enumerable.GetEnumerator();
        }
    }
}
