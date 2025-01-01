using Newtonsoft.Json;

namespace RaindropAutomations.Models.Saving
{
    public class BookmarkSaveModel
    {
        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("collection")]
        public CollectionIdSaveModel Collection { get; set; }

        [JsonProperty("pleaseParse")]
        public object PleaseParse = new();

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        [JsonProperty("important")]
        public bool Important { get; set; }

        [JsonProperty("media")]
        public List<Object> Media { get; set; }
    }

}
