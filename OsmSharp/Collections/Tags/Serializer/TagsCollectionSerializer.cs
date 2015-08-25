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

using ProtoBuf.Meta;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Collections.Tags.Serializer
{
    /// <summary>
    /// Serializer/deserializer for tag collections.
    /// </summary>
    public class TagsCollectionSerializer
    {
        /// <summary>
        /// Serializes a tags collection to a byte array and addes the size in the first 4 bytes.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public void SerializeWithSize(TagsCollectionBase collection, Stream stream)
        {
            RuntimeTypeModel typeModel = TypeModel.Create();
            typeModel.Add(typeof(Tag), true);

            var tagsList = new List<Tag>(collection);
            typeModel.SerializeWithSize(stream, tagsList);
        }

        /// <summary>
        /// Deserializes a tags collection from a byte array and takes the data size from the first 4 bytes.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public TagsCollectionBase DeserializeWithSize(Stream stream)
        {
            RuntimeTypeModel typeModel = TypeModel.Create();
            typeModel.Add(typeof(Tag), true);
            return new TagsCollection(typeModel.DeserializeWithSize(stream, 
                null, typeof(List<Tag>)) as List<Tag>);
        }
    }
}