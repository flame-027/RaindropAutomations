using System.Text.Json.Serialization;

namespace RaindropAutomations.models
{
    public class RaindropCollectionItem
    {
        [JsonPropertyName("_id")]
        public long Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("parent")]
        public RaindropParentCollection Parent { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("created")]
        public DateTime Created { get; set; }

        [JsonPropertyName("lastUpdate")]
        public DateTime LastUpdate { get; set; }

        [JsonPropertyName("color")]
        public string Color { get; set; }

        [JsonPropertyName("view")]
        public string View { get; set; }
    }
}