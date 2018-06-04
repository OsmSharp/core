// The MIT License (MIT)

// Copyright (c) 2016 Ben Abelshausen

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

namespace OsmSharp.Streams.Collections
{
    /// <summary>
    /// Contains extension methods for the osm id index.
    /// </summary>
    public static class OsmIdIndexExtensions
    {
        /// <summary>
        /// Returns true if the given way has a node in the id index.
        /// </summary>
        public static bool HasNodeIn(this Way way, OsmIdIndex index)
        {
            if (way.Nodes != null)
            {
                for (var i = 0; i < way.Nodes.Length; i++)
                {
                    if (index.Contains(way.Nodes[i]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true if the given relation has a member in one of the id indexes.
        /// </summary>
        public static bool HasMemberIn(this Relation relation, OsmIdIndex nodeIndex, OsmIdIndex wayIndex, OsmIdIndex relationIndex)
        {
            if (relation.Members != null)
            {
                for (var i = 0; i < relation.Members.Length; i++)
                {
                    if (relation.Members != null)
                    {
                        if (relation.Members[i].Type == OsmGeoType.Node)
                        { 
                            if (nodeIndex.Contains(relation.Members[i].Id))
                            {
                                return true;
                            }
                        }
                        else if (relation.Members[i].Type == OsmGeoType.Way)
                        {
                            if (wayIndex.Contains(relation.Members[i].Id))
                            {
                                return true;
                            }
                        }
                        else if (relation.Members[i].Type == OsmGeoType.Relation)
                        {
                            if (relationIndex.Contains(relation.Members[i].Id))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}