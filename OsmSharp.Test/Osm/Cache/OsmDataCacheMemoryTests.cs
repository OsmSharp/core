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

using NUnit.Framework;
using OsmSharp.Osm.Cache;

namespace OsmSharp.Test.Osm.Cache
{
    /// <summary>
    /// Contains test for the memory osm data cache.
    /// </summary>
    [TestFixture]
    public class OsmDataCacheMemoryTests : OsmDataCacheTestsBase
    {
        /// <summary>
        /// Tests simple node read/write.
        /// </summary>
        [Test]
        public void OsmDataCacheMemoryNodeTest()
        {
            base.DoOsmDataCacheTestNode(new OsmDataCacheMemory());
        }

        /// <summary>
        /// Tests simple way read/write.
        /// </summary>
        [Test]
        public void OsmDataCacheMemoryWayTest()
        {
            base.DoOsmDataCacheTestWay(new OsmDataCacheMemory());
        }

        /// <summary>
        /// Tests simple relation read/write.
        /// </summary>
        [Test]
        public void OsmDataCacheMemoryRelationTest()
        {
            base.DoOsmDataCacheTestRelation(new OsmDataCacheMemory());
        }

        /// <summary>
        /// Tests clear.
        /// </summary>
        [Test]
        public void OsmDataCacheMemoryClearTest()
        {
            base.DoOsmDataCacheTestClear(new OsmDataCacheMemory());
        }
    }
}
