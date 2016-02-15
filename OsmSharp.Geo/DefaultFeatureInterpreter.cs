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

using GeoAPI.Geometries;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using OsmSharp.Complete;
using OsmSharp.Logging;
using OsmSharp.Tags;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Geo
{
    /// <summary>
    /// The default feature interpreter.
    /// </summary>
    public class DefaultFeatureInterpreter : FeatureInterpreter
    {
        /// <summary>
        /// Interprets an OSM-object and returns the corresponding geometry.
        /// </summary>
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
                    case OsmGeoType.Node:
                        var newCollection = new TagsCollection(
                            osmObject.Tags);
                        newCollection.RemoveKey("FIXME");
                        newCollection.RemoveKey("node");
                        newCollection.RemoveKey("source");

                        if (newCollection.Count > 0)
                        { // there is still some relevant information left.
                            collection.Add(new Feature(new Point((osmObject as Node).GetCoordinate()),
                                osmObject.Tags.ToAttributeTable()));
                        }
                        break;
                    case OsmGeoType.Way:
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
                            var lineairRing = new Feature(new LinearRing((osmObject as CompleteWay).GetCoordinates().
                                ToArray<Coordinate>()), tags.ToAttributeTable());
                            collection.Add(lineairRing);
                        }
                        else
                        { // no area tag leads to just a line.
                            var lineString = new Feature(new LineString((osmObject as CompleteWay).GetCoordinates().
                                ToArray<Coordinate>()), tags.ToAttributeTable());
                            collection.Add(lineString);
                        }
                        break;
                    case OsmGeoType.Relation:
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
            {
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
            List<KeyValuePair<bool, LinearRing>> rings;
            if (!this.AssignRings(ways, out rings))
            {
                Logging.Logger.Log("DefaultFeatureInterpreter", TraceEventType.Error,
                    string.Format("Ring assignment failed: invalid multipolygon relation [{0}] detected!", relation.Id));
            }
            // group the rings and create a multipolygon.
            var geometry = this.GroupRings(rings);
            if (geometry != null)
            {
                feature = new Feature(geometry, relation.Tags.ToAttributeTable());
            }
            return feature;
        }

        /// <summary>
        /// Groups the rings into polygons.
        /// </summary>
        private Geometry GroupRings(List<KeyValuePair<bool, LinearRing>> rings)
        {
            Geometry geometry = null;
            var containsFlags = new bool[rings.Count][]; // means [x] contains [y]
            for (var x = 0; x < rings.Count; x++)
            {
                var xPolygon = new Polygon(rings[x].Value);
                containsFlags[x] = new bool[rings.Count];
                for (var y = 0; y < x; y++)
                {
                    var yPolygon = new Polygon(rings[y].Value);
                    containsFlags[x][y] =
                        xPolygon.Contains(yPolygon);
                }
            }
            var used = new bool[rings.Count];
            List<Polygon> multiPolygon = null;
            while (used.Contains(false))
            { // select a ring not contained by any other.
                LinearRing outer = null;
                int outerIdx = -1;
                for (int idx = 0; idx < rings.Count; idx++)
                {
                    if (!used[idx] && this.CheckUncontained(rings, containsFlags, used, idx))
                    { // this ring is not contained in any other used rings.
                        if (!rings[idx].Key)
                        {
                            Logging.Logger.Log("DefaultFeatureInterpreter", TraceEventType.Error,
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
                    var inners = new List<LinearRing>();
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

                    var unused = !used.Contains(false);
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
                        var polygon = new Polygon(
                            outer, inners.ToArray());
                        geometry = polygon;
                        break;
                    }
                    else
                    { // there have to be other polygons.
                        if (multiPolygon == null)
                        {
                            multiPolygon = new List<Polygon>();
                        }
                        multiPolygon.Add(new Polygon(outer, inners.ToArray()));
                        geometry = new MultiPolygon(multiPolygon.ToArray());
                    }
                }
                else
                { // unused rings left but they cannot be designated as 'outer'.
                    Logging.Logger.Log("DefaultFeatureInterpreter", TraceEventType.Error,
                        "Invalid multipolygon relation: Unassigned rings left.");
                    break;
                }
            }
            return geometry;
        }

        /// <summary>
        /// Checks if a ring is not contained by any other unused ring.
        /// </summary>
        private bool CheckUncontained(List<KeyValuePair<bool, LinearRing>> rings,
            bool[][] containsFlags, bool[] used, int ringIdx)
        {
            for (var i = 0; i < rings.Count; i++)
            {
                if (i != ringIdx && !used[i] && containsFlags[i][ringIdx])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Tries to extract all rings from the given ways.
        /// </summary>
        private bool AssignRings(List<KeyValuePair<bool, CompleteWay>> ways, out List<KeyValuePair<bool, LinearRing>> rings)
        {
            return this.AssignRings(ways, new bool[ways.Count], out rings);
        }

        /// <summary>
        /// Assigns rings to the unassigned ways.
        /// </summary>
        private bool AssignRings(List<KeyValuePair<bool, CompleteWay>> ways, bool[] assignedFlags, out List<KeyValuePair<bool, LinearRing>> rings)
        {
            var assigned = false;
            for (var i = 0; i < ways.Count; i++)
            {
                if (!assignedFlags[i])
                {
                    assigned = true;
                    LinearRing ring;
                    if (this.AssignRing(ways, i, assignedFlags, out ring))
                    { // assigning the ring successed.
                        List<KeyValuePair<bool, LinearRing>> otherRings;
                        if (this.AssignRings(ways, assignedFlags, out otherRings))
                        { // assigning the rings succeeded.
                            rings = otherRings;
                            rings.Add(new KeyValuePair<bool, LinearRing>(ways[i].Key, ring));
                            return true;
                        }
                    }
                }
            }
            rings = new List<KeyValuePair<bool, LinearRing>>();
            return !assigned;
        }

        /// <summary>
        /// Creates a new lineair ring from the given way and updates the assigned flags array.
        /// </summary>
        private bool AssignRing(List<KeyValuePair<bool, CompleteWay>> ways, int way, bool[] assignedFlags, out LinearRing ring)
        {
            List<Coordinate> coordinates = null;
            assignedFlags[way] = true;
            if (ways[way].Value.IsClosed())
            { // the was is closed.
                coordinates = ways[way].Value.GetCoordinates();
            }
            else
            { // the way is open.
                var roleFlag = ways[way].Key;

                // complete the ring.
                var nodes = new List<Node>(ways[way].Value.Nodes);
                if (this.CompleteRing(ways, assignedFlags, nodes, roleFlag))
                { // the ring was completed!
                    coordinates = new List<Coordinate>(nodes.Count);
                    foreach (var node in nodes)
                    {
                        coordinates.Add(node.GetCoordinate());
                    }
                }
                else
                { // oeps, assignment failed: backtrack again!
                    assignedFlags[way] = false;
                    ring = null;
                    return false;
                }
            }
            ring = new LinearRing(coordinates.ToArray());
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
            for (var idx = 0; idx < ways.Count; idx++)
            {
                if (!assignedFlags[idx])
                { // way not assigned.
                    var wayEntry = ways[idx];
                    var nextWay = wayEntry.Value;
                    if (!role.HasValue || wayEntry.Key == role.Value)
                    { // only try matching roles if the role has been set.
                        List<Node> nextNodes = null;
                        if (nodes[nodes.Count - 1].Id == nextWay.Nodes[0].Id)
                        { // last node of the previous way is the first node of the next way.
                            nextNodes = nextWay.Nodes.GetRange(1, nextWay.Nodes.Length - 1);
                            assignedFlags[idx] = true;
                        }
                        else if (nodes[nodes.Count - 1].Id == nextWay.Nodes[nextWay.Nodes.Length - 1].Id)
                        { // last node of the previous way is the last node of the next way.
                            nextNodes = nextWay.Nodes.GetRange(0, nextWay.Nodes.Length - 1);
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