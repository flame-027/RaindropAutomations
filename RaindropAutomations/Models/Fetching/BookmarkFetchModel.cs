using System.Text.Json.Serialization;

namespace RaindropAutomations.Models.Fetching
{
    public class BookmarkFetchModel
    {
        [JsonPropertyName("_id")]
        public long Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("link")]
        public string Link { get; set; }

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; }
    }
}

