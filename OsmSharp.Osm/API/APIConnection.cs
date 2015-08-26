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

using OsmSharp.Collections.Tags;
using OsmSharp.IO.Xml.Sources;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Data.Core.API;
using OsmSharp.Osm.Xml;
using OsmSharp.Osm.Xml.v0_6;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace OsmSharp.Osm.API
{
    /// <summary>
    /// Represents an osm api instance.
    /// </summary>
    public class APIConnection
    {
        /// <summary>
        /// Holds a username.
        /// </summary>
        private readonly string _user;

        /// <summary>
        /// Holds the password.
        /// </summary>
        private readonly string _pass;

        /// <summary>
        /// Creates a new api instance.
        /// </summary>
        public APIConnection(string url, string user, string pass)
        {
            _user = user;
            _pass = pass;

            this.Uri = new Uri(url);
        }

        /// <summary>
        /// The uri of the api.
        /// </summary>
        public Uri Uri
        {
            get;
            private set;
        }

        #region Http Connection

        /// <summary>
        /// Does an actual api call and returnes the webresponse.
        /// </summary>
        /// <param name="authenticate"></param>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private string DoApiCall(bool authenticate, string url, Method method, byte[] data)
        {
            WebRequest request = WebRequest.Create(new Uri(this.Uri.AbsoluteUri + url));
            this.SetBasicAuthHeader(request);
            HttpWebResponse response;
            Encoding enc;
            StreamReader responseStream;
            string responseString;
            Stream requestStream;
            switch (method)
            {
                case Method.PUT:
                    request.Method = "PUT";

                    // get the request stream and write the data.
                    requestStream = request.GetRequestStream();
                    requestStream.Write(data, 0, data.Length);
                    requestStream.Dispose();

                    // get the response.
                    response = (HttpWebResponse)request.GetResponse();
                    enc = System.Text.Encoding.GetEncoding("Windows-1252");
                    responseStream =
                       new StreamReader(response.GetResponseStream(), enc);
                    responseString = responseStream.ReadToEnd();

                    // close everything.
                    response.Dispose();
                    responseStream.Dispose();

                    return responseString;
                case Method.GET:
                    responseString = string.Empty;
                    try
                    {
                        request.Method = "GET";

                        // get the response.
                        response = (HttpWebResponse)request.GetResponse();
                        enc = System.Text.Encoding.GetEncoding("Windows-1252");
                        responseStream =
                           new StreamReader(response.GetResponseStream(), enc);
                        responseString = responseStream.ReadToEnd();

                        // close everything.
                        response.Dispose();
                        responseStream.Dispose();
                    }
                    catch (WebException ex)
                    {
                        if (ex.Response != null)
                        {
                            if (ex.Response is HttpWebResponse)
                            {
                                switch ((ex.Response as HttpWebResponse).StatusCode)
                                {
                                    case HttpStatusCode.NotFound:
                                        // the object never existed, return empty response.
                                        break;
                                    case HttpStatusCode.Gone:
                                        // the object is gone, return empty response.
                                        break;
                                    default:
                                        throw new APIException(string.Format("Unexpected API response: {0}",
                                            (ex.Response as HttpWebResponse).StatusCode.ToString()));
                                }
                            }
                        }
                    }

                    return responseString;
                case Method.DELETE:
                    request.Method = "DELETE";

                    // get the request stream and write the data.
                    requestStream = request.GetRequestStream();
                    requestStream.Write(data, 0, data.Length);
                    requestStream.Dispose();

                    // get the response.
                    response = (HttpWebResponse)request.GetResponse();
                    enc = System.Text.Encoding.GetEncoding("Windows-1252");
                    responseStream =
                       new StreamReader(response.GetResponseStream(), enc);
                    responseString = responseStream.ReadToEnd();

                    // close everything.
                    response.Dispose();
                    responseStream.Dispose();

                    return responseString;
                default:
                    throw new NotSupportedException(string.Format("Method {0} not supported!",
                        method.ToString()));
            }
        }

        /// <summary>
        /// The methods.
        /// </summary>
        private enum Method
        {
            PUT,
            GET,
            DELETE
        }

        /// <summary>
        /// Adds the correct authorisation header to the HTTP requests.
        /// </summary>
        /// <param name="req"></param>
        private void SetBasicAuthHeader(WebRequest req)
        {
            string authInfo = _user + ":" + _pass;
            authInfo = Convert.ToBase64String(Encoding.UTF8.GetBytes(authInfo));
            req.Headers["Authorization"] = "Basic " + authInfo;
        }

        #endregion

        #region Capabilities

        /// <summary>
        /// Holds the api capabilities.
        /// </summary>
        private APICapabilities _api_capabilities;

        /// <summary>
        /// Gets the API's capabilities.
        /// </summary>
        /// <returns></returns>
        public APICapabilities GetCapabilities()
        {
            if (_api_capabilities == null)
            { // get the capabilities from the api.
                string response = this.DoApiCall(false, "/api/capabilities", Method.GET, null);

                if (response != null && response.Trim().Length > 0)
                {
                    // de-serialize response.-
                    XmlReaderSource xml_source = new XmlReaderSource(XmlReader.Create(new StringReader(response)));
                    OsmDocument osm = new OsmDocument(xml_source);
                    Osm.Xml.v0_6.api api = (osm.Osm as OsmSharp.Osm.Xml.v0_6.osm).api;

                    // capabilities.
                    _api_capabilities = new APICapabilities();
                    if (api.area.maximumSpecified)
                    {
                        _api_capabilities.AreaMaximum = api.area.maximum;
                    }
                    if (api.changesets.maximum_elementsSpecified)
                    {
                        _api_capabilities.ChangeSetsMaximumElement = api.changesets.maximum_elements;
                    }
                    if (api.timeout.secondsSpecified)
                    {
                        _api_capabilities.TimeoutSeconds = api.timeout.seconds;
                    }
                    if (api.tracepoints.per_pageSpecified)
                    {
                        _api_capabilities.TracePointsPerPage = api.tracepoints.per_page;
                    }
                    if (api.version.maximumSpecified)
                    {
                        _api_capabilities.VersionMaximum = api.version.maximum;
                    }
                    if (api.version.minimumSpecified)
                    {
                        _api_capabilities.VersionMinimum = api.version.minimum;
                    }
                    if (api.waynodes.maximumSpecified)
                    {
                        _api_capabilities.WayNodesMaximum = api.waynodes.maximum;
                    }

                    return _api_capabilities;
                }
                return null;
            }
            return _api_capabilities;
        }

        #endregion

        /// <summary>
        /// Returns all objects within the given bounding box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public List<OsmGeo> BoundingBoxGet(GeoCoordinateBox box)
        {
            string response = this.DoApiCall(false, string.Format(
                "/api/0.6/map?bbox={0},{1},{2},{3}", 
                    box.MinLon.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    box.MinLat.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    box.MaxLon.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    box.MaxLat.ToString(System.Globalization.CultureInfo.InvariantCulture)), Method.GET, null);

            if (response != null && response.Trim().Length > 0)
            {
                XmlReaderSource xml_source = new XmlReaderSource(XmlReader.Create(new StringReader(response)));
                OsmDocument osm = new OsmDocument(xml_source);

                List<OsmGeo> box_objects = new List<OsmGeo>();
                OsmSharp.Osm.Xml.v0_6.osm xml_osm = (osm.Osm as OsmSharp.Osm.Xml.v0_6.osm);
                if (xml_osm.node != null)
                {
                    foreach (Osm.Xml.v0_6.node xml_node in xml_osm.node)
                    {
                        box_objects.Add(this.Convertv6XmlNode(xml_node));
                    }
                }
                if (xml_osm.way != null)
                {
                    foreach (Osm.Xml.v0_6.way xml_way in xml_osm.way)
                    {
                        box_objects.Add(this.Convertv6XmlWay(xml_way));
                    }
                }
                if (xml_osm.relation != null)
                {
                    foreach (Osm.Xml.v0_6.relation xml_relation in xml_osm.relation)
                    {
                        box_objects.Add(this.Convertv6XmlRelation(xml_relation));
                    }
                }
                return box_objects;
            }
            return null;
        }

        #region Changesets

        /// <summary>
        /// Holds the current changeset.
        /// </summary>
        private changeset _current_changeset;

        /// <summary>
        /// Holds the current changes.
        /// </summary>
        private osmChange _current_changes;

        /// <summary>
        /// Returns the current change.
        /// </summary>
        /// <returns></returns>
        private osmChange GetCurrentChange()
        {
            if (_current_changes == null)
            {
                _current_changes = new osmChange();
            }
            return _current_changes;
        }

        /// <summary>
        /// Gets the changeset information with the given id from the API. Does not include the actual changes.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ChangeSetInfo ChangeSetGet(long id)
        {
            string response = this.DoApiCall(false, string.Format(
                "/api/0.6/changeset/{0}", id), Method.GET, null);

            if (response != null && response.Trim().Length > 0)
            {
                XmlReaderSource xml_source = new XmlReaderSource(XmlReader.Create(new StringReader(response)));
                OsmDocument osm = new OsmDocument(xml_source);
                Osm.Xml.v0_6.changeset xml_node = (osm.Osm as OsmSharp.Osm.Xml.v0_6.osm).changeset[0];
                return this.Convertv6XmlChangeSet(xml_node);
            }
            return null;
        }

        /// <summary>
        /// Gets the changes in the changeset with the given id from the API.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ChangeSet ChangesGet(long id)
        {
            string response = this.DoApiCall(false, string.Format(
                "/api/0.6/changeset/{0}", id), Method.GET, null);

            if (response != null && response.Trim().Length > 0)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(osmChange));
                Osm.Xml.v0_6.osmChange xml_changeset = serializer.Deserialize(new StringReader(response)) as Osm.Xml.v0_6.osmChange;
                return this.Convertv6XmlChanges(xml_changeset);
            }
            return null;
        }

        /// <summary>
        /// Creates a new changeset by opening one.
        /// </summary>
        /// <returns></returns>
        public long ChangeSetOpen(string comment)
        {
            return this.ChangeSetOpen(comment, string.Format("OsmSharp v{0}",
                typeof(APIConnection).Assembly.GetName().Version.ToString(2)));
        }

        /// <summary>
        /// Creates a new changeset by opening one.
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="created_by"></param>
        /// <returns></returns>
        public long ChangeSetOpen(string comment, string created_by)
        {
            if (created_by == null || created_by.Length == 0)
            { // a created by tag always has to exist and have a usefull value.
                throw new ArgumentOutOfRangeException("A created by tag always has to exist and have a usefull value.");
            }

            // build a new changeset.
            OsmSharp.Osm.Xml.v0_6.osm osm = new Osm.Xml.v0_6.osm();
            osm.changeset = new Osm.Xml.v0_6.changeset[1];
            OsmSharp.Osm.Xml.v0_6.changeset changeset = new Osm.Xml.v0_6.changeset();
            changeset.tag = new OsmSharp.Osm.Xml.v0_6.tag[1];
            changeset.tag[0] = new Osm.Xml.v0_6.tag();
            changeset.tag[0].k = "created_by";
            changeset.tag[0].v = created_by;
            osm.changeset[0] = changeset;

            // serialize the changeset.            
            XmlSerializer serializer = new XmlSerializer(typeof(OsmSharp.Osm.Xml.v0_6.osm));
            MemoryStream mem_stream = new MemoryStream();
            Stream stream = mem_stream;
            serializer.Serialize(stream, osm);
            stream.Flush();
            mem_stream.Flush();
            byte[] put_data = mem_stream.ToArray();

            // do the api call.
            string response_string = this.DoApiCall(true, "api/0.6/changeset/create", Method.PUT, put_data);

            _current_changeset = changeset;
            _current_changeset.id = long.Parse(response_string);
            return _current_changeset.id;
        }

        /// <summary>
        /// Close the current changeset on the API.
        /// </summary>
        /// <returns></returns>
        public void ChangeSetClose()
        {
            this.DoApiCall(true, string.Format("api/0.6/changeset/{0}/close", _current_changeset.id), Method.PUT, new byte[0]);
        }

        #endregion

        #region Creation/Modification/Deletion

        #region Node

        /// <summary>
        /// Creates a new node by adding it's creation to the current changeset.
        /// </summary>
        /// <param name="node"></param>
        public Node NodeCreate(Node node)
        {
            if (_current_changeset == null)
            {
                throw new InvalidOperationException("No open changeset found!");
            }

            // build a new node.
            node xml_node = node.ConvertTo();
            xml_node.changeset = _current_changeset.id;
            xml_node.changesetSpecified = true;

            // encapsulate into an osm object.
            OsmSharp.Osm.Xml.v0_6.osm osm = new Osm.Xml.v0_6.osm();
            osm.node = new Osm.Xml.v0_6.node[1];
            osm.node[0] = xml_node;

            // serialize the changeset.            
            XmlSerializer serializer = new XmlSerializer(typeof(OsmSharp.Osm.Xml.v0_6.osm));
            MemoryStream mem_stream = new MemoryStream();
            Stream stream = mem_stream;
            serializer.Serialize(stream, osm);
            stream.Flush();
            mem_stream.Flush();
            byte[] put_data = mem_stream.ToArray();

            // do the api call.
            string response_string = this.DoApiCall(true, "api/0.6/node/create",
                Method.PUT, put_data);

            // get the id-response.
            long id;
            if (!long.TryParse(response_string, out id))
            { // invalid response!
                throw new APIException(string.Format("Invalid response when creating a new node: {0}", response_string));
            }
            node.Id = id;
            return node;
        }

        /// <summary>
        /// Modifies a the given node by adding it's modification to the current changeset.
        /// </summary>
        /// <param name="node"></param>
        public void NodeUpdate(Node node)
        {
            if (_current_changeset == null)
            {
                throw new InvalidOperationException("No open changeset found!");
            }
            if (!node.Id.HasValue)
            {
                throw new ArgumentOutOfRangeException("Cannot update an object without an id!");
            }

            // build a new node.
            node xml_node = node.ConvertTo();
            xml_node.changeset = _current_changeset.id;
            xml_node.changesetSpecified = true;

            // encapsulate into an osm object.
            OsmSharp.Osm.Xml.v0_6.osm osm = new Osm.Xml.v0_6.osm();
            osm.node = new Osm.Xml.v0_6.node[1];
            osm.node[0] = xml_node;

            // serialize the changeset.            
            XmlSerializer serializer = new XmlSerializer(typeof(OsmSharp.Osm.Xml.v0_6.osm));
            MemoryStream mem_stream = new MemoryStream();
            Stream stream = mem_stream;
            serializer.Serialize(stream, osm);
            stream.Flush();
            mem_stream.Flush();
            byte[] put_data = mem_stream.ToArray();

            // do the api call.
            this.DoApiCall(true, string.Format("api/0.6/node/{0}", node.Id.Value),
                Method.PUT, put_data);
        }

        /// <summary>
        /// Deletes the given node by adding it's deletion to the current changeset.
        /// </summary>
        /// <param name="node"></param>
        public void NodeDelete(Node node)
        {
            if (_current_changeset == null)
            {
                throw new InvalidOperationException("No open changeset found!");
            }
            if (!node.Id.HasValue)
            {
                throw new ArgumentOutOfRangeException("Cannot delete an object without an id!");
            }

            // build a new node.
            node xml_node = node.ConvertTo();
            xml_node.changeset = _current_changeset.id;
            xml_node.changesetSpecified = true;

            // encapsulate into an osm object.
            OsmSharp.Osm.Xml.v0_6.osm osm = new Osm.Xml.v0_6.osm();
            osm.node = new Osm.Xml.v0_6.node[1];
            osm.node[0] = xml_node;

            // serialize the changeset.            
            XmlSerializer serializer = new XmlSerializer(typeof(OsmSharp.Osm.Xml.v0_6.osm));
            MemoryStream mem_stream = new MemoryStream();
            Stream stream = mem_stream;
            serializer.Serialize(stream, osm);
            stream.Flush();
            mem_stream.Flush();
            byte[] put_data = mem_stream.ToArray();

            // do the api call.
            this.DoApiCall(true, string.Format("api/0.6/node/{0}", node.Id.Value),
                Method.DELETE, put_data);
        }

        /// <summary>
        /// Returns the node with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Node NodeGet(long id)
        {
            string response = this.DoApiCall(false, string.Format(
                "/api/0.6/node/{0}", id), Method.GET, null);

            if (response != null && response.Trim().Length > 0)
            {
                XmlReaderSource xml_source = new XmlReaderSource(XmlReader.Create(new StringReader(response)));
                OsmDocument osm = new OsmDocument(xml_source);
                Osm.Xml.v0_6.node xml_node = (osm.Osm as OsmSharp.Osm.Xml.v0_6.osm).node[0];
                return this.Convertv6XmlNode(xml_node);
            }
            return null;
        }

        /// <summary>
        /// Converts an API v6 xml changeset object to a SimpleChange object.
        /// </summary>
        /// <param name="xml_changeset"></param>
        /// <returns></returns>
        private ChangeSetInfo Convertv6XmlChangeSet(OsmSharp.Osm.Xml.v0_6.changeset xml_changeset)
        {
            ChangeSetInfo simple_changeset = new ChangeSetInfo();
            if (xml_changeset.idSpecified)
            {
                simple_changeset.Id = xml_changeset.id;
            }
            if (xml_changeset.closed_atSpecified)
            {
                simple_changeset.ClosedAt = xml_changeset.closed_at;
            }
            if (xml_changeset.closed_atSpecified)
            {
                simple_changeset.CreatedAt = xml_changeset.created_at;
            }
            if (xml_changeset.max_latSpecified)
            {
                simple_changeset.MaxLat = xml_changeset.max_lat;
            }
            if (xml_changeset.max_lonSpecified)
            {
                simple_changeset.MaxLon = xml_changeset.max_lon;
            }
            if (xml_changeset.min_latSpecified)
            {
                simple_changeset.MinLat = xml_changeset.min_lat;
            }
            if (xml_changeset.min_lonSpecified)
            {
                simple_changeset.MinLon = xml_changeset.min_lon;
            }
            if (xml_changeset.openSpecified)
            {
                simple_changeset.Open = xml_changeset.open;
            }
            if (xml_changeset.tag != null)
            {
                simple_changeset.Tags = new Dictionary<string, string>();
                foreach (Osm.Xml.v0_6.tag xml_tag in xml_changeset.tag)
                {
                    simple_changeset.Tags.Add(xml_tag.k, xml_tag.v);
                }
            }
            if (xml_changeset.uidSpecified)
            {
                simple_changeset.UserId = xml_changeset.uid;
            }
            simple_changeset.User = xml_changeset.user;
            return simple_changeset;
        }

        /// <summary>
        /// Converts an API v6 xml osmChange object to a SimpleChange object.
        /// </summary>
        /// <param name="osm_change"></param>
        /// <returns></returns>
        private ChangeSet Convertv6XmlChanges(OsmSharp.Osm.Xml.v0_6.osmChange osm_change)
        {
            List<Change> changes = new List<Change>();

            if (osm_change.create != null)
            {
                for (int idx = 0; idx < osm_change.create.Length; idx++)
                {
                    OsmSharp.Osm.Xml.v0_6.create create = osm_change.create[idx];

                    List<OsmGeo> changed_objects = new List<OsmGeo>();

                    if (create.node != null)
                    { // change represents a change in a node.
                        for (int node_idx = 0; node_idx < create.node.Length; node_idx++)
                        {
                            changed_objects.Add(this.Convertv6XmlNode(create.node[node_idx]));
                        }
                    }
                    if (create.way != null)
                    { // change represents a change in a way.
                        for (int way_idx = 0; way_idx < create.way.Length; way_idx++)
                        {
                            changed_objects.Add(this.Convertv6XmlWay(create.way[way_idx]));
                        }
                    }
                    if (create.relation != null)
                    { // change represents a change in a relation.
                        for (int relation_idx = 0; relation_idx < create.relation.Length; relation_idx++)
                        {
                            changed_objects.Add(this.Convertv6XmlRelation(create.relation[relation_idx]));
                        }
                    }

                    if (changed_objects.Count > 0)
                    { // there are actually changed objects.
                        changes.Add(new OsmSharp.Osm.Change()
                        {
                            OsmGeo = changed_objects,
                            Type = OsmSharp.Osm.ChangeType.Create
                        });
                    }
                }
            }

            ChangeSet simple_change_set = new ChangeSet();
            simple_change_set.Changes = changes;

            return simple_change_set;
        }

        /// <summary>
        /// Converts an API v6 xml node to a SimpleNode object.
        /// </summary>
        /// <param name="xml_node"></param>
        /// <returns></returns>
        private Node Convertv6XmlNode(Osm.Xml.v0_6.node xml_node)
        {
            Node node = new Node();
            node.Id = xml_node.id;
            node.Latitude = xml_node.lat;
            node.Longitude = xml_node.lon;

            if (xml_node.tag != null)
            {
                node.Tags = new TagsCollection();
                foreach (Osm.Xml.v0_6.tag xml_tag in xml_node.tag)
                {
                    node.Tags.Add(xml_tag.k, xml_tag.v);
                }
            }

            node.ChangeSetId = xml_node.changeset;
            node.TimeStamp = xml_node.timestamp;
            node.UserName = xml_node.user;
            node.UserId = xml_node.uid;
            node.Version = xml_node.version;
            node.Visible = xml_node.visible;

            return node;
        }

        /// <summary>
        /// Converts an API v6 xml node to a SimpleWay object.
        /// </summary>
        /// <param name="xml_way"></param>
        /// <returns></returns>
        private Way Convertv6XmlWay(way xml_way)
        {
            Way way = new Way();
            way.Id = xml_way.id;

            if (xml_way.tag != null)
            {
                way.Tags = new TagsCollection();
                foreach (Osm.Xml.v0_6.tag xml_tag in xml_way.tag)
                {
                    way.Tags.Add(xml_tag.k, xml_tag.v);
                }
            }

            if (xml_way.nd != null)
            {
                way.Nodes = new List<long>();
                foreach (Osm.Xml.v0_6.nd xml_nd in xml_way.nd)
                {
                    way.Nodes.Add(xml_nd.@ref);
                }
            }

            way.ChangeSetId = xml_way.changeset;
            way.TimeStamp = xml_way.timestamp;
            way.UserName = xml_way.user;
            way.UserId = xml_way.uid;
            way.Version = xml_way.version;
            way.Visible = xml_way.visible;

            return way;
        }

        /// <summary>
        /// Converts an API v6 xml node to a SimpleRelation object.
        /// </summary>
        /// <param name="xml_relation"></param>
        /// <returns></returns>
        private Relation Convertv6XmlRelation(relation xml_relation)
        {
            Relation relation = new Relation();
            relation.Id = xml_relation.id;

            if (xml_relation.tag != null)
            {
                relation.Tags = new TagsCollection();
                foreach (Osm.Xml.v0_6.tag xml_tag in xml_relation.tag)
                {
                    relation.Tags.Add(xml_tag.k, xml_tag.v);
                }
            }

            if (xml_relation.member != null)
            {
                relation.Members = new List<RelationMember>();
                foreach (Osm.Xml.v0_6.member xml_member in xml_relation.member)
                {
                    OsmGeoType? member_type = null;
                    switch (xml_member.type)
                    {
                        case memberType.node:
                            member_type = OsmGeoType.Node;
                            break;
                        case memberType.way:
                            member_type = OsmGeoType.Way;
                            break;
                        case memberType.relation:
                            member_type = OsmGeoType.Relation;
                            break;
                    }

                    relation.Members.Add(new RelationMember()
                    {
                        MemberId = xml_member.@ref,
                        MemberRole = xml_member.role,
                        MemberType = member_type
                    });
                }
            }

            relation.ChangeSetId = xml_relation.changeset;
            relation.TimeStamp = xml_relation.timestamp;
            relation.UserName = xml_relation.user;
            relation.UserId = xml_relation.uid;
            relation.Version = xml_relation.version;
            relation.Visible = xml_relation.visible;

            return relation;
        }

        #endregion

        #region Way

        /// <summary>
        /// Creates a new way by adding it to the current changeset.
        /// </summary>
        /// <param name="way"></param>
        public Way WayCreate(Way way)
        {
            if (_current_changeset == null)
            {
                throw new InvalidOperationException("No open changeset found!");
            }

            // build a new node.
            way xml_way = way.ConvertTo();
            xml_way.changeset = _current_changeset.id;
            xml_way.changesetSpecified = true;

            // encapsulate into an osm object.
            OsmSharp.Osm.Xml.v0_6.osm osm = new Osm.Xml.v0_6.osm();
            osm.way = new Osm.Xml.v0_6.way[1];
            osm.way[0] = xml_way;

            // serialize the changeset.            
            XmlSerializer serializer = new XmlSerializer(typeof(OsmSharp.Osm.Xml.v0_6.osm));
            MemoryStream mem_stream = new MemoryStream();
            Stream stream = mem_stream;
            serializer.Serialize(stream, osm);
            stream.Flush();
            mem_stream.Flush();
            byte[] put_data = mem_stream.ToArray();

            // do the api call.
            string response_string = this.DoApiCall(true, "api/0.6/way/create",
                Method.PUT, put_data);

            // get the id-response.
            long id;
            if (!long.TryParse(response_string, out id))
            { // invalid response!
                throw new APIException(string.Format("Invalid response when creating a new way: {0}", response_string));
            }
            way.Id = id;
            return way;
        }

        /// <summary>
        /// Updates the given way by adding it's changes to the current changeset.
        /// </summary>
        /// <param name="way"></param>
        public void WayUpdate(Way way)
        {
            if (_current_changeset == null)
            {
                throw new InvalidOperationException("No open changeset found!");
            }
            if (!way.Id.HasValue)
            {
                throw new ArgumentOutOfRangeException("Cannot update an object without an id!");
            }

            // build a new node.
            way xml_way = way.ConvertTo();
            xml_way.changeset = _current_changeset.id;
            xml_way.changesetSpecified = true;

            // encapsulate into an osm object.
            OsmSharp.Osm.Xml.v0_6.osm osm = new Osm.Xml.v0_6.osm();
            osm.way = new Osm.Xml.v0_6.way[1];
            osm.way[0] = xml_way;

            // serialize the changeset.            
            XmlSerializer serializer = new XmlSerializer(typeof(OsmSharp.Osm.Xml.v0_6.osm));
            MemoryStream mem_stream = new MemoryStream();
            Stream stream = mem_stream;
            serializer.Serialize(stream, osm);
            stream.Flush();
            mem_stream.Flush();
            byte[] put_data = mem_stream.ToArray();

            // do the api call.
            this.DoApiCall(true, string.Format("api/0.6/way/{0}", way.Id.Value),
                Method.PUT, put_data);
        }

        /// <summary>
        /// Updates the given way.
        /// </summary>
        /// <param name="way"></param>
        public void WayDelete(Way way)
        {
            if (_current_changeset == null)
            {
                throw new InvalidOperationException("No open changeset found!");
            }
            if (!way.Id.HasValue)
            {
                throw new ArgumentOutOfRangeException("Cannot update an object without an id!");
            }

            // build a new node.
            way xml_way = way.ConvertTo();
            xml_way.changeset = _current_changeset.id;
            xml_way.changesetSpecified = true;

            // encapsulate into an osm object.
            OsmSharp.Osm.Xml.v0_6.osm osm = new Osm.Xml.v0_6.osm();
            osm.way = new Osm.Xml.v0_6.way[1];
            osm.way[0] = xml_way;

            // serialize the changeset.            
            XmlSerializer serializer = new XmlSerializer(typeof(OsmSharp.Osm.Xml.v0_6.osm));
            MemoryStream mem_stream = new MemoryStream();
            Stream stream = mem_stream;
            serializer.Serialize(stream, osm);
            stream.Flush();
            mem_stream.Flush();
            byte[] put_data = mem_stream.ToArray();

            // do the api call.
            this.DoApiCall(true, string.Format("api/0.6/way/{0}", way.Id.Value),
                Method.DELETE, put_data);
        }

        /// <summary>
        /// Returns the way with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Way WayGet(long id)
        {
            string response = this.DoApiCall(false, string.Format(
                "/api/0.6/way/{0}", id), Method.GET, null);

            if (response != null && response.Trim().Length > 0)
            {
                XmlReaderSource xml_source = new XmlReaderSource(XmlReader.Create(new StringReader(response)));
                OsmDocument osm = new OsmDocument(xml_source);
                Osm.Xml.v0_6.way xml_way = (osm.Osm as OsmSharp.Osm.Xml.v0_6.osm).way[0];
                return this.Convertv6XmlWay(xml_way);
            }
            return null;
        }

        #endregion

        #region Relation

        /// <summary>
        /// Returns the relation with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Relation RelationGet(long id)
        {
            string response = this.DoApiCall(false, string.Format(
                "/api/0.6/relation/{0}", id), Method.GET, null);

            if (response != null && response.Trim().Length > 0)
            {
                XmlReaderSource xml_source = new XmlReaderSource(XmlReader.Create(new StringReader(response)));
                OsmDocument osm = new OsmDocument(xml_source);
                Osm.Xml.v0_6.relation xml_relation = (osm.Osm as OsmSharp.Osm.Xml.v0_6.osm).relation[0];
                return this.Convertv6XmlRelation(xml_relation);
            }
            return null;
        }

        /// <summary>
        /// Creates a new relation.
        /// </summary>
        /// <param name="relation"></param>
        /// <returns></returns>
        public Relation RelationCreate(Relation relation)
        {
            if (_current_changeset == null)
            {
                throw new InvalidOperationException("No open changeset found!");
            }

            // build a new node.
            relation xml_relation = relation.ConvertTo();
            xml_relation.changeset = _current_changeset.id;
            xml_relation.changesetSpecified = true;

            // encapsulate into an osm object.
            OsmSharp.Osm.Xml.v0_6.osm osm = new Osm.Xml.v0_6.osm();
            osm.relation = new Osm.Xml.v0_6.relation[1];
            osm.relation[0] = xml_relation;

            // serialize the changeset.            
            XmlSerializer serializer = new XmlSerializer(typeof(OsmSharp.Osm.Xml.v0_6.osm));
            MemoryStream mem_stream = new MemoryStream();
            Stream stream = mem_stream;
            serializer.Serialize(stream, osm);
            stream.Flush();
            mem_stream.Flush();
            byte[] put_data = mem_stream.ToArray();

            // do the api call.
            string response_string = this.DoApiCall(true, "api/0.6/relation/create",
                Method.PUT, put_data);

            // get the id-response.
            long id;
            if (!long.TryParse(response_string, out id))
            { // invalid response!
                throw new APIException(string.Format("Invalid response when creating a new relation: {0}", response_string));
            }
            relation.Id = id;
            return relation;
        }

        /// <summary>
        /// Updates the given relation by adding it's changes to the current changeset.
        /// </summary>
        /// <param name="relation"></param>
        public void RelationUpdate(Relation relation)
        {
            if (_current_changeset == null)
            {
                throw new InvalidOperationException("No open changeset found!");
            }
            if (!relation.Id.HasValue)
            {
                throw new ArgumentOutOfRangeException("Cannot update an object without an id!");
            }

            // build a new node.
            relation xml_relation = relation.ConvertTo();
            xml_relation.changeset = _current_changeset.id;
            xml_relation.changesetSpecified = true;

            // encapsulate into an osm object.
            OsmSharp.Osm.Xml.v0_6.osm osm = new Osm.Xml.v0_6.osm();
            osm.relation = new Osm.Xml.v0_6.relation[1];
            osm.relation[0] = xml_relation;

            // serialize the changeset.            
            XmlSerializer serializer = new XmlSerializer(typeof(OsmSharp.Osm.Xml.v0_6.osm));
            MemoryStream mem_stream = new MemoryStream();
            Stream stream = mem_stream;
            serializer.Serialize(stream, osm);
            stream.Flush();
            mem_stream.Flush();
            byte[] put_data = mem_stream.ToArray();

            // do the api call.
            this.DoApiCall(true, string.Format("api/0.6/relation/{0}", relation.Id.Value),
                Method.PUT, put_data);
        }

        /// <summary>
        /// Deletes the given relation by adding it's deletion to the current changeset.
        /// </summary>
        /// <param name="relation"></param>
        public void RelationDelete(Relation relation)
        {
            if (_current_changeset == null)
            {
                throw new InvalidOperationException("No open changeset found!");
            }
            if (!relation.Id.HasValue)
            {
                throw new ArgumentOutOfRangeException("Cannot update an object without an id!");
            }

            // build a new node.
            relation xml_relation = relation.ConvertTo();
            xml_relation.changeset = _current_changeset.id;
            xml_relation.changesetSpecified = true;

            // encapsulate into an osm object.
            OsmSharp.Osm.Xml.v0_6.osm osm = new Osm.Xml.v0_6.osm();
            osm.relation = new Osm.Xml.v0_6.relation[1];
            osm.relation[0] = xml_relation;

            // serialize the changeset.            
            XmlSerializer serializer = new XmlSerializer(typeof(OsmSharp.Osm.Xml.v0_6.osm));
            MemoryStream mem_stream = new MemoryStream();
            Stream stream = mem_stream;
            serializer.Serialize(stream, osm);
            stream.Flush();
            mem_stream.Flush();
            byte[] put_data = mem_stream.ToArray();

            // do the api call.
            this.DoApiCall(true, string.Format("api/0.6/relation/{0}", relation.Id.Value),
                Method.DELETE, put_data);
        }

        #endregion

        #endregion
    }
}