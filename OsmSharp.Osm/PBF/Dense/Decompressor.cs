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
using System.Collections.Generic;

namespace OsmSharp.Osm.PBF.Dense
{
    /// <summary>
    /// 'Decompresses' dense formatted primitives to their regular counterparts.
    /// </summary>
    internal class Decompressor : IPBFPrimitiveBlockConsumer
    {
        /// <summary>
        /// Holds the consumer of primitives.
        /// </summary>
        private IPBFOsmPrimitiveConsumer _primitives_consumer;

        /// <summary>
        /// Creates a decompressor.
        /// </summary>
        internal Decompressor(IPBFOsmPrimitiveConsumer primitives_consumer)
        {
            _primitives_consumer = primitives_consumer;
        }

        /// <summary>
        /// Consumes a primitive block.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="ignoreNodes"></param>
        /// <param name="ignoreWays"></param>
        /// <param name="ignoreRelations"></param>
        public bool ProcessPrimitiveBlock(PrimitiveBlock block, bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
        {
            bool success = false;

            if (block.primitivegroup != null)
            {
                foreach (PrimitiveGroup primitivegroup in block.primitivegroup)
                {
                    if (!ignoreNodes && primitivegroup.dense != null)
                    {
                        int key_vals_idx = 0;
                        long current_id = 0;
                        long current_lat = 0;
                        long current_lon = 0;
                        long current_changeset = 0;
                        long current_timestamp = 0;
                        int current_uid = 0;
                        int current_user_sid = 0;
                        int current_version = 0;

                        for (int idx = 0; idx < primitivegroup.dense.id.Count; idx++)
                        {
                            // do the delta decoding stuff.
                            current_id = current_id + 
                                primitivegroup.dense.id[idx];
                            current_lat = current_lat + 
                                primitivegroup.dense.lat[idx];
                            current_lon = current_lon + 
                                primitivegroup.dense.lon[idx];
                            if (primitivegroup.dense.denseinfo != null)
                            { // add all the metadata.
                                current_changeset = current_changeset +
                                    primitivegroup.dense.denseinfo.changeset[idx];
                                current_timestamp = current_timestamp +
                                    primitivegroup.dense.denseinfo.timestamp[idx];
                                current_uid = current_uid +
                                    primitivegroup.dense.denseinfo.uid[idx];
                                current_user_sid = current_user_sid +
                                    primitivegroup.dense.denseinfo.user_sid[idx];
                                current_version = current_version +
                                    primitivegroup.dense.denseinfo.version[idx];
                            }

                            Node node = new Node();
                            node.id = current_id;
                            node.info = new Info();
                            node.info.changeset = current_changeset;
                            node.info.timestamp = (int)current_timestamp;
                            node.info.uid = current_uid;
                            node.info.user_sid = current_user_sid;
                            node.info.version = current_version;
                            node.lat = current_lat;
                            node.lon = current_lon;

                            // get the keys/vals.
                            List<int> keys_vals = primitivegroup.dense.keys_vals;
//                            List<uint> keys = new List<uint>();
//                            List<uint> vals = new List<uint>();
                            while (keys_vals.Count > key_vals_idx && 
                                keys_vals[key_vals_idx] != 0)
                            {
                                node.keys.Add((uint)keys_vals[key_vals_idx]);
                                key_vals_idx++;
                                node.vals.Add((uint)keys_vals[key_vals_idx]);
                                key_vals_idx++;
                            }
                            key_vals_idx++;

                            success = true;
                            _primitives_consumer.ProcessNode(block, node);
                        }
                    }
                    else
                    {
                        if (!ignoreNodes && primitivegroup.nodes != null)
                        {
                            foreach (Node node in primitivegroup.nodes)
                            {
                                success = true;
                                _primitives_consumer.ProcessNode(block, node);
                            }
                        }
                        if (!ignoreWays && primitivegroup.ways != null)
                        {
                            foreach (Way way in primitivegroup.ways)
                            {
                                success = true;
                                _primitives_consumer.ProcessWay(block, way);
                            }
                        }
                        if (!ignoreRelations && primitivegroup.relations != null)
                        {
                            foreach (Relation relation in primitivegroup.relations)
                            {
                                success = true;
                                _primitives_consumer.ProcessRelation(block, relation);
                            }
                        }
                    }
                }
            }
            return success;
        }
    }
}