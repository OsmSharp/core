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

namespace OsmSharp.Collections.Tags
{
    /// <summary>
    /// A dictionary that uses a string table behind.
    /// </summary>
    public class StringTableTagsCollection : TagsCollectionBase
    {
        /// <summary>
        /// Holds the list of encoded tags.
        /// </summary>
        private readonly List<TagEncoded> _tagsList;

        /// <summary>
        /// Holds the stringtable.
        /// </summary>
        private readonly ObjectTable<string> _stringTable; 

        /// <summary>
        /// Creates a new dictionary.
        /// </summary>
        /// <param name="stringTable"></param>
        public StringTableTagsCollection(ObjectTable<string> stringTable)
        {
            _stringTable = stringTable;
            _tagsList = new List<TagEncoded>();
        }

        /// <summary>
        /// Adds key-value pair of strings.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public override void Add(string key, string value)
        {
            _tagsList.Add(new TagEncoded()
                              {
                                  Key = _stringTable.Add(key),
                                  Value = _stringTable.Add(value)
                              });
        }

        /// <summary>
        /// Returns the number of tags in this collection.
        /// </summary>
        public override int Count
        {
            get { return _tagsList.Count; }
        }

        /// <summary>
        /// Returns true if this collection is readonly.
        /// </summary>
        public override bool IsReadonly
        {
            get { return false; }
        }

        /// <summary>
        /// Adds a tag.
        /// </summary>
        /// <param name="tag"></param>
        public override void Add(Tag tag)
        {
            this.Add(tag.Key, tag.Value);
        }

        /// <summary>
        /// Adds a new tag (key-value pair) to this tags collection.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public override void AddOrReplace(string key, string value)
        {
            uint keyInt = _stringTable.Add(key);  // TODO: this could be problematic, testing contains adds objects to string table.
            uint valueInt = _stringTable.Add(value);

            for (int idx = 0; idx < _tagsList.Count; idx++)
            {
                TagEncoded tag = _tagsList[idx];
                if (tag.Key == keyInt)
                {
                    tag.Value = valueInt;
                    _tagsList[idx] = tag;
                    return;
                }
            }
            _tagsList.Add(new TagEncoded()
            {
                Key = keyInt,
                Value = valueInt
            });
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
        /// Returns true if the given key is present.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override bool ContainsKey(string key)
        {
            uint keyInt = _stringTable.Add(key);  // TODO: this could be problematic, testing contains adds objects to string table.

            return _tagsList.Any(tag => tag.Key == keyInt);
        }

        /// <summary>
        /// Returns true when the given key is found and sets the value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TryGetValue(string key, out string value)
        {
            uint keyInt = _stringTable.Add(key);  // TODO: this could be problematic, testing contains adds objects to string table.

            foreach (var tagEncoded in _tagsList.Where(tagEncoded => tagEncoded.Key == keyInt))
            {
                value = _stringTable.Get(tagEncoded.Value);
                return true;
            }
            value = null;
            return false;
        }

        /// <summary>
        /// Returns true when key-value are contained.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool ContainsKeyValue(string key, string value)
        {
            uint keyInt = _stringTable.Add(key);  // TODO: this could be problematic, testing contains adds objects to string table.
            uint valueInt = _stringTable.Add(value);

            return _tagsList.Any(tagEncoded => tagEncoded.Key == keyInt && tagEncoded.Value == valueInt);
        }

        /// <summary>
        /// Returns an enumerator for the tags.
        /// </summary>
        /// <returns></returns>
        public override IEnumerator<Tag> GetEnumerator()
        {
            foreach (var tagEncoded in _tagsList)
            {
                var tag = new Tag()
                              {
                                  Key = _stringTable.Get(tagEncoded.Key),
                                  Value = _stringTable.Get(tagEncoded.Value)
                              };
                yield return tag;
            }
        }

        /// <summary>
        /// An encoded tag.
        /// </summary>
        private struct TagEncoded
        {
            /// <summary>
            /// The encoded key.
            /// </summary>
            public uint Key { get; set; }

            /// <summary>
            /// The encode value.
            /// </summary>
            public uint Value { get; set; }
        }

        /// <summary>
        /// Clears all tags.
        /// </summary>
        public override void Clear()
        {
            _tagsList.Clear();
        }

        /// <summary>
        /// Removes all tags with the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override bool RemoveKey(string key)
        {
            uint keyInt = _stringTable.Add(key);

            return _tagsList.RemoveAll(tagEncoded => tagEncoded.Key == keyInt) > 0;
        }

        /// <summary>
        /// Removes the given key-value pair.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool RemoveKeyValue(string key, string value)
        {
            uint keyInt = _stringTable.Add(key);
            uint valueInt = _stringTable.Add(value);

            return _tagsList.RemoveAll(tagEncoded => tagEncoded.Key == keyInt && tagEncoded.Value == valueInt) > 0;
        }

        /// <summary>
        /// Removes all tags that match the given predicate.
        /// </summary>
        /// <param name="predicate"></param>
        public override void RemoveAll(System.Predicate<Tag> predicate)
        {
            _tagsList.RemoveAll(x => predicate.Invoke(new Tag(_stringTable.Get(x.Key), _stringTable.Get(x.Value))));
        }
    }
}