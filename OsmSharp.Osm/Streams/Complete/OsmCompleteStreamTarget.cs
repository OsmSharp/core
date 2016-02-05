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

using OsmSharp.Osm.Data;

namespace OsmSharp.Osm.Streams.Complete
{
    /// <summary>
    /// Any target of osm data (Nodes, Ways and Relations).
    /// </summary>
    public abstract class OsmCompleteStreamTarget
    {
        /// <summary>
        /// Creates a new target.
        /// </summary>
        protected OsmCompleteStreamTarget()
        {

        }

        private OsmCompleteStreamSource _source; // Holds the source for this target.

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
        public abstract void AddWay(CompleteWay way);

        /// <summary>
        /// Adds a relation to the target.
        /// </summary>
        public abstract void AddRelation(CompleteRelation relation);

        /// <summary>
        /// Registers a source on this target.
        /// </summary>
        public void RegisterSource(OsmCompleteStreamSource source)
        {
            _source = source;
        }

        /// <summary>
        /// Registers a simple source on this target.
        /// </summary>
        public void RegisterSource(OsmStreamSource source)
        {
            _source = new OsmSimpleCompleteStreamSource(source);
        }

        /// <summary>
        /// Registers a simple source on this target with a given cache.
        /// </summary>
        public void RegisterSource(OsmStreamSource source, ISnapshotDb cacheDb)
        {
            _source = new OsmSimpleCompleteStreamSource(source, cacheDb);
        }

        /// <summary>
        /// Returns the registered reader.
        /// </summary>
        protected OsmCompleteStreamSource Source
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
            while (_source.MoveNext())
            {
                var sourceObject = _source.Current();
                if (sourceObject is Node)
                {
                    this.AddNode(sourceObject as Node);
                }
                else if (sourceObject is CompleteWay)
                {
                    this.AddWay(sourceObject as CompleteWay);
                }
                else if (sourceObject is CompleteRelation)
                {
                    this.AddRelation(sourceObject as CompleteRelation);
                }
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
                var sourceObject = _source.Current();
                if (sourceObject is Node)
                {
                    this.AddNode(sourceObject as Node);
                }
                else if (sourceObject is CompleteWay)
                {
                    this.AddWay(sourceObject as CompleteWay);
                }
                else if (sourceObject is CompleteRelation)
                {
                    this.AddRelation(sourceObject as CompleteRelation);
                }
                return true;
            }
            return false;
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