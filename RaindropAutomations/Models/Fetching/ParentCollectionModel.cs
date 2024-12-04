using System.Text.Json.Serialization;

namespace RaindropAutomations.Models.Fetching
{
    public class ParentCollectionModel
    {
        [JsonPropertyName("$id")]
        public long Id { get; set; }

        [JsonPropertyName("$ref")]
        public string Ref { get; set; }
    }
}