using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OsmSharp.Tags;

namespace OsmSharp.IO.Json.Converters
{
    public class NodeJsonConverter : JsonConverter<Node>
    {
        public override Node Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            var node = new Node();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return node;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName)
                    {
                        case "id":
                            node.Id = reader.GetInt64();
                            break;
                        case "lat":
                            node.Latitude = reader.GetDouble();
                            break;
                        case "lon":
                            node.Longitude = reader.GetDouble();
                            break;
                        case "version":
                            node.Version = reader.GetInt32();
                            break;
                        case "timestamp":
                            node.TimeStamp = reader.GetDateTime();
                            break;
                        case "changeset":
                            node.ChangeSetId = reader.GetInt64();
                            break;
                        case "user":
                            node.UserName = reader.GetString();
                            break;
                        case "uid":
                            node.UserId = reader.GetInt64();
                            break;
                        case "tags":
                            node.Tags = JsonSerializer.Deserialize<TagsCollectionBase>(ref reader);
                            break;
                    }
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, Node value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}