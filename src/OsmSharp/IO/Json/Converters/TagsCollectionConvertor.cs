using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OsmSharp.Tags;

namespace OsmSharp.IO.Json.Converters
{
    public sealed class TagsCollectionConvertor : JsonConverter<TagsCollectionBase>
    {
        public override TagsCollectionBase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            var tagsCollection = new TagsCollection();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return tagsCollection;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var key = reader.GetString();
                    reader.Read();
                    var value = reader.GetString();
                    
                    tagsCollection.AddOrReplace(key, value);
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, TagsCollectionBase value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type typeToConvert)
        {
            if (base.CanConvert(typeToConvert)) return true;

            if (typeof(TagsCollection) == typeToConvert) return true;

            return false;
        }
    }
}