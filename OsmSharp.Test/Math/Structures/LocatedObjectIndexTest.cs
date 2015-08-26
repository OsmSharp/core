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
using NUnit.Framework;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Structures;

namespace OsmSharp.Test.Math.Structures
{
    /// <summary>
    /// Contains common test functions for located object index tests.
    /// </summary>
    public abstract class LocatedObjectIndexTest
    {
        /// <summary>
        /// Creates the located object index to test.
        /// </summary>
        /// <returns></returns>
        public abstract ILocatedObjectIndex<GeoCoordinate, LocatedObjectData> CreateIndex();

        /// <summary>
        /// Tests adding some simple data.
        /// </summary>
        protected void DoTestSimple()
        {
            // create the index.
            ILocatedObjectIndex<GeoCoordinate, LocatedObjectData> index = this.CreateIndex();

            // add the data.
            GeoCoordinate point1 = new GeoCoordinate(0, 0);
            LocatedObjectData point1_data = new LocatedObjectData()
            {
                SomeData = point1.ToString()
            };
            GeoCoordinate point2 = new GeoCoordinate(1, 1);
            LocatedObjectData point2_data = new LocatedObjectData()
            {
                SomeData = point2.ToString()
            };
            
            GeoCoordinateBox location_box = new GeoCoordinateBox(
                new GeoCoordinate(point1.Latitude - 0.0001, point1.Longitude - 0.0001),
                new GeoCoordinate(point1.Latitude + 0.0001, point1.Longitude + 0.0001));

            // try and get data from empty index.
            // regression test for issue: https://osmsharp.codeplex.com/workitem/1244
            IEnumerable<LocatedObjectData> location_box_data = index.GetInside(location_box);
            Assert.IsNotNull(location_box_data);
            Assert.AreEqual(0, location_box_data.Count());

            // try point1.
            index.Add(point1, point1_data);

            location_box_data = index.GetInside(
                location_box);
            Assert.IsNotNull(location_box_data);

            bool found = false;
            foreach (LocatedObjectData location_data in location_box_data)
            {
                if (location_data.SomeData == point1.ToString())
                {
                    found = true;
                }
            }
            Assert.IsTrue(found, string.Format("Data added at location {0} not found in box {1}!",
                point1, location_box));

            // try point2.
            index.Add(point2, point2_data);
            location_box = new GeoCoordinateBox(
                new GeoCoordinate(point2.Latitude - 0.0001, point2.Longitude - 0.0001),
                new GeoCoordinate(point2.Latitude + 0.0001, point2.Longitude + 0.0001));

            location_box_data = index.GetInside(
                location_box);
            Assert.IsNotNull(location_box_data);

            found = false;
            foreach (LocatedObjectData location_data in location_box_data)
            {
                if (location_data.SomeData == point2.ToString())
                {
                    found = true;
                }
            }
            Assert.IsTrue(found, string.Format("Data added at location {0} not found in box {1}!",
                point2, location_box));
        }

        /// <summary>
        /// Tests adding a lot of random data.
        /// </summary>
        /// <param name="count"></param>
        public void DoTestAddingRandom(int count)
        {
            ILocatedObjectIndex<GeoCoordinate, LocatedObjectData> index = this.CreateIndex();

            GeoCoordinateBox box = new GeoCoordinateBox(new GeoCoordinate(50, 3), new GeoCoordinate(40, 2));
            HashSet<GeoCoordinate> locations = new HashSet<GeoCoordinate>();
            Random random = new Random();
            while (count > 0)
            {
                GeoCoordinate location = box.GenerateRandomIn(random);
                LocatedObjectData data = new LocatedObjectData()
                {
                    SomeData = location.ToString()
                };
                locations.Add(location);
                index.Add(location, data);

                // try immidiately after.
                GeoCoordinateBox location_box = new GeoCoordinateBox(
                    new GeoCoordinate(location.Latitude - 0.0001, location.Longitude - 0.0001),
                    new GeoCoordinate(location.Latitude + 0.0001, location.Longitude + 0.0001));

                IEnumerable<LocatedObjectData> location_box_data = index.GetInside(
                    location_box);

                Assert.IsNotNull(location_box_data);

                bool found = false;
                foreach (LocatedObjectData location_data in location_box_data)
                {
                    if (location_data.SomeData == location.ToString())
                    {
                        found = true;
                    }
                }
                Assert.IsTrue(found, string.Format("Data added at location {0} not found in box {1}!",
                    location, location_box));

                count--;
            }

            foreach (GeoCoordinate location in locations)
            {
                GeoCoordinateBox location_box = new GeoCoordinateBox(
                    new GeoCoordinate(location.Latitude - 0.0001, location.Longitude - 0.0001),
                    new GeoCoordinate(location.Latitude + 0.0001, location.Longitude + 0.0001));

                IEnumerable<LocatedObjectData> location_box_data = index.GetInside(
                    location_box);

                Assert.IsNotNull(location_box_data);

                bool found = false;
                foreach (LocatedObjectData location_data in location_box_data)
                {
                    if (location_data.SomeData == location.ToString())
                    {
                        found = true;
                    }
                }
                Assert.IsTrue(found, string.Format("Data added at location {0} not found in box {1}!",
                    location, location_box));
            }
        }
    }

    /// <summary>
    /// Test data object.
    /// </summary>
    public class LocatedObjectData
    {
        /// <summary>
        /// Some data.
        /// </summary>
        public string SomeData { get; set; }
    }
}
