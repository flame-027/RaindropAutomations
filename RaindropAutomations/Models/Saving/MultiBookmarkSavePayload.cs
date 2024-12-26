using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RaindropAutomations.Models.Saving
{
    public class MultiBookmarkSavePayload
    {
        [JsonPropertyName("result")]
        public bool Result { get; set; }

        [JsonPropertyName("items")]
        public List<BookmarkSaveModel> Bookmarks { get; set; }
    }
}
