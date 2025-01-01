using Newtonsoft.Json;

namespace RaindropAutomations.Models.Fetching
{
    public class SingleCollectionPayload
    {
        [JsonProperty("result")]
        public bool Result { get; set; }

        [JsonProperty("item")]
        public CollectionFetchModel Item { get; set; }
    }
}