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

using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace OsmSharp.Test
{
    /// <summary>
    /// Contains utilities to optimize test-code.
    /// </summary>
    public static class TestUtilities
    {
        /// <summary>
        /// Reads a string.
        /// </summary>
        public static string ReadBeginToEnd(this MemoryStream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var streamReader = new StreamReader(stream);
            return streamReader.ReadToEnd();
        }

        /// <summary>
        /// Serializes to xml with default settings for OSM-related entities.
        /// </summary>
        public static string SerializeToXml<T>(this T value)
        {
            var serializer = new XmlSerializer(typeof(T));

            var settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Indent = false;
            settings.NewLineChars = string.Empty;
            var emptyNamespace = new XmlSerializerNamespaces();
            emptyNamespace.Add(string.Empty, string.Empty);

            var result = string.Empty;
            using (var resultStream = new MemoryStream())
            {
                using (var stringWriter = XmlWriter.Create(resultStream, settings))
                {
                    serializer.Serialize(stringWriter, value, emptyNamespace);
                    return resultStream.ReadBeginToEnd();
                }
            }
        }
    }
}
