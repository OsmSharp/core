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

using OsmSharp.Geo;
using ProtoBuf;

namespace OsmSharp.Math.Geo.Simple
{
    /// <summary>
    /// A simple version of a coordinate.
    /// </summary>
    [ProtoContract]
    public struct GeoCoordinateSimple : ICoordinate
    {
        /// <summary>
        /// Latitude.
        /// </summary>
        [ProtoMember(1)]
        public float Latitude { get; set; }

        /// <summary>
        /// Longitude.
        /// </summary>
        [ProtoMember(2)]
        public float Longitude { get; set; }

        /// <summary>
        /// Returns true if the given obj is a GeoCoordinateSimple and represents the exact same location.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if(obj is GeoCoordinateSimple)
            {
                var coord = (GeoCoordinateSimple)obj;
                return coord.Latitude == this.Latitude &&
                    coord.Longitude == this.Longitude;
            }
            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Longitude.GetHashCode() ^
                this.Latitude.GetHashCode();
        }

        /// <summary>
        /// Returns a string describing this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[{0}, {1}]", this.Latitude.ToInvariantString(), 
                this.Longitude.ToInvariantString());
        }
    }
}