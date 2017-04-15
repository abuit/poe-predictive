using Newtonsoft.Json;

namespace POEStash
{
    [JsonObject]
    public class Stash
    {
        [JsonProperty("accountName")]
        public string AccountName       { get; set; }

        [JsonProperty("lastCharacterName")]
        public string LastCharacterName { get; set; }

        [JsonProperty("id")]
        public string ID                { get; set; }

        [JsonProperty("stash")]
        public string Name              { get; set; }

        [JsonProperty("stashType")]
        public string StashType         { get; set; }

        [JsonProperty("items")]
        public Item[] Items             { get; set; }

        [JsonProperty("public")]
        public bool IsPublic            { get; set; }

        public Currency Price
        {
            get
            {
                return Currency.Parse(Name);
            }
        }

        public Stash() { }
    }
}
