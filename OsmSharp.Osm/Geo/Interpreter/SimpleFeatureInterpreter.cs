// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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
using System.Linq;
using OsmSharp.Collections.Tags;
using OsmSharp.Geo.Attributes;
using OsmSharp.Geo.Geometries;
using OsmSharp.Math.Geo;
using OsmSharp.Logging;
using OsmSharp.Geo.Features;

namespace OsmSharp.Osm.Geo.Interpreter
{
    /// <summary>
    /// Simple implementation of OSM-data interpreter.
    /// </summary>
    public class SimpleFeatureInterpreter : FeatureInterpreter
    {
        /// <summary>
        /// Interprets an OSM-object and returns the corresponding geometry.
        /// </summary>
        /// <param name="osmObject"></param>
        /// <returns></returns>
        public override FeatureCollection Interpret(ICompleteOsmGeo osmObject)
        {
            // DISCLAIMER: this is a very very very simple geometry interpreter and
            // contains hardcoded all relevant tags.

            var collection = new FeatureCollection();
            TagsCollectionBase tags;
            if (osmObject != null)
            {
                switch (osmObject.Type)
                {
                    case CompleteOsmType.Node:
                        var newCollection = new TagsCollection(
                            osmObject.Tags);
                        newCollection.RemoveKey("FIXME");
                        newCollection.RemoveKey("node");
                        newCollection.RemoveKey("source");

                        if (newCollection.Count > 0)
                        { // there is still some relevant information left.
                            collection.Add(new Feature(new Point((osmObject as Node).Coordinate),
                                new SimpleGeometryAttributeCollection(osmObject.Tags)));
                        }
                        break;
                    case CompleteOsmType.Way:
                        tags = osmObject.Tags;

                        bool isArea = false;
                        if ((tags.ContainsKey("building") && !tags.IsFalse("building")) ||
                            (tags.ContainsKey("landuse") && !tags.IsFalse("landuse")) ||
                            (tags.ContainsKey("amenity") && !tags.IsFalse("amenity")) ||
                            (tags.ContainsKey("harbour") && !tags.IsFalse("harbour")) ||
                            (tags.ContainsKey("historic") && !tags.IsFalse("historic")) ||
                            (tags.ContainsKey("leisure") && !tags.IsFalse("leisure")) ||
                            (tags.ContainsKey("man_made") && !tags.IsFalse("man_made")) ||
                            (tags.ContainsKey("military") && !tags.IsFalse("military")) ||
                            (tags.ContainsKey("natural") && !tags.IsFalse("natural")) ||
                            (tags.ContainsKey("office") && !tags.IsFalse("office")) ||
                            (tags.ContainsKey("place") && !tags.IsFalse("place")) ||
                            (tags.ContainsKey("power") && !tags.IsFalse("power")) ||
                            (tags.ContainsKey("public_transport") && !tags.IsFalse("public_transport")) ||
                            (tags.ContainsKey("shop") && !tags.IsFalse("shop")) ||
                            (tags.ContainsKey("sport") && !tags.IsFalse("sport")) ||
                            (tags.ContainsKey("tourism") && !tags.IsFalse("tourism")) ||
                            (tags.ContainsKey("waterway") && !tags.IsFalse("waterway")) ||
                            (tags.ContainsKey("wetland") && !tags.IsFalse("wetland")) ||
                            (tags.ContainsKey("water") && !tags.IsFalse("water")) ||
                            (tags.ContainsKey("aeroway") && !tags.IsFalse("aeroway")))
                        { // these tags usually indicate an area.
                            isArea = true;
                        }

                        if (tags.IsTrue("area"))
                        { // explicitly indicated that this is an area.
                            isArea = true;
                        }
                        else if (tags.IsFalse("area"))
                        { // explicitly indicated that this is not an area.
                            isArea = false;
                        }

                        if (isArea)
                        { // area tags leads to simple polygon
                            var lineairRing = new Feature(new LineairRing((osmObject as CompleteWay).GetCoordinates().ToArray<GeoCoordinate>()),
                                new SimpleGeometryAttributeCollection(tags));
                            collection.Add(lineairRing);
                        }
                        else
                        { // no area tag leads to just a line.
                            var lineString = new Feature(new LineString((osmObject as CompleteWay).GetCoordinates().ToArray<GeoCoordinate>()),
                                new SimpleGeometryAttributeCollection(tags));
                            collection.Add(lineString);
                        }
                        break;
                    case CompleteOsmType.Relation:
                        var relation = (osmObject as CompleteRelation);
                        tags = relation.Tags;

                        string typeValue;
                        if (tags.TryGetValue("type", out typeValue))
                        { // there is a type in this relation.
                            if (typeValue == "multipolygon")
                            { // this relation is a multipolygon.
                                var feature = this.InterpretMultipolygonRelation(relation);
                                if (feature != null)
                                { // add the geometry.
                                    collection.Add(feature);
                                }
                            }
                            else if (typeValue == "boundary")
                            { // this relation is a boundary.

                            }
                        }
                        break;
                }
            }
            return collection;
        }

        /// <summary>
        /// Returns true if the given tags collection contains tags that could represents an area.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public override bool IsPotentiallyArea(TagsCollectionBase tags)
        {
            if (tags == null || tags.Count == 0) { return false; } // no tags, assume no area.

            bool isArea = false;
            if ((tags.ContainsKey("building") && !tags.IsFalse("building")) ||
                (tags.ContainsKey("landuse") && !tags.IsFalse("landuse")) ||
                (tags.ContainsKey("amenity") && !tags.IsFalse("amenity")) ||
                (tags.ContainsKey("harbour") && !tags.IsFalse("harbour")) ||
                (tags.ContainsKey("historic") && !tags.IsFalse("historic")) ||
                (tags.ContainsKey("leisure") && !tags.IsFalse("leisure")) ||
                (tags.ContainsKey("man_made") && !tags.IsFalse("man_made")) ||
                (tags.ContainsKey("military") && !tags.IsFalse("military")) ||
                (tags.ContainsKey("natural") && !tags.IsFalse("natural")) ||
                (tags.ContainsKey("office") && !tags.IsFalse("office")) ||
                (tags.ContainsKey("place") && !tags.IsFalse("place")) ||
                (tags.ContainsKey("power") && !tags.IsFalse("power")) ||
                (tags.ContainsKey("public_transport") && !tags.IsFalse("public_transport")) ||
                (tags.ContainsKey("shop") && !tags.IsFalse("shop")) ||
                (tags.ContainsKey("sport") && !tags.IsFalse("sport")) ||
                (tags.ContainsKey("tourism") && !tags.IsFalse("tourism")) ||
                (tags.ContainsKey("waterway") && !tags.IsFalse("waterway")) ||
                (tags.ContainsKey("wetland") && !tags.IsFalse("wetland")) ||
                (tags.ContainsKey("water") && !tags.IsFalse("water")) ||
                (tags.ContainsKey("aeroway") && !tags.IsFalse("aeroway")))
            { // these tags usually indicate an area.
                isArea = true;
            }

            string typeValue;
            if (tags.TryGetValue("type", out typeValue))
            { // there is a type in this relation.
                if (typeValue == "multipolygon")
                { // this relation is a multipolygon.
                    isArea = true;
                }
                else if (typeValue == "boundary")
                { // this relation is a boundary.
                    isArea = true;
                }
            }

            if (tags.IsTrue("area"))
            { // explicitly indicated that this is an area.
                isArea = true;
            }
            else if (tags.IsFalse("area"))
            { // explicitly indicated that this is not an area.
                isArea = false;
            }

            return isArea;
        }

        /// <summary>
        /// Tries to interpret a given multipolygon relation.
        /// </summary>
        /// <param name="relation"></param>
        /// <returns></returns>
        private Feature InterpretMultipolygonRelation(CompleteRelation relation)
        {
            Feature feature = null;
            if (relation.Members == null)
            { // the relation has no members.
                return feature;
            }

            // build lists of outer and inner ways.
            var ways = new List<KeyValuePair<bool, CompleteWay>>(); // outer=true
            foreach (var member in relation.Members)
            {
                if (member.Role == "inner" &&
                    member.Member is CompleteWay)
                { // an inner member.
                    ways.Add(new KeyValuePair<bool, CompleteWay>(
                        false, member.Member as CompleteWay));
                }
                else if (member.Role == "outer" &&
                    member.Member is CompleteWay)
                { // an outer member.
                    ways.Add(new KeyValuePair<bool, CompleteWay>(
                        true, member.Member as CompleteWay));
                }
            }
            
            // started a similar algorithm and then found this:
            // loosely based on: http://wiki.openstreetmap.org/wiki/Relation:multipolygon/Algorithm

            // recusively try to assign the rings.
            List<KeyValuePair<bool, LineairRing>> rings;
            if (!this.AssignRings(ways, out rings))
            {
                OsmSharp.Logging.Log.TraceEvent("OsmSharp.Osm.Interpreter.SimpleGeometryInterpreter", TraceEventType.Error,
                    string.Format("Ring assignment failed: invalid multipolygon relation [{0}] detected!", relation.Id));
            }
            // group the rings and create a multipolygon.
            var geometry = this.GroupRings(rings);
            if (geometry != null)
            {
                feature = new Feature(geometry, 
                    new SimpleGeometryAttributeCollection(relation.Tags));
            }
            return feature;
        }

        /// <summary>
        /// Groups the rings into polygons.
        /// </summary>
        /// <param name="rings"></param>
        /// <returns></returns>
        private Geometry GroupRings(List<KeyValuePair<bool, LineairRing>> rings)
        {
            Geometry geometry = null;
            bool[][] containsFlags = new bool[rings.Count][]; // means [x] contains [y]
            for (int x = 0; x < rings.Count; x++)
            {
                containsFlags[x] = new bool[rings.Count];
                for (int y = 0; y < x; y++)
                {
                    containsFlags[x][y] =
                        rings[x].Value.Contains(rings[y].Value);
                }
            }
            bool[] used = new bool[rings.Count];
            MultiPolygon multiPolygon = null;
            while (used.Contains(false))
            { // select a ring not contained by any other.
                LineairRing outer = null;
                int outerIdx = -1;
                for (int idx = 0; idx < rings.Count; idx++)
                {
                    if (!used[idx] && this.CheckUncontained(rings, containsFlags, used, idx))
                    { // this ring is not contained in any other used rings.
                        if (!rings[idx].Key)
                        {
                            OsmSharp.Logging.Log.TraceEvent("OsmSharp.Osm.Interpreter.SimpleGeometryInterpreter", TraceEventType.Error,
                                "Invalid multipolygon relation: an 'inner' ring was detected without an 'outer'.");
                        }
                        outerIdx = idx;
                        outer = rings[idx].Value;
                        used[idx] = true;
                        break;
                    }
                }
                if (outer != null)
                { // an outer ring was found, find inner rings.
                    List<LineairRing> inners = new List<LineairRing>();
                    // select all rings contained by inner but not by any others.
                    for (int idx = 0; idx < rings.Count; idx++)
                    {
                        if (!used[idx] && containsFlags[outerIdx][idx] &&
                            this.CheckUncontained(rings, containsFlags, used, idx))
                        {
                            inners.Add(rings[idx].Value);
                            used[idx] = true;
                        }
                    }

                    bool unused = !used.Contains(false);
                    if (multiPolygon == null &&
                        inners.Count == 0 &&
                        unused)
                    { // there is just one lineair ring.
                        geometry = outer;
                        break;
                    }
                    else if (multiPolygon == null &&
                        unused)
                    { // there is just one polygon.
                        Polygon polygon = new Polygon(
                            outer, inners);
                        geometry = polygon;
                        break;
                    }
                    else
                    { // there have to be other polygons.
                        {
                            multiPolygon = new MultiPolygon();
                            geometry = multiPolygon;
                        }
                        Polygon polygon = new Polygon(
                            outer, inners);
                        multiPolygon.Add(polygon);
                    }
                }
                else
                { // unused rings left but they cannot be designated as 'outer'.
                    OsmSharp.Logging.Log.TraceEvent("OsmSharp.Osm.Interpreter.SimpleGeometryInterpreter", TraceEventType.Error,
                        "Invalid multipolygon relation: Unassigned rings left.");
                    break;
                }
            }
            return geometry;
        }

        /// <summary>
        /// Checks if a ring is not contained by any other unused ring.
        /// </summary>
        /// <param name="rings"></param>
        /// <param name="containsFlags"></param>
        /// <param name="used"></param>
        /// <param name="ringIdx"></param>
        /// <returns></returns>
        private bool CheckUncontained(List<KeyValuePair<bool, LineairRing>> rings,
            bool[][] containsFlags, bool[] used, int ringIdx)
        {
            for (int idx = 0; idx < rings.Count; idx++)
            {
                if (idx != ringIdx && !used[idx] && containsFlags[idx][ringIdx])
                { // oeps: the ring at index 'ringIdx' is contained inside another.
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Tries to extract all rings from the given ways.
        /// </summary>
        /// <param name="ways"></param>
        /// <param name="rings"></param>
        /// <returns></returns>
        private bool AssignRings(
            List<KeyValuePair<bool, CompleteWay>> ways, out List<KeyValuePair<bool, LineairRing>> rings)
        {
            return this.AssignRings(ways, new bool[ways.Count], out rings);
        }

        /// <summary>
        /// Assigns rings to the unassigned ways.
        /// </summary>
        /// <param name="ways"></param>
        /// <param name="assignedFlags"></param>
        /// <param name="rings"></param>
        /// <returns></returns>
        private bool AssignRings(
            List<KeyValuePair<bool, CompleteWay>> ways, bool[] assignedFlags, out List<KeyValuePair<bool, LineairRing>> rings)
        {
            bool assigned = false;
            for (int idx = 0; idx < ways.Count; idx++)
            {
                if (!assignedFlags[idx])
                {
                    assigned = true;
                    LineairRing ring;
                    if (this.AssignRing(ways, idx, assignedFlags, out ring))
                    { // assigning the ring successed.
                        List<KeyValuePair<bool, LineairRing>> otherRings;
                        if (this.AssignRings(ways, assignedFlags, out otherRings))
                        { // assigning the rings succeeded.
                            rings = otherRings;
                            rings.Add(new KeyValuePair<bool, LineairRing>(ways[idx].Key, ring));
                            return true;
                        }
                    }
                }
            }
            rings = new List<KeyValuePair<bool,LineairRing>>();
            return !assigned;
        }

        /// <summary>
        /// Creates a new lineair ring from the given way and updates the assigned flags array.
        /// </summary>
        /// <param name="ways"></param>
        /// <param name="way"></param>
        /// <param name="assignedFlags"></param>
        /// <param name="ring"></param>
        /// <returns></returns>
        private bool AssignRing(List<KeyValuePair<bool, CompleteWay>> ways, int way, bool[] assignedFlags, out LineairRing ring)
        {
            List<GeoCoordinate> coordinates = null;
            assignedFlags[way] = true;
            if (ways[way].Value.IsClosed())
            { // the was is closed.
                // TODO: validate geometry: this should be a non-intersecting way.
                coordinates = ways[way].Value.GetCoordinates();
            }
            else
            { // the way is open.
                bool roleFlag = ways[way].Key;

                // complete the ring.
                List<Node> nodes = new List<Node>(ways[way].Value.Nodes);
                if (this.CompleteRing(ways, assignedFlags, nodes, roleFlag))
                { // the ring was completed!
                    coordinates = new List<GeoCoordinate>(nodes.Count);
                    foreach (Node node in nodes)
                    {
                        coordinates.Add(node.Coordinate);
                    }
                }
                else
                { // oeps, assignment failed: backtrack again!
                    assignedFlags[way] = false;
                    ring = null;
                    return false;
                }
            }
            ring = new LineairRing(coordinates);
            return true;
        }

        /// <summary>
        /// Completes an uncompleted ring.
        /// </summary>
        /// <param name="ways"></param>
        /// <param name="assignedFlags"></param>
        /// <param name="nodes"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        private bool CompleteRing(List<KeyValuePair<bool, CompleteWay>> ways, bool[] assignedFlags, 
            List<Node> nodes, bool? role)
        {
            for (int idx = 0; idx < ways.Count; idx++)
            {
                if (!assignedFlags[idx])
                { // way not assigned.
                    KeyValuePair<bool, CompleteWay> wayEntry = ways[idx];
                    CompleteWay nextWay = wayEntry.Value;
                    if (!role.HasValue || wayEntry.Key == role.Value)
                    { // only try matching roles if the role has been set.
                        List<Node> nextNodes = null;
                        if (nodes[nodes.Count - 1].Id == nextWay.Nodes[0].Id)
                        { // last node of the previous way is the first node of the next way.
                            nextNodes = nextWay.Nodes.GetRange(1, nextWay.Nodes.Count - 1);
                            assignedFlags[idx] = true;
                        }
                        else if (nodes[nodes.Count - 1].Id == nextWay.Nodes[nextWay.Nodes.Count - 1].Id)
                        { // last node of the previous way is the last node of the next way.
                            nextNodes = nextWay.Nodes.GetRange(0, nextWay.Nodes.Count - 1);
                            nextNodes.Reverse();
                            assignedFlags[idx] = true;
                        }

                        // add the next nodes if any.
                        if (assignedFlags[idx])
                        { // yep, way was assigned!
                            nodes.AddRange(nextNodes);
                            if (nodes[nodes.Count - 1].Id == nodes[0].Id)
                            { // yes! a closed ring was found!
                                return true;
                            }
                            else
                            { // noo! ring not closed yet!
                                if (this.CompleteRing(ways, assignedFlags, nodes, role))
                                { // yes! a complete ring was found
                                    return true;
                                }
                                else
                                { // damn complete ring not found. backtrack people!
                                    assignedFlags[idx] = false;
                                    nodes.RemoveRange(nodes.Count - nextNodes.Count, nextNodes.Count);
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}