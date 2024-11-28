using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace RaindropAutomations.models
{
    public class RaindropSingleCollection
    {
        [JsonPropertyName("result")]
        public bool Result { get; set; }

        [JsonPropertyName("item")]
        public RaindropCollectionItem Item { get; set; }
    }
}