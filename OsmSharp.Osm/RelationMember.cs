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

namespace OsmSharp.Osm
{
    /// <summary>
    /// Represents simple relation member.
    /// </summary>
    public class RelationMember
    {
        /// <summary>
        /// The type of this relation member.
        /// </summary>
        public OsmGeoType? MemberType { get; set; }

        /// <summary>
        /// The member id.
        /// </summary>
        public long? MemberId { get; set; }

        /// <summary>
        /// The member role.
        /// </summary>
        public string MemberRole { get; set; }

        /// <summary>
        /// Creates a new relation member.
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="memberRole"></param>
        /// <param name="memberType"></param>
        /// <returns></returns>
        public static RelationMember Create(int memberId, string memberRole, OsmGeoType memberType)
        {
            RelationMember member = new RelationMember();
            member.MemberId = memberId;
            member.MemberRole = memberRole;
            member.MemberType = memberType;
            return member;
        }
    }
}
