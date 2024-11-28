using System.Text.Json.Serialization;

namespace RaindropAutomations.models
{
    public class RaindropParentCollection
    {
        [JsonPropertyName("$id")]
        public long Id { get; set; }

        [JsonPropertyName("$ref")]
        public string Ref { get; set; }
    }
}