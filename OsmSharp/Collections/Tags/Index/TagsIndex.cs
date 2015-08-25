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

using OsmSharp.Collections.Indexes;
using OsmSharp.Collections.Indexes.MemoryMapped;
using OsmSharp.Collections.MemoryMapped;
using OsmSharp.IO;
using OsmSharp.IO.MemoryMappedFiles;
using System;
using System.Collections.Generic;

namespace OsmSharp.Collections.Tags.Index
{
    /// <summary>
    /// An osm tags index.
    /// </summary>
    public class TagsIndex : ITagsIndex
    {
        /// <summary>
        /// Contains all strings tags/values.
        /// </summary>
        private IndexBase<string> _stringIndex;

        /// <summary>
        /// Contains all tag collections as arrays:
        ///     [key1Id, value1Id, key2Id, value2Id]
        ///     
        /// The collections should be sorted by key and then compared.
        /// </summary>
        private IndexBase<int[]> _tagsIndex;

        /// <summary>
        /// Holds all strings and their id.
        /// </summary>
        private IDictionary<string, int> _stringReverseIndex;

        /// <summary>
        /// Holds all tag collections and their reverse index.
        /// </summary>
        private IDictionary<int[], uint> _tagsReverseIndex;

        /// <summary>
        /// Creates a new empty tags index.
        /// </summary>
        public TagsIndex()
            : this(false)
        {

        }

        /// <summary>
        /// Creates a new tags index.
        /// </summary>
        /// <param name="readOnly"></param>
        public TagsIndex(bool readOnly)
        {
            _stringIndex = new Index<string>();
            _tagsIndex = new Index<int[]>();

            if(!readOnly)
            { // this index is not readonly.
                _stringReverseIndex = new Dictionary<string, int>();
                _tagsReverseIndex = new Dictionary<int[], uint>(
                    new DelegateEqualityComparer<int[]>(
                        (obj) =>
                        { // assumed the array is sorted.
                            var hash = obj.Length.GetHashCode();
                            for(int idx = 0; idx < obj.Length; idx++)
                            {
                                hash = hash ^ obj[idx].GetHashCode();
                            }
                            return hash;
                        },
                        (x, y) =>
                        {
                            if(x.Length == y.Length)
                            {
                                for (int idx = 0; idx < x.Length; idx++)
                                {
                                    if(x[idx] != y[idx])
                                    {
                                        return false;
                                    }
                                }
                                return true;
                            }
                            return false;
                        }));
            }
        }

        /// <summary>
        /// Creates a new memory mapped tags index.
        /// </summary>
        /// <param name="file"></param>
        public TagsIndex(MemoryMappedFile file)
        {
            _stringIndex = new MemoryMappedIndex<string>(file, MemoryMappedDelegates.ReadFromString, MemoryMappedDelegates.WriteToString);
            _tagsIndex = new MemoryMappedIndex<int[]>(file, MemoryMappedDelegates.ReadFromIntArray, MemoryMappedDelegates.WriteToIntArray);

            _tagsReverseIndex = new MemoryMappedHugeDictionary<int[], uint>(file, MemoryMappedDelegates.ReadFromIntArray, MemoryMappedDelegates.WriteToIntArray,
                MemoryMappedDelegates.ReadFromUInt32, MemoryMappedDelegates.WriteToUInt32, (x) =>
            {
                var hash = 0;
                for (int idx = 0; idx < x.Length; idx++)
                {
                    hash = hash ^ x[idx].GetHashCode();
                }
                return hash;
            },
            (x, y) =>
            {
                var comp = x.Length.CompareTo(y.Length);
                if(comp == 0)
                {
                    for (int idx = 0; idx < x.Length; idx++)
                    {
                        comp = x[idx].CompareTo(y[idx]);
                        if(comp != 0)
                        {
                            return comp;
                        }
                    }
                    return 0;
                }
                return comp;
            });
            _stringReverseIndex = new MemoryMappedHugeDictionary<string, int>(file, MemoryMappedDelegates.ReadFromString, MemoryMappedDelegates.WriteToString,
                MemoryMappedDelegates.ReadFromInt32, MemoryMappedDelegates.WriteToInt32);
        }

        /// <summary>
        /// Creates a new tags index given an existing string index and tags index.
        /// </summary>
        /// <param name="stringIndex"></param>
        /// <param name="tagsIndex"></param>
        internal TagsIndex(IndexBase<string> stringIndex, IndexBase<int[]> tagsIndex)
        {
            _stringIndex = stringIndex;
            _tagsIndex = tagsIndex;

            _stringReverseIndex = null;
            _tagsReverseIndex = null;
        }

        /// <summary>
        /// Returns true if this tags index is readonly.
        /// </summary>
        public bool IsReadonly
        {
            get { return _stringReverseIndex == null; }
        }

        /// <summary>
        /// Returns the tags that belong to the given id.
        /// </summary>
        /// <param name="tagsId"></param>
        /// <returns></returns>
        public TagsCollectionBase Get(uint tagsId)
        {
            return new InternalTagsCollection(_stringIndex, _tagsIndex.Get(tagsId));
        }

        /// <summary>
        /// Adds new tags.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public uint Add(TagsCollectionBase tags)
        {
            if (_stringReverseIndex == null)
            { // this index is readonly.
                throw new InvalidOperationException("This tags index is readonly. Check IsReadonly.");
            }
            else
            { // add new collection.
                var sortedSet = new SortedSet<long>();
                foreach(var tag in tags)
                {
                    int keyId;
                    if(!_stringReverseIndex.TryGetValue(tag.Key, out keyId))
                    { // the key doesn't exist yet.
                        keyId = (int)_stringIndex.Add(tag.Key);
                        _stringReverseIndex.Add(tag.Key, keyId);
                    }
                    int valueId;
                    if (!_stringReverseIndex.TryGetValue(tag.Value, out valueId))
                    { // the key doesn't exist yet.
                        valueId = (int)_stringIndex.Add(tag.Value);
                        _stringReverseIndex.Add(tag.Value, valueId);
                    }
                    sortedSet.Add((long)keyId + (long)int.MaxValue * (long)valueId);
                }

                // sort keys.
                var sorted = new int[sortedSet.Count * 2];
                var idx = 0;
                foreach (var pair in sortedSet)
                {
                    sorted[idx] = (int)(pair % int.MaxValue);
                    idx++;
                    sorted[idx] = (int)(pair / int.MaxValue);
                    idx++;
                }

                // check duplicates.
                uint tagsId;
                if(_tagsReverseIndex.TryGetValue(sorted, out tagsId))
                { // collection already exists.
                    return tagsId;
                }

                tagsId = (uint)_tagsIndex.Add(sorted);
                _tagsReverseIndex.Add(sorted, tagsId);
                return tagsId;
            }
        }

        /// <summary>
        /// An implementation of a tags collection.
        /// </summary>
        private class InternalTagsCollection : TagsCollectionBase
        {
            /// <summary>
            /// Holds the string index.
            /// </summary>
            private IndexBase<string> _stringIndex;

            /// <summary>
            /// Holds the tags.
            /// </summary>
            private int[] _tags;

            /// <summary>
            /// Creates a new internal tags collection.
            /// </summary>
            /// <param name="stringIndex"></param>
            /// <param name="tags"></param>
            public InternalTagsCollection(IndexBase<string> stringIndex, int[] tags)
            {
                _stringIndex = stringIndex;
                _tags = tags;
            }

            /// <summary>
            /// Returns the number of tags in this collection.
            /// </summary>
            public override int Count
            {
                get { return _tags.Length / 2; }
            }

            /// <summary>
            /// Returns true if this collection is readonly.
            /// </summary>
            public override bool IsReadonly
            {
                get { return true; }
            }

            /// <summary>
            /// Adds a key-value pair to this tags collection.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            public override void Add(string key, string value)
            {
                throw new InvalidOperationException("This tags collection is readonly. Check IsReadonly.");
            }

            /// <summary>
            /// Adds a tag.
            /// </summary>
            /// <param name="tag"></param>
            public override void Add(Tag tag)
            {
                throw new InvalidOperationException("This tags collection is readonly. Check IsReadonly.");
            }

            /// <summary>
            /// Adds a tag or replace the existing value if any.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            public override void AddOrReplace(string key, string value)
            {
                throw new InvalidOperationException("This tags collection is readonly. Check IsReadonly.");
            }

            /// <summary>
            /// Adds a tag or replace the existing value if any.
            /// </summary>
            /// <param name="tag"></param>
            public override void AddOrReplace(Tag tag)
            {
                throw new InvalidOperationException("This tags collection is readonly. Check IsReadonly.");
            }

            /// <summary>
            /// Returns true if the given tag exists.
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public override bool ContainsKey(string key)
            {
                for(int idx = 0; idx < _tags.Length; idx = idx + 2)
                {
                    if(key == _stringIndex.Get(_tags[idx]))
                    { // key found!
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// Returns true if the given tag exists.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public override bool TryGetValue(string key, out string value)
            {
                for (int idx = 0; idx < _tags.Length; idx = idx + 2)
                {
                    if (key == _stringIndex.Get(_tags[idx]))
                    { // key found!
                        value = _stringIndex.Get(_tags[idx + 1]);
                        return true;
                    }
                }
                value = null;
                return false;
            }

            /// <summary>
            /// Returns true if the given tag exists with the given value.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public override bool ContainsKeyValue(string key, string value)
            {
                for (int idx = 0; idx < _tags.Length; idx = idx + 2)
                {
                    if (key == _stringIndex.Get(_tags[idx]) &&
                        value == _stringIndex.Get(_tags[idx + 1]))
                    { // key found!
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// Removes all tags with the given key.
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public override bool RemoveKey(string key)
            {
                throw new InvalidOperationException("This tags collection is readonly. Check IsReadonly.");
            }

            /// <summary>
            /// Removes the given tag.
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public override bool RemoveKeyValue(string key, string value)
            {
                throw new InvalidOperationException("This tags collection is readonly. Check IsReadonly.");
            }

            /// <summary>
            /// Clears all tags.
            /// </summary>
            public override void Clear()
            {
                throw new InvalidOperationException("This tags collection is readonly. Check IsReadonly.");
            }

            /// <summary>
            /// Removes all tags that match the given criteria.
            /// </summary>
            /// <param name="predicate"></param>
            public override void RemoveAll(Predicate<Tag> predicate)
            {
                throw new InvalidOperationException("This tags collection is readonly. Check IsReadonly.");
            }

            /// <summary>
            /// Returns the enumerator for this enumerable.
            /// </summary>
            /// <returns></returns>
            public override IEnumerator<Tag> GetEnumerator()
            {
                return new InternalTagsEnumerator(_stringIndex, _tags);
            }
        }

        /// <summary>
        /// An internal implementation of a tags enumerator.
        /// </summary>
        private class InternalTagsEnumerator : IEnumerator<Tag>
        {
            /// <summary>
            /// Holds the string index.
            /// </summary>
            private IndexBase<string> _stringIndex;

            /// <summary>
            /// Holds the tags.
            /// </summary>
            private int[] _tags;

            /// <summary>
            /// Creates a new internal tags collection.
            /// </summary>
            /// <param name="stringIndex"></param>
            /// <param name="tags"></param>
            public InternalTagsEnumerator(IndexBase<string> stringIndex, int[] tags)
            {
                _stringIndex = stringIndex;
                _tags = tags;
            }

            /// <summary>
            /// Holds the current idx.
            /// </summary>
            private int _idx = -2;

            /// <summary>
            /// Returns the current tag.
            /// </summary>

            public Tag Current
            {
                get
                {
                    return new Tag()
                    {
                        Key = _stringIndex.Get(_tags[_idx]),
                        Value = _stringIndex.Get(_tags[_idx + 1])
                    };
                }
            }

            /// <summary>
            /// Returns the current tag.
            /// </summary>
            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return new Tag()
                    {
                        Key = _stringIndex.Get(_tags[_idx]),
                        Value = _stringIndex.Get(_tags[_idx + 1])
                    };
                }
            }

            /// <summary>
            /// Move to the next tag.
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                _idx = _idx + 2;
                return _idx < _tags.Length;
            }

            /// <summary>
            /// Resets this enumerator.
            /// </summary>
            public void Reset()
            {
                _idx = -2;
            }

            /// <summary>
            /// Disposes this enumerator.
            /// </summary>
            public void Dispose()
            {
                _tags = null;
                _stringIndex = null;
            }
        }

        #region Serialization

        /// <summary>
        /// Serializes this tags index to the given stream.
        /// </summary>
        /// <param name="stream">The stream to write to. Writing will start at position 0.</param>
        public long Serialize(System.IO.Stream stream)
        { // serialize the tags and strings index.
            var size = _tagsIndex.Serialize(stream);
            var limitedStream = new LimitedStream(stream);
            return _stringIndex.Serialize(limitedStream) + size;
        }

        /// <summary>
        /// Deserializes a tags index from the given stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static TagsIndex Deserialize(System.IO.Stream stream)
        {
            long size;
            var tagsIndex = MemoryMappedIndex<int[]>.Deserialize(stream, MemoryMappedDelegates.ReadFromIntArray, MemoryMappedDelegates.WriteToIntArray, false, out size);
            stream.Seek(size, System.IO.SeekOrigin.Begin);
            var limitedStream = new LimitedStream(stream);
            var stringIndex = MemoryMappedIndex<string>.Deserialize(limitedStream, MemoryMappedDelegates.ReadFromString, MemoryMappedDelegates.WriteToString, false);

            return new TagsIndex(stringIndex, tagsIndex);
        }

        #endregion
    }
}