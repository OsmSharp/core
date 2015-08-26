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

using OsmSharp.Units.Angle;

namespace OsmSharp.Math.Geo.Meta
{
    /// <summary>
    /// Represents a relative direction.
    /// </summary>
    public class RelativeDirection
    {
        /// <summary>
        /// The relative direction.
        /// </summary>
        public RelativeDirectionEnum Direction { get; set; }

        /// <summary>
        /// The angle.
        /// </summary>
        public Degree Angle { get; set; }

        /// <summary>
        /// Returns a System.String that represents the current System.Object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}@{1}", this.Direction, this.Angle);
        }
    }
}