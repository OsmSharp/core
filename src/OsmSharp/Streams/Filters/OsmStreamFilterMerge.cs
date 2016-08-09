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
using OsmSharp.Tags;

namespace OsmSharp.Streams.Filters
{
    /// <summary>
    /// A filter to merge two sorted stream together.
    /// </summary>
    public class OsmStreamFilterMerge : OsmStreamFilter
    {
        private OsmStreamSource _source1;
        private OsmStreamSource _source2;

        /// <summary>
        /// Creates a new filter.
        /// </summary>
        public OsmStreamFilterMerge()
        {
            _resolutionType = ConflictResolutionType.FirstStream;
        }

        /// <summary>
        /// Creates a new filter.
        /// </summary>
        public OsmStreamFilterMerge(ConflictResolutionType resolutionType)
        {
            _resolutionType = resolutionType;
        }

        /// <summary>
        /// Returns true if this filter can be reset.
        /// </summary>
        public override bool CanReset
        {
            get
            {
                return _source1.CanReset && _source2.CanReset;
            }
        }

        /// <summary>
        /// Resets this filter.
        /// </summary>
        public override void Reset()
        {
            _source1.Reset();
            _source2.Reset();

            _source1Status = null;
            _source2Status = null;
        }

        /// <summary>
        /// Returns true if this filter returns sorted objects.
        /// </summary>
        public override bool IsSorted
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets all meta-data.
        /// </summary>
        public override TagsCollection GetAllMeta()
        {
            var tags = new TagsCollection();
            tags.AddOrReplace(_source1.GetAllMeta());
            tags.AddOrReplace(_source2.GetAllMeta());
            return tags;
        }

        /// <summary>
        /// Registers a source.
        /// </summary>
        public override void RegisterSource(OsmStreamSource source)
        {
            if (_source1 == null)
            {
                _source1 = source;
                return;
            }
            _source2 = source;
        }

        private OsmGeo _current;
        private bool? _source1Status = null; // false when finished, true when there is data, null when unintialized.
        private bool? _source2Status = null; // false when finished, true when there is data, null when unintialized.
        private ConflictResolutionType _resolutionType = ConflictResolutionType.None;

        /// <summary>
        /// Move to the next object.
        /// </summary>
        public override bool MoveNext(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
        {
            OsmGeo source1Current = null;
            OsmGeo source2Current = null;

            // get currents or move to first.
            if (_source1Status == null)
            {
                _source1Status = _source1.MoveNext(ignoreNodes, ignoreWays, ignoreRelations);
            }
            if (_source1Status.Value)
            {
                source1Current = _source1.Current();
            }
            if (_source2Status == null)
            {
                _source2Status = _source2.MoveNext(ignoreNodes, ignoreWays, ignoreRelations);
            }
            if (_source2Status.Value)
            {
                source2Current = _source2.Current();
            }

            // compare currents and select next.
            OsmGeo newCurrent;
            if (source1Current == null && source2Current == null)
            {
                return false;
            }
            else if(source1Current == null)
            {
                newCurrent = source2Current;
                _source2Status = _source2.MoveNext(ignoreNodes, ignoreWays, ignoreRelations);
            }
            else if(source2Current == null)
            {
                newCurrent = source1Current;
                _source1Status = _source1.MoveNext(ignoreNodes, ignoreWays, ignoreRelations);
            }
            else 
            {
                var comp = source1Current.CompareByIdAndType(source2Current);

                if (comp == 0)
                { // oeps, conflict here!
                    if (_resolutionType == ConflictResolutionType.None)
                    { // no conflict resolution return both.
                        newCurrent = source1Current;
                        _source1Status = _source1.MoveNext(ignoreNodes, ignoreWays, ignoreRelations);
                    }
                    else if(_resolutionType == ConflictResolutionType.FirstStream)
                    { // return only the object from the first stream.
                        newCurrent = source1Current;
                        _source1Status = _source1.MoveNext(ignoreNodes, ignoreWays, ignoreRelations);
                        _source2Status = _source2.MoveNext(ignoreNodes, ignoreWays, ignoreRelations);
                    }
                    else
                    {
                        throw new NotImplementedException(string.Format("Conflict resolution {0} not implemented.", _resolutionType));
                    }
                }
                else if (comp < 0)
                { // return from first stream.
                    newCurrent = source1Current;
                    _source1Status = _source1.MoveNext(ignoreNodes, ignoreWays, ignoreRelations);
                }
                else
                { // return from second stream.
                    newCurrent = source2Current;
                    _source2Status = _source2.MoveNext(ignoreNodes, ignoreWays, ignoreRelations);
                }
            }

            // make sure the result is sorted.
            if (_current != null &&
                _current.CompareByIdAndType(newCurrent) > 0)
            {
                throw new Exceptions.StreamNotSortedException();
            }

            _current = newCurrent;
            return true;
        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        public override OsmGeo Current()
        {
            return _current;
        }
    }

    /// <summary>
    /// Types of conflict resolution.
    /// </summary>
    public enum ConflictResolutionType
    {
        /// <summary>
        /// Keep both entities.
        /// </summary>
        None,
        /// <summary>
        /// Keep object from the first stream only.
        /// </summary>
        FirstStream
    }
}