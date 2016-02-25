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

using OsmSharp.Complete;
using OsmSharp.Db;
using OsmSharp.Db.Impl;
using System;
using System.Collections.Generic;

namespace OsmSharp.Streams.Complete
{
    /// <summary>
    /// Represents a complete stream source that converts a simple stream into a complete stream.
    /// </summary>
    public class OsmSimpleCompleteStreamSource : OsmCompleteStreamSource
    {
        private readonly ISnapshotDb _dataCache; // Caches objects that are needed later to complete objects.
        private readonly OsmStreamSource _simpleSource; // Holds the simple source of object.

        /// <summary>
        /// Creates a new osm simple complete stream.
        /// </summary>
        public OsmSimpleCompleteStreamSource(OsmStreamSource source)
        {
            // create an in-memory cache by default.
            _dataCache = (new MemorySnapshotDb()).CreateSnapshotDb();
            _simpleSource = source;

            _nodesToInclude = new HashSet<long>();
            _nodesUsedTwiceOrMore = new Dictionary<long, int>();
            _waysToInclude = new HashSet<long>();
            _waysUsedTwiceOrMore = new Dictionary<long, int>();
            _relationsToInclude = new HashSet<long>();
            _relationsUsedTwiceOrMore = new Dictionary<long, int>();
        }

        /// <summary>
        /// Creates a new osm simple complete stream.
        /// </summary>
        public OsmSimpleCompleteStreamSource(OsmStreamSource source, ISnapshotDb cache)
        {
            _dataCache = cache;
            _simpleSource = source;

            _nodesToInclude = new HashSet<long>();
            _nodesUsedTwiceOrMore = new Dictionary<long, int>();
            _waysToInclude = new HashSet<long>();
            _waysUsedTwiceOrMore = new Dictionary<long, int>();
            _relationsToInclude = new HashSet<long>();
            _relationsUsedTwiceOrMore = new Dictionary<long, int>();
        }

        /// <summary>
        /// Initializes this source.
        /// </summary>
        public override void Initialize()
        {
            _cachingDone = false;
            _nodesToInclude.Clear();
            _nodesUsedTwiceOrMore.Clear();
            _waysToInclude.Clear();
            _waysUsedTwiceOrMore.Clear();
            _relationsToInclude.Clear();
            _relationsUsedTwiceOrMore.Clear();

            if (!_simpleSource.CanReset)
            { // the simple source cannot be reset, each object can be a child, no other option than caching everything!
                // TODO: support this scenario, can be usefull when streaming data from a non-seekable stream.
                throw new NotSupportedException("Creating a complete stream from a non-resettable simple stream is not supported. Wrap the source stream and create a resettable stream.");
            }
            else
            { // the simple source can be reset.

            }
        }

        private bool _cachingDone; // Flag indicating that the caching was done.
        private ICompleteOsmGeo _current; // Holds the current complete object.

        /// <summary>
        /// Moves to the next object.
        /// </summary>
        public override bool MoveNext()
        {
            if (!_cachingDone)
            { // first seek & cache.
                this.Seek();
                _cachingDone = true;
            }

            while (_simpleSource.MoveNext())
            { // there is data.
                var currentSimple = _simpleSource.Current();

                switch (currentSimple.Type)
                {
                    case OsmGeoType.Node:
                        // create complete version.
                        _current = currentSimple as Node;

                        if (_current != null && _current.Tags == null)
                        { // make sure nodes have a default tag collection that is empty not null.
                            _current.Tags = new OsmSharp.Tags.TagsCollection();
                        }
                        if (_nodesToInclude.Contains(currentSimple.Id.Value))
                        { // node needs to be cached.
                            _dataCache.AddOrUpdate(currentSimple as Node);
                            _nodesToInclude.Remove(currentSimple.Id.Value);
                        }
                        break;
                    case OsmGeoType.Way:
                        // create complete way.
                        _current = (currentSimple as Way).CreateComplete(_dataCache);

                        if (_waysToInclude.Contains(currentSimple.Id.Value))
                        { // keep the way because it is needed later on.
                            _dataCache.AddOrUpdate(currentSimple as Way);
                            _waysToInclude.Remove(currentSimple.Id.Value);
                        }
                        else
                        { // only report that the nodes have been used when the way can be let-go.
                            var way = currentSimple as Way;
                            if (way.Nodes != null)
                            { // report usage.
                                for(var i = 0; i < way.Nodes.Length; i++)
                                {
                                    this.ReportNodeUsage(way.Nodes[i]);
                                }
                            }
                        }
                        break;
                    case OsmGeoType.Relation:
                        // create complate relation.
                        _current = (currentSimple as Relation).CreateComplete(_dataCache);

                        if (!_relationsToInclude.Contains(currentSimple.Id.Value))
                        { // only report relation usage when the relation can be let go.
                            var relation = currentSimple as Relation;
                            if (relation.Members != null)
                            {
                                foreach (var member in relation.Members)
                                {
                                    switch (member.Type)
                                    {
                                        case OsmGeoType.Node:
                                            this.ReportNodeUsage(member.Id);
                                            break;
                                        case OsmGeoType.Way:
                                            this.ReportWayUsage(member.Id);
                                            break;
                                        case OsmGeoType.Relation:
                                            this.ReportRelationUsage(member.Id);
                                            break;
                                    }
                                }
                            }
                        }
                        break;
                }
                if (_current != null)
                { // only return complete objects.
                    return true;
                }
            }
            return false;
        }

        private readonly HashSet<long> _nodesToInclude; // An index of extra nodes to include.
        private readonly Dictionary<long, int> _nodesUsedTwiceOrMore; // An index of nodes that are being used twice or more.
        private readonly HashSet<long> _waysToInclude; // An index of extra relations to include.
        private readonly Dictionary<long, int> _waysUsedTwiceOrMore; // An index of ways that are being used twice or more.
        private readonly HashSet<long> _relationsToInclude; // An index of extra ways to include.
        private readonly Dictionary<long, int> _relationsUsedTwiceOrMore; // An index of relations that are being used twice or more.

        /// <summary>
        /// Seeks for objects that are children of other objects.
        /// </summary>
        private void Seek()
        {
            var relations = new List<Relation>();
            while (_simpleSource.MoveNext(true, false, false))
            {
                var osmGeo = _simpleSource.Current();

                switch (osmGeo.Type)
                {
                    case OsmGeoType.Way:
                        var way = (osmGeo as Way);
                        if (way.Nodes != null)
                        {
                            foreach (var nodeId in way.Nodes)
                            {
                                this.MarkNodeAsChild(nodeId);
                            }
                        }
                        break;
                    case OsmGeoType.Relation:
                        var relation = (osmGeo as Relation);
                        if (relation.Members != null)
                        {
                            foreach (var member in relation.Members)
                            {
                                switch (member.Type)
                                {
                                    case OsmGeoType.Node:
                                        this.MarkNodeAsChild(member.Id);
                                        break;
                                    case OsmGeoType.Way:
                                        this.MarkWayAsChild(member.Id);
                                        break;
                                    case OsmGeoType.Relation:
                                        this.MarkRelationAsChild(member.Id);
                                        break;
                                }
                            }
                        }
                        relations.Add(relation);
                        break;
                }
            }
            foreach (var relation in relations)
            {
                if (_relationsToInclude.Contains(relation.Id.Value))
                { // yep, cache relation!
                    _dataCache.AddOrUpdate(relation);
                }
            }
            _simpleSource.Reset();
        }

        /// <summary>
        /// Reports that the node with the given id was used.
        /// </summary>
        private void ReportNodeUsage(long nodeId)
        {
            int nodeCount;
            if (_nodesUsedTwiceOrMore.TryGetValue(nodeId, out nodeCount))
            { // node is used twice or more.
                nodeCount--;
                if (nodeCount > 0)
                { // update count.
                    _nodesUsedTwiceOrMore[nodeId] = nodeCount;
                }
                else
                { // remove twice or more.
                    _nodesUsedTwiceOrMore.Remove(nodeId);
                }
            }
            else
            { // the node was used for the last time.
                _dataCache.DeleteNode(nodeId);
            }
        }

        /// <summary>
        /// Marks the given node as child.
        /// </summary>
        private void MarkNodeAsChild(long nodeId)
        {
            if (_nodesToInclude.Contains(nodeId))
            { // increase the node counter.
                int nodeCount;
                if (!_nodesUsedTwiceOrMore.TryGetValue(nodeId, out nodeCount))
                { // the node is used twice or more.
                    nodeCount = 1;
                    _nodesUsedTwiceOrMore.Add(nodeId, nodeCount);
                }
                else
                { // increase the count.
                    nodeCount++;
                    _nodesUsedTwiceOrMore[nodeId] = nodeCount;
                }
            }
            else
            {
                // just add the node.
                _nodesToInclude.Add(nodeId);
            }
        }

        /// <summary>
        /// Reports that the way with the given id was used.
        /// </summary>
        private void ReportWayUsage(long wayId)
        {
            int wayCount;
            if (_waysUsedTwiceOrMore.TryGetValue(wayId, out wayCount))
            { // way is used twice or more.
                wayCount--;
                if (wayCount > 0)
                { // update count.
                    _waysUsedTwiceOrMore[wayId] = wayCount;
                }
                else
                { // remove twice or more.
                    _waysUsedTwiceOrMore.Remove(wayId);
                }
            }
            else
            { // the way was used for the last time.
                var way = _dataCache.GetWay(wayId); // get the way before it is removed.
                _dataCache.DeleteWay(wayId); // remove from cache.
                if (way != null && way.Nodes != null)
                { // report usage.
                    for(var i = 0; i < way.Nodes.Length; i++)
                    {
                        this.ReportNodeUsage(way.Nodes[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Marks the given way as child.
        /// </summary>
        private void MarkWayAsChild(long wayId)
        {
            if (_waysToInclude.Contains(wayId))
            { // increase the way counter.
                int wayCount;
                if (!_waysUsedTwiceOrMore.TryGetValue(wayId, out wayCount))
                { // the way is used twice or more.
                    wayCount = 1;
                    _waysUsedTwiceOrMore.Add(wayId, wayCount);
                }
                else
                { // increase the count. 
                    wayCount++;
                    _waysUsedTwiceOrMore[wayId] = wayCount;
                }
            }
            else
            {
                // just add the way.
                _waysToInclude.Add(wayId);
            }
        }

        /// <summary>
        /// Reports that the relation with the given id was used.
        /// </summary>
        private void ReportRelationUsage(long relationId)
        {
            int relationCount;
            if (_relationsUsedTwiceOrMore.TryGetValue(relationId, out relationCount))
            { // relation is used twice or more.
                relationCount--;
                if (relationCount > 0)
                { // update count.
                    _relationsUsedTwiceOrMore[relationId] = relationCount;
                }
                else
                { // remove twice or more.
                    _relationsUsedTwiceOrMore.Remove(relationId);
                }
            }
            else
            { // the relation was used for the last time.
                var relation = _dataCache.GetRelation(relationId); // get relation before it is removed.
                _dataCache.DeleteRelation(relationId); // remove relation.
                _relationsToInclude.Remove(relationId); // remove from the to-include list.
            }
        }

        /// <summary>
        /// Marks the given relation as child.
        /// </summary>
        private void MarkRelationAsChild(long relationId)
        {
            if (_relationsToInclude.Contains(relationId))
            { // increase the relation counter.
                int relationCount;
                if (!_relationsUsedTwiceOrMore.TryGetValue(relationId, out relationCount))
                { // the relation is used twice or more.
                    relationCount = 1;
                    _relationsUsedTwiceOrMore.Add(relationId, relationCount);
                }
                else
                { // increase the count.
                    relationCount++;
                    _relationsUsedTwiceOrMore[relationId] = relationCount;
                }
            }
            else
            {
                // just add the relation.
                _relationsToInclude.Add(relationId);
            }
        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        public override ICompleteOsmGeo Current()
        {
            return _current;
        }

        /// <summary>
        /// Resets this source.
        /// </summary>
        public override void Reset()
        {
            _cachingDone = false;

            _dataCache.Clear();
            _simpleSource.Reset();
        }

        /// <summary>
        /// Returns true if this source can be reset.
        /// </summary>
        public override bool CanReset
        {
            get { return _simpleSource.CanReset; }
        }
    }
}