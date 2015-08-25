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
using OsmSharp.Collections.Tags.Index;

namespace OsmSharp.Osm.Streams
{
    /// <summary>
    /// A stream target that fills the given tags index.
    /// </summary>
    public class OsmStreamTargetTags : OsmStreamTarget
    {
        /// <summary>
        /// Holds the tags index.
        /// </summary>
        private ITagsIndex _tagsIndex;

        /// <summary>
        /// Creates a new OSM stream target.
        /// </summary>
        /// <param name="tagsIndex"></param>
        public OsmStreamTargetTags(ITagsIndex tagsIndex)
        {
            _tagsIndex = tagsIndex;
        }

        /// <summary>
        /// Initializes this target.
        /// </summary>
        public override void Initialize()
        {

        }

        /// <summary>
        /// Processes the given node.
        /// </summary>
        /// <param name="node"></param>
        public override void AddNode(Node node)
        {
            if (node.Tags != null)
            {
                _tagsIndex.Add(node.Tags);
            }
        }

        /// <summary>
        /// Processes the given way.
        /// </summary>
        /// <param name="way"></param>
        public override void AddWay(Way way)
        {
            if (way.Tags != null)
            {
                _tagsIndex.Add(way.Tags);
            }
        }

        /// <summary>
        /// Processes the given relation.
        /// </summary>
        /// <param name="relation"></param>
        public override void AddRelation(Relation relation)
        {
            if (relation.Tags != null)
            {
                _tagsIndex.Add(relation.Tags);
            }
        }
    }
}
