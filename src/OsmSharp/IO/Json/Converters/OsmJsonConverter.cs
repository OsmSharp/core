using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using OsmSharp.API;

namespace OsmSharp.IO.Json.Converters
{
    public class OsmJsonConverter : JsonConverter<Osm>
    {
        private readonly OsmGeoJsonConverter _osmGeoJsonConverter = new OsmGeoJsonConverter();
        
        public override Osm Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            var osm = new Osm();
            List<Node> nodes = null;
            List<Way> ways = null;
            List<Relation> relations = null;
            
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    osm.Nodes = nodes?.ToArray();
                    osm.Ways = ways?.ToArray();
                    osm.Relations = relations?.ToArray();

                    return osm;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read();

                    switch (propertyName)
                    {
                        case "version":
                            osm.Version = reader.GetDouble();
                            break;
                        case "generator":
                            osm.Generator = reader.GetString();
                            break;
                        case "elements":
                            reader.Read();
                            while (reader.TokenType != JsonTokenType.EndArray)
                            {
                                var osmGeo = _osmGeoJsonConverter.Read(ref reader, typeof(OsmGeo), options);

                                switch (osmGeo)
                                {
                                    case Node n:
                                        nodes ??= new List<Node>();
                                        nodes.Add(n);
                                        break;
                                    case Way w:
                                        ways ??= new List<Way>();
                                        ways.Add(w);
                                        break;
                                    case Relation r:
                                        relations ??= new List<Relation>();
                                        relations.Add(r);
                                        break;
                                }
                            }
                            break;
                    }
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, Osm value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            
            if (value.Version.HasValue) writer.WriteNumber("version", value.Version.Value);
            if (value.Generator != null) writer.WriteString("generator", value.Generator);

            if (value.Nodes != null || value.Ways != null || value.Relations != null)
            {
                writer.WritePropertyName("elements");
                
                writer.WriteStartArray();
                
                if (value.Nodes != null)
                {
                    foreach (var n in value.Nodes)
                    {
                        _osmGeoJsonConverter.Write(writer, n, options);
                    }
                }
                
                if (value.Ways != null)
                {
                    foreach (var w in value.Ways)
                    {
                        _osmGeoJsonConverter.Write(writer, w, options);
                    }
                }
                
                if (value.Relations != null)
                {
                    foreach (var r in value.Relations)
                    {
                        _osmGeoJsonConverter.Write(writer, r, options);
                    }
                }
                
                writer.WriteEndArray();
            }
            
            writer.WriteEndObject();
        }
    }
}