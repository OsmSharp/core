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

using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Tags
{
    /// <summary>
    /// A tags collection.
    /// </summary>
    public class TagsCollection : TagsCollectionBase
    {
        private readonly Dictionary<string, string> _tags;

        /// <summary>
        /// Creates a new tags collection.
        /// </summary>
        public TagsCollection()
        {
            _tags = new Dictionary<string, string>();
        }

        /// <summary>
        /// Creates a new tags collection.
        /// </summary>
        public TagsCollection(int capacity)
        {
            _tags = new Dictionary<string, string>(capacity);
        }

        /// <summary>
        /// Creates a new tags collection.
        /// </summary>
        public TagsCollection(params Tag[] tags)
            : this((IEnumerable<Tag>)tags)
        {

        }

        /// <summary>
        /// Creates a new tags collection.
        /// </summary>
        public TagsCollection(IEnumerable<Tag> tags)
        {
            _tags = new Dictionary<string, string>();

            if(tags != null)
            {
                foreach(var tag in tags)
                {
                    this.AddOrReplace(tag);
                }
            }
        }

        /// <summary>
        /// Creates a new tags collection.
        /// </summary>
        public TagsCollection(IDictionary<string, string> tags)
        {
            _tags = new Dictionary<string, string>();

            if (tags != null)
            {
                foreach (var pair in tags)
                {
                    _tags.Add(pair.Key, pair.Value);
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
        /// Adds or replaces a tag.
        /// </summary>
        public override void AddOrReplace(Tag tag)
        {
            _tags[tag.Key] = tag.Value;
        }

        /// <summary>
        /// Clears all tags.
        /// </summary>
        public override void Clear()
        {
            _tags.Clear();
        }

        /// <summary>
        /// Removes the tag with the given key.
        /// </summary>
        public override bool RemoveKey(string key)
        {
            return _tags.Remove(key);
        }

        /// <summary>
        /// Gets the value for the given key and returns true if the given key exists.
        /// </summary>
        public override bool TryGetValue(string key, out string value)
        {
            return _tags.TryGetValue(key, out value);
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public override IEnumerator<Tag> GetEnumerator()
        {
            return _tags.Select(x => new Tag()
            {
                Key = x.Key,
                Value = x.Value
            }).GetEnumerator();
        }
    }
}