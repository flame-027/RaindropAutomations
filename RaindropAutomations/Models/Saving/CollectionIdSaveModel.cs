using Newtonsoft.Json;

namespace RaindropAutomations.Models.Saving
{
    public class CollectionIdSaveModel
    {
        [JsonProperty("$id")]
        public int Id { get; set; }
    }

}
