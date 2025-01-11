using Newtonsoft.Json;

namespace RaindropAutomations.Models.Fetching
{
    public class SingleCollectionPayload : ICollectionScope
    {
        [JsonProperty("result")]
        public bool Result { get; set; }

        [JsonProperty("item")]
        public CollectionFetchModel Item { get; set; }

        public List<long> AllIds
        {
            get
            {
                if (Item != null)
                    return [Item.Id];
                else
                    return [];
            }
        }
    }
}