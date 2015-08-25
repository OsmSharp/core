//// OsmSharp - OpenStreetMap (OSM) SDK
//// Copyright (C) 2013 Abelshausen Ben
//// 
//// This file is part of OsmSharp.
//// 
//// OsmSharp is free software: you can redistribute it and/or modify
//// it under the terms of the GNU General Public License as published by
//// the Free Software Foundation, either version 2 of the License, or
//// (at your option) any later version.
//// 
//// OsmSharp is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//// GNU General Public License for more details.
//// 
//// You should have received a copy of the GNU General Public License
//// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using OsmSharp.Osm.Xml;
//using OsmSharp.Math.Geo;
//using OsmSharp.Osm.Xml.v0_6;
//using OsmSharp.Osm.Sources;
//using OsmSharp.Osm.Filters;
//using System.Xml;
//using System.IO;

//namespace OsmSharp.Osm.Xml.OsmSource
//{
//    /// <summary>
//    /// An osm data source for an xml stream.
//    /// </summary>
//    public class OsmDataSource : IDataSource,
//        INodeSource,
//        IWaySource,
//        IRelationSource
//    {
//        /// <summary>
//        /// The osm document this datasource is for.
//        /// </summary>
//        private OsmDocument _document;

//        /// <summary>
//        /// The id of this data source.
//        /// </summary>
//        private Guid _id;

//        /// <summary>
//        /// Creates a new osm data source.
//        /// </summary>
//        /// <param name="document"></param>
//        public OsmDataSource(OsmDocument document)
//        {
//            _document = document;
//            _id = Guid.NewGuid();

//            _read = false;
//            _nodes = new Dictionary<long, Node>();
//            _ways = new Dictionary<long, Way>();
//            _relations = new Dictionary<long, Relation>();

//            _ways_per_node = new Dictionary<long, List<long>>();
//            _relations_per_member = new Dictionary<long, List<long>>();
//            _closed_change_set = new List<long>();
//        }

//        /// <summary>
//        /// Creates a new osm data source.
//        /// </summary>
//        public OsmDataSource(string file)
//            : this(new OsmSharp.Osm.Xml.OsmDocument(new OsmSharp.Xml.Sources.XmlFileSource(file)))
//        {

//        }

//        /// <summary>
//        /// Creates a new osm data source.
//        /// </summary>
//        /// <param name="stream"></param>
//        public OsmDataSource(Stream stream)
//            : this(new OsmSharp.Osm.Xml.OsmDocument(
//                new OsmSharp.Xml.Sources.XmlReaderSource(XmlReader.Create(stream))))
//        {
            
//        }

//        #region Write/Read functions

//        // hold all node, ways, relations and changesets and their bounding box.
//        private IDictionary<long, Node> _nodes;
//        private IDictionary<long, Way> _ways;
//        private IDictionary<long, Relation> _relations;
//        private IList<long> _closed_change_set;
//        private IDictionary<long, List<long>> _ways_per_node;
//        private IDictionary<long, List<long>> _relations_per_member;
//        private GeoCoordinateBox _bb;

//        private bool _read;

//        /// <summary>
//        /// Adds the node-way relations for the given way.
//        /// </summary>
//        /// <param name="way"></param>
//        private void RegisterNodeWayRelation(Way way)
//        {
//            foreach (Node node in way.Nodes)
//            {
//                if (!_ways_per_node.ContainsKey(node.Id))
//                {
//                    _ways_per_node.Add(node.Id, new List<long>());
//                }
//                _ways_per_node[node.Id].Add(way.Id);
//            }
//        }

//        /// <summary>
//        /// Adds the member-relation for the given relation.
//        /// </summary>
//        /// <param name="relation"></param>
//        private void RegisterRelationMemberRelation(Relation relation)
//        {
//            foreach (RelationMember member in relation.Members)
//            {
//                if (!_relations_per_member.ContainsKey(member.Member.Id))
//                {
//                    _relations_per_member.Add(member.Member.Id, new List<long>());
//                }
//                _relations_per_member[member.Member.Id].Add(relation.Id);
//            }
//        }

//        /// <summary>
//        /// Registers a closed change set id.
//        /// </summary>
//        /// <param name="change_set_id"></param>
//        private void RegisterChangeSetId(long? change_set_id)
//        {
//            if (change_set_id.HasValue
//                && !_closed_change_set.Contains(change_set_id.Value))
//            {
//                _closed_change_set.Add(change_set_id.Value);
//            }
//        }

//        /// <summary>
//        /// Reads all the data from the osm document if needed.
//        /// </summary>
//        private void ReadFromDocument()
//        {
//            if (!_read)
//            {
//                _read = true;

//                OsmSharp.Osm.Xml.v0_6.osm osm = (_document.Osm as OsmSharp.Osm.Xml.v0_6.osm);

//                if (osm != null)
//                { // if there was no data to begin with.
//                    if (osm.node != null)
//                    {
//                        foreach (Osm.Xml.v0_6.node node in osm.node)
//                        {
//                            Node new_node = node.ConvertFrom();
//                            _nodes.Add(new_node.Id, new_node);

//                            this.RegisterChangeSetId(new_node.ChangeSetId);
//                        }
//                    }

//                    if (osm.way != null)
//                    {
//                        foreach (Osm.Xml.v0_6.way way in osm.way)
//                        {
//                            Way new_way = way.ConvertFrom(this);
//                            if (new_way != null)
//                            {
//                                _ways.Add(new_way.Id, new_way);

//                                this.RegisterNodeWayRelation(new_way);
//                                this.RegisterChangeSetId(new_way.ChangeSetId);
//                            }
//                        }
//                    }

//                    if (osm.relation != null)
//                    {
//                        foreach (Osm.Xml.v0_6.relation relation in osm.relation)
//                        {
//                            Relation new_relation = relation.ConvertFrom(this, this, this);
//                            if (new_relation != null)
//                            {
//                                _relations.Add(new_relation.Id, new_relation);

//                                this.RegisterRelationMemberRelation(new_relation);
//                                this.RegisterChangeSetId(new_relation.ChangeSetId);
//                            }
//                        }
//                    }

//                    _bb = osm.bounds.ConvertFrom();
//                    if(_bb == null)
//                    {
//                        _bb = osm.bound.ConvertFrom();
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// Writes to the osm document.
//        /// </summary>
//        private void WriteToDocument()
//        {
//            _read = true;

//            // collect all needed data.
//            _bb = this.BoundingBox;

//            // generate osm document.
//            OsmSharp.Osm.Xml.v0_6.osm osm = new OsmSharp.Osm.Xml.v0_6.osm();

//            // dimension the arrays.
//            osm.node = new node[_nodes.Count];
//            osm.way = new way[_ways.Count];
//            osm.relation = new relation[_relations.Count];

//            // iterate over all objects and convert them.
//            IList<Node> nodes = _nodes.Values.ToList<Node>();
//            for(int idx = 0;idx < nodes.Count;idx++)
//            {
//                node xml_obj = nodes[idx].ConvertTo();
//                osm.node[idx] = xml_obj;
//            }
//            IList<Way> ways = _ways.Values.ToList<Way>();
//            for (int idx = 0; idx < ways.Count; idx++)
//            {
//                way xml_obj = ways[idx].ConvertTo();
//                osm.way[idx] = xml_obj;
//            }
//            IList<Relation> relations = _relations.Values.ToList<Relation>();
//            for (int idx = 0; idx < relations.Count; idx++)
//            {
//                relation xml_obj = relations[idx].ConvertTo();
//                osm.relation[idx] = xml_obj;
//            }

//            // convert the bounds as well.
//            osm.bounds = _bb.ConvertTo();
            
//            _document.Osm = osm;
//            _document.Save();
//        }


//        #endregion

//        #region IDataSource Members

//        /// <summary>
//        /// Gets the bounding box around the data in this data source.
//        /// </summary>
//        public GeoCoordinateBox BoundingBox
//        {
//            get 
//            {
//                this.ReadFromDocument();

//                if (_bb == null)
//                { // calculate bounding box.
//                    throw new NotSupportedException("There is no boundingbox in this datafile!");
//                }
//                return _bb;
//            }
//        }

//        /// <summary>
//        /// Returns the id of this data source.
//        /// </summary>
//        public Guid Id
//        {
//            get 
//            {
//                return _id;
//            }
//        }

//        /// <summary>
//        /// Returns true; a bounding box can always be calculated.
//        /// </summary>
//        public bool HasBoundinBox
//        {
//            get 
//            {
//                this.ReadFromDocument();

//                return _bb != null;
//            }
//        }

//        /// <summary>
//        /// Returns false; this source cannot generate native id's.
//        /// </summary>
//        public bool IsBaseIdGenerator
//        {
//            get 
//            {
//                return false;
//            }
//        }

//        /// <summary>
//        /// Returns true if this source can create new objects.
//        /// </summary>
//        public bool IsCreator
//        {
//            get 
//            {
//                return true;
//            }
//        }

//        /// <summary>
//        /// Returns true if this source is readonly.
//        /// </summary>
//        public bool IsReadOnly
//        {
//            get 
//            {
//                return _document.IsReadOnly;
//            }
//        }

//        /// <summary>
//        /// Returns false; data is not persisted live.
//        /// </summary>
//        public bool IsLive
//        {
//            get
//            {
//                return false;
//            }
//        }

//        /// <summary>
//        /// Persists all the data in this source.
//        /// </summary>
//        public void Persist()
//        {
//            if (!this.IsReadOnly)
//            { // persist the data.
//                // generate and write to the xml document.
//                this.WriteToDocument();
//            }
//            else
//            {
//                throw new InvalidOperationException("Cannot persist a readonly datasource!");
//            }
//        }

//        /// <summary>
//        /// Applies the given changeset to the data in this datasource.
//        /// </summary>
//        /// <param name="changeSet"></param>
//        public void ApplyChangeSet(ChangeSet changeSet)
//        {
//            // test if the changeset was not already applied.
//            if (_closed_change_set.Contains(changeSet.Id))
//            {
//                throw new InvalidOperationException("Cannot apply an already closed changeset!");
//            }

//            // change the objects in the changeset.
//            foreach (Change change in changeSet.Changes)
//            {
//                switch (change.Type)
//                {
//                    case ChangeType.Create:
//                        // set the changeset and version field.
//                        change.Object.ChangeSetId = changeSet.Id;
//                        change.Object.Version = 0;

//                        switch (change.Object.Type)
//                        {
//                            case OsmType.Node:
//                                this.AddNode(change.Object as Node);
//                                break;
//                            case OsmType.Relation:
//                                this.AddRelation(change.Object as Relation);
//                                break;
//                            case OsmType.Way:
//                                this.AddWay(change.Object as Way);
//                                break;
//                        }
//                        break;
//                    case ChangeType.Delete:

//                        switch (change.Object.Type)
//                        {
//                            case OsmType.Node:
//                                this.RemoveNode(change.Object as Node);
//                                break;
//                            case OsmType.Relation:
//                                this.RemoveRelation(change.Object as Relation);
//                                break;
//                            case OsmType.Way:
//                                this.RemoveWay(change.Object as Way);
//                                break;
//                        }
//                        this.RegisterChangeSetId(changeSet.Id);
//                        break;
//                    case ChangeType.Modify:

//                        // update the changeset field and the version field.
//                        change.Object.ChangeSetId = changeSet.Id;
//                        if (change.Object.Version.HasValue)
//                        {
//                            change.Object.Version =  change.Object.Version.Value + 1;
//                        }

//                        switch (change.Object.Type)
//                        {
//                            case OsmType.Node:
//                                _nodes[change.Object.Id] = (change.Object as Node);
//                                break;
//                            case OsmType.Relation:
//                                _relations[change.Object.Id] = (change.Object as Relation);

//                                // update the relations data.
//                                IList<long> old_members_to_remove = new List<long>();
//                                foreach (KeyValuePair<long, List<long>> pair in _relations_per_member)
//                                {
//                                    if (pair.Value.Contains(change.Object.Id))
//                                    {
//                                        pair.Value.Remove(change.Object.Id);
//                                        // remove the old members that are only used in this relation.
//                                        if (pair.Value.Count == 0)
//                                        {
//                                            old_members_to_remove.Add(pair.Key);
//                                        }
//                                    }
//                                }
//                                foreach (int old_member in old_members_to_remove)
//                                {
//                                    _relations_per_member.Remove(old_member);
//                                }

//                                // re-index the relation.
//                                this.RegisterRelationMemberRelation(change.Object as Relation);
//                                break;
//                            case OsmType.Way:
//                                _ways[change.Object.Id] = (change.Object as Way);

//                                // update the way-node relation data.
//                                IList<long> old_nodes_to_remove = new List<long>();
//                                foreach (KeyValuePair<long, List<long>> pair in _ways_per_node)
//                                {
//                                    if (pair.Value.Contains(change.Object.Id))
//                                    {
//                                        pair.Value.Remove(change.Object.Id);

//                                        // remove the old nodes that are only used in this way.
//                                        if (pair.Value.Count == 0)
//                                        {
//                                            old_nodes_to_remove.Add(pair.Key);
//                                        }
//                                    }
//                                }
//                                foreach (long old_node in old_nodes_to_remove)
//                                {
//                                    _ways_per_node.Remove(old_node);
//                                }

//                                // re-index the relation.
//                                this.RegisterNodeWayRelation(change.Object as Way);

//                                // remove unused nodes.
//                                // => nodes that are not re-indexed!
//                                foreach (long old_node in old_nodes_to_remove)
//                                {
//                                    if (!_ways_per_node.ContainsKey(old_node))
//                                    {
//                                        this.RemoveNode(this.GetNode(old_node));
//                                    }
//                                    _ways_per_node.Remove(old_node);
//                                }
//                                break;
//                        }
//                        this.RegisterChangeSetId(changeSet.Id);
//                        break;
//                }
//            }
//        }

//        /// <summary>
//        /// Creates a new node.
//        /// </summary>
//        /// <returns></returns>
//        public Node CreateNode()
//        {
//            return Node.Create(KeyGenerator.GenerateNew());
//        }

//        /// <summary>
//        /// Returns the node with the given id.
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        public Node GetNode(long id)
//        {
//            this.ReadFromDocument();

//            if (_nodes.ContainsKey(id))
//            {
//                return _nodes[id];
//            }
//            return null;
//        }

//        /// <summary>
//        /// Returns an enumerable of all nodes in the data source.
//        /// </summary>
//        /// <returns></returns>
//        public IEnumerable<Node> GetNodes()
//        {
//            this.ReadFromDocument();

//            return _nodes.Values;
//        }

//        /// <summary>
//        /// Returns the nodes with the id's in the ids list.
//        /// 
//        /// The returned list will have the same size as the original
//        /// and the returned nodes will be in the same position as their id's.
//        /// </summary>
//        /// <param name="ids"></param>
//        /// <returns></returns>
//        public IList<Node> GetNodes(IList<long> ids)
//        {
//            IList<Node> ret_list = new List<Node>(ids.Count);

//            for(int idx = 0; idx < ids.Count; idx++)
//            {
//                long id = ids[idx];
//                ret_list.Add(this.GetNode(id));
//            }

//            return ret_list;
//        }

//        /// <summary>
//        /// Creates a new relation.
//        /// </summary>
//        /// <returns></returns>
//        public Relation CreateRelation()
//        {
//            return Relation.Create(KeyGenerator.GenerateNew());
//        }

//        /// <summary>
//        /// Returns the relation with the given id.
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        public Relation GetRelation(long id)
//        {
//            if (_relations.ContainsKey(id))
//            {
//                return _relations[id];
//            }
//            return null;
//        }

//        /// <summary>
//        /// Returns the relations with the given id's.
//        /// 
//        /// The returned list will have the same size as the original
//        /// and the returned relations will be in the same position as their id's.
//        /// </summary>
//        /// <param name="ids"></param>
//        /// <returns></returns>
//        public IList<Relation> GetRelations(IList<long> ids)
//        {
//            IList<Relation> ret_list = new List<Relation>(ids.Count);

//            for (int idx = 0; idx < ids.Count; idx++)
//            {
//                long id = ids[idx];
//                ret_list.Add(this.GetRelation(id));
//            }

//            return ret_list;
//        }

//        /// <summary>
//        /// Returns the relations for the given object.
//        /// </summary>
//        /// <param name="obj"></param>
//        /// <returns></returns>
//        public IList<Relation> GetRelationsFor(OsmBase obj)
//        {
//            if (_relations_per_member.ContainsKey(obj.Id))
//            {
//                return this.GetRelations(_relations_per_member[obj.Id]);
//            }
//            return null;
//        }

//        /// <summary>
//        /// Creates a new way.
//        /// </summary>
//        /// <returns></returns>
//        public Way CreateWay()
//        {
//            return Way.Create(KeyGenerator.GenerateNew());
//        }

//        /// <summary>
//        /// Returns the way with the given id.
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        public Way GetWay(long id)
//        {
//            if (_ways.ContainsKey(id))
//            {
//                return _ways[id];
//            }
//            return null;
//        }

//        /// <summary>
//        /// Returns all the ways in this datasource.
//        /// </summary>
//        /// <returns></returns>
//        public IEnumerable<Way> GetWays()
//        {
//            return this._ways.Values;
//        }

//        /// <summary>
//        /// Returns the ways with the id's in the ids list.
//        /// 
//        /// The returned list will have the same size as the original
//        /// and the returned ways will be in the same position as their id's.
//        /// </summary>
//        /// <param name="ids"></param>
//        /// <returns></returns>
//        public IList<Way> GetWays(IList<long> ids)
//        {
//            IList<Way> ret_list = new List<Way>(ids.Count);

//            for (int idx = 0; idx < ids.Count; idx++)
//            {
//                long id = ids[idx];
//                ret_list.Add(this.GetWay(id));
//            }

//            return ret_list;
//        }

//        /// <summary>
//        /// Returns the way(s) for the given node.
//        /// </summary>
//        /// <param name="node"></param>
//        /// <returns></returns>
//        public IList<Way> GetWaysFor(Node node)
//        {
//            this.ReadFromDocument();

//            if (_ways_per_node.ContainsKey(node.Id))
//            {
//                return this.GetWays(_ways_per_node[node.Id]);
//            }
//            return null;
//        }

//        /// <summary>
//        /// Returns the objects that evaluate the filter to true.
//        /// </summary>
//        /// <param name="filter"></param>
//        /// <returns></returns>
//        public IList<OsmBase> Get(Filter filter)
//        {
//            this.ReadFromDocument();

//            IList<OsmBase> res = new List<OsmBase>();
//            foreach (Node node in _nodes.Values)
//            {
//                if (filter == null ||
//                    filter.Evaluate(node))
//                {
//                    res.Add(node);
//                }
//            }
//            foreach (Way way in _ways.Values)
//            {
//                if (filter == null ||
//                    filter.Evaluate(way))
//                {
//                    res.Add(way);
//                }
//            }
//            foreach (Relation relation in _relations.Values)
//            {
//                if (filter == null ||
//                    filter.Evaluate(relation))
//                {
//                    res.Add(relation);
//                }
//            }

//            return res;
//        }

//        /// <summary>
//        /// Returns the objects that exist withing the given box and evaluate the filter to true.
//        /// </summary>
//        /// <param name="box"></param>
//        /// <param name="filter"></param>
//        /// <returns></returns>
//        public IList<OsmGeo> Get(GeoCoordinateBox box, Filter filter)
//        {
//            this.ReadFromDocument();

//            IList<OsmGeo> res = new List<OsmGeo>();
//            foreach (Node node in _nodes.Values)
//            {
//                if ((filter == null || filter.Evaluate(node)) && node.Geometries.IsInside(box))
//                {
//                    res.Add(node);
//                }
//            } 
//            foreach (Way way in _ways.Values)
//            {
//                if ((filter == null || filter.Evaluate(way)) && way.Geometries.IsInside(box))
//                {
//                    res.Add(way);
//                }
//            }
//            foreach (Relation relation in _relations.Values)
//            {
//                if ((filter == null || filter.Evaluate(relation)) && relation.Geometries.IsInside(box))
//                {
//                    res.Add(relation);
//                }
//            }

//            return res;
//        }

//        #endregion

//        #region Private Functions (Add-Remove objects)

//        /// <summary>
//        /// Adds a list of osm objects.
//        /// </summary>
//        /// <param name="objs"></param>
//        public void AddOsmBase(IList<OsmBase> objs)
//        {
//            foreach (OsmBase obj in objs)
//            {
//                this.AddOsmBase(obj);
//            }
//        }

//        /// <summary>
//        /// Adds an osm object.
//        /// </summary>
//        /// <param name="obj"></param>
//        public void AddOsmBase(OsmBase obj)
//        {
//            switch (obj.Type)
//            {
//                case OsmType.Node:
//                    this.AddNode(obj as Node);
//                    break;
//                case OsmType.Relation:
//                    this.AddRelation(obj as Relation);
//                    break;
//                case OsmType.Way:
//                    this.AddWay(obj as Way);
//                    break;
//            }
//        }

//        /// <summary>
//        /// Adds a node.
//        /// </summary>
//        /// <param name="node"></param>
//        public void AddNode(Node node)
//        {
//            if (_nodes.ContainsKey(node.Id))
//            {
//                throw new InvalidOperationException("Cannot add an object that already exists in this source!" + Environment.NewLine +
//                    "If there is a modification use a changeset!");
//            }
//            else
//            {
//                _nodes.Add(node.Id, node);
//            }
//            this.RegisterChangeSetId(node.ChangeSetId);
//        }

//        /// <summary>
//        /// Adds a way.
//        /// </summary>
//        /// <param name="way"></param>
//        public void AddWay(Way way)
//        {
//            if (_ways.ContainsKey(way.Id))
//            {
//                throw new InvalidOperationException("Cannot add an object that already exists in this source!" + Environment.NewLine +
//                    "If there is a modification use a changeset!");
//            }
//            else
//            {
//                _ways.Add(way.Id, way);

//                foreach (Node node in way.Nodes)
//                {
//                    if (this.GetNode(node.Id) == null)
//                    {
//                        this.AddNode(node);
//                    }
//                }

//            }
//            this.RegisterChangeSetId(way.ChangeSetId);
//        }

//        /// <summary>
//        /// Adds a relation.
//        /// </summary>
//        /// <param name="relation"></param>
//        public void AddRelation(Relation relation)
//        {
//            if (_relations.ContainsKey(relation.Id))
//            {
//                throw new InvalidOperationException("Cannot add an object that already exists in this source!" + Environment.NewLine +
//                    "If there is a modification use a changeset!");
//            }
//            else
//            {
//                _relations.Add(relation.Id, relation);

//                foreach (RelationMember member in relation.Members)
//                {
//                    OsmGeo member_already_in = null;
//                    switch (member.Member.Type)
//                    {
//                        case OsmType.Node:
//                            member_already_in = this.GetNode(member.Member.Id);
//                            break;
//                        case OsmType.Relation:
//                            member_already_in = this.GetRelation(member.Member.Id);
//                            break;
//                        case OsmType.Way:
//                            member_already_in = this.GetWay(member.Member.Id);
//                            break;
//                    }
//                    if (member_already_in == null)
//                    {
//                        this.AddOsmBase(member.Member);
//                    }
//                }
//            }
//            this.RegisterChangeSetId(relation.ChangeSetId);
//        }

//        /// <summary>
//        /// Removes a list of objects from this data source.
//        /// </summary>
//        /// <param name="objs"></param>
//        private void RemoveOsmBase(IList<OsmBase> objs)
//        {
//            foreach (OsmBase obj in objs)
//            {
//                this.RemoveOsmBase(obj);
//            }
//        }

//        /// <summary>
//        /// Removes an object from this data source.
//        /// </summary>
//        /// <param name="obj"></param>
//        private void RemoveOsmBase(OsmBase obj)
//        {
//            switch (obj.Type)
//            {
//                case OsmType.Node:
//                    this.RemoveNode(obj as Node);
//                    break;
//                case OsmType.Relation:
//                    this.RemoveRelation(obj as Relation);
//                    break;
//                case OsmType.Way:
//                    this.RemoveWay(obj as Way);
//                    break;
//            }
//        }

//        /// <summary>
//        /// Removes a node from this source.
//        /// </summary>
//        /// <param name="node"></param>
//        private void RemoveNode(Node node)
//        {
//            // check if node can be removed.
//            IList<Way> ways = this.GetWaysFor(node);
//            if (ways.Count > 0)
//            { // cannot remove node; there is still a way for this node.
//                throw new InvalidOperationException("Cannot remove node {0}; there exists al least one way that uses it!");
//            }
//            IList<Relation> relations = this.GetRelationsFor(node);
//            if (relations.Count > 0)
//            { // cannot remove node; there is still a relation for this node.
//                throw new InvalidOperationException("Cannot remove node {0}; there exists al least one relation that uses it!");
//            }

//            _nodes.Remove(node.Id);
//        }

//        /// <summary>
//        /// Removes a way from this source and all the nodes in it that are used only by this way.
//        /// </summary>
//        /// <param name="way"></param>
//        private void RemoveWay(Way way)
//        {
//            // check if way can be removed.
//            IList<Relation> relations = this.GetRelationsFor(way);
//            if (relations.Count > 0)
//            { // cannot remove node; there is still a relation for this way.
//                throw new InvalidOperationException("Cannot remove way {0}; there exists al least one relation that uses it!");
//            }

//            // remove the way and all the nodes that exist only for this way.
//            _ways.Remove(way.Id);
//            foreach (Node node in way.Nodes)
//            {
//                // first remove the way from the ways per node data.
//                _ways_per_node[node.Id].Remove(way.Id);

//                // check if there are other ways that are using this node.
//                if (_ways_per_node[node.Id].Count == 0)
//                { // remove the node if if is not used in any way.
//                    this.RemoveNode(node);
//                }
//            }
//        }

//        /// <summary>
//        /// Removes a relation from this source but none of the sub-objects.
//        /// </summary>
//        /// <param name="relation"></param>
//        private void RemoveRelation(Relation relation)
//        {
//            // check if relation can be removed.
//            IList<Relation> relations = this.GetRelationsFor(relation);
//            if (relations.Count > 0)
//            { // cannot remove node; there is still a relation for this relation.
//                throw new InvalidOperationException("Cannot remove relation {0}; there exists al least one relation that uses it!");
//            }

//            // remove all the relation for all it's members
//            foreach (RelationMember member in relation.Members)
//            {
//                _relations_per_member[member.Member.Id].Remove(relation.Id);

//                // if there was only one relation for this member remove the member.
//                if (_relations_per_member[member.Member.Id].Count == 0)
//                {
//                    _relations_per_member.Remove(member.Member.Id);
//                }
//            }

//            // remove the relation.
//            _relations.Remove(relation.Id);
//        }

//        #endregion

//        #region INodeSource Members

//        Node INodeSource.GetNode(long id)
//        {
//            if (_nodes.ContainsKey(id))
//            {
//                return _nodes[id];
//            }
//            return null;
//        }

//        #endregion
    
//        #region IWaySource Members

//        Way  IWaySource.GetWay(long id)
//        {
//            if (_ways.ContainsKey(id))
//            {
//                return _ways[id];
//            }
//            return null;
//        }

//        #endregion

//        #region IRelationSource Members

//        Relation  IRelationSource.GetRelation(long id)
//        {
//            if (_relations.ContainsKey(id))
//            {
//                return _relations[id];
//            }
//            return null;
//        }

//#endregion
//    }
//}
