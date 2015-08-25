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
using System.Linq;
using System.Text;
using OsmSharp.Collections.Tags;

namespace OsmSharp.Osm
{
    /// <summary>
    /// Represents a simple way.
    /// </summary>
    public class Way : OsmGeo
    {
        /// <summary>
        /// Creates a new simple way.
        /// </summary>
        public Way()
        {
            this.Type = OsmGeoType.Way;
        }

        /// <summary>
        /// Holds the list of nodes.
        /// </summary>
        public List<long>  Nodes { get; set; }

        /// <summary>
        /// Returns a description of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string tags = "{no tags}";
            if (this.Tags != null && this.Tags.Count > 0)
            {
                tags = this.Tags.ToString();
            }
            if (!this.Id.HasValue)
            {
                return string.Format("Way[null]{0}", tags);
            }
            return string.Format("Way[{0}]{1}", this.Id.Value, tags);
        }

        /// <summary>
        /// Creates a new way.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static Way Create(long id, params long[] nodes)
        {
            Way way = new Way();
            way.Id = id;
            way.Nodes = new List<long>(nodes);
            return way;
        }

        /// <summary>
        /// Creates a new way.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nodes"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static Way Create(long id, TagsCollectionBase tags, params long[] nodes)
        {
            Way way = new Way();
            way.Id = id;
            way.Nodes = new List<long>(nodes);
            way.Tags = tags;
            return way;
        }
    }
}
