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

namespace OsmSharp.Collections.Tags.Index
{
    /// <summary>
    /// Abstracts an index containing tags.
    /// </summary>
    public interface ITagsIndex
    {
        /// <summary>
        /// Returns true if this collection is readonly.
        /// </summary>
        bool IsReadonly
        {
            get;
        }

        /// <summary>
        /// Returns the tags that belong to the given id.
        /// </summary>
        /// <param name="tagsId"></param>
        /// <returns></returns>
        TagsCollectionBase Get(uint tagsId);

        /// <summary>
        /// Adds new tags.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        uint Add(TagsCollectionBase tags);

        /// <summary>
        /// Serializes this tags index to the given stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        long Serialize(System.IO.Stream stream);
    }
}