using Newtonsoft.Json;

namespace RaindropAutomations.Models
{
    public class Bookmark
    {
        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("collection")]
        public Collection Collection { get; set; }

        [JsonProperty("pleaseParse")]
        public object PleaseParse = new object();
    }
  
    public class Collection
    {
        [JsonProperty("$id")]
        public int Id { get; set; }
    }

}
