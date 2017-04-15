using Newtonsoft.Json;

namespace POEStash
{
    [JsonObject]
    public class Socket
    {
        [JsonProperty("group")]
        public int Group { get; set; }
        [JsonProperty("attr")]
        public string Attribute { get; set; }
    }
}
