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

using OsmSharp.Tags;
using System;
using System.Collections.Generic;

namespace OsmSharp.IO.PBF
{
    /// <summary>
    /// Encoder/decoder for OSM-PBF format.
    /// </summary>
    public static class Encoder
    {
        /// <summary>
        /// The string representing an OSMHeader type.
        /// </summary>
        public static string OSMHeader = "OSMHeader";

        /// <summary>
        /// The string representing and OSMData type.
        /// </summary>
        public static string OSMData = "OSMData";

        /// <summary>
        /// Decodes the block.
        /// </summary>
        public static bool Decode(this PrimitiveBlock block, IPBFOsmPrimitiveConsumer primitivesConsumer, 
            bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
        {
            var success = false;

            if (block.primitivegroup != null)
            {
                foreach (var primitivegroup in block.primitivegroup)
                {
                    if (!ignoreNodes && primitivegroup.dense != null)
                    {
                        int keyValsIdx = 0;
                        long currentId = 0;
                        long currentLat = 0;
                        long currentLon = 0;
                        long currentChangeset = 0;
                        long currentTimestamp = 0;
                        int currentUid = 0;
                        int currentUserSid = 0;
                        int currentVersion = 0;

                        for (int idx = 0; idx < primitivegroup.dense.id.Count; idx++)
                        {
                            // do the delta decoding stuff.
                            currentId = currentId +
                                primitivegroup.dense.id[idx];
                            currentLat = currentLat +
                                primitivegroup.dense.lat[idx];
                            currentLon = currentLon +
                                primitivegroup.dense.lon[idx];
                            if (primitivegroup.dense.denseinfo != null)
                            { // add all the metadata.
                                currentChangeset = currentChangeset +
                                    primitivegroup.dense.denseinfo.changeset[idx];
                                currentTimestamp = currentTimestamp +
                                    primitivegroup.dense.denseinfo.timestamp[idx];
                                currentUid = currentUid +
                                    primitivegroup.dense.denseinfo.uid[idx];
                                currentUserSid = currentUserSid +
                                    primitivegroup.dense.denseinfo.user_sid[idx];
                                currentVersion = currentVersion +
                                    primitivegroup.dense.denseinfo.version[idx];
                            }

                            var node = new Node();
                            node.id = currentId;
                            node.info = new Info();
                            node.info.changeset = currentChangeset;
                            node.info.timestamp = (int)currentTimestamp;
                            node.info.uid = currentUid;
                            node.info.user_sid = currentUserSid;
                            node.info.version = currentVersion;
                            node.lat = currentLat;
                            node.lon = currentLon;

                            // get the keys/vals.
                            var keyVals = primitivegroup.dense.keys_vals;
                            while (keyVals.Count > keyValsIdx &&
                                keyVals[keyValsIdx] != 0)
                            {
                                node.keys.Add((uint)keyVals[keyValsIdx]);
                                keyValsIdx++;
                                node.vals.Add((uint)keyVals[keyValsIdx]);
                                keyValsIdx++;
                            }
                            keyValsIdx++;

                            success = true;
                            primitivesConsumer.ProcessNode(block, node);
                        }
                    }
                    else
                    {
                        if (!ignoreNodes && primitivegroup.nodes != null)
                        {
                            foreach (var node in primitivegroup.nodes)
                            {
                                success = true;
                                primitivesConsumer.ProcessNode(block, node);
                            }
                        }
                        if (!ignoreWays && primitivegroup.ways != null)
                        {
                            foreach (var way in primitivegroup.ways)
                            {
                                success = true;
                                primitivesConsumer.ProcessWay(block, way);
                            }
                        }
                        if (!ignoreRelations && primitivegroup.relations != null)
                        {
                            foreach (var relation in primitivegroup.relations)
                            {
                                success = true;
                                primitivesConsumer.ProcessRelation(block, relation);
                            }
                        }
                    }
                }
            }
            return success;
        }

        /// <summary>
        /// Encodes the nodes into the given block.
        /// </summary>
        public static void Encode(this PrimitiveBlock block, Dictionary<string, int> reverseStringTable, List<OsmGeo> osmGeos)
        {
            var groupIdx = 0;
            var i = 0;
            var nodeCount = 0;

            if(block.stringtable != null &&
               block.stringtable.s != null)
            {
                block.stringtable.s.Clear();
            }
            while(i < osmGeos.Count)
            {
                PrimitiveGroup group = null;
                if (groupIdx < block.primitivegroup.Count)
                { // get the existing group and clear it's data.
                    group = block.primitivegroup[groupIdx];
                    if(group == null)
                    {
                        group = new PrimitiveGroup();
                    }
                    if (group.dense != null)
                    {
                        if (group.dense.denseinfo != null)
                        {
                            if (group.dense.denseinfo.changeset != null) { group.dense.denseinfo.changeset.Clear(); }
                            if (group.dense.denseinfo.timestamp != null) { group.dense.denseinfo.timestamp.Clear(); }
                            if (group.dense.denseinfo.uid != null) { group.dense.denseinfo.uid.Clear(); }
                            if (group.dense.denseinfo.user_sid != null) { group.dense.denseinfo.user_sid.Clear(); }
                            if (group.dense.denseinfo.version != null) { group.dense.denseinfo.version.Clear(); }
                        }
                        if (group.dense.id != null) { group.dense.id.Clear(); }
                        if (group.dense.keys_vals != null) { group.dense.keys_vals.Clear(); }
                        if (group.dense.lat != null) { group.dense.lat.Clear(); }
                        if (group.dense.lon != null) { group.dense.lon.Clear(); }
                    }
                    if (group.changesets != null) { group.changesets.Clear(); }
                    //if (group.nodes != null) { group.nodes.Clear(); }
                    if (group.ways != null) { group.ways.Clear(); }
                    if (group.relations != null) { group.relations.Clear(); }
                }
                else
                { // add a new group.
                    group = new PrimitiveGroup();
                    block.primitivegroup.Add(group);
                }

                // build group.
                var groupType = osmGeos[i].Type;
                var current = osmGeos[i];
                while (i < osmGeos.Count &&
                    osmGeos[i].Type == groupType)
                {
                    switch(groupType)
                    {
                        case OsmGeoType.Node:
                            if(group.nodes.Count > nodeCount)
                            { // overwrite existing.
                                Encoder.EncodeNode(block, reverseStringTable, group.nodes[nodeCount], osmGeos[i] as OsmSharp.Node);
                            }
                            else
                            { // write new.
                                group.nodes.Add(Encoder.EncodeNode(block, reverseStringTable, osmGeos[i] as OsmSharp.Node));
                            }
                            nodeCount++;
                            break;
                        case OsmGeoType.Way:
                            group.ways.Add(Encoder.EncodeWay(block, reverseStringTable, osmGeos[i] as OsmSharp.Way));
                            break;
                        case OsmGeoType.Relation:
                            group.relations.Add(Encoder.EncodeRelation(block, reverseStringTable, osmGeos[i] as OsmSharp.Relation));
                            break;
                    }
                    i++;
                }


                // remove obsolete nodes.
                if(group.nodes != null)
                {
                    while (nodeCount < group.nodes.Count)
                    {
                        group.nodes.RemoveAt(nodeCount);
                    }
                }

                // move to the next group.
                groupIdx++;
            }

            while (groupIdx < block.primitivegroup.Count)
            {
                block.primitivegroup.RemoveAt(groupIdx);
            }
        }

        /// <summary>
        /// Converts the PBF node into an OsmSharp-node.
        /// </summary>
        /// <returns></returns>
        public static OsmSharp.Node DecodeNode(PrimitiveBlock block, OsmSharp.IO.PBF.Node pbfNode, OsmSharp.Node node)
        {
            // clear old data.
            if (node.Tags != null)
            { // clear tags.
                node.Tags.Clear();
            }
            if (node.Tags == null)
            { // create tags collection.
                node.Tags = new TagsCollection();
            }

            // set new stuff.
            node.ChangeSetId = pbfNode.info.changeset;
            node.Id = pbfNode.id;
            node.Latitude = Encoder.DecodeLatLon(pbfNode.lat, block.lat_offset, block.granularity);
            node.Longitude = Encoder.DecodeLatLon(pbfNode.lon, block.lon_offset, block.granularity);
            for (var i = 0; i < pbfNode.keys.Count; i++)
            {
                node.Tags.Add(new Tag()
                {
                    Key = System.Text.Encoding.UTF8.GetString(block.stringtable.s[(int)pbfNode.keys[i]]),
                    Value = System.Text.Encoding.UTF8.GetString(block.stringtable.s[(int)pbfNode.vals[i]])
                });
            }
            if (pbfNode.info != null)
            { // add the metadata if any.
                node.TimeStamp = Encoder.DecodeTimestamp(pbfNode.info.timestamp, block.date_granularity);
                node.Visible = true;
                node.Version = pbfNode.info.version;
                node.UserId = pbfNode.info.uid;
                node.UserName = null;
                if(block.stringtable != null)
                {
                    node.UserName = System.Text.Encoding.UTF8.GetString(block.stringtable.s[pbfNode.info.user_sid]);
                }
            }
            node.Visible = true;

            return node;
        }

        /// <summary>
        /// Converts the PBF node into an OsmSharp-node.
        /// </summary>
        public static OsmSharp.Node DecodeNode(PrimitiveBlock block, OsmSharp.IO.PBF.Node pbfNode)
        {
            var node = new OsmSharp.Node();
            Encoder.DecodeNode(block, pbfNode, node);
            return node;
        }

        /// <summary>
        /// Encodes a string.
        /// </summary>
        /// <returns></returns>
        public static int EncodeString(PrimitiveBlock block, Dictionary<string, int> reverseStringTable, string value)
        {
            if (value == null) { return 0; }

            if (block.stringtable == null)
            {
                block.stringtable = new StringTable();
                block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes(string.Empty));
                reverseStringTable.Add(string.Empty, 0);
            }

            int id;
            if (reverseStringTable.TryGetValue(value, out id))
            {
                return id;
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(value);
            block.stringtable.s.Add(System.Text.Encoding.UTF8.GetBytes(value));
            reverseStringTable.Add(value, block.stringtable.s.Count - 1);
            return block.stringtable.s.Count - 1;
        }

        /// <summary>
        /// Encodes an OsmSharp-node into a PBF-node.
        /// </summary>
        public static OsmSharp.IO.PBF.Node EncodeNode(PrimitiveBlock block, Dictionary<string, int> reverseStringTable, OsmSharp.Node node)
        {
            var pbfNode = new OsmSharp.IO.PBF.Node();
            Encoder.EncodeNode(block, reverseStringTable, pbfNode, node);
            return pbfNode;
        }

        /// <summary>
        /// Encodes an OsmSharp-node into a PBF-node.
        /// </summary>
        public static OsmSharp.IO.PBF.Node EncodeNode(PrimitiveBlock block, Dictionary<string, int> reverseStringTable,
            OsmSharp.IO.PBF.Node pbfNode, OsmSharp.Node node)
        {
            pbfNode.id = node.Id.Value;
            pbfNode.info = new Info();
            pbfNode.info.version = 0;
            if (node.ChangeSetId.HasValue)
            {
                pbfNode.info.changeset = node.ChangeSetId.Value;
            }
            else
            {
                pbfNode.info.changeset = 0;
            }
            if (node.TimeStamp.HasValue)
            {
                pbfNode.info.timestamp = Encoder.EncodeTimestamp(node.TimeStamp.Value, block.date_granularity);
            }
            else
            {
                pbfNode.info.timestamp = 0;
            }
            if (node.UserId.HasValue)
            {
                pbfNode.info.uid = (int)node.UserId.Value;
            }
            else
            {
                pbfNode.info.uid = 0;
            }
            pbfNode.info.user_sid = Encoder.EncodeString(block, reverseStringTable, node.UserName);
            if (node.Version.HasValue)
            {
                pbfNode.info.version = (int)node.Version.Value;
            }
            else
            {
                pbfNode.info.version = 0;
            }
            pbfNode.lat = Encoder.EncodeLatLon(node.Latitude.Value, block.lat_offset, block.granularity);
            pbfNode.lon = Encoder.EncodeLatLon(node.Longitude.Value, block.lon_offset, block.granularity);

            if(node.Tags != null)
            {
                foreach(var tag in node.Tags)
                {
                    pbfNode.keys.Add((uint)Encoder.EncodeString(block, reverseStringTable, tag.Key));
                    pbfNode.vals.Add((uint)Encoder.EncodeString(block, reverseStringTable, tag.Value));
                }
            }
            else
            {
                pbfNode.keys.Clear();
                pbfNode.vals.Clear();
            }
            return pbfNode;
        }

        /// <summary>
        /// Converts a PBF-way into an OsmSharp-way.
        /// </summary>
        public static void DecodeWay(PrimitiveBlock block, OsmSharp.IO.PBF.Way pbfWay, OsmSharp.Way way)
        {
            // make sure old data is gone.
            if (way.Nodes != null &&
                way.Nodes.Length > 0)
            { // clear nodes list.
                way.Nodes = new long[pbfWay.refs.Count];
            }
            if(way.Tags != null)
            { // clear the tags collection.
                way.Tags.Clear();
            }
            if (way.Nodes == null)
            { // create nodes list.
                way.Nodes = new long[pbfWay.refs.Count];
            }
            if (way.Tags == null)
            { // create tags collection.
                way.Tags = new TagsCollection(pbfWay.keys.Count);
            }

            // set new stuff.
            way.Id = pbfWay.id;
            if(pbfWay.refs.Count > 0)
            { // fill nodes-list.
                long nodeId = 0;
                for (int i = 0; i < pbfWay.refs.Count; i++)
                {
                    nodeId = nodeId + pbfWay.refs[i];
                    way.Nodes[i] = nodeId;
                }
            }
            if (pbfWay.keys.Count > 0)
            {
                for (var i = 0; i < pbfWay.keys.Count; i++)
                {
                    var key = System.Text.Encoding.UTF8.GetString(block.stringtable.s[(int)pbfWay.keys[i]]);
                    var value = System.Text.Encoding.UTF8.GetString(block.stringtable.s[(int)pbfWay.vals[i]]);

                    way.Tags.Add(new Tag(key, value));
                }
            }
            if (pbfWay.info != null)
            { // add the metadata if any.
                way.ChangeSetId = pbfWay.info.changeset;
                way.TimeStamp = Encoder.DecodeTimestamp(pbfWay.info.timestamp, block.date_granularity);
                way.UserId = pbfWay.info.uid;
                way.UserName = null;
                if(block.stringtable != null)
                {
                    way.UserName = System.Text.Encoding.UTF8.GetString(block.stringtable.s[pbfWay.info.user_sid]);
                }
                way.Version = pbfWay.info.version;
            }
            way.Visible = true;
        }

        /// <summary>
        /// Converts a PBF way into an OsmSharp-way.
        /// </summary>
        public static OsmSharp.Way DecodeWay(PrimitiveBlock block, OsmSharp.IO.PBF.Way pbfWay)
        {
            var way = new OsmSharp.Way();
            Encoder.DecodeWay(block, pbfWay, way);
            return way;
        }

        /// <summary>
        /// Encodes an OsmSharp-way into a PBF-way.
        /// </summary>
        public static OsmSharp.IO.PBF.Way EncodeWay(PrimitiveBlock block, Dictionary<string, int> reverseStringTable, OsmSharp.Way way)
        {
            var pbfWay = new OsmSharp.IO.PBF.Way();
            pbfWay.id = way.Id.Value;
            pbfWay.info = new Info();
            if (way.ChangeSetId.HasValue) { pbfWay.info.changeset = way.ChangeSetId.Value; }
            if (way.TimeStamp.HasValue)
            {
                pbfWay.info.timestamp = Encoder.EncodeTimestamp(way.TimeStamp.Value, block.date_granularity);
            }
            if (way.UserId.HasValue) { pbfWay.info.uid = (int)way.UserId.Value; }
            pbfWay.info.user_sid = Encoder.EncodeString(block, reverseStringTable, way.UserName);
            pbfWay.info.version = 0;
            if (way.Version.HasValue) { pbfWay.info.version = (int)way.Version.Value; }

            if (way.Tags != null)
            {
                foreach (var tag in way.Tags)
                {
                    pbfWay.keys.Add((uint)Encoder.EncodeString(block, reverseStringTable, tag.Key));
                    pbfWay.vals.Add((uint)Encoder.EncodeString(block, reverseStringTable, tag.Value));
                }
            }

            if (way.Nodes != null && 
                way.Nodes.Length > 0)
            {
                pbfWay.refs.Add(way.Nodes[0]);
                for (var i = 1; i < way.Nodes.Length; i++)
                {
                    pbfWay.refs.Add(way.Nodes[i] - way.Nodes[i - 1]);
                }
            }
            return pbfWay;
        }

        /// <summary>
        /// Converts a PBF way into an OsmSharp-relation.
        /// </summary>
        public static OsmSharp.Relation DecodeRelation(PrimitiveBlock block, OsmSharp.IO.PBF.Relation pbfRelation,
            OsmSharp.Relation relation)
        {
            // make sure old data is gone.
            if (relation.Members != null &&
                relation.Members.Length > 0)
            { // clear members.
                relation.Members = new RelationMember[pbfRelation.memids.Count];
            }
            if (relation.Tags != null)
            { // clear the tags collection.
                relation.Tags.Clear();
            }
            if (relation.Members == null)
            { // clear members.
                relation.Members = new RelationMember[pbfRelation.memids.Count];
            }
            if (relation.Tags == null)
            { // create tags collection.
                relation.Tags = new TagsCollection(pbfRelation.keys.Count);
            }

            // add nex stuff.
            relation.Id = pbfRelation.id;
            long memberId = 0;
            for (var i = 0; i < pbfRelation.types.Count; i++)
            {
                memberId = memberId + pbfRelation.memids[i];
                var role = System.Text.Encoding.UTF8.GetString(
                    block.stringtable.s[pbfRelation.roles_sid[i]]);

                var member = new OsmSharp.RelationMember();
                member.Id = memberId;
                member.Role = role;
                switch (pbfRelation.types[i])
                {
                    case Relation.MemberType.NODE:
                        member.Type = OsmSharp.OsmGeoType.Node;
                        break;
                    case Relation.MemberType.WAY:
                        member.Type = OsmSharp.OsmGeoType.Way;
                        break;
                    case Relation.MemberType.RELATION:
                        member.Type = OsmSharp.OsmGeoType.Relation;
                        break;
                }

                relation.Members[i] = member;
            }
            for (int i = 0; i < pbfRelation.keys.Count; i++)
            {
                string key = System.Text.Encoding.UTF8.GetString(block.stringtable.s[(int)pbfRelation.keys[i]]);
                string value = System.Text.Encoding.UTF8.GetString(block.stringtable.s[(int)pbfRelation.vals[i]]);

                relation.Tags.Add(new Tag(key, value));
            }
            if (pbfRelation.info != null)
            { // read metadata if any.
                relation.ChangeSetId = pbfRelation.info.changeset;
                relation.TimeStamp = Encoder.DecodeTimestamp(pbfRelation.info.timestamp, block.date_granularity);
                relation.UserId = pbfRelation.info.uid;
                relation.UserName = null;
                if(block.stringtable != null)
                {
                    relation.UserName = System.Text.Encoding.UTF8.GetString(block.stringtable.s[pbfRelation.info.user_sid]);
                }
                relation.Version = pbfRelation.info.version;
            }
            relation.Visible = true;

            return relation;
        }

        /// <summary>
        /// Converts a PBF way into an OsmSharp-relation.
        /// </summary>
        public static OsmSharp.Relation DecodeRelation(PrimitiveBlock block, OsmSharp.IO.PBF.Relation pbfRelation)
        {
            var relation = new OsmSharp.Relation();
            Encoder.DecodeRelation(block, pbfRelation, relation);
            return relation;
        }

        /// <summary>
        /// Encodes an OsmSharp-relation into a PBF-relation.
        /// </summary>
        /// <returns></returns>
        public static OsmSharp.IO.PBF.Relation EncodeRelation(PrimitiveBlock block, Dictionary<string, int> reverseStringTable, OsmSharp.Relation relation)
        {
            var pbfRelation = new OsmSharp.IO.PBF.Relation();
            pbfRelation.id = relation.Id.Value;
            pbfRelation.info = new Info();
            if (relation.ChangeSetId.HasValue) { pbfRelation.info.changeset = relation.ChangeSetId.Value; }
            if (relation.TimeStamp.HasValue) { pbfRelation.info.timestamp = Encoder.EncodeTimestamp(relation.TimeStamp.Value, block.date_granularity); }
            if (relation.UserId.HasValue) { pbfRelation.info.uid = (int)relation.UserId.Value; }
            pbfRelation.info.user_sid = Encoder.EncodeString(block, reverseStringTable, relation.UserName);
            pbfRelation.info.version = 0;
            if (relation.Version.HasValue) { pbfRelation.info.version = (int)relation.Version.Value; }

            if (relation.Tags != null)
            {
                foreach (var tag in relation.Tags)
                {
                    pbfRelation.keys.Add((uint)Encoder.EncodeString(block, reverseStringTable, tag.Key));
                    pbfRelation.vals.Add((uint)Encoder.EncodeString(block, reverseStringTable, tag.Value));
                }
            }

            if (relation.Members != null &&
                relation.Members.Length > 0)
            {
                pbfRelation.memids.Add(relation.Members[0].Id);
                pbfRelation.roles_sid.Add(Encoder.EncodeString(block, reverseStringTable, relation.Members[0].Role));
                switch (relation.Members[0].Type)
                {
                    case OsmGeoType.Node:
                        pbfRelation.types.Add(Relation.MemberType.NODE);
                        break;
                    case OsmGeoType.Way:
                        pbfRelation.types.Add(Relation.MemberType.WAY);
                        break;
                    case OsmGeoType.Relation:
                        pbfRelation.types.Add(Relation.MemberType.RELATION);
                        break;
                }
                for (var i = 1; i < relation.Members.Length; i++)
                {
                    pbfRelation.memids.Add(relation.Members[i].Id - 
                        relation.Members[i - 1].Id);
                    pbfRelation.roles_sid.Add(Encoder.EncodeString(block, reverseStringTable, relation.Members[i].Role));
                    switch(relation.Members[i].Type)
                    {
                        case OsmGeoType.Node:
                            pbfRelation.types.Add(Relation.MemberType.NODE);
                            break;
                        case OsmGeoType.Way:
                            pbfRelation.types.Add(Relation.MemberType.WAY);
                            break;
                        case OsmGeoType.Relation:
                            pbfRelation.types.Add(Relation.MemberType.RELATION);
                            break;
                    }
                }
            }
            return pbfRelation;
        }

        /// <summary>
        /// Encodes a lat/lon value into an offset.
        /// </summary>
        /// <returns></returns>
        public static long EncodeLatLon(float value, long offset, long granularity)
        {
            return ((long)(value / .000000001) - offset) / granularity;
        }

        /// <summary>
        /// Decodes a lat/lon value from an offset.
        /// </summary>
        /// <returns></returns>
        public static float DecodeLatLon(long valueOffset, long offset, long granularity)
        {
            return (float)(.000000001 * (offset + (granularity * valueOffset)));
        }

        /// <summary>
        /// Encodes a timestamp.
        /// </summary>
        public static int EncodeTimestamp(DateTime timestamp, long dateGranularity)
        {
            return (int)(timestamp.ToUnixTime() / dateGranularity);
        }

        /// <summary>
        /// Decodes a timestamp.
        /// </summary>
        public static DateTime DecodeTimestamp(int timestamp, long dateGranularity)
        {
            return (timestamp * dateGranularity).FromUnixTime();
        }
    }
}