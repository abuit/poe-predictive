using Newtonsoft.Json;
using POEStash.Model.Converters;

namespace POEStash.Model
{
    [JsonObject]
    public class JsonPOEProperty
    {
        [JsonProperty("name")]
        public string Name      { get; set; }
        [JsonProperty("displayMode")]
        public int DisplayMode  { get; set; }
        [JsonProperty("type")]
        public int Type         { get; set; }
        [JsonProperty("values"), JsonConverter(typeof(PropertyValueConverter))]
        public JsonPOEPropertyValue Values { get; set; }
        [JsonProperty("progress"), JsonConverter(typeof(JsonScientificConverter))]
        public int Progress     { get; set; }
    }
}

