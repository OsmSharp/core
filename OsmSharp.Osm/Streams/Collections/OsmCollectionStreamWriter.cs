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

namespace OsmSharp.Osm.Streams.Collections
{
    /// <summary>
    /// A data processor target for regular SimpleOsmBase objects.
    /// </summary>
    internal class OsmCollectionStreamWriter : OsmStreamTarget
    {
        /// <summary>
        /// Holds the target list.
        /// </summary>
        private readonly ICollection<OsmGeo> _baseObjects;

        /// <summary>
        /// Creates a new collection data processor target.
        /// </summary>
        /// <param name="baseObjects"></param>
        public OsmCollectionStreamWriter(ICollection<OsmGeo> baseObjects)
        {
            _baseObjects = baseObjects;
        }

        /// <summary>
        /// Initializes this target.
        /// </summary>
        public override void Initialize()
        {

        }

        /// <summary>
        /// Adds a node to the target.
        /// </summary>
        /// <param name="node"></param>
        public override void AddNode(Node node)
        {
            if (_baseObjects == null)
            { // the base object collection is null.
                throw new InvalidOperationException("No target collection set!");
            }

            // add the node to the collection.
            _baseObjects.Add(node);
        }

        /// <summary>
        /// Adds a way to the target.
        /// </summary>
        /// <param name="way"></param>
        public override void AddWay(Way way)
        {
            if (_baseObjects == null)
            { // the base object collection is null.
                throw new InvalidOperationException("No target collection set!");
            }

            // add the way to the collection.
            _baseObjects.Add(way);
        }

        /// <summary>
        /// Adds a relation to the target.
        /// </summary>
        /// <param name="relation"></param>
        public override void AddRelation(Relation relation)
        {
            if (_baseObjects == null)
            { // the base object collection is null.
                throw new InvalidOperationException("No target collection set!");
            }

            // add the relation to the collection.
            _baseObjects.Add(relation);
        }
    }
}