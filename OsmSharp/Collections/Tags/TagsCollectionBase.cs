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

using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace OsmSharp.Collections.Tags
{
    /// <summary>
    /// Represents a generic tags collection.
    /// </summary>
    public abstract class TagsCollectionBase : IEnumerable<Tag>, IEnumerable<KeyValuePair<string, string>>, ITagsSource
    {
        /// <summary>
        /// Returns the number of tags in this collection.
        /// </summary>
        public abstract int Count { get; }

        /// <summary>
        /// Returns true if this collection is readonly.
        /// </summary>
        public abstract bool IsReadonly { get; }

        /// <summary>
        /// Adds a key-value pair to this tags collection.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public abstract void Add(string key, string value);

        /// <summary>
        /// Adds a tag.
        /// </summary>
        /// <param name="tag"></param>
        public abstract void Add(Tag tag);

        /// <summary>
        /// Adds all tags from the given collection.
        /// </summary>
        /// <param name="tagsCollection"></param>
        public void Add(TagsCollectionBase tagsCollection)
        {
            foreach (Tag tag in tagsCollection)
            {
                this.Add(tag);
            }
        }

        /// <summary>
        /// Adds the tags or replaces the existing value if any.
        /// </summary>
        /// <param name="tagsCollection"></param>
        public void AddOrReplace(TagsCollectionBase tagsCollection)
        {
            foreach (Tag tag in tagsCollection)
            {
                this.AddOrReplace(tag);
            }
        }

        /// <summary>
        /// Adds a tag or replace the existing value if any.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public abstract void AddOrReplace(string key, string value);

        /// <summary>
        /// Adds a tag or replace the existing value if any.
        /// </summary>
        /// <param name="tag"></param>
        public abstract void AddOrReplace(Tag tag);

        /// <summary>
        /// Returns true if the given tag exists.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract bool ContainsKey(string key); 

         /// <summary>
        /// Returns true if one of the given keys exists in this tag collection.
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public virtual bool ContainsOneOfKeys(ICollection<string> keys)
        {
            foreach(var tag in this)
            {
                if(keys.Contains(tag.Key))
                {
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
        public abstract bool TryGetValue(string key, out string value);

        /// <summary>
        /// Returns true if the given tag exists with the given value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract bool ContainsKeyValue(string key, string value);

        /// <summary>
        /// Returns true if the given tags exists.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool Contains(Tag tag)
        {
            return this.ContainsKeyValue(tag.Key, tag.Value);
        }

        /// <summary>
        /// Returns the value associated with the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Throws a KeyNotFoundException when the key does not exists. Use TryGetValue.</returns>
        public virtual string this[string key]
        {
            get
            {
                string value;
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
        /// Returns a parsed numeric value if available.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public double? GetNumericValue(string key)
        {
            string value;
            if (this.TryGetValue(key, out value))
            {
                double numericValue;
                if (double.TryParse(value, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture,
                                    out numericValue))
                {
                    return numericValue;
                }
            }
            return null;
        }

        /// <summary>
        /// Removes all tags with the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract bool RemoveKey(string key);

        /// <summary>
        /// Removes the given tag.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public virtual bool RemoveKeyValue(Tag tag)
        {
            return this.RemoveKeyValue(tag.Key, tag.Value);
        }

        /// <summary>
        /// Removes all tags with the given key-values.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract bool RemoveKeyValue(string key, string value);

        /// <summary>
        /// Clears all tags.
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// Removes all tags that match the given criteria.
        /// </summary>
        /// <param name="predicate"></param>
        public abstract void RemoveAll(System.Predicate<Tag> predicate);

        /// <summary>
        /// Creates a new tags collection with only the given keys.
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public virtual TagsCollectionBase KeepKeysOf(ICollection<string> keys)
        {
            TagsCollection collection = new TagsCollection(this.Count);
            foreach(var tag in this)
            {
                if(keys.Contains(tag.Key))
                {
                    collection.Add(tag);
                }
            }
            return collection;
        }

        /// <summary>
        /// Intersects this tags collection with the given one. Removes all tags from this collection that do not occur on the given one.
        /// </summary>
        /// <param name="tags"></param>
        public void Intersect(TagsCollectionBase tags)
        {
            var toRemove = new List<Tag>();
            foreach (var tag in this)
            {
                if(!tags.Contains(tag))
                {
                    toRemove.Add(tag);
                }
            }
            foreach(var tag in toRemove)
            {
                this.RemoveKeyValue(tag);
            }
        }

        /// <summary>
        /// Converts this tags collection to a dictionary with string keys and object values.
        /// </summary>
        /// <remarks>Duplicate keys are overwritten by the last occuring value.</remarks>
        /// <returns></returns>
        public Dictionary<string, object> ToStringObjectDictionary()
        {
            var dic = new Dictionary<string, object>();
            foreach(var tag in this)
            {
                dic[tag.Key] = tag.Value;
            }
            return dic;
        }

        /// <summary>
        /// Converts this tags collection to a dictionary with string keys and string values.
        /// </summary>
        /// <remarks>Duplicate keys are overwritten by the last occuring value.</remarks>
        /// <returns></returns>
        public Dictionary<string, string> ToStringStringDictionary()
        {
            var dic = new Dictionary<string, string>();
            foreach (var tag in this)
            {
                dic[tag.Key] = tag.Value;
            }
            return dic;
        }

        #region IEnumerable<Tag>

        /// <summary>
        /// Returns the enumerator for this enumerable.
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator<Tag> GetEnumerator();

        /// <summary>
        /// Returns the enumerator for this enumerable.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Returns the enumerable for KeyValuePairs.
        /// </summary>
        /// <returns></returns>
        #region IEnumerable<KeyValuePair<string, string>>

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
        
        /// <summary>
        /// A KeyValuePair enumerator wrapper around a IEnumerator tag enumerator.
        /// </summary>
        private class KeyValuePairEnumerator : IEnumerator<KeyValuePair<string, string>>
        {
            private IEnumerator<Tag> _tagEnumerator;

            public KeyValuePairEnumerator(IEnumerator<Tag> tagEnumerator)
            {
                _tagEnumerator = tagEnumerator;
            }

            public KeyValuePair<string, string> Current
            {
                get { return new KeyValuePair<string,string>(_tagEnumerator.Current.Key, _tagEnumerator.Current.Value); }
            }

            public void Dispose()
            {
                _tagEnumerator.Dispose();
            }

            object IEnumerator.Current
            {
                get { return this.Current; }
            }

            public bool MoveNext()
            {
                return _tagEnumerator.MoveNext();
            }

            public void Reset()
            {
                _tagEnumerator.Reset();
            }
        }

        #endregion

        #region Equals

        /// <summary>
        /// Returns true if the objects represent the same information.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!object.ReferenceEquals(this, obj))
            {
                if (obj is TagsCollectionBase)
                {
                    TagsCollectionBase other = (obj as TagsCollectionBase);
                    if (other.Count == this.Count)
                    {
                        // make sure all object in the first are in the second and vice-versa.
                        foreach(var tag in this)
                        {
                            if (!other.Contains(tag))
                            {
                                return false;
                            }
                        }
                        foreach (var tag in other)
                        {
                            if (!this.Contains(tag))
                            {
                                return false;
                            }
                        }
                        return true; // no loop was done without finding the same key-value pair.
                    }
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// Serves as a hash function.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int hashCode = this.Count.GetHashCode();
            foreach(var tag in this)
            {
                hashCode = hashCode ^ tag.GetHashCode();
            }
            return hashCode;
        }

        #endregion
    }
}