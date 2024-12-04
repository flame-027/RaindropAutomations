using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
