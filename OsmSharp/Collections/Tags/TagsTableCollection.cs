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
    /// Represents a tags collection based on a backing tags table.
    /// </summary>
    public class TagsTableCollection : TagsCollectionBase
    {
        /// <summary>
        /// Holds the tags.
        /// </summary>
        private readonly List<uint> _tags;

        /// <summary>
        /// Holds all the tags objects.
        /// </summary>
        private readonly ObjectTable<Tag> _tagsTable;

        /// <summary>
        /// Creates a new tags collection.
        /// </summary>
        /// <param name="tagsTable"></param>
        public TagsTableCollection(ObjectTable<Tag> tagsTable)
        {
            _tags = new List<uint>();
        }

        /// <summary>
        /// Creates a new tags collection initialized with the given existing tags.
        /// </summary>
        /// <param name="tagsTable"></param>
        /// <param name="tags"></param>
        public TagsTableCollection(ObjectTable<Tag> tagsTable, params Tag[] tags)
        {
            _tagsTable = tagsTable;
            _tags = new List<uint>();
            foreach(Tag tag in tags)
            {
                _tags.Add(_tagsTable.Add(tag));
            }
        }

        /// <summary>
        /// Creates a new tags collection initialized with the given existing tags.
        /// </summary>
        /// <param name="tagsTable"></param>
        /// <param name="tags"></param>
        public TagsTableCollection(ObjectTable<Tag> tagsTable, IEnumerable<Tag> tags)
        {
            _tagsTable = tagsTable;
            _tags = new List<uint>();
            foreach(Tag tag in tags)
            {
                _tags.Add(_tagsTable.Add(tag));
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
            this.Add(new Tag()
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
            _tags.Add(_tagsTable.Add(tag));
        }

        /// <summary>
        /// Adds a new tag (key-value pair) to this tags collection.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public override void AddOrReplace(string key, string value)
        {
            for (int idx = 0; idx < _tags.Count; idx++)
            {
                Tag tag = _tagsTable.Get(_tags[idx]);
                if (tag.Key == key)
                {
                    tag.Value = value;
                    _tags[idx] = _tagsTable.Add(tag);
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
            return _tags.Any((tagId) =>
                {
                    return _tagsTable.Get(tagId).Key == key;
                });
        }

        /// <summary>
        /// Returns true if the given key exists and sets the value parameter.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TryGetValue(string key, out string value)
        {
            foreach (var tagId in _tags)
            {
                Tag tag = _tagsTable.Get(tagId);
                if (tag.Key == key)
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
            return _tags.Any((tagId) =>
            {
                Tag tag = _tagsTable.Get(tagId);
                return tag.Key == key && tag.Value == value;
            });
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
            return _tags.Select<uint, Tag>((tagId) =>
            {
                return _tagsTable.Get(tagId);
            }).GetEnumerator();
        }

        /// <summary>
        /// Removes all tags with the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override bool RemoveKey(string key)
        {
            return _tags.RemoveAll((tagId) => 
                {
                    return _tagsTable.Get(tagId).Key == key;
                }) > 0;
        }

        /// <summary>
        /// Removes all tags with given key and value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool RemoveKeyValue(string key, string value)
        {
            return _tags.RemoveAll((tagId) =>
            {
                Tag tag = _tagsTable.Get(tagId);
                return tag.Key == key && tag.Value == value;
            }) > 0;
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

        /// <summary>
        /// Removes all tags that match the given predicate.
        /// </summary>
        /// <param name="predicate"></param>
        public override void RemoveAll(System.Predicate<Tag> predicate)
        {
            _tags.RemoveAll(x => predicate.Invoke(_tagsTable.Get(x)));
        }
    }
}