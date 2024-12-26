using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RaindropAutomations.Models.Fetching
{
    public class BookmarksQueryResponse
    {
        [JsonPropertyName("items")]
        public List<BookmarkFetchModel> Items { get; set; } = [];

        [JsonPropertyName("result")]
        public bool Result { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}

