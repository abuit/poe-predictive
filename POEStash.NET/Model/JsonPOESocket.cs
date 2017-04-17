using Newtonsoft.Json;

namespace POEStash.Model
{
    [JsonObject]
    public class JsonPOESocket
    {
        [JsonProperty("group")]
        public int Group { get; set; }
        [JsonProperty("attr")]
        public string Attribute { get; set; }
    }
}
