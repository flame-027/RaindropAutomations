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
        public List<string> Tags { get; set; } = [];

        [JsonPropertyName("created")]
        public DateTime CreatedOn { get; set; }

        [JsonPropertyName("lastUpdate")]
        public DateTime LastUpdateOn { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("excerpt")]
        public string Excerpt { get; set; }

        [JsonPropertyName("domain")]
        public string Domain { get; set; }

        [JsonPropertyName("note")]
        public string Note { get; set; }

        [JsonPropertyName("cover")]
        public string CoverUrl { get; set; }

        [JsonPropertyName("media")]
        public List<Object> Media { get; set; }
    }
}

