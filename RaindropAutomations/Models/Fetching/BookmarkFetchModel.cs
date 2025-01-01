using Newtonsoft.Json;

namespace RaindropAutomations.Models.Fetching
{
    public class BookmarkFetchModel
    {
        [JsonProperty("_id")]
        public long Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; } = [];

        [JsonProperty("created")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("lastUpdate")]
        public DateTime LastUpdateOn { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("excerpt")]
        public string Excerpt { get; set; }

        [JsonProperty("domain")]
        public string Domain { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }

        [JsonProperty("cover")]
        public string CoverUrl { get; set; }

        [JsonProperty("media")]
        public List<Object> Media { get; set; }
    }
}

