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

namespace OsmSharp.Osm
{
    /// <summary>
    /// Way class.
    /// </summary>
    public class CompleteWay : CompleteOsmGeo
    {
        private readonly List<Node> _nodes;

        /// <summary>
        /// Creates a new way.
        /// </summary>
        public CompleteWay(long id)
            : base(id)
        {
            _nodes = new List<Node>();
        }

        /// <summary>
        /// Returns the way type.
        /// </summary>
        public override CompleteOsmType Type
        {
            get { return CompleteOsmType.Way; }
        }

        /// <summary>
        /// Gets the ordered list of nodes.
        /// </summary>
        public List<Node> Nodes
        {
            get
            {
                return _nodes;
            }
        }

        /// <summary>
        /// Returns a description of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("http://www.openstreetmap.org/?way={0}",
                this.Id);
        }
    }
}