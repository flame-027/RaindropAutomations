using System.Collections.Generic;
using System.Text.Json.Serialization;

public class RaindropPageResponse
{
    [JsonPropertyName("items")]
    public List<RaindropItem> Items { get; set; }

    [JsonPropertyName("result")]
    public bool Result { get; set; }

    [JsonPropertyName("count")]
    public int Count { get; set; }
}

public class RaindropItem
{
    [JsonPropertyName("_id")]
    public long Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("link")]
    public string Link { get; set; }

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; }
}
