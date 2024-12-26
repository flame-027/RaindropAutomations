using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace RaindropAutomations.Models.Saving
{
    public class BookmarkSaveModel
    {
        [JsonPropertyName("link")]
        public string Link { get; set; }

        [JsonPropertyName("collection")]
        public CollectionIdSaveModel Collection { get; set; }

        [JsonPropertyName("pleaseParse")]
        public object PleaseParse = new();

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; }

        [JsonPropertyName("important")]
        public bool Important { get; set; }

        [JsonPropertyName("media")]
        public List<Object> Media { get; set; }
    }

}
