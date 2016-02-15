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

using System.Linq;

namespace OsmSharp.Tags
{
    /// <summary>
    /// Contains extensions related to tags.
    /// </summary>
    public static class TagExtensions
    {
        private static string[] BooleanTrueValues = { "yes", "true", "1" };
        private static string[] BooleanFalseValues = { "no", "false", "0" };

        /// <summary>
        /// Returns true if the given key has a value that means false.
        /// </summary>
        public static bool IsFalse(this TagsCollectionBase tags, string key)
        {
            if (tags == null || string.IsNullOrWhiteSpace(key))
                return false;
            string tagValue;
            return tags.TryGetValue(key, out tagValue) &&
                BooleanFalseValues.Contains(tagValue.ToLowerInvariant());
        }

        /// <summary>
        /// Returns true if the given key has a value that means true.
        /// </summary>
        public static bool IsTrue(this TagsCollectionBase tags, string key)
        {
            if (tags == null || string.IsNullOrWhiteSpace(key))
                return false;
            
            string tagValue;
            return tags.TryGetValue(key, out tagValue) &&
                BooleanTrueValues.Contains(tagValue.ToLowerInvariant());
        }
    }
}