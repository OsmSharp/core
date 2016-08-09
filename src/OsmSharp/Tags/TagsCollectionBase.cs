// The MIT License (MIT)

// Copyright (c) 2016 Ben Abelshausen

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;

namespace OsmSharp.Tags
{
    /// <summary>
    /// Abstract representation of a tags collection.
    /// </summary>
    public abstract class TagsCollectionBase : IEnumerable<Tag>
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
        /// Adds a tag.
        /// </summary>
        public void Add(Tag tag)
        {
            if (this.ContainsKey(tag.Key))
            {
                throw new ArgumentException("A tag with this key already exists.");
            }

            this.AddOrReplace(tag);
        }

        /// <summary>
        /// Adds a tag.
        /// </summary>
        public void Add(string key, string value)
        {
            this.Add(new Tag()
            {
                Key = key,
                Value = value
            });
        }

        /// <summary>
        /// Adds or replaces a tag.
        /// </summary>
        public abstract void AddOrReplace(Tag tag);

        /// <summary>
        /// Adds or replaces a tag.
        /// </summary>
        public void AddOrReplace(string key, string value)
        {
            this.AddOrReplace(new Tag()
            {
                Key = key,
                Value = value
            });
        }

        /// <summary>
        /// Adds or replaces all tags.
        /// </summary>
        public void AddOrReplace(IEnumerable<Tag> tags)
        {
            foreach (var tag in tags)
            {
                this.AddOrReplace(tag);
            }
        }

        /// <summary>
        /// Returns true if the given tag exists.
        /// </summary>
        public bool ContainsKey(string key)
        {
            string value;
            return this.TryGetValue(key, out value);
        }

        /// <summary>
        /// Returns true if the given tag exists.
        /// </summary>
        public bool Contains(Tag tag)
        {
            string value;
            if (this.TryGetValue(tag.Key, out value))
            {
                return value == tag.Value;
            }
            return false;
        }

        /// <summary>
        /// Returns true if the given tag exists.
        /// </summary>
        public bool Contains(string key, string value)
        {
            return this.Contains(new Tag(key, value));
        }

        /// <summary>
        /// Gets the value for the given key and returns true if the given key exists.
        /// </summary>
        public abstract bool TryGetValue(string key, out string value);

        /// <summary>
        /// Returns the value associated with the given key.
        /// </summary>
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
                this.AddOrReplace( new Tag() { Key = key, Value = value });
            }
        }
        
        /// <summary>
        /// Removes all tags with the given key.
        /// </summary>
        public abstract bool RemoveKey(string key);

        /// <summary>
        /// Removes the given tag.
        /// </summary>
        public bool RemoveKeyValue(Tag tag)
        {
            foreach (var t in this)
            {
                if (t.Key == tag.Key &&
                    t.Value == tag.Value)
                {
                    this.RemoveKey(t.Key);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Clears all tags.
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// Removes all tags that match the given criteria.
        /// </summary>
        public void RemoveAll(Predicate<Tag> predicate)
        {
            var keys = new HashSet<string>();
            foreach (var tag in this)
            {
                if (predicate(tag))
                {
                    keys.Add(tag.Key);
                }
            }
            foreach (var key in keys)
            {
                this.RemoveKey(key);
            }
        }

        /// <summary>
        /// Trims the internal data structures to their minimum size.
        /// </summary>
        public virtual void Trim()
        {

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

        #region Equals

        /// <summary>
        /// Returns true if the given object represent the same information.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!object.ReferenceEquals(this, obj))
            {
                if (obj is TagsCollectionBase)
                {
                    var other = (obj as TagsCollectionBase);
                    if (other.Count == this.Count)
                    {
                        // make sure all object in the first are in the second and vice-versa.
                        foreach (var tag in this)
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
                        return true;
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
            var hashCode = this.Count.GetHashCode();
            foreach (var tag in this)
            {
                hashCode = hashCode ^ tag.GetHashCode();
            }
            return hashCode;
        }

        #endregion
    }
}