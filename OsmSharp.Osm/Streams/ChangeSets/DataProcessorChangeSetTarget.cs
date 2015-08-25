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
using OsmSharp.Osm;

namespace OsmSharp.Osm.Streams.ChangeSets
{
    /// <summary>
    /// A changeset target that acts as a source of osm data; 
    /// Input=One changeset stream
    /// </summary>
    public abstract class DataProcessorChangeSetTarget
    {
        /// <summary>
        /// The source this target get's it's data from.
        /// </summary>
        private DataProcessorChangeSetSource _source;

        /// <summary>
        /// Creates a new change set data source by applying the changes to the given source.
        /// </summary>
        public DataProcessorChangeSetTarget()
        {

        }

        /// <summary>
        /// Applies a change to the target.
        /// </summary>
        /// <param name="change"></param>
        public abstract void ApplyChange(ChangeSet change);

        /// <summary>
        /// Pulls the changes from the source to this target.
        /// </summary>
        public void Pull()
        {
            _source.Initialize();
            this.Initialize();
            while (_source.MoveNext())
            {
                ChangeSet change_set = _source.Current();
                this.ApplyChange(change_set);
            }
        }

        /// <summary>
        /// Initializes this target.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Closes the current target.
        /// </summary>
        public virtual void Close()
        {
            _source = null;
        }

        /// <summary>
        /// Registers the source for this target.
        /// </summary>
        /// <param name="source"></param>
        public void RegisterSource(DataProcessorChangeSetSource source)
        {
            _source = source;
        }
    }
}
