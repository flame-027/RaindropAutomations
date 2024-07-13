using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RaindropAutomations.models
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
