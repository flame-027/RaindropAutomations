using System.Text.Json.Serialization;

namespace RaindropAutomations.Models.Saving
{
    public class MultiBookmarkModifyModel
    {
        [JsonPropertyName("ids")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]

        public List<long> Ids { get; set; }

        [JsonPropertyName("collection")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]

        public CollectionIdSaveModel Collection { get; set; }

        [JsonPropertyName("cover")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]

        public string CoverUrl { get; set; }

        [JsonPropertyName("tags")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]

        public List<string> Tags { get; set; }  // DO NOT INTIALIZE AS EMPTY ARRAY WILL WIPE EXISTING DATA

        [JsonPropertyName("important")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]

        public bool? Important { get; set; }

        [JsonPropertyName("media")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]

        public List<Object> Media { get; set; }  // DO NOT INTIALIZE AS EMPTY ARRAY WILL WIPE EXISTING DATA
    }

}
