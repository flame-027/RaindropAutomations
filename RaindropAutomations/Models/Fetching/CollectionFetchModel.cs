using System.Text.Json.Serialization;

namespace RaindropAutomations.Models.Fetching
{
    public class CollectionFetchModel
    {
        [JsonPropertyName("_id")]
        public long Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("parent")]
        public ParentCollectionModel Parent { get; set; }

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