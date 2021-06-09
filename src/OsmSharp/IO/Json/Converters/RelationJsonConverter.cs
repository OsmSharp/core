using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OsmSharp.IO.Json.Converters
{
    public class RelationJsonConverter : JsonConverter<Relation>
    {
        private readonly OsmGeoJsonConverter _osmGeoJsonConverter = new OsmGeoJsonConverter();
        
        public override Relation Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return _osmGeoJsonConverter.Read(ref reader, typeToConvert, options) as Relation;
        }

        public override void Write(Utf8JsonWriter writer, Relation value, JsonSerializerOptions options)
        {
            _osmGeoJsonConverter.Write(writer, value, options);
        }
    }
}