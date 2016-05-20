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
using OsmSharp.Math.Geo;
using OsmSharp.Osm;
using System.Globalization;

namespace OsmSharp.Osm.Xml.v0_6
{
    /// <summary>
    /// Some common extensions.
    /// </summary>
    public static class Extensions
    {
        #region Convert From Xml

        /// <summary>
        /// Extensions method for a bound.
        /// </summary>
        /// <returns></returns>
        public static GeoCoordinateBox ConvertFrom(this bounds bounds)
        {
            if (bounds != null)
            {
                return new GeoCoordinateBox(
                    new GeoCoordinate((double)bounds.maxlat, (double)bounds.maxlon),
                    new GeoCoordinate((double)bounds.minlat, (double)bounds.minlon));
            }
            return null;
        }

        /// <summary>
        /// Extensions method for a bound.
        /// </summary>
        /// <returns></returns>
        public static GeoCoordinateBox ConvertFrom(this bound bound)
        {
            if (bound != null)
            {
                string[] bounds = bound.box.Split(',');
                return new GeoCoordinateBox(
                    new GeoCoordinate(double.Parse(bounds[0], CultureInfo.InvariantCulture),
                        double.Parse(bounds[1], CultureInfo.InvariantCulture)),
                    new GeoCoordinate(double.Parse(bounds[2], CultureInfo.InvariantCulture),
                        double.Parse(bounds[3], CultureInfo.InvariantCulture)));
            }
            return null;
        }

        /// <summary>
        /// Converts an Xml relation to an Osm domain model relation.
        /// </summary>
        /// <returns></returns>
        public static Relation ConvertFrom(this relation xmlRelation)
        {
            // create a new node and immidiately set the id.
            var relation = new Relation();
            relation.Id = xmlRelation.id;

            // set the members
            relation.Members = new List<RelationMember>(xmlRelation.member.Length);
            for (var i = 0; i < xmlRelation.member.Length; i++)
            {
                var xmlMember = xmlRelation.member[i];
                var member = new RelationMember();
                member.MemberId = xmlMember.@ref;
                member.MemberRole = xmlMember.role;
                if (xmlMember.refSpecified && xmlMember.typeSpecified)
                {
                    switch (xmlMember.type)
                    {
                        case memberType.node:
                            member.MemberType = OsmGeoType.Node;
                            break;
                        case memberType.relation:
                            member.MemberType = OsmGeoType.Relation;
                            break;
                        case memberType.way:
                            member.MemberType = OsmGeoType.Way;
                            break;
                    }
                }
                else
                { // way cannot be converted; member could not be created!
                    return null;
                }
                relation.Members.Add(member);
            }

            // set the tags.
            if (xmlRelation.tag != null)
            {
                relation.Tags = new TagsCollection();
                foreach (var tag in xmlRelation.tag)
                {
                    relation.Tags.Add(tag.k, tag.v);
                }
            }

            // set the user info.
            if (xmlRelation.uidSpecified)
            {
                relation.UserId = xmlRelation.uid;
            }
            relation.UserName = xmlRelation.user;

            // set the changeset info.
            if (xmlRelation.changesetSpecified)
            {
                relation.ChangeSetId = xmlRelation.changeset;
            }

            // set the timestamp flag.
            if (xmlRelation.timestampSpecified)
            {
                relation.TimeStamp = xmlRelation.timestamp;
            }

            // set the version flag.
            if(xmlRelation.versionSpecified)
            {
                relation.Version = xmlRelation.version;
            }

            // set the visible flag.
            relation.Visible = xmlRelation.visible;
            return relation;
        }

        /// <summary>
        /// Converts an Xml way to an Osm domain model way.
        /// </summary>
        /// <returns></returns>
        public static Way ConvertFrom(this way xmlWay)
        {
            // create a new node and immidiately set the id.
            var way = new Way();
            way.Id = xmlWay.id;

            // set the nodes.
            way.Nodes = new List<long>(xmlWay.nd.Length);
            for (int i = 0; i < xmlWay.nd.Length;i++)
            {
                var xmlNode = xmlWay.nd[i];
                if (xmlNode.refSpecified)
                {
                    way.Nodes.Add(xmlNode.@ref);
                }
                else
                { // way cannot be converted; node was not found!
                    return null;
                }
            }

            // set the tags.
            if (xmlWay.tag != null)
            {
                way.Tags = new TagsCollection();
                foreach (var tag in xmlWay.tag)
                {
                    way.Tags.Add(tag.k, tag.v);
                }
            }

            // set the user info.
            if (xmlWay.uidSpecified)
            {
                way.UserId = xmlWay.uid;
            }
            way.UserName = xmlWay.user;

            // set the changeset info.
            if (xmlWay.changesetSpecified)
            {
                way.ChangeSetId = xmlWay.changeset;
            }

            // set the timestamp flag.
            if (xmlWay.timestampSpecified)
            {
                way.TimeStamp = xmlWay.timestamp;
            }

            // set the version flag.
            if (xmlWay.versionSpecified)
            {
                way.Version = xmlWay.version;
            }

            // set the visible flag.
            way.Visible = xmlWay.visible;
            return way;
        }

        /// <summary>
        /// Converts an Xml node to an Osm domain model node.
        /// </summary>
        /// <returns></returns>
        public static Node ConvertFrom(this node xmlNode)
        {
            // create a new node an immidiately set the id.
            var node = new Node();
            node.Id = xmlNode.id;

            // set the long- and latitude
            if (!xmlNode.latSpecified || !xmlNode.lonSpecified)
            {
                throw new ArgumentNullException("Latitude and/or longitude cannot be null!");
            }
            node.Latitude = (double)xmlNode.lat;
            node.Longitude = (double)xmlNode.lon;

            // set the tags.
            if (xmlNode.tag != null)
            {
                node.Tags = new TagsCollection();
                foreach (var tag in xmlNode.tag)
                {
                    node.Tags.Add(tag.k, tag.v);
                }
            }

            // set the user info.
            if (xmlNode.uidSpecified)
            {
                node.UserId = (int)xmlNode.uid;
            }
            node.UserName = xmlNode.user;

            // set the changeset info.
            if (xmlNode.changesetSpecified)
            {
                node.ChangeSetId = (int)xmlNode.changeset;
            }

            // set the timestamp flag.
            if (xmlNode.timestampSpecified)
            {
                node.TimeStamp = xmlNode.timestamp;
            }

            // set the version flag.
            if (xmlNode.versionSpecified)
            {
                node.Version = xmlNode.version;
            }

            // set the visible flag.
            node.Visible = xmlNode.visible;
            return node;
        }

        #endregion

        #region Convert To Xml

        /// <summary>
        /// Extensions method for a geocoordinatebox.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public static bounds ConvertTo(this GeoCoordinateBox box)
        {
            var b = new bounds();

            b.maxlat = box.MaxLat;
            b.maxlatSpecified = true;
            b.maxlon = box.MaxLon;
            b.maxlonSpecified = true;
            b.minlat = box.MinLat;
            b.minlatSpecified = true;
            b.minlon = box.MinLon;
            b.minlonSpecified = true;

            return b;
        }

        /// <summary>
        /// Converts an Xml node to an Osm domain model node.
        /// </summary>
        /// <returns></returns>
        public static node ConvertTo(this OsmSharp.Osm.Node node)
        {
            var xmlNode = new node();

            // set the changeset.
            if (node.ChangeSetId.HasValue)
            {
                xmlNode.changeset = node.ChangeSetId.Value;
                xmlNode.changesetSpecified = true;
            }

            // set the id.
            if (node.Id.HasValue)
            {
                xmlNode.id = node.Id.Value;
                xmlNode.idSpecified = true;
            }

            if (node.Tags != null)
            {
                xmlNode.tag = new tag[node.Tags.Count];
                int idx = 0;
                foreach (var tag in node.Tags)
                {
                    var t = new tag();
                    t.k = tag.Key;
                    t.v = tag.Value;
                    xmlNode.tag[idx] = t;
                    idx++;
                }
            }

            // set the timestamp.
            if (node.TimeStamp.HasValue)
            {
                xmlNode.timestamp = node.TimeStamp.Value;
                xmlNode.timestampSpecified = true;
            }

            // set the user data.
            if (node.UserId.HasValue)
            {
                xmlNode.uid = node.UserId.Value;
                xmlNode.uidSpecified = true;
            }
            xmlNode.user = xmlNode.user;

            // set the version.
            if (node.Version.HasValue)
            {
                xmlNode.version = (ulong)node.Version.Value;
                xmlNode.versionSpecified = true;
            }

            // set the visible.
            if (node.Visible.HasValue)
            {
                xmlNode.visible = node.Visible.Value;
                xmlNode.visibleSpecified = true;
            }

            // set the node-specific properties.
            if (node.Latitude.HasValue)
            {
                xmlNode.lat = node.Latitude.Value;
                xmlNode.latSpecified = true;
            }
            if (node.Longitude.HasValue)
            {
                xmlNode.lon = node.Longitude.Value;
                xmlNode.lonSpecified = true;
            }

            return xmlNode;
        }

        /// <summary>
        /// Converts a domain model way to an Xml way.
        /// </summary>
        /// <param name="way"></param>
        /// <returns></returns>
        public static way ConvertTo(this OsmSharp.Osm.Way way)
        {
            way xmlWay = new way();

            // set the changeset.
            if (way.ChangeSetId.HasValue)
            {
                xmlWay.changeset = way.ChangeSetId.Value;
                xmlWay.changesetSpecified = true;
            }

            // set the id.
            if (way.Id.HasValue)
            {
                xmlWay.id = way.Id.Value;
                xmlWay.idSpecified = true;
            }
            else
            {
                xmlWay.idSpecified = false;
            }

            if (way.Tags != null)
            {
                xmlWay.tag = new tag[way.Tags.Count];
                int idx = 0;
                foreach (var tag in way.Tags)
                {
                    var t = new tag();
                    t.k = tag.Key;
                    t.v = tag.Value;
                    xmlWay.tag[idx] = t;
                    idx++;
                }
            }

            // set the timestamp.
            if (way.TimeStamp.HasValue)
            {
                xmlWay.timestamp = way.TimeStamp.Value;
                xmlWay.timestampSpecified = true;
            }

            // set the user data.
            if (way.UserId.HasValue)
            {
                xmlWay.uid = way.UserId.Value;
                xmlWay.uidSpecified = true;
            }
            xmlWay.user = xmlWay.user;

            // set the version.
            if (way.Version.HasValue)
            {
                xmlWay.version = (ulong)way.Version.Value;
                xmlWay.versionSpecified = true;
            }

            // set the visible.
            if (way.Visible.HasValue)
            {
                xmlWay.visible = way.Visible.Value;
                xmlWay.visibleSpecified = true;
            }
            else
            {
                xmlWay.visibleSpecified = false;
            }

            // set the way-specific properties.
            xmlWay.nd = new nd[way.Nodes.Count];
            for (int i = 0; i < way.Nodes.Count; i++)
            {
                var n = new nd();
                n.@ref = way.Nodes[i];
                n.refSpecified = true;
                xmlWay.nd[i] = n;
            }

            return xmlWay;
        }

        /// <summary>
        /// Converts a domain model relation to an Xml relation.
        /// </summary>
        /// <returns></returns>
        public static relation ConvertTo(this OsmSharp.Osm.Relation relation)
        {
            relation xmlRelation = new relation();

            // set the changeset.
            if (relation.ChangeSetId.HasValue)
            {
                xmlRelation.changeset = relation.ChangeSetId.Value;
                xmlRelation.changesetSpecified = true;
            }

            // set the id.
            if (relation.Id.HasValue)
            {
                xmlRelation.id = relation.Id.Value;
                xmlRelation.idSpecified = true;
            }
            else
            {
                xmlRelation.idSpecified = false;
            }

            if (relation.Tags != null)
            {
                xmlRelation.tag = new tag[relation.Tags.Count];
                int idx = 0;
                foreach (var tag in relation.Tags)
                {
                    tag t = new tag();
                    t.k = tag.Key;
                    t.v = tag.Value;
                    xmlRelation.tag[idx] = t;
                    idx++;
                }
            }

            // set the timestamp.
            if (relation.TimeStamp.HasValue)
            {
                xmlRelation.timestamp = relation.TimeStamp.Value;
                xmlRelation.timestampSpecified = true;
            }

            // set the user data.
            if (relation.UserId.HasValue)
            {
                xmlRelation.uid = relation.UserId.Value;
                xmlRelation.uidSpecified = true;
            }
            xmlRelation.user = xmlRelation.user;

            // set the version.
            if (relation.Version.HasValue)
            {
                xmlRelation.version = (ulong)relation.Version.Value;
                xmlRelation.versionSpecified = true;
            }

            // set the visible.
            if (relation.Visible.HasValue)
            {
                xmlRelation.visible = relation.Visible.Value;
                xmlRelation.visibleSpecified = true;
            }
            else
            {
                xmlRelation.visibleSpecified = false;
            }

            // set the way-specific properties.
            xmlRelation.member = new member[relation.Members.Count];
            for (int i = 0; i < relation.Members.Count; i++)
            {
                var member = relation.Members[i];
                var m = new member();

                if (member.MemberType.HasValue)
                {
                    switch (member.MemberType.Value)
                    {
                        case OsmGeoType.Node:
                            m.type = memberType.node;
                            m.typeSpecified = true;
                            break;
                        case OsmGeoType.Relation:
                            m.type = memberType.relation;
                            m.typeSpecified = true;
                            break;
                        case OsmGeoType.Way:
                            m.type = memberType.way;
                            m.typeSpecified = true;
                            break;
                    }
                }
                else
                {
                    m.typeSpecified = false;
                }

                if (member.MemberId.HasValue)
                {
                    m.@ref = member.MemberId.Value;
                    m.refSpecified = true;
                }
                else
                {
                    m.refSpecified = false;
                }
                m.role = member.MemberRole;

                xmlRelation.member[i] = m;
            }

            return xmlRelation;
        }

        #endregion
    }
}
