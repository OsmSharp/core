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

namespace OsmSharp.Math.Geo.Lambert.International.Belgium
{
    /// <summary>
    /// The belgian 1972 lambert system.
    /// </summary>
    public class Belgium1972LambertProjection : LambertProjectionBase
    {
        /// <summary>
        /// Creates a new lambert projection.
        /// </summary>
        public Belgium1972LambertProjection()
            : base("Belgium 1972 Projection",
            LambertEllipsoid.Hayford1924Ellipsoid,
            49.833334,
            51.166667,
            90,
            4.367487,
            150000.01256,
            5400088.438)
        {

        }
    }
}