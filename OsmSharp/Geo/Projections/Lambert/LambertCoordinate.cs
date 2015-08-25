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

namespace OsmSharp.Math.Geo.Lambert
{
    /// <summary>
    /// Represents a lambert coordinate for a given projection.
    /// </summary>
    public class LambertCoordinate
    {
        /// <summary>
        /// The projection this lambert coordinate is for.
        /// </summary>
        private LambertProjectionBase _projection;

        /// <summary>
        /// The x-part of this coordinate.
        /// </summary>
        private double _x;

        /// <summary>
        /// The y-part of this coordinate.
        /// </summary>
        private double _y;

        /// <summary>
        /// Creates a new lambert coordinate.
        /// </summary>
        /// <param name="projection"></param>
        public LambertCoordinate(LambertProjectionBase projection)
        {
            _projection = projection;
        }

        /// <summary>
        /// Gets the projection for this coordinate.
        /// </summary>
        public LambertProjectionBase Projection
        {
            get
            {
                return _projection;
            }
        }

        /// <summary>
        /// Gets/Sets the x-part of this coordinate.
        /// </summary>
        public double X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }

        /// <summary>
        /// Gets/Sets the y-part of this coordinate.
        /// </summary>
        public double Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
            }
        }
    }
}
