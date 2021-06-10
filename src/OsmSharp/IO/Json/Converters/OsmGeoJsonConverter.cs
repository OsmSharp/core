using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using OsmSharp.Tags;

namespace OsmSharp.IO.Json.Converters
{
    public class OsmGeoJsonConverter : JsonConverter<OsmGeo>
    {
        public override OsmGeo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            long? id = null, changeSetId = null, userId = null;
            double? lat = null, lon = null;
            int? version = null;
            DateTime? timestamp = null;
            OsmGeoType? type = null;
            string userName = null;
            TagsCollectionBase tags = null;
            var visible = true;
            List<long> nodes = null;
            List<RelationMember> members = null;
            
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    reader.Read();
                    if (type == null) throw new InvalidDataException("Type not found.");
                    
                    // create the appropriate object.
                    OsmGeo osmGeo = type.Value switch
                    {
                        OsmGeoType.Node => new Node(),
                        OsmGeoType.Way => new Way(),
                        OsmGeoType.Relation => new Relation(),
                        _ => throw new InvalidDataException("Unknown type.")
                    };
                    osmGeo.Id = id;
                    osmGeo.Tags = tags;
                    osmGeo.Version = version;
                    osmGeo.Visible = visible;
                    osmGeo.TimeStamp = timestamp;
                    osmGeo.UserId = userId;
                    osmGeo.UserName = userName;
                    osmGeo.ChangeSetId = changeSetId;

                    switch (osmGeo)
                    {
                        case Node node:
                            node.Latitude = lat;
                            node.Longitude = lon;
                            return node;
                        case Way way:
                            way.Nodes = nodes?.ToArray();
                            return way;
                        case Relation relation:
                            relation.Members = members?.ToArray();
                            return relation;
                    }
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read();

                    switch (propertyName)
                    {
                        case "type":
                            type = reader.GetString() switch
                            {
                                "node" => OsmGeoType.Node,
                                "way" => OsmGeoType.Way,
                                "relation" => OsmGeoType.Relation,
                                _ => throw new InvalidDataException("Unknown type.")
                            };
                            break;
                        case "id":
                            id = reader.GetInt64();
                            break;
                        case "lat":
                            lat = reader.GetDouble();
                            break;
                        case "lon":
                            lon = reader.GetDouble();
                            break;
                        case "version":
                            version = reader.GetInt32();
                            break;
                        case "timestamp":
                            timestamp = reader.GetDateTime();
                            break;
                        case "changeset":
                            changeSetId = reader.GetInt64();
                            break;
                        case "user":
                            userName = reader.GetString();
                            break;
                        case "uid":
                            userId = reader.GetInt64();
                            break;
                        case "tags":
                            tags = JsonSerializer.Deserialize<TagsCollectionBase>(ref reader);
                            break;
                        case "nodes":
                            nodes = new List<long>();
                            reader.Read();
                            while (reader.TokenType != JsonTokenType.EndArray)
                            {
                                nodes.Add(reader.GetInt64());
                                reader.Read();
                            }
                            break;
                        case "members":
                            members = new List<RelationMember>();
                            
                            reader.Read();
                            while (reader.TokenType != JsonTokenType.EndArray)
                            {
                                reader.Read();
                                var member = new RelationMember();
                                while (reader.TokenType != JsonTokenType.EndObject)
                                {
                                    var memberPropertyName = reader.GetString();
                                    reader.Read();
                                    switch (memberPropertyName)
                                    {
                                        case "type":
                                            member.Type = reader.GetString() switch
                                            {
                                                "node" => OsmGeoType.Node,
                                                "way" => OsmGeoType.Way,
                                                "relation" => OsmGeoType.Relation,
                                                _ => member.Type
                                            };
                                            break;
                                        case "ref":
                                            member.Id = reader.GetInt64();
                                            break;
                                        case "role":
                                            member.Role = reader.GetString();
                                            break;
                                    }
                                    reader.Read();
                                }
                                members.Add(member);
                                reader.Read();
                            }
                            break;
                    }
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, OsmGeo value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            switch (value)
            {
                case Node n:
                    writer.WriteString("type", "node");
                    if (n.Latitude.HasValue) writer.WriteNumber("lat", n.Latitude.Value);
                    if (n.Longitude.HasValue) writer.WriteNumber("lon", n.Longitude.Value);
                    break;
                case Way w:
                    writer.WriteString("type", "way");
                    if (w.Nodes != null)
                    {
                        writer.WritePropertyName("nodes");
                        writer.WriteStartArray();

                        foreach (var nid in w.Nodes)
                        {
                            writer.WriteNumberValue(nid);   
                        }
                        
                        writer.WriteEndArray();
                    }
                    break;
                case Relation r:
                    writer.WriteString("type", "relation");
                    if (r.Members != null)
                    {
                        writer.WritePropertyName("members");
                        writer.WriteStartArray();

                        foreach (var member in r.Members)
                        {
                            writer.WriteStartObject();
                            
                            switch (member.Type)
                            {
                                case OsmGeoType.Node:
                                    writer.WriteString("type", "node");
                                    break;
                                case OsmGeoType.Way:
                                    writer.WriteString("type", "way");
                                    break;
                                case OsmGeoType.Relation:
                                    writer.WriteString("type", "relation");
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            writer.WriteNumber("ref", member.Id);
                            writer.WriteString("role", member.Role);
                            
                            writer.WriteEndObject();
                        }
                        
                        writer.WriteEndArray();
                    }
                    break;
            }
            
            if (value.Id.HasValue) writer.WriteNumber("id", value.Id.Value);
            if (value.Tags != null)
            {
                writer.WritePropertyName("tags");
                JsonSerializer.Serialize(writer, value.Tags);
            }
            if (value.TimeStamp.HasValue) writer.WriteString("timestamp", value.TimeStamp.Value);
            if (value.Version.HasValue) writer.WriteNumber("version", value.Version.Value);
            if (value.ChangeSetId.HasValue) writer.WriteNumber("changeset", value.ChangeSetId.Value);
            if (value.UserName != null) writer.WriteString("user", value.UserName);
            if (value.UserId.HasValue) writer.WriteNumber("uid", value.UserId.Value);
            
            writer.WriteEndObject();
        }
    }
}