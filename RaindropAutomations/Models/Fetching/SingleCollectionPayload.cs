using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace RaindropAutomations.Models.Fetching
{
    public class SingleCollectionPayload
    {
        [JsonPropertyName("result")]
        public bool Result { get; set; }

        [JsonPropertyName("item")]
        public CollectionFetchModel Item { get; set; }
    }
}