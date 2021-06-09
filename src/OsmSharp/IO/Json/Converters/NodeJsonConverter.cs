using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OsmSharp.API;
using OsmSharp.Tags;

namespace OsmSharp.IO.Json.Converters
{
    public class NodeJsonConverter : JsonConverter<Node>
    {
        private readonly OsmGeoJsonConverter _osmGeoJsonConverter = new OsmGeoJsonConverter();
        public override Node Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return _osmGeoJsonConverter.Read(ref reader, typeToConvert, options) as Node;
        }

        public override void Write(Utf8JsonWriter writer, Node value, JsonSerializerOptions options)
        {
            _osmGeoJsonConverter.Write(writer, value, options);
        }
    }
}