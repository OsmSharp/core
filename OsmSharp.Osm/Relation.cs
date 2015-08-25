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
    /// Represents a simple relation.
    /// </summary>
    public class Relation : OsmGeo
    {
        /// <summary>
        /// Creates new simple relation.
        /// </summary>
        public Relation()
        {
            this.Type = OsmGeoType.Relation;
        }

        /// <summary>
        /// The relation members.
        /// </summary>
        public List<RelationMember> Members { get; set; }

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
                return string.Format("Relation[null]{0}", tags);
            }
            return string.Format("Relation[{0}]{1}", this.Id.Value, tags);
        }

        /// <summary>
        /// Creates a new relation.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="members"></param>
        /// <returns></returns>
        public static Relation Create(long id, params RelationMember[] members)
        {
            Relation relation = new Relation();
            relation.Id = id;
            relation.Members = new List<RelationMember>(members);
            return relation;
        }

        /// <summary>
        /// Creates a new relation.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tags"></param>
        /// <param name="members"></param>
        /// <returns></returns>
        public static Relation Create(long id, TagsCollectionBase tags, params RelationMember[] members)
        {
            Relation relation = new Relation();
            relation.Id = id;
            relation.Members = new List<RelationMember>(members);
            relation.Tags = tags;
            return relation;
        }
    }
}