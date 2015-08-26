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

using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Geo.Attributes
{
    /// <summary>
    /// Represents a simple tags collection based on a list.
    /// </summary>
    public class SimpleGeometryAttributeCollection : GeometryAttributeCollection
    {
        private readonly List<GeometryAttribute> _attributes;

        /// <summary>
        /// Creates a new attributes collection.
        /// </summary>
        public SimpleGeometryAttributeCollection()
        {
            _attributes = new List<GeometryAttribute>();
        }

        /// <summary>
        /// Creates a new attributes collection initialized with the given existing attributes.
        /// </summary>
        public SimpleGeometryAttributeCollection(IEnumerable<GeometryAttribute> attributes)
        {
            _attributes = new List<GeometryAttribute>();
            _attributes.AddRange(attributes);
        }

        /// <summary>
        /// Creates a new attributes collection initialized with the given existing key-value tags.
        /// </summary>
        public SimpleGeometryAttributeCollection(IEnumerable<OsmSharp.Collections.Tags.Tag> tags)
        {
            _attributes = new List<GeometryAttribute>();
            if (tags != null)
            {
                foreach (OsmSharp.Collections.Tags.Tag tag in tags)
                {
                    _attributes.Add(new GeometryAttribute()
                    {
                        Key = tag.Key,
                        Value = tag.Value
                    });
                }
            }
        }

        /// <summary>
        /// Returns the number of attributes in this collection.
        /// </summary>
        public override int Count
        {
            get { return _attributes.Count; }
        }

        /// <summary>
        /// Adds a new attribute (key-value pair) to this attributes collection.
        /// </summary>
        public override void Add(string key, object value)
        {
            _attributes.Add(new GeometryAttribute()
            {
                Key = key,
                Value = value
            });
        }

        /// <summary>
        /// Adds a new attribute to this collection.
        /// </summary>
        public override void Add(GeometryAttribute attribute)
        {
            _attributes.Add(attribute);
        }

        /// <summary>
        /// Adds a new attribute (key-value pair) to this attributes collection.
        /// </summary>
        public override void AddOrReplace(string key, object value)
        {
            for (int idx = 0; idx < _attributes.Count; idx++)
            {
                GeometryAttribute attribute = _attributes[idx];
                if (attribute.Key == key)
                {
                    attribute.Value = value;
                    _attributes[idx] = attribute;
                    return;
                }
            }
            this.Add(key, value);
        }

        /// <summary>
        /// Adds a new tag to this collection.
        /// </summary>
        public override void AddOrReplace(GeometryAttribute tag)
        {
            this.AddOrReplace(tag.Key, tag.Value);
        }

        /// <summary>
        /// Returns true if the given key is found.
        /// </summary>
        /// <returns></returns>
        public override bool ContainsKey(string key)
        {
            return this.Any(tag => tag.Key == key);
        }

        /// <summary>
        /// Returns true if the given key exists and gets the value parameter.
        /// </summary>
        /// <returns></returns>
        public override bool TryGetValue(string key, out object value)
        {
            foreach (var tag in this.Where(tag => tag.Key == key))
            {
                value = tag.Value;
                return true;
            }
            value = string.Empty;
            return false;
        }

        /// <summary>
        /// Returns true if the given key-value pair is found in this attributes collection.
        /// </summary>
        /// <returns></returns>
        public override bool ContainsKeyValue(string key, object value)
        {
            return this.Any(tag =>
                {
                    if (tag.Key.Equals(key))
                    {
                        if (tag.Value == null)
                        {
                            return value == null;   
                        }
                        return tag.Value.Equals(value);
                    }
                    return false;
                });
        }

        /// <summary>
        /// Clears all attributes.
        /// </summary>
        public override void Clear()
        {
            _attributes.Clear();
        }

        /// <summary>
        /// Returns the enumerator for this attributes collection.
        /// </summary>
        /// <returns></returns>
        public override IEnumerator<GeometryAttribute> GetEnumerator()
        {
            return _attributes.GetEnumerator();
        }
    }
}