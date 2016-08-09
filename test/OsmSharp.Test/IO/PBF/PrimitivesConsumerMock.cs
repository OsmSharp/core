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

using OsmSharp.IO.PBF;
using System.Collections.Generic;

namespace OsmSharp.Test.IO.PBF
{
    /// <summary>
    /// A dummy primitives consumer.
    /// </summary>
    class PrimitivesConsumerMock : IPBFOsmPrimitiveConsumer
    {
        public PrimitivesConsumerMock()
        {
            this.Nodes = new List<OsmSharp.IO.PBF.Node>();
            this.Ways = new List<OsmSharp.IO.PBF.Way>();
            this.Relations = new List<OsmSharp.IO.PBF.Relation>();
        }

        public void ProcessNode(PrimitiveBlock block, OsmSharp.IO.PBF.Node node)
        {
            this.Nodes.Add(node);
        }

        public void ProcessWay(PrimitiveBlock block, OsmSharp.IO.PBF.Way way)
        {
            this.Ways.Add(way);
        }

        public void ProcessRelation(PrimitiveBlock block, OsmSharp.IO.PBF.Relation relation)
        {
            this.Relations.Add(relation);
        }

        public List<OsmSharp.IO.PBF.Node> Nodes { get; set; }

        public List<OsmSharp.IO.PBF.Way> Ways { get; set; }

        public List<OsmSharp.IO.PBF.Relation> Relations { get; set; }
    }
}