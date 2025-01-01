using Newtonsoft.Json;

namespace RaindropAutomations.Models.Saving
{
    public class MultiBookmarkSavePayload
    {
        [JsonProperty("result")]
        public bool Result { get; set; }

        [JsonProperty("items")]
        public List<BookmarkSaveModel> Bookmarks { get; set; }
    }
}
