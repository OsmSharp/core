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

using ProtoBuf;

namespace OsmSharp.Collections.Tags
{
    /// <summary>
    /// Represents a tag (a key-value pair).
    /// </summary>
    [ProtoContract]
    public struct Tag
    {
        /// <summary>
        /// Creates a new tag.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public Tag(string key, string value)
            :this()
        {
            this.Key = key;
            this.Value = value;
        }

        /// <summary>
        /// The key (or the actual tag name).
        /// </summary>
        [ProtoMember(1)]
        public string Key { get; set; }

        /// <summary>
        /// The value of the tag.
        /// </summary>
        [ProtoMember(2)]
        public string Value { get; set; }

        /// <summary>
        /// Creates a new tag.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Tag Create(string key, string value)
        {
            return new Tag(key, value);
        }

        /// <summary>
        /// Returns a description of this tag.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}={1}", this.Key, this.Value);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Tag)
            {
                return this.Key == ((Tag)obj).Key &&
                    this.Value == ((Tag)obj).Value;
            }
            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (this.Key == null && this.Value == null)
            {
                return 1501234;
            }
            else if (this.Key == null)
            {
                return 140011346 ^
                    this.Value.GetHashCode();
            }
            else if (this.Value == null)
            {
                return 103254761 ^
                    this.Key.GetHashCode();
            }
            return this.Key.GetHashCode() ^
                this.Value.GetHashCode();
        }
    }
}