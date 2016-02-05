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

using System.Reflection;
using NUnit.Framework;
using OsmSharp.Osm.Streams;
using OsmSharp.Osm.Xml.Streams;

namespace OsmSharp.Test.Osm.Xml.Streams
{
    /// <summary>
    /// Summary description for XmlDataProcessorSourceTests
    /// </summary>
    [TestFixture]
    public class XmlDataProcessorSourceTests
    {
        /// <summary>
        /// A regression test in resetting and XML data source.
        /// </summary>
        [Test]
        public void XmlDataProcessorSourceReset()
        {
            // generate the source.
            var source = new XmlOsmStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "OsmSharp.Test.data.api.osm"));

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
    }
}