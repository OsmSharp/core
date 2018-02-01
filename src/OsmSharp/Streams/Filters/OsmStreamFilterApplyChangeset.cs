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

using OsmSharp.Changesets;
using OsmSharp.Db;
using System;
using System.Collections.Generic;
using System.Text;

namespace OsmSharp.Streams.Filters
{
    /// <summary>
    /// Applies a filter that applies a set of changesets.
    /// </summary>
    public class OsmStreamFilterApplyChangeset : OsmStreamFilter
    {
        private HashSet<OsmGeoKey> _deletions;
        private List<OsmGeo> _creations;
        private Dictionary<OsmGeoKey, OsmGeo> _modifications;

        /// <summary>
        /// Creates a new apply changeset filter.
        /// </summary>
        public OsmStreamFilterApplyChangeset(params OsmChange[] changes)
        {
            _creations = new List<OsmGeo>();
            _deletions = new HashSet<OsmGeoKey>();
            _modifications = new Dictionary<OsmGeoKey, OsmGeo>();

            var changeset = changes.Squash();

            if (changeset.Create != null)
            {
                foreach(var create in changeset.Create)
                {
                    _creations.Add(create);
                }

                _creations.Sort((x, y) => x.CompareByIdVersionAndType(y));
            }
            if (changeset.Delete != null)
            {
                foreach (var del in changeset.Delete)
                {
                    _deletions.Add(new OsmGeoKey(del));
                }
            }
            if (changeset.Modify != null)
            {
                foreach (var mod in changeset.Modify)
                {
                    _modifications[new OsmGeoKey(mod)] = mod;
                }
            }
        }

        /// <summary>
        /// Returns true if this filter can be reset.
        /// </summary>
        public override bool CanReset => this.Source.CanReset;

        /// <summary>
        /// Returns true if this filter returns objects sorted.
        /// </summary>
        public override bool IsSorted => this.Source.IsSorted;

        private OsmStreamSource _mergedSource = null;
        private OsmGeo _current = null;

        /// <summary>
        /// Returns the current object.
        /// </summary>
        /// <returns></returns>
        public override OsmGeo Current()
        {
            return _current;
        }

        /// <summary>
        /// Move to the next object.
        /// </summary>
        public override bool MoveNext(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
        {
            if (_mergedSource == null)
            {
                var mergedSource = new OsmStreamFilterMerge();
                mergedSource.RegisterSource(this.Source);
                mergedSource.RegisterSource(_creations);
                _mergedSource = mergedSource;
            }

            OsmGeo modified;
            while (_mergedSource.MoveNext(ignoreNodes, ignoreWays, ignoreRelations))
            {
                // returns the next out of the merged source of the creations and the source.
                _current = _mergedSource.Current();

                // check for deletions or modifications.
                var key = new OsmGeoKey(_current);
                if (_deletions.Contains(key))
                {
                    // move next.
                    _current = null;
                    continue;
                }
                else if (_modifications.TryGetValue(key, out modified))
                {
                    _current = modified;
                }
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Resets this filter.
        /// </summary>
        public override void Reset()
        {
            this.Source.Reset();

            _mergedSource = null;
        }
    }
}
