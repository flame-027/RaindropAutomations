using Newtonsoft.Json;

namespace RaindropAutomations.Models.Fetching
{
    public class ParentCollectionModel
    {
        [JsonProperty("$id")]
        public long Id { get; set; }

        [JsonProperty("$ref")]
        public string Ref { get; set; }
    }
}