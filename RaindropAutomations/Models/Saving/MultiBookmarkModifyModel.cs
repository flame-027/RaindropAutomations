using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace RaindropAutomations.Models.Saving
{
    public class MultiBookmarkModifyModel
    {
        [JsonProperty("ids", NullValueHandling = NullValueHandling.Ignore)]

        public List<long> Ids { get; set; }

        [JsonProperty("collection", NullValueHandling = NullValueHandling.Ignore)]

        public CollectionIdSaveModel Collection { get; set; }

        [JsonProperty("cover", NullValueHandling = NullValueHandling.Ignore)]

        public string CoverUrl { get; set; }

        [JsonProperty("tags", NullValueHandling = NullValueHandling.Ignore)]

        public List<string> Tags { get; set; }  // DO NOT INTIALIZE AS EMPTY ARRAY WILL WIPE EXISTING DATA

        [JsonProperty("important", NullValueHandling = NullValueHandling.Ignore)]

        public bool? Important { get; set; }

        [JsonProperty("media", NullValueHandling = NullValueHandling.Ignore)]

        public List<Object> Media { get; set; }  // DO NOT INTIALIZE AS EMPTY ARRAY WILL WIPE EXISTING DATA
    }

}
