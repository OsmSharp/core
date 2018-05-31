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

using OsmSharp.Db;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Changesets
{
    /// <summary>
    /// Contains extension methods related to OsmChange objects.
    /// </summary>
    public static class OsmChangeExtensions
    {
        /// <summary>
        /// Squashes the given changesets into one that has the same effect.
        /// </summary>
        public static OsmChange Squash(this IEnumerable<OsmChange> changes)
        {
            var deletions = new Dictionary<OsmGeoVersionKey, OsmGeo>();
            var modifications = new Dictionary<OsmGeoVersionKey, OsmGeo>();
            var creations = new Dictionary<OsmGeoVersionKey, OsmGeo>();

            foreach(var change in changes)
            {
                if (change.Create != null)
                {
                    foreach(var create in change.Create)
                    { // just add all creates as normal.
                        creations[new OsmGeoVersionKey(create)] = create;
                    }
                }

                if (change.Modify != null)
                {
                    foreach(var mod in change.Modify)
                    {
                        OsmGeo creationOfMod;
                        var key = new OsmGeoVersionKey(mod);
                        if (creations.TryGetPreviousVersion(key, out creationOfMod))
                        { // don't modify a new creation, create it properly once.
                            creations.Remove(new OsmGeoVersionKey(creationOfMod));
                            creations[key] = mod;
                        }
                        else
                        { // add to modifications as normal.
                            modifications[key] = mod;
                        }
                    }
                }

                if (change.Delete != null)
                {
                    foreach(var del in change.Delete)
                    {
                        var key = new OsmGeoVersionKey(del);
                        if (creations.ContainsKey(key))
                        { // don't delete after create, just don't create.
                            creations.Remove(key);
                        }
                        else
                        { // add delete as normal.
                            deletions[new OsmGeoVersionKey(del)] = del;
                        }
                    }
                }
            }

            return new OsmChange()
            {
                Create = creations.Values.ToArray(),
                Delete = deletions.Values.ToArray(),
                Modify = modifications.Values.ToArray(),
                Generator = "OsmSharp",
                Version = 6
            };
        }

        /// <summary>
        /// Returns true if the given key is in the dictionary.
        /// </summary>
        private static bool ContainsPreviousVersion(this Dictionary<OsmGeoVersionKey, OsmGeo> data, OsmGeoVersionKey key)
        {
            OsmGeo result;
            return data.TryGetPreviousVersion(key, out result);
        }


        /// <summary>
        /// Tries to get a previous version in the given dictionary.
        /// </summary>
        private static bool TryGetPreviousVersion(this Dictionary<OsmGeoVersionKey, OsmGeo> data, OsmGeoVersionKey key,
            out OsmGeo result)
        {
            var previous = key;
            result = null;
            while (!data.TryGetValue(previous, out result))
            {
                previous = previous.Previous;

                if (previous == null)
                {
                    return false;
                }
            }
            return true;
        }
    }
}