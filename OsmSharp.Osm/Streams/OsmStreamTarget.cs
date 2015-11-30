// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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
using OsmSharp.Osm;

namespace OsmSharp.Osm.Streams
{
    /// <summary>
    /// Any target of osm data (Nodes, Ways and Relations).
    /// </summary>
    public abstract class OsmStreamTarget
    {
        private readonly TagsCollectionBase _meta;

        /// <summary>
        /// Holds the source for this target.
        /// </summary>
        private OsmStreamSource _source;

        /// <summary>
        /// Creates a new target.
        /// </summary>
        protected OsmStreamTarget()
        {
            _meta = new TagsCollection();
        }

        /// <summary>
        /// Initializes the target.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Adds a node to the target.
        /// </summary>
        /// <param name="simpleNode"></param>
        public abstract void AddNode(Node simpleNode);

        /// <summary>
        /// Adds a way to the target.
        /// </summary>
        /// <param name="simpleWay"></param>
        public abstract void AddWay(Way simpleWay);

        /// <summary>
        /// Adds a relation to the target.
        /// </summary>
        /// <param name="simpleRelation"></param>
        public abstract void AddRelation(Relation simpleRelation);

        /// <summary>
        /// Registers a reader on this writer.
        /// </summary>
        /// <param name="source"></param>
        public virtual void RegisterSource(OsmStreamSource source)
        {
            _source = source;
        }

        /// <summary>
        /// Returns the registered reader.
        /// </summary>
        protected OsmStreamSource Source
        {
            get
            {
                return _source;
            }
        }

        /// <summary>
        /// Pulls the changes from the source to this target.
        /// </summary>
        public void Pull()
        {
            _source.Initialize();
            this.Initialize();
            if (this.OnBeforePull())
            {
                this.DoPull();
                this.OnAfterPull();
            }
            this.Flush();
            this.Close();
        }

        /// <summary>
        /// Pulls the next object and returns true if there was one.
        /// </summary>
        /// <returns></returns>
        public bool PullNext()
        {
            if (_source.MoveNext())
            {
                object sourceObject = _source.Current();
                if (sourceObject is Node)
                {
                    this.AddNode(sourceObject as Node);
                }
                else if (sourceObject is Way)
                {
                    this.AddWay(sourceObject as Way);
                }
                else if (sourceObject is Relation)
                {
                    this.AddRelation(sourceObject as Relation);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Does the pull operation until source is exhausted.
        /// </summary>
        protected void DoPull()
        {
            this.DoPull(false, false, false);
        }

        /// <summary>
        /// Does the pull operation until source is exhausted.
        /// </summary>
        /// <param name="ignoreNodes">Makes the source skip all nodes.</param>
        /// <param name="ignoreWays">Makes the source skip all ways.</param>
        /// <param name="ignoreRelations">Makes the source skip all relations.</param>
        protected void DoPull(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
        {
            while (_source.MoveNext(ignoreNodes, ignoreWays, ignoreRelations))
            {
                object sourceObject = _source.Current();
                if (sourceObject is Node)
                {
                    this.AddNode(sourceObject as Node);
                }
                else if (sourceObject is Way)
                {
                    this.AddWay(sourceObject as Way);
                }
                else if (sourceObject is Relation)
                {
                    this.AddRelation(sourceObject as Relation);
                }
            }
        }

        /// <summary>
        /// Called right before pull and right after initialization.
        /// </summary>
        public virtual bool OnBeforePull()
        {
            return true;
        }

        /// <summary>
        /// Called right after pull and right before flush.
        /// </summary>
        public virtual void OnAfterPull()
        {

        }

        /// <summary>
        /// Gets the meta-data.
        /// </summary>
        public TagsCollectionBase Meta
        {
            get
            {
                return _meta;
            }
        }

        /// <summary>
        /// Gets all meta-data from all sources and filters that provide this target of data.
        /// </summary>
        /// <returns></returns>
        public TagsCollection GetAllMeta()
        {
            var tags = this.Source.GetAllMeta();
            tags.AddOrReplace(new TagsCollection(_meta));
            return tags;
        }

        /// <summary>
        /// Closes the current target.
        /// </summary>
        public virtual void Close()
        {

        }

        /// <summary>
        /// Flushes the current target.
        /// </summary>
        public virtual void Flush()
        {

        }
    }
}