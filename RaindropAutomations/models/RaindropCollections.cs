using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace RaindropAutomations.models
{
    public class RaindropCollections
    {
        [JsonPropertyName("result")]
        public bool Result { get; set; }

        [JsonPropertyName("items")]
        public List<RaindropCollectionItem> Items { get; set; }
    }
}