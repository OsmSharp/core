using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OsmSharp.IO.Json.Converters
{
    public class WayJsonConverter : JsonConverter<Way>
    {
        private readonly OsmGeoJsonConverter _osmGeoJsonConverter = new OsmGeoJsonConverter();
        
        public override Way Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return _osmGeoJsonConverter.Read(ref reader, typeToConvert, options) as Way;
        }

        public override void Write(Utf8JsonWriter writer, Way value, JsonSerializerOptions options)
        {
            _osmGeoJsonConverter.Write(writer, value, options);
        }
    }
}