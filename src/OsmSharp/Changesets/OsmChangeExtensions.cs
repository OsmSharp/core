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
            // keep mutations:
            // type: 
            // - null: DELETE
            // - true: CREATE
            // - false: MODIFY.
            
            // - deletes overrules everything, any create or modify is ignored.
            // - we keep the object with the highest version #.
            
            var mutations = new Dictionary<OsmGeoKey, (OsmGeo osmGeo, bool create, bool delete)>();

            foreach (var change in changes)
            {
                if (change.Delete == null) continue;
                
                foreach (var del in change.Delete)
                {
                    mutations[new OsmGeoKey(del)] = (del, false, true);
                }
            }

            foreach (var change in changes)
            {
                if (change.Create == null) continue;
                
                foreach(var create in change.Create)
                {
                    var key = new OsmGeoKey(create);
                    if (mutations.TryGetValue(key, out var _))
                    {
                        // was deleted already, set the create flag.
                        mutations[key] = (create, true, true);
                        continue;
                    }

                    mutations.Add(key, (create, true, false));
                }
            }

            foreach (var change in changes)
            {
                if (change.Modify == null) continue;
                
                foreach (var mod in change.Modify)
                {
                    var key = new OsmGeoKey(mod);
                    if (mutations.TryGetValue(key, out var current))
                    {
                        if (current.delete)
                        {
                            // was deleted already, ignore modification.
                            continue;
                        }

                        if (current.create)
                        {
                            // was already modified/created.
                            if (current.osmGeo.Version < mod.Version)
                            {
                                // create created, replace by modification.
                                mutations[key] = (mod, true, false);
                            }
                        }
                        else
                        {
                            // was already modified.
                            if (current.osmGeo.Version < mod.Version)
                            {
                                // this version is higher, replace the current one.
                                mutations[key] = (mod, false, false);
                            }
                        }
                    }
                    else
                    {
                        mutations[key] = (mod, false, false);
                    }
                }
            }

            return new OsmChange()
            {
                Create = mutations.Values.Where(x => x.create == true && x.delete == false).Select(x => x.osmGeo).ToArray(),
                Delete = mutations.Values.Where(x => x.create == false && x.delete == true).Select(x => x.osmGeo).ToArray(),
                Modify = mutations.Values.Where(x => x.create == false && x.delete == false).Select(x => x.osmGeo).ToArray(),
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