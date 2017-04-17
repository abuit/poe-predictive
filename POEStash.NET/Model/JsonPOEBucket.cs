using Newtonsoft.Json;

namespace POEStash.Model
{
    [JsonObject]
    public class JsonPOEBucket
    {
        [JsonProperty("next_change_id")]
        public string NextChangeID { get; set; }

        [JsonProperty("stashes")]
        public JsonPOEStash[] Stashes { get; set; }

        public JsonPOEBucket() { }
    }
}
