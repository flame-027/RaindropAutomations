using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace RaindropAutomations.Models.Fetching
{
    public class MultiCollectionPayload
    {
        [JsonPropertyName("result")]
        public bool Result { get; set; }

        [JsonPropertyName("items")]
        public List<CollectionFetchModel> Items { get; set; } = [];
    }
}