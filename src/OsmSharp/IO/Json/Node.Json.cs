using System.Text.Json.Serialization;
using OsmSharp.IO.Json.Converters;

namespace OsmSharp
{
    [JsonConverter(typeof(NodeJsonConverter))]
    public partial class Node
    {
        
    }
}