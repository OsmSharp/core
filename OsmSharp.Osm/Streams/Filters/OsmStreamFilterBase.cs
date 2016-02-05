// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2016 Abelshausen Ben
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
using OsmSharp.Osm.Data;

namespace OsmSharp.Osm.Streams.Filters
{
    /// <summary>
    /// Base class some filter that keeps used nodes, ways and relations.
    /// </summary>
    public abstract class OsmStreamFilterBase : OsmStreamFilter
    {
        private readonly ISnapshotDb _cacheDb; // Caches objects that are needed later to complete objects.

        /// <summary>
        /// Creates a new stream filter.
        /// </summary>
        public OsmStreamFilterBase(ISnapshotDb cache)
        {
            // create an in-memory cache by default.
            _cacheDb = cache;

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
        public OsmStreamFilterBase(OsmStreamSource source, ISnapshotDb cache)
        {
            _cacheDb = cache;

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

            if (!this.Source.CanReset)
            {
                throw new NotSupportedException("Creating a filter from a non-resettable simple stream is not supported. Wrap the source stream and create a resettable stream.");
            }
            else
            { // the simple source can be reset.

            }
        }

        /// <summary>
        /// Returns true if the given object has to be kept.
        /// </summary>
        public abstract bool Include(OsmGeo osmGeo);

        private bool _cachingDone; // Flag indicating that the caching was done.
        private OsmGeo _current; // Holds the current complete object.

        /// <summary>
        /// Move to the next item in the stream.
        /// </summary>
        public override bool MoveNext(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
        {
            while (this.DoMoveNext())
            {
                if (this.Current().Type == OsmGeoType.Node &&
                    !ignoreNodes)
                { // there is a node and it is not to be ignored.
                    return true;
                }
                else if (this.Current().Type == OsmGeoType.Way &&
                        !ignoreWays)
                { // there is a way and it is not to be ignored.
                    return true;
                }
                else if (this.Current().Type == OsmGeoType.Relation &&
                        !ignoreRelations)
                { // there is a relation and it is not to be ignored.
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Moves to the next object.
        /// </summary>
        /// <returns></returns>
        private bool DoMoveNext()
        {
            if (!_cachingDone)
            { // first seek & cache.
                this.Seek();
                this.CacheRelations();
                _cachingDone = true;
            }

            if (this.Source.MoveNext())
            { // there is data.
                var currentSimple = this.Source.Current();

                // make sure the object needs to be included.
                while (! this.IsChild(currentSimple) && 
                    !this.Include(currentSimple))
                { // don't include this object!
                    if (!this.Source.MoveNext())
                    { // oeps no more data!
                        return false;
                    }
                    currentSimple = this.Source.Current();
                }

                switch (currentSimple.Type)
                {
                    case OsmGeoType.Node:
                        // create complete version.
                        _current = currentSimple;
                        if (_nodesToInclude.Contains(currentSimple.Id.Value))
                        { // node needs to be cached.
                            _cacheDb.AddOrUpdate(currentSimple as Node);
                            _nodesToInclude.Remove(currentSimple.Id.Value);
                        }
                        break;
                    case OsmGeoType.Way:
                        // create complete way.
                        _current = currentSimple;

                        if (_waysToInclude.Contains(currentSimple.Id.Value))
                        { // keep the way because it is needed later on.
                            _cacheDb.AddOrUpdate(currentSimple as Way);
                            _waysToInclude.Remove(currentSimple.Id.Value);
                        }
                        else
                        { // only report that the nodes have been used when the way can be let-go.
                            var way = currentSimple as Way;
                            if (way.Nodes != null)
                            { // report usage.
                                way.Nodes.ForEach(x => this.ReportNodeUsage(x));
                            }
                        }
                        break;
                    case OsmGeoType.Relation:
                        // create complate relation.
                        _current = currentSimple;

                        if(!_relationsToInclude.Contains(currentSimple.Id.Value))
                        { // only report relation usage when the relation can be let go.
                            var relation = currentSimple as Relation;
                            if (relation.Members != null)
                            {
                                foreach (var member in relation.Members)
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
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if the given object is an object that needs to be included.
        /// </summary>
        private bool IsChild(OsmGeo current)
        {
            switch (current.Type)
            {
                case OsmGeoType.Node:
                    return _nodesToInclude.Contains(current.Id.Value) ||
                        _cacheDb.ExistsNode(current.Id.Value);
                case OsmGeoType.Way:
                    return _waysToInclude.Contains(current.Id.Value) ||
                        _cacheDb.ExistsWay(current.Id.Value);
                case OsmGeoType.Relation:
                    return _relationsToInclude.Contains(current.Id.Value) ||
                        _cacheDb.ExistsRelation(current.Id.Value);
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
        /// Seeks for objects that are children of other objects and children of children.
        /// </summary>
        private void Seek()
        {
            var possibleWayChildren = new HashSet<long>();
            var possibleRelationChildren = new HashSet<long>();
            foreach (var osmGeo in this.Source)
            {
                if (this.Include(osmGeo))
                {
                    switch (osmGeo.Type)
                    {
                        case OsmGeoType.Way:
                            foreach (long nodeId in (osmGeo as Way).Nodes)
                            {
                                this.MarkNodeAsChild(nodeId);
                            }
                            break;
                        case OsmGeoType.Relation:
                            foreach (RelationMember member in (osmGeo as Relation).Members)
                            {
                                switch (member.MemberType.Value)
                                {
                                    case OsmGeoType.Node:
                                        this.MarkNodeAsChild(member.MemberId.Value);
                                        break;
                                    case OsmGeoType.Way:
                                        this.MarkWayAsChild(member.MemberId.Value);

                                        possibleWayChildren.Add(member.MemberId.Value);
                                        break;
                                    case OsmGeoType.Relation:
                                        this.MarkRelationAsChild(member.MemberId.Value);

                                        possibleRelationChildren.Add(member.MemberId.Value);
                                        break;
                                }
                            }
                            break;
                    }
                }
            }
            while (possibleRelationChildren.Count > 0 ||
                possibleWayChildren.Count > 0)
            { // keep looping until all children are accounted for.
                this.Source.Reset();
                var newPossibleWayChildren = new HashSet<long>();
                var newPossibleRelationChildren = new HashSet<long>();
                foreach (OsmGeo osmGeo in this.Source)
                {
                    switch (osmGeo.Type)
                    {
                        case OsmGeoType.Way:
                            if (possibleWayChildren.Contains(osmGeo.Id.Value))
                            {
                                foreach (long nodeId in (osmGeo as Way).Nodes)
                                {
                                    this.MarkNodeAsChild(nodeId);
                                }
                            }
                            break;
                        case OsmGeoType.Relation:
                            if (possibleRelationChildren.Contains(osmGeo.Id.Value))
                            {
                                foreach (RelationMember member in (osmGeo as Relation).Members)
                                {
                                    switch (member.MemberType.Value)
                                    {
                                        case OsmGeoType.Node:
                                            this.MarkNodeAsChild(member.MemberId.Value);
                                            break;
                                        case OsmGeoType.Way:
                                            this.MarkWayAsChild(member.MemberId.Value);

                                            newPossibleWayChildren.Add(member.MemberId.Value);
                                            break;
                                        case OsmGeoType.Relation:
                                            this.MarkRelationAsChild(member.MemberId.Value);

                                            newPossibleRelationChildren.Add(member.MemberId.Value);
                                            break;
                                    }
                                }
                            }
                            break;
                    }
                }
                possibleWayChildren = newPossibleWayChildren;
                possibleRelationChildren = newPossibleRelationChildren;
            }
            this.Source.Reset();
        }

        /// <summary>
        /// Cache all needed relations.
        /// </summary>
        private void CacheRelations()
        {
            foreach (var osmGeo in this.Source)
            {
                switch (osmGeo.Type)
                {
                    case OsmGeoType.Relation:
                        if (this.Include(osmGeo))
                        {
                            if (_relationsToInclude.Contains(osmGeo.Id.Value))
                            { // yep, cache relation!
                                _cacheDb.AddOrUpdate(osmGeo as Relation);
                            }
                        }
                        break;
                }
            }
            this.Source.Reset();
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
                _cacheDb.DeleteNode(nodeId);
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
                var way = _cacheDb.GetWay(wayId); // get the way before it is removed.
                _cacheDb.DeleteWay(wayId); // remove from cache.
                if (way.Nodes != null)
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
                var relation = _cacheDb.GetRelation(relationId); // get relation before it is removed.
                _cacheDb.DeleteRelation(relationId); // remove relation.
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
        public override OsmGeo Current()
        {
            return _current;
        }
        
        /// <summary>
        /// Resets this source.
        /// </summary>
        public override void Reset()
        {
            _cachingDone = false;

            _cacheDb.Clear();
            this.Source.Reset();
        }

        /// <summary>
        /// Returns true if this source can be reset.
        /// </summary>
        public override bool CanReset
        {
            get { return this.Source.CanReset; }
        }
    }
}