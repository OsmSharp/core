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

using OsmSharp.Osm.PBF;
using System;
using System.Collections.Generic;

namespace OsmSharp.Test.Osm.PBF
{
    /// <summary>
    /// A dummy primitives consumer.
    /// </summary>
    class PrimitivesConsumerMock : IPBFOsmPrimitiveConsumer
    {
        public PrimitivesConsumerMock()
        {
            this.Nodes = new List<Node>();
            this.Ways = new List<Way>();
            this.Relations = new List<Relation>();
        }

        public void ProcessNode(PrimitiveBlock block, Node node)
        {
            this.Nodes.Add(node);
        }

        public void ProcessWay(PrimitiveBlock block, Way way)
        {
            this.Ways.Add(way);
        }

        public void ProcessRelation(PrimitiveBlock block, Relation relation)
        {
            this.Relations.Add(relation);
        }

        public List<Node> Nodes { get; set; }

        public List<Way> Ways { get; set; }

        public List<Relation> Relations { get; set; }
    }
}
