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

using NUnit.Framework;
using System.Reflection;
using OsmSharp.Osm.Streams;
using OsmSharp.Osm.PBF.Streams;

namespace OsmSharp.Test.Osm.PBF.Streams
{
    /// <summary>
    /// Contains tests for the PBF osm stream source.
    /// </summary>
    [TestFixture]
    class PBFOsmStreamSourceTests
    {
        /// <summary>
        /// A regression test on resetting a PBF osm stream.
        /// </summary>
        [Test]
        public void PBFOsmStreamReaderReset()
        {
            // generate the source.
            var source = new PBFOsmStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "OsmSharp.Test.data.api.osm.pbf"));

            // pull the data out.
            var target = new OsmStreamTargetEmpty();
            target.RegisterSource(source);
            target.Pull();

            // reset the source.
            if (source.CanReset)
            {
                source.Reset();

                // pull the data again.
                target.Pull();
            }
        }

        /// <summary>
        /// A regression test on initializing a stream.
        /// </summary>
        [Test]
        public void MoveNextWayRegression1()
        {
            using (var fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "OsmSharp.Test.data.api.osm.pbf"))
            {
                using (var reader = new PBFOsmStreamSource(fileStream))
                {
                    reader.Initialize();
                    var counter = 0;
                    while (reader.MoveNextWay())
                    {
                        if (counter++ % 10000 == 0)
                        {
                            
                        }
                    }
                }
            }
        }
    }
}