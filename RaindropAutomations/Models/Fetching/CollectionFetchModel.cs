using Newtonsoft.Json;

namespace RaindropAutomations.Models.Fetching
{
    public class CollectionFetchModel
    {
        [JsonProperty("_id")]
        public long Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("parent")]
        public ParentCollectionModel Parent { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }

        [JsonProperty("lastUpdate")]
        public DateTime LastUpdate { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("view")]
        public string View { get; set; }
    }
}