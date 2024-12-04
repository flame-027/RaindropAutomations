using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaindropAutomations.Models
{
    public class BookmarksCreationPayload
    {
        [JsonProperty("result")]
        public bool Result { get; set; }

        [JsonProperty("items")]
        public List<Bookmark> Bookmarks { get; set; }
    }
}
