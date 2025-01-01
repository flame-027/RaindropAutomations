using Newtonsoft.Json;

namespace RaindropAutomations.Models.Fetching
{
    public class MultiCollectionPayload
    {
        [JsonProperty("result")]
        public bool Result { get; set; }

        [JsonProperty("items")]
        public List<CollectionFetchModel> Items { get; set; }
    }
}