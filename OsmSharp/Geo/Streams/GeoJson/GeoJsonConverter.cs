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

using OsmSharp.Geo.Attributes;
using OsmSharp.Geo.Features;
using OsmSharp.Geo.Geometries;
using OsmSharp.IO.Json;
using OsmSharp.IO.Json.Linq;
using OsmSharp.Math.Geo;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Geo.Streams.GeoJson
{
    /// <summary>
    /// A GeoJson converter. Converts GeoJson strings from/to Features.
    /// </summary>
    public static class GeoJsonConverter
    {
        /// <summary>
        /// Generates GeoJson for the given feature collection.
        /// </summary>
        public static string ToGeoJson(this FeatureCollection featureCollection)
        {
            if (featureCollection == null) { throw new ArgumentNullException("featureCollection"); }

            var jsonWriter = new JTokenWriter();
            GeoJsonConverter.Write(jsonWriter, featureCollection);
            return jsonWriter.Token.ToString();
        }

        /// <summary>
        /// Generates GeoJson for the given feature collection.
        /// </summary>
        public static string ToGeoJson(this IEnumerable<Feature> featureCollection)
        {
            if (featureCollection == null) { throw new ArgumentNullException("featureCollection"); }

            var jsonWriter = new JTokenWriter();
            GeoJsonConverter.Write(jsonWriter, featureCollection);
            return jsonWriter.Token.ToString();
        }

        /// <summary>
        /// Reads GeoJson and returns the feature collection.
        /// </summary>
        /// <param name="geoJson"></param>
        /// <returns></returns>
        public static FeatureCollection ToFeatureCollection(this string geoJson)
        {
            var jsonReader = new JsonTextReader(new StringReader(geoJson));
            return GeoJsonConverter.ReadFeatureCollection(jsonReader);
        }

        /// <summary>
        /// Reads GeoJson and returns the feature collection.
        /// </summary>
        /// <param name="jsonReader"></param>
        /// <returns></returns>
        public static FeatureCollection ReadFeatureCollection(JsonReader jsonReader)
        {
            var type = string.Empty;
            List<Feature> features = null;
            while (jsonReader.Read())
            {
                if (jsonReader.TokenType == JsonToken.EndObject)
                { // end of geometry.
                    break;
                }

                if (jsonReader.TokenType == JsonToken.PropertyName)
                {
                    if ((string)jsonReader.Value == "type")
                    { // the geometry type.
                        type = jsonReader.ReadAsString();
                    }
                    else if ((string)jsonReader.Value == "features")
                    { // the geometry.
                        features = GeoJsonConverter.ReadFeatureArray(jsonReader);
                    }
                }
            }
            switch (type)
            {
                case "FeatureCollection":
                    if (features == null)
                    {
                        return new FeatureCollection();
                    }
                    return new FeatureCollection(features);
            }
            throw new Exception("Invalid type.");
        }

        /// <summary>
        /// Reads GeoJson and returns the list of features.
        /// </summary>
        /// <param name="jsonReader"></param>
        /// <returns></returns>
        internal static List<Feature> ReadFeatureArray(JsonReader jsonReader)
        {
            var features = new List<Feature>();
            jsonReader.Read();
            while (jsonReader.TokenType != JsonToken.EndArray)
            {
                var feature = GeoJsonConverter.ReadFeature(jsonReader);
                if (feature == null)
                {
                    return features;
                }
                features.Add(feature);
                jsonReader.Read();
            }
            return features;
        }


        /// <summary>
        /// Generates GeoJson for the given feature collection.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="featureCollection"></param>
        public static void Write(JsonWriter writer, IEnumerable<Feature> featureCollection)
        {
            if (writer == null) { throw new ArgumentNullException("writer"); }
            if (featureCollection == null) { throw new ArgumentNullException("featureCollection"); }

            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue("FeatureCollection");
            writer.WritePropertyName("features");
            writer.WriteStartArray();
            foreach(var feature in featureCollection)
            {
                GeoJsonConverter.Write(writer, feature);
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        /// <summary>
        /// Generates GeoJson for the given feature.
        /// </summary>
        /// <param name="feature"></param>
        public static string ToGeoJson(this Feature feature)
        {
            if (feature == null) { throw new ArgumentNullException("feature"); }

            var jsonWriter = new JTokenWriter();
            GeoJsonConverter.Write(jsonWriter, feature);
            return jsonWriter.Token.ToString();
        }

        /// <summary>
        /// Reads GeoJson and returns the feature.
        /// </summary>
        /// <param name="geoJson"></param>
        /// <returns></returns>
        public static Feature ToFeature(this string geoJson)
        {
            var jsonReader = new JsonTextReader(new StringReader(geoJson));
            return GeoJsonConverter.ReadFeature(jsonReader);
        }

        /// <summary>
        /// Reads GeoJson and returns the feature.
        /// </summary>
        /// <param name="jsonReader"></param>
        /// <returns></returns>
        public static Feature ReadFeature(JsonReader jsonReader)
        {
            var type = string.Empty;
            Geometry geometry = null;
            GeometryAttributeCollection attributes = null;
            while (jsonReader.Read())
            {
                if (jsonReader.TokenType == JsonToken.EndArray)
                {
                    return null;
                }

                if (jsonReader.TokenType == JsonToken.EndObject)
                { // end of geometry.
                    break;
                }

                if (jsonReader.TokenType == JsonToken.PropertyName)
                {
                    if ((string)jsonReader.Value == "type")
                    { // the geometry type.
                        type = jsonReader.ReadAsString();
                    }
                    else if ((string)jsonReader.Value == "geometry")
                    { // the geometry.
                        geometry = GeoJsonConverter.ReadGeometry(jsonReader);
                    }
                    else if ((string)jsonReader.Value == "properties")
                    { // the properties/attributes.
                        attributes = GeoJsonConverter.ReadAttributes(jsonReader);
                    }
                }
            }
            switch(type)
            {
                case "Feature":
                    if(geometry == null)
                    {
                        throw new Exception("No geometry found.");
                    }
                    if(attributes != null)
                    {
                        return new Feature(geometry, attributes);
                    }
                    return new Feature(geometry);
            }
            throw new Exception("Invalid type.");
        }

        /// <summary>
        /// Reads the properties/attributes.
        /// </summary>
        /// <param name="jsonReader"></param>
        /// <returns></returns>
        internal static GeometryAttributeCollection ReadAttributes(JsonReader jsonReader)
        {
            var attributes = new SimpleGeometryAttributeCollection();
            while (jsonReader.Read())
            {
                if (jsonReader.TokenType == JsonToken.EndObject)
                { // end of geometry.
                    break;
                }
                
                if (jsonReader.TokenType == JsonToken.PropertyName)
                {
                    var key = (string)jsonReader.Value;
                    jsonReader.Read();
                    var value = jsonReader.Value;

                    attributes.Add(new GeometryAttribute()
                    {
                        Key = key,
                        Value = value
                    });
                }
            }
            return attributes;
        }

        /// <summary>
        /// Generates GeoJson for the given feature.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="feature"></param>
        public static void Write(JsonWriter writer, Feature feature)
        {
            if (writer == null) { throw new ArgumentNullException("writer"); }
            if (feature == null) { throw new ArgumentNullException("feature"); }

            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue("Feature");
            writer.WritePropertyName("properties");
            GeoJsonConverter.Write(writer, feature.Attributes);
            writer.WritePropertyName("geometry");
            GeoJsonConverter.Write(writer, feature.Geometry);
            writer.WriteEndObject();
        }

        /// <summary>
        /// Generates GeoJson for the given attribute collection.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="attributes"></param>
        internal static void Write(JsonWriter writer, GeometryAttributeCollection attributes)
        {
            if (writer == null) { throw new ArgumentNullException("writer"); }
            if (attributes == null) { throw new ArgumentNullException("attributes"); }

            writer.WriteStartObject();
            var keys = new HashSet<string>();
            foreach (var attribute in attributes)
            {
                if (!keys.Contains(attribute.Key))
                {
                    writer.WritePropertyName(attribute.Key);
                    writer.WriteValue(attribute.Value);
                    keys.Add(attribute.Key);
                }
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Generates GeoJson for the given geometry.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="geometry"></param>
        internal static void Write(JsonWriter writer, Geometry geometry)
        {
            if (writer == null) { throw new ArgumentNullException("writer"); }
            if (geometry == null) { throw new ArgumentNullException("geometry"); }

            if (geometry is LineairRing)
            {
                GeoJsonConverter.Write(writer, geometry as LineairRing);
            }
            else if (geometry is Point)
            {
                GeoJsonConverter.Write(writer, geometry as Point);
            }
            else if (geometry is LineString)
            {
                GeoJsonConverter.Write(writer, geometry as LineString);
            }
            else if (geometry is Polygon)
            {
                GeoJsonConverter.Write(writer, geometry as Polygon);
            }
            else if (geometry is MultiPoint)
            {
                GeoJsonConverter.Write(writer, geometry as MultiPoint);
            }
            else if (geometry is MultiPolygon)
            {
                GeoJsonConverter.Write(writer, geometry as MultiPolygon);
            }
            else if (geometry is MultiLineString)
            {
                GeoJsonConverter.Write(writer, geometry as MultiLineString);
            }
            else if (geometry is GeometryCollection)
            {
                GeoJsonConverter.Write(writer, geometry as GeometryCollection);
            }
            else
            {
                throw new Exception(string.Format("Unknown geometry of type: {0}", geometry.GetType()));
            }
        }

        /// <summary>
        /// Generates GeoJson for the given geometry collection.
        /// </summary>
        /// <param name="geometryCollection"></param>
        public static string ToGeoJson(this MultiPoint geometryCollection)
        {
            if (geometryCollection == null) { throw new ArgumentNullException("geometryCollection"); }

            var jsonWriter = new JTokenWriter();
            GeoJsonConverter.Write(jsonWriter, geometryCollection);
            return jsonWriter.Token.ToString();
        }

        /// <summary>
        /// Generates GeoJson for the given geometry collection.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="geometryCollection"></param>
        internal static void Write(JsonWriter writer, MultiPoint geometryCollection)
        {
            if (writer == null) { throw new ArgumentNullException("writer"); }
            if (geometryCollection == null) { throw new ArgumentNullException("geometryCollection"); }

            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue("MultiPoint");
            writer.WritePropertyName("coordinates");
            writer.WriteStartArray();
            foreach (var point in geometryCollection)
            {
                writer.WriteStartArray();
                writer.WriteValue(point.Coordinate.Longitude);
                writer.WriteValue(point.Coordinate.Latitude);
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        /// <summary>
        /// Generates GeoJson for the given geometry collection.
        /// </summary>
        /// <param name="geometryCollection"></param>
        public static string ToGeoJson(this MultiLineString geometryCollection)
        {
            if (geometryCollection == null) { throw new ArgumentNullException("geometryCollection"); }

            var jsonWriter = new JTokenWriter();
            GeoJsonConverter.Write(jsonWriter, geometryCollection);
            return jsonWriter.Token.ToString();
        }

        /// <summary>
        /// Generates GeoJson for the given geometry collection.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="geometryCollection"></param>
        internal static void Write(JsonWriter writer, MultiLineString geometryCollection)
        {
            if (writer == null) { throw new ArgumentNullException("writer"); }
            if (geometryCollection == null) { throw new ArgumentNullException("geometryCollection"); }

            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue("MultiLineString");
            writer.WritePropertyName("coordinates");
            writer.WriteStartArray();
            foreach (var geometry in geometryCollection)
            {
                writer.WriteStartArray();
                foreach (var coordinate in geometry.Coordinates)
                {
                    writer.WriteStartArray();
                    writer.WriteValue(coordinate.Longitude);
                    writer.WriteValue(coordinate.Latitude);
                    writer.WriteEndArray();
                }
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        /// <summary>
        /// Generates GeoJson for this geometry.
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static string ToGeoJson(this LineairRing geometry)
        {
            if (geometry == null) { throw new ArgumentNullException("geometry"); }

            var jsonWriter = new JTokenWriter();
            GeoJsonConverter.Write(jsonWriter, geometry);
            return jsonWriter.Token.ToString();
        }

        /// <summary>
        /// Generates GeoJson for the given geometry collection.
        /// </summary>
        /// <param name="geometryCollection"></param>
        public static string ToGeoJson(this MultiPolygon geometryCollection)
        {
            if (geometryCollection == null) { throw new ArgumentNullException("geometryCollection"); }

            var jsonWriter = new JTokenWriter();
            GeoJsonConverter.Write(jsonWriter, geometryCollection);
            return jsonWriter.Token.ToString();
        }

        /// <summary>
        /// Generates GeoJson for the given geometry collection.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="geometryCollection"></param>
        internal static void Write(JsonWriter writer, MultiPolygon geometryCollection)
        {
            if (writer == null) { throw new ArgumentNullException("writer"); }
            if (geometryCollection == null) { throw new ArgumentNullException("geometryCollection"); }

            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue("MultiPolygon");
            writer.WritePropertyName("coordinates");
            writer.WriteStartArray();
            foreach (var geometry in geometryCollection)
            {
                writer.WriteStartArray();
                writer.WriteStartArray();
                foreach (var coordinate in geometry.Ring.Coordinates)
                {
                    writer.WriteStartArray();
                    writer.WriteValue(coordinate.Longitude);
                    writer.WriteValue(coordinate.Latitude);
                    writer.WriteEndArray();
                }
                writer.WriteEndArray();
                foreach (var hole in geometry.Holes)
                {
                    writer.WriteStartArray();
                    foreach (var coordinate in hole.Coordinates)
                    {
                        writer.WriteStartArray();
                        writer.WriteValue(coordinate.Longitude);
                        writer.WriteValue(coordinate.Latitude);
                        writer.WriteEndArray();
                    }
                    writer.WriteEndArray();
                }
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        /// <summary>
        /// Generates GeoJson for the given geometry.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="geometry"></param>
        /// <returns></returns>
        internal static void Write(JsonWriter writer, LineairRing geometry)
        {
            if (writer == null) { throw new ArgumentNullException("writer"); }
            if (geometry == null) { throw new ArgumentNullException("geometry"); }

            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue("Polygon");
            writer.WritePropertyName("coordinates");
            writer.WriteStartArray();
            writer.WriteStartArray();
            foreach(var coordinate in geometry.Coordinates)
            {
                writer.WriteStartArray();
                writer.WriteValue(coordinate.Longitude);
                writer.WriteValue(coordinate.Latitude);
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        /// <summary>
        /// Generates GeoJson for this geometry.
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static string ToGeoJson(this Polygon geometry)
        {
            if (geometry == null) { throw new ArgumentNullException("geometry"); }

            var jsonWriter = new JTokenWriter();
            GeoJsonConverter.Write(jsonWriter, geometry);
            return jsonWriter.Token.ToString();
        }

        /// <summary>
        /// Generates GeoJson for the given geometry.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="geometry"></param>
        /// <returns></returns>
        internal static void Write(JsonWriter writer, Polygon geometry)
        {
            if (writer == null) { throw new ArgumentNullException("writer"); }
            if (geometry == null) { throw new ArgumentNullException("geometry"); }

            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue("Polygon");
            writer.WritePropertyName("coordinates");
            writer.WriteStartArray();
            writer.WriteStartArray();
            foreach (var coordinate in geometry.Ring.Coordinates)
            {
                writer.WriteStartArray();
                writer.WriteValue(coordinate.Longitude);
                writer.WriteValue(coordinate.Latitude);
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
            foreach(var hole in geometry.Holes)
            {
                writer.WriteStartArray();
                foreach (var coordinate in hole.Coordinates)
                {
                    writer.WriteStartArray();
                    writer.WriteValue(coordinate.Longitude);
                    writer.WriteValue(coordinate.Latitude);
                    writer.WriteEndArray();
                }
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        /// <summary>
        /// Generates GeoJson for this geometry.
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static string ToGeoJson(this LineString geometry)
        {
            if (geometry == null) { throw new ArgumentNullException("geometry"); }

            var jsonWriter = new JTokenWriter();
            GeoJsonConverter.Write(jsonWriter, geometry);
            return jsonWriter.Token.ToString();
        }

        /// <summary>
        /// Generates GeoJson for the given geometry.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="geometry"></param>
        /// <returns></returns>
        internal static void Write(JsonWriter writer, LineString geometry)
        {
            if (writer == null) { throw new ArgumentNullException("writer"); }
            if (geometry == null) { throw new ArgumentNullException("geometry"); }

            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue("LineString");
            writer.WritePropertyName("coordinates");
            writer.WriteStartArray();
            foreach (var coordinate in geometry.Coordinates)
            {
                writer.WriteStartArray();
                writer.WriteValue(coordinate.Longitude);
                writer.WriteValue(coordinate.Latitude);
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        /// <summary>
        /// Generates GeoJson for this geometry.
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static string ToGeoJson(this Point geometry)
        {
            if (geometry == null) { throw new ArgumentNullException("geometry"); }

            var jsonWriter = new JTokenWriter();
            GeoJsonConverter.Write(jsonWriter, geometry);
            return jsonWriter.Token.ToString();
        }

        /// <summary>
        /// Generates GeoJson for the given geometry.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="geometry"></param>
        /// <returns></returns>
        internal static void Write(JsonWriter writer, Point geometry)
        {
            if (writer == null) { throw new ArgumentNullException("writer"); }
            if (geometry == null) { throw new ArgumentNullException("geometry"); }

            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue("Point");
            writer.WritePropertyName("coordinates");
            writer.WriteStartArray();
            writer.WriteValue(geometry.Coordinate.Longitude);
            writer.WriteValue(geometry.Coordinate.Latitude);
            writer.WriteEndArray();
            writer.WriteEndObject();
        } 
        
        /// <summary>
        /// Reads GeoJson and returns the geometry.
        /// </summary>
        /// <param name="geoJson"></param>
        /// <returns></returns>
        public static Geometry ToGeometry(this string geoJson)
        {
            var jsonReader = new JsonTextReader(new StringReader(geoJson));
            return GeoJsonConverter.ReadGeometry(jsonReader);
        }

        /// <summary>
        /// Reads GeoJson and returns the geometry.
        /// </summary>
        /// <param name="jsonReader"></param>
        /// <returns></returns>
        internal static Geometry ReadGeometry(JsonReader jsonReader)
        {
            var geometryType = string.Empty;
            var coordinates = new List<object>();
            List<Geometry> geometries = null;
            while (jsonReader.Read())
            {
                if (jsonReader.TokenType == JsonToken.EndObject)
                { // end of geometry.
                    break;
                }

                if (jsonReader.TokenType == JsonToken.PropertyName)
                {
                    if ((string)jsonReader.Value == "type")
                    { // the geometry type.
                        geometryType = jsonReader.ReadAsString();
                    }
                    else if ((string)jsonReader.Value == "geometries")
                    { // the geometries if a geometry collection.
                        geometries = GeoJsonConverter.ReadGeometryArray(jsonReader);
                    }
                    else if ((string)jsonReader.Value == "coordinates")
                    { // the coordinates.
                        jsonReader.Read(); // move to first array start.
                        coordinates = GeoJsonConverter.ReadCoordinateArrays(jsonReader);
                    }
                }
            }

            // data has been read, instantiate the actual object.
            switch(geometryType)
            {
                case "Point":
                    return GeoJsonConverter.BuildPoint(coordinates);
                case "LineString":
                    return GeoJsonConverter.BuildLineString(coordinates);
                case "Polygon":
                    return GeoJsonConverter.BuildPolygon(coordinates);
                case "MultiPoint":
                    return GeoJsonConverter.BuildMultiPoint(coordinates);
                case "MultiLineString":
                    return GeoJsonConverter.BuildMultiLineString(coordinates);
                case "MultiPolygon":
                    return GeoJsonConverter.BuildMultiPolygon(coordinates);
                case "GeometryCollection":
                    return GeoJsonConverter.BuildGeometryCollection(geometries);

            }
            throw new Exception(string.Format("Unknown geometry type: {0}", geometryType));
        }

        /// <summary>
        /// Reads GeoJson and returns the list of geometries..
        /// </summary>
        /// <param name="jsonReader"></param>
        /// <returns></returns>
        internal static List<Geometry> ReadGeometryArray(JsonReader jsonReader)
        {
            var geometries = new List<Geometry>();
            jsonReader.Read();
            while(jsonReader.TokenType != JsonToken.EndArray)
            {
                geometries.Add(GeoJsonConverter.ReadGeometry(jsonReader));
                jsonReader.Read();
            }
            return geometries;
        }

        /// <summary>
        /// Reads GeoJson and returns a list of coordinates.
        /// </summary>
        /// <param name="jsonReader"></param>
        /// <returns></returns>
        internal static List<object> ReadCoordinateArrays(JsonReader jsonReader)
        {
            var coordinates = new List<object>();
            jsonReader.Read(); // move to next array start.
            if (jsonReader.TokenType == JsonToken.StartArray)
            {
                while (jsonReader.TokenType == JsonToken.StartArray)
                {
                    coordinates.Add(GeoJsonConverter.ReadCoordinateArrays(jsonReader));
                    jsonReader.Read(); // move to next array start.
                }
            }
            else if(jsonReader.TokenType == JsonToken.Float) 
            {
                while(jsonReader.TokenType != JsonToken.EndArray)
                {
                    coordinates.Add((double)jsonReader.Value);
                    jsonReader.Read();
                }
            }
            else
            {
                throw new Exception(string.Format("Invalid token in coordinates array: {0}", jsonReader.TokenType.ToInvariantString()));
            }
            return coordinates;
        }

        /// <summary>
        /// Builds the geometry from the given coordinates.
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        internal static Point BuildPoint(List<object> coordinates)
        {
            if (coordinates == null) { throw new ArgumentNullException(); }
            if (coordinates != null &&
                coordinates.Count == 2 &&
                coordinates[0] is double &&
                coordinates[1] is double)
            {
                return new Point(new Math.Geo.GeoCoordinate(
                    (double)coordinates[1], (double)coordinates[0]));
            }
            throw new Exception("Invalid coordinate collection.");
        }

        /// <summary>
        /// Builds the geometry from the given coordinates.
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        internal static LineString BuildLineString(List<object> coordinates)
        {
            if (coordinates == null) { throw new ArgumentNullException(); }
            if (coordinates.Count > 1)
            {
                var lineStringCoordinates = new List<GeoCoordinate>();
                for (int idx = 0; idx < coordinates.Count; idx++)
                {
                    var pointCoordinate = coordinates[idx] as List<object>;
                    if (pointCoordinate != null &&
                        pointCoordinate.Count == 2 &&
                        pointCoordinate[0] is double &&
                        pointCoordinate[1] is double)
                    {
                        lineStringCoordinates.Add(new Math.Geo.GeoCoordinate(
                            (double)pointCoordinate[1], (double)pointCoordinate[0]));
                    }
                }
                return new LineString(lineStringCoordinates);
            }
            throw new Exception("Invalid coordinate collection.");
        }

        /// <summary>
        /// Builds the geometry from the given coordinates.
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        internal static Polygon BuildPolygon(List<object> coordinates)
        {
            if (coordinates == null) { throw new ArgumentNullException(); }
            if (coordinates.Count >= 1)
            {
                var polygonCoordinates = new List<List<GeoCoordinate>>();
                foreach (List<object> coordinates1 in coordinates)
                {
                    var lineStringCoordinates = new List<GeoCoordinate>();
                    for (int idx = 0; idx < coordinates1.Count; idx++)
                    {
                        var pointCoordinate = coordinates1[idx] as List<object>;
                        if (pointCoordinate != null &&
                            pointCoordinate.Count == 2 &&
                            pointCoordinate[0] is double &&
                            pointCoordinate[1] is double)
                        {
                            lineStringCoordinates.Add(new Math.Geo.GeoCoordinate(
                                (double)pointCoordinate[1], (double)pointCoordinate[0]));
                        }
                    }
                    polygonCoordinates.Add(lineStringCoordinates);
                }

                var outer = new LineairRing(polygonCoordinates[0]);
                var holes = new List<LineairRing>();
                for (int idx = 1; idx < polygonCoordinates.Count; idx++)
                {
                    holes.Add(new LineairRing(polygonCoordinates[idx]));
                }
                return new Polygon(outer, holes);
            }
            throw new Exception("Invalid coordinate collection.");
        }

        /// <summary>
        /// Builds the geometry from the given coordinates.
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        internal static MultiPoint BuildMultiPoint(List<object> coordinates)
        {
            if (coordinates == null) { throw new ArgumentNullException(); }
            if (coordinates.Count > 1)
            {
                var multiPointCoordinates = new List<GeoCoordinate>();
                for (int idx = 0; idx < coordinates.Count; idx++)
                {
                    var pointCoordinate = coordinates[idx] as List<object>;
                    if (pointCoordinate != null &&
                        pointCoordinate.Count == 2 &&
                        pointCoordinate[0] is double &&
                        pointCoordinate[1] is double)
                    {
                        multiPointCoordinates.Add(new Math.Geo.GeoCoordinate(
                            (double)pointCoordinate[1], (double)pointCoordinate[0]));
                    }
                }
                var points = new List<Point>();
                foreach(var pointCoordinate in multiPointCoordinates)
                {
                    points.Add(new Point(pointCoordinate));
                }
                return new MultiPoint(points);
            }
            throw new Exception("Invalid coordinate collection.");
        }

        /// <summary>
        /// Builds the geometry from the given coordinates.
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        internal static MultiLineString BuildMultiLineString(List<object> coordinates)
        {
            if (coordinates == null) { throw new ArgumentNullException(); }
            if (coordinates.Count >= 1)
            {
                var multiLineStringCoordinates = new List<List<GeoCoordinate>>();
                foreach (List<object> coordinates1 in coordinates)
                {
                    var lineStringCoordinates = new List<GeoCoordinate>();
                    for (int idx = 0; idx < coordinates1.Count; idx++)
                    {
                        var pointCoordinate = coordinates1[idx] as List<object>;
                        if (pointCoordinate != null &&
                            pointCoordinate.Count == 2 &&
                            pointCoordinate[0] is double &&
                            pointCoordinate[1] is double)
                        {
                            lineStringCoordinates.Add(new Math.Geo.GeoCoordinate(
                                (double)pointCoordinate[1], (double)pointCoordinate[0]));
                        }
                    }
                    multiLineStringCoordinates.Add(lineStringCoordinates);
                }

                var multiLineStrings = new List<LineString>();
                for (int idx = 0; idx < multiLineStringCoordinates.Count; idx++)
                {
                    multiLineStrings.Add(new LineString(multiLineStringCoordinates[idx]));
                }
                return new MultiLineString(multiLineStrings);
            }
            throw new Exception("Invalid coordinate collection.");
        }

        /// <summary>
        /// Builds the geometry from the given coordinates.
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        internal static MultiPolygon BuildMultiPolygon(List<object> coordinates)
        {
            if (coordinates == null) { throw new ArgumentNullException(); }
            if (coordinates.Count >= 1)
            {
                var polygons = new List<Polygon>();
                foreach (List<object> coordinates1 in coordinates)
                {
                    polygons.Add(GeoJsonConverter.BuildPolygon(coordinates1));
                }
                return new MultiPolygon(polygons);
            }
            throw new Exception("Invalid coordinate collection.");
        }

        /// <summary>
        /// Builds the geometry from the given geometries.
        /// </summary>
        /// <param name="geometries"></param>
        /// <returns></returns>
        internal static GeometryCollection BuildGeometryCollection(List<Geometry> geometries)
        {
            return new GeometryCollection(geometries);
        }

        /// <summary>
        /// Generates GeoJson for the given geometry collection.
        /// </summary>
        /// <param name="geometryCollection"></param>
        public static string ToGeoJson(this GeometryCollection geometryCollection)
        {
            if (geometryCollection == null) { throw new ArgumentNullException("geometryCollection"); }

            var jsonWriter = new JTokenWriter();
            GeoJsonConverter.Write(jsonWriter, geometryCollection);
            return jsonWriter.Token.ToString();
        }

        /// <summary>
        /// Generates GeoJson for the given geometry collection.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="geometryCollection"></param>
        internal static void Write(JsonWriter writer, GeometryCollection geometryCollection)
        {
            if (writer == null) { throw new ArgumentNullException("writer"); }
            if (geometryCollection == null) { throw new ArgumentNullException("geometryCollection"); }

            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue("GeometryCollection");
            writer.WritePropertyName("geometries");
            writer.WriteStartArray();
            foreach (var geometry in geometryCollection)
            {
                GeoJsonConverter.Write(writer, geometry);
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }
}