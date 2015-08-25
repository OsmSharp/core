//// OsmSharp - OpenStreetMap (OSM) SDK
//// Copyright (C) 2013 Abelshausen Ben
//// 
//// This file is part of OsmSharp.
//// 
//// OsmSharp is free software: you can redistribute it and/or modify
//// it under the terms of the GNU General Public License as published by
//// the Free Software Foundation, either version 2 of the License, or
//// (at your option) any later version.
//// 
//// OsmSharp is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//// GNU General Public License for more details.
//// 
//// You should have received a copy of the GNU General Public License
//// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

//using NUnit.Framework;
//using OsmSharp.Osm.Cache;

//namespace OsmSharp.Test.Unittests.Osm.Cache
//{
//    /// <summary>
//    /// Contains test for the disk osm data cache.
//    /// </summary>
//    [TestFixture]
//    public class OsmDataCacheDiskTests : OsmDataCacheTestsBase
//    {
//        /// <summary>
//        /// Tests simple node read/write.
//        /// </summary>
//        [Test]
//        public void OsmDataCacheDiskNodeTest()
//        {
//            OsmDataCacheDisk cache = new OsmDataCacheDisk();
//            base.DoOsmDataCacheTestNode(cache);
//            cache.Dispose();
//        }

//        /// <summary>
//        /// Tests simple way read/write.
//        /// </summary>
//        [Test]
//        public void OsmDataCacheDiskWayTest()
//        {
//            OsmDataCacheDisk cache = new OsmDataCacheDisk();
//            base.DoOsmDataCacheTestWay(cache);
//            cache.Dispose();
//        }

//        /// <summary>
//        /// Tests simple relation read/write.
//        /// </summary>
//        [Test]
//        public void OsmDataCacheDiskRelationTest()
//        {
//            OsmDataCacheDisk cache = new OsmDataCacheDisk();
//            base.DoOsmDataCacheTestRelation(cache);
//            cache.Dispose();
//        }

//        /// <summary>
//        /// Tests clear.
//        /// </summary>
//        [Test]
//        public void OsmDataCacheDiskClearTest()
//        {
//            base.DoOsmDataCacheTestClear(new OsmDataCacheDisk());
//        }
//    }
//}
