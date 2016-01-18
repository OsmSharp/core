// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2016 Abelshausen Ben
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
using System;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace OsmSharp.Test.Osm.Xml
{
    /// <summary>
    /// Contains tests for the diff result class.
    /// </summary>
    [TestFixture]
    public class DiffResultTests
    {
        /// <summary>
        /// Tests serialization.
        /// </summary>
        [Test]
        public void TestSerialize()
        {
            var diffResult = new OsmSharp.Osm.Xml.v0_6.diffResult()
            { 
                version = 0.6,
                versionSpecified = true,
                generator = "OsmSharp",
                osmresult = new OsmSharp.Osm.Xml.v0_6.osmresult[]
                {
                    new OsmSharp.Osm.Xml.v0_6.noderesult()
                    {
                        old_id = 1,
                        old_idSpecified = true,
                        new_id = 2,
                        new_idSpecified = true,
                        new_version = 2,
                        new_versionSpecified = true
                    }
                }
            };


            var settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Indent = false;
            settings.NewLineChars = string.Empty;
            var emptyNamespace = new XmlSerializerNamespaces();
            emptyNamespace.Add(string.Empty, string.Empty);

            var serializer = new XmlSerializer(typeof(OsmSharp.Osm.Xml.v0_6.diffResult));
            var resultStream = new MemoryStream();
            using (var stringWriter = XmlWriter.Create(resultStream, settings))
            {
                serializer.Serialize(stringWriter, diffResult, emptyNamespace);
                resultStream.Seek(0, SeekOrigin.Begin);
            }
            var result = string.Empty;
            using (var streamReader = new StreamReader(resultStream))
            {
                result = streamReader.ReadToEnd();
            }

            Assert.AreEqual("<diffResult generator=\"OsmSharp\" version=\"0.6\"><node old_id=\"1\" new_id=\"2\" new_version=\"2\" /></diffResult>", 
                result);

            diffResult = new OsmSharp.Osm.Xml.v0_6.diffResult();
            diffResult.osmresult = new OsmSharp.Osm.Xml.v0_6.osmresult[]
            {
                new OsmSharp.Osm.Xml.v0_6.noderesult()
                {
                    old_id = 1,
                    old_idSpecified = true,
                    new_id = 2,
                    new_idSpecified = true,
                    new_version = 2,
                    new_versionSpecified = true
                },
                new OsmSharp.Osm.Xml.v0_6.wayresult()
                {
                    old_id = 1,
                    old_idSpecified = true,
                    new_idSpecified = false,
                    new_versionSpecified = false
                },
                new OsmSharp.Osm.Xml.v0_6.relationresult()
                {
                    old_id = 4,
                    old_idSpecified = true,
                    new_id = 5,
                    new_idSpecified = true,
                    new_version = 6,
                    new_versionSpecified = true
                },
                new OsmSharp.Osm.Xml.v0_6.wayresult()
                {
                    old_id = 7,
                    old_idSpecified = true,
                    new_id = 8,
                    new_idSpecified = true,
                    new_version = 9,
                    new_versionSpecified = true
                },
                new OsmSharp.Osm.Xml.v0_6.noderesult()
                {
                    old_id = 10,
                    old_idSpecified = true,
                    new_id = 11,
                    new_idSpecified = true,
                    new_version = 12,
                    new_versionSpecified = true
                }
            };
            
            resultStream = new MemoryStream();
            using (var stringWriter = XmlWriter.Create(resultStream, settings))
            {
                serializer.Serialize(stringWriter, diffResult, emptyNamespace);
                resultStream.Seek(0, SeekOrigin.Begin);
            }
            result = string.Empty;
            using (var streamReader = new StreamReader(resultStream))
            {
                result = streamReader.ReadToEnd();
            }

            Assert.AreEqual("<diffResult>" + 
                "<node old_id=\"1\" new_id=\"2\" new_version=\"2\" />" +
                "<way old_id=\"1\" />" +
                "<relation old_id=\"4\" new_id=\"5\" new_version=\"6\" />" +
                "<way old_id=\"7\" new_id=\"8\" new_version=\"9\" />" +
                "<node old_id=\"10\" new_id=\"11\" new_version=\"12\" />" +
                "</diffResult>", result);
        }
    }
}
