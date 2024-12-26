using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace RaindropAutomations.Models.Saving
{
    public class CollectionIdSaveModel
    {
        [JsonPropertyName("$id")]
        public long Id { get; set; }
    }

}
