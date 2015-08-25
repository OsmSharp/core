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

using System;
using System.Collections.Generic;
using OsmSharp.Osm.Cache;

namespace OsmSharp.Osm.Streams.Complete
{
    /// <summary>
    /// Represents a complete stream source that converts a simple stream into a complete stream.
    /// </summary>
    public class OsmSimpleCompleteStreamSource : OsmCompleteStreamSource
    {
        /// <summary>
        /// Caches objects that are needed later to complete objects.
        /// </summary>
        private readonly OsmDataCache _dataCache;

        /// <summary>
        /// Holds the simple source of object.
        /// </summary>
        private readonly OsmStreamSource _simpleSource;

        /// <summary>
        /// Creates a new osm simple complete stream.
        /// </summary>
        /// <param name="source"></param>
        public OsmSimpleCompleteStreamSource(OsmStreamSource source)
        {
            // create an in-memory cache by default.
            _dataCache = new OsmDataCacheMemory();
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
        /// <param name="source"></param>
        /// <param name="cache"></param>
        public OsmSimpleCompleteStreamSource(OsmStreamSource source, OsmDataCache cache)
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

        /// <summary>
        /// Flag indicating that the caching was done.
        /// </summary>
        private bool _cachingDone;

        /// <summary>
        /// Holds the current complete object.
        /// </summary>
        private ICompleteOsmGeo _current;

        /// <summary>
        /// Moves to the next object.
        /// </summary>
        /// <returns></returns>
        public override bool MoveNext()
        {
            if (!_cachingDone)
            { // first seek & cache.
                this.Seek();
                _cachingDone = true;
            }

            while (_simpleSource.MoveNext())
            { // there is data.
                OsmGeo currentSimple = _simpleSource.Current();

                switch (currentSimple.Type)
                {
                    case OsmGeoType.Node:
                        // create complete version.
                        _current = currentSimple as Node;

                        if(_current != null && _current.Tags == null)
                        { // make sure nodes have a default tag collection that is empty not null.
                            _current.Tags = new OsmSharp.Collections.Tags.TagsCollection();
                        }
                        if (_nodesToInclude.Contains(currentSimple.Id.Value))
                        { // node needs to be cached.
                            _dataCache.AddNode(currentSimple as Node);
                            _nodesToInclude.Remove(currentSimple.Id.Value);
                        }
                        break;
                    case OsmGeoType.Way:
                        // create complete way.
                        _current = CompleteWay.CreateFrom(currentSimple as Way, _dataCache);

                        if (_waysToInclude.Contains(currentSimple.Id.Value))
                        { // keep the way because it is needed later on.
                            _dataCache.AddWay(currentSimple as Way);
                            _waysToInclude.Remove(currentSimple.Id.Value);
                        }
                        else
                        { // only report that the nodes have been used when the way can be let-go.
                            Way way = currentSimple as Way;
                            if (way.Nodes != null)
                            { // report usage.
                                way.Nodes.ForEach(x => this.ReportNodeUsage(x));
                            }
                        }
                        break;
                    case OsmGeoType.Relation:
                        // create complate relation.
                        _current = CompleteRelation.CreateFrom(currentSimple as Relation, _dataCache);

                        if(!_relationsToInclude.Contains(currentSimple.Id.Value))
                        { // only report relation usage when the relation can be let go.
                            Relation relation = currentSimple as Relation;
                            if (relation.Members != null)
                            {
                                foreach (RelationMember member in relation.Members)
                                {
                                    switch (member.MemberType.Value)
                                    {
                                        case OsmGeoType.Node:
                                            this.ReportNodeUsage(member.MemberId.Value);
                                            break;
                                        case OsmGeoType.Way:
                                            this.ReportWayUsage(member.MemberId.Value);
                                            break;
                                        case OsmGeoType.Relation:
                                            this.ReportRelationUsage(member.MemberId.Value);
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

        /// <summary>
        /// An index of extra nodes to include.
        /// </summary>
        private readonly HashSet<long> _nodesToInclude;

        /// <summary>
        /// An index of nodes that are being used twice or more.
        /// </summary>
        private readonly Dictionary<long, int> _nodesUsedTwiceOrMore;

        /// <summary>
        /// An index of extra relations to include.
        /// </summary>
        private readonly HashSet<long> _waysToInclude;

        /// <summary>
        /// An index of ways that are being used twice or more.
        /// </summary>
        private readonly Dictionary<long, int> _waysUsedTwiceOrMore;

        /// <summary>
        /// An index of extra ways to include.
        /// </summary>
        private readonly HashSet<long> _relationsToInclude;

        /// <summary>
        /// An index of relations that are being used twice or more.
        /// </summary>
        private readonly Dictionary<long, int> _relationsUsedTwiceOrMore;

        /// <summary>
        /// Seeks for objects that are children of other objects.
        /// </summary>
        private void Seek()
        {
            var relations = new List<Relation>();
            _simpleSource.Initialize();
            while(_simpleSource.MoveNext(true, false, false))
            {
                var osmGeo = _simpleSource.Current();

                switch (osmGeo.Type)
                {
                    case OsmGeoType.Way:
                        var way = (osmGeo as Way);
                        if (way.Nodes != null)
                        {
                            foreach (long nodeId in way.Nodes)
                            {
                                this.MarkNodeAsChild(nodeId);
                            }
                        }
                        break;
                    case OsmGeoType.Relation:
                        var relation = (osmGeo as Relation);
                        if (relation.Members != null)
                        {
                            foreach (RelationMember member in relation.Members)
                            {
                                switch (member.MemberType.Value)
                                {
                                    case OsmGeoType.Node:
                                        this.MarkNodeAsChild(member.MemberId.Value);
                                        break;
                                    case OsmGeoType.Way:
                                        this.MarkWayAsChild(member.MemberId.Value);
                                        break;
                                    case OsmGeoType.Relation:
                                        this.MarkRelationAsChild(member.MemberId.Value);
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
                    _dataCache.AddRelation(relation);
                }
            }
            _simpleSource.Reset();
        }

        /// <summary>
        /// Reports that the node with the given id was used.
        /// </summary>
        /// <param name="nodeId"></param>
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
                _dataCache.RemoveNode(nodeId);
            }
        }

        /// <summary>
        /// Marks the given node as child.
        /// </summary>
        /// <param name="nodeId"></param>
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
        /// <param name="wayId"></param>
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
                Way way = _dataCache.GetWay(wayId); // get the way before it is removed.
                _dataCache.RemoveWay(wayId); // remove from cache.
                if (way != null && way.Nodes != null)
                { // report usage.
                    way.Nodes.ForEach(x => this.ReportNodeUsage(x));
                }
            }
        }

        /// <summary>
        /// Marks the given way as child.
        /// </summary>
        /// <param name="wayId"></param>
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
        /// <param name="relationId"></param>
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
                Relation relation = _dataCache.GetRelation(relationId); // get relation before it is removed.
                _dataCache.RemoveRelation(relationId); // remove relation.
                _relationsToInclude.Remove(relationId); // remove from the to-include list.

                //if (relation.Members != null)
                //{ // also report usage to the members.
                //    foreach (RelationMember member in relation.Members)
                //    {
                //        switch (member.MemberType.Value)
                //        {
                //            case OsmGeoType.Node:
                //                this.ReportNodeUsage(member.MemberId.Value);
                //                break;
                //            case OsmGeoType.Way:
                //                this.ReportWayUsage(member.MemberId.Value);
                //                break;
                //            case OsmGeoType.Relation:
                //                this.ReportRelationUsage(member.MemberId.Value);
                //                break;
                //        }
                //    }
                //}
            }
        }

        /// <summary>
        /// Marks the given relation as child.
        /// </summary>
        /// <param name="relationId"></param>
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
        /// <returns></returns>
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
