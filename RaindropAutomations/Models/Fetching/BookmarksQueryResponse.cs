using Newtonsoft.Json;

namespace RaindropAutomations.Models.Fetching
{
    public class BookmarksQueryResponse
    {
        [JsonProperty("items")]
        public List<BookmarkFetchModel> Items { get; set; } = [];

        [JsonProperty("result")]
        public bool Result { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }
    }
}

