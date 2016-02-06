using OsmSharp.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Streams
{
    /// <summary>
    /// Any target of osm data (Nodes, Ways and Relations).
    /// </summary>
    public abstract class OsmStreamTarget
    {
        private readonly TagsCollectionBase _meta;

        /// <summary>
        /// Creates a new target.
        /// </summary>
        protected OsmStreamTarget()
        {
            _meta = new TagsCollection();
        }

        private OsmStreamSource _source; // Holds the source for this target.

        /// <summary>
        /// Initializes the target.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Adds a node to the target.
        /// </summary>
        public abstract void AddNode(Node node);

        /// <summary>
        /// Adds a way to the target.
        /// </summary>
        public abstract void AddWay(Way way);

        /// <summary>
        /// Adds a relation to the target.
        /// </summary>
        public abstract void AddRelation(Relation relation);

        /// <summary>
        /// Registers a source on this target.
        /// </summary>
        public void RegisterSource(IEnumerable<OsmGeo> source)
        {
            this.RegisterSource(new OsmEnumerableStreamSource(source));
        }

        /// <summary>
        /// Registers a reader on this writer.
        /// </summary>
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
        public bool PullNext()
        {
            if (_source.MoveNext())
            {
                var sourceObject = _source.Current();
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
        protected void DoPull(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
        {
            while (_source.MoveNext(ignoreNodes, ignoreWays, ignoreRelations))
            {
                var sourceObject = _source.Current();
                switch (sourceObject.Type)
                {
                    case OsmGeoType.Node:
                        this.AddNode(sourceObject as Node);
                        break;
                    case OsmGeoType.Way:
                        this.AddWay(sourceObject as Way);
                        break;
                    case OsmGeoType.Relation:
                        this.AddRelation(sourceObject as Relation);
                        break;
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
        public TagsCollection GetAllMeta()
        {
            var tags = this.Source.GetAllMeta();
            tags.AddOrReplace(_meta);
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