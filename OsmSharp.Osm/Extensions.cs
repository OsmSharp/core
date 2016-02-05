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

using System;
using System.Globalization;
using System.Xml;

namespace OsmSharp.Osm
{
    /// <summary>
    /// Contains extensions.
    /// </summary>
    public static class Extensions
    {
        private static string DATE_FORMAT = "yyyy-MM-ddTHH:mm:ssZ";

        /// <summary>
        /// Writes a datetime as an attribute.
        /// </summary>
        public static void WriteAttribute(this XmlWriter writer, string name, DateTime? value)
        {
            if (value.HasValue)
            {
                writer.WriteAttributeString(name, value.Value.ToString(DATE_FORMAT));
            }
        }

        /// <summary>
        /// Writes an Int64 as an attribute.
        /// </summary>
        public static void WriteAttribute(this XmlWriter writer, string name, long? value)
        {
            if(value.HasValue)
            {
                writer.WriteAttributeString(name, value.Value.ToInvariantString());
            }
        }

        /// <summary>
        /// Writes an Int32 as an attribute.
        /// </summary>
        public static void WriteAttribute(this XmlWriter writer, string name, int? value)
        {
            if (value.HasValue)
            {
                writer.WriteAttributeString(name, value.Value.ToInvariantString());
            }
        }

        /// <summary>
        /// Writes a single as an attribute.
        /// </summary>
        public static void WriteAttribute(this XmlWriter writer, string name, float? value)
        {
            if (value.HasValue)
            {
                writer.WriteAttributeString(name, value.Value.ToInvariantString());
            }
        }

        /// <summary>
        /// Writes a double as an attribute.
        /// </summary>
        public static void WriteAttribute(this XmlWriter writer, string name, double? value)
        {
            if (value.HasValue)
            {
                writer.WriteAttributeString(name, value.Value.ToInvariantString());
            }
        }

        /// <summary>
        /// Writes a string as an attribute.
        /// </summary>
        public static void WriteAttribute(this XmlWriter writer, string name, string value)
        {
            if (value != null)
            {
                writer.WriteAttributeString(name, value);
            }
        }

        /// <summary>
        /// Writes a bool as an attribute.
        /// </summary>
        public static void WriteAttribute(this XmlWriter writer, string name, bool? value)
        {
            if (value.HasValue)
            {
                writer.WriteAttributeString(name, value.Value.ToInvariantString().ToLowerInvariant());
            }
        }

        /// <summary>
        /// Reads a double attribute.
        /// </summary>
        public static double? GetAttributeDouble(this XmlReader reader, string name)
        {
            var valueString = reader.GetAttribute(name);
            double value = 0;
            if (!string.IsNullOrWhiteSpace(valueString) &&
               double.TryParse(valueString, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            {
                return value;
            }
            return null;
        }

        /// <summary>
        /// Reads a single attribute.
        /// </summary>
        public static float? GetAttributeSingle(this XmlReader reader, string name)
        {
            var valueString = reader.GetAttribute(name);
            float value = 0;
            if (!string.IsNullOrWhiteSpace(valueString) &&
               float.TryParse(valueString, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            {
                return value;
            }
            return null;
        }

        /// <summary>
        /// Reads an Int64 attribute.
        /// </summary>
        public static long? GetAttributeInt64(this XmlReader reader, string name)
        {
            var valueString = reader.GetAttribute(name);
            long value = 0;
            if (!string.IsNullOrWhiteSpace(valueString) &&
               long.TryParse(valueString, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            {
                return value;
            }
            return null;
        }

        /// <summary>
        /// Reads an Int32 attribute.
        /// </summary>
        public static int? GetAttributeInt32(this XmlReader reader, string name)
        {
            var valueString = reader.GetAttribute(name);
            int value = 0;
            if (!string.IsNullOrWhiteSpace(valueString) &&
               int.TryParse(valueString, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            {
                return value;
            }
            return null;
        }

        /// <summary>
        /// Reads a boolean attribute.
        /// </summary>
        public static bool? GetAttributeBool(this XmlReader reader, string name)
        {
            var valueString = reader.GetAttribute(name);
            bool value = false;
            if (!string.IsNullOrWhiteSpace(valueString) &&
               bool.TryParse(valueString, out value))
            {
                return value;
            }
            return null;
        }

        /// <summary>
        /// Reads a datetime attribute.
        /// </summary>
        public static DateTime? GetAttributeDateTime(this XmlReader reader, string name)
        {
            var valueString = reader.GetAttribute(name);
            DateTime value;
            if (!string.IsNullOrWhiteSpace(valueString) &&
               DateTime.TryParse(valueString, out value))
            {
                return value;
            }
            return null;
        }
    }
}
