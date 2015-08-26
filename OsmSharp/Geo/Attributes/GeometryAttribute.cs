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

namespace OsmSharp.Geo.Attributes
{
    /// <summary>
    /// Represents an attribute related to a geometry.
    /// </summary>
    public class GeometryAttribute
    {
        /// <summary>
        /// Gets/sets the key property describing the data in this attribute.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets/sets the value of this attribute.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Returns a System.String that represents this GeometryAttribute.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.Key != null)
            {
                if (this.Value != null)
                {
                    return string.Format("{0}={1}", this.Key, this.Value.ToString());
                }
                else
                {
                    return string.Format("{0}=null", this.Key);
                }
            }
            else
            {
                if (this.Value != null)
                {
                    return string.Format("null={0}", this.Value.ToString());
                }
                else
                {
                    return "null=null";
                }
            }
        }
    }
}
