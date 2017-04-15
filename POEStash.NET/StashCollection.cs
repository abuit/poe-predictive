using Newtonsoft.Json;

namespace POEStash
{
    [JsonObject]
    public class StashCollection
    {
        [JsonProperty("next_change_id")]
        public string NextChangeID { get; set; }

        [JsonProperty("stashes")]
        public Stash[] Stashes { get; set; }

        public StashCollection() { }
    }
}
