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

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Collections.Tags
{
    /// <summary>
    /// Represents a simple tags collection based on a list.
    /// </summary>
    public class TagsCollection : TagsCollectionBase
    {
        /// <summary>
        /// Holds the tags.
        /// </summary>
        private readonly List<Tag> _tags;

        /// <summary>
        /// Creates a new tags collection.
        /// </summary>
        public TagsCollection()
        {
            _tags = new List<Tag>();
        }

        /// <summary>
        /// Creates a new tags collection.
        /// </summary>
        /// <param name="capacity">The number of tags the collection can initially store.</param>
        public TagsCollection(int capacity)
        {
            _tags = new List<Tag>(capacity);
        }

        /// <summary>
        /// Creates a new tags collection.
        /// </summary>
        public TagsCollection(params Tag[] tags)
        {
            _tags = new List<Tag>();
            if (tags != null)
            { // allow null.
                _tags.AddRange(tags);
            }
        }

        /// <summary>
        /// Creates a new tags collection initialized with the given existing tags.
        /// </summary>
        /// <param name="tags"></param>
        public TagsCollection(IEnumerable<Tag> tags)
        {
            _tags = new List<Tag>();
            if (tags != null)
            { // allow null.
                _tags.AddRange(tags);
            }
        }

        /// <summary>
        /// Creates a new tags collection initialized with the given existing tags.
        /// </summary>
        /// <param name="tags"></param>
        public TagsCollection(IDictionary<string, string> tags)
        {
            _tags = new List<Tag>();
            if (tags != null)
            { // allow null.
                foreach(KeyValuePair<string, string> pair in tags)
                {
                    _tags.Add(new Tag(pair.Key, pair.Value));
                }
            }
        }

        /// <summary>
        /// Returns the number of tags in this collection.
        /// </summary>
        public override int Count
        {
            get { return _tags.Count; }
        }

        /// <summary>
        /// Returns true if this collection is readonly.
        /// </summary>
        public override bool IsReadonly
        {
            get { return false; }
        }

        /// <summary>
        /// Adds a new tag (key-value pair) to this tags collection.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public override void Add(string key, string value)
        {
            _tags.Add(new Tag()
            {
                Key = key,
                Value = value
            });
        }

        /// <summary>
        /// Adds a new tag to this collection.
        /// </summary>
        /// <param name="tag"></param>
        public override void Add(Tag tag)
        {
            _tags.Add(tag);
        }

        /// <summary>
        /// Adds a new tag (key-value pair) to this tags collection.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public override void AddOrReplace(string key, string value)
        {
            for(int idx = 0; idx < _tags.Count; idx++)
            {
                Tag tag = _tags[idx];
                if (tag.Key == key)
                {
                    tag.Value = value;
                    _tags[idx] = tag;
                    return;
                }
            }
            this.Add(key, value);
        }

        /// <summary>
        /// Adds a new tag to this collection.
        /// </summary>
        /// <param name="tag"></param>
        public override void AddOrReplace(Tag tag)
        {
            this.AddOrReplace(tag.Key, tag.Value);
        }

        /// <summary>
        /// Returns true if the given key is found in this tags collection.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override bool ContainsKey(string key)
        {
            return _tags.Any(tag => tag.Key == key);
        }

        /// <summary>
        /// Returns true if the given key exists and sets the value parameter.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TryGetValue(string key, out string value)
        {
            foreach(var tag in _tags)
            {
                if(tag.Key == key)
                {
                    value = tag.Value;
                    return true;
                }
            }
            value = string.Empty;
            return false;
        }

        /// <summary>
        /// Returns true if the given key-value pair is found in this tags collection.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool ContainsKeyValue(string key, string value)
        {
            return _tags.Any(tag => tag.Key == key && tag.Value == value);
        }

        /// <summary>
        /// Clears all tags.
        /// </summary>
        public override void Clear()
        {
            _tags.Clear();
        }

        /// <summary>
        /// Returns the enumerator for this tags collection.
        /// </summary>
        /// <returns></returns>
        public override IEnumerator<Tag> GetEnumerator()
        {
            return _tags.GetEnumerator();
        }

        /// <summary>
        /// Removes all tags with the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override bool RemoveKey(string key)
        {
            return _tags.RemoveAll(tag => tag.Key == key) > 0;
        }

        /// <summary>
        /// Removes all tags with given key and value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool RemoveKeyValue(string key, string value)
        {
            return _tags.RemoveAll(tag => tag.Key == key && tag.Value == value) > 0;
        }

        /// <summary>
        /// Removes all tags that matches the given predicate.
        /// </summary>
        /// <param name="predicate"></param>
        public override void RemoveAll(System.Predicate<Tag> predicate)
        {
            _tags.RemoveAll(predicate);
        }

        /// <summary>
        /// Returns a string that represents this tags collection.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder tags = new StringBuilder();
            foreach (Tag tag in this)
            {
                tags.Append(tag.ToString());
                tags.Append(',');
            }
            if (tags.Length > 0)
            {
                return tags.ToString(0, tags.Length - 1);
            }
            return "empty";
        }
    }
}