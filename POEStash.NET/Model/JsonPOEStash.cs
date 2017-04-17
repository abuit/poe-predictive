using System.Collections;
using System.Collections.Generic;
using LiteDB;
using Newtonsoft.Json;

namespace POEStash.Model
{
    [JsonObject]
    public class JsonPOEStash
    {
        [JsonProperty("accountName")]
        public string AccountName { get; set; }

        [JsonProperty("lastCharacterName")]
        public string LastCharacterName { get; set; }

        [JsonProperty("id"), BsonIndex(true)]
        public string Id { get; set; }

        [JsonProperty("stash")]
        public string Name { get; set; }

        [JsonProperty("stashType")]
        public string StashType { get; set; }

        [JsonProperty("items")]
        public JsonPOEItem[] Items { get; set; }

        [JsonProperty("public")]
        public bool IsPublic { get; set; }

        [BsonIgnore]
        public Currency.Currency Price => Currency.Currency.Parse(Name);

        public JsonPOEStash() { }
    }
}
