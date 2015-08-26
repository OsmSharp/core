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

using System.Collections;
using System.Collections.Generic;

namespace OsmSharp.Geo.Attributes
{
    /// <summary>
    /// Represents a collection of geometry attributes.
    /// </summary>
    public abstract class GeometryAttributeCollection : IEnumerable<GeometryAttribute>
    {
        /// <summary>
        /// Returns the number of attributes in this collection.
        /// </summary>
        public abstract int Count { get; }

        /// <summary>
        /// Adds a key-value pair to this attributes collection.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public abstract void Add(string key, object value);

        /// <summary>
        /// Adds an attribute.
        /// </summary>
        /// <param name="attribute"></param>
        public abstract void Add(GeometryAttribute attribute);

        /// <summary>
        /// Adds all attributes from the given collection.
        /// </summary>
        /// <param name="attributeCollection"></param>
        public void Add(GeometryAttributeCollection attributeCollection)
        {
            foreach (GeometryAttribute attribute in attributeCollection)
            {
                this.Add(attribute);
            }
        }

        /// <summary>
        /// Adds the attributes or replaces the existing value if any.
        /// </summary>
        /// <param name="attributeCollection"></param>
        public void AddOrReplace(GeometryAttributeCollection attributeCollection)
        {
            foreach (GeometryAttribute attribute in attributeCollection)
            {
                this.AddOrReplace(attribute);
            }
        }

        /// <summary>
        /// Adds an attribute or replace the existing value if any.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public abstract void AddOrReplace(string key, object value);

        /// <summary>
        /// Adds an attribute or replace the existing value if any.
        /// </summary>
        /// <param name="attribute"></param>
        public abstract void AddOrReplace(GeometryAttribute attribute);

        /// <summary>
        /// Returns true if the given tag exists.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract bool ContainsKey(string key);

        /// <summary>
        /// Returns true if the given tag exists.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract bool TryGetValue(string key, out object value);

        /// <summary>
        /// Returns true if the given tags exists with the given value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract bool ContainsKeyValue(string key, object value);

        /// <summary>
        /// Returns the value associated with the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Throws a KeyNotFoundException when the key does not exists. Use TryGetValue.</returns>
        public virtual object this[string key]
        {
            get
            {
                object value;
                if (this.TryGetValue(key, out value))
                {
                    return value;
                }
                throw new KeyNotFoundException();
            }
            set
            {
                this.AddOrReplace(key, value);
            }
        }

        /// <summary>
        /// Clears all attributes.
        /// </summary>
        public abstract void Clear();

        #region IEnumerable<GeometryAttribute>

        /// <summary>
        /// Returns the enumerator for this enumerable.
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator<GeometryAttribute> GetEnumerator();

        /// <summary>
        /// Returns the enumerator for this enumerable.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}