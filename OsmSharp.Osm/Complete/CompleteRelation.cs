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
    /// A complete relation.
    /// </summary>
    public class CompleteRelation : CompleteOsmGeo
    {
        private readonly IList<CompleteRelationMember> _members;

        /// <summary>
        /// Creates a new relation.
        /// </summary>
        internal protected CompleteRelation(long id)
            : base(id)
        {
            _members = new List<CompleteRelationMember>();
        }

        /// <summary>
        /// Returns the relation type.
        /// </summary>
        public override CompleteOsmType Type
        {
            get { return CompleteOsmType.Relation; }
        }

        /// <summary>
        /// Gets the relation members.
        /// </summary>
        public IList<CompleteRelationMember> Members
        {
            get
            {
                return _members;
            }
        }

        /// <summary>
        /// Returns a description of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("http://www.openstreetmap.org/?relation={0}",
                this.Id);
        }
    }
}