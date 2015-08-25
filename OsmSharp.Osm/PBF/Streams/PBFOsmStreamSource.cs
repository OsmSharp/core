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
using System.IO;
using System.Text;
using OsmSharp.Collections.Tags;
using OsmSharp.Osm.Streams;

namespace OsmSharp.Osm.PBF.Streams
{
    /// <summary>
    /// A source of PBF formatted OSM data.
    /// </summary>
    public class PBFOsmStreamSource : OsmStreamSource, IPBFOsmPrimitiveConsumer
    {
        /// <summary>
        /// Holds the source of the data.
        /// </summary>
        private readonly Stream _stream;

        /// <summary>
        /// Creates a new source of PBF formated OSM data.
        /// </summary>
        /// <param name="stream"></param>
        public PBFOsmStreamSource(Stream stream)
        {
            _stream = stream;
        }

        /// <summary>
        /// Initializes the current source.
        /// </summary>
        public override void Initialize()
        {
            _stream.Seek(0, SeekOrigin.Begin);

            this.InitializePBFReader();
        }

        /// <summary>
        /// Move to the next item in the stream.
        /// </summary>
        /// <param name="ignoreNodes">Makes this source skip all nodes.</param>
        /// <param name="ignoreWays">Makes this source skip all ways.</param>
        /// <param name="ignoreRelations">Makes this source skip all relations.</param>
        /// <returns></returns>
        public override bool MoveNext(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
        {
            var nextPBFPrimitive = this.MoveToNextPrimitive(ignoreNodes, ignoreWays, ignoreRelations);
            while(nextPBFPrimitive.Value != null)
            {
                OsmSharp.Osm.PBF.Node node = (nextPBFPrimitive.Value as OsmSharp.Osm.PBF.Node);
                if(node != null && !ignoreNodes)
                { // next primitve is a node.
                    _current = this.ConvertNode(nextPBFPrimitive.Key, node);
                    return true;
                }
                OsmSharp.Osm.PBF.Way way = (nextPBFPrimitive.Value as OsmSharp.Osm.PBF.Way);
                if(way != null && !ignoreWays)
                { // next primitive is a way.
                    _current = this.ConvertWay(nextPBFPrimitive.Key, way);
                    return true;
                }
                OsmSharp.Osm.PBF.Relation relation = (nextPBFPrimitive.Value as OsmSharp.Osm.PBF.Relation);
                if (relation != null && !ignoreRelations)
                { // next primitive is a relation.
                    _current = this.ConvertRelation(nextPBFPrimitive.Key, relation);
                    return true;
                }
                nextPBFPrimitive = this.MoveToNextPrimitive(ignoreNodes, ignoreWays, ignoreRelations);
            }
            return false;
        }

        /// <summary>
        /// Holds the current object.
        /// </summary>
        private OsmSharp.Osm.OsmGeo _current;

        /// <summary>
        /// Returns the current geometry.
        /// </summary>
        /// <returns></returns>
        public override OsmSharp.Osm.OsmGeo Current()
        {
            return _current;
        }

        /// <summary>
        /// Resetting this data source 
        /// </summary>
        public override void Reset()
        {
            _current = null;
            if (_cachedPrimitives != null) { _cachedPrimitives.Clear(); }
            _stream.Seek(0, SeekOrigin.Begin);
        }

        /// <summary>
        /// Returns true if this source can be reset.
        /// </summary>
        public override bool CanReset
        {
            get
            {
                return _stream.CanSeek;
            }
        }

        #region Primitive Conversion

        /// <summary>
        /// Converts the PBF node into an OsmSharp-node.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        internal OsmSharp.Osm.Node ConvertNode(PrimitiveBlock block, OsmSharp.Osm.PBF.Node node)
        {
            var simpleNode = new OsmSharp.Osm.Node();
            simpleNode.ChangeSetId = node.info.changeset;
            simpleNode.Id = node.id;
            simpleNode.Latitude = .000000001 * ((double)block.lat_offset
                + ((double)block.granularity * (double)node.lat));
            simpleNode.Longitude = .000000001 * ((double)block.lon_offset
                + ((double)block.granularity * (double)node.lon));
            simpleNode.Tags = new TagsCollection(node.keys.Count);
            if (node.keys.Count > 0)
            {
                for (int tag_idx = 0; tag_idx < node.keys.Count; tag_idx++)
                {
                    string key = Encoding.UTF8.GetString(block.stringtable.s[(int)node.keys[tag_idx]]);
                    string value = Encoding.UTF8.GetString(block.stringtable.s[(int)node.vals[tag_idx]]);

                    simpleNode.Tags.Add(new Tag() { Key = key, Value = value });
                }
            }
            simpleNode.TimeStamp = Utilities.FromUnixTime((long)node.info.timestamp *
                (long)block.date_granularity);
            simpleNode.Visible = true;
            simpleNode.Version = (uint)node.info.version;
            simpleNode.UserId = node.info.uid;
            simpleNode.UserName = Encoding.UTF8.GetString(block.stringtable.s[node.info.user_sid]);
            simpleNode.Version = (ulong)node.info.version;
            simpleNode.Visible = true;

            return simpleNode;
        }

        /// <summary>
        /// Converts a PBF way into an OsmSharp-way.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="way"></param>
        /// <returns></returns>
        internal OsmSharp.Osm.Way ConvertWay(PrimitiveBlock block, OsmSharp.Osm.PBF.Way way)
        {
            var simpleWay = new OsmSharp.Osm.Way();
            simpleWay.Id = way.id;
            simpleWay.Nodes = new List<long>(way.refs.Count);
            long node_id = 0;
            for (int node_idx = 0; node_idx < way.refs.Count; node_idx++)
            {
                node_id = node_id + way.refs[node_idx];
                simpleWay.Nodes.Add(node_id);
            }
            simpleWay.Tags = new TagsCollection(way.keys.Count);
            if (way.keys.Count > 0)
            {
                for (int tag_idx = 0; tag_idx < way.keys.Count; tag_idx++)
                {
                    string key = Encoding.UTF8.GetString(block.stringtable.s[(int)way.keys[tag_idx]]);
                    string value = Encoding.UTF8.GetString(block.stringtable.s[(int)way.vals[tag_idx]]);

                    simpleWay.Tags.Add(new Tag(key, value));
                }
            }
            if (way.info != null)
            { // add the metadata if any.
                simpleWay.ChangeSetId = way.info.changeset;
                simpleWay.TimeStamp = Utilities.FromUnixTime((long)way.info.timestamp *
                    (long)block.date_granularity);
                simpleWay.UserId = way.info.uid;
                simpleWay.UserName = Encoding.UTF8.GetString(block.stringtable.s[way.info.user_sid]);
                simpleWay.Version = (ulong)way.info.version;
            }
            simpleWay.Visible = true;

            return simpleWay;
        }
        
        /// <summary>
        /// Converts a PBF way into an OsmSharp-relation.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="relation"></param>
        /// <returns></returns>
        internal OsmSharp.Osm.Relation ConvertRelation(PrimitiveBlock block, OsmSharp.Osm.PBF.Relation relation)
        {
            var simpleRelation = new OsmSharp.Osm.Relation();
            simpleRelation.Id = relation.id;
            if (relation.types.Count > 0)
            {
                simpleRelation.Members = new List<OsmSharp.Osm.RelationMember>();
                long member_id = 0;
                for (int member_idx = 0; member_idx < relation.types.Count; member_idx++)
                {
                    member_id = member_id + relation.memids[member_idx];
                    string role = Encoding.UTF8.GetString(
                        block.stringtable.s[relation.roles_sid[member_idx]]);
                    var member = new OsmSharp.Osm.RelationMember();
                    member.MemberId = member_id;
                    member.MemberRole = role;
                    switch (relation.types[member_idx])
                    {
                        case Relation.MemberType.NODE:
                            member.MemberType = OsmSharp.Osm.OsmGeoType.Node;
                            break;
                        case Relation.MemberType.WAY:
                            member.MemberType = OsmSharp.Osm.OsmGeoType.Way;
                            break;
                        case Relation.MemberType.RELATION:
                            member.MemberType = OsmSharp.Osm.OsmGeoType.Relation;
                            break;
                    }

                    simpleRelation.Members.Add(member);
                }
            }
            simpleRelation.Tags = new TagsCollection(relation.keys.Count);
            if (relation.keys.Count > 0)
            {
                for (int tag_idx = 0; tag_idx < relation.keys.Count; tag_idx++)
                {
                    string key = Encoding.UTF8.GetString(block.stringtable.s[(int)relation.keys[tag_idx]]);
                    string value = Encoding.UTF8.GetString(block.stringtable.s[(int)relation.vals[tag_idx]]);

                    simpleRelation.Tags.Add(new Tag(key, value));
                }
            }
            if (relation.info != null)
            { // read metadata if any.
                simpleRelation.ChangeSetId = relation.info.changeset;
                simpleRelation.TimeStamp = Utilities.FromUnixTime((long)relation.info.timestamp *
                    (long)block.date_granularity);
                simpleRelation.UserId = relation.info.uid;
                simpleRelation.UserName = Encoding.UTF8.GetString(block.stringtable.s[relation.info.user_sid]);
                simpleRelation.Version = (ulong)relation.info.version;
            }
            simpleRelation.Visible = true;

            return simpleRelation;
        }

        #endregion

        #region PBF Blocks Reader

        /// <summary>
        /// Holds the PBF reader.
        /// </summary>
        private PBFReader _reader;

        /// <summary>
        /// Holds the primitives decompressor.
        /// </summary>
        private OsmSharp.Osm.PBF.Dense.Decompressor _decompressor;

        /// <summary>
        /// Initializes the PBF reader.
        /// </summary>
        private void InitializePBFReader()
        {
            _reader = new PBFReader(_stream);
            _decompressor = new OsmSharp.Osm.PBF.Dense.Decompressor(this);

            this.InitializeBlockCache();
        }

        /// <summary>
        /// Moves the PBF reader to the next primitive or returns one of the cached ones.
        /// </summary>
        /// <returns></returns>
        private KeyValuePair<PrimitiveBlock, object> MoveToNextPrimitive(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
        {
            var next = this.DeQueuePrimitive();
            if (next.Value == null)
            {
                var block = _reader.MoveNext();
                while (block != null && !_decompressor.ProcessPrimitiveBlock(block, ignoreNodes, ignoreWays, ignoreRelations))
                {
                    block = _reader.MoveNext();
                }
                next = this.DeQueuePrimitive();
            }
            return next;
        }

        #region Block Cache

        /// <summary>
        /// Holds the cached primitives.
        /// </summary>
        private Queue<KeyValuePair<PrimitiveBlock, object>> _cachedPrimitives;

        /// <summary>
        /// Initializes the block cache.
        /// </summary>
        private void InitializeBlockCache()
        {
            _cachedPrimitives = new Queue<KeyValuePair<PrimitiveBlock, object>>();
        }

        /// <summary>
        /// Queues the primitives.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="primitive"></param>
        private void QueuePrimitive(PrimitiveBlock block, object primitive)
        {
            _cachedPrimitives.Enqueue(new KeyValuePair<PrimitiveBlock, object>(block, primitive));
        }

        /// <summary>
        /// DeQueues a primitive.
        /// </summary>
        /// <returns></returns>
        private KeyValuePair<PrimitiveBlock, object> DeQueuePrimitive()
        {
            if (_cachedPrimitives.Count > 0)
            {
                return _cachedPrimitives.Dequeue();
            }
            return new KeyValuePair<PrimitiveBlock, object>();
        }

        #endregion

        #endregion

        /// <summary>
        /// Processes a node.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="node"></param>
        void IPBFOsmPrimitiveConsumer.ProcessNode(PrimitiveBlock block, Node node)
        {
            this.QueuePrimitive(block, node);
        }

        /// <summary>
        /// Processes a way.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="way"></param>
        void IPBFOsmPrimitiveConsumer.ProcessWay(PrimitiveBlock block, Way way)
        {
            this.QueuePrimitive(block, way);
        }

        /// <summary>
        /// Processes a relation.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="relation"></param>
        void IPBFOsmPrimitiveConsumer.ProcessRelation(PrimitiveBlock block, Relation relation)
        {
            this.QueuePrimitive(block, relation);
        }
    }
}