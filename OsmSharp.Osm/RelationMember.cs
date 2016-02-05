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

using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace OsmSharp.Osm
{
    /// <summary>
    /// Represents simple relation member.
    /// </summary>
    public class RelationMember : IXmlSerializable
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
        public static RelationMember Create(int memberId, string memberRole, OsmGeoType memberType)
        {
            var member = new RelationMember();
            member.MemberId = memberId;
            member.MemberRole = memberRole;
            member.MemberType = memberType;
            return member;
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.MoveToContent();

            this.MemberId = reader.GetAttributeInt64("ref");
            this.MemberRole = reader.GetAttribute("role");
            var type = reader.GetAttribute("type");
            switch(type)
            {
                case "node":
                    this.MemberType = OsmGeoType.Node;
                    break;
                case "way":
                    this.MemberType = OsmGeoType.Way;
                    break;
                case "relation":
                    this.MemberType = OsmGeoType.Relation;
                    break;
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            switch (this.MemberType)
            {
                case OsmGeoType.Node:
                    writer.WriteAttribute("type", "node");
                    break;
                case OsmGeoType.Way:
                    writer.WriteAttribute("type", "way");
                    break;
                case OsmGeoType.Relation:
                    writer.WriteAttribute("type", "relation");
                    break;
            }
            writer.WriteAttribute("ref", this.MemberId);
            writer.WriteAttribute("role", this.MemberRole);
        }
    }
}
